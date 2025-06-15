using CommunityToolkit.Mvvm.ComponentModel;
using headspace.Models.Common;

namespace headspace.Models
{
    public partial class DocumentItem : ProjectItemBase
    {
        [ObservableProperty]
        private string content;
    }
}
