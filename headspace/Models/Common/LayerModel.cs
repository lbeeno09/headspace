using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace headspace.Models.Common
{
    public partial class LayerModel : ObservableObject
    {
        [ObservableProperty]
        private string? _name;

        [ObservableProperty]
        private bool _isVisible = true;

        public ObservableCollection<StrokeData> Strokes { get; set; } = new();
    }
}
