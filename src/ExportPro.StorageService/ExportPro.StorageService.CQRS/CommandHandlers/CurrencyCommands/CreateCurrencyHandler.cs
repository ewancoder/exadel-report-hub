using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Responses;
using MediatR;

namespace ExportPro.StorageService.CQRS.CommandHandlers.CurrencyCommands;

public record CreateCurrencyCommand(string Code) : IRequest<BaseResponse<CurrencyResponse>>;
public sealed class CreateCurrencyHandler(ICurrencyRepository repository, IMapper mapper) : IRequestHandler<CreateCurrencyCommand, BaseResponse<CurrencyResponse>>
{
    private readonly ICurrencyRepository _repository = repository;
    private readonly IMapper _mapper = mapper;

    public async Task<BaseResponse<CurrencyResponse>> Handle(CreateCurrencyCommand request, CancellationToken cancellationToken)
    {
        var currency = new Currency
        {
            CurrencyCode = request.Code
        };

        await _repository.AddOneAsync(currency, cancellationToken);
        var curResponse = _mapper.Map<CurrencyResponse>(currency);
        return new BaseResponse<CurrencyResponse> { Data = curResponse };
    }
}