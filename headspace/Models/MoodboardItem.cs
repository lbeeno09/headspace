using CommunityToolkit.Mvvm.ComponentModel;
using headspace.Models.Common;

namespace headspace.Models
{
    public partial class MoodboardItem : ProjectItemBase
    {
        [ObservableProperty]
        private string content;
    }
}
