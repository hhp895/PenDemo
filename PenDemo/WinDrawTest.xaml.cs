using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PenDemo.Util;

namespace PenDemo
{
    /// <summary>
    /// WinDrawTest.xaml 的交互逻辑
    /// </summary>
    public partial class WinDrawTest : Window
    {
       private PathFigure pathFigure;

        public WinDrawTest()
        {
            InitializeComponent();
            initViews();

        }

        private void initViews()
        {
            PathGeometry pathGeometry = new PathGeometry();
            pathFigure = new PathFigure();
            pathGeometry.Figures.Add(pathFigure);
            Point p1 = new Point(100, 300);
            Point p2 = new Point(200, 200);
            Point p3 = new Point(300, 300);
            Point p4= new Point(400, 300);

//            List<Point> controlPoints = getControlPoints(0.3, p1, p2, p3);
            
            pathFigure.StartPoint= p1;
            
//            BezierSegment bezierSegment=new BezierSegment(p1,controlPoints[0],p2,true);
//            pathFigure.Segments.Add(bezierSegment);
//            Point lastControlPoint = controlPoints[1];
//
//            controlPoints = getControlPoints(0.3, p2, p3, p4); ;
//            bezierSegment = new BezierSegment(lastControlPoint, controlPoints[0], p3, true);
//
//            pathFigure.Segments.Add(bezierSegment);


            List<Point> controlPoints = BezierHelper.getControlPoints(0.4, p1, p2);

            pathFigure.StartPoint = p1;
            LineSegment lineSegment = new LineSegment(controlPoints[0], true);
            pathFigure.Segments.Add(lineSegment);

            controlPoints = BezierHelper.getControlPoints(0.4, p2, p3);
            QuadraticBezierSegment bezierSegment = new QuadraticBezierSegment(p2, controlPoints[0], true);
            pathFigure.Segments.Add(bezierSegment);

            lineSegment = new LineSegment(controlPoints[1], true);
            pathFigure.Segments.Add(lineSegment);

            controlPoints = BezierHelper.getControlPoints(0.4, p3, p4);
            bezierSegment = new QuadraticBezierSegment(p3, controlPoints[0], true);
            pathFigure.Segments.Add(bezierSegment);

            Path path = new Path();
            path.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            path.Data = pathGeometry;
            canvas.Children.Add(path);

        }

        private List<Point> getControlPoints(double k,Point p1, Point p2, Point p3)
        {
            List<Point> controlPoints=new List<Point>();
            Point p12=new Point();
            Point p23=new Point();

            Point diffPoint1 = new Point();
            Point diffPoint2 = new Point();

            diffPoint1.X = p2.X - p1.X;
            diffPoint1.Y = p2.Y - p1.Y;
            diffPoint2.X = p3.X - p2.X;
            diffPoint2.Y = p3.Y - p2.Y;

            double length1 =Math.Sqrt(diffPoint1.X*diffPoint1.X + diffPoint1.Y * diffPoint1.Y);
            double length2 =Math.Sqrt(diffPoint2.X * diffPoint2.X + diffPoint2.Y * diffPoint2.Y);

            p12.X = p2.X - (k * diffPoint1.X);
            p12.Y = p2.Y - (k * diffPoint1.Y);
            p23.X = p2.X + (k * diffPoint2.X);
            p23.Y = p2.Y + (k * diffPoint2.Y);

            Point p1223 = new Point();
            p1223.X = p12.X + (p23.X - p12.X) * (length1 / (length1 + length2));
            p1223.Y = p12.Y + (p23.Y - p12.Y) * (length1 / (length1 + length2));
            Point controlP12 = new Point();
            Point controlP23 = new Point();
            controlP12.X = p12.X + p2.X - p1223.X;
            controlP12.Y = p12.Y + p2.Y - p1223.Y;
            controlP23.X = p23.X + p2.X - p1223.X;
            controlP23.Y = p23.Y + p2.Y - p1223.Y;

            controlPoints.Add(controlP12);
            controlPoints.Add(controlP23);
            return controlPoints;
        }
    }
}
