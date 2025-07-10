using headspace.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.Web.WebView2.Core;
using System;
using System.ComponentModel;
using Windows.System;

namespace headspace.Views
{
    public sealed partial class NotePage : Page
    {
        public NoteViewModel ViewModel { get; }
        private bool _isWebViewReady = false;

        public NotePage()
        {
            this.InitializeComponent();

            ViewModel = ((App)Application.Current).Services.GetRequiredService<NoteViewModel>();
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            this.Loaded += (s, e) =>
            {
                ViewModel.ViewXamlRoot = this.XamlRoot;
            };

            NoteWebView.NavigationCompleted += NoteWebView_NavigationCompleted;
        }

        private void NoteWebView_NavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            _isWebViewReady = true;

            UpdatePreview();
        }


        private async void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName is nameof(ViewModel.SelectedItem) or "SelectedItem.Content")
            {
                UpdatePreview();
            }
        }

        private async void UpdatePreview()
        {
            if(ViewModel.SelectedItem == null)
            {
                return;
            }

            // 1. Get markdown string from model
            var markdownText = ViewModel.SelectedItem.Content ?? string.Empty;

            // 2. Convert markdown to HTML with Markdig
            var htmlText = Markdig.Markdown.ToHtml(markdownText);

            // 3. Load HTML to WebView2
            await NoteWebView.EnsureCoreWebView2Async();
            NoteWebView.NavigateToString(htmlText);
        }

        private void Editor_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == VirtualKey.Tab)
            {
                var editor = sender as TextBox;
                var cursorPosition = editor.SelectionStart;

                editor.Text = editor.Text.Insert(cursorPosition, "\t");
                editor.SelectionStart = cursorPosition + 1;

                e.Handled = true;
            }
        }
    }
}
