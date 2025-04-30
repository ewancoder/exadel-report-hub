using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace ExportPro.StorageService.SDK.Responses;

public sealed class CurrencyResponse
{
    public required Guid Id { get; set; }
    public required string CurrencyCode { get; set; } // e.g. "USD"
}
