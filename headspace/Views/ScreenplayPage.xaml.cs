using headspace.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.ComponentModel;
using Windows.System;

namespace headspace.Views
{
    public sealed partial class ScreenplayPage : Page
    {
        public ScreenplayViewModel ViewModel { get; }
        private bool _isProgrammaticChange = false;

        public ScreenplayPage()
        {
            this.InitializeComponent();

            ViewModel = ((App)Application.Current).Services.GetRequiredService<ScreenplayViewModel>();
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            this.Loaded += (s, e) =>
            {
                ViewModel.ViewXamlRoot = this.XamlRoot;
            };
            this.Loaded += ScreenplayPage_Loaded;

            ScreenplayEditor.TextChanged += ScreenplayEditor_TextChanged;
        }

        private void ScreenplayPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadScreenplayContent();
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(ViewModel.SelectedItem))
            {
                LoadScreenplayContent();
            }
        }

        private void LoadScreenplayContent()
        {
            _isProgrammaticChange = true;

            string rtfContent = ViewModel.SelectedItem?.Content ?? string.Empty;
            ScreenplayEditor.Document.SetText(TextSetOptions.FormatRtf, rtfContent);

            _isProgrammaticChange = false;
        }

        private void ScreenplayEditor_TextChanged(object sender, RoutedEventArgs e)
        {
            if(ViewModel.SelectedItem != null && !_isProgrammaticChange)
            {
                ScreenplayEditor.Document.GetText(TextGetOptions.FormatRtf, out string rtfContent);
                ViewModel.SelectedItem.Content = rtfContent;
            }
        }

        private void Editor_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == VirtualKey.Tab)
            {
                var editor = sender as RichEditBox;
                editor.Document.Selection.TypeText("\t");

                e.Handled = true;
            }
        }

        // --- Formatting Buttons ---
        private void BoldButton_Click(object sender, RoutedEventArgs e)
        {
            ScreenplayEditor.Document.Selection.CharacterFormat.Bold = FormatEffect.Toggle;
        }

        private void ItalicButton_Click(object sender, RoutedEventArgs e)
        {
            ScreenplayEditor.Document.Selection.CharacterFormat.Italic = FormatEffect.Toggle;
        }

        private void UnderlineButton_Click(object sender, RoutedEventArgs e)
        {
            ScreenplayEditor.Document.Selection.CharacterFormat.Underline = UnderlineType.Single;
        }

        private void AlignLeftButton_Click(object sender, RoutedEventArgs e)
        {
            ScreenplayEditor.Document.Selection.ParagraphFormat.Alignment = ParagraphAlignment.Left;
        }

        private void AlignCenterButton_Click(object sender, RoutedEventArgs e)
        {
            ScreenplayEditor.Document.Selection.ParagraphFormat.Alignment = ParagraphAlignment.Center;
        }
        private void AlignRightButton_Click(object sender, RoutedEventArgs e)
        {
            ScreenplayEditor.Document.Selection.ParagraphFormat.Alignment = ParagraphAlignment.Right;
        }
    }
}
