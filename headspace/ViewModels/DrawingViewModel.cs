using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using headspace.Models;
using headspace.Models.Common;
using headspace.Services.Interfaces;
using headspace.ViewModels.Common;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using System.Diagnostics;
using System.Linq;
using Windows.UI;

namespace headspace.ViewModels
{
    public partial class DrawingViewModel : ViewModelBase<DrawingModel>
    {
        private readonly IDialogService _dialogService;
        public XamlRoot? ViewXamlRoot { get; set; }

        [ObservableProperty]
        private Color _primaryColor = Colors.Black;

        [ObservableProperty]
        private Color _secondaryColor = Colors.White;

        [ObservableProperty]
        private float _strokeThickness = 2.0f;

        [ObservableProperty]
        private bool _isEraserMode = false;

        [ObservableProperty]
        private LayerModel? _activeLayer;

        public DrawingViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        [RelayCommand]
        private void AddLayer()
        {
            if(SelectedItem == null)
            {
                return;
            }

            var newLayer = new LayerModel { Name = $"Layer {SelectedItem.Layers.Count + 1}" };
            SelectedItem.Layers.Add(newLayer);
            ActiveLayer = newLayer;
        }

        [RelayCommand]
        private void DeleteLayer()
        {
            if(SelectedItem == null || ActiveLayer == null || SelectedItem.Layers.Count <= 1)
            {
                return;
            }

            SelectedItem.Layers.Remove(ActiveLayer);
            ActiveLayer = SelectedItem.Layers.FirstOrDefault();
        }

        protected override void Add()
        {
            var newDrawing = new DrawingModel { Title = $"New Drawing {Items.Count + 1}" };
            var initialLayer = new LayerModel { Name = "Base Layer" };
            newDrawing.Layers.Add(initialLayer);

            Items.Add(newDrawing);
            SelectedItem = newDrawing;
            ActiveLayer = initialLayer;
        }

        protected override async void Rename()
        {
            if(SelectedItem == null || ViewXamlRoot == null)
            {
                return;
            }

            var newName = await _dialogService.ShowRenameDialogAsync(SelectedItem.Title, ViewXamlRoot);
            if(!string.IsNullOrWhiteSpace(newName))
            {
                SelectedItem.Title = newName;
            }
        }

        protected override void Delete()
        {
            if(SelectedItem == null)
            {
                return;
            }

            Items.Remove(SelectedItem);
            SelectedItem = Items.FirstOrDefault();
        }

        protected override void Save()
        {
            if(SelectedItem == null)
            {
                return;
            }

            Debug.WriteLine($"SAVING ITEM: {SelectedItem.Title}");
        }

        protected override void SaveAll()
        {
            Debug.WriteLine("SAVING ALL ITEMS...");
            if(Items.Count == 0)
            {
                Debug.WriteLine("No items to save.");
                return;
            }

            foreach(var note in Items)
            {
                Debug.WriteLine($" -> Saving: {note.Title}");
            }
            Debug.WriteLine("...DONE");
        }
    }
}
