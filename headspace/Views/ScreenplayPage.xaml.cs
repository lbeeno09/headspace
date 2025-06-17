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
    public sealed partial class ScreenplayPage : Page, ISavablePage
    {
        private ScreenplayViewModel ViewModel => DataContext as ScreenplayViewModel;

        private enum CharacterFormatting
        {
            Bold,
            Italic,
            Underline
        }

        public ScreenplayPage()
        {
            InitializeComponent();

            this.DataContext = new ScreenplayViewModel();
            this.Loaded += (s, e) =>
            {
                if(ViewModel != null)
                {
                    ViewModel.PageXamlRoot = this.XamlRoot;
                }
            };
            this.Loaded += ScreenplayPage_Loaded;

            ScreenplayEditor.SelectionChanged += ScreenplayEditor_SelectionChanged;
            ScreenplayEditor.KeyDown += ScreenplayEditor_KeyDown;
        }

        private void ScreenplayPage_Loaded(object sender, RoutedEventArgs e)
        {
            if(ViewModel != null)
            {
                ViewModel.RequestDisplayScreenplay -= ViewModel_Request_DisplayScreenplay;
                ViewModel.RequestDisplayScreenplay += ViewModel_Request_DisplayScreenplay;
            }
            LoadDocument(ViewModel.SelectedScreenplay);
        }

        private void ViewModel_Request_DisplayScreenplay(object sender, ScreenplayItem screenplayItem)
        {
            LoadDocument(ViewModel.SelectedScreenplay);
        }


        private void BoldButton_Click(object sender, RoutedEventArgs e)
        { ApplyCharacterFormatting(CharacterFormatting.Bold); }
        private void ItalicButton_Click(object sender, RoutedEventArgs e)
        { ApplyCharacterFormatting(CharacterFormatting.Italic); }
        private void UnderlineButton_Click(object sender, RoutedEventArgs e)
        { ApplyCharacterFormatting(CharacterFormatting.Underline); }

        private void AlignLeftButton_Click(object sender, RoutedEventArgs e)
        {
            ScreenplayEditor.Document.Selection.ParagraphFormat.Alignment = ParagraphAlignment.Left;
            UpdateAlignmentButtonStates(ParagraphAlignment.Left);
        }
        private void AlignCenterButton_Click(object sender, RoutedEventArgs e)
        {
            ScreenplayEditor.Document.Selection.ParagraphFormat.Alignment = ParagraphAlignment.Center;
            UpdateAlignmentButtonStates(ParagraphAlignment.Center);
        }
        private void AlignRightButton_Click(object sender, RoutedEventArgs e)
        {
            ScreenplayEditor.Document.Selection.ParagraphFormat.Alignment = ParagraphAlignment.Right;
            UpdateAlignmentButtonStates(ParagraphAlignment.Right);
        }

        private void ApplyCharacterFormatting(CharacterFormatting format)
        {
            var selection = ScreenplayEditor.Document.Selection;
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
                paragraphAlignment = ScreenplayEditor.Document.Selection.ParagraphFormat.Alignment;
            }

            AlignLeftButton.IsChecked = paragraphAlignment == ParagraphAlignment.Left;
            AlignCenterButton.IsChecked = paragraphAlignment == ParagraphAlignment.Center;
            AlignRightButton.IsChecked = paragraphAlignment == ParagraphAlignment.Right;
        }

        private void ScreenplayEditor_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var charFormat = ScreenplayEditor.Document.Selection.CharacterFormat;

            // Update Bold Button
            BoldButton.IsChecked = charFormat.Weight == FontWeights.Bold.Weight;
            // Update Italics Button
            ItalicButton.IsChecked = charFormat.Italic == FormatEffect.On;
            // Update Underline Button
            UnderlineButton.IsChecked = charFormat.Underline != UnderlineType.None;

            // Update alignment
            UpdateAlignmentButtonStates();
        }

        private void ScreenplayEditor_KeyDown(object sender, KeyRoutedEventArgs e)
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
                ScreenplayEditor.Document.Selection.TypeText("\t");
                e.Handled = true;
            }
        }

        public void SavePageContentToModel()
        {
            if(ViewModel.SelectedScreenplay != null)
            {
                ScreenplayEditor.Document.GetText(TextGetOptions.FormatRtf, out string rtfContent);
                ViewModel.SelectedScreenplay.Content = rtfContent;
                ViewModel.SelectedScreenplay.LastModified = DateTime.Now;

                System.Diagnostics.Debug.WriteLine($"Document content save to ViewModel for: {ViewModel.SelectedScreenplay.Title}");
            }
        }

        private void LoadDocument(ScreenplayItem screenplayItem)
        {
            if(screenplayItem == null)
            {
                return;
            }

            if(!string.IsNullOrEmpty(screenplayItem.Content))
            {
                ScreenplayEditor.Document.SetText(TextSetOptions.FormatRtf, screenplayItem.Content);
                System.Diagnostics.Debug.WriteLine($"Document content loaded for: {screenplayItem.Title}");
            }
            else
            {
                ScreenplayEditor.Document.SetText(TextSetOptions.None, string.Empty);
                System.Diagnostics.Debug.WriteLine($"No content loaded for: {screenplayItem.Title}. Editor cleared.");
            }

            ScreenplayEditor_SelectionChanged(this, new RoutedEventArgs());
        }
    }
}
