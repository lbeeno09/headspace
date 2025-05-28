using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Collections.Generic;
using System.Numerics;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace headspace.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StoryboardPage : Page
    {
        private record struct Stroke(Vector2 Start, Vector2 End, Color Color, float Thickness);
        private List<Stroke> strokes = new();
        private Vector2? previousPoint = null;
        private bool isDrawing = false;
        private Color currentColor = Colors.Black;
        private float currentThickness = 1.0f;

        public StoryboardPage()
        {
            this.InitializeComponent();

            ColorPicker.SelectedIndex = 0;
            ThicknessPicker.SelectedIndex = 0;
        }

        private void DrawingCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            foreach (var stroke in strokes)
            {
                args.DrawingSession.DrawLine(stroke.Start, stroke.End, stroke.Color, stroke.Thickness);
            }
        }

        private void DrawingCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            isDrawing = true;

            var point = e.GetCurrentPoint(DrawingCanvas).Position;
            previousPoint = new Vector2((float)point.X, (float)point.Y);
        }

        private void DrawingCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!isDrawing || !e.GetCurrentPoint(DrawingCanvas).IsInContact)
            {
                return;
            }

            var point = e.GetCurrentPoint(DrawingCanvas).Position;
            Vector2 current = new((float)point.X, (float)point.Y);

            if (previousPoint is Vector2 start)
            {
                strokes.Add(new Stroke(start, current, currentColor, currentThickness));
                previousPoint = current;
                DrawingCanvas.Invalidate();
            }
        }


        private void DrawingCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            isDrawing = false;
            previousPoint = null;
        }

        private void ColorPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tag = ((ComboBoxItem)ColorPicker.SelectedItem).Tag.ToString();
            currentColor = tag switch
            {
                "Red" => Colors.Red,
                "Orange" => Colors.Orange,
                "Yellow" => Colors.Yellow,
                "Green" => Colors.Green,
                "Blue" => Colors.Blue,
                "Indigo" => Colors.Indigo,
                "Violet" => Colors.Violet,
                "White" => Colors.White,
                "Black" => Colors.Black,
                _ => Colors.Black,
            };
        }

        private void ThicknessPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tag = ((ComboBoxItem)ThicknessPicker.SelectedItem).Tag.ToString();
            currentThickness = float.Parse(tag);
        }
    }
}
