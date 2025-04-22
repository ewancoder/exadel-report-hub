using ExportPro.Export.CQRS.Queries;
using Microsoft.AspNetCore.Mvc;

namespace ExportPro.Export.ServiceHost.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InvoicePdfExportController : ControllerBase
{
    [HttpGet("{id}/pdf")]
    public async Task<IActionResult> GetPdf(string id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
