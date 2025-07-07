using headspace.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.ComponentModel;
using System.IO;
using System.Text.Json;

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
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            this.Loaded += (s, e) =>
            {
                ViewModel.ViewXamlRoot = this.XamlRoot;
            };

            string htmlPath = Path.Combine(AppContext.BaseDirectory, "Assets", "MusicRenderer", "renderer.html");
            MusicWebView.Source = new Uri(htmlPath);

            MusicWebView.NavigationCompleted += MusicWebView_NavigationCompleted;
        }

        private void MusicWebView_NavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            _isWebViewReady = true;

            UpdateMusicPreview();
        }

        private async void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName is nameof(ViewModel.SelectedItem) or "SelectedItem.Content")
            {
                UpdateMusicPreview();
            }
        }

        private async void UpdateMusicPreview()
        {
            if(ViewModel.SelectedItem == null)
            {
                return;
            }

            string abcString = ViewModel.SelectedItem.Content ?? "";
            string jsonString = JsonSerializer.Serialize(abcString);
            string script = $"renderAbc({jsonString})";

            await MusicWebView.CoreWebView2.ExecuteScriptAsync(script);
        }
    }
}
