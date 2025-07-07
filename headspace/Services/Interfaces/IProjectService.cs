using headspace.Models.Common;
using System.Threading.Tasks;

namespace headspace.Services.Interfaces
{
    public interface IProjectService
    {
        Project CurrentProject { get; }

        void CreateNewProject();
        Task SaveProjectAsync();
        Task SaveProjectAsAsync();

        Task LoadProjectAsync();
    }
}
