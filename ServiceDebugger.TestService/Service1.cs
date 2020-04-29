using System;
using System.ServiceProcess;
using System.Threading;

namespace ServiceDebugger.TestService
{
    public class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
            CanPauseAndContinue = true;
        }

        protected override void OnStart(string[] args)
        {
            Thread.Sleep(500);
            Console.WriteLine($"{nameof(Service1)} started");

        }

        protected override void OnStop()
        {
            Thread.Sleep(500);
            Console.WriteLine($"{nameof(Service1)} stopped");
        }

        protected override void OnPause()
        {
            Thread.Sleep(500);
            Console.WriteLine($"{nameof(Service1)} paused");
        }
        
        

        protected override void Dispose(bool disposing)
        {
            if (disposing) { }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            ServiceName = "Service 1";
        }
    }
}
