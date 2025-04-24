namespace ExportPro.StorageService.SDK.DTOs.CustomerDTOs;

public class CreateUpdateCustomerDto
{
    public required string Name { get; set; } 
    public required string Email { get; set; }
    public string? CountryId { get; set; } 
}