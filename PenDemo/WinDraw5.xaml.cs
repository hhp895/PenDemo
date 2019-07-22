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
    public partial class WinDraw5 : Window
    {
        private static WinDraw5 instance;
        private List<PenStroke> penStrokes;

        private List<MyPoint> points;
        private Point m_point;
        private int m_nPenStatus = 0;
        private StylusPointCollection stylusPointCollection;
        private Stroke stroke;
        private int brushWidth = 3;
        public static WinDraw5 getInstance()
        {
            if (instance == null)
            {
                instance = new WinDraw5();
                instance.WindowState = WindowState.Maximized;
                instance.Activate();
            }

            return instance;
        }

        public WinDraw5()
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
                startCount = 0;
                var penStroke = penStrokes[index];
                var currentIndex = index;
               var isStartedPoint = false;
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
//                    if (currentIndex > 0)
//                    {
//                        int sleepTime = Convert.ToInt16(penStroke.startTime - penStrokes[currentIndex - 1].endTime);
//                        Thread.Sleep(sleepTime);
//                    }

                    int everySleepTime = Convert.ToInt16(penStroke.endTime - penStroke.startTime) / penStroke.points.Count;
                    for (int i = 0; i < penStroke.points.Count; i++)
                    {
                        var ii = i;
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        {
                            Thread.Sleep(everySleepTime);
                            Point p = penStroke.points[ii].Point;
                        

                            drawLine2(penStroke.points[ii].IsFirst,penStroke.points[ii].Pressure, p,ii>2?penStroke.points[ii-2]:new MyPoint(),ii>2?penStroke.points[ii-1]:new MyPoint());
                        }));
                    }
                }));
            }


        }

        private void initView()
        {
            penStrokes = new List<PenStroke>();

            points = new List<MyPoint>();
            ic.EditingMode = InkCanvasEditingMode.None;
        }

        private Point lastPoint;
        private bool isStartedPoint = false;
        private long startTime;
        private long endTime;
        private int startCount = 0;

        private Point lastControlPoint;
        private bool isHaveLastControlPoint;
        private long lastTime;
        private DateTime dtFrom= new DateTime(2019, 7, 1, 0, 0, 0, 0);
        private bool isFirst = false;
        public void drawLine(int nPenStatus, int x, int y, float nCompress)
        {
            Point p = new Point(x, y);
            if (!pointIsInvalid(nPenStatus, p))
            {
                return;
            }

            if (nPenStatus == 0 )//离笔
            {
                if (points.Count > 0)
                {
                    endTime = (DateTime.Now.Ticks - dtFrom.Ticks) / 10000;
                    penStrokes.Add(new PenStroke { startTime = startTime, endTime = endTime, points = points });

                    isStartedPoint = false;
                    startCount = 0;
                    isHaveLastControlPoint = false;
                    points=new List<MyPoint>();
                }
               
               
            }
            else
            {
               
                var thisTime = (DateTime.Now.Ticks - dtFrom.Ticks) / 10000;
                if(lastTime==0) lastTime=thisTime;
                else
                {
                    if (thisTime - lastTime > 140)
                    {
                       
                        drawLine(nCompress, p, points.Count>2? points[points.Count-2].Point:new Point(), points.Count > 2 ? points[points.Count-1].Point:new Point());
                     
                    }
                   

                }
               
            }
        }

        private void drawLine( float nCompress, Point p,Point lastTwoPoint,Point lastOnePoint)
        {
            startCount++;
            if (!isStartedPoint)
            {
                startTime = (DateTime.Now.Ticks - dtFrom.Ticks) / 10000;
                lastControlPoint = p;
                stylusPointCollection = new StylusPointCollection();
                stylusPointCollection.Add(new StylusPoint(p.X, p.Y, 0.1f));
              
                stroke = new Stroke(stylusPointCollection);
                stroke.DrawingAttributes = new DrawingAttributes
                    {Width = brushWidth, Height = brushWidth, Color = Color.FromRgb(0, 0, 0)};
                isStartedPoint = true;
                isFirst = true;
                ic.Strokes.Add(stroke);
                isHaveLastControlPoint = false;
            }
            else
            {
                isFirst = false;
                if (startCount < 3)
                {
                    stylusPointCollection.Add(new StylusPoint(p.X, p.Y, 0.1f));
                }
                else
                {
                    var controlPoints = BezierHelper.getControlPoints(0.4, lastTwoPoint, lastOnePoint);
                    if (isHaveLastControlPoint && lastControlPoint.X!=0 && lastControlPoint.Y!=0)
                    {
                        var quadraticBezierPoints = BezierHelper.getQuadraticBezierPoints(lastControlPoint,
                            lastTwoPoint,
                            controlPoints[0], 6);
                        foreach (var item in quadraticBezierPoints)
                        {
                            stylusPointCollection.Add(new StylusPoint(item.X, item.Y, nCompress));
                        }

                        lastControlPoint = controlPoints[1];
                    }
                    else
                    {
                        isHaveLastControlPoint = true;
                        lastControlPoint = p;
                        stylusPointCollection.Add(new StylusPoint(p.X, p.Y, nCompress));
                    }
                }

                Console.WriteLine("draw5：{0},{1},{2}", p.X, p.Y, nCompress);
            }
            points.Add(new MyPoint { Point = p, Pressure = nCompress, IsFirst = isFirst,LastControlPoint = lastControlPoint });

        }
        private void drawLine2(bool isFrist,float nCompress, Point p, MyPoint lastTwoPoint, MyPoint lastOnePoint)
        {
            
            if (isFrist)
            {
              
                Console.WriteLine("isFirst:"+isFrist);
                startTime = (DateTime.Now.Ticks - dtFrom.Ticks) / 10000;
                lastControlPoint = p;
                stylusPointCollection = new StylusPointCollection();
                stylusPointCollection.Add(new StylusPoint(p.X, p.Y, 0.1f));

                stroke = new Stroke(stylusPointCollection);
                stroke.DrawingAttributes = new DrawingAttributes
                { Width = brushWidth, Height = brushWidth, Color = Color.FromRgb(0, 0, 0) };
             
           
                ic.Strokes.Add(stroke);
                isHaveLastControlPoint = false;
                
            }
            else
            {
            
                if ((lastTwoPoint.Point.X==0 && lastTwoPoint.Point.Y==0)||(lastOnePoint.Point.X == 0 && lastOnePoint.Point.Y == 0))
                {
                    stylusPointCollection.Add(new StylusPoint(p.X, p.Y, 0.1f));
                }
                else
                {
                    var controlPoints = BezierHelper.getControlPoints(0.4, lastTwoPoint.Point, lastOnePoint.Point);
                    if (isHaveLastControlPoint && lastOnePoint.LastControlPoint.X!=0 && lastOnePoint.LastControlPoint.Y!=0)
                    {
                        Console.WriteLine("lastOnePoint.LastControlPoint:"+ lastOnePoint.LastControlPoint);
                        var quadraticBezierPoints = BezierHelper.getQuadraticBezierPoints(lastOnePoint.LastControlPoint,
                            lastTwoPoint.Point,
                            controlPoints[0], 12);
                        foreach (var item in quadraticBezierPoints)
                        {
                            stylusPointCollection.Add(new StylusPoint(item.X, item.Y, nCompress));
                        }

                        lastControlPoint = controlPoints[1];
                    }
                    else
                    {
                        isHaveLastControlPoint = true;
                        stylusPointCollection.Add(new StylusPoint(p.X, p.Y, nCompress));
                    }
                }

                Console.WriteLine("draw5：{0},{1},{2}", p.X, p.Y, nCompress);
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
