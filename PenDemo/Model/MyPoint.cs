using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace PenDemo.Model
{
    class MyPoint
    {
        public Point Point { get; set; }
        public float Pressure { get; set; }
        public bool IsFirst { get; set; }
        public  Point LastControlPoint { get; set; }
    }
}