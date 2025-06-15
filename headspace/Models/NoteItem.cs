using CommunityToolkit.Mvvm.ComponentModel;
using headspace.Models.Common;

namespace headspace.Models
{
    public partial class NoteItem : ProjectItemBase
    {
        [ObservableProperty]
        private string content;
    }
}
