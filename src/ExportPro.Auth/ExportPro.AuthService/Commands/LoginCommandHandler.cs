using ExportPro.AuthService.Services;
using ExportPro.Common.Shared.DTOs;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;

namespace ExportPro.AuthService.Commands;

public class LoginCommandHandler(IAuthService authService)
    : ICommandHandler<LoginCommand, AuthResponseDto>
{
    public async Task<BaseResponse<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await authService.LoginAsync(request.LoginDto);
            return new SuccessResponse<AuthResponseDto>(result);
        }
        catch (Exception ex)
        {
            return new BaseResponse<AuthResponseDto>
            {
                IsSuccess = false,
                ApiState = System.Net.HttpStatusCode.Unauthorized,
                Messages = [ex.Message]
            };
        }
    }
}