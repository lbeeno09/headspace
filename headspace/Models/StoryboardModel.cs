using headspace.Models.Common;
using System.Collections.ObjectModel;

namespace headspace.Models
{
    public partial class StoryboardModel : ModelBase
    {
        private ObservableCollection<PanelModel?> _panels = new();
        public ObservableCollection<PanelModel?> Panels
        {
            get => _panels;
            set => SetPropertyAndMarkDirty(ref _panels, value);
        }
        public override string FilePathPrefix => "storyboard";
    }
}
