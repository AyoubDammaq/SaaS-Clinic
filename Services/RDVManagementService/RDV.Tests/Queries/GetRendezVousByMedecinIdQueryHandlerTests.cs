using FluentAssertions;
using Moq;
using RDV.Application.Queries.GetRendezVousByMedecinId;
using RDV.Domain.Entities;
using RDV.Domain.Enums;
using RDV.Domain.Interfaces;

namespace RDV.Tests.Queries
{
    public class GetRendezVousByMedecinIdQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnRendezVousList_WhenMedecinIdIsValid()
        {
            // Arrange
            var medecinId = Guid.NewGuid();
            var rendezVousList = new List<RendezVous>
                {
                    new RendezVous
                    {
                        Id = Guid.NewGuid(),
                        PatientId = Guid.NewGuid(),
                        MedecinId = medecinId,
                        DateHeure = DateTime.Now,
                        Statut = RDVstatus.CONFIRME,
                        Commentaire = "Test"
                    },
                    new RendezVous
                    {
                        Id = Guid.NewGuid(),
                        PatientId = Guid.NewGuid(),
                        MedecinId = medecinId,
                        DateHeure = DateTime.Now.AddHours(1),
                        Statut = RDVstatus.EN_ATTENTE,
                        Commentaire = null
                    }
                };

            var repoMock = new Mock<IRendezVousRepository>();
            repoMock.Setup(r => r.GetRendezVousByMedecinIdAsync(medecinId))
                .ReturnsAsync(rendezVousList);

            var handler = new GetRendezVousByMedecinIdQueryHandler(repoMock.Object);
            var query = new GetRendezVousByMedecinIdQuery(medecinId);

            // Act
            var result = (await handler.Handle(query, CancellationToken.None)).ToList();

            // Assert
            result.Should().HaveCount(2);
            result[0].MedecinId.Should().Be(medecinId);
            result[1].Statut.Should().Be(RDVstatus.EN_ATTENTE);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoRendezVousForMedecin()
        {
            // Arrange
            var medecinId = Guid.NewGuid();
            var repoMock = new Mock<IRendezVousRepository>();
            repoMock.Setup(r => r.GetRendezVousByMedecinIdAsync(medecinId))
                .ReturnsAsync(new List<RendezVous>());

            var handler = new GetRendezVousByMedecinIdQueryHandler(repoMock.Object);
            var query = new GetRendezVousByMedecinIdQuery(medecinId);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenMedecinIdIsEmpty()
        {
            // Arrange
            var repoMock = new Mock<IRendezVousRepository>();
            var handler = new GetRendezVousByMedecinIdQueryHandler(repoMock.Object);
            var query = new GetRendezVousByMedecinIdQuery(Guid.Empty);

            // Act
            Func<Task> act = async () => await handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*L'identifiant du médecin ne peut pas être vide*");
        }
    }
}
