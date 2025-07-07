using CommunityToolkit.Mvvm.ComponentModel;
using headspace.Models.Common;
using headspace.Services.Interfaces;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

// Project Data Manager

namespace headspace.Services.Implementations
{
    public partial class ProjectService : ObservableObject, IProjectService
    {
        [ObservableProperty]
        private Project _currentProject;

        public string? ProjectFilePath { get; private set; }
        private bool _isTemporaryProject;

        private readonly IFilePickerService _filePickerService;

        public ProjectService(IFilePickerService filePickerService)
        {
            _filePickerService = filePickerService;

            CreateNewProject();
        }

        public void CreateNewProject()
        {
            // 1. Generate path for temporary file
            string tempFileName = $"temp_project_{Guid.NewGuid()}.hsp";
            string tempPath = Path.Combine(Path.GetTempPath(), tempFileName);

            // 2. Set project state
            CurrentProject = new Project { ProjectName = "New Project" };
            ProjectFilePath = tempPath;
            _isTemporaryProject = true;

            // 3. Perform initial save on temp file
            Task.Run(SaveProjectAsync);
        }

        public async Task SaveProjectAsync()
        {
            if(string.IsNullOrEmpty(ProjectFilePath))
            {
                await SaveProjectAsAsync();
            }
            else
            {
                var json = JsonSerializer.Serialize(CurrentProject);
                await File.WriteAllTextAsync(ProjectFilePath, json);

                ResetDirtyState();
            }
        }

        public async Task SaveProjectAsAsync()
        {
            var newPath = await _filePickerService.PickSaveFileAsync();
            if(string.IsNullOrEmpty(newPath))
            {
                return;
            }

            if(_isTemporaryProject && !string.IsNullOrEmpty(ProjectFilePath) && File.Exists(ProjectFilePath))
            {
                File.Delete(ProjectFilePath);
            }

            // Update the temp file to permanent path
            ProjectFilePath = newPath;
            CurrentProject.ProjectName = Path.GetFileNameWithoutExtension(newPath);
            _isTemporaryProject = false;

            await SaveProjectAsync();
        }

        public void CleanupTemporaryProject()
        {
            if(_isTemporaryProject && !string.IsNullOrEmpty(ProjectFilePath) && File.Exists(ProjectFilePath))
            {
                try
                {
                    File.Delete(ProjectFilePath);
                }
                catch(Exception ex)
                {

                }
            }
        }

        private void ResetDirtyState()
        {
            foreach(var item in CurrentProject.Notes)
            {
                item.IsDirty = false;
            }
            foreach(var item in CurrentProject.Documents)
            {
                item.IsDirty = false;
            }
            foreach(var item in CurrentProject.Screenplays)
            {
                item.IsDirty = false;
            }
            foreach(var item in CurrentProject.Drawings)
            {
                item.IsDirty = false;
            }
            foreach(var item in CurrentProject.Moodboards)
            {
                item.IsDirty = false;
            }
            foreach(var item in CurrentProject.Storyboards)
            {
                item.IsDirty = false;
            }
            foreach(var item in CurrentProject.Musics)
            {
                item.IsDirty = false;
            }
        }

        public async Task LoadProjectAsync()
        {
            var path = await _filePickerService.PickOpenFileAsync();
            if(string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                return;
            }

            var json = await File.ReadAllTextAsync(path);
            var loadedProject = JsonSerializer.Deserialize<Project>(json);
            if(loadedProject != null)
            {
                CurrentProject = loadedProject;
                ProjectFilePath = path;
                ResetDirtyState();
            }
        }
    }
}
