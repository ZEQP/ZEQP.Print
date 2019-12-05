using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace ZEQP.Print.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            var rc = HostFactory.Run(x =>
            {
                x.SetServiceName("ZEQPPrintService");
                x.SetDisplayName("ZEQPPrintService");
                x.SetDescription("中南智能打印服务");
                x.Service<HttpPrintService>();
                x.StartAutomaticallyDelayed();
                x.RunAsLocalSystem();
                x.UseNLog();
                x.EnableShutdown();
                x.OnException((ex) => {
                    Console.WriteLine(ex.Message);
                });
            });
            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode()); //11
            Environment.ExitCode = exitCode;
        }
    }
}
