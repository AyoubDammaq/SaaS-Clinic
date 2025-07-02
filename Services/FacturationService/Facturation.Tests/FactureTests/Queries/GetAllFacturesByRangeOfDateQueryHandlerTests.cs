using Facturation.Application.FactureService.Queries.GetAllFacturesByRangeOfDate;
using Facturation.Domain.Entities;
using Facturation.Domain.Enums;
using Facturation.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Facturation.Tests.FactureTests.Queries
{
    public class GetAllFacturesByRangeOfDateQueryHandlerTests
    {
        private readonly Mock<IFactureRepository> _factureRepositoryMock;
        private readonly GetAllFacturesByRangeOfDateQueryHandler _handler;

        public GetAllFacturesByRangeOfDateQueryHandlerTests()
        {
            _factureRepositoryMock = new Mock<IFactureRepository>();
            _handler = new GetAllFacturesByRangeOfDateQueryHandler(_factureRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnMappedFactures_When_FacturesExist()
        {
            // Arrange
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 1, 31);
            var factures = new List<Facture>
                {
                    new Facture
                    {
                        Id = Guid.NewGuid(),
                        PatientId = Guid.NewGuid(),
                        ConsultationId = Guid.NewGuid(),
                        ClinicId = Guid.NewGuid(),
                        DateEmission = new DateTime(2024, 1, 10),
                        MontantTotal = 100m,
                        Status = FactureStatus.PAYEE
                    },
                    new Facture
                    {
                        Id = Guid.NewGuid(),
                        PatientId = Guid.NewGuid(),
                        ConsultationId = Guid.NewGuid(),
                        ClinicId = Guid.NewGuid(),
                        DateEmission = new DateTime(2024, 1, 20),
                        MontantTotal = 200m,
                        Status = FactureStatus.IMPAYEE
                    }
                };

            _factureRepositoryMock.Setup(r => r.GetAllFacturesByRangeOfDateAsync(startDate, endDate))
                .ReturnsAsync(factures);

            var request = new GetAllFacturesByRangeOfDateQuery(startDate, endDate);

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
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 1, 31);

            _factureRepositoryMock.Setup(r => r.GetAllFacturesByRangeOfDateAsync(startDate, endDate))
                .ReturnsAsync(new List<Facture>());

            var request = new GetAllFacturesByRangeOfDateQuery(startDate, endDate);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentException_When_StartDateIsAfterEndDate()
        {
            // Arrange
            var startDate = new DateTime(2024, 2, 1);
            var endDate = new DateTime(2024, 1, 1);
            var request = new GetAllFacturesByRangeOfDateQuery(startDate, endDate);

            // Act
            Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*La date de début ne peut pas être postérieure à la date de fin*");
        }
    }
}
