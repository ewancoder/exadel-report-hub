using ExportPro.Common.Shared.Library;
using MediatR;

namespace ExportPro.Common.Shared.Mediator
{
    public interface IQueryHandler<TQuery, TResponse>
        : IRequestHandler<TQuery, BaseResponse<TResponse>>
        where TQuery : IQuery<TResponse>
    {
    }
}
