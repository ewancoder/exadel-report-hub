namespace ExportPro.Front.Models
{
    public class ItemInsertRequest
    {
        public Item? Item { get; set; }
        public Guid ClientId { get; set; }
    }

    public class Item
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public Status Status { get; set; } = Status.Unpaid;
        public Guid CurrencyId { get; set; }
    }
}
