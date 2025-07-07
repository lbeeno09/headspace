using headspace.Models.Common;
using System.Collections.ObjectModel;

namespace headspace.Models
{
    public partial class DrawingModel : ModelBase
    {
        private ObservableCollection<LayerModel> _layers = new();
        public ObservableCollection<LayerModel> Layers
        {
            get => _layers;
            set => SetPropertyAndMarkDirty(ref _layers, value);
        }
    }
}
