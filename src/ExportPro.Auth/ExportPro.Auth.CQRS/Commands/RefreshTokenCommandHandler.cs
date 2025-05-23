﻿using ExportPro.Auth.SDK.DTOs;
using ExportPro.AuthService.Services;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;

namespace ExportPro.Auth.CQRS.Commands;

public record RefreshTokenCommand(string RefreshToken) : ICommand<AuthResponseDto>;

public class RefreshTokenCommandHandler(IAuthService authService)
    : ICommandHandler<RefreshTokenCommand, AuthResponseDto>
{
    public async Task<BaseResponse<AuthResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var result = await authService.RefreshTokenAsync(request.RefreshToken);
        return new SuccessResponse<AuthResponseDto>(result);
    }
}
