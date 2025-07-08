using headspace.Models;
using headspace.Services.Interfaces;
using headspace.ViewModels.Common;
using Microsoft.UI.Xaml;
using System.Linq;
using System.Threading.Tasks;

namespace headspace.ViewModels
{
    public class ScreenplayViewModel : ViewModelBase<ScreenplayModel>
    {
        private readonly IProjectService _projectService;
        private readonly IDialogService _dialogService;

        public XamlRoot? ViewXamlRoot { get; set; }

        public ScreenplayViewModel(IDialogService dialogService, IProjectService projectService)
        {
            _projectService = projectService;
            _dialogService = dialogService;

            Items = _projectService.CurrentProject.Screenplays;
        }

        protected override void Add()
        {
            var newScreenplay = new ScreenplayModel { Title = $"New Screenplay {Items.Count + 1}", Content = @"" };

            Items.Add(newScreenplay);
            SelectedItem = newScreenplay;
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
            foreach(var screenplay in Items.Where(i => i.IsDirty))
            {
                await _projectService.SaveItemAsync(screenplay);
            }
        }
    }
}
