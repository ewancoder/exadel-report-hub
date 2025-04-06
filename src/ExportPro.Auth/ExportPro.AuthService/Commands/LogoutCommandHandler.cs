using ExportPro.AuthService.Services;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using MediatR;

namespace ExportPro.AuthService.Commands;

public record LogoutCommand(string RefreshToken) : ICommand;

public class LogoutCommandHandler(IAuthService authService)
    : IRequestHandler<LogoutCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await authService.LogoutAsync(request.RefreshToken);
        return new SuccessApiResponse("Logged out successfully.");
    }
}
