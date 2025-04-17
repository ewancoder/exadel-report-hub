using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs.CustomerDTO;

namespace ExportPro.StorageService.CQRS.Queries.CustomerQueries;

public class GetAllCustomersQuery : IQuery<List<CustomerDto>> { }
