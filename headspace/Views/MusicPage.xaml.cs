using CommunityToolkit.Mvvm.Messaging;
using headspace.Messages;
using headspace.Models;
using headspace.Services.Interfaces;
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
        private readonly IFilePickerService _filePickerService;

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

            _filePickerService = ((App)Application.Current).Services.GetRequiredService<IFilePickerService>();

            MusicWebView.CoreWebView2Initialized += (s, e) =>
            {
                s.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
            };

            string htmlPath = Path.Combine(AppContext.BaseDirectory, "Assets", "MusicRenderer", "renderer.html");
            MusicWebView.Source = new Uri(htmlPath);

            ((App)Application.Current).Services.GetRequiredService<IMessenger>()
                .Register<ExportMusicAsSvgMessage>(this, (recipient, message) =>
                {
                    TriggerExport();
                });

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

            // call function to render output
            string outputScript = $"renderAbc({jsonString})";
            await MusicWebView.CoreWebView2.ExecuteScriptAsync(outputScript);
        }

        private async void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if(MusicWebView.CoreWebView2 == null || ViewModel.SelectedItem == null)
            {
                return;
            }

            // 1. get abc text from viewmodel
            string abcString = ViewModel.SelectedItem.Content ?? "";
            string jsonString = JsonSerializer.Serialize(abcString);

            // 2. call play function
            string playScript = $"play({jsonString})";
            await MusicWebView.CoreWebView2.ExecuteScriptAsync(playScript);
        }

        private async void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if(MusicWebView.CoreWebView2 != null)
            {
                await MusicWebView.CoreWebView2.ExecuteScriptAsync("stop();");
            }
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

        private async void TriggerExport()
        {
            if(MusicWebView.CoreWebView2 != null)
            {
                await MusicWebView.CoreWebView2.ExecuteScriptAsync("exportSvg();");
            }
        }

        private async void CoreWebView2_WebMessageReceived(CoreWebView2 sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            string svgContent = args.TryGetWebMessageAsString();

            var path = await _filePickerService.PickSaveFileAsync_Svg(ViewModel.SelectedItem.Title);
            if(string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            await File.WriteAllTextAsync(path, svgContent);
        }
    }
}
