using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Responses;
using MediatR;

namespace ExportPro.StorageService.CQRS.Commands.CurrencyCommand;

public class CreateCurrencyCommand : IRequest<BaseResponse<CurrencyResponse>>
{
    public required string Code { get; set; }
}