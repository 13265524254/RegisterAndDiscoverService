using HiShop.OC.ThriftS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hishop.OC.ServerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var server = new ThriftSServer();
                server.Start(false, int.Parse(System.Configuration.ConfigurationManager.AppSettings["RegServerPort"]));
                Console.WriteLine("{0}注册机服务已经启动",DateTime.Now);
                Console.ReadLine();

                server.Stop();
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("服务启动失败");
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }
    }
}
