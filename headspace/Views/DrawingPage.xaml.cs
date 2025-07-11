using headspace.Models;
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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
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

        private DrawingModel? _lastSelectedDrawing;

        public DrawingPage()
        {
            this.InitializeComponent();

            ViewModel = ((App)Application.Current).Services.GetRequiredService<DrawingViewModel>();
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(ViewModel.SelectedItem))
            {
                // Unsub from previous item's layer events
                if(_lastSelectedDrawing?.Layers != null)
                {
                    _lastSelectedDrawing.Layers.CollectionChanged -= Layers_CollectionChanged;
                    foreach(var layer in _lastSelectedDrawing.Layers)
                    {
                        layer.PropertyChanged -= Layer_PropertyChanged;
                    }
                }

                // sub to new item's layer events
                if(ViewModel.SelectedItem?.Layers != null)
                {
                    ViewModel.SelectedItem.Layers.CollectionChanged += Layers_CollectionChanged;
                    foreach(var layer in ViewModel.SelectedItem.Layers)
                    {
                        layer.PropertyChanged += Layer_PropertyChanged;
                    }
                }

                // remember current item for next item change
                _lastSelectedDrawing = ViewModel.SelectedItem;
                DrawingCanvas.Invalidate();
            }
        }

        private void Layers_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.OldItems != null)
            {
                foreach(LayerModel item in e.OldItems)
                {
                    item.PropertyChanged -= Layer_PropertyChanged;
                }
            }
            if(e.NewItems != null)
            {
                foreach(LayerModel item in e.NewItems)
                {
                    item.PropertyChanged += Layer_PropertyChanged;
                }
            }
        }

        private void Layer_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(LayerModel.IsVisible))
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
                        // if geometry is not cached, create from points
                        if(stroke.CachedGeometry == null && stroke.Points.Count > 1)
                        {
                            using var pathBuilder = new CanvasPathBuilder(sender);
                            pathBuilder.BeginFigure(stroke.Points[0]);
                            for(int i = 1; i < stroke.Points.Count; i++)
                            {
                                pathBuilder.AddLine(stroke.Points[i]);
                            }
                            pathBuilder.EndFigure(CanvasFigureLoop.Open);

                            stroke.CachedGeometry = CanvasGeometry.CreatePath(pathBuilder);
                        }

                        if(stroke.CachedGeometry != null)
                        {
                            args.DrawingSession.DrawGeometry(stroke.CachedGeometry, stroke.Color, stroke.StrokeWidth);
                        }
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
            if(!properties.IsLeftButtonPressed && !properties.IsRightButtonPressed)
            {
                return;
            }

            _isDrawing = true;
            _activeColor = properties.IsLeftButtonPressed ? ViewModel.PrimaryColor : ViewModel.SecondaryColor;

            _currentPoints.Clear();
            _currentPoints.Add(e.GetCurrentPoint(DrawingCanvas).Position);

            (sender as UIElement).CapturePointer(e.Pointer);
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
            (sender as UIElement).ReleasePointerCapture(e.Pointer);

            if(!_isDrawing ||  ViewModel.ActiveLayer == null)
            {
                return;
            }

            _isDrawing = false;

            CanvasGeometry geometry;
            if(_currentPoints.Count == 1)
            {
                geometry = CanvasGeometry.CreateCircle(DrawingCanvas.Device, (float)_currentPoints[0].X, (float)_currentPoints[0].Y, ViewModel.StrokeThickness / 2);
            } 
            else
            {
                using(var pathBuilder = new CanvasPathBuilder(DrawingCanvas))
                {
                    pathBuilder.BeginFigure((float)_currentPoints[0].X, (float)_currentPoints[0].Y);
                    for(int i = 1; i < _currentPoints.Count; i++)
                    {
                        pathBuilder.AddLine((float)_currentPoints[i].X, (float)_currentPoints[i].Y);
                    }
                    pathBuilder.EndFigure(CanvasFigureLoop.Open);

                    geometry = CanvasGeometry.CreatePath(pathBuilder);
                }
            }

            var stroke = new StrokeData
            {
                CachedGeometry = geometry,
                Color = _activeColor,
                StrokeWidth = ViewModel.StrokeThickness
            };
            ViewModel.ActiveLayer.Strokes.Add(stroke);

            _currentPoints.Clear();
            
            DrawingCanvas.Invalidate();
        }
    }
}
