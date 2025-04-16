using System.ComponentModel.DataAnnotations;
using AutoMapper;
using ExportPro.Auth.SDK.DTOs;
using ExportPro.Auth.SDK.Interfaces;
using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.CQRS.Handlers.Client;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.DataAccess.Repositories;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Swashbuckle.AspNetCore.Annotations;

namespace ExportPro.StorageService.API.Controllers;

[Route("api/client/")]
[ApiController]
public class ClientController(IMapper mapper, IAuth auth, IClientRepository clientRepository, IMediator mediator)
    : ControllerBase
{
    private readonly IAuth _auth = auth;
    private readonly IClientRepository _clientRepository = clientRepository;
    private readonly IMediator _mediator = mediator;
    private readonly IMapper _mapper = mapper;

    [HttpPost]
    [SwaggerOperation(Summary = "Creating a client")]
    [ProducesResponseType(typeof(ClientResponse), 200)]
    public async Task<IActionResult> CreateClient([FromBody] ClientDto clientdto)
    {
        var clientResponse = await _mediator.Send(new CreateClientCommand(clientdto));
        return Ok(clientResponse);
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Getting  clients")]
    [ProducesResponseType(typeof(List<ClientResponse>), 200)]
    public async Task<IActionResult> GetClients([FromQuery] int top = 5, [FromQuery] int skip = 1)
    {
        var clients = await _mediator.Send(new GetClientsQuery(top, skip));
        return Ok(clients);
    }

    [HttpGet("{clientId}")]
    [SwaggerOperation(Summary = "Getting  client by client id which is not soft deleted")]
    [ProducesResponseType(typeof(List<ClientResponse>), 200)]
    public async Task<IActionResult> GetClientById([Required] string clientId)
    {
        if (!ModelState.IsValid)
            return BadRequest(new BadRequestResponse());
        var clientresponse = await _mediator.Send(new GetClientByIdQuery(clientId));
        if (clientresponse.Data == null)
            return BadRequest(new BadRequestResponse { Messages = ["Client Id does not exist"] });
        return Ok(clientresponse);
    }

    [HttpPut("update/client")]
    [SwaggerOperation(Summary = "Updating the client")]
    [ProducesResponseType(typeof(List<ClientResponse>), 200)]
    public async Task<IActionResult> UpdateClient([Required] string clientid, [FromBody] ClientUpdateDto clientdto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        if (!ObjectId.TryParse(clientid, out var objectId))
            return BadRequest(new BadRequestResponse { Messages = ["Invalid client id format"] });
        var client = await _clientRepository.GetOneAsync(x => x.Id == objectId, CancellationToken.None);
        if (client == null)
            return BadRequest(new BadRequestResponse { Messages = ["Client id does not exists"] });
        BaseResponse<ClientResponse> afterUpdate = await _mediator.Send(new UpdateClientCommand(clientdto, clientid));
        var response = new
        {
            before = new BaseResponse<ClientResponse>
            {
                Messages = ["before the update"],
                Data = _mapper.Map<ClientResponse>(client),
            },
            after = new BaseResponse<BaseResponse<ClientResponse>>
            {
                Messages = ["after The Update"],
                Data = afterUpdate,
            },
        };
        return Ok(new SuccessResponse<object>(response, "Updated"));
    }

    [HttpDelete("{client_id}")]
    [SwaggerOperation(Summary = "deleting the client by clientid")]
    [ProducesResponseType(typeof(BaseResponse<ClientResponse>), 200)]
    public async Task<IActionResult> SoftDeleteClient([Required] string client_id)
    {
        if (!ObjectId.TryParse(client_id, out var objectId))
            return BadRequest(new BadRequestResponse { Messages = ["Invalid client id format"] });
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var clientDeleting = await _mediator.Send(new SoftDeleteClientCommand(objectId));
        if (clientDeleting.Messages[0] == "Client does not exist")
            return BadRequest(clientDeleting);
        return Ok(clientDeleting);
    }
    //[HttpDelete("delete/client/{ClientId}")]
    //[SwaggerOperation(Summary = "deleting the client by clientid")]
    //[ProducesResponseType(typeof(Response<BaseResponse<ClientResponse>>), 200)]
    //public async Task<IActionResult> DeleteClient([Required]string ClientId)
    //{
    //    if (!ObjectId.TryParse(ClientId, out var objectId))
    //        return BadRequest(new BadRequestResponse
    //        {
    //            Messages =["Invalid client id format"]
    //        });
    //    if (!ModelState.IsValid) return BadRequest(ModelState);
    //    var client_exists=await _mediator.Send(new GetClientByIdIncludingSoftDeletedQuery(objectId));
    //    if(client_exists  == null) return BadRequest(new BadRequestResponse{Messages = ["Client does not exists"]});
    //    var messege = await  _mediator.Send(new DeleteClientCommand(objectId));
    //   Response<BaseResponse<ClientResponse>> response = new()
    //    {
    //        Message = messege.Data,
    //        Data = new BaseResponse<ClientResponse>{Data = client_exists.Data}
    //    };
    //    return Ok(new SuccessResponse<Response<BaseResponse<ClientResponse>>>(response));
    //}
    //[HttpPost("add/itemids/{clientId}")]
    //[SwaggerOperation(Summary = "Add multiple item IDs to a client")]
    //[ProducesResponseType(typeof(ClientResponse), 200)]
    //public async Task<IActionResult> AddItemIds(
    //    [Required] string clientId,
    //    [FromBody, Required] List<string> itemIds)
    //{
    //    if (!ObjectId.TryParse(clientId, out var objId))
    //        return BadRequest(new BadRequestResponse { Messages = ["Invalid client ID format"] });

    //    var client = await _clientRepository.GetOneAsync(x => x.Id == objId,CancellationToken.None);
    //    if (client is null)
    //        return BadRequest(new BadRequestResponse { Messages = ["Client ID does not exist"] });
    //    var responseClient = await _clientRepository.AddItemIds(clientId, itemIds);
    //    return Ok(new SuccessResponse<ClientResponse>(responseClient, "Item IDs added successfully"));
    //}

    //[HttpPost("add/customerids/{clientId}")]
    //[SwaggerOperation(Summary = "Add multiple customer IDs to a client")]
    //[ProducesResponseType(typeof(ClientResponse), 200)]
    //public async Task<IActionResult> AddCustomerIds(
    //    [Required] string clientId,
    //    [FromBody, Required] List<string> customerIds,
    //    CancellationToken cancellationToken)
    //{
    //    if (!ObjectId.TryParse(clientId, out var objId))
    //        return BadRequest(new BadRequestResponse { Messages = ["Invalid client ID format"] });

    //    var client = await _clientRepository.GetOneAsync(x => x.Id == objId, cancellationToken);
    //    if (client is null)
    //        return BadRequest(new BadRequestResponse { Messages = ["Client ID does not exist"] });
    //    var responseClient =await _clientRepository.AddItemIds(clientId,customerIds);
    //    return Ok(new SuccessResponse<ClientResponse>(responseClient, "Customer IDs added successfully"));
    //}
    // [HttpPost("add/invoiceids/{clientId}")]
    // [SwaggerOperation(Summary = "Add multiple invoice IDs to a client")]
    // [ProducesResponseType(typeof(ClientResponse), 200)]
    // public async Task<IActionResult> AddInvoiceIds(
    //     [Required] string clientId,
    //     [FromBody, Required] List<string> invoiceIds,
    //     CancellationToken cancellationToken)
    // {
    //     if (!ObjectId.TryParse(clientId, out var objId))
    //         return BadRequest(new BadRequestResponse { Messages = ["Invalid client ID format"] });
    //     var client = await _clientRepository.GetOneAsync(x => x.Id == objId, cancellationToken);
    //     if (client is null)
    //         return BadRequest(new BadRequestResponse { Messages = ["Client ID does not exist"] });
    //     var responseClient = await _clientRepository.AddInvoiceIds(clientId, invoiceIds);
    //     return Ok(new SuccessResponse<ClientResponse>(responseClient, "Invoice IDs added successfully"));
    //}
}
