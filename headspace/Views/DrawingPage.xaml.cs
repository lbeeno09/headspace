using headspace.Models.Common;
using headspace.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Windows.Foundation;
using Windows.UI;

namespace headspace.Views
{
    public sealed partial class DrawingPage : Page
    {
        public DrawingViewModel ViewModel { get; }

        private readonly List<Point> _currentPoints = new();
        private bool _isDrawing = false;
        private Color _activeColor;

        public DrawingPage()
        {
            this.InitializeComponent();

            ViewModel = ((App)Application.Current).Services.GetRequiredService<DrawingViewModel>();
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName is nameof(ViewModel.SelectedItem) or nameof(ViewModel.ActiveLayer))
            {
                DrawingCanvas.Invalidate();
            }
        }

        private void DrawingCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            // 1. Draw already drawn lines
            if(ViewModel.SelectedItem != null)
            {
                foreach(var layer in ViewModel.SelectedItem.Layers.Where(l => l.IsVisible))
                {
                    foreach(var stroke in layer.Strokes)
                    {
                        args.DrawingSession.DrawGeometry(stroke.Geometry, stroke.Color, stroke.StrokeWidth);
                    }
                }
            }

            // 2. draw live strokes
            if(_isDrawing && _currentPoints.Count > 1)
            {
                var strokeWidth = ViewModel.IsEraserMode ? 20.0f : ViewModel.StrokeThickness;
                for(int i = 0; i < _currentPoints.Count - 1; i++)
                {
                    args.DrawingSession.DrawLine(
                        (float)_currentPoints[i].X, (float)_currentPoints[i].Y,
                        (float)_currentPoints[i + 1].X, (float)_currentPoints[i + 1].Y,
                        _activeColor, strokeWidth
                        );
                }
            }
        }

        private void DrawingCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if(ViewModel.ActiveLayer == null)
            {
                return;
            }

            var properties = e.GetCurrentPoint(DrawingCanvas).Properties;
            if(properties.IsLeftButtonPressed)
            {
                _activeColor = ViewModel.IsEraserMode ? Colors.White : ViewModel.PrimaryColor;
            }
            else if(properties.IsRightButtonPressed)
            {
                _activeColor = ViewModel.IsEraserMode ? Colors.White : ViewModel.SecondaryColor;
            }
            else
            {
                return;
            }

            _isDrawing = true;
            _currentPoints.Clear();
            _currentPoints.Add(e.GetCurrentPoint(DrawingCanvas).Position);
        }

        private void DrawingCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if(!_isDrawing)
            {
                return;
            }

            _currentPoints.Add(e.GetCurrentPoint(DrawingCanvas).Position);

            DrawingCanvas.Invalidate();
        }


        private void DrawingCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if(!_isDrawing || _currentPoints.Count == 0 || ViewModel.ActiveLayer == null)
            {
                return;
            }

            _isDrawing = false;
            using(var pathBuilder = new CanvasPathBuilder(DrawingCanvas))
            {
                pathBuilder.BeginFigure((float)_currentPoints[0].X, (float)_currentPoints[0].Y);
                for(int i = 1; i < _currentPoints.Count; i++)
                {
                    pathBuilder.AddLine((float)_currentPoints[i].X, (float)_currentPoints[i].Y);
                }
                pathBuilder.EndFigure(CanvasFigureLoop.Open);

                var geometry = CanvasGeometry.CreatePath(pathBuilder);
                var stroke = new StrokeData
                {
                    Geometry = geometry,
                    Color = _activeColor,
                    StrokeWidth = ViewModel.IsEraserMode ? 20.0f : ViewModel.StrokeThickness
                };
                ViewModel.ActiveLayer.Strokes.Add(stroke);
            }

            _currentPoints.Clear();

            DrawingCanvas.Invalidate();
        }
    }
}
