using AutoMapper;
using FluentAssertions;
using Moq;
using PatientManagementService.Application.DossierMedicalService.Commands.AddDossierMedical;
using PatientManagementService.Application.DTOs;
using PatientManagementService.Domain.Entities;
using PatientManagementService.Domain.Interfaces;

namespace PatientManagementService.Tests.DossierMedicalTests.Commands
{
    public class AddDossierMedicalCommandHandlerTests
    {
        private readonly Mock<IDossierMedicalRepository> _dossierMedicalRepositoryMock;
        private readonly Mock<IPatientRepository> _patientRepositoryMock;
        private readonly Mock<IMapper> _mapperMock; // Ajoutez ce mock
        private readonly AddDossierMedicalCommandHandler _handler;

        public AddDossierMedicalCommandHandlerTests()
        {
            _dossierMedicalRepositoryMock = new Mock<IDossierMedicalRepository>();
            _patientRepositoryMock = new Mock<IPatientRepository>();
            _mapperMock = new Mock<IMapper>(); // Instanciez le mock
            _handler = new AddDossierMedicalCommandHandler(
                _dossierMedicalRepositoryMock.Object,
                _patientRepositoryMock.Object,
                _mapperMock.Object // Passez-le au handler
            );
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenPatientNotFound()
        {
            // Arrange
            var dossierDto = new DossierMedicalDTO
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.NewGuid()
            };
            var command = new AddDossierMedicalCommand(dossierDto);
            _patientRepositoryMock.Setup(r => r.GetPatientByIdAsync(dossierDto.PatientId))
                .ReturnsAsync((Patient?)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            var ex = await act.Should().ThrowAsync<Exception>();
            ex.Which.Message.Should().Be("Patient not found");
            _dossierMedicalRepositoryMock.Verify(r => r.AddDossierMedicalAsync(It.IsAny<DossierMedical>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowInvalidOperationException_WhenPatientAlreadyHasDossier()
        {
            // Arrange
            var dossierDto = new DossierMedicalDTO
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.NewGuid()
            };
            var patient = new Patient { Id = dossierDto.PatientId, DossierMedicalId = Guid.NewGuid() };
            var command = new AddDossierMedicalCommand(dossierDto);
            _patientRepositoryMock.Setup(r => r.GetPatientByIdAsync(dossierDto.PatientId))
                .ReturnsAsync(patient);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            var ex = await act.Should().ThrowAsync<InvalidOperationException>();
            ex.Which.Message.Should().Be("Patient already has a dossier médical.");
            _dossierMedicalRepositoryMock.Verify(r => r.AddDossierMedicalAsync(It.IsAny<DossierMedical>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldAddDossierMedicalAndUpdatePatient_WhenValid()
        {
            // Arrange
            var dossierDto = new DossierMedicalDTO
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                Allergies = "Aucune",
                MaladiesChroniques = "Asthme",
                MedicamentsActuels = "Ventoline",
                AntécédentsFamiliaux = "Aucun",
                AntécédentsPersonnels = "Opération appendicite",
                GroupeSanguin = "O+"
            };
            var patientMock = new Mock<Patient>();
            patientMock.SetupAllProperties();
            patientMock.Object.Id = dossierDto.PatientId;
            patientMock.Object.DossierMedicalId = null;

            var command = new AddDossierMedicalCommand(dossierDto);
            _patientRepositoryMock.Setup(r => r.GetPatientByIdAsync(dossierDto.PatientId))
                .ReturnsAsync(patientMock.Object);

            // Ajoutez ce setup si le handler utilise AutoMapper pour mapper le DTO vers l'entité
            _mapperMock.Setup(m => m.Map<DossierMedical>(dossierDto))
                .Returns(new DossierMedical
                {
                    Id = dossierDto.Id,
                    PatientId = dossierDto.PatientId,
                    Allergies = dossierDto.Allergies,
                    MaladiesChroniques = dossierDto.MaladiesChroniques,
                    MedicamentsActuels = dossierDto.MedicamentsActuels,
                    AntécédentsFamiliaux = dossierDto.AntécédentsFamiliaux,
                    AntécédentsPersonnels = dossierDto.AntécédentsPersonnels,
                    GroupeSanguin = dossierDto.GroupeSanguin
                });

            _dossierMedicalRepositoryMock.Setup(r => r.AddDossierMedicalAsync(It.IsAny<DossierMedical>()))
                .Returns(Task.CompletedTask)
                .Verifiable();
            _patientRepositoryMock.Setup(r => r.UpdatePatientAsync(It.IsAny<Patient>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            patientMock.Object.DossierMedicalId.Should().Be(dossierDto.Id);
            patientMock.Verify(p => p.ModifierPatientEvent(), Times.Once);
            _dossierMedicalRepositoryMock.Verify(r => r.AddDossierMedicalAsync(It.Is<DossierMedical>(
                d => d.Id == dossierDto.Id &&
                     d.PatientId == dossierDto.PatientId &&
                     d.Allergies == dossierDto.Allergies &&
                     d.MaladiesChroniques == dossierDto.MaladiesChroniques &&
                     d.MedicamentsActuels == dossierDto.MedicamentsActuels &&
                     d.AntécédentsFamiliaux == dossierDto.AntécédentsFamiliaux &&
                     d.AntécédentsPersonnels == dossierDto.AntécédentsPersonnels &&
                     d.GroupeSanguin == dossierDto.GroupeSanguin
            )), Times.Once);
            _patientRepositoryMock.Verify(r => r.UpdatePatientAsync(patientMock.Object), Times.Once);
        }
    }
}
