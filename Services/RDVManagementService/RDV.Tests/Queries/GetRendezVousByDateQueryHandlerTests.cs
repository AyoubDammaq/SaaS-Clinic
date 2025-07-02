using FluentAssertions;
using Moq;
using RDV.Application.Queries.GetRendezVousByDate;
using RDV.Domain.Entities;
using RDV.Domain.Enums;
using RDV.Domain.Interfaces;

namespace RDV.Tests.Queries
{
    public class GetRendezVousByDateQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnRendezVousList_WhenRepositoryReturnsEntities()
        {
            // Arrange
            var date = new DateTime(2024, 6, 1);
            var rendezVousList = new List<RendezVous>
                {
                    new RendezVous
                    {
                        Id = Guid.NewGuid(),
                        PatientId = Guid.NewGuid(),
                        MedecinId = Guid.NewGuid(),
                        DateHeure = date.AddHours(10),
                        Statut = RDVstatus.CONFIRME,
                        Commentaire = "Test"
                    },
                    new RendezVous
                    {
                        Id = Guid.NewGuid(),
                        PatientId = Guid.NewGuid(),
                        MedecinId = Guid.NewGuid(),
                        DateHeure = date.AddHours(14),
                        Statut = RDVstatus.EN_ATTENTE,
                        Commentaire = null
                    }
                };

            var repoMock = new Mock<IRendezVousRepository>();
            repoMock.Setup(r => r.GetRendezVousByDateAsync(date))
                .ReturnsAsync(rendezVousList);

            var handler = new GetRendezVousByDateQueryHandler(repoMock.Object);
            var query = new GetRendezVousByDateQuery(date);

            // Act
            var result = (await handler.Handle(query, CancellationToken.None)).ToList();

            // Assert
            result.Should().HaveCount(2);
            result[0].Id.Should().Be(rendezVousList[0].Id);
            result[0].DateHeure.Should().Be(rendezVousList[0].DateHeure);
            result[1].Statut.Should().Be(RDVstatus.EN_ATTENTE);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenRepositoryReturnsNoEntities()
        {
            // Arrange
            var date = DateTime.Today;
            var repoMock = new Mock<IRendezVousRepository>();
            repoMock.Setup(r => r.GetRendezVousByDateAsync(date))
                .ReturnsAsync(new List<RendezVous>());

            var handler = new GetRendezVousByDateQueryHandler(repoMock.Object);
            var query = new GetRendezVousByDateQuery(date);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
