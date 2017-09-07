using HexMaster.Types;
using System;
using System.Reflection;
using System.ServiceProcess;
using System.Threading.Tasks;
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
			Type serviceBaseType = _service.GetType();
			object[] parameters = null;
			if (command == ServiceCommands.Start)
				parameters = new object[] { null };

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
				serviceBaseType.InvokeMember(method,
					BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null,
					_service, parameters);
			}
			catch (Exception ex)
			{
				throw new Exception(
					$"An exception was thrown while trying to call the {method} of the {_service.ServiceName} service.  Examine the inner exception for more information.", ex.InnerException);
			}
			return true;
		}


		public void btnPlay_Click(object sender, RoutedEventArgs e) => Play();

		private void btnPause_Click(object sender, RoutedEventArgs e) => Pause();

		private void btnStop_Click(object sender, RoutedEventArgs e) => Stop();

		public void Play()
		{
			IsEnabled = false;
			Task.Run(() => InvokeServiceMethod(ServiceCommands.Start))
				.ContinueWith(t =>
				{
					IsEnabled = true;
					if (!t.Result) return;
					btnStop.IsEnabled = _service.CanStop;
					btnPause.IsEnabled = _service.CanStop;
					btnPlay.IsEnabled = false;
				}, TaskScheduler.FromCurrentSynchronizationContext());
		}

		private void Pause()
		{
			IsEnabled = false;
			Task.Run(() => InvokeServiceMethod(ServiceCommands.Pause))
				.ContinueWith(t =>
				{
					IsEnabled = true;
					if (!t.Result) return;

					btnStop.IsEnabled = _service.CanStop;
					btnPause.IsEnabled = false;
					btnPlay.IsEnabled = true;
				});
		}

		private void Stop()
		{
			IsEnabled = false;
			Task.Run(() => InvokeServiceMethod(ServiceCommands.Start))
				.ContinueWith(t =>
				{
					IsEnabled = true;
					if (!t.Result) return;

					btnStop.IsEnabled = false;
					btnPause.IsEnabled = false;
					btnPlay.IsEnabled = true;
				} );
		}
	}
}
