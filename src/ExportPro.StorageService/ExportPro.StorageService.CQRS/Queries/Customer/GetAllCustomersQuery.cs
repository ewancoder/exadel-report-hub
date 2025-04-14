using ExportPro.Common.Shared.Mediator;

namespace ExportPro.StorageService.CQRS.Queries.Customer;

public class GetAllCustomersQuery : IQuery<List<Models.Models.Customer>> { }
