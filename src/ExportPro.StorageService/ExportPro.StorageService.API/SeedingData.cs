using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.PaginationParams;
using ExportPro.StorageService.SDK.Refit;
using MongoDB.Driver;
using MongoDB.Driver.Core;
using Refit;
using ILogger = Serilog.ILogger;

namespace ExportPro.StorageService.API;


public class SeedingData(ICountryRepository countryRepository, ILogger logger, IConfiguration configuration);
    // public async Task SeedCountries()
    // {
    //     PaginationParameters paginationParameters = new PaginationParameters()
    //     {
    //         PageNumber=1,
    //         PageSize=100
    //     };
    //     var countriesList = await countryRepository.GetAllPaginatedAsync(paginationParameters,CancellationToken.None);
    //     var cnt = countriesList.Items.Count;
    //     logger.Information("Countries count: {Count}", cnt);
    //     if (cnt == 0)
    //     {
    //         logger.Information("Seeding countries");
    //         var restCountries = RestService.For<IRestCountries>(configuration["Refit:restcountries"]!);
    //         var countries = await restCountries.GetAllCountries();
    //
    //         foreach (var i in countries)
    //         {
    //             var name=i.Name?.Common ?? "";
    //             var cioc = i.Cioc ?? "";
    //             var currency = i.Currencies.Keys.FirstOrDefault() ?? "";
    //             if(cioc == null || currency == null || name == null)
    //             {
    //                 continue;
    //             }
    //             logger.Debug($"Seeding country: {name} Cioc {cioc} Currency: {currency}");
    //         }
    //     }
