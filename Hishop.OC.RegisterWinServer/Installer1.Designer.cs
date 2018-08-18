using System.Xml;

namespace Hishop.OC.RegisterWinServer
{
    partial class Installer1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.serviceProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstaller1 = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstaller1
            // 
            this.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstaller1.Password = null;
            this.serviceProcessInstaller1.Username = null;
            // 
            // serviceInstaller1
            // 
            this.serviceInstaller1.ServiceName = Get_ConfigValue("ServiceName");
            this.serviceInstaller1.Description = Get_ConfigValue("Description");
            this.serviceInstaller1.DisplayName = Get_ConfigValue("DisplayName");
            // 
            // Installer1
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller1,
            this.serviceInstaller1});

        }
        private string Get_ConfigValue(string strKeyName)
        {
            string root = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string xmlfile = root.Remove(root.LastIndexOf('\\') + 1) + "ServiceSetting.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlfile);
            XmlNode xn = doc.SelectSingleNode("Settings/" + strKeyName);
            doc = null;
            return xn.InnerText;
        }
        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller1;
        private System.ServiceProcess.ServiceInstaller serviceInstaller1;
    }
}