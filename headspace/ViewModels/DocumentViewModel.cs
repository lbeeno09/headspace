using CommunityToolkit.Mvvm.ComponentModel;
using headspace.Models;
using headspace.ViewModels.Common;
using Microsoft.UI.Xaml;
using System.Linq;

namespace headspace.ViewModels
{
    public partial class DocumentViewModel : ObservableObject
    {
        public ListItemManagerViewModel<DocumentItem> DocumentListManager { get; }

        public DocumentItem SelectedDocument => DocumentListManager.SelectedItem;

        public XamlRoot PageXamlRoot
        {
            set
            {
                if(value != null)
                {
                    DocumentListManager.XamlRoot = value;
                }
            }
        }

        public DocumentViewModel()
        {
            DocumentListManager = new ListItemManagerViewModel<DocumentItem>((App.Current as App).CurrentProject.Documents);

            DocumentListManager.SelectedItem = DocumentListManager.Items.FirstOrDefault();
            DocumentListManager.OnItemSelected += (sender, item) => OnPropertyChanged(nameof(SelectedDocument));
        }
    }
}
