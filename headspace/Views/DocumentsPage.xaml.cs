using headspace.Models;
using headspace.ViewModels;
using headspace.Views.Common;
using Microsoft.UI.Input;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using Windows.System;
using Windows.UI.Core;

namespace headspace.Views
{
    public sealed partial class DocumentsPage : Page, ISavablePage
    {
        private DocumentViewModel ViewModel => DataContext as DocumentViewModel;

        private enum CharacterFormatting
        {
            Bold,
            Italic,
            Underline
        }

        public DocumentsPage()
        {
            this.InitializeComponent();

            this.DataContext = new DocumentViewModel();
            this.Loaded += (s, e) =>
            {
                if(ViewModel != null)
                {
                    ViewModel.PageXamlRoot = this.XamlRoot;
                }
            };
            this.Loaded += DocumentsPage_Loaded;

            // attach event handler for rtf box
            DocumentEditor.SelectionChanged += DocumentEditor_SelectionChanged;
            DocumentEditor.KeyDown += DocumentEditor_KeyDown;
        }

        private void DocumentsPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDocument(ViewModel.SelectedDocument);
        }

        private void BoldButton_Click(object sender, RoutedEventArgs e)
        { ApplyCharacterFormatting(CharacterFormatting.Bold); }
        private void ItalicButton_Click(object sender, RoutedEventArgs e)
        { ApplyCharacterFormatting(CharacterFormatting.Italic); }
        private void UnderlineButton_Click(object sender, RoutedEventArgs e)
        { ApplyCharacterFormatting(CharacterFormatting.Underline); }

        private void AlignLeftButton_Click(object sender, RoutedEventArgs e)
        {
            DocumentEditor.Document.Selection.ParagraphFormat.Alignment = ParagraphAlignment.Left;
            UpdateAlignmentButtonStates(ParagraphAlignment.Left);
        }
        private void AlignCenterButton_Click(object sender, RoutedEventArgs e)
        {
            DocumentEditor.Document.Selection.ParagraphFormat.Alignment = ParagraphAlignment.Center;
            UpdateAlignmentButtonStates(ParagraphAlignment.Center);
        }
        private void AlignRightButton_Click(object sender, RoutedEventArgs e)
        {
            DocumentEditor.Document.Selection.ParagraphFormat.Alignment = ParagraphAlignment.Right;
            UpdateAlignmentButtonStates(ParagraphAlignment.Right);
        }

        private void ApplyCharacterFormatting(CharacterFormatting format)
        {
            var selection = DocumentEditor.Document.Selection;
            if(selection != null)
            {
                var charFormat = selection.CharacterFormat;
                switch(format)
                {
                    case CharacterFormatting.Bold:
                        charFormat.Bold = FormatEffect.Toggle;
                        selection.CharacterFormat = charFormat;
                        break;
                    case CharacterFormatting.Italic:
                        charFormat.Italic = FormatEffect.Toggle;
                        selection.CharacterFormat = charFormat;
                        break;
                    case CharacterFormatting.Underline:
                        charFormat.Underline = (charFormat.Underline == UnderlineType.None) ? UnderlineType.Single : UnderlineType.None;
                        selection.CharacterFormat = charFormat;
                        break;
                }
            }
        }

        private void UpdateAlignmentButtonStates(ParagraphAlignment? newAlignment = null)
        {
            ParagraphAlignment paragraphAlignment;
            if(newAlignment.HasValue)
            {
                paragraphAlignment = newAlignment.Value;
            }
            else
            {
                paragraphAlignment = DocumentEditor.Document.Selection.ParagraphFormat.Alignment;
            }

            AlignLeftButton.IsChecked = paragraphAlignment == ParagraphAlignment.Left;
            AlignCenterButton.IsChecked = paragraphAlignment == ParagraphAlignment.Center;
            AlignRightButton.IsChecked = paragraphAlignment == ParagraphAlignment.Right;
        }

        private void DocumentEditor_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var charFormat = DocumentEditor.Document.Selection.CharacterFormat;

            // Update Bold Button
            BoldButton.IsChecked = charFormat.Weight == FontWeights.Bold.Weight;
            // Update Italics Button
            ItalicButton.IsChecked = charFormat.Italic == FormatEffect.On;
            // Update Underline Button
            UnderlineButton.IsChecked = charFormat.Underline != UnderlineType.None;

            // Update alignment
            UpdateAlignmentButtonStates();
        }

        private void DocumentEditor_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            bool isControlPressed = (InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control) & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
            if(isControlPressed)
            {
                switch(e.Key)
                {
                    case VirtualKey.B:
                        ApplyCharacterFormatting(CharacterFormatting.Bold);
                        BoldButton.IsChecked = !BoldButton.IsChecked;
                        e.Handled = true;
                        break;
                    case VirtualKey.I:
                        ApplyCharacterFormatting(CharacterFormatting.Italic);
                        ItalicButton.IsChecked = !ItalicButton.IsChecked;
                        e.Handled = true;
                        break;
                    case VirtualKey.U:
                        ApplyCharacterFormatting(CharacterFormatting.Underline);
                        UnderlineButton.IsChecked = !UnderlineButton.IsChecked;
                        e.Handled = true;
                        break;
                    case VirtualKey.L:
                        UpdateAlignmentButtonStates(ParagraphAlignment.Left);
                        e.Handled = true;
                        break;
                    case VirtualKey.E:
                        UpdateAlignmentButtonStates(ParagraphAlignment.Center);
                        e.Handled = true;
                        break;
                    case VirtualKey.R:
                        UpdateAlignmentButtonStates(ParagraphAlignment.Right);
                        e.Handled = true;
                        break;
                }
            }

            if(e.Key == VirtualKey.Tab)
            {
                DocumentEditor.Document.Selection.TypeText("\t");
                e.Handled = true;
            }
        }

        public void SavePageContentToModel()
        {
            if(ViewModel.SelectedDocument != null)
            {
                DocumentEditor.Document.GetText(TextGetOptions.FormatRtf, out string rtfContent);
                ViewModel.SelectedDocument.Content = rtfContent;
                ViewModel.SelectedDocument.LastModified = DateTime.Now;

                System.Diagnostics.Debug.WriteLine($"Document content save to ViewModel for: {ViewModel.SelectedDocument.Title}");
            }
        }

        private void LoadDocument(DocumentItem documentItem)
        {
            if(documentItem == null)
            {
                return;
            }

            if(!string.IsNullOrEmpty(documentItem.Content))
            {
                DocumentEditor.Document.SetText(TextSetOptions.FormatRtf, documentItem.Content);
                System.Diagnostics.Debug.WriteLine($"Document content loaded for: {documentItem.Title}");
            }
            else
            {
                DocumentEditor.Document.SetText(TextSetOptions.None, string.Empty);
                System.Diagnostics.Debug.WriteLine($"No content loaded for: {documentItem.Title}. Editor cleared.");
            }

            DocumentEditor_SelectionChanged(this, new RoutedEventArgs());
        }
    }
}
