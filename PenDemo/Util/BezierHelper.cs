using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace PenDemo.Util
{
    class BezierHelper
    {
        public static List<Point> getControlPoints(double k, Point p1, Point p2, Point p3)
        {
            List<Point> controlPoints = new List<Point>();
            Point p12 = new Point();
            Point p23 = new Point();

            Point diffPoint1 = new Point();
            Point diffPoint2 = new Point();

            diffPoint1.X = p2.X - p1.X;
            diffPoint1.Y = p2.Y - p1.Y;
            diffPoint2.X = p3.X - p2.X;
            diffPoint2.Y = p3.Y - p2.Y;

            double length1 = Math.Sqrt(diffPoint1.X * diffPoint1.X + diffPoint1.Y * diffPoint1.Y);
            double length2 = Math.Sqrt(diffPoint2.X * diffPoint2.X + diffPoint2.Y * diffPoint2.Y);

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

        public static List<Point> getControlPoints(double k, Point begin, Point end)
        {
            List<Point> controlPoints = new List<Point>();
            Point p1 = new Point();
            Point p2 = new Point();

            p1.X = begin.X + k * (end.X - begin.X);
            p1.Y = begin.Y + k * (end.Y - begin.Y);

            p2.X = begin.X + (1 - k) * (end.X - begin.X);
            p2.Y = begin.Y + (1 - k) * (end.Y - begin.Y);

            controlPoints.Add(p1);
            controlPoints.Add(p2);
            return controlPoints;
        }
    }
}
