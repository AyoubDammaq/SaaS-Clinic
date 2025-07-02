using FluentAssertions;
using Moq;
using RDV.Application.Queries.GetRendezVousByPatientId;
using RDV.Domain.Entities;
using RDV.Domain.Enums;
using RDV.Domain.Interfaces;

namespace RDV.Tests.Queries
{
    public class GetRendezVousByPatientIdQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnRendezVousList_WhenPatientIdIsValid()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var rendezVousList = new List<RendezVous>
                {
                    new RendezVous
                    {
                        Id = Guid.NewGuid(),
                        PatientId = patientId,
                        MedecinId = Guid.NewGuid(),
                        DateHeure = DateTime.Now,
                        Statut = RDVstatus.CONFIRME,
                        Commentaire = "Test"
                    },
                    new RendezVous
                    {
                        Id = Guid.NewGuid(),
                        PatientId = patientId,
                        MedecinId = Guid.NewGuid(),
                        DateHeure = DateTime.Now.AddHours(1),
                        Statut = RDVstatus.EN_ATTENTE,
                        Commentaire = null
                    }
                };

            var repoMock = new Mock<IRendezVousRepository>();
            repoMock.Setup(r => r.GetRendezVousByPatientIdAsync(patientId))
                .ReturnsAsync(rendezVousList);

            var handler = new GetRendezVousByPatientIdQueryHandler(repoMock.Object);
            var query = new GetRendezVousByPatientIdQuery(patientId);

            // Act
            var result = (await handler.Handle(query, CancellationToken.None)).ToList();

            // Assert
            result.Should().HaveCount(2);
            result[0].PatientId.Should().Be(patientId);
            result[1].Statut.Should().Be(RDVstatus.EN_ATTENTE);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoRendezVousForPatient()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var repoMock = new Mock<IRendezVousRepository>();
            repoMock.Setup(r => r.GetRendezVousByPatientIdAsync(patientId))
                .ReturnsAsync(new List<RendezVous>());

            var handler = new GetRendezVousByPatientIdQueryHandler(repoMock.Object);
            var query = new GetRendezVousByPatientIdQuery(patientId);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenPatientIdIsEmpty()
        {
            // Arrange
            var repoMock = new Mock<IRendezVousRepository>();
            var handler = new GetRendezVousByPatientIdQueryHandler(repoMock.Object);
            var query = new GetRendezVousByPatientIdQuery(Guid.Empty);

            // Act
            Func<Task> act = async () => await handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*L'identifiant du patient ne peut pas être vide*");
        }
    }
}
