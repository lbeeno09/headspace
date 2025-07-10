using headspace.Models;
using System.Threading.Tasks;

namespace headspace.Services.Interfaces
{
    public interface ICanvasExportService
    {
        Task ExportAsPngAsync(DrawingModel drawing, string filePath);
        Task ExportAsPngAsync(MoodboardModel drawing, string filePath);
        Task ExportAsPdfAsync(StoryboardModel drawing, string filePath);
    }
}
