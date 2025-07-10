using headspace.Views;
using Microsoft.UI.Xaml;

namespace headspace
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            ExtendsContentIntoTitleBar = true;
            SetTitleBar(null);

            RootFrame.Navigate(typeof(MainViewPage));
        }
    }
}
