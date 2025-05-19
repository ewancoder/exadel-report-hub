namespace ExportPro.Front.Models;
public sealed class CreateUpdateCustomerDto
{
    public string Name { get; set; }
    public  string Email { get; set; }
    public  string Address { get; set; }
    public Guid CountryId { get; set; }
}