using headspace.Models.Common;
using System.Collections.ObjectModel;

namespace headspace.Models
{
    public partial class MoodboardModel : ModelBase
    {
        private ObservableCollection<StrokeData> _strokes = new();
        public ObservableCollection<StrokeData> Strokes
        {
            get => _strokes;
            set => SetPropertyAndMarkDirty(ref _strokes, value);
        }
    }
}
