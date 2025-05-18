using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Models;
using Refit;

namespace ExportPro.Common.Shared.Refit
{
    public interface IACLSharedApi
    {
        [Post("/api/permissions/check")]
        Task<BaseResponse<bool>> CheckPermissionAsync(   CheckPermissionRequest request);

        [Get("/api/permissions/user-clients")]
        Task<BaseResponse<List<Guid>>> GetUserClientsAsync(CancellationToken cancellationToken = default);
        [Post("/api/permissions/grant/{userId}/{clientId}/{role}")]
        Task<BaseResponse<bool>> GrantPermissionAsync(Guid userId, Guid clientId, UserRole role);
        [Post("/api/update-role/{userId}/{clientId}/{role}")]
        Task<BaseResponse<bool>> UpdateUserRoleAsync(Guid userId, Guid clientId, UserRole role);
        [Delete("/api/permissions/{userId}")]
        Task<BaseResponse<bool>> DeletePermissionAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
