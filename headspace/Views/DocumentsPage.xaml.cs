using headspace.ViewModels;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.ComponentModel;
using Windows.System;

namespace headspace.Views
{
    public sealed partial class DocumentsPage : Page
    {
        public DocumentsPage()
        {
            this.InitializeComponent();
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
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

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(ViewModel.SelectedDocument))
            {
                if(ViewModel.SelectedDocument != null)
                {
                    Editor.Document.SetText(TextSetOptions.FormatRtf, ViewModel.SelectedDocument.Content);
                }
            }
        }

        private void RenameButton_Click(object sender, RoutedEventArgs e)
        {
            if(DataContext is DocumentViewModel viewModel)
            {
                _ = viewModel.RenameDocumentAsync(this.XamlRoot);
            }
        }
    }
}
