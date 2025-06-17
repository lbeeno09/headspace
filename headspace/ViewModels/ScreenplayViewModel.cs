using CommunityToolkit.Mvvm.ComponentModel;
using headspace.Models;
using headspace.ViewModels.Common;
using Microsoft.UI.Xaml;
using System;
using System.Linq;

namespace headspace.ViewModels
{
    public partial class ScreenplayViewModel : ObservableObject
    {
        public ListItemManagerViewModel<ScreenplayItem> ScreenplayListManager { get; }

        public ScreenplayItem SelectedScreenplay => ScreenplayListManager.SelectedItem;

        public event EventHandler<ScreenplayItem> RequestDisplayScreenplay;

        public XamlRoot PageXamlRoot
        {
            set
            {
                if(value != null)
                {
                    ScreenplayListManager.XamlRoot = value;
                }
            }
        }

        public ScreenplayViewModel()
        {
            ScreenplayListManager = new ListItemManagerViewModel<ScreenplayItem>((App.Current as App).CurrentProject.Screenplays);

            ScreenplayListManager.SelectedItem = ScreenplayListManager.Items.FirstOrDefault();
            ScreenplayListManager.OnItemSelected += (sender, item) => OnPropertyChanged(nameof(SelectedScreenplay));
        }
    }
}
