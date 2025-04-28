using ExportPro.Common.Shared.Models;
using Refit;

namespace ExportPro.Common.Shared.Refit
{
    public interface IACLSharedApi
    {
        [Post("/api/permissions/check")]
        Task<bool> CheckPermissionAsync([Body] CheckPermissionRequest request);
    }
}
