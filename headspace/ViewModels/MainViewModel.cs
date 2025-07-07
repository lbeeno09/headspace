using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using headspace.Services.Interfaces;
using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;

namespace headspace.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;
        private readonly IProjectService _projectService;

        public MainViewModel(INavigationService navigationService, IProjectService projectService)
        {
            _navigationService = navigationService;
            _projectService = projectService;
        }

        [RelayCommand]
        private void Navigate(NavigationViewItemInvokedEventArgs args)
        {
            if(args.IsSettingsInvoked)
            {
                _navigationService.NavigateTo("PreferencePage");
            }
            else if(args.InvokedItemContainer?.Tag is string pageTag)
            {
                _navigationService.NavigateTo(pageTag);
            }
        }

        [RelayCommand]
        private void NewProject()
        {
            // TODO: Check for unsaved changes
            _projectService.CreateNewProject();
        }

        [RelayCommand]
        private async Task SaveProject()
        {
            await _projectService.SaveProjectAsync();
        }

        [RelayCommand]
        private async Task SaveProjectAs()
        {
            await _projectService.SaveProjectAsAsync();
        }

        [RelayCommand]
        private async Task OpenProject()
        {
            // TODO: Check for unsaved changes
            await _projectService.LoadProjectAsync();
        }
    }
}
