using ExportPro.Export.SDK.DTOs;
using MediatR;

namespace ExportPro.Export.CQRS.Queries;

public sealed record GenerateInvoiceReportQuery(ReportFilterDto Filters) : IRequest<ReportFileDto>;

public sealed class GenerateInvoiceReportQueryHandler : IRequestHandler<GenerateInvoiceReportQuery, ReportFileDto>
{
    public async Task<ReportFileDto> Handle(GenerateInvoiceReportQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
