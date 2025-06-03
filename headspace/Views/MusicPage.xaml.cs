using headspace.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.ComponentModel;

namespace headspace.Views
{
    public sealed partial class MusicPage : Page
    {
        private bool isPreviewReady = false;

        public MusicPage()
        {
            this.InitializeComponent();
            this.Loaded += Page_Loaded;

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await MusicWebView.EnsureCoreWebView2Async();

            UpdatePreview(ViewModel.HtmlContent);
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(ViewModel.HtmlContent))
            {
                UpdatePreview(ViewModel.HtmlContent);
            }
        }

        private async void UpdatePreview(string html)
        {
            if(MusicWebView.CoreWebView2 != null)
            {
                MusicWebView.NavigateToString(html);
            }
        }

        private async void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if(textBox != null && ViewModel.SelectedMusic != null)
            {
                UpdatePreview(textBox.Text);
            }
        }

        private void RenameButton_Click(object sender, RoutedEventArgs e)
        {
            if(DataContext is MusicViewModel viewModel)
            {
                _ = viewModel.RenameMusicAsync(this.XamlRoot);
            }
        }

    }
}
