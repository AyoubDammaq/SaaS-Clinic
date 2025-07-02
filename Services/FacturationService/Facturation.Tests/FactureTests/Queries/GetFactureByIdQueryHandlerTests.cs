using Facturation.Application.FactureService.Queries.GetFactureById;
using Facturation.Domain.Entities;
using Facturation.Domain.Enums;
using Facturation.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Facturation.Tests.FactureTests.Queries
{
    public class GetFactureByIdQueryHandlerTests
    {
        private readonly Mock<IFactureRepository> _factureRepositoryMock;
        private readonly GetFactureByIdQueryHandler _handler;

        public GetFactureByIdQueryHandlerTests()
        {
            _factureRepositoryMock = new Mock<IFactureRepository>();
            _handler = new GetFactureByIdQueryHandler(_factureRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnFactureResponse_When_IdIsValid()
        {
            // Arrange
            var factureId = Guid.NewGuid();
            var facture = new Facture
            {
                Id = factureId,
                PatientId = Guid.NewGuid(),
                ConsultationId = Guid.NewGuid(),
                ClinicId = Guid.NewGuid(),
                DateEmission = DateTime.UtcNow,
                MontantTotal = 123.45m,
                Status = FactureStatus.PAYEE
            };
            _factureRepositoryMock.Setup(r => r.GetFactureByIdAsync(factureId))
                .ReturnsAsync(facture);

            var request = new GetFactureByIdQuery(factureId);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.PatientId.Should().Be(facture.PatientId);
            result.ConsultationId.Should().Be(facture.ConsultationId);
            result.ClinicId.Should().Be(facture.ClinicId);
            result.DateEmission.Should().Be(facture.DateEmission);
            result.MontantTotal.Should().Be(facture.MontantTotal);
            result.Status.Should().Be(facture.Status);
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentException_When_IdIsEmpty()
        {
            // Arrange
            var request = new GetFactureByIdQuery(Guid.Empty);

            // Act
            Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*L'identifiant de la facture ne peut pas être vide*");
        }

        [Fact]
        public async Task Handle_Should_ThrowNullReferenceException_When_FactureNotFound()
        {
            // Arrange
            var factureId = Guid.NewGuid();
            _factureRepositoryMock.Setup(r => r.GetFactureByIdAsync(factureId))
                .ReturnsAsync((Facture?)null);

            var request = new GetFactureByIdQuery(factureId);

            // Act
            Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NullReferenceException>();
        }
    }
}
