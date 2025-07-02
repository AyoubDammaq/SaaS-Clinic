using FluentAssertions;
using Moq;
using RDV.Application.Queries.GetAllRendezVous;
using RDV.Domain.Entities;
using RDV.Domain.Enums;
using RDV.Domain.Interfaces;

namespace RDV.Tests.Queries
{
    public class GetAllRendezVousQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnRendezVousDTOList_WhenRepositoryReturnsEntities()
        {
            // Arrange
            var rendezVousList = new List<RendezVous>
                {
                    new RendezVous
                    {
                        Id = Guid.NewGuid(),
                        PatientId = Guid.NewGuid(),
                        MedecinId = Guid.NewGuid(),
                        DateHeure = DateTime.Now,
                        Statut = RDVstatus.CONFIRME,
                        Commentaire = "Test"
                    },
                    new RendezVous
                    {
                        Id = Guid.NewGuid(),
                        PatientId = Guid.NewGuid(),
                        MedecinId = Guid.NewGuid(),
                        DateHeure = DateTime.Now.AddDays(1),
                        Statut = RDVstatus.EN_ATTENTE,
                        Commentaire = null
                    }
                };

            var repoMock = new Mock<IRendezVousRepository>();
            repoMock.Setup(r => r.GetAllRendezVousAsync())
                .ReturnsAsync(rendezVousList);

            var handler = new GetAllRendezVousQueryHandler(repoMock.Object);
            var query = new GetAllRendezVousQuery();

            // Act
            var result = (await handler.Handle(query, CancellationToken.None)).ToList();

            // Assert
            result.Should().HaveCount(2);
            result[0].Id.Should().Be(rendezVousList[0].Id);
            result[0].PatientId.Should().Be(rendezVousList[0].PatientId);
            result[0].MedecinId.Should().Be(rendezVousList[0].MedecinId);
            result[0].DateHeure.Should().Be(rendezVousList[0].DateHeure);
            result[0].Statut.Should().Be(rendezVousList[0].Statut);
            result[0].Commentaire.Should().Be("Test");

            result[1].Id.Should().Be(rendezVousList[1].Id);
            result[1].Commentaire.Should().BeEmpty(); // null devient string.Empty
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenRepositoryReturnsNoEntities()
        {
            // Arrange
            var repoMock = new Mock<IRendezVousRepository>();
            repoMock.Setup(r => r.GetAllRendezVousAsync())
                .ReturnsAsync(new List<RendezVous>());

            var handler = new GetAllRendezVousQueryHandler(repoMock.Object);
            var query = new GetAllRendezVousQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
