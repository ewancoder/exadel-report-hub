using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.Models.Models;
using MediatR;

namespace ExportPro.StorageService.CQRS.Queries.CurrencyQueries;

public class GetAllCurrenciesQuery : IRequest<BaseResponse<List<Currency>>> { }