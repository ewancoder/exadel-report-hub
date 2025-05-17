using ExportPro.Common.Shared.Extensions;
using ExportPro.StorageService.CQRS.QueryHandlers.InvoiceQueries;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Services;
using FluentAssertions;
using MongoDB.Bson;
using Moq;
using Serilog;
using System.Net;

namespace ExportPro.StorageService.Tests;

[TestFixture]
public class GetOverduePaymentsQueryHandlerTests
{
    private Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private Mock<ICurrencyExchangeService> _currencyExchangeServiceMock;
    private Mock<ICurrencyRepository> _currencyRepositoryMock;
    private Mock<ILogger> _loggerMock;
    private GetOverduePaymentsQueryHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _currencyExchangeServiceMock = new Mock<ICurrencyExchangeService>();
        _currencyRepositoryMock = new Mock<ICurrencyRepository>();
        _loggerMock = new Mock<ILogger>();

        _handler = new GetOverduePaymentsQueryHandler(
            _invoiceRepositoryMock.Object,
            _currencyExchangeServiceMock.Object,
            _currencyRepositoryMock.Object,
            _loggerMock.Object
        );
    }

    [Test]
    public async Task Handle_ShouldReturnBadRequest_WhenNoOverdueInvoicesFound()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var clientCurrencyId = Guid.NewGuid();
        var query = new GetOverduePaymentsQuery(clientId, clientCurrencyId);

        _invoiceRepositoryMock
            .Setup(repo => repo.GetOverdueInvoices(clientId.ToObjectId(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ApiState.Should().Be(HttpStatusCode.BadRequest);
        result.Messages.Should().ContainSingle(x => x.Contains("No invoices issued in selected period"));
    }

    [Test]
    public async Task Handle_ShouldCalculateTotalAmount_WhenAllInvoicesInClientCurrency()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var clientCurrencyId = Guid.NewGuid();
        var clientCurrencyObjectId = clientCurrencyId.ToObjectId();
        var query = new GetOverduePaymentsQuery(clientId, clientCurrencyId);

        var invoices = new List<Invoice>
        {
            new() { Id = ObjectId.GenerateNewId(), Amount = 100, CurrencyId = clientCurrencyObjectId },
            new() { Id = ObjectId.GenerateNewId(), Amount = 200, CurrencyId = clientCurrencyObjectId },
            new() { Id = ObjectId.GenerateNewId(), Amount = 300, CurrencyId = clientCurrencyObjectId }
        };

        _invoiceRepositoryMock
            .Setup(repo => repo.GetOverdueInvoices(clientId.ToObjectId(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoices);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.OverdueInvoicesCount.Should().Be(3);
        result.Data.TotalOverdueAmount.Should().Be(600);
    }

    [Test]
    public async Task Handle_ShouldReturnBadRequest_WhenCurrencyNotFound()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var clientCurrencyId = Guid.NewGuid();
        var otherCurrencyId = ObjectId.GenerateNewId();
        var query = new GetOverduePaymentsQuery(clientId, clientCurrencyId);

        var invoices = new List<Invoice>
        {
            new() {
                Id = ObjectId.GenerateNewId(),
                Amount = 100,
                CurrencyId = otherCurrencyId,
                IssueDate = DateTime.UtcNow.AddDays(-30)
            }
        };

        _invoiceRepositoryMock
            .Setup(repo => repo.GetOverdueInvoices(clientId.ToObjectId(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoices);

        _currencyRepositoryMock
            .Setup(repo => repo.GetCurrencyCodeById(otherCurrencyId))
            .ReturnsAsync(null as Currency);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ApiState.Should().Be(HttpStatusCode.BadRequest);
        result.Messages.Should().ContainSingle(x => x.Contains("One or more currencies not found"));
    }

    [Test]
    public async Task Handle_ShouldConvertCurrency_WhenInvoiceCurrencyDifferentFromClientCurrency()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var clientCurrencyId = Guid.NewGuid();
        var clientCurrencyObjectId = clientCurrencyId.ToObjectId();
        var otherCurrencyId = ObjectId.GenerateNewId();
        var issueDate = DateTime.UtcNow.AddDays(-30);
        var query = new GetOverduePaymentsQuery(clientId, clientCurrencyId);

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
            .Setup(repo => repo.GetOverdueInvoices(clientId.ToObjectId(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoices);

        var invoiceCurrency = new Currency { Id = otherCurrencyId, CurrencyCode = "USD" };
        var clientCurrency = new Currency { Id = clientCurrencyObjectId, CurrencyCode = "EUR" };

        _currencyRepositoryMock
            .Setup(repo => repo.GetCurrencyCodeById(otherCurrencyId))
            .ReturnsAsync(invoiceCurrency);

        _currencyRepositoryMock
            .Setup(repo => repo.GetCurrencyCodeById(clientCurrencyObjectId))
            .ReturnsAsync(clientCurrency);

        // USD to EUR rate is 0.85
        _currencyExchangeServiceMock
            .Setup(service => service.ExchangeRate(
                It.Is<CurrencyExchangeModel>(m => m.From == "USD" && m.Date == issueDate),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(0.85);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.OverdueInvoicesCount.Should().Be(1);
        result.Data.TotalOverdueAmount.Should().BeApproximately(117.65, 0.01); // 100 * 1.0 / 0.85 ≈ 117.65
    }

    [Test]
    public async Task Handle_ShouldHandleMultipleCurrencyConversions()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var clientCurrencyId = Guid.NewGuid();
        var clientCurrencyObjectId = clientCurrencyId.ToObjectId();
        var usdCurrencyId = ObjectId.GenerateNewId();
        var gbpCurrencyId = ObjectId.GenerateNewId();
        var issueDate = DateTime.UtcNow.AddDays(-30);
        var query = new GetOverduePaymentsQuery(clientId, clientCurrencyId);

        var invoices = new List<Invoice>
        {
            // Client currency invoice (no conversion needed)
            new() {
                Id = ObjectId.GenerateNewId(),
                Amount = 100,
                CurrencyId = clientCurrencyObjectId,
                IssueDate = issueDate
            },
            // USD invoice needing conversion
            new() {
                Id = ObjectId.GenerateNewId(),
                Amount = 100,
                CurrencyId = usdCurrencyId,
                IssueDate = issueDate
            },
            // GBP invoice needing conversion
            new() {
                Id = ObjectId.GenerateNewId(),
                Amount = 100,
                CurrencyId = gbpCurrencyId,
                IssueDate = issueDate
            }
        };

        _invoiceRepositoryMock
            .Setup(repo => repo.GetOverdueInvoices(clientId.ToObjectId(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoices);

        var clientCurrency = new Currency { Id = clientCurrencyObjectId, CurrencyCode = "PLN" };
        var usdCurrency = new Currency { Id = usdCurrencyId, CurrencyCode = "USD" };
        var gbpCurrency = new Currency { Id = gbpCurrencyId, CurrencyCode = "GBP" };

        _currencyRepositoryMock
            .Setup(repo => repo.GetCurrencyCodeById(clientCurrencyObjectId))
            .ReturnsAsync(clientCurrency);

        _currencyRepositoryMock
            .Setup(repo => repo.GetCurrencyCodeById(usdCurrencyId))
            .ReturnsAsync(usdCurrency);

        _currencyRepositoryMock
            .Setup(repo => repo.GetCurrencyCodeById(gbpCurrencyId))
            .ReturnsAsync(gbpCurrency);

        // PLN to EUR rate is 0.23
        _currencyExchangeServiceMock
            .Setup(service => service.ExchangeRate(
                It.Is<CurrencyExchangeModel>(m => m.From == "PLN" && m.Date == issueDate),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(0.23);

        // USD to EUR rate is 0.85
        _currencyExchangeServiceMock
            .Setup(service => service.ExchangeRate(
                It.Is<CurrencyExchangeModel>(m => m.From == "USD" && m.Date == issueDate),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(0.85);

        // GBP to EUR rate is 1.15
        _currencyExchangeServiceMock
            .Setup(service => service.ExchangeRate(
                It.Is<CurrencyExchangeModel>(m => m.From == "GBP" && m.Date == issueDate),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(1.15);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.OverdueInvoicesCount.Should().Be(3);

        // Expected calculation:
        // 100 (PLN invoice) + 
        // 100 * 0.23 / 0.85 (USD to PLN) + 
        // 100 * 0.23 / 1.15 (GBP to PLN)
        // = 100 + 27.06 + 20.00 = 147.06
        result.Data.TotalOverdueAmount.Should().BeApproximately(147.06, 0.1);
    }

    [Test]
    public async Task Handle_ShouldLogAppropriateMessages()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var clientCurrencyId = Guid.NewGuid();
        var clientCurrencyObjectId = clientCurrencyId.ToObjectId();
        var query = new GetOverduePaymentsQuery(clientId, clientCurrencyId);

        var invoices = new List<Invoice>
        {
            new() { Id = ObjectId.GenerateNewId(), Amount = 100, CurrencyId = clientCurrencyObjectId }
        };

        _invoiceRepositoryMock
            .Setup(repo => repo.GetOverdueInvoices(clientId.ToObjectId(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoices);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        // Verify logging
        _loggerMock.Verify(
            logger => logger.Information(
                It.Is<string>(s => s.Contains("Start handling GetOverduePaymentsQuery")),
                clientId),
            Times.Once);

        _loggerMock.Verify(
            logger => logger.Information(
                It.Is<string>(s => s.Contains("overdue invoices found")),
                invoices.Count,
                clientId),
            Times.Once);

        _loggerMock.Verify(
            logger => logger.Information(
                It.Is<string>(s => s.Contains("Successfully handled")),
                clientId),
            Times.Once);
    }
}