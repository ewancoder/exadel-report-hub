using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.Models.Models;

namespace ExportPro.StorageService.CQRS.Queries.invoice;

public class GetAllInvoicesQuery : IQuery<List<Invoice>> { }