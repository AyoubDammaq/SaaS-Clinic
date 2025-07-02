using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using PatientManagementService.Application.DTOs;
using PatientManagementService.Application.PatientService.Commands.UpdatePatient;
using PatientManagementService.Domain.Entities;
using PatientManagementService.Domain.Interfaces;

namespace PatientManagementService.Tests.PatientTests.Commands
{
    public class UpdatePatientCommandHandlerTests
    {
        private readonly Mock<IPatientRepository> _patientRepositoryMock;
        private readonly Mock<ILogger<UpdatePatientCommandHandler>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock; // Ajout du mock pour IMapper
        private readonly UpdatePatientCommandHandler _handler;

        public UpdatePatientCommandHandlerTests()
        {
            _patientRepositoryMock = new Mock<IPatientRepository>();
            _loggerMock = new Mock<ILogger<UpdatePatientCommandHandler>>();
            _mapperMock = new Mock<IMapper>();
            _handler = new UpdatePatientCommandHandler(_patientRepositoryMock.Object, _loggerMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_AndLogWarning_WhenPatientIsNull()
        {
            // Arrange
            var command = new UpdatePatientCommand(null!);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v != null && v.ToString() != null && v.ToString().Contains("Mise à jour échouée : patient null.")),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
            _patientRepositoryMock.Verify(r => r.UpdatePatientAsync(It.IsAny<Patient>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_AndLogWarning_WhenPatientNotFound()
        {
            // Arrange
            var patientDto = new PatientDTO
            {
                Id = Guid.NewGuid(),
                Nom = "Durand",
                Prenom = "Marie",
                DateNaissance = new DateTime(1985, 5, 5),
                Sexe = "F",
                Adresse = "2 avenue de Lyon",
                Telephone = "0607080910",
                Email = "marie.durand@email.com"
            };
            var command = new UpdatePatientCommand(patientDto);

            _patientRepositoryMock.Setup(r => r.GetPatientByIdAsync(patientDto.Id))
                .ReturnsAsync((Patient?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v != null && v.ToString() != null && v.ToString().Contains($"Patient introuvable pour mise à jour (ID : {patientDto.Id})")),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
            _patientRepositoryMock.Verify(r => r.UpdatePatientAsync(It.IsAny<Patient>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldUpdatePatient_AndReturnTrue_WhenPatientExists()
        {
            // Arrange
            var patientDto = new PatientDTO
            {
                Id = Guid.NewGuid(),
                Nom = "Martin",
                Prenom = "Paul",
                DateNaissance = new DateTime(1970, 3, 3),
                Sexe = "M",
                Adresse = "3 rue Victor Hugo",
                Telephone = "0708091011",
                Email = "paul.martin@email.com"
            };
            var existingPatient = new Patient
            {
                Id = patientDto.Id,
                Nom = "AncienNom",
                Prenom = "AncienPrenom",
                DateNaissance = new DateTime(1960, 1, 1),
                Sexe = "F",
                Adresse = "Ancienne adresse",
                Telephone = "0000000000",
                Email = "ancien@email.com"
            };
            var command = new UpdatePatientCommand(patientDto);

            _patientRepositoryMock.Setup(r => r.GetPatientByIdAsync(patientDto.Id))
                .ReturnsAsync(existingPatient);
            _patientRepositoryMock.Setup(r => r.UpdatePatientAsync(It.IsAny<Patient>()))
                .Returns(Task.CompletedTask);

            // Ajout du mapping pour simuler la mise à jour des propriétés
            _mapperMock.Setup(m => m.Map<PatientDTO, Patient>(patientDto, existingPatient))
                .Callback<PatientDTO, Patient>((src, dest) =>
                {
                    dest.Nom = src.Nom;
                    dest.Prenom = src.Prenom;
                    dest.DateNaissance = src.DateNaissance;
                    dest.Sexe = src.Sexe;
                    dest.Adresse = src.Adresse;
                    dest.Telephone = src.Telephone;
                    dest.Email = src.Email;
                })
                .Returns(existingPatient);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            existingPatient.Nom.Should().Be(patientDto.Nom);
            existingPatient.Prenom.Should().Be(patientDto.Prenom);
            existingPatient.DateNaissance.Should().Be(patientDto.DateNaissance);
            existingPatient.Sexe.Should().Be(patientDto.Sexe);
            existingPatient.Adresse.Should().Be(patientDto.Adresse);
            existingPatient.Telephone.Should().Be(patientDto.Telephone);
            existingPatient.Email.Should().Be(patientDto.Email);
            _patientRepositoryMock.Verify(r => r.UpdatePatientAsync(existingPatient), Times.Once);
        }
    }
}
