using Microsoft.UI;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;

namespace headspace.Models.Common
{
    public class StrokeData
    {
        public List<Point> Points { get; set; } = new();
        public Color Color { get; set; } = Colors.Black;
        public double Thickness { get; set; } = 2;
    }
}
