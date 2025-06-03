using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using headspace.Models;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;

namespace headspace.ViewModels
{
    public partial class MoodboardViewModel : ObservableObject
    {
        public List<NamedColor> ColorOptions { get; } = new()
        {
            new NamedColor { Name = "Red", Color = Colors.Red },
            new NamedColor { Name = "Orange", Color = Colors.Orange },
            new NamedColor { Name = "Yellow", Color = Colors.Yellow },
            new NamedColor { Name = "Green", Color = Colors.Green },
            new NamedColor { Name = "Blue", Color = Colors.Blue },
            new NamedColor { Name = "Indigo", Color = Colors.Indigo },
            new NamedColor { Name = "Violet", Color = Colors.Violet },
            new NamedColor { Name = "Black", Color = Colors.Black },
            new NamedColor { Name = "White", Color = Colors.White }
        };

        public ObservableCollection<MoodboardItem> Moodboards { get; } = new();

        [ObservableProperty]
        private MoodboardItem selectedMoodboard;

        [ObservableProperty]
        private Color selectedColor = Colors.Black;

        [ObservableProperty]
        private NamedColor selectedNamedColor;

        [ObservableProperty]
        private double selectedThickness = 2.0;

        [ObservableProperty]
        private bool isEraserMode;

        public MoodboardViewModel()
        {
            SelectedNamedColor = ColorOptions[0];
        }

        [RelayCommand]
        private void AddMoodboard()
        {
            string baseTitle = "Untitled";
            string newTitle = baseTitle;

            int i = 1;
            while(Moodboards.Select(n => n.Title).Contains(newTitle))
            {
                newTitle = $"{baseTitle}{i++}";
            }

            var newMoodboard = new MoodboardItem { Title = newTitle };

            Moodboards.Add(newMoodboard);
            SelectedMoodboard = newMoodboard;
        }

        [RelayCommand]
        private void DeleteMoodboard()
        {
            if(SelectedMoodboard != null)
            {
                Moodboards.Remove(SelectedMoodboard);
            }
        }

        [RelayCommand]
        public async Task RenameMoodboardAsync(XamlRoot xamlRoot)
        {
            if(SelectedMoodboard == null || xamlRoot == null)
            {
                return;
            }

            var inputDialog = new ContentDialog
            {
                Title = "Rename Moodboard",
                Content = new TextBox
                {
                    Text = SelectedMoodboard.Title,
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
                    SelectedMoodboard.Title = newTitle;
                }
            }
        }

        [RelayCommand]
        private void ToggleErase()
        {
            IsEraserMode = !IsEraserMode;
        }
    }
}
