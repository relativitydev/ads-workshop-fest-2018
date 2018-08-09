using OverrideCustomPage.Models;
using Relativity.API;
using System.Threading.Tasks;

namespace OverrideCustomPage.Helpers
{
    public interface IApiHelper
    {
        IAPILog ApiLog { get; set; }
        Task SaveNewValuesAsync(NewRdoModel newRdoModel);
        Task<EditRdoModel> RetrieveCurrentValuesAsync(int workspaceArtifactId, int rdoArtifactId);
        Task UpdateNewValuesAsync(EditRdoModel editRdoModel);
    }
}