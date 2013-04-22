#region 版本说明

/*
 * 功能内容：   系统功能窗口工厂
 *
 * 作    者：   王荣策, 李志慧, 汪斌龙
 *
 * 审 查 者：   
 *
 * 日    期：   2010-12-27
 */

#endregion


using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Xml;


namespace Granity.granityMgr.util
{
    /// <summary>
    /// 系统功能窗口工厂
    /// </summary>
    public class FormFactory
    {
        /// <summary>
        /// 根据单元名称打开窗口
        /// </summary>
        /// <param name="unit">单元名称</param>
        /// <returns></returns>
        public static Form CreateForm(string unit)
        {
            if (string.IsNullOrEmpty(unit))
                return null;
            return null;
        }

        /// <summary>
        /// 判断连接是否成功
        /// </summary>
        /// <param name="configKeyValueName">配置文件值的名称，一般输入Value即可</param>
        /// <param name="appRunName">应用程序的名称</param>
        /// <param name="sqlString">数据库连接字符串</param>
        /// <returns>0表示不能访问，1表示能够访问</returns>
        public bool ConnectionResult(string configKeyValueName, string appRunName, string sqlString)
        {
            bool ConType = false;
            string value = ReadConfig(configKeyValueName, appRunName);
            if (value != "")
            {
                string[] str = value.Split(';');
                if (str.Length == 8)
                {
                    //string TestStr = "Server=" + str[0].ToString() + ";DataBase=" + str[1].ToString() + ";UID=" + str[2].ToString() + ";PWD=" + str[3].ToString() + "";
                    ConType = openConnectionResult(value, sqlString);
                }
                else if (str.Length == 3 && str[2].ToString() != "")
                {
                    string TestStr = "Server=" + str[0].ToString() + ";DataBase=" + str[1].ToString() + ";UID=" + str[2].ToString() + ";PWD=";
                    ConType = openConnectionResult(TestStr, sqlString);
                }
                else
                {
                    // Data Source=60.188.86.49,8888;Initial Catalog=MyDBA;User ID=sa;pwd="
                    string TestStr = "Data Source=" + str[0].ToString() + ";Initial Catalog=" + str[1].ToString() + ";Integrated Security=True";
                    ConType = openConnectionResult(TestStr, sqlString);
                }

            }
            return ConType;
        }
        /// <summary>
        /// 判断现有连接是否能够访问
        /// </summary>
        /// <param name="Constring">连接字符串</param>
        /// <param name="sqlstring">访问数据库脚本</param>
        /// <returns>失败返回0成功返回1</returns>
        public bool openConnectionResult(string Constring, string sqlstring)
        {
            bool IsEstimate = false;
            try
            {
                SqlConnection con = new SqlConnection(Constring);
                try
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(sqlstring, con);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    IsEstimate = true;
                    string aa = ds.GetXml();
                }
                catch
                {
                    IsEstimate = false;
                    return IsEstimate;
                }
                finally
                {
                    con.Close();
                }
            }
            catch
            {
                IsEstimate = false;
                return IsEstimate;
            }
            return IsEstimate;
        }

        /// <summary>
        /// 读取数据库(app.config)配置文件
        /// </summary>
        /// <param name="configKeyValue">配置文件(config)的值的名称，一般出入Value即可</param>
        /// <param name="appRunName">应用程序的名称</param>
        /// <returns>用分号隔开的字符串</returns>
        public string ReadConfig(string configKeyValueName, string appRunName)
        {
            string configValue = "";
            try
            {
                XmlTextReader reader = new XmlTextReader(Application.StartupPath + " \\" + appRunName + ".exe.config"); // new一个XMLTextReader实例   
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);
                reader.Close();//关闭reader，不然config文件就变成只读的了  
                XmlNode xn1 = doc.SelectNodes("/configuration/CustomSection/DataSource/add").Item(0);
                configValue = xn1.Attributes[configKeyValueName].Value;
            }
            catch
            {
                configValue = "";
            }
            return configValue;
        }
        /// <summary>
        ///写Config文件(配置信息)，配置键值对和应用程序名称
        /// </summary>
        /// <param name="configKeyValue">config文件中的Value，这里指的是连接字符串</param>
        /// <param name="appRunName">应用程序的名称</param>
        public bool WriteConfig(string strServer, string strDatabase, string strUserID, string strPassword, string appRunName, string XT, string serverIP)
        {
            bool state = false;
            string configKeyName = "Add";
            if (strServer != "" && strDatabase != "" && strUserID != "" && strPassword != "" && appRunName != "")
            {
                //value="server=192.168.1.2,1433\sqlexpress;user id=sa;password=sasa;database=jdf12;Min Pool Size=10;Connection Lifetime=240;Connection Timeout=120;"/>

                string value = "server=" + strServer + ";user id=sa" + ";password=" + strPassword + ";database=" + strDatabase + ";Min Pool Size=10;Connection Lifetime=240;Connection Timeout=120;";
                state = SetValue(configKeyName, value, appRunName);

                state = SetValue2(configKeyName, XT, appRunName, serverIP);
            }

            return state;
        }

        /// <summary>
        /// 向配置文件里写入配置信息
        /// </summary>
        /// <param name="AppKey"></param>
        /// <param name="AppValue"></param>
        /// <param name="appRunName"></param>
        public bool SetValue(string AppKey, string AppValue, string appRunName)
        {
            bool state = true;
            try
            {
                XmlDocument xDoc = new XmlDocument();
                //此处配置文件在程序目录下
                string appPath = "\\" + appRunName + ".exe.config";
                xDoc.Load(Application.StartupPath + appPath);
                XmlNode xNode;
                XmlElement xElem1;
                XmlElement xElem2;
                xNode = xDoc.SelectSingleNode("//CustomSection//DataSource");

                xElem1 = (XmlElement)xNode.SelectSingleNode("//add[@key='" + AppKey + "']");
                if (xElem1 != null)
                {
                    xElem1.SetAttribute("value", AppValue);
                }
                else
                {
                    xElem2 = xDoc.CreateElement("add");
                    xElem2.SetAttribute("name", "default");

                    xElem2.SetAttribute("type", "SqlClient");
                    xNode.AppendChild(xElem2);

                    xElem2.SetAttribute("value", AppValue);
                    xNode.AppendChild(xElem2);

                }
                xDoc.Save(Application.StartupPath + "\\" + appRunName + ".exe.config");

            }
            catch
            {
                state = false;
            }
            return state;
        }








        /// <summary>
        /// 向配置文件里写入配置信息
        /// </summary>
        /// <param name="AppKey"></param>
        /// <param name="AppValue"></param>
        /// <param name="appRunName"></param>
        public bool SetValue2(string AppKey, string XT, string appRunName, string ServerIp)
        {
            bool state = true;
            try
            {
                //<add key="服务器" value="192.168.1.181"/>
                //<add key="SystemDB" value="SmartCardsys"/>
                XmlDocument xDoc = new XmlDocument();
                //此处配置文件在程序目录下
                string appPath = "\\" + appRunName + ".exe.config";
                xDoc.Load(Application.StartupPath + appPath);
                XmlNode xNode;
                XmlElement xElem1;
                XmlElement xElem2;
                XmlElement xElem3;
                xNode = xDoc.SelectSingleNode("//appSettings");
                xElem1 = (XmlElement)xNode.SelectSingleNode("//add[@key='" + AppKey + "']");
                if (xElem1 != null)
                {
                    xElem1.SetAttribute("value", "add");
                }
                else
                {
                    xElem2 = xDoc.CreateElement("add");
                    xElem2.SetAttribute("key", "服务器");
                    xElem2.SetAttribute("value", ServerIp);
                    xNode.AppendChild(xElem2);

                    xElem3 = xDoc.CreateElement("add");
                    xElem3.SetAttribute("key", "SystemDB");
                    xElem3.SetAttribute("value", XT);
                    xNode.AppendChild(xElem3);



                }
                xDoc.Save(Application.StartupPath + "\\" + appRunName + ".exe.config");
            }
            catch
            {
                state = false;
            }
            return state;
        }






    }
}
