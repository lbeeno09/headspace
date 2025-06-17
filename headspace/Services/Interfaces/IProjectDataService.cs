using headspace.Models.Common;
using System.Threading.Tasks;
using Windows.Storage;

namespace headspace.Services.Interfaces
{
    public interface IProjectDataService
    {
        // Save/Load Object
        Task<Project> LoadProjectAsync(string fileName, StorageFile? file = null);
        Task SaveProjectAsync(Project projectToSave, string fileName, StorageFile? file = null);

        void CreateNewProject();

        void SetProjectName(Project project, string newName);
    }
}
