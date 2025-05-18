using Microsoft.AspNetCore.Components;

namespace ExportPro.Front.Pages;

public partial class Clients
{
    [Inject] private HttpClient HttpClient { get; set; } = null!;   


}

