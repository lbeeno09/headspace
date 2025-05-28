using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace headspace.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ScreenplayPage : Page
    {
        public ScreenplayPage()
        {
            InitializeComponent();
        }

        private void BoldButton_Click(object sender, RoutedEventArgs e)
        {
            Editor.Document.Selection.CharacterFormat.Bold = BoldButton.IsChecked == true ? FormatEffect.On : FormatEffect.Off;
        }

        private void ItalicButton_Click(object sender, RoutedEventArgs e)
        {
            Editor.Document.Selection.CharacterFormat.Italic = ItalicButton.IsChecked == true ? FormatEffect.On : FormatEffect.Off;
        }

        private void UnderlineButton_Click(object sender, RoutedEventArgs e)
        {
            Editor.Document.Selection.CharacterFormat.Underline = UnderlineButton.IsChecked == true ? UnderlineType.Single : UnderlineType.None;
        }

        private void Editor_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Tab)
            {
                Editor.Document.Selection.TypeText("\t");
                e.Handled = true;
            }
        }
    }
}
