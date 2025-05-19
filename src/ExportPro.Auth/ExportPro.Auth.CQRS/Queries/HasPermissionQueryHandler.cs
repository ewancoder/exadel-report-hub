using ExportPro.AuthService.Services;
using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;

namespace ExportPro.Auth.CQRS.Queries;

public record HasPermissionQuery(Guid UserId, Guid? ClientId, Resource Resource, CrudAction Action) : IQuery<bool>;

public class HasPermissionQueryHandler(IACLService aclService) : IQueryHandler<HasPermissionQuery, bool>
{
    public async Task<BaseResponse<bool>> Handle(HasPermissionQuery request, CancellationToken cancellationToken)
    {
        var hasPermission = await aclService.HasPermission(
            request.UserId.ToObjectId(),
            request.ClientId.GetValueOrDefault().ToObjectId(),
            request.Resource,
            request.Action,
            cancellationToken
        );

        if (!hasPermission)
            return new BadRequestResponse<bool>("Permission denied");

        return new SuccessResponse<bool>(true);
    }
}
