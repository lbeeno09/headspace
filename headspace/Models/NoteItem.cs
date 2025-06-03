using CommunityToolkit.Mvvm.ComponentModel;

namespace headspace.Models
{
    public partial class NoteItem : ObservableObject
    {
        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private string content;

    }
}
