using CommunityToolkit.Mvvm.ComponentModel;
using headspace.Models;
using headspace.ViewModels.Common;
using Microsoft.UI.Xaml;
using System.Linq;

namespace headspace.ViewModels
{
    public partial class NoteViewModel : ObservableObject
    {
        public ListItemManagerViewModel<NoteItem> NoteListManager { get; }

        public NoteItem SelectedNote => NoteListManager.SelectedItem;

        public XamlRoot PageXamlRoot
        {
            set
            {
                if(value != null)
                {
                    NoteListManager.XamlRoot = value;
                }
            }
        }

        public NoteViewModel()
        {
            NoteListManager = new ListItemManagerViewModel<NoteItem>((App.Current as App).CurrentProject.Notes);

            NoteListManager.SelectedItem = NoteListManager.Items.FirstOrDefault();
            NoteListManager.OnItemSelected += (sender, item) => OnPropertyChanged(nameof(SelectedNote));
        }
    }
}
