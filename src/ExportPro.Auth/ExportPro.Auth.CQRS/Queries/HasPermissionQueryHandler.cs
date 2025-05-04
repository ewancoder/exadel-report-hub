using ExportPro.AuthService.Services;
using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using MediatR;
using MongoDB.Bson;


namespace ExportPro.Auth.CQRS.Queries;

public record HasPermissionQuery(
    string UserId,
    string? ClientId,
    Resource Resource,
    CrudAction Action
) : IQuery<bool>;

public class HasPermissionQueryHandler(IACLService aclService) : IQueryHandler<HasPermissionQuery, bool>
{

    public async Task<BaseResponse<bool>> Handle(HasPermissionQuery request, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(request.UserId, out var userId))
            return new BadRequestResponse<bool>("Invalid UserId");

        ObjectId clientId = ObjectId.Empty;

        if (!string.IsNullOrEmpty(request.ClientId))
        {
            if (!ObjectId.TryParse(request.ClientId, out clientId))
                return new BadRequestResponse<bool>("Invalid ClientId");
        }

        var hasPermission = await aclService.HasPermission(
            userId,
            clientId,
            request.Resource,
            request.Action,
            cancellationToken
        );

        if (!hasPermission)
            return new BadRequestResponse<bool>("Permission denied");

        return new SuccessResponse<bool>(true);
    }
}