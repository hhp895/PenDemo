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
    public partial class WinDrawTest4 : Window
    {
        private static WinDrawTest4 instance;

   
        private List<PenStroke> penStrokes;

        private List<Point> points;
        private Point m_point;
        private int m_nPenStatus = 0;
        public static WinDrawTest4 getInstance()
        {
            if (instance == null)
            {
                instance = new WinDrawTest4();
                instance.WindowState = WindowState.Maximized;
                instance.Activate();
            }

            return instance;
        }

        public WinDrawTest4()
        {
            InitializeComponent();

            initView();
            initListener();
        }

        private DrawingAttributes da;
        private void initListener()
        {
            this.btnReplay.Click += BtnReplay_Click;
            this.btnClear.Click += BtnClear_Click;
            this.btnStroke.Click += BtnStroke_Click;
            this.btnQuadratciBezier.Click += BtnQuadratciBezier_Click; ;
            da = new DrawingAttributes
            {
                Width = 125,
                Height = 125,
                Color = Color.FromRgb(0, 0, 0)
            };
            this.inkCanvas.DefaultDrawingAttributes = da;
            this.inkCanvas.EditingMode = InkCanvasEditingMode.None;
            //            this.PreviewMouseLeftButtonDown += Canvas_PreviewMouseDown;
            //            this.PreviewMouseMove += Canvas_PreviewMouseMove;
            //            this.PreviewMouseLeftButtonUp += Canvas_PreviewMouseUp;
        }

        private void BtnQuadratciBezier_Click(object sender, RoutedEventArgs e)
        {
            this.inkCanvas.Strokes.Clear();
            Point p1=new Point() {X=200,Y=400};
            Point p2=new Point() {X=300,Y=200};
            Point p3=new Point() {X=400,Y=400};

            List<Point> points = BezierHelper.getQuadraticBezierPoints(p1,p2,p3, Convert.ToInt16(txtCount.Text));
            StylusPointCollection stylusPointCollection = new StylusPointCollection();
            for (int i = 0; i < points.Count; i++)
            {
                stylusPointCollection.Add(new StylusPoint(points[i].X,points[i].Y));
            }
            Stroke stroke = new Stroke(stylusPointCollection);
            stroke.DrawingAttributes = new DrawingAttributes { Width = 1, Height = 1, Color = Color.FromRgb(0, 0, 0) };
            this.inkCanvas.Strokes.Add(stroke);

        }

        private void BtnStroke_Click(object sender, RoutedEventArgs e)
        {
            StylusPointCollection stylusPointCollection = new StylusPointCollection();
//            StylusPoint stylusPoint = new StylusPoint(100, 200, .2f);

            stylusPointCollection.Add(new StylusPoint(100, 200, .2f));
            stylusPointCollection.Add(new StylusPoint(100, 250, .5f));
            stylusPointCollection.Add(new StylusPoint(100, 300, .9f));
            Stroke stroke = new Stroke(stylusPointCollection);
            stroke.DrawingAttributes = new DrawingAttributes {Width = 25, Height = 25, Color = Color.FromRgb(0, 0, 0)};
            this.inkCanvas.Strokes.Add(stroke);


            stylusPointCollection = new StylusPointCollection();
            //            StylusPoint stylusPoint = new StylusPoint(100, 200, .2f);

            stylusPointCollection.Add(new StylusPoint(200, 200, .2f));
            stylusPointCollection.Add(new StylusPoint(200, 250, .5f));
            stylusPointCollection.Add(new StylusPoint(200, 300, .9f));
            stroke = new Stroke(stylusPointCollection);
            stroke.DrawingAttributes = new DrawingAttributes { Width = 25, Height = 25, Color = Color.FromRgb(0, 0, 0) };
            this.inkCanvas.Strokes.Add(stroke);
        }

        private void Canvas_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("Canvas_PreviewMouseUp");
//            if (points.Count > 0)
//            {
//                //                    Console.WriteLine(DateTime.Now.Ticks);
//                endTime = (DateTime.Now.Ticks - dtFrom.Ticks) / 10000;
//                var penStroke = new PenStroke { points = points };
//                penStroke.startTime = startTime;
//                penStroke.endTime = endTime;
//         
//                penStrokes.Add(penStroke);
//
//            }
//
//            isHaveLastControlPoint = false;
//            index = 0;
//            points = new List<Point>();
//            IsHaveLastPoint = false;

            isPress = false;
        }

        private void Canvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Console.WriteLine("Canvas_PreviewMouseMove");

            if (isPress)
            {
                Point p = e.GetPosition(inkCanvas);
                drawLine(oldPoint, p);
                oldPoint = p;
                points.Add(p);
            }
        }

        private bool isPress = false;
        private Point oldPoint;
        private void Canvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("Canvas_PreviewMouseDown");
            if (!isPress)
            {
                isPress = true;
                oldPoint = e.GetPosition(inkCanvas);
            
               
            }
            
        }


        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
         
        }

        private void BtnReplay_Click(object sender, RoutedEventArgs e)
        {
//
//            for (int index = 0; index < penStrokes.Count; index++)
//            {
//                var penStroke = penStrokes[index];
//                var currentIndex = index;
//                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
//                {
//                    if (currentIndex > 0)
//                    {
//                        int sleepTime = Convert.ToInt16(penStroke.startTime - penStrokes[currentIndex - 1].endTime);
//                        Thread.Sleep(sleepTime);
//                    }
//
//                    int everySleepTime = Convert.ToInt16(penStroke.endTime - penStroke.startTime) / penStroke.points.Count;
//                    for (int i = 0; i < penStroke.points.Count; i++)
//                    {
//                        var ii = i;
//                        this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
//                        {
//                            Thread.Sleep(everySleepTime);
//                            Point p = penStroke.points[ii];
//                            if (ii == 0) lastPoint = p;
//
//                            drawLine(lastPoint, p);
//                            lastPoint = p;
//                        }));
//                    }
//                }));
//            }


        }

        private void initView()
        {
            this.WindowState = WindowState.Maximized;
            penStrokes = new List<PenStroke>();

            points = new List<Point>();
//            createPath(5);
        }

//        private void createPath(double strokeWidth)
//        {
//            path = new Path();
//            path.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
//            path.StrokeThickness = strokeWidth;
//            pathGeometry = new PathGeometry();
//            path.Data = pathGeometry;
//            canvas.Children.Add(path);
//        }

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
          
            if (points.Count == 0  )
            {
                pathFigure = new PathFigure();
              
                IsHaveLastPoint = true;
                startTime = (DateTime.Now.Ticks - dtFrom.Ticks) / 10000;
            }

            if (isHaveLastControlPoint)
            {
                pathFigure.StartPoint = lastControlPoint;
            }
            else
            {
                pathFigure.StartPoint = p;
            }
            points.Add(p);
           
            if (IsHaveLastPoint && points.Count > 1)
            {
                drawLine(points[points.Count - 2], points[points.Count - 1]);
            }
            lastPoint = p;

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

//            List<Point> controlPoints = BezierHelper.getControlPoints(0.4, p1, p2);
//            if (isHaveLastControlPoint == false)
//            {
//                lastControlPoint = p1;
//                isHaveLastControlPoint = true;
//            }
//
//            QuadraticBezierSegment bezierSegment = new QuadraticBezierSegment(p1, controlPoints[0], true);
//
//            lastControlPoint = controlPoints[1];
//            pathFigure.Segments.Add(bezierSegment);
//            LineSegment lineSegment = new LineSegment(controlPoints[1], true);
//            pathFigure.Segments.Add(lineSegment);
//            //if (isHaveLastControlPoint == false)
//            //{
//            //    lastControlPoint = p1;
//            //    isHaveLastControlPoint = true;
//            //}
//            //LineSegment lineSegment=new LineSegment(p2,true);
//
//            //pathFigure.Segments.Add(lineSegment);
//            //            Graphics.FromHwnd(this)
//            lastControlPoint = controlPoints[1];
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
//            if (path != null)
//                path.StrokeThickness = sliderPenWidth.Value;
        }
    }
}
