using headspace.Models;
using headspace.Services.Interfaces;
using headspace.ViewModels.Common;
using Microsoft.UI.Xaml;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace headspace.ViewModels
{
    public partial class NoteViewModel : ViewModelBase<NoteModel>
    {
        private readonly IProjectService _projectService;
        private readonly IFilePickerService _filePickerService;
        private readonly IDialogService _dialogService;

        public XamlRoot? ViewXamlRoot { get; set; }

        public NoteViewModel(IProjectService projectService, IDialogService dialogService, IFilePickerService filePickerService)
        {
            _projectService = projectService;
            _filePickerService = filePickerService;
            _dialogService = dialogService;

            // TODO: When navigating to this page, preserve the selected item to somewhere
            Items = _projectService.CurrentProject.Notes;
            // TEMPORARY: Will choose the first/default one
            SelectedItem = Items.FirstOrDefault();
        }

        protected override void Add()
        {
            var newNote = new NoteModel { Title = $"New Note {Items.Count + 1}", Content = "" };

            Items.Add(newNote);
            SelectedItem = newNote;
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
                return;
            }

            Debug.WriteLine("Saving Note's particular item");

            await _projectService.SaveItemAsync(SelectedItem);
        }

        protected override async Task SaveAll()
        {
            foreach(var note in Items.Where(i => i.IsDirty))
            {
                await _projectService.SaveItemAsync(note);
            }
        }

        protected override async Task Export()
        {
            var path = await _filePickerService.PickSaveFileAsync_Markdown(SelectedItem.Title);
            if(string.IsNullOrEmpty(path))
            {
                return;
            }

            await File.WriteAllTextAsync(path, SelectedItem.Content);
        }
    }
}
