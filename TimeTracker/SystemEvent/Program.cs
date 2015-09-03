using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Microsoft.Win32;

namespace SystemEvent
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                log4net.Config.XmlConfigurator.Configure();
                Initialize();
                System.Windows.Forms.Application.Run();
            }
            catch (Exception ex)
            {
                log.Debug(ex.ToString());
            }
        }

        private static void Initialize()
        {
            TimeTracker.CalculateElapsedTime();
        }
    }
}
