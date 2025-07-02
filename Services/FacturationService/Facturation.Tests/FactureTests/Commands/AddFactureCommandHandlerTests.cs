using Facturation.Application.DTOs;
using Facturation.Application.FactureService.Commands.AddFacture;
using Facturation.Domain.Entities;
using Facturation.Domain.Enums;
using Facturation.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Facturation.Tests.FactureTests.Commands
{
    public class AddFactureCommandHandlerTests
    {
        private readonly Mock<IFactureRepository> _factureRepositoryMock;
        private readonly Mock<ILogger<AddFactureCommandHandler>> _loggerMock;
        private readonly AddFactureCommandHandler _handler;

        public AddFactureCommandHandlerTests()
        {
            _factureRepositoryMock = new Mock<IFactureRepository>();
            _loggerMock = new Mock<ILogger<AddFactureCommandHandler>>();
            _handler = new AddFactureCommandHandler(_factureRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_Should_AddFacture_When_RequestIsValid()
        {
            // Arrange
            var createRequest = new CreateFactureRequest
            {
                PatientId = Guid.NewGuid(),
                ConsultationId = Guid.NewGuid(),
                ClinicId = Guid.NewGuid(),
                MontantTotal = 100m
            };
            var request = new AddFactureCommand(createRequest);

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _factureRepositoryMock.Verify(r => r.AddFactureAsync(It.Is<Facture>(f =>
                f.PatientId == createRequest.PatientId &&
                f.ConsultationId == createRequest.ConsultationId &&
                f.ClinicId == createRequest.ClinicId &&
                f.MontantTotal == createRequest.MontantTotal &&
                f.Status == FactureStatus.IMPAYEE
            )), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentNullException_When_RequestIsNull()
        {
            // Act
            Func<Task> act = async () => await _handler.Handle(null!, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentException_When_MontantTotalIsZeroOrNegative()
        {
            // Arrange
            var createRequest = new CreateFactureRequest
            {
                PatientId = Guid.NewGuid(),
                ConsultationId = Guid.NewGuid(),
                ClinicId = Guid.NewGuid(),
                MontantTotal = 0m
            };
            var request = new AddFactureCommand(createRequest);

            // Act
            Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Le montant total de la facture doit être supérieur à zéro*");
        }
    }
}
