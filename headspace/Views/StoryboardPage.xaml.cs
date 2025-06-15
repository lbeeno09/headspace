using headspace.Models;
using headspace.Models.Common;
using headspace.ViewModels;
using headspace.Views.Common;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Windows.Foundation;
using Windows.UI;

namespace headspace.Views
{
    public sealed partial class StoryboardPage : Page, ISavablePage
    {
        private StoryboardViewModel ViewModel => DataContext as StoryboardViewModel;
        private Polyline currentStroke;
        private List<Point> currentPoints;

        public class SerializableStroke
        {
            public List<Point> Points { get; set; } = new List<Point>();
            public string StrokeColor { get; set; }
            public double Thickness { get; set; }
            public bool IsEraser { get; set; }
        }

        public StoryboardPage()
        {
            this.InitializeComponent();
            this.DataContext = new StoryboardViewModel();

            ViewModel.ClearCanvasCommand.CanExecuteChanged += (sender, e) => ClearCanvasUI();
            ViewModel.StoryboardListManager.OnItemSelected += StoryboardListManager_OnItemSelected;

            this.Loaded += StoryboardPage_Loaded;
        }

        private void StoryboardPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadStoryboard(ViewModel.SelectedStoryboard);
        }

        private void StoryboardListManager_OnItemSelected(object sender, ProjectItemBase e)
        {
            LoadStoryboard(e as StoryboardItem);
        }

        private void StoryboardCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if(ViewModel.SelectedStoryboard == null)
            {
                return;
            }

            currentPoints = new List<Point>();
            currentStroke = new Polyline
            {
                Stroke = ViewModel.IsEraserMode ? StoryboardCanvas.Background : ViewModel.PrimaryColor,
                StrokeThickness = ViewModel.StrokeThickness,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeLineJoin = PenLineJoin.Round
            };

            currentPoints.Add(e.GetCurrentPoint(StoryboardCanvas).Position);
            StoryboardCanvas.Children.Add(currentStroke);

            e.Handled = true;
        }

        private void StoryboardCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if(!e.Pointer.IsInContact || currentStroke == null || currentPoints == null)
            {
                return;
            }

            currentPoints.Add(e.GetCurrentPoint(StoryboardCanvas).Position);

            var newPointCollection = new PointCollection();
            foreach(var point in currentPoints)
            {
                newPointCollection.Add(point);
            }
            currentStroke.Points = newPointCollection;

            e.Handled = true;
        }


        private void StoryboardCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if(currentStroke != null && currentPoints != null)
            {
                var newPointCollection = new PointCollection();
                foreach(var point in currentPoints)
                {
                    newPointCollection.Add(point);
                }

                currentStroke.Points = newPointCollection;
            }

            currentStroke = null;
            currentPoints = null;
            e.Handled = true;
        }

        private void ClearCanvasUI()
        {
            StoryboardCanvas.Children.Clear();
        }

        public void SavePageContentToModel()
        {
            if(ViewModel.SelectedStoryboard == null)
            {
                return;
            }

            var serializableStrokes = new List<SerializableStroke>();
            foreach(UIElement child in StoryboardCanvas.Children)
            {
                if(child is Polyline polyline)
                {
                    string colorHex = (polyline.Stroke as SolidColorBrush)?.Color.ToString() ?? Colors.Black.ToString();

                    serializableStrokes.Add(new SerializableStroke
                    {
                        Points = polyline.Points.ToList(),
                        StrokeColor = colorHex,
                        Thickness = polyline.StrokeThickness,
                        IsEraser = (polyline.Stroke as SolidColorBrush)?.Color == ((SolidColorBrush)StoryboardCanvas.Background)?.Color
                    });
                }
            }

            ViewModel.SelectedStoryboard.Content = JsonSerializer.Serialize(serializableStrokes);
            ViewModel.SelectedStoryboard.LastModified = DateTime.Now;
            System.Diagnostics.Debug.WriteLine($"Drawing data serialized for: {ViewModel.SelectedStoryboard.ToString}. Size: {ViewModel.SelectedStoryboard.Content.Length} chars");
        }

        private void LoadStoryboard(StoryboardItem storyboardItem)
        {
            ClearCanvasUI();

            if(storyboardItem == null || string.IsNullOrEmpty(storyboardItem.Content))
            {
                return;
            }

            try
            {
                var loadedStrokes = JsonSerializer.Deserialize<List<SerializableStroke>>(storyboardItem.Content);
                if(loadedStrokes != null)
                {
                    foreach(var sStroke in loadedStrokes)
                    {
                        if(sStroke.Points == null || sStroke.Points.Count == 0)
                        {
                            continue;
                        }

                        var polyline = new Polyline
                        {
                            Stroke = new SolidColorBrush(ParseColor(sStroke.StrokeColor)),
                            StrokeThickness = sStroke.Thickness,
                            StrokeEndLineCap = PenLineCap.Round,
                            StrokeStartLineCap = PenLineCap.Round,
                            StrokeLineJoin = PenLineJoin.Round
                        };
                        polyline.Points = new PointCollection();
                        foreach(Point point in sStroke.Points)
                        {
                            polyline.Points.Add(point);
                        }
                        StoryboardCanvas.Children.Add(polyline);
                    }
                    System.Diagnostics.Debug.WriteLine($"Loaded {loadedStrokes.Count} strokes for: {storyboardItem.Title}");
                }
            }
            catch(JsonException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deserializing drawing data for {storyboardItem.Title}: {ex.Message}");

            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Unexpected error loading drawing data for {storyboardItem.Title}: {ex.Message}");
            }
        }

        private Color ParseColor(string hexColor)
        {
            if(string.IsNullOrEmpty(hexColor) || hexColor.Length != 9 || !hexColor.StartsWith("#"))
            {
                return Colors.Black;
            }

            try
            {
                byte a = byte.Parse(hexColor.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
                byte r = byte.Parse(hexColor.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
                byte g = byte.Parse(hexColor.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
                byte b = byte.Parse(hexColor.Substring(7, 2), System.Globalization.NumberStyles.HexNumber);

                return Color.FromArgb(a, r, g, b);
            }
            catch
            {
                return Colors.Red;
            }
        }
    }
}
