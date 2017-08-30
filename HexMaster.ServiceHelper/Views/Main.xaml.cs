using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Animation;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace HexMaster.Views
{
	/// <summary>
	///     Interaction logic for Main.xaml
	/// </summary>
	public partial class Main
	{
		public Main()
		{
			InitializeComponent();
			SetupNotifyIcon();

			string autoStartStr = ConfigurationManager.AppSettings["HexMaster:AutoStart"] ?? "";
			AutoStart = autoStartStr.ToLower() == "true" || autoStartStr.ToLower() == "1";
		}

		public bool AutoStart { get; set; }

		public IEnumerable<ServiceBase> Services { get; set; }

		private void SetupNotifyIcon()
		{
			var ni = new NotifyIcon();
			ni.Visible = true;
			ni.Icon = Properties.Resources.config;
			ni.Click += (s, a) => SwitchWindowState();
		}

		private void SwitchWindowState()
		{
			if (WindowState == WindowState.Normal || WindowState == WindowState.Maximized)
			{
				Hide();
				WindowState = WindowState.Minimized;
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
	}
}