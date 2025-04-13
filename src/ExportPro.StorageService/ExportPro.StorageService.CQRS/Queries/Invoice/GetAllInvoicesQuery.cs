using ExportPro.Common.Shared.Mediator;

namespace ExportPro.StorageService.CQRS.Queries.Invoice;

public class GetAllInvoicesQuery : IQuery<List<Models.Models.Invoice>> { }