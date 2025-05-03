namespace ExportPro.Export.SDK.DTOs;

public sealed class ReportFilterDto
{
    public Guid? ClientId { get; set; }
    public List<Guid>? ClientIds { get; set; }
    
    public DateTime? IssueDateFrom { get; set; }
}