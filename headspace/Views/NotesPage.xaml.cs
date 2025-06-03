using headspace.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.ComponentModel;

namespace headspace.Views
{
    public sealed partial class NotesPage : Page
    {
        public NotesPage()
        {
            this.InitializeComponent();
            this.Loaded += Page_Loaded;

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await MarkdownPreview.EnsureCoreWebView2Async();

            ViewModel.UpdateMarkdown(ViewModel.SelectedNote?.Content ?? "");
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(ViewModel.MarkdownHtml))
            {
                if(MarkdownPreview != null && MarkdownPreview.CoreWebView2 != null)
                {
                    MarkdownPreview.NavigateToString(ViewModel.MarkdownHtml);
                }
            }
        }

        private async void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if(textBox != null && ViewModel.SelectedNote != null)
            {
                ViewModel.UpdateMarkdown(textBox.Text);
            }
        }

        private void RenameButton_Click(object sender, RoutedEventArgs e)
        {
            if(DataContext is NoteViewModel viewModel)
            {
                _ = viewModel.RenameNoteAsync(this.XamlRoot);
            }
        }
    }
}
