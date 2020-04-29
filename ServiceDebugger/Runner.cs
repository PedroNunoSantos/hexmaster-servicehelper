using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using ServiceDebugger.Threading;
using ServiceDebugger.Views;
using ServiceDebugger.Wpf;

namespace ServiceDebugger
{
    public static class Runner
    {
        public static void Run(this IEnumerable<ServiceBase> services)
        {
            string runEvenIfNotAttachedStr = ConfigurationManager.AppSettings["ServiceDebugger.RunEvenIfNotAttached"] ?? "";
            bool runEvenIfNotAttached = runEvenIfNotAttachedStr.ToLower() == "true" || runEvenIfNotAttachedStr.ToLower() == "1";

            //if(!Debugger.IsAttached)
            AttachConsoleLog();

            if (runEvenIfNotAttached || Debugger.IsAttached)
            {
                Task task = Task.Factory.StartNew(() =>
                    {
                        var app = new App();
                        app.InitializeComponent();
                        app.Exit += (o, e) => DettachConsoleLog();
                        app.Startup += (o, e) =>
                        {
                            var mainWindow = new Main();
                            mainWindow.Width = 250;
                            mainWindow.Height = 250;
                            mainWindow.Services = services;
                            mainWindow.Show();
                        };
                        app.Run();

                    },
                    CancellationToken.None,
                    TaskCreationOptions.PreferFairness,
                    new StaTaskScheduler(Environment.ProcessorCount)
                );
                task.Wait();
            }
            else
            {
                ServiceBase.Run(services.ToArray());
            }
        }

        public static ObservableCollection<string> ConsoleLogMessages { get; } = new ObservableCollection<string>();
        private static ConsoleRedirectWriter _consoleRedirect;
        
        private static void AttachConsoleLog()
        {
            _consoleRedirect = new ConsoleRedirectWriter();
            _consoleRedirect.OnWrite += consoleMessage => ConsoleLogMessages.Add(consoleMessage);
        }
        
        private static void DettachConsoleLog()
        {
            _consoleRedirect.Release();
        }

    }
}
