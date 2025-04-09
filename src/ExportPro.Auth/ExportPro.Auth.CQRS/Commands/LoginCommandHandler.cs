using ExportPro.AuthService.Services;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.Common.Shared.DTOs;
namespace ExportPro.Auth.CQRS.Commands;

public record LoginCommand(UserLoginDto LoginDto) : ICommand<AuthResponseDto>;

public class LoginCommandHandler(IAuthService authService)
    : ICommandHandler<LoginCommand, AuthResponseDto>
{
    public async Task<BaseResponse<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(request.LoginDto);
        return new SuccessResponse<AuthResponseDto>(result);
    }
}
