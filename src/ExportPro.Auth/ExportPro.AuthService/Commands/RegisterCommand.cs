using ExportPro.Common.Shared.DTOs;
using ExportPro.Common.Shared.Mediator;

namespace ExportPro.AuthService.Commands;

public record RegisterCommand(UserRegisterDto RegisterDto) : ICommand<AuthResponseDto>;
