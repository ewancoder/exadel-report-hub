﻿namespace ExportPro.StorageService.SDK.DTOs.CustomerDTO;

public sealed class CreateUpdateCustomerDto
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Address { get; set; }
    public Guid CountryId { get; set; }
}
