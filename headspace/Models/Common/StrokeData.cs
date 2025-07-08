using Microsoft.Graphics.Canvas.Geometry;
using System.Collections.Generic;
using System.Numerics;
using System.Text.Json.Serialization;
using Windows.UI;

namespace headspace.Models.Common
{
    public class StrokeData
    {
        public List<Vector2> Points { get; set; } = new();
        public Color Color { get; set; }
        public float StrokeWidth { get; set; }

        [JsonIgnore]
        public CanvasGeometry? CachedGeometry { get; set; }
    }
}
