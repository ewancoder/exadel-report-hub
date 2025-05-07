using ClosedXML.Excel;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.CommandHandlers.CustomerCommands;

public sealed record ImportCustomersCommand(IFormFile? File) : ICommand<int>;

public sealed class ImportCustomersCommandHandler(ICustomerRepository repository)
    : ICommandHandler<ImportCustomersCommand, int>
{
    public async Task<BaseResponse<int>> Handle(ImportCustomersCommand request, CancellationToken ct)
    {
        if (request.File == null || request.File.Length == 0)
            return new BadRequestResponse<int>("File is empty.");

        var customers = new List<Customer>();

        using var stream = request.File.OpenReadStream();
        using var wb = new XLWorkbook(stream);
        var ws = wb.Worksheets.First(); // first sheet

        foreach (var row in ws.RowsUsed().Skip(1)) // skip header
        {
            var name = row.Cell(1).GetString().Trim();
            var email = row.Cell(2).GetString().Trim();
            var cidRaw = row.Cell(3).GetString().Trim();

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(cidRaw) ||
                !Guid.TryParse(cidRaw, out var cid))
                continue; // ignore bad lines

            customers.Add(new Customer
            {
                Id = ObjectId.GenerateNewId(),
                Name = name,
                Email = email,
                CountryId = cid.ToObjectId(),
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            });
        }

        var inserted = await repository.AddManyAsync(customers, ct);
        return new SuccessResponse<int>(inserted, $"Imported {inserted} customers.");
    }
}