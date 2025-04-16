using ExportPro.Common.Shared.Library;
using MediatR;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Commands.CurrencyCommand;

public class DeleteCurrencyCommand : IRequest<BaseResponse<bool>>
{
    public ObjectId Id { get; set; }
}