using ExportPro.Common.Shared.Mediator;

namespace ExportPro.AuthService.Commands;

public record LogoutCommand(string RefreshToken) : ICommand;