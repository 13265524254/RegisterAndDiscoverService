using HiShop.OC.ThriftS.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiShop.OC.ThriftS;
using HiShop.OC.Strategy.Cache;

namespace Hishop.OC.RegService
{
    public class RegAndDiscoverService : IRegisterService
    {
        static string serverIp = System.Configuration.ConfigurationManager.AppSettings["serverIp"];
        static Dictionary<string, ServiceData> dicService;
        static object lockobj = new object();
        static readonly string key = System.Configuration.ConfigurationManager.AppSettings["serverKey"]; //"reSer";

        static RegAndDiscoverService() {
            lock (lockobj)
            {
                InitializationServiceData();
            }
        }

        /// <summary>
        /// 从Json文件中还原服务节点数据
        /// </summary>
        private static void InitializationServiceData()
        {
            if (Cache.Exists(key))
                dicService = Cache.Get<Dictionary<string, ServiceData>>(key);
            else
                dicService = new Dictionary<string, ServiceData>();
            Log.Debug("共加载服务项" + dicService.Keys.Count);
            //    FileStream fs = null;
            //    try
            //    {
            //        fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "ServiceData.json", FileMode.OpenOrCreate);
            //        byte[] byt = new byte[fs.Length];
            //        fs.Read(byt, 0, byt.Length);
            //        dicService = JsonConvert.DeserializeObject<Dictionary<string, ServiceData>>(Encoding.UTF8.GetString(byt));
            //    }
            //    catch (Exception ex)
            //    {
            //        dicService = new Dictionary<string, ServiceData>();
            //        Console.WriteLine("加载json文件错误{0}",ex);
            //        Log.Error(string.Format("加载json文件错误{0}", ex));
            //    }
            //    finally
            //    {
            //        if (dicService == null)
            //            dicService = new Dictionary<string, ServiceData>();
            //        if (fs != null)
            //        {
            //            fs.Dispose();
            //            fs.Close();
            //        }
            //    }


        }

        /// <summary>
        /// release模式下，如果没有，则返回debug
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ServiceData DiscoverService(string name)
        {
            //Log.Debug(string.Format("DiscoverService请求服务{0}", name));
            if (!dicService.ContainsKey(name))
                name = BuildDebugServiceName(name);//是否有debug服务
            if (dicService.ContainsKey(name))
            {
                //Console.WriteLine("返回服务节点{0},IP>{1}:{2}", dicService[name], dicService[name].Ip, dicService[name].Port);
                //Log.Debug(string.Format("返回realease服务节点{0},IP>{1}:{2}", dicService[name], dicService[name].Ip, dicService[name].Port));

                return dicService[name];
            }
            else
            {
                //Console.WriteLine("无服务节点{0}", name);
                //Log.Debug(string.Format("无服务节点{0},", name));
                return null;
            }
              
        }

        private static string BuildDebugServiceName(string servicename)
        {
            string IP = HiShop.OC.ThriftS.Service.ThriftSContext.Current.Request.ClientIP;
            if (IP == "127.0.0.1" && !string.IsNullOrEmpty(serverIp))
                IP = serverIp;
            string serviceName = string.Format("{0}_{1}", IP, servicename);
            return serviceName;
        }

        public bool RegisterService(string name, int port)
        {
            //Console.WriteLine("{1} 注册服务{0}", name, DateTime.Now);
            //Log.Info(string.Format("{1} 注册服务{0}", name, DateTime.Now));
            var IP = HiShop.OC.ThriftS.Service.ThriftSContext.Current.Request.ClientIP;
            return RegThriftSService(name, IP, port);

        }

        private static bool RegThriftSService(string name,string ip, int port)
        {
            try
            {
                Log.Debug("RegThriftSService" + name);

                if (ip == "127.0.0.1"&&!string.IsNullOrEmpty(serverIp))
                    ip = serverIp;
                ServiceData serviceData = new ServiceData()
                {
                    Ip = ip,
                    Port = port,
                    ServiceName = name,
                    UpdateTime = DateTime.Now
                };
                if (dicService.ContainsKey(name))
                {
                    dicService[name] = serviceData;
                    SaveServiceJson();
                    Log.Debug("更新服务》" + name);
                }
                else
                {
                    lock (lockobj)
                    {
                        if (!dicService.ContainsKey(name))
                        {
                            //Log.Debug("服务不存在" + name);
                            dicService.Add(name, serviceData);
                            SaveServiceJson();
                        }
                    }

                }
                //Console.WriteLine("{0}服务注册成功", name);
                Log.Info(string.Format("{0}服务注册成功", name));
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("{0}服务注册失败：{1}", name, ex.Message));
                return false;
            }
            return true;
        }

        /// <summary>
        /// 将服务节点保存至Jason文件中
        /// </summary>
        private static void SaveServiceJson()
        {
            Cache.Remove(key);
            Cache.Insert(key, dicService);
            //    Log.Debug("SaveServiceJson");

            //    FileStream fs = null;
            //    try
            //    {
            //        fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "ServiceData.json", FileMode.Open);
            //        var dataJson = JsonConvert.SerializeObject(dicService);
            //        byte[] byt = System.Text.Encoding.UTF8.GetBytes(dataJson);
            //        fs.Write(byt, 0, byt.Length);
            //    }
            //    catch (Exception ex)
            //    {
            //        Log.Error(string.Format("保存json文件出错{0}", ex));
            //        Console.WriteLine("保存json文件出错{0}", ex);
            //    }
            //    finally
            //    {
            //        if (fs != null)
            //        {
            //            fs.Dispose();
            //            fs.Close();
            //        }
            //    }

        }

        public bool RegisterServiceWithIP(string name, string host, int port)
        {

            //Console.WriteLine("{1} 注册服务{0}", name, DateTime.Now);
            return RegThriftSService(name, host, port);
        }

      
        public Dictionary<string, ServiceData> UpdateService(List<string> names)
        {
            Dictionary<string, ServiceData> result = new Dictionary<string, ServiceData>();
            try
            {
                result = dicService.Where(t => names.Contains(t.Key)).Select(s => s).ToDictionary(s => s.Key, s => s.Value);
                //foreach (var key in Cache.GetKeys("*"))
                //{
                //    if (names.Contains(key))
                //    {
                //        result.Add(key, Cache.Get<ServiceData>(key));
                //    }
                //}
                Log.Info(string.Format("检测以下服务{0}", names.Aggregate((t, n) => t + "、" + n)));
            }
            catch (Exception ex)
            {

                Log.Error("UpdateService error:" + ex.Message);
            }
            return result;
        }

        
        /// <summary>
        /// debug模式下 更新服务
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public Dictionary<string, ServiceData> UpdateDebugService(List<string> names)
        {
            Dictionary<string, ServiceData> result = new Dictionary<string, ServiceData>();
            try
            {
                foreach (var serviceName in names)
                {
                    var serviceData = DiscoverDebugService(serviceName);
                    if(serviceData!=null)
                    result.Add(serviceName, serviceData);
                }
                //result = dicService.Where(t => names.Contains(t.Key)).Select(s => s).ToDictionary(s => s.Key, s => s.Value);
                //Log.Info(string.Format("检测以下服务{0}", names.Aggregate((t, n) => t + "、" + n)));
            }
            catch (Exception ex)
            {

                Log.Error("UpdateService error:" + ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 注册debu模式下的服务
        /// </summary>
        /// <param name="name"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool RegisterDebugService(string name, int port)
        {
            //Console.WriteLine("{1} Debug模式下注册服务{0}", name, DateTime.Now);
            //Log.Debug(string.Format("{1} Debug模式下注册服务{0}", name, DateTime.Now));
            //服务名称默认增加IP前缀
            string IP = HiShop.OC.ThriftS.Service.ThriftSContext.Current.Request.ClientIP;
            string serviceName = string.Format("{0}_{1}", IP, name);
            return RegThriftSService(serviceName,IP,port);
        }

        public ServiceData DiscoverDebugService(string name)
        {

            //Log.Debug(string.Format("DiscoverDebugService请求服务{0}", name));
            //优先查找debug模式下的服务,r如果没有，则查找正常注册的服务
            string serviceName = BuildDebugServiceName(name);

            //Log.Debug(string.Format("DiscoverDebugService请求服务{0}", serviceName));
            //Console.WriteLine("发现Debug服务{0}", serviceName);
            if (dicService.ContainsKey(serviceName))
            {
                //Console.WriteLine("返回debug服务节点{0},IP>{1}:{2}", dicService[serviceName],dicService[serviceName].Ip, dicService[serviceName].Port);
                //Log.Debug(string.Format("返回debug服务节点{0},IP>{1}:{2}", dicService[serviceName].ServiceName, dicService[serviceName].Ip, dicService[serviceName].Port));
                return dicService[serviceName];
            }
            if (dicService.ContainsKey(name))
            {
                //Console.WriteLine("返回服务节点{0},IP>{1}:{2}", dicService[name], dicService[serviceName].Ip, dicService[serviceName].Port);
                //Log.Debug(string.Format("返回debug服务节点{0},IP>{1}:{2}", dicService[name].ServiceName, dicService[name].Ip, dicService[name].Port));
                return dicService[name];
            }
            else {
                //Console.WriteLine("无服务节点{0}", name);
                //Log.Error(string.Format("无服务节点{0}", name));
                return null;
            }

        }

      
    }
}
