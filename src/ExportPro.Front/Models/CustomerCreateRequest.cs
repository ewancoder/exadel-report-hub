namespace ExportPro.Front.Models
{
    public class CustomerCreateRequest
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public Guid CountryId { get; set; }
    }
}
