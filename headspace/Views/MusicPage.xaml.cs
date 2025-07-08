using headspace.Models;
using headspace.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.Web.WebView2.Core;
using System;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using Windows.System;

namespace headspace.Views
{
    public sealed partial class MusicPage : Page
    {
        public MusicViewModel ViewModel { get; }
        private bool _isWebViewReady = false;

        public MusicPage()
        {
            this.InitializeComponent();

            ViewModel = ((App)Application.Current).Services.GetRequiredService<MusicViewModel>();
            this.Loaded += (s, e) =>
            {
                ViewModel.ViewXamlRoot = this.XamlRoot;
            };

            string htmlPath = Path.Combine(AppContext.BaseDirectory, "Assets", "MusicRenderer", "renderer.html");
            MusicWebView.Source = new Uri(htmlPath);

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            MusicWebView.NavigationCompleted += MusicWebView_NavigationCompleted;
        }

        private void MusicWebView_NavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            _isWebViewReady = true;

            UpdateMusicPreview();
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(ViewModel.SelectedItem))
            {
                if(ViewModel.SelectedItem != null)
                {
                    ViewModel.SelectedItem.PropertyChanged += SelectedItem_PropertyChanged;
                }

                UpdateMusicPreview();
            }
        }

        private void SelectedItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(MusicModel.Content))
            {
                UpdateMusicPreview();
            }
        }

        private async void UpdateMusicPreview()
        {
            if(!_isWebViewReady || ViewModel.SelectedItem == null || MusicWebView.CoreWebView2 == null)
            {
                return;
            }

            string abcString = ViewModel.SelectedItem.Content ?? "";
            string jsonString = JsonSerializer.Serialize(abcString);
            string script = $"renderAbc({jsonString})";

            await MusicWebView.CoreWebView2.ExecuteScriptAsync(script);
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
