using System.Diagnostics;
using System.ServiceProcess;

namespace HexMaster.TestService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Trace.WriteLine ("Service started");
        }

        protected override void OnStop()
        {
            Trace.WriteLine("Service stopped");
        }
    }
}
