using CommunityToolkit.Mvvm.ComponentModel;
using headspace.Models;
using headspace.Services.Interfaces;
using headspace.ViewModels.Common;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;

namespace headspace.ViewModels
{
    public partial class MoodboardViewModel : ViewModelBase<MoodboardModel>
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


        public MoodboardViewModel(IDialogService dialogService, IProjectService projectService, IFilePickerService filePickerService, ICanvasExportService canvasExportService)
        {
            _dialogService = dialogService;
            _filePickerService = filePickerService;
            _canvasExportService = canvasExportService;
            _projectService = projectService;

            Items = _projectService.CurrentProject.Moodboards;
        }

        protected override void Add()
        {
            var newMoodboard = new MoodboardModel { Title = $"New Moodboard {Items.Count + 1}" };

            Items.Add(newMoodboard);
            SelectedItem = newMoodboard;
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
            foreach(var moodboard in Items.Where(i => i.IsDirty))
            {
                await _projectService.SaveItemAsync(moodboard);
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
