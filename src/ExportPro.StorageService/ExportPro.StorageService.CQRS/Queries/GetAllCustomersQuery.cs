using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.Models.Models;

namespace ExportPro.StorageService.CQRS.Queries;

public class GetAllCustomersQuery : IQuery<List<Customer>> { }
