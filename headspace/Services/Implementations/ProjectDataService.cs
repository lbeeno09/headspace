using headspace.Models.Common;
using headspace.Services.Interfaces;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

// Project Data Maanger

namespace headspace.Services.Implementations
{
    public class ProjectDataService : IProjectDataService
    {
        private Window _hostWindow;
        private const string _FileExtension = ".hsp";

        public ProjectDataService(Window hostWindow)
        {
            _hostWindow = hostWindow;
        }

        // --- Core Load Method ---
        public async Task<Project> LoadProjectAsync(string fileName, StorageFile? file = null)
        {
            Project? loadedProject = null;
            try
            {
                if(file == null)
                {
                    StorageFolder localFolder = ApplicationData.Current.LocalFolder;

                    file = await localFolder.GetFileAsync(fileName);
                }

                string json = await FileIO.ReadTextAsync(file);
                loadedProject = JsonSerializer.Deserialize<Project>(json);

                System.Diagnostics.Debug.WriteLine($"Project '{loadedProject?.ProjectName}' loaded from: {file.Path}");
            }
            catch(FileNotFoundException)
            {
                System.Diagnostics.Debug.WriteLine($"No project file '{fileName}' found. Returning new project.");

                loadedProject = new Project();
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading project: {ex.Message}");

                loadedProject = new Project();
            }

            return loadedProject;
        }

        // --- Core Save Method ---
        public async Task SaveProjectAsync(Project projectToSave, string fileName, StorageFile? file = null)
        {
            try
            {
                if(file == null)
                {
                    StorageFolder localFolder = ApplicationData.Current.LocalFolder;

                    file = await localFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                }

                string json = JsonSerializer.Serialize(projectToSave, new JsonSerializerOptions { WriteIndented = true });

                await FileIO.WriteTextAsync(file, json);

                System.Diagnostics.Debug.WriteLine($"Project '{projectToSave.ProjectName}' saved to: {file.Path}");
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving project: {ex.Message}");
            }
        }

        public void CreateNewProject()
        {
            System.Diagnostics.Debug.Write("New Project Created.");
        }

        public void SetProjectName(Project project, string newName)
        {
            project.ProjectName = newName;

            System.Diagnostics.Debug.Write($"Project name updated to: {newName}");
        }

        // --- Helper Methods ---
        public async Task<StorageFile> PickSaveFileAsync(string suggestedFileName, List<string> fileTypeChoices)
        {
            var savePicker = new FileSavePicker();
            var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(_hostWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(savePicker, windowHandle);

            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("Headspace Project", fileTypeChoices);
            savePicker.SuggestedFileName = suggestedFileName;

            return await savePicker.PickSaveFileAsync();
        }

        public async Task<StorageFile> PickOpenFileAsync(List<string> fileTypeFilter)
        {
            var openPicker = new FileOpenPicker();
            var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(_hostWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, windowHandle);

            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add(_FileExtension);

            return await openPicker.PickSingleFileAsync();
        }
    }
}
