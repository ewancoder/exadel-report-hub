using ExportPro.StorageService.CQRS.QueryHandlers.InvoiceQueries;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using MongoDB.Driver;
using Moq;
using FluentAssertions;

namespace ExportPro.StorageService.Tests
{
    [TestFixture]
    public class GetTotalInvoicesHandlerTests
    {
        private Mock<IInvoiceRepository>? _invoiceRepositoryMock;
        private GetTotalInvoicesHandler? _handler;

        [SetUp]
        public void SetUp()
        {
            _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
            _handler = new GetTotalInvoicesHandler(_invoiceRepositoryMock.Object);
        }

        [Test]
        public async Task Handle_ShouldReturnZero_WhenNoInvoicesExist()
        {
            var dto = new TotalInvoicesDto
            {
                StartDate = DateTime.UtcNow.AddDays(-10),
                EndDate = DateTime.UtcNow,
                ClientId = Guid.NewGuid()
            };

            _invoiceRepositoryMock
                .Setup(repo => repo.CountAsync(It.IsAny<FilterDefinition<Invoice>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            var query = new GetTotalInvoicesQuery(dto);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(0);
            result.Messages.Should().ContainSingle(x => x.Contains("No invoices issued"));
        }

        [Test]
        public async Task Handle_ShouldReturnCorrectCount_WhenInvoicesExist()
        {
            var dto = new TotalInvoicesDto
            {
                StartDate = DateTime.UtcNow.AddMonths(-1),
                EndDate = DateTime.UtcNow,
                ClientId = Guid.NewGuid()
            };

            const long expectedCount = 7;

            _invoiceRepositoryMock
                .Setup(repo => repo.CountAsync(It.IsAny<FilterDefinition<Invoice>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCount);

            var query = new GetTotalInvoicesQuery(dto);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(expectedCount);
            result.Messages.Should().ContainSingle(x => x.Contains("fetched successfully"));
        }
    }
}