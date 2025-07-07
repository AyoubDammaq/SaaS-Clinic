using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using PatientManagementService.Application.DTOs;
using PatientManagementService.Application.PatientService.Commands.AddPatient;
using PatientManagementService.Domain.Entities;
using PatientManagementService.Domain.Interfaces;

namespace PatientManagementService.Tests.PatientTests.Commands
{
    public class AddPatientCommandHandlerTests
    {
        private readonly Mock<IPatientRepository> _patientRepositoryMock;
        private readonly Mock<ILogger<AddPatientCommandHandler>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock; // Ajoutez ce mock
        private readonly AddPatientCommandHandler _handler;

        public AddPatientCommandHandlerTests()
        {
            _patientRepositoryMock = new Mock<IPatientRepository>();
            _loggerMock = new Mock<ILogger<AddPatientCommandHandler>>();
            _mapperMock = new Mock<IMapper>(); // Instanciez le mock
            _handler = new AddPatientCommandHandler(_patientRepositoryMock.Object, _loggerMock.Object, _mapperMock.Object); // Passez-le au handler
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_AndLogWarning_WhenPatientIsNull()
        {
            // Arrange
            CreatePatientDTO? patientDto = null; // Utilisez un type nullable
            var command = new AddPatientCommand(patientDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) =>
                        v != null && v.ToString() != null && v.ToString().Contains("Tentative d'ajouter un patient null.")), // Remplacer l'opérateur de propagation null
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
            _patientRepositoryMock.Verify(r => r.AddPatientAsync(It.IsAny<Patient>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnTrue_AndCallRepository_WhenPatientIsValid()
        {
            // Arrange
            var patientDto = new CreatePatientDTO
            {
                Nom = "Dupont",
                Prenom = "Jean",
                DateNaissance = new DateTime(1990, 1, 1),
                Sexe = "M",
                Adresse = "1 rue de Paris",
                Telephone = "0102030405",
                Email = "jean.dupont@email.com"
            };
            var command = new AddPatientCommand(patientDto);

            // Setup du mapping si nécessaire
            _mapperMock.Setup(m => m.Map<Patient>(patientDto)).Returns(new Patient
            {
                Nom = patientDto.Nom,
                Prenom = patientDto.Prenom,
                DateNaissance = patientDto.DateNaissance,
                Sexe = patientDto.Sexe,
                Adresse = patientDto.Adresse,
                Telephone = patientDto.Telephone,
                Email = patientDto.Email
            });

            _patientRepositoryMock.Setup(r => r.AddPatientAsync(It.IsAny<Patient>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            _patientRepositoryMock.Verify(r => r.AddPatientAsync(It.Is<Patient>(
                p => p.Nom == patientDto.Nom &&
                     p.Prenom == patientDto.Prenom &&
                     p.DateNaissance == patientDto.DateNaissance &&
                     p.Sexe == patientDto.Sexe &&
                     p.Adresse == patientDto.Adresse &&
                     p.Telephone == patientDto.Telephone &&
                     p.Email == patientDto.Email
            )), Times.Once);
        }
    }
}
