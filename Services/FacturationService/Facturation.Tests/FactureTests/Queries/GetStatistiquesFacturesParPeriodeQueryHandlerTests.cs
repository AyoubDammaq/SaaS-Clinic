using Facturation.Application.FactureService.Queries.GetStatistiquesFacturesParPeriode;
using Facturation.Domain.Entities;
using Facturation.Domain.Enums;
using Facturation.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Facturation.Tests.FactureTests.Queries
{
    public class GetStatistiquesFacturesParPeriodeQueryHandlerTests
    {
        private readonly Mock<IFactureRepository> _factureRepositoryMock;
        private readonly GetStatistiquesFacturesParPeriodeQueryHandler _handler;

        public GetStatistiquesFacturesParPeriodeQueryHandlerTests()
        {
            _factureRepositoryMock = new Mock<IFactureRepository>();
            _handler = new GetStatistiquesFacturesParPeriodeQueryHandler(_factureRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnCorrectStats_When_FacturesExist()
        {
            // Arrange
            var dateDebut = new DateTime(2024, 1, 1);
            var dateFin = new DateTime(2024, 1, 31);
            var clinicA = Guid.NewGuid();
            var clinicB = Guid.NewGuid();

            var factures = new List<Facture>
                {
                    new Facture { Id = Guid.NewGuid(), ClinicId = clinicA, Status = FactureStatus.PAYEE, MontantTotal = 100, MontantPaye = 100 },
                    new Facture { Id = Guid.NewGuid(), ClinicId = clinicA, Status = FactureStatus.IMPAYEE, MontantTotal = 200, MontantPaye = 0 },
                    new Facture { Id = Guid.NewGuid(), ClinicId = clinicB, Status = FactureStatus.PARTIELLEMENT_PAYEE, MontantTotal = 300, MontantPaye = 150 },
                    new Facture { Id = Guid.NewGuid(), ClinicId = clinicB, Status = FactureStatus.PAYEE, MontantTotal = 400, MontantPaye = 400 }
                };

            _factureRepositoryMock.Setup(r => r.GetFacturesParPeriode(dateDebut, dateFin))
                .ReturnsAsync(factures);

            var request = new GetStatistiquesFacturesParPeriodeQuery(dateDebut, dateFin);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.NombreTotal.Should().Be(4);
            result.NombrePayees.Should().Be(2);
            result.NombreImpayees.Should().Be(1);
            result.NombrePartiellementPayees.Should().Be(1);
            result.MontantTotal.Should().Be(100 + 200 + 300 + 400);
            result.MontantTotalPaye.Should().Be(100 + 0 + 150 + 400);
            result.NombreParClinique.Should().ContainKey(clinicA).And.ContainKey(clinicB);
            result.NombreParClinique[clinicA].Should().Be(2);
            result.NombreParClinique[clinicB].Should().Be(2);
        }

        [Fact]
        public async Task Handle_Should_ReturnZeroStats_When_NoFactures()
        {
            // Arrange
            var dateDebut = new DateTime(2024, 1, 1);
            var dateFin = new DateTime(2024, 1, 31);

            _factureRepositoryMock.Setup(r => r.GetFacturesParPeriode(dateDebut, dateFin))
                .ReturnsAsync(new List<Facture>());

            var request = new GetStatistiquesFacturesParPeriodeQuery(dateDebut, dateFin);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.NombreTotal.Should().Be(0);
            result.NombrePayees.Should().Be(0);
            result.NombreImpayees.Should().Be(0);
            result.NombrePartiellementPayees.Should().Be(0);
            result.MontantTotal.Should().Be(0);
            result.MontantTotalPaye.Should().Be(0);
            result.NombreParClinique.Should().BeEmpty();
        }
    }
}
