using CommunityToolkit.Mvvm.ComponentModel;

namespace headspace.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        [ObservableProperty]
        private string projectTitle;

        public HomeViewModel()
        {
            UpdateProjectTitle();

            System.Diagnostics.Debug.WriteLine("HomeViewModel initialized");
        }

        public void RefreshProjectTitle()
        {
            UpdateProjectTitle();
        }

        private void UpdateProjectTitle()
        {
            var appInstance = App.Current as App;
            if(appInstance != null && appInstance.CurrentProject != null)
            {
                ProjectTitle = appInstance.CurrentProject.ProjectName;
            }
            else
            {
                ProjectTitle = "No Project Loaded";
            }
        }
    }
}
