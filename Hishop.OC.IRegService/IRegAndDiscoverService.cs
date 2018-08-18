using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hishop.OmniChannel.ThriftS.Common;
using Hishop.OmniChannel.ThriftS.Client;

namespace Hishop.OmniChannel.IRegServ
{
    interface IRegAndDiscoverService: IRegisterService
    {
        /// <summary>
        /// 发现服务
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ServiceData DiscoverService(string name);
    }
}
