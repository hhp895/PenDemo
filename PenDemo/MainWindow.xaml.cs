using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using PenDemo.Model;
using robotpenetdevice_cs;

namespace PenDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public RbtNet rbtNet = null;
        private bool optimize = false;
        private DispatcherTimer dispatcherTimer = null;
        private bool isStart = false;
        private List<Device> devices;

        public WinDraw winDraw = null;

        public MainWindow()
        {
            InitializeComponent();
            initData();
            initView();
            initRbt();
            initEvent();
        }

        private void initData()
        {
            devices = new List<Device>();
        }

        private void initEvent()
        {
            this.btnStart.Click += BtnStart_Click;
            this.btnTest.Click += BtnTest_Click;


            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        private void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            WinDrawTest winDrawTest = new WinDrawTest();
            winDrawTest.Show();
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (isStart)
            {
                string strIP = this.cbIps.Text;
                if (!string.IsNullOrEmpty(strIP))
                {
                    rbtNet.configNet(strIP, 6001, false, true, "");
                }
            }
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (this.btnStart.Content.Equals("开始"))
            {
                isStart = rbtNet.start();
                if (!isStart)
                {
                    MessageBox.Show("启动失败！");
                    return;
                }

                this.btnStart.Content = "停止";
            }
            else
            {
                rbtNet.stop();
                devices.Clear();
            }
        }

        private void initRbt()
        {
            rbtNet = new RbtNet();
            Init_Param param = new Init_Param();
            optimize = Convert.ToBoolean(System.Configuration.ConfigurationSettings.AppSettings["optimize"]);
            param.optimize = optimize;

            rbtNet.init(ref param);

            rbtNet.deviceNameEvt_ += RbtNet_deviceNameEvt_;
            rbtNet.deviceOriginDataEvt_ += RbtNet_deviceOriginDataEvt_;
            rbtNet.deviceOptimizeDataEvt_ += RbtNet_deviceOptimizeDataEvt_;
        }

        private void RbtNet_deviceOriginDataEvt_(IntPtr ctx, IntPtr strDeviceMac, ushort us, ushort ux, ushort uy,
            ushort up)
        {
            if (optimize)
            {
                return;
            }

            string sMac = Marshal.PtrToStringAnsi(strDeviceMac);
            int npenStatus = Convert.ToInt32(us);
            if (npenStatus != 17 && npenStatus != 33)
            {
                npenStatus = 0;
            }

            Console.WriteLine("DeviceMac:{0},status:{1},x:{2},y:{3},up:{4}", sMac, npenStatus, ux, uy, up);
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (winDraw.Visibility == Visibility.Visible)
                {
                    winDraw.drawLine(npenStatus, ux/22, uy/22, up);
                }
            }));
        }

        private void RbtNet_deviceOptimizeDataEvt_(IntPtr ctx, IntPtr pmac, ushort us, ushort ux, ushort uy,
            float width, float speed)
        {
            if (!optimize)
            {
                return;
            }

            string sMac = Marshal.PtrToStringAnsi(pmac);
            int npenStatus = Convert.ToInt32(us);
            if (width == 0)
            {
                npenStatus = 0;
            }

            Console.WriteLine("pmac:{0},status:{1},x:{2},y:{3},width:{4},speed:{5}", sMac, npenStatus, ux, uy, width,
                speed);
           
        }

        private void RbtNet_deviceNameEvt_(IntPtr ctx, string strDeviceMac, string strDeviceName)
        {
            Console.WriteLine("{0},{1}", strDeviceMac, strDeviceName);
            devices.Add(new Device {DeviceMac = strDeviceMac, DeviceName = strDeviceName});
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.lbPenPads.ItemsSource = null;
                this.lbPenPads.ItemsSource = devices;
            }));
        }

        private void initView()
        {
            winDraw = WinDraw.getInstance();


            string HostName = Dns.GetHostName();
            IPHostEntry ipHostEntry = Dns.GetHostEntry(HostName);
            this.cbIps.ItemsSource = null;
            List<String> ips = new List<string>();


            for (int i = 0; i < ipHostEntry.AddressList.Length; i++)
            {
                if (ipHostEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    ips.Add(ipHostEntry.AddressList[i].ToString());
                }
            }

            this.cbIps.ItemsSource = ips;
            this.cbIps.SelectedIndex = 0;
//         
//            devices.Add(new Device {DeviceMac = "001",DeviceName = "stu01"});
//            devices.Add(new Device {DeviceMac = "002",DeviceName = "stu02"});
            this.lbPenPads.ItemsSource = devices;
        }

        private void ListBoxItem_PreviewMouseDown(object sender, MouseEventArgs e)
        {
            // isDragChar = true;
            ListBoxItem item = sender as ListBoxItem;
            Console.WriteLine("" + item.Content);

            winDraw.Show();
            this.WindowState=WindowState.Minimized;
        }
    }
}