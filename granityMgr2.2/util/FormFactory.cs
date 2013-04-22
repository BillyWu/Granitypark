#region �汾˵��

/*
 * �������ݣ�   ϵͳ���ܴ��ڹ���
 *
 * ��    �ߣ�   ���ٲ�, ��־��, ������
 *
 * �� �� �ߣ�   
 *
 * ��    �ڣ�   2010-12-27
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
    /// ϵͳ���ܴ��ڹ���
    /// </summary>
    public class FormFactory
    {
        /// <summary>
        /// ���ݵ�Ԫ���ƴ򿪴���
        /// </summary>
        /// <param name="unit">��Ԫ����</param>
        /// <returns></returns>
        public static Form CreateForm(string unit)
        {
            if (string.IsNullOrEmpty(unit))
                return null;
            return null;
        }

        /// <summary>
        /// �ж������Ƿ�ɹ�
        /// </summary>
        /// <param name="configKeyValueName">�����ļ�ֵ�����ƣ�һ������Value����</param>
        /// <param name="appRunName">Ӧ�ó��������</param>
        /// <param name="sqlString">���ݿ������ַ���</param>
        /// <returns>0��ʾ���ܷ��ʣ�1��ʾ�ܹ�����</returns>
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
        /// �ж����������Ƿ��ܹ�����
        /// </summary>
        /// <param name="Constring">�����ַ���</param>
        /// <param name="sqlstring">�������ݿ�ű�</param>
        /// <returns>ʧ�ܷ���0�ɹ�����1</returns>
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
        /// ��ȡ���ݿ�(app.config)�����ļ�
        /// </summary>
        /// <param name="configKeyValue">�����ļ�(config)��ֵ�����ƣ�һ�����Value����</param>
        /// <param name="appRunName">Ӧ�ó��������</param>
        /// <returns>�÷ֺŸ������ַ���</returns>
        public string ReadConfig(string configKeyValueName, string appRunName)
        {
            string configValue = "";
            try
            {
                XmlTextReader reader = new XmlTextReader(Application.StartupPath + " \\" + appRunName + ".exe.config"); // newһ��XMLTextReaderʵ��   
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);
                reader.Close();//�ر�reader����Ȼconfig�ļ��ͱ��ֻ������  
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
        ///дConfig�ļ�(������Ϣ)�����ü�ֵ�Ժ�Ӧ�ó�������
        /// </summary>
        /// <param name="configKeyValue">config�ļ��е�Value������ָ���������ַ���</param>
        /// <param name="appRunName">Ӧ�ó��������</param>
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
        /// �������ļ���д��������Ϣ
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
                //�˴������ļ��ڳ���Ŀ¼��
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
        /// �������ļ���д��������Ϣ
        /// </summary>
        /// <param name="AppKey"></param>
        /// <param name="AppValue"></param>
        /// <param name="appRunName"></param>
        public bool SetValue2(string AppKey, string XT, string appRunName, string ServerIp)
        {
            bool state = true;
            try
            {
                //<add key="������" value="192.168.1.181"/>
                //<add key="SystemDB" value="SmartCardsys"/>
                XmlDocument xDoc = new XmlDocument();
                //�˴������ļ��ڳ���Ŀ¼��
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
                    xElem2.SetAttribute("key", "������");
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
