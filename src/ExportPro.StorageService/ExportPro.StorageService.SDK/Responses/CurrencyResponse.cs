using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace ExportPro.StorageService.SDK.Responses;

public class CurrencyResponse
{
    public string? Id { get; set; }
    public required string CurrencyCode { get; set; }   // e.g. "USD"
}
