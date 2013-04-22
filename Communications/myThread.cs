#region 版本说明

/*
 * 功能内容：   自定义线程封装
 *
 * 作    者：   王荣策
 *
 * 审 查 者：   王荣策
 *
 * 日    期：   2010-05-23
 */

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Granity.communications
{
    /// <summary>
    /// 扩展线程,强化池的使用效率
    /// </summary>
    internal class myThread
    {
        /// <summary>
        /// 内部封装线程
        /// </summary>
        private Thread innerThead;
        /// <summary>
        /// 线程执行的委托
        /// </summary>
        private WaitCallback callback;
        /// <summary>
        /// 委托参数
        /// </summary>
        private object arg;
        /// <summary>
        /// 最近一次执行时间
        /// </summary>
        private DateTime dtRunned = DateTime.Now;
        /// <summary>
        /// 同步线程,启用调用回调过程的信号
        /// </summary>
        private ManualResetEvent resetwh = new ManualResetEvent(true);

        private bool iswait = true;
        /// <summary>
        /// 读取线程是否正在执行回调过程
        /// </summary>
        public bool IsWait
        {
            get { return iswait; }
        }


        /// <summary>
        /// 启动并执行线程,在忙则返回false忽略执行
        /// </summary>
        /// <param name="cb">线程回调函数</param>
        /// <param name="arg">回调函数的参数</param>
        public bool Start(WaitCallback cb, object arg)
        {
            if (!this.iswait || null == cb)
                return false;
            this.iswait=false;

            this.callback = cb;
            this.arg = arg;
            if (null == this.innerThead)
            {
                this.innerThead = new Thread(new ThreadStart(this.threadProcess));
                this.innerThead.Start();
            }
            else
                this.resetwh.Set();
            return true;
        }

        /// <summary>
        /// 执行线程,使用信号通知机制保持线程缓存
        /// </summary>
        private void threadProcess()
        {
            while (true)
            {
                this.iswait = false;
                this.dtRunned = DateTime.Now;

                //执行线程任务过程
                if (null != this.callback)
                    this.callback(this.arg);
                
                this.iswait = true;
                //等待新任务
                this.callback = null;
                this.arg = null;
                if (this.isEnd)
                {
                    this.resetwh.Set();
                    return;
                }
                this.resetwh.Reset();
                this.resetwh.WaitOne();
            }
        }

        /// <summary>
        /// 判断是否有效,超过一个小时空闲可以确定为无效
        /// </summary>
        /// <returns>线程是否空闲太久,空闲太久可释放</returns>
        public bool IsEffect()
        {
            if (this.iswait && DateTime.Now.AddHours(-1) > this.dtRunned)
                return false;
            return true;
        }

        /// <summary>
        /// 是否结束线程,在线程空闲太久可以关闭结束线程
        /// </summary>
        private bool isEnd = false;
        /// <summary>
        /// 关闭线程,释放相关资源
        /// </summary>
        public void Close()
        {
            this.isEnd = true;
            if (null != this.innerThead)
            {
                this.resetwh.Set();
                Thread.Sleep(10);
                this.innerThead.Abort();
            }
            this.resetwh.Close();
        }
    }

}
