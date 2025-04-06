using ExportPro.Auth.SDK.DTOs;
using ExportPro.AuthService.Services;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;

namespace ExportPro.Auth.CQRS.Commands;

public record RegisterCommand(UserRegisterDto RegisterDto) : ICommand<AuthResponseDto>;

public class RegisterCommandHandler(IAuthService authService)
    : ICommandHandler<RegisterCommand, AuthResponseDto>
{
    private readonly IAuthService _authService = authService;

    public async Task<BaseResponse<AuthResponseDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request.RegisterDto);
        return new SuccessResponse<AuthResponseDto>(result);
    }
}
