using ExportPro.AuthService.Services;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using Microsoft.AspNetCore.Http;

namespace ExportPro.Auth.CQRS.Queries;

public record GetUserClientsQuery : IQuery<List<Guid>>;

public class GetUserClientsQueryHandler(IACLService aclService, IHttpContextAccessor httpContextAccessor)
    : IQueryHandler<GetUserClientsQuery, List<Guid>>
{
    public async Task<BaseResponse<List<Guid>>> Handle(GetUserClientsQuery request, CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.HttpContext?.User;
        if (user == null)
            return new BadRequestResponse<List<Guid>>("User context not found.");

        var clientObjectIds = await aclService.GetAccessibleUserRolesAsync(
            TokenHelper.GetUserId(user),
            cancellationToken
        );
        var clientIds = clientObjectIds.Select(id => id.ClientId.ToGuid()).ToList();

        return new SuccessResponse<List<Guid>>(clientIds);
    }
}
