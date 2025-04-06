using ExportPro.AuthService.Services;
using ExportPro.Common.Shared.DTOs;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;

namespace ExportPro.AuthService.Commands;

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
