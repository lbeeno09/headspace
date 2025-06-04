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
    public partial class DrawingViewModel : ObservableObject
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

        public ObservableCollection<DrawingItem> Drawings { get; } = new();

        [ObservableProperty]
        private DrawingItem? selectedDrawing;

        [ObservableProperty]
        private string selectedPrimaryColor;
        [ObservableProperty]
        private string selectedSecondaryColor;

        [ObservableProperty]
        private double selectedThickness;

        [ObservableProperty]
        private bool isEraserMode;

        public DrawingViewModel()
        {
            SelectedPrimaryColor = "Black";
            SelectedSecondaryColor = "White";
            SelectedThickness = 2.0;
            IsEraserMode = false;
        }
        
        [RelayCommand]
        private void AddDrawing()
        {
            string baseTitle = "Untitled";
            string newTitle = baseTitle;

            int i = 1;
            while(Drawings.Select(n => n.Title).Contains(newTitle))
            {
                newTitle = $"{baseTitle}{i++}";
            }

            var newDrawing = new DrawingItem { Title = newTitle };

            Drawings.Add(newDrawing);
            SelectedDrawing = newDrawing;
        }

        [RelayCommand]
        private void DeleteDrawing()
        {
            if(SelectedDrawing != null)
            {
                Drawings.Remove(SelectedDrawing);
            }
        }

        [RelayCommand]
        public async Task RenameDrawingAsync(XamlRoot xamlRoot)
        {
            if(SelectedDrawing == null || xamlRoot == null)
            {
                return;
            }

            var inputDialog = new ContentDialog
            {
                Title = "Rename Drawing",
                Content = new TextBox
                {
                    Text = SelectedDrawing.Title,
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
                    SelectedDrawing.Title = newTitle;
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
