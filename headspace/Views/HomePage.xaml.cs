using headspace.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;


namespace headspace.Views
{
    public sealed partial class HomePage : Page
    {
        private HomeViewModel ViewModel => DataContext as HomeViewModel;

        public HomePage()
        {
            this.InitializeComponent();
            this.DataContext = new HomeViewModel();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if(ViewModel != null)
            {
                ViewModel.RefreshProjectTitle();
            }
        }
    }
}
