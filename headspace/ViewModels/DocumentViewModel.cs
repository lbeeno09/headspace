using headspace.Models;
using headspace.Services.Interfaces;
using headspace.ViewModels.Common;
using Microsoft.UI.Xaml;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace headspace.ViewModels
{
    public partial class DocumentViewModel : ViewModelBase<DocumentModel>
    {
        private readonly IDialogService _dialogService;
        private readonly IFilePickerService _filePickerService;
        private readonly IProjectService _projectService;

        public XamlRoot? ViewXamlRoot { get; set; }

        public DocumentViewModel(IDialogService dialogService, IProjectService projectService, IFilePickerService filePickerService)
        {
            _dialogService = dialogService;
            _filePickerService = filePickerService;
            _projectService = projectService;

            Items = _projectService.CurrentProject.Documents;
        }

        protected override void Add()
        {
            var newDoc = new DocumentModel { Title = $"New Document {Items.Count + 1}", Content = @"" };

            Items.Add(newDoc);
            SelectedItem = newDoc;
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
            foreach(var document in Items.Where(i => i.IsDirty))
            {
                await _projectService.SaveItemAsync(document);
            }
        }

        protected override async Task Export()
        {
            var path = await _filePickerService.PickSaveFileAsync_Rtf(SelectedItem.Title);
            if(string.IsNullOrEmpty(path))
            {
                return;
            }

            await File.WriteAllTextAsync(path, SelectedItem.Content);
        }
    }
}
