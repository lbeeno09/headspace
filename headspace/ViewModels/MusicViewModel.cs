using headspace.Models;
using headspace.Services.Interfaces;
using headspace.ViewModels.Common;
using Microsoft.UI.Xaml;
using System.Linq;
using System.Threading.Tasks;

namespace headspace.ViewModels
{
    public class MusicViewModel : ViewModelBase<MusicModel>
    {
        private readonly IProjectService _projectService;
        private readonly IDialogService _dialogService;

        public XamlRoot? ViewXamlRoot { get; set; }

        public MusicViewModel(IDialogService dialogService, IProjectService projectService)
        {
            _dialogService = dialogService;
            _projectService = projectService;

            Items = _projectService.CurrentProject.Musics;
        }

        protected override void Add()
        {
            string exampleAbc = @"X:1
T:Example Scale
M:4/4
K:C
C D E F | G A B c";

            var newMusic = new MusicModel { Title = $"New Music {Items.Count + 1}", Content = exampleAbc };

            Items.Add(newMusic);
            SelectedItem = newMusic;
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
            foreach(var music in Items.Where(i => i.IsDirty))
            {
                await _projectService.SaveItemAsync(music);
            }
        }
    }
}
