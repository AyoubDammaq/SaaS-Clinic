using FluentAssertions;
using Moq;
using RDV.Application.Queries.GetStatistiques;
using RDV.Domain.Entities;
using RDV.Domain.Enums;
using RDV.Domain.Interfaces;

namespace RDV.Tests.Queries
{
    public class GetStatistiquesQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnStatistiquesGroupedByDate()
        {
            // Arrange
            var dateDebut = new DateTime(2024, 6, 1);
            var dateFin = new DateTime(2024, 6, 2);

            var rendezVousList = new List<RendezVous>
                {
                    new RendezVous
                    {
                        Id = Guid.NewGuid(),
                        PatientId = Guid.NewGuid(),
                        MedecinId = Guid.NewGuid(),
                        DateHeure = new DateTime(2024, 6, 1, 10, 0, 0),
                        Statut = RDVstatus.CONFIRME,
                        Commentaire = "A"
                    },
                    new RendezVous
                    {
                        Id = Guid.NewGuid(),
                        PatientId = Guid.NewGuid(),
                        MedecinId = Guid.NewGuid(),
                        DateHeure = new DateTime(2024, 6, 1, 11, 0, 0),
                        Statut = RDVstatus.ANNULE,
                        Commentaire = "B"
                    },
                    new RendezVous
                    {
                        Id = Guid.NewGuid(),
                        PatientId = Guid.NewGuid(),
                        MedecinId = Guid.NewGuid(),
                        DateHeure = new DateTime(2024, 6, 2, 9, 0, 0),
                        Statut = RDVstatus.EN_ATTENTE,
                        Commentaire = "C"
                    }
                };

            var repoMock = new Mock<IRendezVousRepository>();
            repoMock.Setup(r => r.GetRendezVousByPeriod(dateDebut, dateFin))
                .ReturnsAsync(rendezVousList);

            var handler = new GetStatistiquesQueryHandler(repoMock.Object);
            var query = new GetStatistiquesQuery(dateDebut, dateFin);

            // Act
            var result = (await handler.Handle(query, CancellationToken.None)).ToList();

            // Assert
            result.Should().HaveCount(2);

            var stat1 = result.FirstOrDefault(s => s.Date == new DateTime(2024, 6, 1));
            stat1.Should().NotBeNull();
            stat1.TotalRendezVous.Should().Be(2);
            stat1.Confirmes.Should().Be(1);
            stat1.Annules.Should().Be(1);
            stat1.EnAttente.Should().Be(0);

            var stat2 = result.FirstOrDefault(s => s.Date == new DateTime(2024, 6, 2));
            stat2.Should().NotBeNull();
            stat2.TotalRendezVous.Should().Be(1);
            stat2.Confirmes.Should().Be(0);
            stat2.Annules.Should().Be(0);
            stat2.EnAttente.Should().Be(1);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoRendezVousInPeriod()
        {
            // Arrange
            var dateDebut = new DateTime(2024, 6, 1);
            var dateFin = new DateTime(2024, 6, 2);

            var repoMock = new Mock<IRendezVousRepository>();
            repoMock.Setup(r => r.GetRendezVousByPeriod(dateDebut, dateFin))
                .ReturnsAsync(new List<RendezVous>());

            var handler = new GetStatistiquesQueryHandler(repoMock.Object);
            var query = new GetStatistiquesQuery(dateDebut, dateFin);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
