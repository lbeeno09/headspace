using headspace.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace headspace
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            ContentFrame.Navigate(typeof(HomePage));
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            RootNavigationView.IsPaneOpen = false;

            ContentFrame.Navigate(typeof(HomePage));
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            string pageName = args.InvokedItemContainer?.Tag?.ToString();
            if(pageName is null)
            {
                return;
            }

            switch(pageName)
            {
                case "HomePage":
                    ContentFrame.Navigate(typeof(HomePage));
                    break;
                case "NotesPage":
                    ContentFrame.Navigate(typeof(NotesPage));
                    break;
                case "DocumentsPage":
                    ContentFrame.Navigate(typeof(DocumentsPage));
                    break;
                case "ScreenplayPage":
                    ContentFrame.Navigate(typeof(ScreenplayPage));
                    break;
                case "DrawingsPage":
                    ContentFrame.Navigate(typeof(DrawingsPage));
                    break;
                case "MoodboardPage":
                    ContentFrame.Navigate(typeof(MoodboardPage));
                    break;
                case "StoryboardPage":
                    ContentFrame.Navigate(typeof(StoryboardPage));
                    break;
                case "MusicPage":
                    ContentFrame.Navigate(typeof(MusicPage));
                    break;
            }

            RootNavigationView.IsPaneOpen = false;
        }
    }
}
