using Facturation.Application.DTOs;
using Facturation.Application.FactureService.Commands.UpdateFacture;
using Facturation.Domain.Entities;
using Facturation.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Facturation.Tests.FactureTests.Commands
{
    public class UpdateFactureCommandHandlerTests
    {
        private readonly Mock<IFactureRepository> _factureRepositoryMock;
        private readonly UpdateFactureCommandHandler _handler;

        public UpdateFactureCommandHandlerTests()
        {
            _factureRepositoryMock = new Mock<IFactureRepository>();
            _handler = new UpdateFactureCommandHandler(_factureRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_Should_UpdateFacture_When_RequestIsValid()
        {
            // Arrange
            var factureId = Guid.NewGuid();
            var existingFacture = new Facture
            {
                Id = factureId,
                PatientId = Guid.NewGuid(),
                ConsultationId = Guid.NewGuid(),
                ClinicId = Guid.NewGuid(),
                MontantTotal = 100m
            };
            var updateRequest = new UpdateFactureRequest
            {
                Id = factureId,
                PatientId = Guid.NewGuid(),
                ConsultationId = Guid.NewGuid(),
                ClinicId = Guid.NewGuid(),
                MontantTotal = 200m
            };
            var request = new UpdateFactureCommand(updateRequest);

            _factureRepositoryMock.Setup(r => r.GetFactureByIdAsync(factureId))
                .ReturnsAsync(existingFacture);

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _factureRepositoryMock.Verify(r => r.GetFactureByIdAsync(factureId), Times.Once);
            _factureRepositoryMock.Verify(r => r.UpdateFactureAsync(It.Is<Facture>(f =>
                f.Id == updateRequest.Id &&
                f.PatientId == updateRequest.PatientId &&
                f.ConsultationId == updateRequest.ConsultationId &&
                f.ClinicId == updateRequest.ClinicId &&
                f.MontantTotal == updateRequest.MontantTotal
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
        public async Task Handle_Should_ThrowArgumentException_When_IdIsEmpty()
        {
            // Arrange
            var updateRequest = new UpdateFactureRequest
            {
                Id = Guid.Empty,
                PatientId = Guid.NewGuid(),
                ConsultationId = Guid.NewGuid(),
                ClinicId = Guid.NewGuid(),
                MontantTotal = 100m
            };
            var request = new UpdateFactureCommand(updateRequest);

            // Act
            Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*L'identifiant de la facture ne peut pas être vide*");
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentException_When_PatientIdIsEmpty()
        {
            // Arrange
            var updateRequest = new UpdateFactureRequest
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.Empty,
                ConsultationId = Guid.NewGuid(),
                ClinicId = Guid.NewGuid(),
                MontantTotal = 100m
            };
            var request = new UpdateFactureCommand(updateRequest);

            // Act
            Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*L'identifiant du patient ne peut pas être vide*");
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentException_When_ConsultationIdIsEmpty()
        {
            // Arrange
            var updateRequest = new UpdateFactureRequest
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                ConsultationId = Guid.Empty,
                ClinicId = Guid.NewGuid(),
                MontantTotal = 100m
            };
            var request = new UpdateFactureCommand(updateRequest);

            // Act
            Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*L'identifiant de la consultation ne peut pas être vide*");
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentException_When_ClinicIdIsEmpty()
        {
            // Arrange
            var updateRequest = new UpdateFactureRequest
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                ConsultationId = Guid.NewGuid(),
                ClinicId = Guid.Empty,
                MontantTotal = 100m
            };
            var request = new UpdateFactureCommand(updateRequest);

            // Act
            Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*L'identifiant de la clinique ne peut pas être vide*");
        }

        [Fact]
        public async Task Handle_Should_ThrowKeyNotFoundException_When_FactureNotFound()
        {
            // Arrange
            var updateRequest = new UpdateFactureRequest
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                ConsultationId = Guid.NewGuid(),
                ClinicId = Guid.NewGuid(),
                MontantTotal = 100m
            };
            var request = new UpdateFactureCommand(updateRequest);

            _factureRepositoryMock.Setup(r => r.GetFactureByIdAsync(updateRequest.Id))
                .ReturnsAsync((Facture?)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage($"*{updateRequest.PatientId}*");
        }
    }
}
