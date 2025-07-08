using headspace.Models;
using headspace.Services.Interfaces;
using headspace.ViewModels.Common;
using Microsoft.UI.Xaml;
using System.Linq;
using System.Threading.Tasks;

namespace headspace.ViewModels
{
    public partial class DocumentViewModel : ViewModelBase<DocumentModel>
    {
        private readonly IDialogService _dialogService;
        private readonly IProjectService _projectService;

        public XamlRoot? ViewXamlRoot { get; set; }

        public DocumentViewModel(IDialogService dialogService, IProjectService projectService)
        {
            _dialogService = dialogService;
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
    }
}
