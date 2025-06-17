using headspace.ViewModels;
using headspace.Views.Common;
using Microsoft.UI.Xaml.Controls;
using System;

namespace headspace.Views
{
    public sealed partial class MusicPage : Page, ISavablePage
    {
        private MusicViewModel ViewModel => DataContext as MusicViewModel;

        public MusicPage()
        {
            this.InitializeComponent();
            this.DataContext = new MusicViewModel();

            this.Loaded += (s, e) =>
            {
                if(ViewModel != null)
                {
                    ViewModel.PageXamlRoot = this.XamlRoot;
                }
            };
        }

        public void SavePageContentToModel()
        {
            if(ViewModel.SelectedMusic != null)
            {
                ViewModel.SelectedMusic.LastModified = DateTime.Now;

                System.Diagnostics.Debug.WriteLine($"Musics page content updated in model for {ViewModel.SelectedMusic.Title}");
            }
        }
    }
}
