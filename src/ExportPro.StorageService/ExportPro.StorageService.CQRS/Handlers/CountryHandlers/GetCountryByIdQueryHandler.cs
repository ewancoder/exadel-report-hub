using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Queries.CountryQueries;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.DTOs.CountryDTO;
using MongoDB.Bson;
using System.Net;

namespace ExportPro.StorageService.CQRS.Handlers.CountryHandlers;

public class GetCountryByIdQueryHandler(ICountryRepository repository, IMapper mapper) : IQueryHandler<GetCountryByIdQuery, CountryDto>
{
    public async Task<BaseResponse<CountryDto>> Handle(GetCountryByIdQuery request, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(request.Id, out var objectId))
        {
            return new BaseResponse<CountryDto>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.BadRequest,
                Messages = new List<string> { "Invalid country ID format." }
            };
        }

        var country = await repository.GetByIdAsync(objectId, cancellationToken);
        if (country == null)
        {
            return new BaseResponse<CountryDto>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.NotFound,
                Messages = new List<string> { "Country not found." }
            };
        }

        return new BaseResponse<CountryDto>
        {
            IsSuccess = true,
            ApiState = HttpStatusCode.OK,
            Data = mapper.Map<CountryDto>(country)
        };
    }
}
