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
        public async Task<string?> PickSaveFileAsync()
        {
            var savePicker = new FileSavePicker();
            var window = App.MainWindow;
            InitializeWithWindow.Initialize(savePicker, WindowNative.GetWindowHandle(window));

            savePicker.FileTypeChoices.Add("headspace project", new List<string>() { ".hsp" });
            savePicker.SuggestedFileName = "New Project";

            StorageFile file = await savePicker.PickSaveFileAsync();

            return file?.Path;
        }

        public async Task<string?> PickOpenFileAsync()
        {
            var openPicker = new FileOpenPicker();
            var window = App.MainWindow;
            InitializeWithWindow.Initialize(openPicker, WindowNative.GetWindowHandle(window));

            openPicker.FileTypeFilter.Add(".hsp");
            StorageFile file = await openPicker.PickSingleFileAsync();

            return file?.Path;
        }
    }
}
