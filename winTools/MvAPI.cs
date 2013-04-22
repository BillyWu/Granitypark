using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Granity.winTools
{
    /// <summary>
    /// ΢����Ƶ�������ӿ�
    /// </summary>
    internal class MvAPI
    {
        /// <summary>
        /// ��ȡͨ������
        /// </summary>
        /// <returns>����ͨ����,û���򷵻�0</returns>
        [DllImport("mvapi.dll", CharSet = CharSet.Auto)]
        public static extern uint MV_GetDeviceNumber();

        /// <summary>
        /// ��ͨ��,����ͨ�����
        /// </summary>
        /// <param name="Index">ͨ����</param>
        /// <param name="bRelese">�Ƿ񷢲��汾SDK</param>
        /// <returns>����ͨ�����,ʧ�ܷ���0</returns>
        [DllImport("mvapi.dll", CharSet = CharSet.Auto)]
        public static extern int MV_OpenDevice(uint Index, bool bRelese);

        /// <summary>
        /// �����豸��SDK�Ĺ�������
        /// </summary>
        /// <param name="hDevice">ͨ�����</param>
        /// <param name="Oper">��������</param>
        /// <param name="Val">����ֵ</param>
        /// <returns>�ɹ�����true,ʧ�ܷ���false</returns>
        [DllImport("mvapi.dll", CharSet = CharSet.Auto)]
        public static extern bool MV_SetDeviceParameter(int hDevice, int Oper, uint Val);

        /// <summary>
        /// ����һ��ͼ������ݣ���pInBuffΪNULLʱʹ���ڲ��Ļ�������
        /// </summary>
        /// <param name="hDevice">ͨ�����</param>
        /// <param name="IsProcess">�����ͼ���Ƿ񾭹�����</param>
        /// <param name="Buffer">�û�ָ���Ļ�������ַ</param>
        /// <param name="Bufflen">����������</param>
        /// <param name="pinfo">���÷��غ�Ϊͼ����Ϣ�����Ľṹ</param>
        /// <returns>���ش���ͼ�����ݵĻ�����ָ��</returns>
        [DllImport("MVAPI.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr MV_CaptureSingle(int hDevice, bool IsProcess, int Buffer, int Bufflen, ref MV_IMAGEINFO pinfo);

        /// <summary>
        /// �����豸, ��ʹ�ɼ�����������/ֹͣ/��ͣ״̬��
        /// </summary>
        /// <param name="hDevice">ͨ�����</param>
        /// <param name="Oper">�豸��״̬(���У�ֹͣ����ͣ����ѯ)</param>
        /// <returns>���ؿ��ĵ�ǰ����״̬</returns>
        [DllImport("mvapi.dll", CharSet = CharSet.Auto)]
        public static extern int MV_OperateDevice(int hDevice, int Oper);

        /// <summary>
        /// ����pImageData��ͼ��MV_FILETYPEָ�����͵��ļ���
        /// </summary>
        /// <param name="FileName">�ļ�·��</param>
        /// <param name="FileType">�ļ�����RAW:1/BMP�ļ���2/JPEG�ļ���</param>
        /// <param name="pImageData">ͼ�����ݻ�����ָ��</param>
        /// <param name="pinfo">ͼ��������Ϣ</param>
        /// <param name="ImageTotal">ͼ������</param>
        /// <param name="IsUpDown">����ͼ��ʱ�Ƿ����µߵ�</param>
        /// <param name="ColororNot">��JPEG�ļ�ʱ�Ƿ�Ҫ��ɫ</param>
        /// <param name="Quality">��JPEG�ļ�ʱ������0��99</param>
        /// <param name="m_bRGB15">�Ƿ�16bitɫ��ת��Ϊ24bitɫ��</param>
        /// <returns>�ɹ�����true,ʧ�ܷ���false</returns>
        [DllImport("MVAPI.dll", CharSet = CharSet.Auto)]  // �����ļ�
        public static extern bool MV_SaveFile([MarshalAs(UnmanagedType.LPStr)] String FileName, int FileType, IntPtr pImageData, ref MV_IMAGEINFO pinfo, int ImageTotal, bool IsUpDown, bool ColororNot, int Quality, bool m_bRGB15);

        /// <summary>
        /// �ر�ͨ�����ͷž����Դ
        /// </summary>
        /// <param name="hDevice">ͨ�����</param>
        [DllImport("mvapi.dll", CharSet = CharSet.Auto)]
        public static extern void MV_CloseDevice(int hDevice);  //ɾ���豸
    }

    /// <summary>
    /// ΢����Ƶ��ͼ��������Ϣ
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct MV_IMAGEINFO
    {
        /// <summary>
        /// ͼ��Ĵ�С�����ֽڼ�
        /// </summary>
        [FieldOffset(0)]
        public uint Length;
        /// <summary>
        /// ͼ�����ɫ
        /// </summary>
        [FieldOffset(8)]
        public uint nColor;
        /// <summary>
        /// ͼ��ĸ�
        /// </summary>
        [FieldOffset(16)]
        public uint Heigth;
        /// <summary>
        /// ͼ��Ŀ�
        /// </summary>
        [FieldOffset(24)]
        public uint Width;
        /// <summary>
        /// ÿ������������
        /// </summary>
        [FieldOffset(32)]
        public uint SkipPixel;
    }

    /// <summary>
    /// ΢����Ƶ������״̬
    /// </summary>
    internal enum RUNOPER
    {
        /// <summary>
        /// ֹͣ����
        /// </summary>
        MVSTOP = 0,
        /// <summary>
        /// ����ʼ����
        /// </summary>
        MVRUN = 1,
        /// <summary>
        /// ��ͣ���Ĺ���
        /// </summary>
        MVPAUSE = 2,
        /// <summary>
        /// ��ѯ���ĵ�ǰ״̬
        /// </summary>
        MVQUERYSTATU = 3,
        /// <summary>
        /// Ϊ�����״̬
        /// </summary>
        MVERROR = 4
    }

    /// <summary>
    /// ΢����Ƶ����������
    /// </summary>
    internal enum MV_PARAMTER
    {
        GET_BOARD_TYPE = 0, GET_GRAPHICAL_INTERFACE = 1,
        SET_GARBIMAGEINFO = 2, SET_DISPIMAGEINFO = 3,
        BUFFERTYPE = 4, DEFAULT_PARAM = 5,

        // ������ʾ��
        DISP_PRESENCE = 6, 
        /// <summary>
        /// ��ʾ�ؼ����
        /// </summary>
        DISP_WHND = 7,
        /// <summary>
        /// ��ʾ����λ��
        /// </summary>
        DISP_TOP = 8,
        /// <summary>
        /// ��ʾ����λ��
        /// </summary>
        DISP_LEFT = 9,
        /// <summary>
        /// ��ʾ���߶�
        /// </summary>
        DISP_HEIGHT = 10,
        /// <summary>
        /// ��ʾ�����
        /// </summary>
        DISP_WIDTH = 11,

        // ����A/D�ĵ��ڲ���
        ADJUST_STANDARD = 12, ADJUST_SOURCE = 13,
        ADJUST_CHANNEL = 14, ADJUST_LUMINANCE = 15,
        ADJUST_CHROMINANE = 16, ADJUST_SATURATION = 17,
        ADJUST_HUE = 18, ADJUST_CONTRAST = 19,

        //֧��RGB��
        ADJUST_R_LUM = 20, ADJUST_G_LUM = 21,
        ADJUST_B_LUM = 22, ADJUST_R_COARSE = 23,
        ADJUST_G_COARSE = 24, ADJUST_B_COARSE = 25,

        // ���ư忨�Ĳ������
        GRAB_XOFF = 60, GRAB_YOFF = 61,
        GRAB_HEIGHT = 62, GRAB_WIDTH = 63,
        GRAB_IN_HEIGHT = 64, GRAB_IN_WIDTH = 65,
        GRAB_BITDESCRIBE = 66, GRAB_WHOLEWIDTH = 67,

        // ���ư忨�Ĺ�������
        WORK_UPDOWN = 34, WORK_FLIP = 35,
        WORK_SKIP = 36, WORK_SYNC = 37,
        WORK_INTERLACE = 38, WORK_ISBLACK = 39,
        WORK_FIELD = 40, OSD_MODE = 41,

        //֧��V500ϵ�п�
        TENBIT_MODE = 42, OUTPUT_VIDEO = 43,
        FILERSELECT1 = 44, FILERSELECT2 = 45,

        // ���ư忨�Ĳ������(����,�����ϰ汾)
        GARB_XOFF = 26, GARB_YOFF = 27,
        GARB_HEIGHT = 28, GARB_WIDTH = 29,
        GARB_IN_HEIGHT = 30, GARB_IN_WIDTH = 31,
        GARB_BITDESCRIBE = 32, GARB_WHOLEWIDTH = 33,


        //shen add 
        //֧�ֿ�����MVBOARD2.h�����п�
        DISP_FLIP = 201, IMAGE_PROCESS = 202,
        VIDEO_SINGLE = 203, GET_BOARD_PASS = 204,
        //20050407����
        RESTARTCAPTURE = 300, RESTOPCAPTURE = 301,
    };
}
