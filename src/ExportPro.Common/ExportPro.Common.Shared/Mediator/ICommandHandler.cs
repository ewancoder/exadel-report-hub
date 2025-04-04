using MediatR;
using ExportPro.Common.Shared.Library;

namespace ExportPro.Common.Shared.Mediator;

public interface ICommandHandler<TCommand, TResponse>
    : IRequestHandler<TCommand, BaseResponse<TResponse>>
    where TCommand : ICommand<TResponse>
{
}
