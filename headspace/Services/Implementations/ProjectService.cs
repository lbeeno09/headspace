using headspace.Models.Common;
using headspace.Services.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

// Project Data Manager

namespace headspace.Services.Implementations
{
    public partial class ProjectService : IProjectService
    {
        public Project CurrentProject { get; private set; }
        public string? ProjectFolderPath { get; private set; }
        private bool _isTemporaryProject;


        private readonly IFilePickerService _filePickerService;
        private readonly IDialogService _dialogService;

        public ProjectService(IFilePickerService filePickerService, IDialogService dialogService)
        {
            _filePickerService = filePickerService;
            _dialogService = dialogService;

            CreateNewProject(isInitialLaunch: true);
        }

        public async Task<bool> CreateNewProject(bool isInitialLaunch = false)
        {
            // 1. Check for unsaved changed
            if(!isInitialLaunch && CurrentProject.IsDirty)
            {
                var result = await _dialogService.ShowConfirmUnsavedChangesDialogAsync();
                if(result == ConfirmDialogResult.Cancel)
                {
                    return false;
                }
            }

            // 2. Cleanup old temp folder and make a new one
            CleanupTemporaryProject();
            string tempFolderName = $"headspace_tmp_{Guid.NewGuid()}";
            ProjectFolderPath = Path.Combine(Path.GetTempPath(), tempFolderName);
            Directory.CreateDirectory(ProjectFolderPath);
            _isTemporaryProject = true;

            // 3. Create new project
            CurrentProject = new Project { ProjectName = "New Project" };

            return true;
        }

        public async Task SaveItemAsync(ModelBase item)
        {
            if(_isTemporaryProject)
            {
                await SaveProjectAsAsync();

                // didnt change location
                if(_isTemporaryProject)
                {
                    return;
                }
            }

            var itemPath = Path.Combine(ProjectFolderPath, "data", item.FilePathPrefix, $"{item.Id}.json");
            Directory.CreateDirectory(Path.GetDirectoryName(itemPath));

            var json = JsonSerializer.Serialize(item as object, item.GetType());
            await File.WriteAllTextAsync(itemPath, json);

            item.IsDirty = false;
        }

        public async Task SaveProjectAsync()
        {
            var newPath = await _filePickerService.PickSaveProjectAsync();
            if(string.IsNullOrEmpty(newPath))
            {
                return;
            }

            var oldPath = ProjectFolderPath;
            Directory.CreateDirectory(newPath);

            var allItems = CurrentProject.Notes.Cast<ModelBase>()
                .Concat(CurrentProject.Documents.Cast<ModelBase>())
                .Concat(CurrentProject.Screenplays.Cast<ModelBase>())
                .Concat(CurrentProject.Drawings.Cast<ModelBase>())
                .Concat(CurrentProject.Moodboards.Cast<ModelBase>())
                .Concat(CurrentProject.Storyboards.Cast<ModelBase>())
                .Concat(CurrentProject.Musics.Cast<ModelBase>());
            foreach(var item in allItems)
            {
                var directoryPath = Path.Combine(newPath, "data", item.FilePathPrefix);
                Directory.CreateDirectory(directoryPath);

                var itemPath = Path.Combine(directoryPath, $"{item.Id}.json");
                var json = JsonSerializer.Serialize(item as object, item.GetType());

                await File.WriteAllTextAsync(itemPath, json);
            }

            if(oldPath != null && oldPath.Contains(Path.GetTempPath()))
            {
                try
                {
                    Directory.Delete(oldPath, true);
                }
                catch { }
            }

            ProjectFolderPath = newPath;
            CurrentProject.ProjectName = Path.GetFileName(newPath);
            _isTemporaryProject = false;
            ResetDirtyState();
        }

        public async Task SaveProjectAsAsync()
        {
            var newPath = await _filePickerService.PickSaveProjectAsync();
            if(string.IsNullOrEmpty(newPath))
            {
                return;
            }

            var oldTempPath = _isTemporaryProject ? ProjectFolderPath : null;

            // 1. update state to new permanent location
            ProjectFolderPath = newPath;
            CurrentProject.ProjectName = Path.GetFileName(newPath);

            // 2. create main data directory
            var dataDirectory = Path.Combine(ProjectFolderPath, "data");
            Directory.CreateDirectory(dataDirectory);

            // 3. save every single item to new location
            var allItems = CurrentProject.Notes.Cast<ModelBase>()
                .Concat(CurrentProject.Documents.Cast<ModelBase>())
                .Concat(CurrentProject.Screenplays.Cast<ModelBase>())
                .Concat(CurrentProject.Drawings.Cast<ModelBase>())
                .Concat(CurrentProject.Moodboards.Cast<ModelBase>())
                .Concat(CurrentProject.Storyboards.Cast<ModelBase>())
                .Concat(CurrentProject.Musics.Cast<ModelBase>());
            foreach(var item in allItems)
            {
                var itemDirectory = Path.Combine(dataDirectory, item.FilePathPrefix);
                Directory.CreateDirectory(itemDirectory);

                var itemPath = Path.Combine(itemDirectory, $"{item.Id}.json");
                var json = JsonSerializer.Serialize(item as object, item.GetType());

                await File.WriteAllTextAsync(itemPath, json);
            }

            // 4. save the main project manifest
            var projectManifestPath = Path.Combine(ProjectFolderPath, "project.json");
            var projectJson = JsonSerializer.Serialize(CurrentProject);

            await File.WriteAllTextAsync(projectManifestPath, projectJson);

            // 5. if successful, mark the project as not temp
            _isTemporaryProject = false;
            if(!string.IsNullOrEmpty(oldTempPath) && Directory.Exists(oldTempPath))
            {
                Directory.Delete(oldTempPath, true);
            }
            ResetDirtyState();
        }

        public async Task LoadProjectAsync()
        {
            var path = await _filePickerService.PickOpenProjectAsync();
            if(string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                return;
            }

            var json = await File.ReadAllTextAsync(path);
            var loadedProject = JsonSerializer.Deserialize<Project>(json);
            if(loadedProject != null)
            {
                CurrentProject = loadedProject;
                ProjectFolderPath = path;
            }
        }

        public void CleanupTemporaryProject()
        {
            if(_isTemporaryProject && !string.IsNullOrEmpty(ProjectFolderPath) && Directory.Exists(ProjectFolderPath))
            {
                try
                {
                    Directory.Delete(ProjectFolderPath, recursive: true);
                }
                catch
                {

                }
            }
        }

        public void ResetDirtyState()
        {
            var allItems = CurrentProject.Notes.Cast<ModelBase>()
                .Concat(CurrentProject.Documents.Cast<ModelBase>())
                .Concat(CurrentProject.Screenplays.Cast<ModelBase>())
                .Concat(CurrentProject.Drawings.Cast<ModelBase>())
                .Concat(CurrentProject.Moodboards.Cast<ModelBase>())
                .Concat(CurrentProject.Storyboards.Cast<ModelBase>())
                .Concat(CurrentProject.Musics.Cast<ModelBase>());
            foreach(var item in allItems)
            {
                item.IsDirty = false;
            }
        }
    }
}
