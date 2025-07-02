using FluentAssertions;
using Moq;
using RDV.Application.Queries.GetRendezVousByStatut;
using RDV.Domain.Entities;
using RDV.Domain.Enums;
using RDV.Domain.Interfaces;

namespace RDV.Tests.Queries
{
    public class GetRendezVousByStatutQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnRendezVousList_WhenRepositoryReturnsEntities()
        {
            // Arrange
            var statut = RDVstatus.CONFIRME;
            var rendezVousList = new List<RendezVous>
                {
                    new RendezVous
                    {
                        Id = Guid.NewGuid(),
                        PatientId = Guid.NewGuid(),
                        MedecinId = Guid.NewGuid(),
                        DateHeure = DateTime.Now,
                        Statut = statut,
                        Commentaire = "Test"
                    },
                    new RendezVous
                    {
                        Id = Guid.NewGuid(),
                        PatientId = Guid.NewGuid(),
                        MedecinId = Guid.NewGuid(),
                        DateHeure = DateTime.Now.AddHours(1),
                        Statut = statut,
                        Commentaire = null
                    }
                };

            var repoMock = new Mock<IRendezVousRepository>();
            repoMock.Setup(r => r.GetRendezVousByStatutAsync(statut))
                .ReturnsAsync(rendezVousList);

            var handler = new GetRendezVousByStatutQueryHandler(repoMock.Object);
            var query = new GetRendezVousByStatutQuery(statut);

            // Act
            var result = (await handler.Handle(query, CancellationToken.None)).ToList();

            // Assert
            result.Should().HaveCount(2);
            result.All(r => r.Statut == statut).Should().BeTrue();
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenRepositoryReturnsNoEntities()
        {
            // Arrange
            var statut = RDVstatus.ANNULE;
            var repoMock = new Mock<IRendezVousRepository>();
            repoMock.Setup(r => r.GetRendezVousByStatutAsync(statut))
                .ReturnsAsync(new List<RendezVous>());

            var handler = new GetRendezVousByStatutQueryHandler(repoMock.Object);
            var query = new GetRendezVousByStatutQuery(statut);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
