using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Data.SqlClient;
using System.Collections.Specialized;
using Microsoft.ApplicationBlocks.ExceptionManagement;
using System.Windows.Forms;
using Estar.Common.Tools;
using System.Configuration;
using System.Xml;

namespace Granity.granityMgr
{
    [RunInstaller(true)]
    public partial class installerDB : Installer
    {
        public installerDB()
        {
            InitializeComponent();
        }
        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.Install(stateSaver);
            StringDictionary ps = this.Context.Parameters;
            string server = ps["server"];
            string dbname = ps["dbname"];
            string user = ps["user"];
            string psw = ps["psw"];
            string path = ps["targetdir"];
            string cnf = "server={0};database={1};user id={2};password={3};";
            string cnn = string.Format(cnf, server,"master", user, psw);
            try
            {
                CreateDataBase(cnn, dbname, path + @"db\SmartCard.mdf", path + @"db\SmartCard.ldf");
                CreateDataBase(cnn, "SmartCardsys", path + @"db\SmartCardsys.mdf", path + @"db\SmartCardsys.ldf");
            }
            catch (Exception ex)
            {
                NameValueCollection data = new NameValueCollection();
                data.Add("参数", cnn + "path=" + path);
                ExceptionManager.Publish(ex, data);
                MessageBox.Show("创建数据库失败，请检查数据库是否冲突或用户密码是否正确！", "安装数据库", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            cnn = string.Format(cnf, server, dbname, user, psw);
            cnn += "Min Pool Size=10;Connection Lifetime=240;Connection Timeout=120;";
            XmlDocument xmldoc = new XmlDocument();
            try
            {
                string cfgfile = path + "Granity.granityMgr.exe.config";
                xmldoc.Load(cfgfile);
                XmlElement xe = xmldoc.SelectSingleNode("//DataSource/add[@name='default']") as XmlElement;
                if (null != xe)
                {
                    xe.SetAttribute("value", cnn);
                    xmldoc.Save(cfgfile);
                }
            }
            catch (Exception ex)
            {
                NameValueCollection data = new NameValueCollection();
                data.Add("参数", path + "Granity.granityMgr.exe.config");
                ExceptionManager.Publish(ex, data);
                MessageBox.Show("更改配置失败，请检查配置文件！", "设置配置", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateDataBase(string cnn, string dbname, string mdf, string ldf)
        {
            String str = "EXEC sp_attach_db @dbname = '{0}', @filename1 = '{1}',@filename2='{2}'";
            SqlConnection myConn = new SqlConnection(cnn);
            str = string.Format(str, dbname, mdf, ldf);
            SqlCommand myCommand = new SqlCommand(str, myConn);
            try
            {
                myConn.Open();
                myCommand.ExecuteNonQuery();
            }
            finally
            {
                myConn.Close();
            }

        }
    }
}