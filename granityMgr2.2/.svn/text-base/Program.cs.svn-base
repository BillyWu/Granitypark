using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Estar.Business.UserRight;
using Granity.winTools;
using Granity.granityMgr.ParkMgr;
using Granity.granityMgr.util;

namespace Granity.granityMgr
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            DevExpress.UserSkins.BonusSkins.Register();
            DevExpress.UserSkins.OfficeSkins.Register();
            Application.EnableVisualStyles();
            DevExpress.Skins.SkinManager.EnableFormSkins();
            DevExpress.Skins.SkinManager.EnableMdiFormSkins();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmLogin());
              User user = BindManager.getUser();
             
            if (null != user)
                Application.Run(new FrmStationWatchingII());
        }
    }
}