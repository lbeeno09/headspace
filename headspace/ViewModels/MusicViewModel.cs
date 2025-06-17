using CommunityToolkit.Mvvm.ComponentModel;
using headspace.Models;
using headspace.ViewModels.Common;
using Microsoft.UI.Xaml;
using System.Linq;

namespace headspace.ViewModels
{
    public partial class MusicViewModel : ObservableObject
    {
        public ListItemManagerViewModel<MusicItem> MusicListManager { get; }

        public MusicItem SelectedMusic => MusicListManager.SelectedItem;

        public XamlRoot PageXamlRoot
        {
            set
            {
                if(value != null)
                {
                    MusicListManager.XamlRoot = value;
                }
            }
        }

        public MusicViewModel()
        {
            MusicListManager = new ListItemManagerViewModel<MusicItem>((App.Current as App).CurrentProject.Musics);

            MusicListManager.SelectedItem = MusicListManager.Items.FirstOrDefault();
            MusicListManager.OnItemSelected += (sender, item) => OnPropertyChanged(nameof(SelectedMusic));
        }
    }
}
