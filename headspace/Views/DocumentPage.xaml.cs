// TODO: Fix Alignment Toggle Button Reflect Immediately

using headspace.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.ComponentModel;
using System.Linq;
using Windows.System;

namespace headspace.Views
{
    public sealed partial class DocumentPage : Page
    {
        public DocumentViewModel ViewModel { get; }
        private bool _isProgrammaticChange = false;

        public DocumentPage()
        {
            this.InitializeComponent();

            ViewModel = ((App)Application.Current).Services.GetRequiredService<DocumentViewModel>();
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            this.Loaded += (s, e) =>
            {
                ViewModel.ViewXamlRoot = this.XamlRoot;
            };
            this.Loaded += DocumentPage_Loaded;

            DocumentEditor.TextChanged += DocumentEditor_TextChanged;
        }

        private void DocumentPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDocumentContent();
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(ViewModel.SelectedItem))
            {
                LoadDocumentContent();
            }
        }

        private void LoadDocumentContent()
        {
            _isProgrammaticChange = true;

            string rtfContent = ViewModel.SelectedItem?.Content ?? string.Empty;
            DocumentEditor.Document.SetText(TextSetOptions.FormatRtf, rtfContent);

            _isProgrammaticChange = false;
        }

        private void DocumentEditor_TextChanged(object sender, RoutedEventArgs e)
        {
            if(ViewModel.SelectedItem != null && !_isProgrammaticChange)
            {
                DocumentEditor.Document.GetText(TextGetOptions.FormatRtf, out string rtfContent);
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

        private void DocumentEditor_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var charFormat = DocumentEditor.Document.Selection.CharacterFormat;
            BoldButton.IsChecked = charFormat.Bold == FormatEffect.On;
            ItalicButton.IsChecked = charFormat.Italic == FormatEffect.On;
            UnderlineButton.IsChecked = charFormat.Underline == UnderlineType.Single;

            ViewModel.SelectedAlignment = DocumentEditor.Document.Selection.ParagraphFormat.Alignment;

            var fontName = charFormat.Name;
            var fontItem = FontComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(item => item.Content.ToString() == fontName);
            if(fontItem != null)
            {
                FontComboBox.SelectedItem = fontItem;
            }
        }

        // --- Formatting Buttons ---
        private void FontComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(FontComboBox.SelectedItem is ComboBoxItem selectedFont)
            {
                DocumentEditor.Document.Selection.CharacterFormat.Name = selectedFont.Content.ToString();
            }
        }

        private void BoldButton_Click(object sender, RoutedEventArgs e)
        {
            DocumentEditor.Document.Selection.CharacterFormat.Bold = FormatEffect.Toggle;
        }

        private void ItalicButton_Click(object sender, RoutedEventArgs e)
        {
            DocumentEditor.Document.Selection.CharacterFormat.Italic = FormatEffect.Toggle;
        }

        private void UnderlineButton_Click(object sender, RoutedEventArgs e)
        {
            DocumentEditor.Document.Selection.CharacterFormat.Underline = UnderlineType.Single;
        }

        private void AlignLeftButton_Click(object sender, RoutedEventArgs e)
        {
            DocumentEditor.Document.Selection.ParagraphFormat.Alignment = ParagraphAlignment.Left;
        }

        private void AlignCenterButton_Click(object sender, RoutedEventArgs e)
        {
            DocumentEditor.Document.Selection.ParagraphFormat.Alignment = ParagraphAlignment.Center;
        }

        private void AlignRightButton_Click(object sender, RoutedEventArgs e)
        {
            DocumentEditor.Document.Selection.ParagraphFormat.Alignment = ParagraphAlignment.Right;
        }
    }
}
