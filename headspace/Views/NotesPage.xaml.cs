using headspace.ViewModels;
using Markdig;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.ComponentModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace headspace.Views
{
    public sealed partial class NotesPage : Page
    {
        public NotesViewModel ViewModel => (NotesViewModel)DataContext;

        public NotesPage()
        {
            this.InitializeComponent();
            this.Loaded += NotesPage_Loaded;
        }

        private async void NotesPage_Loaded(object sender, RoutedEventArgs e)
        {
            await MarkdownPreview.EnsureCoreWebView2Async();

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;

            if(ViewModel.SelectedNote != null)
            {
                ViewModel.SelectedNote.PropertyChanged += SelectedNote_PropertyChanged;
            }
            UpdateMarkdownPreview(ViewModel.SelectedNote.Content);
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(ViewModel.SelectedNote))
            {
                if(ViewModel.SelectedNote != null)
                {
                    ViewModel.SelectedNote.PropertyChanged += SelectedNote_PropertyChanged;
                    UpdateMarkdownPreview(ViewModel.SelectedNote.Content);

                }
            }
        }

        private void SelectedNote_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(ViewModel.SelectedNote.Content))
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    UpdateMarkdownPreview(ViewModel.SelectedNote.Content);
                });
            }
        }

        private void UpdateMarkdownPreview(string markdown)
        {
            if(MarkdownPreview?.CoreWebView2 == null || markdown == null)
            {
                return;
            }

            var html = Markdown.ToHtml(markdown ?? string.Empty);
            MarkdownPreview.NavigateToString($"""
                <html>
                    <head>
                        <meta charset="UTF-8">
                    </head>
                    <body>
                        {html}
                    </body>
                </html>
            """);
        }
    }
}
