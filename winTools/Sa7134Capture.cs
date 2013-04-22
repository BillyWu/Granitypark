using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Granity.winTools
{
    /// <summary>
    /// ������Ƶ�������ӿ�
    /// </summary>
    internal class Sa7134Capture
    {
        /// <summary>
        /// ��ʼ����Ƶ������
        /// </summary>
        /// <param name="hWndMain">��ǰ��Ⱦ�Ĵ��ھ��</param>
        /// <param name="bInitVidDev">��Ƶģʽ</param>
        /// <param name="bInitAudDev">�Ƿ���Ƶ</param>
        /// <returns>��ʼ���ɹ�����true,�޿�����������ʧ��</returns>
        [DllImport("Sa7134Capture.dll", EntryPoint = "VCAInitSdk")]
        public extern static bool VCAInitSdk(IntPtr hWndMain, DISPLAYTRANSTYPE bInitVidDev, bool bInitAudDev);
        //��ʼ��ϵͳ��Դ
        // [DllImport("Sa7134Capture.dll", EntryPoint = "VCAInitSdk")]
        // public extern static bool VCAInitSdk(IntPtr hWndMain, bool bInitVidDev, bool bLnitAuDev);

        /// <summary>
        /// ��ȡ��ǰ��Ƶͨ������
        /// </summary>
        /// <returns>����ͨ����,û���򷵻�0</returns>
        [DllImport("Sa7134Capture.dll", EntryPoint = "VCAGetDevNum")]
        public extern static int VCAGetDevNum();

        /// <summary>
        /// ��ָ��ͨ��,������Ӧ��Դ
        /// ϵͳ���Զ����������Ӵ��ڵĴ�С����ʾ��Ƶ. Ԥ�����ڵķֱ���Ϊ�ô��ڵĳߴ��С
        /// </summary>
        /// <param name="dwCard">ͨ����</param>
        /// <param name="hPreviewWnd">��ƵԤ�����ھ��</param>
        /// <returns>�ɹ�����true,ʧ�ܷ���false</returns>
        [DllImport("Sa7134Capture.dll", EntryPoint = "VCAOpenDevice")]
        public extern static bool VCAOpenDevice(Int32 dwCard, IntPtr hPreviewWnd);

        /// <summary>
        /// ������ƵԤ��
        /// </summary>
        /// <param name="dwCard">ͨ����</param>
        /// <returns>�ɹ�����true,ʧ�ܷ���false</returns>
        [DllImport("Sa7134Capture.dll", EntryPoint = "VCAStartVideoPreview")]
        public extern static bool VCAStartVideoPreview(Int32 dwCard);

        /// <summary>
        /// ֹͣ��ƵԤ��
        /// </summary>
        /// <param name="dwCard">ͨ����</param>
        /// <returns>�ɹ�����true,ʧ�ܷ���false</returns>
        [DllImport("Sa7134Capture.dll", EntryPoint = "VCAStopVideoPreview")]
        public extern static bool VCAStopVideoPreview(Int32 dwCard);

        /// <summary>
        /// ������ƵԤ��������ı�����ʾ����ʱ������
        /// </summary>
        /// <param name="dwCard">ͨ����</param>
        /// <param name="hPreviewWnd">Ԥ�����ھ��</param>
        /// <returns>�ɹ�����true,ʧ�ܷ���false</returns>
        [DllImport("Sa7134Capture.dll", EntryPoint = "VCAUpdateVideoPreview")]
        public static extern bool VCAUpdateVideoPreview(Int32 dwCard, IntPtr hPreviewWnd);

        /// <summary>
        /// ����������ݵ���Ӧ�Ļ�����
        /// </summary>
        /// <param name="dwCard">ͨ����</param>
        /// <param name="pDestBuf">������ָ��</param>
        /// <param name="dwWidth">ͼ����</param>
        /// <param name="dwHeight">ͼ��߶�</param>
        /// <returns>�ɹ�����true,ʧ�ܷ���false</returns>
        [DllImport("Sa7134Capture.dll", EntryPoint = "VCASaveBitsToBuf")]
        public static extern bool VCASaveBitsToBuf(Int32 dwCard, IntPtr pDestBuf, ref Int32 dwWidth, ref Int32 dwHeight);

        /// <summary>
        /// �������ΪJPEG�ļ�
        /// </summary>
        /// <param name="dwCard">ͨ����</param>
        /// <param name="lpFileName">�ļ�����·��</param>
        /// <param name="dwQuality">��Ƶ������1��100</param>
        /// <returns>�ɹ�����true,ʧ�ܷ���false</returns>
        [DllImport("Sa7134Capture.dll", EntryPoint = "VCASaveAsJpegFile")]
        public extern static bool VCASaveAsJpegFile(Int32 dwCard, string lpFileName, Int32 dwQuality);

        /// <summary>
        /// �ر�ͨ�����ͷ���Ӧϵͳ��Դ
        /// </summary>
        /// <param name="dwCard">ͨ����</param>
        /// <returns>�ɹ�����true,ʧ�ܷ���false</returns>
        [DllImport("Sa7134Capture.dll", EntryPoint = "VCACloseDevice")]
        public extern static bool VCACloseDevice(Int32 dwCard);
    }
    /// <summary>
    /// ��������Ƶ��ʾģʽ
    /// </summary>
    internal enum DISPLAYTRANSTYPE
    {
        /// <summary>
        /// ����ʾ
        /// </summary>
        NOT_DISPLAY = 0,
        /// <summary>
        /// 
        /// </summary>
        PCI_VIEDOMEMORY = 1,
        /// <summary>
        /// ����ģʽ
        /// </summary>
        PCI_MEMORY_VIDEOMEMORY = 2
    }

}
