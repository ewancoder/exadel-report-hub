using System.Net;
using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.API.Controllers;
using ExportPro.StorageService.CQRS.Handlers.Client;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace ExportPro.StorageService.Tests.ExportPro.StorageService.API.Controllers;

[TestFixture]
public class ClientControllerTests
{
    private ClientController _clientController;
    private IMediator _mediator;

    [SetUp]
    public void Setup()
    {
        _mediator = Substitute.For<IMediator>();
        _clientController = new ClientController(_mediator);
    }

    [Test]
    public async Task CreateClient_WithValidClientDto_ReturnsOkResult()
    {
        ClientDto clientDto = new() { Name = "Test Client", Description = "Test Description" };
        ClientResponse clientResponse = Substitute.For<ClientResponse>();
        BaseResponse<ValidationModel<ClientResponse>> validationModel = new()
        {
            Data = new(clientResponse),
            Messages = ["Client Created Successfully"],
            ApiState = HttpStatusCode.Created,
            IsSuccess = true,
        };
        _mediator.Send(Arg.Is<CreateClientCommand>(obj => obj.Clientdto.Equals(clientDto))).Returns(validationModel);
        var result = await _clientController.CreateClient(clientDto);
        var objectResult = result as ObjectResult;
        Assert.That(objectResult?.StatusCode, Is.EqualTo(201));
    }

    [Test]
    public async Task CreateClient_WithInvalidClientDto_ReturnsBadRequest()
    {
        ClientDto clientDto = new() { Name = "Test Client", Description = "Test Description" };
        var validationResult = Substitute.For<ValidationResult>();
        BaseResponse<ValidationModel<ClientResponse>> validationModel = new()
        {
            Data = new(validationResult),
            Messages = ["Client failed to create"],
            ApiState = HttpStatusCode.BadRequest,
            IsSuccess = false,
        };
        _mediator.Send(Arg.Is<CreateClientCommand>(obj => obj.Clientdto.Equals(clientDto))).Returns(validationModel);
        var result = await _clientController.CreateClient(clientDto);
        var objectResult = result as ObjectResult;
        Assert.That(objectResult?.StatusCode, Is.EqualTo(400));
    }
}
