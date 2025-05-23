﻿namespace ExportPro.StorageService.SDK.DTOs.CountryDTO;

public sealed class CountryDto
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? CurrencyId { get; set; }
}
