using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Web;
using System.Windows.Forms;

namespace Lua_test_CS
{
    /// <summary>
    /// SB101数据接收socket
    /// </summary>
    public partial class Form1 : Form
    {
        #region 数据结构定义
        public struct _SENDTAB
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]			//通道名称[英文名]
            public string ChName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]			//通道类型，采集名称[中文名]
            public string ChName_CHN;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]			//工程单位
            public string EngUnit;
            public float DownLimit;                            //下限值
            public float UpLimit;                          //上限值
        };

        //===========网络传送试验信息,远程试验/东汽数据显示专用====================
        public struct _PACKETHEAD
        {
            public Int32 nPktLens;	//包长度,不包含包头长度
            public Int32 nNums;		//通道、数据或试验信息个数	
            public Int32 nCommandID;  //1--收/发物理通,2--收/发通道表, 3--收/发工程单位转换表，4--收/发数据，5-收/发零点数据,6-收/发试验项目信息
            public Int32 nBlank;		//保留
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 56)]	////试验信息
            public string strTestInfo;
        };

        public struct _SYSTEMTIME
        {
            short sYear;
            short sMonth;
            short sDayOfWeek;
            short sDay;
            short sHour;
            short sMinute;
            short sSecond;
            short sMillisecond;
        };

        //===============DATAHEAD1=================
        public struct DATAHEAD1
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string StatusName;     //状态名，例如MC,ZD
            _SYSTEMTIME SysTime;          //采集时间和采集日期
        };
        #endregion

        static Socket socket;
        static string ipAddr = "127.0.0.1";
        static int port = 9000;

        #region  Socket相关变量定义
        static byte[] t_buffer = new byte[1024 * 1024];
        public static float[] data = new float[2000];
        static int dataNums = 0;
        public static ArrayList arraylist = new ArrayList();
        static DataTable table = new DataTable();
        public static bool m_bIsRcvChannelTable = false;
        public int m_nRcvChannelNums = 0;
        public int m_nRcvDataNums = 0;
        Thread th;
        ArrayList listData = new ArrayList();
        public static bool bIsConnectOk = false;
        #endregion

        #region 连接服务器
        public bool Connect(string ipAddr, int port)
        {
            IPAddress ip = IPAddress.Parse(ipAddr);
            IPEndPoint point = new IPEndPoint(ip, port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                //连接服务器
                socket.Connect(point);
                //接收服务器发送的消息
                th = new Thread(new ThreadStart(RecMsg));
                th.IsBackground = true;
                th.Start();

                //启动刷新控件的Timer
                //timer1.Enabled = true;
                bIsConnectOk = true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("连接服务器出错！");
                bIsConnectOk = false;
                return false;
            }
            bIsConnectOk = true;
            return true;
        }
        #endregion

        public static int g_nRecvBufLenSum = 0;
        public static bool g_bIsRecvPackHead = false;
        byte[] buffer = new byte[1024 * 1024];
        public static int g_nLeaveLen = 0;
        public string[] g_strAllChannels = new string[10000];
        void RecMsg()
        {
            if (th.IsAlive)
                bIsConnectOk = !socket.Poll(1000, SelectMode.SelectRead);


            float[] dt = new float[10000];
            while (true)
            {
                try
                {   ///已经接收到的长度
                    int num = socket.Receive(buffer, g_nRecvBufLenSum, 500000, SocketFlags.None);               //当需要接收的长度大于Receive一次接收的缓冲区长度时，这次需要多次使用Receive函数接收
                    //每次Receive函数实际接收到的长度是不同的。
                    g_nRecvBufLenSum += num;

                    _PACKETHEAD pkt = new _PACKETHEAD();            //接收到包头
                    int nHeadLen = Marshal.SizeOf(pkt);
                    IntPtr t_buf = Marshal.AllocHGlobal(nHeadLen);
                    Marshal.Copy(buffer, 0, t_buf, nHeadLen);
                    pkt = (_PACKETHEAD)Marshal.PtrToStructure(t_buf, pkt.GetType());
                    Marshal.FreeHGlobal(t_buf);

                    //   System.Buffer.BlockCopy(buffer, 0, ph, 0, 72);

                    int k = nHeadLen;
                    if (pkt.nCommandID == 2)
                    {
                        int nTabLen = 0;
                        _SENDTAB chtab = new _SENDTAB();
                        nTabLen = Marshal.SizeOf(chtab);
                        int nDataNums = pkt.nNums;
                        int nMustRecvLen = nHeadLen + nTabLen * nDataNums;   //需要接收的长度

                        if (g_nRecvBufLenSum >= nMustRecvLen)
                        {
                            arraylist.Clear();
                            dataNums = pkt.nNums;

                            IntPtr t_buf1 = Marshal.AllocHGlobal(nTabLen);
                            for (int i = 0; i < dataNums; i++)
                            {
                                Marshal.Copy(buffer, k, t_buf1, nTabLen);
                                chtab = (_SENDTAB)Marshal.PtrToStructure(t_buf1, chtab.GetType());
                                arraylist.Add(chtab);

                                k = k + nTabLen;

                                g_strAllChannels[i] = chtab.ChName.ToString() + "[" + chtab.ChName_CHN.ToString() + "]";

                            }
                            Marshal.FreeHGlobal(t_buf1);

                            g_nLeaveLen = g_nRecvBufLenSum - nMustRecvLen;

                            IntPtr t_bufLeave = Marshal.AllocHGlobal(g_nLeaveLen);

                            Marshal.Copy(buffer, nMustRecvLen, t_bufLeave, g_nLeaveLen);
                            Marshal.Copy(t_bufLeave, buffer, 0, g_nLeaveLen);
                            Marshal.FreeHGlobal(t_bufLeave);

                            g_nRecvBufLenSum = g_nLeaveLen;

                            m_nRcvChannelNums++;
                            m_bIsRcvChannelTable = true;
                            data = new float[nDataNums];

                            //如果是用socket接收通道表，从数据库读取数据，在接受完通道表后需要将socket关闭
                            //socket.Close();

                            //timer1.Enabled = true;
                            //timer1.Start();

                            SetChannelTableToGrid();
                        }
                    }

                    if (pkt.nCommandID == 4)  //数据
                    {
                        DATAHEAD1 dataHead1 = new DATAHEAD1();
                        int dataHead1_len = Marshal.SizeOf(dataHead1);
                        int nMustRecvLen = nHeadLen + sizeof(float) * pkt.nNums + dataHead1_len;
                        if (g_nRecvBufLenSum >= nMustRecvLen)
                        {
                            int nPacketNums = g_nRecvBufLenSum / nMustRecvLen;
                            for (int i = 0; i < nPacketNums; i++)
                            {
                                //解析包头1-状态信息和时间标签
                                IntPtr t_bufDataHead1 = Marshal.AllocHGlobal(dataHead1_len);
                                Marshal.Copy(buffer, nHeadLen, t_bufDataHead1, dataHead1_len);
                                dataHead1 = (DATAHEAD1)Marshal.PtrToStructure(t_bufDataHead1, dataHead1.GetType());
                                Marshal.FreeHGlobal(t_bufDataHead1);

                                //解析数据
                                System.Buffer.BlockCopy(buffer, (nHeadLen + dataHead1_len) * (i + 1) + sizeof(float) * pkt.nNums * i, data, 0, sizeof(float) * pkt.nNums);

                                g_nLeaveLen = g_nRecvBufLenSum - nMustRecvLen;
                                IntPtr t_bufLeave = Marshal.AllocHGlobal(g_nLeaveLen);

                                Marshal.Copy(buffer, nMustRecvLen, t_bufLeave, g_nLeaveLen);
                                Marshal.Copy(t_bufLeave, buffer, 0, g_nLeaveLen);
                                Marshal.FreeHGlobal(t_bufLeave);

                                g_nRecvBufLenSum = g_nLeaveLen;

                                m_nRcvDataNums++;

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("接收数据错误！"+ " 错误信息：\r\n" + ex.ToString());
                    //listBox_info.Items.Add("接收数据错误！");
                    break;
                }
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void DisConnect()
        {
            //th.Abort();
            if (bIsConnectOk)
            socket.Close();
        }
    }
}