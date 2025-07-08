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
    }
}
