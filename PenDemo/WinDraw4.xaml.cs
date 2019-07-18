using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using PenDemo.Model;
using PenDemo.Util;
using System.Security.Permissions;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;

namespace PenDemo
{
    /// <summary>
    /// WinDraw.xaml 的交互逻辑
    /// </summary>
    public partial class WinDraw4 : Window
    {
        private static WinDraw4 instance;
        private List<PenStroke> penStrokes;

        private List<Point> points;
        private Point m_point;
        private int m_nPenStatus = 0;
        private StylusPointCollection stylusPointCollection;
        private Stroke stroke;
        private int brushWidth = 3;
        public static WinDraw4 getInstance()
        {
            if (instance == null)
            {
                instance = new WinDraw4();
                instance.WindowState = WindowState.Maximized;
                instance.Activate();
            }

            return instance;
        }

        public WinDraw4()
        {
            InitializeComponent();

            initView();
            initListener();
        }

        private void initListener()
        {
            this.btnReplay.Click += BtnReplay_Click;
            this.btnClear.Click += BtnClear_Click;

        }



        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ic.Strokes.Clear();
        }

        private void BtnReplay_Click(object sender, RoutedEventArgs e)
        {

            for (int index = 0; index < penStrokes.Count; index++)
            {
                var penStroke = penStrokes[index];
                var currentIndex = index;
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    if (currentIndex > 0)
                    {
                        int sleepTime = Convert.ToInt16(penStroke.startTime - penStrokes[currentIndex - 1].endTime);
                        Thread.Sleep(sleepTime);
                    }

                    int everySleepTime = Convert.ToInt16(penStroke.endTime - penStroke.startTime) / penStroke.points.Count;
                    for (int i = 0; i < penStroke.points.Count; i++)
                    {
                        var ii = i;
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        {
                            Thread.Sleep(everySleepTime);
                            Point p = penStroke.points[ii];
                            if (ii == 0) lastPoint = p;

//                            drawLine(lastPoint, p);
                            lastPoint = p;
                        }));
                    }
                }));
            }


        }

        private void initView()
        {
            penStrokes = new List<PenStroke>();

            points = new List<Point>();
            ic.EditingMode = InkCanvasEditingMode.None;
        }

        private Point lastPoint;
        private bool IsHaveLastPoint = false;
        private long startTime;
        private long endTime;
        private DateTime dtFrom = new DateTime(2019, 1, 1, 0, 0, 0, 0);
        private int startCount = 0;
        public void drawLine(int nPenStatus, int x, int y, float nCompress)
        {
            Point p = new Point(x, y);
            if (!pointIsInvalid(nPenStatus, p))
            {
                return;
            }

            if (nPenStatus == 0)//离笔
            {
                IsHaveLastPoint = false;
                startCount = 0;
            }
            else
            {
                startCount++;
                if (!IsHaveLastPoint)
                {
                    stylusPointCollection = new StylusPointCollection();
                    stylusPointCollection.Add(new StylusPoint(x, y,0.1f));
                    Console.WriteLine("draw4：{0},{1},{2}",x,y,nCompress);
                    stroke = new Stroke(stylusPointCollection);
                    stroke.DrawingAttributes = new DrawingAttributes { Width = brushWidth, Height = brushWidth, Color = Color.FromRgb(0, 0, 0) };
                    IsHaveLastPoint = true;
                    ic.Strokes.Add(stroke);
                }
                else
                {
                    if (startCount < 3)
                    {
                        stylusPointCollection.Add(new StylusPoint(x, y, 0.1f));
                    }
                    else
                    {
                        stylusPointCollection.Add(new StylusPoint(x, y, nCompress));
                    }
                   
                    Console.WriteLine("draw4：{0},{1},{2}", x, y, nCompress);
                }
            }
        }



 
    
      


        private bool pointIsInvalid(int nPenStatus, Point pointValue)
        {
            if ((m_point == pointValue) && (m_nPenStatus == nPenStatus))
                return false;
            m_point = pointValue;
            m_nPenStatus = nPenStatus;
            return true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

     
    }
}
