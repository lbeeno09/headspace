using Microsoft.Graphics.Canvas.Geometry;
using Windows.UI;

namespace headspace.Models.Common
{
    public class StrokeData
    {
        public required CanvasGeometry Geometry { get; set; }
        public Color Color { get; set; }
        public float StrokeWidth { get; set; }
    }
}
