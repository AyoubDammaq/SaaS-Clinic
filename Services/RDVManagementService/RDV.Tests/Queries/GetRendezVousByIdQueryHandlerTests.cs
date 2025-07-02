using FluentAssertions;
using Moq;
using RDV.Application.Queries.GetRendezVousById;
using RDV.Domain.Entities;
using RDV.Domain.Enums;
using RDV.Domain.Interfaces;

namespace RDV.Tests.Queries
{
    public class GetRendezVousByIdQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnRendezVous_WhenIdIsValid()
        {
            // Arrange
            var rendezVousId = Guid.NewGuid();
            var rendezVous = new RendezVous
            {
                Id = rendezVousId,
                PatientId = Guid.NewGuid(),
                MedecinId = Guid.NewGuid(),
                DateHeure = DateTime.Now,
                Statut = RDVstatus.CONFIRME,
                Commentaire = "Test"
            };
            var repoMock = new Mock<IRendezVousRepository>();
            repoMock.Setup(r => r.GetRendezVousByIdAsync(rendezVousId))
                .ReturnsAsync(rendezVous);

            var handler = new GetRendezVousByIdQueryHandler(repoMock.Object);
            var query = new GetRendezVousByIdQuery(rendezVousId);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(rendezVousId);
            repoMock.Verify(r => r.GetRendezVousByIdAsync(rendezVousId), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenIdIsEmpty()
        {
            // Arrange
            var repoMock = new Mock<IRendezVousRepository>();
            var handler = new GetRendezVousByIdQueryHandler(repoMock.Object);
            var query = new GetRendezVousByIdQuery(Guid.Empty);

            // Act
            Func<Task> act = async () => await handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*L'identifiant du rendez-vous ne peut pas être vide*");
        }

        [Fact]
        public async Task Handle_ShouldReturnNull_WhenRendezVousNotFound()
        {
            // Arrange
            var rendezVousId = Guid.NewGuid();
            var repoMock = new Mock<IRendezVousRepository>();
            repoMock.Setup(r => r.GetRendezVousByIdAsync(rendezVousId))
                .ReturnsAsync((RendezVous)null);

            var handler = new GetRendezVousByIdQueryHandler(repoMock.Object);
            var query = new GetRendezVousByIdQuery(rendezVousId);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
            repoMock.Verify(r => r.GetRendezVousByIdAsync(rendezVousId), Times.Once);
        }
    }
}
