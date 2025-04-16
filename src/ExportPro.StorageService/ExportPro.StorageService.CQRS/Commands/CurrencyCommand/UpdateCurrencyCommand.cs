using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.Models.Models;
using MediatR;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Commands.CurrencyCommand;

public class UpdateCurrencyCommand : IRequest<BaseResponse<Currency>>
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
}