// using System.Net;
// using ExportPro.Common.Shared.Library;
// using ExportPro.StorageService.API.Controllers;
// using ExportPro.StorageService.CQRS.Handlers.Client;
// using ExportPro.StorageService.Models.Models;
// using ExportPro.StorageService.SDK.DTOs;
// using ExportPro.StorageService.SDK.Responses;
// using FluentValidation.Results;
// using MediatR;
// using Microsoft.AspNetCore.Mvc;
// using NSubstitute;
// namespace ExportPro.StorageService.Tests.ExportPro.StorageService.API.Controllers;
// [TestFixture]
// public class ClientControllerTests
// {
//     private ClientController _clientController;
//     private IMediator _mediator;
//
//     [SetUp]
//     public void Setup()
//     {
//         _mediator = Substitute.For<IMediator>();
//         _clientController = new ClientController(_mediator);
//     }
//
//     [Test]
//     public async Task CreateClient_WithValidClientDto_ReturnsOkResult()
//     {
//         ClientDto clientDto = new() { Name = "Test Client", Description = "Test Description" };
//         ClientResponse clientResponse = Substitute.For<ClientResponse>();
//         BaseResponse<ValidationModel<ClientResponse>> validationModel = new()
//         {
//             Data = new(clientResponse),
//             Messages = ["Client Created Successfully"],
//             ApiState = HttpStatusCode.Created,
//             IsSuccess = true,
//         };
//         _mediator.Send(Arg.Is<CreateClientCommand>(obj => obj.Clientdto.Equals(clientDto))).Returns(validationModel);
//         var result = await _clientController.CreateClient(clientDto);
//         var objectResult = result as ObjectResult;
//         Assert.That(objectResult?.Value, Is.Not.Null);
//         Assert.That(objectResult?.Value, Is.TypeOf<BaseResponse<ValidationModel<ClientResponse>>>());
//         Assert.That(objectResult?.StatusCode, Is.EqualTo(201));
//     }
//     [Test]
//     public async Task CreateClient_WithInvalidClientDto_ReturnsBadRequest()
//     {
//         ClientDto clientDto = new() { Name = "Test Client", Description = "Test Description" };
//         var validationResult = Substitute.For<ValidationResult>();
//         BaseResponse<ValidationModel<ClientResponse>> validationModel = new()
//         {
//             Data = new(validationResult),
//             Messages = ["Client failed to create"],
//             ApiState = HttpStatusCode.BadRequest,
//             IsSuccess = false,
//         };
//         _mediator.Send(Arg.Is<CreateClientCommand>(obj => obj.Clientdto.Equals(clientDto))).Returns(validationModel);
//         var result = await _clientController.CreateClient(clientDto);
//         var objectResult = result as ObjectResult;
//         Assert.That(objectResult?.Value, Is.Not.Null);
//         Assert.That(objectResult?.Value, Is.TypeOf<BaseResponse<ValidationModel<ClientResponse>>>());
//         Assert.That(objectResult?.StatusCode, Is.EqualTo(400));
//     }
//     [Test]
//     public async Task GetClients_WithInvalidQueries_ReturnsBadRequest()
//     {
//         int top = 5;
//         int skip = -1;
//         var validationResult = Substitute.For<ValidationResult>();
//         _mediator.Send(Arg.Is<GetClientsQuery>(obj => obj.top == top && obj.skip == skip))
//             .Returns(new BaseResponse<ValidationModel<List<ClientResponse>>>
//             {
//                 Data = new(validationResult),
//                 ApiState = HttpStatusCode.BadRequest,
//                 IsSuccess = false,
//             });
//         var result = await _clientController.GetClients(top, skip);
//         var objectResult = result as ObjectResult;
//         Assert.That(objectResult?.Value, Is.Not.Null);
//         Assert.That(objectResult?.Value, Is.TypeOf<BaseResponse<ValidationModel<List<ClientResponse>>>>());
//         Assert.That(objectResult?.StatusCode, Is.EqualTo(400));
//     }
//     [Test]
//     public async Task GetClients_WithValidQueries_ReturnsOkResult()
//     {
//         int top = 5;
//         int skip = 0;
//         var clientResponse = Substitute.For<List<ClientResponse>>();
//         _mediator.Send(Arg.Is<GetClientsQuery>(obj => obj.top == top && obj.skip == skip))
//             .Returns(new BaseResponse<ValidationModel<List<ClientResponse>>>
//             {
//                 Data = new(clientResponse),
//                 ApiState = HttpStatusCode.OK,
//                 IsSuccess = true,
//             });
//         var result = await _clientController.GetClients(top, skip);
//         var objectResult = result as ObjectResult;
//         Assert.That(objectResult?.Value, Is.Not.Null);
//         Assert.That(objectResult?.Value, Is.TypeOf<BaseResponse<ValidationModel<List<ClientResponse>>>>());
//         Assert.That(objectResult?.StatusCode, Is.EqualTo(200));
//     }
//     [Test]
//     public async Task GetClientById_WithValidClientId_ReturnsOkResult()
//     {
//         string clientId = "6803a0ac085ca8a4984681f4";
//         var clientResponse = Substitute.For<ClientResponse>();
//         _mediator.Send(Arg.Is<GetClientByIdQuery>(obj => obj.Id == clientId))
//             .Returns(new BaseResponse<ValidationModel<ClientResponse>>
//             {
//                 Data = new(clientResponse),
//                 ApiState = HttpStatusCode.OK,
//                 IsSuccess = true,
//             });
//         var result = await _clientController.GetClientById(clientId);
//         var objectResult = result as ObjectResult;
//         Assert.That(objectResult?.Value, Is.Not.Null);
//         Assert.That(objectResult?.Value, Is.TypeOf<BaseResponse<ValidationModel<ClientResponse>>>());
//         Assert.That(objectResult?.StatusCode, Is.EqualTo(200));
//     }
//     [Test]
//     public async Task GetClientById_WithInvalidClientId_ReturnsBadRequest()
//     {
//         string clientId = "invalidia";
//         var validationResult = Substitute.For<ValidationResult>();
//         _mediator.Send(Arg.Is<GetClientByIdQuery>(obj => obj.Id == clientId))
//             .Returns(new BaseResponse<ValidationModel<ClientResponse>>
//             {
//                 Data = new(validationResult),
//                 ApiState = HttpStatusCode.BadRequest,
//                 IsSuccess = false,
//             });
//         var result = await _clientController.GetClientById(clientId);
//         var objectResult = result as ObjectResult;
//         Assert.That(objectResult?.Value, Is.Not.Null);
//         Assert.That(objectResult?.Value, Is.TypeOf<BaseResponse<ValidationModel<ClientResponse>>>());
//         Assert.That(objectResult?.StatusCode, Is.EqualTo(400));
//     }
//     [Test]
//     public async Task UpdateClient_WithValidClientId_ReturnsOkResult()
//     {
//         string clientId = "6803a0ac085ca8a4984681f4";
//         ClientUpdateDto clientUpdateDto = Substitute.For<ClientUpdateDto>();
//         var clientResponse = Substitute.For<ClientResponse>();
//         _mediator.Send(Arg.Is<UpdateClientCommand>(obj => obj.clientUpdateDto.Equals(clientUpdateDto) && obj.ClientId == clientId))
//             .Returns(new BaseResponse<ValidationModel<ClientResponse>>
//             {
//                 Data = new(clientResponse),
//                 ApiState = HttpStatusCode.OK,
//                 IsSuccess = true,
//             });
//         var result = await _clientController.UpdateClient(clientId, clientUpdateDto);
//         var objectResult = result as ObjectResult;
//         Assert.That(objectResult?.Value, Is.Not.Null);
//         Assert.That(objectResult?.Value, Is.TypeOf<BaseResponse<ValidationModel<ClientResponse>>>());
//         Assert.That(objectResult?.StatusCode, Is.EqualTo(200));
//     }
//     [Test]
//     public async Task UpdateClient_WithInvalidClientId_ReturnsBadRequest()
//     {
//         string clientId = "invalidia";
//         ClientUpdateDto clientUpdateDto = Substitute.For<ClientUpdateDto>();
//         var validationResult = Substitute.For<ValidationResult>();
//         _mediator.Send(Arg.Is<UpdateClientCommand>(obj => obj.clientUpdateDto.Equals(clientUpdateDto) && obj.ClientId == clientId))
//             .Returns(new BaseResponse<ValidationModel<ClientResponse>>
//             {
//                 Data = new(validationResult),
//                 ApiState = HttpStatusCode.BadRequest,
//                 IsSuccess = false,
//             });
//         var result = await _clientController.UpdateClient(clientId, clientUpdateDto);
//         var objectResult = result as ObjectResult;
//         Assert.That(objectResult?.Value, Is.Not.Null);
//         Assert.That(objectResult?.Value, Is.TypeOf<BaseResponse<ValidationModel<ClientResponse>>>());
//         Assert.That(objectResult?.StatusCode, Is.EqualTo(400));
//     }
//     [Test]
//     public async Task UpdateClient_WithValidClientIdAndInvalidName_ReturnsBadReqeust()
//     {
//         string clientId = "6803a0ac085ca8a4984681f4";
//         ClientUpdateDto clientUpdateDto = new()
//         {
//             Name ="",
//             Description = "Test Description",
//         };
//         var validationResult = Substitute.For<ValidationResult>();
//         _mediator.Send(Arg.Is<UpdateClientCommand>(obj => obj.clientUpdateDto.Equals(clientUpdateDto) && obj.ClientId == clientId))
//             .Returns(new BaseResponse<ValidationModel<ClientResponse>>
//             {
//                 Data = new(validationResult),
//                 ApiState = HttpStatusCode.BadRequest,
//                 IsSuccess = false,
//             });
//         var result = await _clientController.UpdateClient(clientId, clientUpdateDto);
//         var objectResult = result as ObjectResult;
//         Assert.That(objectResult?.Value, Is.Not.Null);
//         Assert.That(objectResult?.Value, Is.TypeOf<BaseResponse<ValidationModel<ClientResponse>>>());
//         Assert.That(objectResult?.StatusCode, Is.EqualTo(400));
//     }
//     [Test]
//     public async Task SoftDeleteClient_WithValidClientId_ReturnsOkResult()
//     {
//         string clientId = "6803a0ac085ca8a4984681f4";
//         var clientResponse = Substitute.For<ClientResponse>();
//         _mediator.Send(Arg.Is<SoftDeleteClientCommand>(obj => obj.ClientId == clientId))
//             .Returns(new BaseResponse<ValidationModel<ClientResponse>>
//             {
//                 Data = new(clientResponse),
//                 ApiState = HttpStatusCode.OK,
//                 IsSuccess = true,
//             });
//         var result = await _clientController.SoftDeleteClient(clientId);
//         var objectResult = result as ObjectResult;
//         Assert.That(objectResult?.Value, Is.Not.Null);
//         Assert.That(objectResult?.Value, Is.TypeOf<BaseResponse<ValidationModel<ClientResponse>>>());
//         Assert.That(objectResult?.StatusCode, Is.EqualTo(200));
//     }
//     [Test]
//     public async Task SoftDeleteClient_WithInvalidClientId_ReturnsBadRequest()
//     {
//         string clientId = "invalidia";
//         var validationResult = Substitute.For<ValidationResult>();
//         _mediator.Send(Arg.Is<SoftDeleteClientCommand>(obj => obj.ClientId == clientId))
//             .Returns(new BaseResponse<ValidationModel<ClientResponse>>
//             {
//                 Data = new(validationResult),
//                 ApiState = HttpStatusCode.BadRequest,
//                 IsSuccess = false,
//             });
//         var result = await _clientController.SoftDeleteClient(clientId);
//         var objectResult = result as ObjectResult;
//         Assert.That(objectResult?.Value, Is.Not.Null);
//         Assert.That(objectResult?.Value, Is.TypeOf<BaseResponse<ValidationModel<ClientResponse>>>());
//         Assert.That(objectResult?.StatusCode, Is.EqualTo(400));
//     }
// }
