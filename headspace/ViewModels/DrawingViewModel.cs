using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using headspace.Models;
using headspace.Models.Common;
using headspace.Services.Interfaces;
using headspace.ViewModels.Common;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;

namespace headspace.ViewModels
{
    public partial class DrawingViewModel : ViewModelBase<DrawingModel>
    {
        private readonly IProjectService _projectService;
        private readonly IFilePickerService _filePickerService;
        private readonly ICanvasExportService _canvasExportService;
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

        public DrawingViewModel(IDialogService dialogService, IProjectService projectService, IFilePickerService filePickerService, ICanvasExportService canvasExportService)
        {
            _dialogService = dialogService;
            _filePickerService = filePickerService;
            _canvasExportService = canvasExportService;
            _projectService = projectService;

            Items = _projectService.CurrentProject.Drawings;
        }

        [RelayCommand]
        private void AddLayer()
        {
            if(SelectedItem == null)
            {
                return;
            }

            var newLayer = new LayerModel { Name = $"Layer {SelectedItem.Layers.Count}" };
            SelectedItem.Layers.Insert(0, newLayer);
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

        [RelayCommand]
        private void MoveLayerUp()
        {
            if(ActiveLayer == null || SelectedItem == null)
            {
                return;
            }

            int index = SelectedItem.Layers.IndexOf(ActiveLayer);
            if(index > 0)
            {
                SelectedItem.Layers.Move(index, index - 1);
            }
        }

        [RelayCommand]
        private void MoveLayerDown()
        {
            if(ActiveLayer == null || SelectedItem == null)
            {
                return;
            }

            int index = SelectedItem.Layers.IndexOf(ActiveLayer);
            if(index < SelectedItem.Layers.Count - 1)
            {
                SelectedItem.Layers.Move(index, index + 1);
            }
        }

        protected override void Add()
        {
            var newDrawing = new DrawingModel { Title = $"New Drawing {Items.Count + 1}" };
            var initialLayer = new LayerModel { Name = "Layer 0" };

            Items.Add(newDrawing);
            newDrawing.Layers.Add(initialLayer);

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

        protected override async Task Save()
        {
            if(SelectedItem == null)
            {
                await _projectService.SaveItemAsync(SelectedItem);
            }
        }

        protected override async Task SaveAll()
        {
            foreach(var drawing in Items.Where(i => i.IsDirty))
            {
                await _projectService.SaveItemAsync(drawing);
            }
        }

        protected override async Task Export()
        {
            var path = await _filePickerService.PickSaveFileAsync_Png(SelectedItem.Title);
            if(string.IsNullOrEmpty(path))
            {
                return;
            }

            await _canvasExportService.ExportAsPngAsync(SelectedItem, path);
        }
    }
}
