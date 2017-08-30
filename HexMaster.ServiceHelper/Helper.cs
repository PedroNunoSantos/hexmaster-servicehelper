using HexMaster.ServiceHelper;
using HexMaster.Threading;
using HexMaster.Views;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace HexMaster
{
	public static class Helper
	{
		public static void Run(this IEnumerable<ServiceBase> services)
		{
			if (!Debugger.IsAttached)
			{
				ServiceBase.Run(services.ToArray());
				return;
			}

			Task t = Task.Factory.StartNew(() =>
				{
					var app = new App();
					app.InitializeComponent();
					app.Startup += (o, e) =>
					{
						var win = new Main();
						win.Width = 300;
						win.Height = 200;
						win.Services = services;
						win.Show();
					};
					app.Run();
				},
				CancellationToken.None,
				TaskCreationOptions.PreferFairness,
				new StaticThreadTaskScheduler(25)
			);
			t.Wait();
		}
	}
}
