using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace PenDemo.Model
{
    class PenStroke
    {
        public long startTime;
        public long endTime;
        public List<Point> points { get; set; }
    }
}
