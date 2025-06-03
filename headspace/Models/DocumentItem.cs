using CommunityToolkit.Mvvm.ComponentModel;

namespace headspace.Models
{
    public partial class DocumentItem : ObservableObject
    {
        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private string content;

    }
}
