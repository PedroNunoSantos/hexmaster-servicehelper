using System;
using System.ComponentModel;
using System.Reflection;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows;
using ServiceDebugger.Types;
using ServiceDebugger.Wpf;

namespace ServiceDebugger.Views
{
    public sealed partial class ServiceView : INotifyPropertyChanged
    {
        public ServiceView()
        {
            InitializeComponent();
        }

        private ServiceBase _service;
        private ServiceStatus _status = ServiceStatus.Stopped;

        public ServiceBase Service
        {
            get { return _service; }
            set
            {
                _service = value;
                lblServiceName.Text = _service.ServiceName;
                btnPause.IsEnabled = false;
                btnStop.IsEnabled = false;
            }
        }

        public ServiceStatus Status
        {
            get { return _status; }
            private set
            {
                if (value == _status) return;
                _status = value;
                OnPropertyChanged();
            }
        }

        private bool InvokeServiceMethod(ServiceCommands command)
        {
            Type serviceBaseType = _service.GetType();
            object[] parameters = null;
            if (command == ServiceCommands.Start)
                parameters = new object[] { null };

            string method;
            switch (command)
            {
                case ServiceCommands.Stop:
                    method = "OnStop";
                    break;
                case ServiceCommands.Pause:
                    method = "OnPause";
                    break;
                case ServiceCommands.Start:
                    method = "OnStart";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(command), command, null);
            }

            try
            {
                serviceBaseType.InvokeMember(method,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null,
                    _service, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"An exception was thrown while trying to call the {method} of the {_service.ServiceName} service. Examine the inner exception for more information.", ex.InnerException);
            }
            return true;
        }

        public async void btnPlay_Click(object sender, RoutedEventArgs e) => await Start();
        private async void btnPause_Click(object sender, RoutedEventArgs e) => await Pause();
        private async void btnStop_Click(object sender, RoutedEventArgs e) => await Stop();

        public async Task Start()
        {

            IsEnabled = false;
            bool isDone = await Task.Run(() => InvokeServiceMethod(ServiceCommands.Start));
            Status = ServiceStatus.Running;
            IsEnabled = true;

            if (!isDone) return;
            btnStop.IsEnabled = _service.CanStop;
            btnPause.IsEnabled = _service.CanPauseAndContinue;
            btnPlay.IsEnabled = false;

        }

        private async Task Pause()
        {
            IsEnabled = false;
            bool isDone = await Task.Run(() => InvokeServiceMethod(ServiceCommands.Pause));
            Status = ServiceStatus.Paused;
            IsEnabled = true;

            if (!isDone) return;
            btnStop.IsEnabled = _service.CanStop;
            btnPause.IsEnabled = false;
            btnPlay.IsEnabled = true;
        }

        private async Task Stop()
        {
            IsEnabled = false;
            bool isDone = await Task.Run(() => InvokeServiceMethod(ServiceCommands.Stop));
            Status = ServiceStatus.Stopped;
            IsEnabled = true;

            if (!isDone) return;

            btnStop.IsEnabled = false;
            btnPause.IsEnabled = false;
            btnPlay.IsEnabled = true;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [TypeConverter(typeof(StatusToBrushConverter))]
    public enum ServiceStatus
    {
        Stopped, Running, Paused
    }
}
