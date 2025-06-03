using System.Collections.Generic;

namespace headspace.Models
{
    public class MoodboardItem
    {
        public string Title { get; set; } = string.Empty;

        public List<StrokeData> Strokes { get; set; } = new();
    }
}
