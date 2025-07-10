using headspace.Models;
using headspace.Services.Interfaces;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.UI;
using QuestPDF.Fluent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace headspace.Services.Implementations
{
    public class CanvasExportService : ICanvasExportService
    {
        public async Task ExportAsPngAsync(DrawingModel drawing, string filePath)
        {
            // TODO: Make size fit model
            var width = 1920;
            var height = 1080;

            var device = CanvasDevice.GetSharedDevice();
            using var renderTarget = new CanvasRenderTarget(device, width, height, 96);

            using(var ds = renderTarget.CreateDrawingSession())
            {
                ds.Clear(Colors.White);

                foreach(var layer in drawing.Layers.Where(l => l.IsVisible))
                {
                    foreach(var stroke in layer.Strokes)
                    {
                        if(stroke.CachedGeometry == null && stroke.Points.Count > 1)
                        {
                            using var pathBuilder = new CanvasPathBuilder(device);
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
                            ds.DrawGeometry(stroke.CachedGeometry, stroke.Color, stroke.StrokeWidth);
                        }
                    }
                }
            }

            using var stream = new MemoryStream();
            await renderTarget.SaveAsync(stream.AsRandomAccessStream(), CanvasBitmapFileFormat.Png);
            await File.WriteAllBytesAsync(filePath, stream.ToArray());
        }

        public async Task ExportAsPngAsync(MoodboardModel drawing, string filePath)
        {
            // TODO: Make size fit model
            var width = 1920;
            var height = 1080;

            var device = CanvasDevice.GetSharedDevice();
            using var renderTarget = new CanvasRenderTarget(device, width, height, 96);

            using(var ds = renderTarget.CreateDrawingSession())
            {
                ds.Clear(Colors.White);

                foreach(var stroke in drawing.Strokes)
                {
                    if(stroke.CachedGeometry == null && stroke.Points.Count > 1)
                    {
                        using var pathBuilder = new CanvasPathBuilder(device);
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
                        ds.DrawGeometry(stroke.CachedGeometry, stroke.Color, stroke.StrokeWidth);
                    }
                }
            }

            using var stream = new MemoryStream();
            await renderTarget.SaveAsync(stream.AsRandomAccessStream(), CanvasBitmapFileFormat.Png);
            await File.WriteAllBytesAsync(filePath, stream.ToArray());
        }

        public async Task ExportAsPdfAsync(StoryboardModel drawing, string filePath)
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var panelImages = new List<byte[]>();
            var device = CanvasDevice.GetSharedDevice();

            // render each panel to png
            foreach(var panel in drawing.Panels)
            {
                using var renderTarget = new CanvasRenderTarget(device, 1080, 1920, 96);
                using(var ds = renderTarget.CreateDrawingSession())
                {
                    ds.Clear(Colors.White);
                    foreach(var stroke in panel.Strokes)
                    {
                        if(stroke.CachedGeometry == null && stroke.Points.Count > 1)
                        {
                            using var pathBuilder = new CanvasPathBuilder(device);

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
                            ds.DrawGeometry(stroke.CachedGeometry, stroke.Color, stroke.StrokeWidth);
                        }
                    }
                }

                using var stream = new MemoryStream();
                await renderTarget.SaveAsync(stream.AsRandomAccessStream(), CanvasBitmapFileFormat.Png);
                panelImages.Add(stream.ToArray());
            }

            // 2. create pdf from images
            Document.Create(container =>
            {
                foreach(var imageData in panelImages)
                {
                    container.Page(page =>
                    {
                        page.Margin(20);
                        page.Content().Image(imageData).FitArea();
                    });
                }
            }).GeneratePdf(filePath);
        }
    }
}
