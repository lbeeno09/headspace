using System.Collections.ObjectModel;

namespace headspace.Models.Common
{
    public partial class PanelModel : ModelBase
    {
        public ObservableCollection<StrokeData> Strokes { get; set; } = new();

        public override string FilePathPrefix => "storyboard_panels";
    }
}
