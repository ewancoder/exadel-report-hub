using System.Net;
using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.DTOs.CountryDTO;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.QueryHandlers.CountryQueries;

public record GetCountryByIdQuery(Guid Id) : IQuery<CountryDto>;

public class GetCountryByIdQueryHandler(ICountryRepository repository, IMapper mapper)
    : IQueryHandler<GetCountryByIdQuery, CountryDto>
{
    public async Task<BaseResponse<CountryDto>> Handle(GetCountryByIdQuery request, CancellationToken cancellationToken)
    {
        var country = await repository.GetOneAsync(
            x => x.Id == request.Id.ToObjectId() && !x.IsDeleted,
            cancellationToken
        );
        if (country == null)
        {
            return new NotFoundResponse<CountryDto>() { Messages = ["Country not found."] };
        }
        var countryResp = mapper.Map<CountryDto>(country);
        return new SuccessResponse<CountryDto>(countryResp, "Country  found successfully.");
    }
}
