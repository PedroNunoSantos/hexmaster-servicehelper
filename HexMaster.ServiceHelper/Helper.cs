using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using ServiceDebugger.Threading;
using ServiceDebugger.Views;

namespace ServiceDebugger
{
    public static class Helper
    {
        public static void Run(this IEnumerable<ServiceBase> services)
        {
            string runEvenIfNotAttachedStr = ConfigurationManager.AppSettings["HexMaster.RunEvenIfNotAttached"] ?? "";
            bool runEvenIfNotAttached = runEvenIfNotAttachedStr.ToLower() == "true" || runEvenIfNotAttachedStr.ToLower() == "1";

            if (runEvenIfNotAttached || Debugger.IsAttached)
            {
                Task task = Task.Factory.StartNew(() =>
                    {
                        var app = new App();
                        app.InitializeComponent();
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
                    new StaticThreadTaskScheduler(Environment.ProcessorCount)
                );
                task.Wait();
            }
            else
            {
                ServiceBase.Run(services.ToArray());
            }
        }
    }
}
