using headspace.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace headspace.Services.Implementations
{
    public class FilePickerService : IFilePickerService
    {
        public async Task<string?> PickSaveProjectAsync()
        {
            var savePicker = new FileSavePicker();
            var window = App.MainWindow;
            InitializeWithWindow.Initialize(savePicker, WindowNative.GetWindowHandle(window));

            savePicker.FileTypeChoices.Add("headspace project", new List<string>() { ".hsp" });
            savePicker.SuggestedFileName = "New Project";

            StorageFile file = await savePicker.PickSaveFileAsync();

            return file?.Path;
        }

        public async Task<string?> PickOpenProjectAsync()
        {
            var openPicker = new FolderPicker();
            var window = App.MainWindow;
            InitializeWithWindow.Initialize(openPicker, WindowNative.GetWindowHandle(window));

            openPicker.FileTypeFilter.Add(".hsp");
            var folder = await openPicker.PickSingleFolderAsync();

            return folder?.Path;
        }

        public async Task<string?> PickSaveFileAsync_Markdown(string selectedItem)
        {
            var savePicker = new FileSavePicker();
            InitializeWithWindow.Initialize(savePicker, WindowNative.GetWindowHandle(App.MainWindow));

            savePicker.FileTypeChoices.Add("Markdown File", new List<string>() { ".md" });
            savePicker.SuggestedFileName = selectedItem;

            var file = await savePicker.PickSaveFileAsync();
            return file?.Path;
        }

        public async Task<string?> PickSaveFileAsync_Rtf(string selectedItem)
        {
            var savePicker = new FileSavePicker();
            InitializeWithWindow.Initialize(savePicker, WindowNative.GetWindowHandle(App.MainWindow));

            savePicker.FileTypeChoices.Add("Rich Text Format", new List<string>() { ".rtf" });
            savePicker.SuggestedFileName = selectedItem;

            var file = await savePicker.PickSaveFileAsync();
            return file?.Path;
        }

        public async Task<string?> PickSaveFileAsync_Png(string selectedItem)
        {
            var savePicker = new FileSavePicker();
            InitializeWithWindow.Initialize(savePicker, WindowNative.GetWindowHandle(App.MainWindow));

            savePicker.FileTypeChoices.Add("PNG Image", new List<string>() { ".png" });
            savePicker.SuggestedFileName = selectedItem;

            var file = await savePicker.PickSaveFileAsync();
            return file?.Path;
        }

        public async Task<string?> PickSaveFileAsync_Svg(string selectedItem)
        {
            var savePicker = new FileSavePicker();
            InitializeWithWindow.Initialize(savePicker, WindowNative.GetWindowHandle(App.MainWindow));

            savePicker.FileTypeChoices.Add("SVG Image", new List<string>() { ".svg" });
            savePicker.SuggestedFileName = selectedItem;

            var file = await savePicker.PickSaveFileAsync();
            return file?.Path;
        }

        public async Task<string?> PickSaveFileAsync_Pdf(string selectedItem)
        {
            var savePicker = new FileSavePicker();
            InitializeWithWindow.Initialize(savePicker, WindowNative.GetWindowHandle(App.MainWindow));

            savePicker.FileTypeChoices.Add("PDF Document", new List<string>() { ".pdf" });
            savePicker.SuggestedFileName = selectedItem;

            var file = await savePicker.PickSaveFileAsync();
            return file?.Path;
        }
    }
}
