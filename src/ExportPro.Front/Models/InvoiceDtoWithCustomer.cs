namespace ExportPro.Front.Models
{
    public class InvoiceDtoWithCustomer
    {
       public InvoiceDto Invoice { get; set; } 
       public CustomerResponse Customer { get; set; }
    }
}
