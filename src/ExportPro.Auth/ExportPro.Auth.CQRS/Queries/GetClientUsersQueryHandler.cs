using ExportPro.Auth.SDK.DTOs;
using ExportPro.AuthService.Repositories;
using ExportPro.AuthService.Services;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;

namespace ExportPro.Auth.CQRS.Queries;

public record GetClientUsers(Guid ClientId) : IQuery<List<UserInfo>>;

public record GetClientUsersQueryHandler(IACLRepository aclRepository, UserRepository Repository)
    : IQueryHandler<GetClientUsers, List<UserInfo>>
{
    public async Task<BaseResponse<List<UserInfo>>> Handle(GetClientUsers request, CancellationToken cancellationToken)
    {
        var clientUsers = await aclRepository.GetClientUsers(request.ClientId.ToObjectId(), cancellationToken);
        if (clientUsers == null)
            return new BadRequestResponse<List<UserInfo>>("Client users not found.");
        List<UserInfo> userInfos = new();
        foreach (var userId in clientUsers)
        {
            var user = await Repository.GetByIdAsync(userId.ToObjectId(), cancellationToken);
            if (user == null)
                continue;
            userInfos.Add(
                new UserInfo
                {
                    Id = user.Id.ToGuid(),
                    Username = user.Username,
                    Email = user.Email,
                }
            );
        }
        return new SuccessResponse<List<UserInfo>>(userInfos);
    }
}
