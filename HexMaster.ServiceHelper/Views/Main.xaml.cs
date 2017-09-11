using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace HexMaster.Views
{
	/// <summary>
	///     Interaction logic for Main.xaml
	/// </summary>
	public partial class Main
	{
		private System.Windows.Forms.NotifyIcon _ni;

		public Main()
		{
			InitializeComponent();
			AddNotifyIcon();

			string AutoStartStr = ConfigurationManager.AppSettings["HexMaster.AutoStart"] ?? "";
			AutoStart = AutoStartStr.ToLower() == "true" || AutoStartStr.ToLower() == "1";

			string startMinimizedStr = ConfigurationManager.AppSettings["HexMaster.StartMinimized"] ?? "";
			StartMinimized = startMinimizedStr.ToLower() == "true" || startMinimizedStr.ToLower() == "1";

		}

		public bool StartMinimized { get; set; }
		public bool AutoStart { get; set; }
		
		public IEnumerable<ServiceBase> Services { get; set; }

		private void AddNotifyIcon()
		{
			_ni = new System.Windows.Forms.NotifyIcon();
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
					, System.Windows.Forms.ToolTipIcon.Info);
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

		private void frmMain_Loaded(object sender, RoutedEventArgs e)
		{
			if (StartMinimized)
				SwitchWindowState();

			StartServices();
		}

		private void StartServices()
		{
			if (!(Services?.Any() ?? false)) return;

			spServices.Children.Clear();
			foreach (ServiceBase service in Services)
			{
				var view = new ServiceView();
				view.Service = service;
				spServices.Children.Add(view);

				if (AutoStart)
					view.Play();
			}
		}

		private void frmMain_MouseEnter(object sender, MouseEventArgs e)
		{
			BeginAnimation(OpacityProperty, null);
			Opacity = 1.0;
		}

		private void frmMain_MouseLeave(object sender, MouseEventArgs e)
		{
			var opacityAnimation = new DoubleAnimation(1.0, Properties.Resources.Opacity, TimeSpan.FromMilliseconds(250));
			BeginAnimation(OpacityProperty, opacityAnimation);
		}

		private void MinimizeButtonClick(object sender, RoutedEventArgs e)
		{
			SwitchWindowState();
		}

		private void HelpButtonClick(object sender, RoutedEventArgs e)
		{
			System.Windows.MessageBox.Show(@"<add key=""HexMaster.AutoStart"" value=""true""/>
Starts immediately all services on startup

<add key=""HexMaster.RunEvenIfNotAttached"" value=""true""/>
Runs HexMaster even if debugger is not attached.
Don't forget to remove when you release your software!

<add key=""HexMaster.StartMinimized"" value=""true""/>
Starts HexMaster minimized

Ctrl+C to Copy");

		}
	}
}