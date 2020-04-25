using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Animation;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace ServiceDebugger.Views
{
	/// <summary>
	///     Interaction logic for Main.xaml
	/// </summary>
	public partial class Main
	{
		private NotifyIcon _ni;

		public Main()
		{
			InitializeComponent();
			AddNotifyIcon();

			string AutoStartStr = ConfigurationManager.AppSettings["ServiceDebugger.AutoStart"] ?? "";
			AutoStart = AutoStartStr.ToLower() == "true" || AutoStartStr.ToLower() == "1";

			string startMinimizedStr = ConfigurationManager.AppSettings["ServiceDebugger.StartMinimized"] ?? "";
			StartMinimized = startMinimizedStr.ToLower() == "true" || startMinimizedStr.ToLower() == "1";

		}

		public bool StartMinimized { get; set; }
		public bool AutoStart { get; set; }
		
		public IEnumerable<ServiceBase> Services { get; set; }

		private void AddNotifyIcon()
		{
			_ni = new NotifyIcon();
			_ni.Visible = true;
			_ni.Icon = Properties.Resources.config;
			_ni.Click += (s, a) => SwitchWindowState();
		}

		private void SwitchWindowState()
		{
			if (WindowState == WindowState.Normal || WindowState == WindowState.Maximized)
			{
				Hide();
				WindowState = WindowState.Minimized;

				_ni.ShowBalloonTip(2000
					, "Hex Master Service Helper"
					, "Hex Master is running.\nClick the icon to restore"
					, ToolTipIcon.Info);
			}
			else
			{
				Show();
				WindowState = WindowState.Normal;
			}
		}

		private void ExitButtonClick(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void frmMain_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
				DragMove();
		}

		private async void frmMain_Loaded(object sender, RoutedEventArgs e)
		{
			if (StartMinimized)
				SwitchWindowState();

			await StartServices();
		}

		private Task StartServices()
		{
			if (!(Services?.Any() ?? false)) 
                return Task.FromResult(false);;

			spServices.Children.Clear();

			var tasks = new List<Task>();
			foreach (ServiceBase service in Services)
			{
				var view = new ServiceView();
				view.Service = service;
				spServices.Children.Add(view);

				if (AutoStart)
					tasks.Add(view.Play());
            }

			return Task.WhenAll(tasks);

		}

		private void frmMain_MouseEnter(object sender, MouseEventArgs e)
		{
			BeginAnimation(UIElement.OpacityProperty, null);
			Opacity = 1.0;
		}

		private void frmMain_MouseLeave(object sender, MouseEventArgs e)
		{
			var opacityAnimation = new DoubleAnimation(1.0, ServiceDebugger.Properties.Resources.Opacity, TimeSpan.FromMilliseconds(250));
			BeginAnimation(UIElement.OpacityProperty, opacityAnimation);
		}

		private void MinimizeButtonClick(object sender, RoutedEventArgs e)
		{
			SwitchWindowState();
		}

		private void HelpButtonClick(object sender, RoutedEventArgs e)
		{
			System.Windows.MessageBox.Show(@"<add key=""ServiceDebugger.AutoStart"" value=""true""/>
Starts immediately all services on startup

<add key=""ServiceDebugger.RunEvenIfNotAttached"" value=""true""/>
Runs ServiceDebugger even if debugger is not attached.
Don't forget to remove when you release your software!

<add key=""ServiceDebugger.StartMinimized"" value=""true""/>
Starts ServiceDebugger minimized

Ctrl+C to Copy");

		}
	}
}