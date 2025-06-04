using Microsoft.UI;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;

namespace headspace.Models
{
    public class StrokeData
    {
        public List<Point> Points { get; set; } = new();
        public Color Color { get; set; } = Colors.Black;
        public double Thickness { get; set; } = 2;
    }

    public class ComponentItem
    {
        public string Title { get; set; }

        public string Content { get; set; }
    }
}
