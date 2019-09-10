using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    class YoloData
    {
        public YoloData(string tp, double Confi, int x, int y, int w, int h)
        {
            Type = tp;
            Confidence = Confi;
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }
        public double Confidence { get; set; }
        public int Height { get; set; }
        public string Type { get; set; }
        public int Width { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}
