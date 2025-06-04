using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using headspace.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace headspace.ViewModels
{
    public partial class ScreenplayViewModel : ObservableObject
    {
        public ObservableCollection<ScreenplayItem> Screenplays { get; } = new();

        [ObservableProperty]
        private ScreenplayItem? selectedScreenplay;

        public bool IsItemSelected => SelectedScreenplay is not null;

        public ScreenplayViewModel()
        {
        }

        partial void OnSelectedScreenplayChanged(ScreenplayItem? value)
        {
            OnPropertyChanged(nameof(IsItemSelected));
        }

        [RelayCommand]
        private void AddScreenplay()
        {
            string baseTitle = "Untitled";
            string newTitle = baseTitle;

            int i = 1;
            while(Screenplays.Select(n => n.Title).Contains(newTitle))
            {
                newTitle = $"{baseTitle}{i++}";
            }

            ScreenplayItem newScreenplay = new ScreenplayItem { Title = newTitle, Content = "" };

            Screenplays.Add(newScreenplay);
            SelectedScreenplay = newScreenplay;
        }

        [RelayCommand]
        private void DeleteScreenplay()
        {
            if(SelectedScreenplay != null)
            {
                Screenplays.Remove(SelectedScreenplay);
            }
        }

        [RelayCommand]
        public async Task RenameScreenplayAsync(XamlRoot xamlRoot)
        {
            if(SelectedScreenplay == null || xamlRoot == null)
            {
                return;
            }

            var inputDialog = new ContentDialog
            {
                Title = "Rename Screenplay",
                Content = new TextBox
                {
                    Text = SelectedScreenplay.Title,
                    AcceptsReturn = false,
                    PlaceholderText = "Enter new title"
                },
                PrimaryButtonText = "Rename",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = xamlRoot
            };

            var result = await inputDialog.ShowAsync();

            if(result == ContentDialogResult.Primary && inputDialog.Content is TextBox textBox)
            {
                var newTitle = textBox.Text.Trim();

                if(!string.IsNullOrWhiteSpace(newTitle))
                {
                    SelectedScreenplay.Title = newTitle;
                }
            }
        }
    }
}
