using Hishop.OC.RegService;
using HiShop.OC.ThriftS.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Hishop.OC.RegisterWinServer
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                var server = new ThriftSServer();
                int port = int.Parse(System.Configuration.ConfigurationManager.AppSettings["RegServerPort"]);
                server.Start(false, port);
                Log.Debug(string.Format("{0}注册机服务已经启动,端口{1}", DateTime.Now, port));
            }
            catch (Exception ex)
            {
                Console.WriteLine("服务启动失败");
                //Console.WriteLine(ex);
                Log.Error(ex.Message);
            }
        }

        protected override void OnStop()
        {
            Log.Debug("服务Stop");
        }
    }
}
