using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Responses;
using MediatR;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Queries.CurrencyQueries;
public class GetCurrencyByIdQuery : IRequest<BaseResponse<CurrencyResponse>>
{
    public ObjectId Id { get; set; }
}