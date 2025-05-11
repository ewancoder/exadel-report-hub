using ExportPro.AuthService.Services;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using Microsoft.AspNetCore.Http;


namespace ExportPro.Auth.CQRS.Queries
{
    public record GetUserClientsQuery : IQuery<List<string>>;

    public class GetUserClientsQueryHandler(IACLService aclService, IHttpContextAccessor httpContextAccessor) : IQueryHandler<GetUserClientsQuery, List<string>>
    {
        public async Task<BaseResponse<List<string>>> Handle(GetUserClientsQuery request, CancellationToken cancellationToken)
        {
            var user = httpContextAccessor.HttpContext?.User;
            if (user == null)
                return new BadRequestResponse<List<string>>("User context not found.");
            
            var clientObjectIds = await aclService.GetAccessibleClientIdsAsync(TokenHelper.GetUserId(user), cancellationToken);
            var clientIds = clientObjectIds.Select(id => id.ToString()).ToList();

            return new SuccessResponse<List<string>>(clientIds);
        }
    }
}
