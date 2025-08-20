// C#
using Doctor.Application.AvailibilityServices.Queries.GetMedecinsDisponibles;
using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;
using Doctor.Application.DTOs;
using Doctor.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Doctor.Tests.DisponibiliteTests.Queries
{
    public class GetMedecinsDisponiblesQueryHandlerTests
    {
        private readonly Mock<IDisponibiliteRepository> _disponibiliteRepositoryMock;
        private readonly Mock<IRendezVousHttpClient> _rdvClientMock;
        private readonly Mock<ILogger<GetMedecinsDisponiblesQueryHandler>> _loggerMock;
        private readonly GetMedecinsDisponiblesQueryHandler _handler;

        public GetMedecinsDisponiblesQueryHandlerTests()
        {
            _disponibiliteRepositoryMock = new Mock<IDisponibiliteRepository>();
            _rdvClientMock = new Mock<IRendezVousHttpClient>();
            _loggerMock = new Mock<ILogger<GetMedecinsDisponiblesQueryHandler>>();
            _handler = new GetMedecinsDisponiblesQueryHandler(
                _disponibiliteRepositoryMock.Object,
                _rdvClientMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenDateIsDefault()
        {
            var query = new GetMedecinsDisponiblesQuery(default, null, null);
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoMedecinHasDisponibiliteForDay()
        {
            var date = new DateTime(2024, 6, 1); // Saturday
            var medecins = new List<Medecin>
            {
                new Medecin
                {
                    Id = Guid.NewGuid(),
                    Prenom = "Paul",
                    Nom = "Martin",
                    Specialite = "Cardiologie",
                    Email = "paul.martin@email.com",
                    Telephone = "0600000000",
                    PhotoUrl = "http://photo.url",
                    Disponibilites = new List<Disponibilite>() // aucune dispo
                }
            };
            var query = new GetMedecinsDisponiblesQuery(date, null, null);

            _disponibiliteRepositoryMock
                .Setup(r => r.ObtenirMedecinsAvecDisponibilitesAsync())
                .ReturnsAsync(medecins);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenAllSlotsAreTakenByConfirmedRDV()
        {
            var date = new DateTime(2024, 6, 3); // Monday
            var medecinId = Guid.NewGuid();
            var dispo = new Disponibilite
            {
                Id = Guid.NewGuid(),
                Jour = DayOfWeek.Monday,
                HeureDebut = new TimeSpan(8, 0, 0),
                HeureFin = new TimeSpan(9, 0, 0),
                MedecinId = medecinId
            };
            var medecin = new Medecin
            {
                Id = medecinId,
                Prenom = "Paul",
                Nom = "Martin",
                Specialite = "Cardiologie",
                Email = "paul.martin@email.com",
                Telephone = "0600000000",
                PhotoUrl = "http://photo.url",
                Disponibilites = new List<Disponibilite> { dispo }
            };
            var rdvs = new List<RendezVousDTO>
            {
                new RendezVousDTO
                {
                    Id = Guid.NewGuid(),
                    MedecinId = medecinId,
                    DateHeure = date.Date + dispo.HeureDebut,
                    Statut = RDVstatus.CONFIRME
                },
                new RendezVousDTO
                {
                    Id = Guid.NewGuid(),
                    MedecinId = medecinId,
                    DateHeure = date.Date + dispo.HeureDebut + TimeSpan.FromMinutes(30),
                    Statut = RDVstatus.CONFIRME
                }
            };
            var query = new GetMedecinsDisponiblesQuery(date, null, null);

            _disponibiliteRepositoryMock
                .Setup(r => r.ObtenirMedecinsAvecDisponibilitesAsync())
                .ReturnsAsync(new List<Medecin> { medecin });

            _rdvClientMock
                .Setup(c => c.GetRendezVousParMedecinEtDate(medecinId, date))
                .ReturnsAsync(rdvs);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task Handle_ShouldReturnMedecin_WhenAtLeastOneSlotIsAvailable()
        {
            var date = DateTime.Now.Date.AddDays(1); // Demain
            var medecinId = Guid.NewGuid();
            var dispo = new Disponibilite
            {
                Id = Guid.NewGuid(),
                Jour = date.DayOfWeek,
                HeureDebut = new TimeSpan(8, 0, 0),
                HeureFin = new TimeSpan(9, 0, 0),
                MedecinId = medecinId
            };
            var medecin = new Medecin
            {
                Id = medecinId,
                Prenom = "Paul",
                Nom = "Martin",
                Specialite = "Cardiologie",
                Email = "paul.martin@email.com",
                Telephone = "0600000000",
                PhotoUrl = "http://photo.url",
                Disponibilites = new List<Disponibilite> { dispo }
            };
            var rdvs = new List<RendezVousDTO>
            {
                new RendezVousDTO
                {
                    Id = Guid.NewGuid(),
                    MedecinId = medecinId,
                    DateHeure = date.Date + dispo.HeureDebut, // 8h00-8h30 occupé
                    Statut = RDVstatus.CONFIRME
                }
            };
            var query = new GetMedecinsDisponiblesQuery(date, null, null);

            _disponibiliteRepositoryMock
                .Setup(r => r.ObtenirMedecinsAvecDisponibilitesAsync())
                .ReturnsAsync(new List<Medecin> { medecin });

            _rdvClientMock
                .Setup(c => c.GetRendezVousParMedecinEtDate(medecinId, date))
                .ReturnsAsync(rdvs);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(medecinId, result[0].Id);
        }

        [Fact]
        public async Task Handle_ShouldRespectHeureDebutEtHeureFin()
        {
            var date = DateTime.Now.Date.AddDays(1);
            var medecinId = Guid.NewGuid();
            var dispo = new Disponibilite
            {
                Id = Guid.NewGuid(),
                Jour = date.DayOfWeek,
                HeureDebut = new TimeSpan(8, 0, 0),
                HeureFin = new TimeSpan(12, 0, 0),
                MedecinId = medecinId
            };
            var medecin = new Medecin
            {
                Id = medecinId,
                Prenom = "Paul",
                Nom = "Martin",
                Specialite = "Cardiologie",
                Email = "paul.martin@email.com",
                Telephone = "0600000000",
                PhotoUrl = "http://photo.url",
                Disponibilites = new List<Disponibilite> { dispo }
            };
            var rdvs = new List<RendezVousDTO>(); // aucun RDV
            var heureDebut = new TimeSpan(10, 0, 0);
            var heureFin = new TimeSpan(11, 0, 0);
            var query = new GetMedecinsDisponiblesQuery(date, heureDebut, heureFin);

            _disponibiliteRepositoryMock
                .Setup(r => r.ObtenirMedecinsAvecDisponibilitesAsync())
                .ReturnsAsync(new List<Medecin> { medecin });

            _rdvClientMock
                .Setup(c => c.GetRendezVousParMedecinEtDate(medecinId, date))
                .ReturnsAsync(rdvs);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(medecinId, result[0].Id);
        }

        [Fact]
        public async Task Handle_ShouldLogInformation_WhenMedecinsFound()
        {
            var date = DateTime.Now.Date.AddDays(1);
            var medecinId = Guid.NewGuid();
            var dispo = new Disponibilite
            {
                Id = Guid.NewGuid(),
                Jour = date.DayOfWeek,
                HeureDebut = new TimeSpan(8, 0, 0),
                HeureFin = new TimeSpan(9, 0, 0),
                MedecinId = medecinId
            };
            var medecin = new Medecin
            {
                Id = medecinId,
                Prenom = "Paul",
                Nom = "Martin",
                Specialite = "Cardiologie",
                Email = "paul.martin@email.com",
                Telephone = "0600000000",
                PhotoUrl = "http://photo.url",
                Disponibilites = new List<Disponibilite> { dispo }
            };
            var rdvs = new List<RendezVousDTO>();
            var query = new GetMedecinsDisponiblesQuery(date, null, null);

            _disponibiliteRepositoryMock
                .Setup(r => r.ObtenirMedecinsAvecDisponibilitesAsync())
                .ReturnsAsync(new List<Medecin> { medecin });

            _rdvClientMock
                .Setup(c => c.GetRendezVousParMedecinEtDate(medecinId, date))
                .ReturnsAsync(rdvs);

            var result = await _handler.Handle(query, CancellationToken.None);

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("médecins disponibles trouvés")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once
            );
        }
    }
}
