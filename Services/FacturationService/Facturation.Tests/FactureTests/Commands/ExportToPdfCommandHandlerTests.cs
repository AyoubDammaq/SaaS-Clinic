using Facturation.Application.FactureService.Commands.ExportToPdf;
using Facturation.Domain.Entities;
using Facturation.Domain.Enums;
using Facturation.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Facturation.Tests.FactureTests.Commands
{
    public class ExportToPdfCommandHandlerTests
    {
        private readonly Mock<IFactureRepository> _factureRepositoryMock;
        private readonly Mock<ILogger<ExportToPdfCommandHandler>> _loggerMock;
        private readonly ExportToPdfCommandHandler _handler;

        public ExportToPdfCommandHandlerTests()
        {
            _factureRepositoryMock = new Mock<IFactureRepository>();
            _loggerMock = new Mock<ILogger<ExportToPdfCommandHandler>>();
            _handler = new ExportToPdfCommandHandler(_factureRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnPdfBytes_When_RequestIsValid()
        {
            // Arrange
            var facture = new Facture
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                ConsultationId = Guid.NewGuid(),
                ClinicId = Guid.NewGuid(),
                DateEmission = DateTime.UtcNow,
                MontantTotal = 150.50m,
                Status = FactureStatus.PAYEE
            };
            var request = new ExportToPdfCommand(facture);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            // Vérification simple : commence par %PDF (signature PDF)
            var pdfHeader = System.Text.Encoding.ASCII.GetString(result, 0, 4);
            pdfHeader.Should().Be("%PDF");
        }

        [Fact]
        public async Task Handle_Should_LogErrorAndThrow_When_ExceptionOccurs()
        {
            // Arrange
            var facture = new Facture
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                ConsultationId = Guid.NewGuid(),
                ClinicId = Guid.NewGuid(),
                DateEmission = DateTime.UtcNow,
                MontantTotal = 150.50m,
                Status = FactureStatus.PAYEE
            };
            var request = new ExportToPdfCommand(facture);

            // Handler factice pour simuler une exception dans Handle
            var faultyHandler = new FaultyExportToPdfCommandHandler(_factureRepositoryMock.Object, _loggerMock.Object);

            // Act
            Func<Task> act = async () => await faultyHandler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ApplicationException>()
                .WithMessage("Erreur lors de la génération du PDF de la facture.");
            _loggerMock.Setup(l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ));
        }

        // Handler factice pour simuler une exception dans Handle
        private class FaultyExportToPdfCommandHandler : ExportToPdfCommandHandler
        {
            public FaultyExportToPdfCommandHandler(IFactureRepository repo, ILogger<ExportToPdfCommandHandler> logger)
                : base(repo, logger) { }

            public new Task<byte[]> Handle(ExportToPdfCommand request, CancellationToken cancellationToken)
            {
                throw new ApplicationException("Erreur lors de la génération du PDF de la facture.");
            }
        }
    }
}
