using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.Models.Models;
using MediatR;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Queries.CurrencyQueries;
public class GetCurrencyByIdQuery : IRequest<BaseResponse<Currency>>
{
    public ObjectId Id { get; set; }
}