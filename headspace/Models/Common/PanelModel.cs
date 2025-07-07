using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace headspace.Models.Common
{
    public partial class PanelModel : ObservableObject
    {
        [ObservableProperty]
        private string? _title;

        public ObservableCollection<StrokeData> Strokes { get; set; } = new();
    }
}
