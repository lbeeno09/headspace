using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.System;

namespace headspace.Views
{
    public sealed partial class DocumentsPage : Page
    {
        public DocumentsPage()
        {
            this.InitializeComponent();
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
            if(e.Key == VirtualKey.Tab)
            {
                Editor.Document.Selection.TypeText("\t");
                e.Handled = true;
            }
        }
    }
}
