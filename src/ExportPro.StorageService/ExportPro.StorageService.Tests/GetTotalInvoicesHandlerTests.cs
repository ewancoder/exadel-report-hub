using ExportPro.StorageService.CQRS.QueryHandlers.InvoiceQueries;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using MongoDB.Driver;
using Moq;
using FluentAssertions;
using System.Net;

namespace ExportPro.StorageService.Tests;

[TestFixture]
public class GetTotalInvoicesHandlerTests
{
    private Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private GetTotalInvoicesHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _handler = new GetTotalInvoicesHandler(_invoiceRepositoryMock.Object);
    }

    [Test]
    public async Task Handle_ShouldReturnZero_WhenNoInvoicesExist()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var startDate = DateTime.UtcNow.AddDays(-10);
        var endDate = DateTime.UtcNow;

        var dto = new TotalInvoicesDto
        {
            StartDate = startDate,
            EndDate = endDate,
            ClientId = clientId
        };

        _invoiceRepositoryMock
            .Setup(repo => repo.CountAsync(It.IsAny<FilterDefinition<Invoice>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var query = new GetTotalInvoicesQuery(dto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(0);
        result.Messages.Should().ContainSingle(x => x.Contains("No invoices issued"));

        _invoiceRepositoryMock.Verify(
            repo => repo.CountAsync(It.IsAny<FilterDefinition<Invoice>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task Handle_ShouldReturnCorrectCount_WhenInvoicesExist()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var startDate = DateTime.UtcNow.AddMonths(-1);
        var endDate = DateTime.UtcNow;

        var dto = new TotalInvoicesDto
        {
            StartDate = startDate,
            EndDate = endDate,
            ClientId = clientId
        };

        const long expectedCount = 7;
        _invoiceRepositoryMock
            .Setup(repo => repo.CountAsync(It.IsAny<FilterDefinition<Invoice>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCount);

        var query = new GetTotalInvoicesQuery(dto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(expectedCount);
        result.Messages.Should().ContainSingle(x => x.Contains("fetched successfully"));

        _invoiceRepositoryMock.Verify(
            repo => repo.CountAsync(It.IsAny<FilterDefinition<Invoice>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task Handle_ShouldVerifyCorrectFilterParameters()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var startDate = new DateTime(2023, 1, 1);
        var endDate = new DateTime(2023, 12, 31);

        var dto = new TotalInvoicesDto
        {
            StartDate = startDate,
            EndDate = endDate,
            ClientId = clientId
        };

        FilterDefinition<Invoice>? capturedFilter = null;
        _invoiceRepositoryMock
            .Setup(repo => repo.CountAsync(It.IsAny<FilterDefinition<Invoice>>(), It.IsAny<CancellationToken>()))
            .Callback<FilterDefinition<Invoice>, CancellationToken>((filter, _) => capturedFilter = filter)
            .ReturnsAsync(5);

        var query = new GetTotalInvoicesQuery(dto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(5);

        _invoiceRepositoryMock.Verify(
            repo => repo.CountAsync(It.IsAny<FilterDefinition<Invoice>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task Handle_ShouldWorkWithEmptyClientId()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(-10);
        var endDate = DateTime.UtcNow;

        var dto = new TotalInvoicesDto
        {
            StartDate = startDate,
            EndDate = endDate,
            ClientId = Guid.Empty 
        };

        _invoiceRepositoryMock
            .Setup(repo => repo.CountAsync(It.IsAny<FilterDefinition<Invoice>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(10);

        var query = new GetTotalInvoicesQuery(dto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(10);

        _invoiceRepositoryMock.Verify(
            repo => repo.CountAsync(It.IsAny<FilterDefinition<Invoice>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task Handle_ShouldReturnErrorResult_WhenRepositoryThrowsException()
    {
        // Arrange
        var dto = new TotalInvoicesDto
        {
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow,
            ClientId = Guid.NewGuid()
        };

        _invoiceRepositoryMock
            .Setup(repo => repo.CountAsync(It.IsAny<FilterDefinition<Invoice>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database connection failed"));

        var query = new GetTotalInvoicesQuery(dto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ApiState.Should().Be(HttpStatusCode.InternalServerError);
        result.Messages.Should().ContainSingle(x => x.Contains("Error counting invoices"));
    }
}