using AutoMapper;
using Clinic.Application.Commands.AjouterClinique;
using Clinic.Application.DTOs;
using Clinic.Application.EventHandlers;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;
using Clinic.Domain.Events;
using Clinic.Domain.Interfaces;
using Clinic.Domain.Interfaces.Messaging;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Clinic.Tests.Commands
{
    public class AjouterCliniqueHandlerTests
    {
        private readonly Mock<ICliniqueRepository> _repositoryMock;
        private readonly Mock<ILogger<AjouterCliniqueHandler>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly AjouterCliniqueHandler _handler;

        public AjouterCliniqueHandlerTests()
        {
            _repositoryMock = new Mock<ICliniqueRepository>();
            _loggerMock = new Mock<ILogger<AjouterCliniqueHandler>>();
            _mapperMock = new Mock<IMapper>();
            _handler = new AjouterCliniqueHandler(_repositoryMock.Object, _loggerMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidData_ShouldAddCliniqueAndReturnIt()
        {
            // Arrange
            var dto = new CliniqueDto
            {
                Nom = "Clinique Ibn Nafis",
                Adresse = "Ariana",
                NumeroTelephone = "12345678",
                Email = "contact@nafis.tn",
                SiteWeb = "www.nafis.tn",
                Description = "Clinique multidisciplinaire",
                TypeClinique = TypeClinique.Privee,
                Statut = StatutClinique.Active
            };

            var clinique = new Clinique
            {
                Nom = dto.Nom,
                Adresse = dto.Adresse,
                NumeroTelephone = dto.NumeroTelephone,
                Email = dto.Email,
                SiteWeb = dto.SiteWeb,
                Description = dto.Description,
                TypeClinique = dto.TypeClinique,
                Statut = dto.Statut
            };

            _mapperMock.Setup(m => m.Map<Clinique>(dto)).Returns(clinique);

            var command = new AjouterCliniqueCommand(dto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Nom.Should().Be(dto.Nom);
            result.Adresse.Should().Be(dto.Adresse);
            result.Email.Should().Be(dto.Email);
            result.TypeClinique.Should().Be(dto.TypeClinique);
            result.Statut.Should().Be(dto.Statut);

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Clinique>()), Times.Once);
        }

        [Theory]
        [InlineData("", "Rue 123")]
        [InlineData("Clinique ABC", "")]
        public async Task Handle_WithMissingRequiredFields_ShouldThrowArgumentException(string nom, string adresse)
        {
            // Arrange
            var dto = new CliniqueDto
            {
                Nom = nom,
                Adresse = adresse,
                NumeroTelephone = "98765432",
                Email = "abc@clinique.tn",
                SiteWeb = "www.abc.tn",
                Description = "Description",
                TypeClinique = TypeClinique.Privee,
                Statut = StatutClinique.Active
            };

            var clinique = new Clinique
            {
                Nom = dto.Nom,
                Adresse = dto.Adresse,
                NumeroTelephone = dto.NumeroTelephone,
                Email = dto.Email,
                SiteWeb = dto.SiteWeb,
                Description = dto.Description,
                TypeClinique = dto.TypeClinique,
                Statut = dto.Statut
            };

            _mapperMock.Setup(m => m.Map<Clinique>(dto)).Returns(clinique);

            var command = new AjouterCliniqueCommand(dto);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>();
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Clinique>()), Times.Never);
        }

        [Fact]
        public async Task Handle_PublieMessageKafkaEtLogInfo()
        {
            // Arrange
            var producerMock = new Mock<IKafkaProducer>();
            var loggerMock = new Mock<ILogger<CliniqueCreatedHandler>>();

            var handler = new CliniqueCreatedHandler(producerMock.Object, loggerMock.Object);

            var clinique = new Clinique
            {
                Id = Guid.NewGuid(),
                Nom = "Clinique Demo",
                Adresse = "Adresse",
                NumeroTelephone = "0102030405",
                Email = "demo@clinique.fr",
                Statut = StatutClinique.Active,
                TypeClinique = TypeClinique.Publique,
                DateCreation = DateTime.UtcNow
            };
            var evt = new CliniqueCreated(clinique);

            // Act
            await handler.Handle(evt, CancellationToken.None);

            // Assert
            producerMock.Verify(p => p.PublishAsync("clinique-created", evt, It.IsAny<CancellationToken>()), Times.Once);

            loggerMock.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Nouvelle clinique créée")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
