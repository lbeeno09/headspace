using headspace.Models.Common;
using System.Threading.Tasks;

namespace headspace.Services.Interfaces
{
    public interface IProjectService
    {
        Project CurrentProject { get; }

        Task<bool> CreateNewProject(bool isInitialLaunch = false);

        Task SaveItemAsync(ModelBase item);
        Task SaveProjectAsync();
        Task SaveProjectAsAsync();

        Task LoadProjectAsync();
    }
}
