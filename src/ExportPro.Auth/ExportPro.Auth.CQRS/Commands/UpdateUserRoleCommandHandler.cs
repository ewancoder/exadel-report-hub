﻿using ExportPro.AuthService.Services;
using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Helpers;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using MongoDB.Bson;

namespace ExportPro.Auth.CQRS.Commands;

public record UpdateUserRoleCommand(ObjectId UserId, ObjectId ClientId, UserRole NewRole)
    : ICommand<bool>,
        IPermissionedRequest
{
    public List<Guid>? ClientIds => [ClientId.ToGuid()];
    public Resource Resource => Resource.Users;
    public CrudAction Action => CrudAction.Update;
}

public class UpdateUserRoleCommandHandler(IACLService aclService) : ICommandHandler<UpdateUserRoleCommand, bool>
{
    public async Task<BaseResponse<bool>> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
    {
        var res = await aclService.UpdateUserRole(request.UserId, request.ClientId, request.NewRole, cancellationToken);
        if (!res)
            return new BadRequestResponse<bool>("Failed to update user role");
        return new SuccessResponse<bool>(true);
    }
}
