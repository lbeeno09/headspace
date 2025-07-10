using headspace.Services.Implementations;
using headspace.Services.Interfaces;
using headspace.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Linq;

namespace headspace.Views
{
    public sealed partial class MainViewPage : Page
    {
        public MainViewModel ViewModel { get; }

        public MainViewPage()
        {
            this.InitializeComponent();

            // Get ViewModel from DI Container
            ViewModel = ((App)Application.Current).Services.GetRequiredService<MainViewModel>();

            this.Loaded += OnMainViewPageLoaded;
        }

        private void OnMainViewPageLoaded(object sender, RoutedEventArgs e)
        {
            // Configure navigation to use this page's frame
            var navigationService = ((App)Application.Current).Services.GetRequiredService<INavigationService>() as NavigationService;
            navigationService?.Initialize(ContentFrame);

            navigationService?.NavigateTo("HomePage");
            SidebarView.SelectedItem = SidebarView.MenuItems.OfType<NavigationViewItem>().FirstOrDefault();

            // Set custom titlebar
            App.MainWindow.SetTitleBar(AppMenuBar);
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            ViewModel.NavigateCommand.Execute(args);
        }
    }
}
