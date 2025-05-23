﻿using ExportPro.Common.Shared.Extensions;
using ExportPro.StorageService.CQRS.QueryHandlers.InvoiceQueries;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Services;
using FluentAssertions;
using FluentValidation;
using MongoDB.Bson;
using Moq;
using System.Net;

namespace ExportPro.StorageService.Tests;

[TestFixture]
public class GetTotalRevenueHandlerTests
{
    private Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private Mock<ICurrencyRepository> _currencyRepositoryMock;
    private Mock<ICurrencyExchangeService> _exchangeServiceMock;
    private Mock<IValidator<CurrencyExchangeModel>> _validatorMock;
    private GetTotalRevenueHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _currencyRepositoryMock = new Mock<ICurrencyRepository>();
        _exchangeServiceMock = new Mock<ICurrencyExchangeService>();
        _validatorMock = new Mock<IValidator<CurrencyExchangeModel>>();

        // Create the handler with mocked dependencies
        _handler = new GetTotalRevenueHandler(
            _invoiceRepositoryMock.Object,
            _currencyRepositoryMock.Object,
            _exchangeServiceMock.Object,
            _validatorMock.Object
        );
    }

    [Test]
    public async Task Handle_ShouldReturnZero_WhenNoInvoicesFound()
    {
        // Arrange
        var clientCurrencyId = Guid.NewGuid();
        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;

        var revenueDto = new TotalRevenueDto
        {
            ClientCurrencyId = clientCurrencyId,
            StartDate = startDate,
            EndDate = endDate
        };

        var query = new GetTotalRevenueQuery(revenueDto);

        _invoiceRepositoryMock
            .Setup(repo => repo.GetInvoicesInDateRangeAsync(startDate, endDate))
            .ReturnsAsync([]);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.ApiState.Should().Be(HttpStatusCode.OK);
        result.Data.Should().Be(0);
        result.Messages.Should().ContainSingle(x => x.Contains("No invoices issued"));
    }

    [Test]
    public async Task Handle_ShouldCalculateTotalAmount_WhenAllInvoicesInClientCurrency()
    {
        // Arrange
        var clientCurrencyId = Guid.NewGuid();
        var clientCurrencyObjectId = ObjectId.GenerateNewId();
        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;

        var revenueDto = new TotalRevenueDto
        {
            ClientCurrencyId = clientCurrencyObjectId.ToGuid(),
            StartDate = startDate,
            EndDate = endDate
        };

        var query = new GetTotalRevenueQuery(revenueDto);

        var invoices = new List<Invoice>
        {
            new() { Id = ObjectId.GenerateNewId(), Amount = 100, CurrencyId = clientCurrencyObjectId },
            new() { Id = ObjectId.GenerateNewId(), Amount = 200, CurrencyId = clientCurrencyObjectId },
            new() { Id = ObjectId.GenerateNewId(), Amount = 300, CurrencyId = clientCurrencyObjectId }
        };

        _invoiceRepositoryMock
            .Setup(repo => repo.GetInvoicesInDateRangeAsync(startDate, endDate))
            .ReturnsAsync(invoices);

        var clientCurrency = new Currency { Id = clientCurrencyObjectId, CurrencyCode = "USD" };

        _currencyRepositoryMock
            .Setup(repo => repo.GetCurrencyCodeById(clientCurrencyObjectId))
            .ReturnsAsync(clientCurrency);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.ApiState.Should().Be(HttpStatusCode.OK);
        result.Data.Should().Be(600);
        result.Messages.Should().ContainSingle(x => x.Contains("Total revenue calculated successfully"));
    }

    [Test]
    public async Task Handle_ShouldReturnBadRequest_WhenCurrencyNotFound()
    {
        // Arrange
        var clientCurrencyId = Guid.NewGuid();
        var clientCurrencyObjectId = ObjectId.GenerateNewId();
        var otherCurrencyId = ObjectId.GenerateNewId();
        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;
        var issueDate = DateTime.UtcNow.AddDays(-15);

        var revenueDto = new TotalRevenueDto
        {
            ClientCurrencyId = clientCurrencyObjectId.ToGuid(),
            StartDate = startDate,
            EndDate = endDate
        };

        var query = new GetTotalRevenueQuery(revenueDto);

        var invoices = new List<Invoice>
        {
            new() {
                Id = ObjectId.GenerateNewId(),
                Amount = 100,
                CurrencyId = otherCurrencyId,
                IssueDate = issueDate
            }
        };

        _invoiceRepositoryMock
            .Setup(repo => repo.GetInvoicesInDateRangeAsync(startDate, endDate))
            .ReturnsAsync(invoices);

        _currencyRepositoryMock
            .Setup(repo => repo.GetCurrencyCodeById(otherCurrencyId))
            .ReturnsAsync((Currency)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ApiState.Should().Be(HttpStatusCode.BadRequest);
        result.Messages.Should().ContainSingle(x => x.Contains("Currency not found"));
    }

    [Test]
    public async Task Handle_ShouldConvertFromNonEuroToClientCurrency()
    {
        // Arrange
        var clientCurrencyId = Guid.NewGuid();
        var clientCurrencyObjectId = ObjectId.GenerateNewId();
        var otherCurrencyId = ObjectId.GenerateNewId();
        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;
        var issueDate = DateTime.UtcNow.AddDays(-15);

        var revenueDto = new TotalRevenueDto
        {
            ClientCurrencyId = clientCurrencyObjectId.ToGuid(),
            StartDate = startDate,
            EndDate = endDate
        };

        var query = new GetTotalRevenueQuery(revenueDto);

        var invoices = new List<Invoice>
        {
            new Invoice
            {
                Id = ObjectId.GenerateNewId(),
                Amount = 100,
                CurrencyId = otherCurrencyId,
                IssueDate = issueDate
            }
        };

        _invoiceRepositoryMock
            .Setup(repo => repo.GetInvoicesInDateRangeAsync(startDate, endDate))
            .ReturnsAsync(invoices);

        var clientCurrency = new Currency { Id = clientCurrencyObjectId, CurrencyCode = "EUR" };
        var invoiceCurrency = new Currency { Id = otherCurrencyId, CurrencyCode = "USD" };

        _currencyRepositoryMock
            .Setup(repo => repo.GetCurrencyCodeById(clientCurrencyObjectId))
            .ReturnsAsync(clientCurrency);

        _currencyRepositoryMock
            .Setup(repo => repo.GetCurrencyCodeById(otherCurrencyId))
            .ReturnsAsync(invoiceCurrency);

        // USD to EUR rate is 0.85
        _exchangeServiceMock
            .Setup(service => service.ExchangeRate(
                It.Is<CurrencyExchangeModel>(m => m.From == "USD" && m.Date == issueDate),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(0.85);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.ApiState.Should().Be(HttpStatusCode.OK);
        result.Data.Should().BeApproximately(117.65, 0.01); // 100 * (1 / 0.85) ≈ 117.65
        result.Messages.Should().ContainSingle(x => x.Contains("Total revenue calculated successfully"));
    }

    [Test]
    public async Task Handle_ShouldConvertFromEuroToClientCurrency()
    {
        // Arrange
        var clientCurrencyId = Guid.NewGuid();
        var clientCurrencyObjectId = ObjectId.GenerateNewId();
        var eurCurrencyId = ObjectId.GenerateNewId();
        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;
        var issueDate = DateTime.UtcNow.AddDays(-15);

        var revenueDto = new TotalRevenueDto
        {
            ClientCurrencyId = clientCurrencyObjectId.ToGuid(),
            StartDate = startDate,
            EndDate = endDate
        };

        var query = new GetTotalRevenueQuery(revenueDto);

        var invoices = new List<Invoice>
        {
            new() {
                Id = ObjectId.GenerateNewId(),
                Amount = 100,
                CurrencyId = eurCurrencyId,
                IssueDate = issueDate
            }
        };

        _invoiceRepositoryMock
            .Setup(repo => repo.GetInvoicesInDateRangeAsync(startDate, endDate))
            .ReturnsAsync(invoices);

        var clientCurrency = new Currency { Id = clientCurrencyObjectId, CurrencyCode = "USD" };
        var eurCurrency = new Currency { Id = eurCurrencyId, CurrencyCode = "EUR" };

        _currencyRepositoryMock
            .Setup(repo => repo.GetCurrencyCodeById(clientCurrencyObjectId))
            .ReturnsAsync(clientCurrency);

        _currencyRepositoryMock
            .Setup(repo => repo.GetCurrencyCodeById(eurCurrencyId))
            .ReturnsAsync(eurCurrency);

        // USD to EUR rate is 0.85
        _exchangeServiceMock
            .Setup(service => service.ExchangeRate(
                It.Is<CurrencyExchangeModel>(m => m.From == "USD" && m.Date == issueDate),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(0.85);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.ApiState.Should().Be(HttpStatusCode.OK);
        result.Data.Should().BeApproximately(117.65, 0.01); // 100 * 1.0 * (1 / 0.85) ≈ 117.65
        result.Messages.Should().ContainSingle(x => x.Contains("Total revenue calculated successfully"));
    }

    [Test]
    public async Task Handle_ShouldReturnBadRequest_WhenInvoiceCurrencyNotSupportedByECB()
    {
        // Arrange
        var clientCurrencyId = Guid.NewGuid();
        var clientCurrencyObjectId = ObjectId.GenerateNewId();
        var otherCurrencyId = ObjectId.GenerateNewId();
        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;
        var issueDate = DateTime.UtcNow.AddDays(-15);

        var revenueDto = new TotalRevenueDto
        {
            ClientCurrencyId = clientCurrencyObjectId.ToGuid(),
            StartDate = startDate,
            EndDate = endDate
        };

        var query = new GetTotalRevenueQuery(revenueDto);

        var invoices = new List<Invoice>
        {
            new() {
                Id = ObjectId.GenerateNewId(),
                Amount = 100,
                CurrencyId = otherCurrencyId,
                IssueDate = issueDate
            }
        };

        _invoiceRepositoryMock
            .Setup(repo => repo.GetInvoicesInDateRangeAsync(startDate, endDate))
            .ReturnsAsync(invoices);

        var clientCurrency = new Currency { Id = clientCurrencyObjectId, CurrencyCode = "EUR" };
        var invoiceCurrency = new Currency { Id = otherCurrencyId, CurrencyCode = "XYZ" }; // Unsupported currency

        _currencyRepositoryMock
            .Setup(repo => repo.GetCurrencyCodeById(clientCurrencyObjectId))
            .ReturnsAsync(clientCurrency);

        _currencyRepositoryMock
            .Setup(repo => repo.GetCurrencyCodeById(otherCurrencyId))
            .ReturnsAsync(invoiceCurrency);

        // XYZ to EUR rate returns 0 (not supported)
        _exchangeServiceMock
            .Setup(service => service.ExchangeRate(
                It.Is<CurrencyExchangeModel>(m => m.From == "XYZ" && m.Date == issueDate),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ApiState.Should().Be(HttpStatusCode.BadRequest);
        result.Messages.Should().ContainSingle(x => x.Contains("Currency XYZ is not supported by ECB"));
    }

    [Test]
    public async Task Handle_ShouldReturnBadRequest_WhenClientCurrencyNotSupportedByECB()
    {
        // Arrange
        var clientCurrencyId = Guid.NewGuid();
        var clientCurrencyObjectId = ObjectId.GenerateNewId();
        var otherCurrencyId = ObjectId.GenerateNewId();
        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;
        var issueDate = DateTime.UtcNow.AddDays(-15);

        var revenueDto = new TotalRevenueDto
        {
            ClientCurrencyId = clientCurrencyObjectId.ToGuid(),
            StartDate = startDate,
            EndDate = endDate
        };

        var query = new GetTotalRevenueQuery(revenueDto);

        var invoices = new List<Invoice>
        {
            new() {
                Id = ObjectId.GenerateNewId(),
                Amount = 100,
                CurrencyId = otherCurrencyId,
                IssueDate = issueDate
            }
        };

        _invoiceRepositoryMock
            .Setup(repo => repo.GetInvoicesInDateRangeAsync(startDate, endDate))
            .ReturnsAsync(invoices);

        var clientCurrency = new Currency { Id = clientCurrencyObjectId, CurrencyCode = "XYZ" }; // Unsupported currency
        var invoiceCurrency = new Currency { Id = otherCurrencyId, CurrencyCode = "EUR" };

        _currencyRepositoryMock
            .Setup(repo => repo.GetCurrencyCodeById(clientCurrencyObjectId))
            .ReturnsAsync(clientCurrency);

        _currencyRepositoryMock
            .Setup(repo => repo.GetCurrencyCodeById(otherCurrencyId))
            .ReturnsAsync(invoiceCurrency);

        // XYZ to EUR rate returns 0 (not supported)
        _exchangeServiceMock
            .Setup(service => service.ExchangeRate(
                It.Is<CurrencyExchangeModel>(m => m.From == "XYZ" && m.Date == issueDate),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ApiState.Should().Be(HttpStatusCode.BadRequest);
        result.Messages.Should().ContainSingle(x => x.Contains("Currency XYZ is not supported by ECB"));
    }

    [Test]
    public async Task Handle_ShouldHandleNullAmounts()
    {
        // Arrange
        var clientCurrencyId = Guid.NewGuid();
        var clientCurrencyObjectId = ObjectId.GenerateNewId();
        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;

        var revenueDto = new TotalRevenueDto
        {
            ClientCurrencyId = clientCurrencyObjectId.ToGuid(),
            StartDate = startDate,
            EndDate = endDate
        };

        var query = new GetTotalRevenueQuery(revenueDto);

        var invoices = new List<Invoice>
        {
            new Invoice { Id = ObjectId.GenerateNewId(), Amount = null, CurrencyId = clientCurrencyObjectId },
            new Invoice { Id = ObjectId.GenerateNewId(), Amount = 200, CurrencyId = clientCurrencyObjectId }
        };

        _invoiceRepositoryMock
            .Setup(repo => repo.GetInvoicesInDateRangeAsync(startDate, endDate))
            .ReturnsAsync(invoices);

        var clientCurrency = new Currency { Id = clientCurrencyObjectId, CurrencyCode = "USD" };

        _currencyRepositoryMock
            .Setup(repo => repo.GetCurrencyCodeById(clientCurrencyObjectId))
            .ReturnsAsync(clientCurrency);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.ApiState.Should().Be(HttpStatusCode.OK);
        result.Data.Should().Be(200); // Only the non-null amount should be counted
        result.Messages.Should().ContainSingle(x => x.Contains("Total revenue calculated successfully"));
    }
}