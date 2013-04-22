using System;
using System.Web;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.ApplicationBlocks.ExceptionManagement;
using Estar.Common.Tools.Bits;


namespace Estar.Common.Tools
{
    /// <summary>
    /// 利用windows的后台智能传输服务,实现基于Http协议的文件交换,并利用系统来保证断点续传
    /// </summary>
    public class BitsDownFile
    {
        static private int _maxFiles = 20;

        /// <summary>
        /// 读取或设置并行传输文件的最大数
        /// </summary>
        static public int MaxFiles
        {
            get { return _maxFiles; }
            set { _maxFiles = value; }
        }
        /// <summary>
        /// 传输文件,多线程传输文件,默认最大20个;
        /// </summary>
        static public void TransFile()
        {
            BitsFileList.MoveFirst();
            int i = 0, imax = _maxFiles*100;
            XmlNode nodeFile = null;
            while (!BitsFileList.IsEOF() && i<imax )
            {
                i++;
                nodeFile = BitsFileList.GetCurrentFile();
                if (null == nodeFile)
                    break;
                string strState = ((XmlElement)nodeFile).GetAttribute("state");
                if ("moving" == strState)
                    continue;
                if ("doing" == strState)
                {
                    nodeFile.Attributes.RemoveNamedItem("state");
                    BitsFileList.MoveNext();
                    continue;
                }
                ((XmlElement)nodeFile).SetAttribute("state", "moving");
                ThreadPool.QueueUserWorkItem(new WaitCallback(HandleBits));
            }
            if (null != nodeFile)
                nodeFile.Attributes.RemoveNamedItem("state");
            BitsFileList.MoveFirst(); 
            i = 0;
            while (!BitsFileList.IsEOF() && i < imax)
            {
                i++;
                nodeFile = BitsFileList.GetCurrentFile();
                if(null!=nodeFile)
                    nodeFile.Attributes.RemoveNamedItem("state");
                BitsFileList.MoveNext();
            }
            BitsFileList.SaveFile();
        }

        /// <summary>
        /// 检查/启动/结束传输作业进程,参数文件节点为空的取当前文件节点
        /// </summary>
        /// <param name="nodeFile">要管理的传输文件节点</param>
        static private void HandleBits(Object o)
        {
            IBackgroundCopyManager bcm = null;
            IBackgroundCopyJob job = null;
            XmlNode nodeFile = null;
            nodeFile = BitsFileList.GetCurrentFile();
            if (null == nodeFile)
                return;
            ((XmlElement)nodeFile).SetAttribute("state", "doing");
            try
            {
                // Create BITS object
                bcm = (IBackgroundCopyManager)new BackgroundCopyManager();
                Guid jobID = Guid.Empty;
                if (null != nodeFile.Attributes["jobguid"] && !string.IsNullOrEmpty(nodeFile.Attributes["jobguid"].Value))
                { // Do we already have a job in place?
                    jobID = new Guid(nodeFile.Attributes["jobguid"].Value);
                    BG_JOB_STATE state;
                    try
                    {
                        bcm.GetJob(ref jobID, out job); // Get the BITS job object
                        job.GetState(out state);        // check its state
                        switch (state)
                        {
                            case BG_JOB_STATE.BG_JOB_STATE_ERROR: // If it is an error, re-poll
                                job.Complete();
                                nodeFile.Attributes.RemoveNamedItem("jobguid");
                                Marshal.ReleaseComObject(job);
                                job = null;
                                break;
                            case BG_JOB_STATE.BG_JOB_STATE_CANCELLED:
                            case BG_JOB_STATE.BG_JOB_STATE_TRANSFERRED: // If we got the job
                                job.Complete();                          // then complete it
                                nodeFile.ParentNode.RemoveChild(nodeFile);
                                Marshal.ReleaseComObject(job);
                                Marshal.ReleaseComObject(bcm);
                                return;
                            default:
                                Marshal.ReleaseComObject(bcm);
                                return;
                        }
                    }
                    catch (Exception e)
                    {
                        NameValueCollection errInfo = new NameValueCollection();
                        errInfo["文件类别"] = nodeFile.Attributes["doctype"].Value;
                        errInfo["远程文件"] = nodeFile.Attributes["srcurl"].Value;
                        errInfo["作业Guid"] = nodeFile.Attributes["jobguid"].Value;
                        ExceptionManager.Publish(e, errInfo);
                        if (null != (e as UnauthorizedAccessException))
                        {
                            if (job != null) Marshal.ReleaseComObject(job);
                            if (bcm != null) Marshal.ReleaseComObject(bcm);
                            return;
                        }
                        COMException exCOM = e as COMException;
                        if (null != exCOM && exCOM.ErrorCode == unchecked((Int32)0x80200001))
                            nodeFile.Attributes.RemoveNamedItem("jobguid");
                        else
                        {
                            return;
                        }
                    }
                }

                // Create a bits job to download the next expected update
                if (null != nodeFile && (null == nodeFile.Attributes["jobguid"] || string.IsNullOrEmpty(nodeFile.Attributes["jobguid"].Value)))
                {
                    bcm.CreateJob("下载远程文件",
                       BG_JOB_TYPE.BG_JOB_TYPE_DOWNLOAD, out jobID, out job);
                    string doctype = nodeFile.Attributes["doctype"].Value;
                    string srcurl = nodeFile.Attributes["srcurl"].Value;
                    string dest = nodeFile.Attributes["localname"].Value;

                    job.SetDescription("下载文件位置: " + doctype);
                    job.AddFile(srcurl, dest);
                    job.Resume(); // start the job in action
                    ((XmlElement)nodeFile).SetAttribute("jobguid", jobID.ToString());
                }
                if (bcm != null)
                    Marshal.ReleaseComObject(bcm);
                return;
            }
            catch { }
        }
    }

    /// <summary>
    /// 后台智能传输服务的文件列表,静态类,对应于一个存储文件BitsFileList.xml
    /// srcurl:远程文件位置, doctype 存储本地的文件类型(对应本地文件夹), jobguid:传输服务guid, state:内部使用维护列表的同步属性
    /// 文件格式:<FileList><File srcurl="" doctype="" jobguid="" state="new" /></FileList>
    /// </summary>
    static public class BitsFileList
    {
        public  const    string FileName = "BitsFileList.xml";
        private static XmlDocument _xmlDocList = null;
        private static XmlNode _xmlNodeFileC = null; 

        /// <summary>
        /// 构造函数,初始化全局文件列表
        /// </summary>
        static BitsFileList()
        {
            if (null == _xmlDocList)
            {
                _xmlDocList = new XmlDocument();
                string filename = HttpContext.Current.Server.MapPath(FileName);
                if (!File.Exists(filename))
                {
                    _xmlDocList.LoadXml("<?xml version=\"1.0\" encoding=\"gb2312\" ?><FileList></FileList>");
                    _xmlDocList.Save(filename);
                }
                else
                    _xmlDocList.Load(filename);
                XmlNodeList fileList = _xmlDocList.SelectNodes("/*/File[@state='new']");
                if (fileList.Count > 0)
                {
                    foreach (XmlNode nodeFile in fileList)
                        nodeFile.Attributes.RemoveNamedItem("state");
                    _xmlDocList.Save(filename);
                }
            }
        }

        /// <summary>
        /// 添加传输文件,列表已经有的,停止原来的下载并使用新的下载任务
        /// </summary>
        /// <param name="srcurl">文件远程静态url地址</param>
        /// <param name="doctype">存储本地的文件类型</param>
        /// <returns>成功返回true,失败返回false</returns>
        public static bool AddFile(string srcurl, string doctype)
        {
            if (string.IsNullOrEmpty(srcurl))
                return true;
            string XPath = "//File[@srcurl='{0}' and @doctype='{1}']";
            XPath = string.Format(XPath, srcurl, doctype);
            XmlElement xmlElem = _xmlDocList.SelectSingleNode(XPath) as XmlElement;
            if (null != xmlElem)
                return true;
            xmlElem = _xmlDocList.CreateElement("File");
            xmlElem.SetAttribute("srcurl", srcurl);
            xmlElem.SetAttribute("doctype", doctype);
            xmlElem.SetAttribute("state", "new");

            string path = DataAccRes.AppSettings(doctype);
            if (string.IsNullOrEmpty(path))
                path = DataAccRes.AppSettings("DefaultFilePath");
            string dest = HttpContext.Current.Server.MapPath(path + Path.GetFileName(srcurl));
            xmlElem.SetAttribute("localname", dest);

            _xmlDocList.DocumentElement.AppendChild(xmlElem);
            return true;
        }

        /// <summary>
        /// 将更新文件列表保存本地文件中
        /// </summary>
        /// <returns>保存成功返回true,并去掉新添加文件的新标识,否则返回false,并回退加入的节点</returns>
        public static bool SaveFile()
        {
            string filename = HttpContext.Current.Server.MapPath(FileName);
            try
            {
                _xmlDocList.Save(filename);
                XmlNodeList fileList = _xmlDocList.SelectNodes("/*/File[@state='new']");
                foreach (XmlNode nodeFile in fileList)
                    nodeFile.Attributes.RemoveNamedItem("state");
                return true;
            }
            catch
            {
                XmlNodeList fileList = _xmlDocList.SelectNodes("/*/File[@state='new']");
                foreach (XmlNode nodeFile in fileList)
                    nodeFile.ParentNode.RemoveChild(nodeFile);
                return false;
            }
        }

        /// <summary>
        /// 获取当前处理文件节点
        /// </summary>
        /// <returns>当前的文件</returns>
        public static XmlNode GetCurrentFile()
        {
            if (null == _xmlNodeFileC)
                _xmlNodeFileC = _xmlDocList.DocumentElement.FirstChild;
            return _xmlNodeFileC;
        }

        /// <summary>
        /// 移动到第一个文件
        /// </summary>
        public static void MoveFirst()
        {
            _xmlNodeFileC = _xmlDocList.DocumentElement.FirstChild;
        }

        /// <summary>
        /// 当前的文件列表向前移动一个,到结尾,文件就指向空,
        /// </summary>
        public static void MoveNext()
        {
            if (null == _xmlNodeFileC )
                return ;
            _xmlNodeFileC = _xmlNodeFileC.NextSibling;
            if (null == _xmlNodeFileC )
                return;
            if (null != _xmlNodeFileC.Attributes["state"] && "new" == _xmlNodeFileC.Attributes["state"].Value)
                _xmlNodeFileC = null;
        }

        /// <summary>
        /// 是否是结尾
        /// </summary>
        /// <returns>是结尾返回true</returns>
        public static bool IsEOF()
        {
            if (null == _xmlNodeFileC )
                return true;
            else
                return false;
        }

        /// <summary>
        /// 获取下一个文件,并当前文件节点向后移动一个
        /// 如果到了最后就跳到第一个
        /// </summary>
        /// <returns>返回下一个文件的XmlNode</returns>
        public static XmlNode FectchFile()
        {
            if (null == _xmlNodeFileC)
                _xmlNodeFileC = _xmlDocList.DocumentElement.FirstChild;
            if (null == _xmlNodeFileC)
                return null;
            XmlNode nodenext = _xmlNodeFileC.NextSibling;
            if (null == nodenext || (null != nodenext.Attributes["state"] && "new" == nodenext.Attributes["state"].Value))
                nodenext = _xmlDocList.DocumentElement.FirstChild;
            _xmlNodeFileC = nodenext;
            return nodenext;
        }

    }

}
