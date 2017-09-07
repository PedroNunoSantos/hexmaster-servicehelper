﻿using System;
using System.ServiceProcess;

namespace HexMaster.TestService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
			ServiceBase[] servicesToRun = { new Service1(), new Service1(), new Service1() };
	        Helper.Run(servicesToRun);
        }
    }
}
