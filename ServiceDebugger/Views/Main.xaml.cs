using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace ServiceDebugger.Views
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
                    , "Service Debugger"
                    , "Service Debugger is running.\nClick the icon to restore"
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

        private async void frmMain_Loaded(object sender, RoutedEventArgs e)
        {
            if (StartMinimized)
                SwitchWindowState();

            await StartServices();
        }

        private Task StartServices()
        {
            if (!(Services?.Any() ?? false))
                return Task.FromResult(false); ;

            spServices.Children.Clear();

            var tasks = new List<Task>();
            foreach (ServiceBase service in Services)
            {
                var view = new ServiceView();
                view.Service = service;
                spServices.Children.Add(view);

                if (AutoStart)
                    tasks.Add(view.Start());
            }

            return Task.WhenAll(tasks);

        }

        private void frmMain_MouseEnter(object sender, MouseEventArgs mouseEventArgs)
        {
            BeginAnimation(OpacityProperty, null);
            Opacity = 1.0;
        }

        private void frmMain_MouseLeave(object sender, MouseEventArgs mouseEventArgs)
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
            MessageBox.Show(@"<configuration>
	<appSettings>
        <!-- Starts all services immediately -->
		<add key=""ServiceDebugger.AutoStart"" value=""true""/>
        
        <!-- Start's even if there is no debugger attached to the process -->
		<add key=""ServiceDebugger.RunEvenIfNotAttached"" value=""true""/>

        <!-- Will start minimized -->
		<add key=""ServiceDebugger.StartMinimized"" value=""true""/>
	</appSettings>
</configuration>

Ctrl+C to Copy");

        }

        private void LogButtonClick(object sender, RoutedEventArgs e)
        {
            var window = new ConsoleLogWindow();
            window.Show();
        }
    }
}