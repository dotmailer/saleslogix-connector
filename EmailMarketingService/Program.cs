﻿using System.ServiceProcess;

namespace EmailMarketing.SalesLogix
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new EmailMarketingServiceController()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}