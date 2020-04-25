using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;

namespace ServiceDebugger.TestService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
			Thread.Sleep(1000);
            Trace.WriteLine ("Service started");

        }

        protected override void OnStop()
        {
	        Thread.Sleep(1000);
			Trace.WriteLine("Service stopped");
        }


        protected override void OnPause()
        {
            Thread.Sleep(1000);
            Trace.WriteLine("Service stopped");
        }
    }
}
