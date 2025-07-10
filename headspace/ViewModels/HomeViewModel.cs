using CommunityToolkit.Mvvm.ComponentModel;
using headspace.Services.Interfaces;

namespace headspace.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? _projectName;

        public HomeViewModel(IProjectService projectService)
        {
            ProjectName = projectService.CurrentProject.ProjectName;
        }
    }
}
