using headspace.Views;
using Microsoft.UI.Xaml;

namespace headspace
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            RootFrame.Navigate(typeof(MainViewPage));
        }
    }
}
