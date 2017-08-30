using HexMaster.Types;
using System;
using System.Reflection;
using System.ServiceProcess;
using System.Windows;
using System.Windows.Controls;

namespace HexMaster.Views
{
    /// <summary>
    /// Interaction logic for ServiceView.xaml
    /// </summary>
    public partial class ServiceView : UserControl
    {
        public ServiceView()
        {
            InitializeComponent();
        }

        private ServiceBase _service;
		
        public ServiceBase Service
        {
            get => _service;
	        set
            {
                _service = value;
                lblServiceName.Text = _service.ServiceName;
                btnPause.IsEnabled = false;
                btnStop.IsEnabled = false;
            }
        }

        private bool InvokeServiceMethod(ServiceCommands command)
        {
            bool success;
            Type serviceBaseType = _service.GetType();
            object[] parameters = null;
	        if (command == ServiceCommands.Start)
		        parameters = new object[] {null};

	        string method = "OnStart";
            switch (command)
            {
                case ServiceCommands.Stop:
                    method = "OnStop";
                    break;
                case ServiceCommands.Pause:
                    method = "OnPause";
                    break;
                case ServiceCommands.Start:
                default:
                    method = "OnStart";
                    break;
            }

            try
            {
                serviceBaseType.InvokeMember(method, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, _service, parameters);
                success = true;
            }
            catch (Exception ex)
            {
                throw new Exception(
	                $"An exception was thrown while trying to call the {method} of the {_service.ServiceName} service.  Examine the inner exception for more information.", ex.InnerException);
            }
            return true;
        }


        public void btnPlay_Click(object sender, RoutedEventArgs e)
        {
	        Play();
        }

	    private void btnPause_Click(object sender, RoutedEventArgs e)
	    {
		    Pause();
	    }

	    private void btnStop_Click(object sender, RoutedEventArgs e)
	    {
		    Stop();
	    }

	    public void Play()
	    {
		    if (!InvokeServiceMethod(ServiceCommands.Start)) return;
		    btnStop.IsEnabled = _service.CanStop;
		    btnPause.IsEnabled = _service.CanStop;
		    btnPlay.IsEnabled = false;
	    }

	    private void Pause()
	    {
		    if (!InvokeServiceMethod(ServiceCommands.Pause)) return;
		    btnStop.IsEnabled = _service.CanStop;
		    btnPause.IsEnabled = false;
		    btnPlay.IsEnabled = true;
	    }

	    private void Stop()
	    {
		    if (!InvokeServiceMethod(ServiceCommands.Stop)) return;
		    btnStop.IsEnabled = false;
		    btnPause.IsEnabled = false;
		    btnPlay.IsEnabled = true;
	    }
    }
}
