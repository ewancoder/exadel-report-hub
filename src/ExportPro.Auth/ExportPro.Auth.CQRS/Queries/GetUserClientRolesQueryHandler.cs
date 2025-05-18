using ExportPro.Auth.SDK.Models;
using ExportPro.AuthService.Services;
using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Helpers;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.Common.Shared.Models;
using Microsoft.AspNetCore.Http;

namespace ExportPro.Auth.CQRS.Queries;


public record GetUserClientRolesQuery : IQuery<List<UserClientRolesDTO>>, IPermissionedRequest
{
    public List<Guid>? ClientIds => null;
    public Resource Resource => Resource.Users;
    public CrudAction Action => CrudAction.Read;
};

public sealed class GetUserClientRolesQueryHandler(IACLService aCLService, IHttpContextAccessor httpContextAccessor) : IQueryHandler<GetUserClientRolesQuery, List<UserClientRolesDTO>>
{
    public async  Task<BaseResponse<List<UserClientRolesDTO>>> Handle(GetUserClientRolesQuery request, CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.HttpContext?.User;
        if (user == null)
            return new BadRequestResponse<List<UserClientRolesDTO>>("User context not found.");
        
        var clientRoles = await aCLService.GetAccessibleUserRolesAsync(TokenHelper.GetUserId(user), cancellationToken);
        if (clientRoles == null)
            return new BadRequestResponse<List<UserClientRolesDTO>>("User roles not found.");
        
        var clientRolesDto = clientRoles.Select(cr => new UserClientRolesDTO
        {
            ClientId = cr.ClientId.ToGuid(), 
            Role = cr.Role
        }).ToList();

        return new SuccessResponse<List<UserClientRolesDTO>>(clientRolesDto);
    }
}

