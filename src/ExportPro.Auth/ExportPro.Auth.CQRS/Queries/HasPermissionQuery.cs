using ExportPro.AuthService.Services;
using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using MediatR;
using MongoDB.Bson;


namespace ExportPro.Auth.CQRS.Queries;
public record HasPermissionQuery(
    ObjectId UserId,
    ObjectId ClientId,
    Resource Resource,
    CrudAction Action
) : IQuery<bool>;

public class HasPermissionQueryHandler(IACLService aclService) : IRequestHandler<HasPermissionQuery, BaseResponse<bool>>
{

    public async Task<BaseResponse<bool>> Handle(HasPermissionQuery request, CancellationToken cancellationToken)
    {
        var hasPermission = await aclService.HasPermission(
            request.UserId,
            request.ClientId,
            request.Resource,
            request.Action,
            cancellationToken
        );
        if(!hasPermission)
           return new BadRequestResponse<bool>("Permission denied");

        return new SuccessResponse<bool>(hasPermission);
    }
}