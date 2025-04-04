using ExportPro.Common.Shared.DTOs;
using ExportPro.Common.Shared.Mediator;

namespace ExportPro.AuthService.Commands;

public record LoginCommand(UserLoginDto LoginDto) : ICommand<AuthResponseDto>;