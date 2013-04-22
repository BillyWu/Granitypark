using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.ServiceProcess;

namespace winFileSrv
{
    [RunInstaller(true)]
    public partial class InstallerSrv : Installer
    {
        public InstallerSrv()
        {
            string logname = "GranityLog";

            InitializeComponent();
            //安装日志
            if (EventLog.Exists(logname))
                EventLog.Delete(logname);
            if (EventLog.SourceExists(logname))
                EventLog.DeleteEventSource(logname);
            EventLogInstaller log = new EventLogInstaller();
            log.Source = logname;
            log.Log = logname;
            this.Installers.Add(log);

            //安装服务
            ServiceProcessInstaller prsInstaller = new ServiceProcessInstaller();
            ServiceInstaller srvInstall = new ServiceInstaller();

            // The services run under the system account.
            prsInstaller.Account = ServiceAccount.LocalSystem;
            prsInstaller.Username = null;
            prsInstaller.Password = null;

            // The services are started manually.
            srvInstall.StartType = ServiceStartMode.Automatic;
            srvInstall.ServiceName = "Granity文件服务";
            srvInstall.Description = "上海克立司帝停车场抓拍图片文件服务";

            // Add installers to collection. Order is not important.
            Installers.Add(srvInstall);
            Installers.Add(prsInstaller);
        }
    }
}