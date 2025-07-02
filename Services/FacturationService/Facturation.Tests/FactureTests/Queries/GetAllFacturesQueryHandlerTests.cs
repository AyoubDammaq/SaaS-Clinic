using Facturation.Application.FactureService.Queries.GetAllFactures;
using Facturation.Domain.Entities;
using Facturation.Domain.Enums;
using Facturation.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Facturation.Tests.FactureTests.Queries
{
    public class GetAllFacturesQueryHandlerTests
    {
        private readonly Mock<IFactureRepository> _factureRepositoryMock;
        private readonly GetAllFacturesQueryHandler _handler;

        public GetAllFacturesQueryHandlerTests()
        {
            _factureRepositoryMock = new Mock<IFactureRepository>();
            _handler = new GetAllFacturesQueryHandler(_factureRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnMappedFactures_When_FacturesExist()
        {
            // Arrange
            var factures = new List<Facture>
                {
                    new Facture
                    {
                        Id = Guid.NewGuid(),
                        PatientId = Guid.NewGuid(),
                        ConsultationId = Guid.NewGuid(),
                        ClinicId = Guid.NewGuid(),
                        DateEmission = DateTime.UtcNow,
                        MontantTotal = 100m,
                        Status = FactureStatus.PAYEE
                    },
                    new Facture
                    {
                        Id = Guid.NewGuid(),
                        PatientId = Guid.NewGuid(),
                        ConsultationId = Guid.NewGuid(),
                        ClinicId = Guid.NewGuid(),
                        DateEmission = DateTime.UtcNow.AddDays(-1),
                        MontantTotal = 200m,
                        Status = FactureStatus.IMPAYEE
                    }
                };

            _factureRepositoryMock.Setup(r => r.GetAllFacturesAsync())
                .ReturnsAsync(factures);

            var request = new GetAllFacturesQuery();

            // Act
            var result = (await _handler.Handle(request, CancellationToken.None)).ToList();

            // Assert
            result.Should().HaveCount(2);
            result[0].PatientId.Should().Be(factures[0].PatientId);
            result[0].ConsultationId.Should().Be(factures[0].ConsultationId);
            result[0].ClinicId.Should().Be(factures[0].ClinicId);
            result[0].DateEmission.Should().Be(factures[0].DateEmission);
            result[0].MontantTotal.Should().Be(factures[0].MontantTotal);
            result[0].Status.Should().Be(factures[0].Status);

            result[1].PatientId.Should().Be(factures[1].PatientId);
            result[1].ConsultationId.Should().Be(factures[1].ConsultationId);
            result[1].ClinicId.Should().Be(factures[1].ClinicId);
            result[1].DateEmission.Should().Be(factures[1].DateEmission);
            result[1].MontantTotal.Should().Be(factures[1].MontantTotal);
            result[1].Status.Should().Be(factures[1].Status);
        }

        [Fact]
        public async Task Handle_Should_ReturnEmptyList_When_NoFacturesExist()
        {
            // Arrange
            _factureRepositoryMock.Setup(r => r.GetAllFacturesAsync())
                .ReturnsAsync(new List<Facture>());

            var request = new GetAllFacturesQuery();

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
