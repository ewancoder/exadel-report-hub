using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExportPro.StorageService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;



}

