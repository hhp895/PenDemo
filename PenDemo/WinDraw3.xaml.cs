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
    public partial class WinDraw3 : Window
    {
        private static WinDraw3 instance;
        private GeometryGroup geometryGroup;
        private List<PenStroke> penStrokes;

        private List<MyPoint> points;
        private Point m_point;
        private int m_nPenStatus = 0;
        private Path path;
        public static WinDraw3 getInstance()
        {
            if (instance == null)
            {
                instance = new WinDraw3();
                instance.WindowState = WindowState.Maximized;
                instance.Activate();
            }

            return instance;
        }

        public WinDraw3()
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
            geometryGroup.Children.Clear();
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
                            Point p = penStroke.points[ii].Point;
                            if (ii == 0) lastPoint = p;

                            drawLine(lastPoint, p);
                            lastPoint = p;
                        }));
                    }
                }));
            }


        }

        private void initView()
        {
            penStrokes = new List<PenStroke>();

            points = new List<MyPoint>();
            path = new Path();
            path.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            path.StrokeThickness = sliderPenWidth.Value;




            geometryGroup = new GeometryGroup();

            path.Data = geometryGroup;
            canvas.Children.Add(path);
        }

        private Point lastPoint;
        private bool IsHaveLastPoint = false;
        private long startTime;
        private long endTime;
        private DateTime dtFrom = new DateTime(2019, 1, 1, 0, 0, 0, 0);

        private int index;
        PathFigure pathFigure;
        Stopwatch stopwatch = new Stopwatch();
        public void drawLine(int nPenStatus, int x, int y, int nCompress)
        {
            Point p = new Point(x, y);
            if (!pointIsInvalid(nPenStatus, p))
            {
                return;
            }

            if (nPenStatus == 0)
            {
                if (points.Count > 0)
                {
                    //                    Console.WriteLine(DateTime.Now.Ticks);
                    endTime = (DateTime.Now.Ticks - dtFrom.Ticks) / 10000;
                    var penStroke = new PenStroke { points = points };
                    penStroke.startTime = startTime;
                    penStroke.endTime = endTime;
                    penStrokes.Add(penStroke);

                }

                isHaveLastControlPoint = false;
                index = 0;
                points = new List<MyPoint>();
                IsHaveLastPoint = false;
            }
            else
            {
                if (IsHaveLastPoint)
                {
                    drawLine(lastPoint, p);
                }
                if (points.Count == 0)
                {
                    IsHaveLastPoint = true;
                    startTime = (DateTime.Now.Ticks - dtFrom.Ticks) / 10000;

                }

            

                lastPoint = p;
                points.Add(new MyPoint { Point = p, Pressure = nCompress });



            }


        }



        private bool isHaveLastControlPoint;
        private Point lastControlPoint;
        private void drawLine(Point p1, Point p2, Point p3)
        {

            List<Point> controlPoints = BezierHelper.getControlPoints(0.3, p1, p2, p3);
            if (isHaveLastControlPoint == false)
            {
                lastControlPoint = p1;
                isHaveLastControlPoint = true;
            }
            BezierSegment bezierSegment = new BezierSegment(lastControlPoint, controlPoints[0], p2, true);

            lastControlPoint = controlPoints[1];
            pathFigure.Segments.Add(bezierSegment);



        }
        private void drawLine(Point p1, Point p2)
        {

            List<Point> controlPoints = BezierHelper.getControlPoints(0.4, p1, p2);
            if (isHaveLastControlPoint == false)
            {
                lastControlPoint = p1;
                isHaveLastControlPoint = true;
            }
            //先画bezier，再画直线
            //                        QuadraticBezierSegment bezierSegment = new QuadraticBezierSegment(p1, controlPoints[0],  true);
            //
            //                        lastControlPoint = controlPoints[1];
            //                        pathFigure.Segments.Add(bezierSegment);
            //            LineSegment lineSegment = new LineSegment(controlPoints[1], true);
            //            pathFigure.Segments.Add(lineSegment);



            StreamGeometry streamGeometry = new StreamGeometry();
            using (StreamGeometryContext ctx = streamGeometry.Open())
            {
                ctx.BeginFigure(lastControlPoint, false, false);
                ctx.QuadraticBezierTo(p1,controlPoints[0],true,true);
                ctx.LineTo(controlPoints[1], true, true);

            }

            lastControlPoint = controlPoints[1];
            geometryGroup.Children.Add(streamGeometry);

            

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

        private void SliderPenWidth_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (path != null)
                path.StrokeThickness = sliderPenWidth.Value;
        }
    }
}
