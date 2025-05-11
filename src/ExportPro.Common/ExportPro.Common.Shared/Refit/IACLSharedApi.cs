using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Refit;

namespace ExportPro.Common.Shared.Refit
{
    public interface IACLSharedApi
    {
        [Post("/api/permissions/check")]
        Task<BaseResponse<bool>> CheckPermissionAsync(CheckPermissionRequest request);

        [Get("/api/permissions/user-clients")]
        Task<List<Guid>> GetUserClientsAsync(CancellationToken cancellationToken = default);
    }
}
