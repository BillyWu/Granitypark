#region 版本说明

/*
 * 功能内容：   停车场协议应用
 *
 * 作    者：   王荣策
 *
 * 审 查 者：   王荣策
 *
 * 日    期：   2010-07-27
 */

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Granity.communications;
using Estar.Business.DataManager;
using System.Data;
using Estar.Common.Tools;
using ComLib;

namespace Granity.winTools
{
    /// <summary>
    /// 协议命令,约束(a:一个通讯位置只有一套协议在执行;b:一个位置可管理多设备;c:当前时刻只处理一个指令)
    /// </summary>
    public class CmdProtocol:CommandBase
    {
        /// <summary>
        /// 协议数据源
        /// </summary>
        private const string dbSrc = "基础类";
        /// <summary>
        /// 协议参数数据项
        /// </summary>
        private const string dbItem = "协议指令";

        #region 内置类

        /// <summary>
        /// 协议参数
        /// </summary>
        private class PtlParam
        {
            /// <summary>
            /// 是否ASCII
            /// </summary>
            public bool   IsAscii = false;
            /// <summary>
            /// 转义字符集
            /// </summary>
            public string Exchanges = "";
        }

        #endregion

        private QueryDataRes query = new QueryDataRes(dbSrc);
        private static ComClass comparse = new ComClass();

        /// <summary>
        /// 协议参数列表,缓存协议
        /// </summary>
        private static Dictionary<string, PtlParam> PTLParam = new Dictionary<string, PtlParam>();
        /// <summary>
        /// 设备通讯通用信息
        /// </summary>
        private devObj dvParam = new devObj();
        /// <summary>
        /// 请求格式
        /// </summary>
        private string inputParam = "";
        /// <summary>
        /// 响应格式
        /// </summary>
        private string outterParam = "";

        private string responseFormat = "";
        /// <summary>
        /// 读取当前响应格式化后字符串(tag格式化)
        /// </summary>
        public string ResponseFormat
        {
            get { return responseFormat; }
        }

        private static Protocol ptl = new Protocol();
        /// <summary>
        /// 读取通讯协议帧头帧尾及键值位置
        /// </summary>
        public static Protocol PTL
        {
            get
            {
                if (ptl.FrameHeader.Length<1)
                {
                    ptl.FrameHeader = new byte[] { 2 };
                    ptl.FrameFoot = new byte[] { 3 };
                    ptl.KeyIndexStart = 1;
                    ptl.KeyLength = 1;
                }
                return ptl;
            }
        }

        #region 构造函数

        /// <summary>
        /// 默认方式构造实例,不初始化同步事件句柄,无指令ID
        /// </summary>
        public CmdProtocol()
            : base()
        {
            this.IsResposeHandle = this.isResponse;
        }

        /// <summary>
        /// 构造函数指定构造指令ID(指令ID是可更改的)
        /// </summary>
        /// <param name="id">指令ID</param>
        public CmdProtocol(string id)
            : base(id)
        {
            this.IsResposeHandle = this.isResponse;
        }

        /// <summary>
        /// 构造函数,初始化同步事件句柄状态
        /// </summary>
        /// <param name="ewhState">同步事件初始状态</param>
        public CmdProtocol(bool ewhState)
            : base(ewhState)
        {
            this.IsResposeHandle = this.isResponse;
        }

        /// <summary>
        /// 构造函数,初始化指令ID,初始化同步事件句柄状态
        /// </summary>
        /// <param name="id">指令ID,使用中可更改</param>
        /// <param name="ewhState">同步事件初始状态</param>
        public CmdProtocol(string id, bool ewhState)
            : base(id, ewhState)
        {
            this.IsResposeHandle = this.isResponse;
        }

        #endregion

        /// <summary>
        /// 设置设备指令,使用dvID做指令ID
        /// </summary>
        /// <param name="tpl">协议类型</param>
        /// <param name="cmd">指令</param>
        /// <param name="tagdata">数据参数,使用tag标记格式</param>
        public bool setCommand(string tpl,  string cmd, string tagdata)
        {
            if (CmdState.Response == this.CheckState())
                return false;
            Estar.Common.Tools.NameObjectList ps = new Estar.Common.Tools.NameObjectList();
            ps["tpl"] = tpl;
            ps["cmd"] = cmd;
            DataTable tab = query.getTable(dbItem, ps);
            if (null == tab || tab.Rows.Count < 1)
                return false;
            //设备参数
            string pmdevice = "";
            //输入格式参数
            string pminput = "";
            //输出格式参数
            string pmout = "";

            //从数据库中取出的参数转换成tag格式参数
            DataColumnCollection dbcols = tab.Columns;
            if (!dbcols.Contains("type") || !dbcols.Contains("pms"))
                return false;
            foreach (DataRow dr in tab.Rows)
            {
                if (DBNull.Value == dr["type"])
                    continue;
                string pmtype = Convert.ToString(dr["type"]);
                switch(pmtype)
                {
                    case "协议":
                        pmdevice = Convert.ToString(dr["pms"]);
                        break;
                    case "Input":
                        pminput += Convert.ToString(dr["pms"]) + ";";
                        break;
                    case "Output":
                        pmout += Convert.ToString(dr["pms"]) + ";";
                        break;
                }
            }

            if (pmdevice.EndsWith(";")) pmdevice = pmdevice.Substring(0, pmdevice.Length - 1);
            if (pminput.EndsWith(";"))  pminput  = pminput.Substring(0, pminput.Length - 1);
            if (pmout.EndsWith(";"))    pmout    = pmout.Substring(0, pmout.Length - 1);
            pmdevice = pmdevice.Replace(";;", ";");
            this.inputParam  = pminput = pminput.Replace(";;", ";");
            this.outterParam = pmout = pmout.Replace(";;", ";");

            // begin 临时补充,在将来完善协议解析后去掉
            pmdevice = basefun.setvaltag(pmdevice, "devid", basefun.valtag(tagdata, "设备地址"));
            // end
            string cmdstr = comparse.CommandString(pmdevice, pminput, tagdata, ref dvParam);
            IsAscii = dvParam.IsAsc;
            Exchanges = dvParam.Exchanges;
            //转义成字节,使用""""代替原来帧头帧尾的处理
            if (dvParam.IsAsc)
            {
                if (cmdstr.StartsWith("02") && cmdstr.EndsWith("03"))
                    cmdstr = "" + cmdstr.Substring(2, cmdstr.Length - 4) + "";
                return this.setCommand(cmdstr);
            }
            return this.setCommand(cmdstr, true);

        }

        #region 监听响应触发指令响应事件

        /// <summary>
        /// 临时,是否ASCII
        /// </summary>
        private static bool IsAscii = false;
        /// <summary>
        /// 临时,转义字符集
        /// </summary>
        private static string Exchanges;

        /// <summary>
        /// 按照指定格式格式化响应结果
        /// </summary>
        /// <param name="response">响应数据字符串</param>
        /// <returns>返回格式化后的字符串</returns>
        private string FormatResponse(byte[] response)
        {
            if (null == response || response.Length < 1)
                return "";
            string info = CommandBase.Parse(response, !IsAscii);
            //恢复转义字符
            if (Exchanges != "")
            {
                string[] strExs = Exchanges.Split('#');
                for (int i = 0; i < strExs.Length; i++)
                {
                    string[] _a = strExs[i].Split('/');
                    info = comparse.SpecialRestore(info, _a[1], _a[0]);
                }
            }
            string state = "";
            if (info.StartsWith("") && info.EndsWith(""))
                info = info.Replace("", "02").Replace("", "03");
            return comparse.AnalysisEateryResults(info, this.outterParam, dvParam, false, ref state);
        }
        
        /// <summary>
        /// 是否当前指令的结果,验证设备ID和指令
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private bool isResponse(CommandBase cmd, byte[] response)
        {
            if (null == response || response.Length < 2)
                return false;
            byte[] bcmd = cmd.getCommand();
            if (null == bcmd || bcmd.Length < 2)
                return false;
            for (int i = 0; i < 2; i++)
                if (bcmd[i] != response[i])
                    return false;
            return true;
        }

        /// <summary>
        /// 重载触发事件,解析响应格式
        /// </summary>
        /// <param name="arg"></param>
        public override void RaiseResponse(ResponseEventArgs arg)
        {
            if (null == arg) return;
            if (arg.Success)
                this.responseFormat = this.FormatResponse(arg.Response);
            else
                this.responseFormat = "";
            base.RaiseResponse(arg);
        }

        #endregion


        #region 公共静态方法

        /// <summary>
        /// 对同一个target循环执行数据记录关联的指令,执行成功后执行sql更新
        /// </summary>
        /// <param name="drs">指令数据数组</param>
        /// <param name="colmap">字段与指令的映射</param>
        /// <param name="tpl">指令协议类别：停车场/消费/门禁/考勤/卡务中心</param>
        /// <param name="cmd">指令命令名称</param>
        /// <param name="target">通讯目标位置</param>
        /// <param name="deviceID">设备下位机地址</param>
        /// <param name="query">数据更新执行实例</param>
        /// <param name="dbItem">执行更新的数据项</param>
        /// <returns>成功执行返回空,失败返回错误原因</returns>
        public static string ExecuteDataRows(DataRow[] drs, string[,] colmap, string tpl, string cmd, CommiTarget target, string deviceID, QueryDataRes query, string dbItem)
        {
            if (null == drs || drs.Length < 1 || null == colmap || colmap.GetLength(1) < 2 || string.IsNullOrEmpty(tpl) || string.IsNullOrEmpty(cmd))
                return "";
            if (null == target || string.IsNullOrEmpty(deviceID))
                return "";
            //声明一个集合来储存数据
            List<DataRow> drList = new List<DataRow>();
            string msg = "";

            //执行指令
            CmdProtocol cmdP = new CmdProtocol(deviceID, false);
            //循环得到传过来的数据
            foreach (DataRow dr in drs)
            {
                if (null == dr) continue;
                string tagdata = "@设备地址=" + deviceID;
                for (int c = 0; c < colmap.GetLength(0); c++)
                {
                    object val = dr[colmap[c, 1]];
                    if (true.Equals(val)) val = "1";
                    if (false.Equals(val)) val = "0";
                    tagdata = basefun.setvaltag(tagdata, colmap[c, 0], Convert.ToString(val));
                }
                //设置指令
                cmdP.setCommand(tpl, cmd, tagdata);
                cmdP.ResetState();
                //发送指令
                CommiManager.GlobalManager.SendCommand(target, cmdP);
                if (!cmdP.EventWh.WaitOne(2000, false))
                {
                    msg = basefun.setvaltag(tagdata, "{状态}", "通讯超时失败！");
                    break;
                }
                if (string.IsNullOrEmpty(cmdP.ResponseFormat))
                {
                    msg = basefun.setvaltag(tagdata, "{状态}", tagdata + "指令错误！");
                    break;
                }
                drList.Add(dr);
            }
            //更新数据库
            //判断数据源和存储过程是否为空
            if (null == query || string.IsNullOrEmpty(dbItem))
                return msg;
            //得到数据的数量
            Estar.Common.Tools.NameObjectList[] ps = new Estar.Common.Tools.NameObjectList[drList.Count];
            for (int i = 0; i < drList.Count; i++)
                ps[i] = ParamManager.createParam(drList[i]);
            //得到数据与配置的数据已经保持一致，但发现没有执行存储过程。。。
            //执行数据库操作（方法封装在query内）
            bool success = query.ExecuteNonQuery(dbItem, ps, ps, ps);
            
            if (!success)
                msg = basefun.setvaltag(msg, "{状态}", basefun.valtag(msg, "{状态}") + "更新数据库失败！");
            return msg;
        }

        #endregion
    }

}
