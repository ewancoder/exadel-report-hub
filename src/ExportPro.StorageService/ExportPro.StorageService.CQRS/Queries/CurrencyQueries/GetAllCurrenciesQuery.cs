using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Responses;
using MediatR;

namespace ExportPro.StorageService.CQRS.Queries.CurrencyQueries;

public class GetAllCurrenciesQuery : IRequest<BaseResponse<List<CurrencyResponse>>> { }