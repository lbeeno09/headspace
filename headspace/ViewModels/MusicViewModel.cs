using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using headspace.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace headspace.ViewModels
{
    public partial class MusicViewModel : ObservableObject
    {
        public ObservableCollection<MusicItem> Musics { get; } = new();

        [ObservableProperty]
        private MusicItem selectedMusic;

        [ObservableProperty]
        private string musicHtml;

        public MusicViewModel()
        {
        }

        partial void OnSelectedMusicChanged(MusicItem value)
        {
            OnPropertyChanged(nameof(EditorText));
            OnPropertyChanged(nameof(HtmlContent));
        }

        public string EditorText
        {
            get => SelectedMusic?.Content ?? string.Empty;
            set
            {
                if(SelectedMusic != null)
                {
                    SelectedMusic.Content = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(HtmlContent));
                }
            }
        }

        public string HtmlContent => RenderToHtml(EditorText);

        private string RenderToHtml(string mei)
        {
            string indexPath = Path.Combine(AppContext.BaseDirectory, "Assets", "verovio", "index.html");
            string html = File.ReadAllText(indexPath);

            string escapedMei = mei.Replace("\\", "\\\\")
                .Replace("`", "\\`")
                .Replace("$", "\\$")
                .Replace("\r", "")
                .Replace("\n", "\\n");

            return html.Replace("{{MEI_DATA}}", escapedMei);
        }

        [RelayCommand]
        private void AddMusic()
        {
            string baseTitle = "Untitled";
            string newTitle = baseTitle;

            int i = 1;
            while(Musics.Select(n => n.Title).Contains(newTitle))
            {
                newTitle = $"{baseTitle}{i++}";
            }

            var newMusic = new MusicItem { Title = newTitle, Content = "" };

            Musics.Add(newMusic);
            SelectedMusic = newMusic;
        }

        [RelayCommand]
        private void DeleteMusic()
        {
            if(SelectedMusic != null)
            {
                Musics.Remove(SelectedMusic);
            }
        }

        [RelayCommand]
        public async Task RenameMusicAsync(XamlRoot xamlRoot)
        {
            if(SelectedMusic == null || xamlRoot == null)
            {
                return;
            }

            var inputDialog = new ContentDialog
            {
                Title = "Rename Music",
                Content = new TextBox
                {
                    Text = SelectedMusic.Title,
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
                    SelectedMusic.Title = newTitle;
                }
            }
        }
    }
}
