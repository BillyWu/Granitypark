using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Granity.granityMgr.ParkMgr.VideoPlugin
{
    /// <summary>
    /// ������Ƶ���ӿ�
    /// </summary>
    public class VideoMiniMV
    {
        //Mini V110��Ƶ�����÷���
        [DllImport("mvapi.dll", CharSet = CharSet.Auto)]
        public static extern uint MV_GetDeviceNumber();   //�忨��

        [DllImport("mvapi.dll", CharSet = CharSet.Auto)]
        public static extern int MV_OpenDevice(uint Index, bool bRelese);  //�����豸

        [DllImport("mvapi.dll", CharSet = CharSet.Auto)]
        public static extern bool MV_SetDeviceParameter(int hDevice, int Oper, uint Val); //�����豸����

        [DllImport("MVAPI.dll", CharSet = CharSet.Auto)]  // ����һ֡
        public static extern IntPtr MV_CaptureSingle(int hDevice, bool IsProcess, int Buffer, int Bufflen, ref MV_IMAGEINFO pinfo);

        [DllImport("mvapi.dll", CharSet = CharSet.Auto)]
        public static extern int MV_OperateDevice(int hDevice, int Oper);   //�����豸

        [DllImport("MVAPI.dll", CharSet = CharSet.Auto)]  // �����ļ�
        public static extern bool MV_SaveFile([MarshalAs(UnmanagedType.LPStr)] String FileName, int FileType, IntPtr pImageData, ref MV_IMAGEINFO pinfo, int ImageTotal, bool IsUpDown, bool ColororNot, int Quality, bool m_bRGB15);

        [DllImport("mvapi.dll", CharSet = CharSet.Auto)]
        public static extern void MV_CloseDevice(int hDevice);  //ɾ���豸
    }
    [StructLayout(LayoutKind.Explicit)]
    public struct MV_IMAGEINFO
    {
        [FieldOffset(0)]
        public uint Length;      // ͼ��Ĵ�С�����ֽڼ�
        [FieldOffset(8)]
        public uint nColor;      // ͼ�����ɫ
        [FieldOffset(16)]
        public uint Heigth;      // ͼ��ĸ�
        [FieldOffset(24)]
        public uint Width;       // ͼ��Ŀ�
        [FieldOffset(32)]
        public uint SkipPixel;   // ÿ������������
    }
    enum RUNOPER
    {
        MVSTOP = 0, // MVSTOP: ֹͣ������
        MVRUN = 1, // MVRUN: ����ʼ������
        MVPAUSE = 2, //MVPAUSE: ��ͣ���Ĺ�����
        MVQUERYSTATU = 3, //MVQUERYSTATU: ��ѯ���ĵ�ǰ״̬��
        MVERROR = 4 //MVERROR:Ϊ�����״̬��
    };
    enum MV_PARAMTER
    {
        GET_BOARD_TYPE = 0, GET_GRAPHICAL_INTERFACE = 1,
        SET_GARBIMAGEINFO = 2, SET_DISPIMAGEINFO = 3,
        BUFFERTYPE = 4, DEFAULT_PARAM = 5,

        // ������ʾ��
        DISP_PRESENCE = 6, DISP_WHND = 7,
        DISP_TOP = 8, DISP_LEFT = 9,
        DISP_HEIGHT = 10, DISP_WIDTH = 11,

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
