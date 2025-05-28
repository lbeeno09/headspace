using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using System.Threading.Tasks;

namespace headspace.Views
{
    public sealed partial class MusicPage : Page
    {
        private string meiData;

        public MusicPage()
        {
            this.InitializeComponent();

            LoadWebViewAsync();
        }

        private async Task LoadWebViewAsync()
        {
            await MusicWebView.EnsureCoreWebView2Async();
            MusicWebView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                "appassets",
                Path.Combine(Directory.GetCurrentDirectory(), "Assets/js/verovio"),
                CoreWebView2HostResourceAccessKind.Allow);

            MusicWebView.Source = new Uri("https://appassets/index.html");
        }
    }
}
