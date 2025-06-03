using headspace.Models;
using headspace.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.Foundation;

namespace headspace.Views
{
    public sealed partial class MoodboardPage : Page
    {
        private bool isDrawing = false;
        private Polyline? currentStroke;
        private List<Point> points = new();

        public MoodboardPage()
        {
            this.InitializeComponent();

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(ViewModel.SelectedMoodboard))
            {
                RedrawCanvas();
            }
        }

        private void RedrawCanvas()
        {
            DrawingCanvas.Children.Clear();
            if(ViewModel.SelectedMoodboard == null)
            {
                return;
            }

            foreach(var stroke in ViewModel.SelectedMoodboard.Strokes)
            {
                var polyline = new Polyline
                {
                    Stroke = new SolidColorBrush(stroke.Color),
                    StrokeThickness = stroke.Thickness
                };
                foreach(var pt in stroke.Points)
                {
                    polyline.Points.Add(pt);
                }
                DrawingCanvas.Children.Add(polyline);
            }
        }

        private void Canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if(ViewModel.SelectedMoodboard == null)
            {
                return;
            }

            isDrawing = true;
            points.Clear();

            currentStroke = new Polyline
            {
                Stroke = new SolidColorBrush(ViewModel.IsEraserMode ? Colors.White : ViewModel.SelectedNamedColor.Color),
                StrokeThickness = ViewModel.SelectedThickness
            };

            var pos = e.GetCurrentPoint(DrawingCanvas).Position;
            points.Add(pos);
            currentStroke.Points.Add(pos);

            DrawingCanvas.Children.Add(currentStroke);
        }

        private void Canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if(!isDrawing || currentStroke == null)
            {
                return;
            }

            var pos = e.GetCurrentPoint(DrawingCanvas).Position;
            points.Add(pos);
            currentStroke.Points.Add(pos);
        }

        private void Canvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if(!isDrawing || ViewModel.SelectedMoodboard == null || points.Count < 2)
            {
                return;
            }

            ViewModel.SelectedMoodboard.Strokes.Add(new StrokeData
            {
                Points = new List<Point>(points),
                Color = ViewModel.IsEraserMode ? Colors.White : ViewModel.SelectedNamedColor.Color,
                Thickness = ViewModel.SelectedThickness
            });

            isDrawing = false;
        }

        private void RenameButton_Click(object sender, RoutedEventArgs e)
        {
            if(DataContext is MoodboardViewModel viewModel)
            {
                _ = viewModel.RenameMoodboardAsync(this.XamlRoot);
            }
        }
    }
}
