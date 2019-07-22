using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Point = System.Windows.Point;

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

        public static Point getBezierInterpolationPoint(float t, Point[] points, int count)
        {
            if (points.Length < 1)
            {
               throw new ArgumentOutOfRangeException();
            }
            Point[] tmp_points=new Point[count];
            for (int i = 1; i < count; i++)
            {
                for (int j = 0; j < count-i; j++)
                {
                    if (i == 1)
                    {
                        float x1 =(float) (points[j].X * (1 - t) + points[j + 1].X * t);
                        float y1= (float)(points[j].Y * (1 - t) + points[j + 1].Y * t);
                        tmp_points[j]=new Point {X=x1,Y=y1};
                      
                    }
                    else
                    {
                        float x = (float) (tmp_points[j].X * (1 - t) + tmp_points[j + 1].X * t);
                        float y = (float) (tmp_points[j].Y * (1 - t) + tmp_points[j + 1].Y * t);
                        tmp_points[j]=new Point {X=x,Y=y};
                    }
                }

                
            }
            return tmp_points[0];
        }

        public static List<Point> getBezierCurvesPoints(List<Point> points, float step)
        {
            List<Point> bezierCurvesPoints=new List<Point>();
            float t = 0f;
            do
            {
                Point tmp_point = getBezierInterpolationPoint(t, points.ToArray(), points.Count);
                t += step;
                bezierCurvesPoints.Add(tmp_point);

            } while (t <= 1 && points.Count > 1);

            return bezierCurvesPoints;

        }

        public static double getQuadraticBezierInterpolation(double z0, double z1, double z2, double t)
        {
            double a1 = (double) ((1.0 - t) * (1.0 - t) * z0);
            double a2 = (double) (2.0*t*(1-t)*z1);
            double a3 = (double) (t*t*z2);
            double a4 = a1 + a2 + a3;
            return a4;
        }

        public static List<Point> getQuadraticBezierPoints(Point p1, Point p2, Point p3, int steps)
        {
            List<Point> points=new List<Point>();
            float stepLength = 1.0f / steps;
            float t = 0f;
            while (t <= 1)
            {
                double x = getQuadraticBezierInterpolation(p1.X, p2.X, p3.X,t);
                double y = getQuadraticBezierInterpolation(p1.Y, p2.Y, p3.Y,t);
                points.Add(new Point {X=x,Y=y});
                t = t + stepLength;
            }

            return points;
        }   


       
    }
}
