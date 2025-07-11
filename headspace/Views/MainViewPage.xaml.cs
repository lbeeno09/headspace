using headspace.Services.Implementations;
using headspace.Services.Interfaces;
using headspace.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Linq;
using Windows.System;
using Windows.UI.Core;

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

            this.Focus(FocusState.Programmatic);
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            ViewModel.NavigateCommand.Execute(args);
        }

        private void MainViewPage_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            var ctrlState = InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control);
            bool isCtrlDown = ctrlState.HasFlag(CoreVirtualKeyStates.Down);

            if(isCtrlDown && e.Key == VirtualKey.Tab)
            {
                e.Handled = true;

                var shiftState = InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift);
                bool isShiftDown = shiftState.HasFlag(CoreVirtualKeyStates.Down);

                int totalItems = SidebarView.MenuItems.Count; // we have something
                int currentIndex = SidebarView.MenuItems.IndexOf(SidebarView.SelectedItem);
                if(currentIndex < 0)
                {
                    currentIndex = 0;
                }

                int nextIndex;
                if(isShiftDown)
                {
                    nextIndex = (currentIndex - 1 + totalItems) % totalItems;
                }
                else
                {
                    nextIndex = (currentIndex + 1) % totalItems;
                }

                if(SidebarView.MenuItems[nextIndex] is NavigationViewItem nextItem)
                {
                    SidebarView.SelectedItem = nextItem;
                    if(nextItem.Tag is string pageTag)
                    {
                        var navigationService = ((App)Application.Current).Services.GetRequiredService<INavigationService>();
                        navigationService.NavigateTo(pageTag);
                    }
                }
            }
        }
    }
}
