using Clinic.Domain.Entities;
using Clinic.Domain.Enums;
using Clinic.Infrastructure.Data;
using Clinic.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http.Json;

namespace Clinic.Tests
{
    public class CliniqueRepositoryTests
    {
        private CliniqueDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<CliniqueDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            // IMediator n'est pas utilisé dans les tests, on peut passer null
            return new CliniqueDbContext(options, null!);
        }

        private IConfiguration GetMockConfiguration(string? gatewayUrl = "http://fake-gateway")
        {
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c[It.IsAny<string>()]).Returns(gatewayUrl);
            return configMock.Object;
        }

        private ILogger<CliniqueRepository> GetMockLogger()
        {
            return new Mock<ILogger<CliniqueRepository>>().Object;
        }

        private HttpClient GetMockHttpClient(HttpResponseMessage response)
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);
            return new HttpClient(handlerMock.Object);
        }

        [Fact]
        public async Task AddAsync_AjouteClinique()
        {
            var db = GetDbContext("Generated_" + nameof(AddAsync_AjouteClinique));
            var repo = new CliniqueRepository(db, new HttpClient(), GetMockConfiguration(), GetMockLogger());

            var clinique = new Clinique
            {
                Id = Guid.NewGuid(),
                Nom = "Clinique Test",
                Adresse = "Adresse",
                NumeroTelephone = "0102030405",
                Email = "test@clinique.fr",
                Statut = StatutClinique.Active,
                TypeClinique = TypeClinique.Publique,
                DateCreation = DateTime.UtcNow
            };

            await repo.AddAsync(clinique);

            Assert.Single(db.Cliniques);
            Assert.Equal("Clinique Test", db.Cliniques.First().Nom);
        }

        [Fact]
        public async Task GetAllAsync_RetourneToutesLesCliniques()
        {
            var db = GetDbContext("Original_" + nameof(GetAllAsync_RetourneToutesLesCliniques));
            db.Cliniques.Add(new Clinique
            {
                Id = Guid.NewGuid(),
                Nom = "A",
                Adresse = "B",
                NumeroTelephone = "1",
                Email = "a@b.fr",
                Statut = StatutClinique.Active,
                TypeClinique = TypeClinique.Publique,
                DateCreation = DateTime.UtcNow
            });
            db.SaveChanges();
            var repo = new CliniqueRepository(db, new HttpClient(), GetMockConfiguration(), GetMockLogger());

            var result = await repo.GetAllAsync();

            Assert.Single(result);
        }


        [Fact]
        public async Task GetStatistiquesDesCliniquesAsync_RetourneStatistiqueClinique()
        {
            var db = GetDbContext(nameof(GetStatistiquesDesCliniquesAsync_RetourneStatistiqueClinique));
            var cliniqueId = Guid.NewGuid();
            db.Cliniques.Add(new Clinique
            {
                Id = cliniqueId,
                Nom = "Clinique",
                Adresse = "Adresse",
                NumeroTelephone = "1",
                Email = "a@b.fr",
                Statut = StatutClinique.Active,
                TypeClinique = TypeClinique.Publique,
                DateCreation = DateTime.UtcNow
            });
            db.SaveChanges();

            var medecinIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var responseMedecin = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(medecinIds)
            };
            var responseConsultation = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(5)
            };
            var responseRDV = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(10)
            };
            var responsePatient = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(3)
            };

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .SetupSequence<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMedecin)
                .ReturnsAsync(responseConsultation)
                .ReturnsAsync(responseRDV)
                .ReturnsAsync(responsePatient);

            var httpClient = new HttpClient(handlerMock.Object);

            var repo = new CliniqueRepository(db, httpClient, GetMockConfiguration(), GetMockLogger());

            var stats = await repo.GetStatistiquesDesCliniquesAsync(cliniqueId);

            Assert.Equal(cliniqueId, stats.CliniqueId);
            Assert.Equal("Clinique", stats.Nom);
            Assert.Equal(2, stats.NombreMedecins);
            Assert.Equal(5, stats.NombreConsultations);
            Assert.Equal(10, stats.NombreRendezVous);
            Assert.Equal(3, stats.NombrePatients);
        }
    }
    public class CliniqueRepositoryGeneratedTests
    {
        private CliniqueDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<CliniqueDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new CliniqueDbContext(options, null!);
        }

        private IConfiguration GetMockConfiguration(string? gatewayUrl = "http://fake-gateway")
        {
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["ServiceUrls:Gateway"]).Returns(gatewayUrl);
            return configMock.Object;
        }

        private ILogger<CliniqueRepository> GetMockLogger()
        {
            return new Mock<ILogger<CliniqueRepository>>().Object;
        }

        private HttpClient GetMockHttpClient(params HttpResponseMessage[] responses)
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            var sequence = handlerMock.Protected()
                .SetupSequence<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>());
            foreach (var resp in responses)
                sequence = sequence.ReturnsAsync(resp);
            return new HttpClient(handlerMock.Object);
        }

        [Fact]
        public async Task AddAsync_AjouteClinique()
        {
            var db = GetDbContext(nameof(AddAsync_AjouteClinique));
            var repo = new CliniqueRepository(db, new HttpClient(), GetMockConfiguration(), GetMockLogger());
            var clinique = new Clinique
            {
                Id = Guid.NewGuid(),
                Nom = "Test",
                Adresse = "Adresse",
                NumeroTelephone = "0102030405",
                Email = "test@clinique.fr",
                Statut = StatutClinique.Active,
                TypeClinique = TypeClinique.Publique,
                DateCreation = DateTime.UtcNow
            };
            await repo.AddAsync(clinique);
            Assert.Single(db.Cliniques);
        }

        [Fact]
        public async Task GetAllAsync_RetourneToutesLesCliniques()
        {
            var db = GetDbContext(nameof(GetAllAsync_RetourneToutesLesCliniques));
            db.Cliniques.Add(new Clinique
            {
                Id = Guid.NewGuid(),
                Nom = "A",
                Adresse = "B",
                NumeroTelephone = "1",
                Email = "a@b.fr",
                Statut = StatutClinique.Active,
                TypeClinique = TypeClinique.Publique,
                DateCreation = DateTime.UtcNow
            });
            db.SaveChanges();
            var repo = new CliniqueRepository(db, new HttpClient(), GetMockConfiguration(), GetMockLogger());
            var result = await repo.GetAllAsync();
            Assert.Single(result);
        }

        [Fact]
        public async Task GetByIdAsync_RetourneCliniqueOuNull()
        {
            var db = GetDbContext(nameof(GetByIdAsync_RetourneCliniqueOuNull));
            var id = Guid.NewGuid();
            db.Cliniques.Add(new Clinique
            {
                Id = id,
                Nom = "A",
                Adresse = "B",
                NumeroTelephone = "1",
                Email = "a@b.fr",
                Statut = StatutClinique.Active,
                TypeClinique = TypeClinique.Publique,
                DateCreation = DateTime.UtcNow
            });
            db.SaveChanges();
            var repo = new CliniqueRepository(db, new HttpClient(), GetMockConfiguration(), GetMockLogger());
            var found = await repo.GetByIdAsync(id);
            var notFound = await repo.GetByIdAsync(Guid.NewGuid());
            Assert.NotNull(found);
            Assert.Null(notFound);
        }

        [Fact]
        public async Task UpdateAsync_ModifieClinique()
        {
            var db = GetDbContext(nameof(UpdateAsync_ModifieClinique));
            var id = Guid.NewGuid();
            db.Cliniques.Add(new Clinique
            {
                Id = id,
                Nom = "A",
                Adresse = "B",
                NumeroTelephone = "1",
                Email = "a@b.fr",
                Statut = StatutClinique.Active,
                TypeClinique = TypeClinique.Publique,
                DateCreation = DateTime.UtcNow
            });
            db.SaveChanges();
            var repo = new CliniqueRepository(db, new HttpClient(), GetMockConfiguration(), GetMockLogger());
            var clinique = db.Cliniques.First();
            clinique.Nom = "Modifié";
            await repo.UpdateAsync(clinique);
            Assert.Equal("Modifié", db.Cliniques.First().Nom);
        }

        [Fact]
        public async Task DeleteAsync_SupprimeClinique()
        {
            var db = GetDbContext(nameof(DeleteAsync_SupprimeClinique));
            var id = Guid.NewGuid();
            db.Cliniques.Add(new Clinique
            {
                Id = id,
                Nom = "A",
                Adresse = "B",
                NumeroTelephone = "1",
                Email = "a@b.fr",
                Statut = StatutClinique.Active,
                TypeClinique = TypeClinique.Publique,
                DateCreation = DateTime.UtcNow
            });
            db.SaveChanges();
            var repo = new CliniqueRepository(db, new HttpClient(), GetMockConfiguration(), GetMockLogger());
            await repo.DeleteAsync(id);
            Assert.Empty(db.Cliniques);
        }

        [Fact]
        public async Task GetByNameAsync_RetourneCliniquesParNom()
        {
            var db = GetDbContext(nameof(GetByNameAsync_RetourneCliniquesParNom));
            db.Cliniques.Add(new Clinique
            {
                Id = Guid.NewGuid(),
                Nom = "Clinique Alpha",
                Adresse = "Adresse",
                NumeroTelephone = "1",
                Email = "a@b.fr",
                Statut = StatutClinique.Active,
                TypeClinique = TypeClinique.Publique,
                DateCreation = DateTime.UtcNow
            });
            db.SaveChanges();
            var repo = new CliniqueRepository(db, new HttpClient(), GetMockConfiguration(), GetMockLogger());
            var result = await repo.GetByNameAsync("alpha");
            Assert.Single(result);
        }

        [Fact]
        public async Task GetByAddressAsync_RetourneCliniquesParAdresse()
        {
            var db = GetDbContext(nameof(GetByAddressAsync_RetourneCliniquesParAdresse));
            db.Cliniques.Add(new Clinique
            {
                Id = Guid.NewGuid(),
                Nom = "Clinique",
                Adresse = "Rue de Paris",
                NumeroTelephone = "1",
                Email = "a@b.fr",
                Statut = StatutClinique.Active,
                TypeClinique = TypeClinique.Publique,
                DateCreation = DateTime.UtcNow
            });
            db.SaveChanges();
            var repo = new CliniqueRepository(db, new HttpClient(), GetMockConfiguration(), GetMockLogger());
            var result = await repo.GetByAddressAsync("paris");
            Assert.Single(result);
        }

        [Fact]
        public async Task GetByTypeAsync_RetourneCliniquesParType()
        {
            var db = GetDbContext(nameof(GetByTypeAsync_RetourneCliniquesParType));
            db.Cliniques.Add(new Clinique
            {
                Id = Guid.NewGuid(),
                Nom = "Clinique",
                Adresse = "Adresse",
                NumeroTelephone = "1",
                Email = "a@b.fr",
                Statut = StatutClinique.Active,
                TypeClinique = TypeClinique.Dentaire,
                DateCreation = DateTime.UtcNow
            });
            db.SaveChanges();
            var repo = new CliniqueRepository(db, new HttpClient(), GetMockConfiguration(), GetMockLogger());
            var result = await repo.GetByTypeAsync(TypeClinique.Dentaire);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetByStatusAsync_RetourneCliniquesParStatut()
        {
            var db = GetDbContext(nameof(GetByStatusAsync_RetourneCliniquesParStatut));
            db.Cliniques.Add(new Clinique
            {
                Id = Guid.NewGuid(),
                Nom = "Clinique",
                Adresse = "Adresse",
                NumeroTelephone = "1",
                Email = "a@b.fr",
                Statut = StatutClinique.Inactive,
                TypeClinique = TypeClinique.Publique,
                DateCreation = DateTime.UtcNow
            });
            db.SaveChanges();
            var repo = new CliniqueRepository(db, new HttpClient(), GetMockConfiguration(), GetMockLogger());
            var result = await repo.GetByStatusAsync(StatutClinique.Inactive);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetNombreCliniquesAsync_RetourneNombre()
        {
            var db = GetDbContext(nameof(GetNombreCliniquesAsync_RetourneNombre));
            db.Cliniques.Add(new Clinique
            {
                Id = Guid.NewGuid(),
                Nom = "Clinique",
                Adresse = "Adresse",
                NumeroTelephone = "1",
                Email = "a@b.fr",
                Statut = StatutClinique.Active,
                TypeClinique = TypeClinique.Publique,
                DateCreation = DateTime.UtcNow
            });
            db.SaveChanges();
            var repo = new CliniqueRepository(db, new HttpClient(), GetMockConfiguration(), GetMockLogger());
            var count = await repo.GetNombreCliniquesAsync();
            Assert.Equal(1, count);
        }

        [Fact]
        public async Task GetNombreNouvellesCliniquesDuMoisAsync_RetourneNombre()
        {
            var db = GetDbContext(nameof(GetNombreNouvellesCliniquesDuMoisAsync_RetourneNombre));
            db.Cliniques.Add(new Clinique
            {
                Id = Guid.NewGuid(),
                Nom = "Clinique",
                Adresse = "Adresse",
                NumeroTelephone = "1",
                Email = "a@b.fr",
                Statut = StatutClinique.Active,
                TypeClinique = TypeClinique.Publique,
                DateCreation = DateTime.UtcNow
            });
            db.Cliniques.Add(new Clinique
            {
                Id = Guid.NewGuid(),
                Nom = "Ancienne",
                Adresse = "Adresse",
                NumeroTelephone = "2",
                Email = "b@b.fr",
                Statut = StatutClinique.Active,
                TypeClinique = TypeClinique.Publique,
                DateCreation = DateTime.UtcNow.AddMonths(-2)
            });
            db.SaveChanges();
            var repo = new CliniqueRepository(db, new HttpClient(), GetMockConfiguration(), GetMockLogger());
            var count = await repo.GetNombreNouvellesCliniquesDuMoisAsync();
            Assert.Equal(1, count);
        }

        [Fact]
        public async Task GetNombreNouvellesCliniquesParMoisAsync_RetourneStatistiques()
        {
            var db = GetDbContext(nameof(GetNombreNouvellesCliniquesParMoisAsync_RetourneStatistiques));
            db.Cliniques.Add(new Clinique
            {
                Id = Guid.NewGuid(),
                Nom = "Clinique",
                Adresse = "Adresse",
                NumeroTelephone = "1",
                Email = "a@b.fr",
                Statut = StatutClinique.Active,
                TypeClinique = TypeClinique.Publique,
                DateCreation = DateTime.UtcNow
            });
            db.Cliniques.Add(new Clinique
            {
                Id = Guid.NewGuid(),
                Nom = "Ancienne",
                Adresse = "Adresse",
                NumeroTelephone = "2",
                Email = "b@b.fr",
                Statut = StatutClinique.Active,
                TypeClinique = TypeClinique.Publique,
                DateCreation = DateTime.UtcNow.AddMonths(-2)
            });
            db.SaveChanges();
            var repo = new CliniqueRepository(db, new HttpClient(), GetMockConfiguration(), GetMockLogger());
            var stats = (await repo.GetNombreNouvellesCliniquesParMoisAsync()).ToList();
            Assert.True(stats.Count >= 1);
            Assert.All(stats, s => Assert.True(s.Nombre > 0));
        }

        [Fact]
        public async Task GetStatistiquesDesCliniquesAsync_RetourneStatistiqueClinique()
        {
            var db = GetDbContext(nameof(GetStatistiquesDesCliniquesAsync_RetourneStatistiqueClinique));
            var cliniqueId = Guid.NewGuid();
            db.Cliniques.Add(new Clinique
            {
                Id = cliniqueId,
                Nom = "Clinique",
                Adresse = "Adresse",
                NumeroTelephone = "1",
                Email = "a@b.fr",
                Statut = StatutClinique.Active,
                TypeClinique = TypeClinique.Publique,
                DateCreation = DateTime.UtcNow
            });
            db.SaveChanges();

            var medecinIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var responseMedecin = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(medecinIds)
            };
            var responseConsultation = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(5)
            };
            var responseRDV = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(10)
            };
            var responsePatient = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(3)
            };

            var httpClient = GetMockHttpClient(responseMedecin, responseConsultation, responseRDV, responsePatient);

            var repo = new CliniqueRepository(db, httpClient, GetMockConfiguration(), GetMockLogger());
            var stats = await repo.GetStatistiquesDesCliniquesAsync(cliniqueId);

            Assert.Equal(cliniqueId, stats.CliniqueId);
            Assert.Equal("Clinique", stats.Nom);
            Assert.Equal(2, stats.NombreMedecins);
            Assert.Equal(5, stats.NombreConsultations);
            Assert.Equal(10, stats.NombreRendezVous);
            Assert.Equal(3, stats.NombrePatients);
        }
    }
}
