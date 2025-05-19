namespace ExportPro.Front.Models;
public sealed class CustomerResponse
{
    public  Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public Guid CountryId { get; set; }
    public bool IsDeleted { get; set; }
}
