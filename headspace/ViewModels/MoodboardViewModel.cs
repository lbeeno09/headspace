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
        public Dictionary<string, Color> ColorOptions { get; } = new()
        {
            { "Red", Colors.Red },
            { "Orange", Colors.Orange },
            { "Yellow",Colors.Yellow },
            { "Green", Colors.Green },
            { "Blue", Colors.Blue },
            { "Indigo", Colors.Indigo },
            { "Violet", Colors.Violet },
            { "Black", Colors.Black },
            { "White", Colors.White }
        };

        public ObservableCollection<MoodboardItem> Moodboards { get; } = new();

        [ObservableProperty]
        private MoodboardItem? selectedMoodboard;

        [ObservableProperty]
        private string selectedPrimaryColor;
        [ObservableProperty]
        private string selectedSecondaryColor;

        [ObservableProperty]
        private double selectedThickness;

        [ObservableProperty]
        private bool isEraserMode;

        public MoodboardViewModel()
        {
            SelectedPrimaryColor = "Black";
            SelectedSecondaryColor = "White";
            SelectedThickness = 2.0;
            IsEraserMode = false;
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
