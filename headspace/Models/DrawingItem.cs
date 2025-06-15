using CommunityToolkit.Mvvm.ComponentModel;
using headspace.Models.Common;

namespace headspace.Models
{
    public partial class DrawingItem : ProjectItemBase
    {
        [ObservableProperty]
        private string content;
    }
}
