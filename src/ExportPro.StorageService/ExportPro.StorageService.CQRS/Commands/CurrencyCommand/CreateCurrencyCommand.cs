using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.Models.Models;
using MediatR;

namespace ExportPro.StorageService.CQRS.Commands.CurrencyCommand;

public class CreateCurrencyCommand : IRequest<BaseResponse<Currency>>
{
    public required string Name { get; set; } 
    public required string Code { get; set; }
}