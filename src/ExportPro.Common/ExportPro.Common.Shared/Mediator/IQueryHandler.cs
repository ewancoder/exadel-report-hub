using ExportPro.Common.Shared.Library;
using MediatR;

namespace ExportPro.Common.Shared.Mediator
{
    public interface IQueryHandler<TQuery, TResponse>
        : IRequestHandler<TQuery, Result<TResponse>>
        where TQuery : IQuery<TResponse>
    {
    }
}
