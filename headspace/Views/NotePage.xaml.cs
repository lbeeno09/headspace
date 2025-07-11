using headspace.Models;
using headspace.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Dispatching;
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

        private readonly Microsoft.UI.Dispatching.DispatcherQueueTimer _debounceTimer;

        public NotePage()
        {
            this.InitializeComponent();

            ViewModel = ((App)Application.Current).Services.GetRequiredService<NoteViewModel>();
            this.Loaded += (s, e) =>
            {
                ViewModel.ViewXamlRoot = this.XamlRoot;
            };

            _debounceTimer = this.DispatcherQueue.CreateTimer();
            _debounceTimer.Interval = TimeSpan.FromMilliseconds(500);
            _debounceTimer.IsRepeating = false;
            _debounceTimer.Tick += (s, e) => UpdatePreview();

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private async void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(ViewModel.SelectedItem))
            {
                if(ViewModel.SelectedItem != null)
                {
                    ViewModel.SelectedItem.PropertyChanged += SelectedItem_PropertyChanged;
                }
                UpdatePreview();
            }
        }

        private void SelectedItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(NoteModel.Content))
            {
                _debounceTimer.Stop();
                _debounceTimer.Start();

                UpdatePreview();
            }
        }

        private async void UpdatePreview()
        {
            // 1. Get markdown string from model
            var markdownText = ViewModel.SelectedItem?.Content ?? string.Empty;

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
