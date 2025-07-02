using AutoMapper;
using FluentAssertions;
using Moq;
using PatientManagementService.Application.DossierMedicalService.Commands.UpdateDossierMedical;
using PatientManagementService.Application.DTOs;
using PatientManagementService.Domain.Entities;
using PatientManagementService.Domain.Interfaces;

namespace PatientManagementService.Tests.DossierMedicalTests.Commands
{
    public class UpdateDossierMedicalCommandHandlerTests
    {
        private readonly Mock<IDossierMedicalRepository> _dossierMedicalRepositoryMock;
        private readonly Mock<IMapper> _mapperMock; // Ajoutez ce mock
        private readonly UpdateDossierMedicalCommandHandler _handler;

        public UpdateDossierMedicalCommandHandlerTests()
        {
            _dossierMedicalRepositoryMock = new Mock<IDossierMedicalRepository>();
            _mapperMock = new Mock<IMapper>(); // Instanciez le mock
            _handler = new UpdateDossierMedicalCommandHandler(_dossierMedicalRepositoryMock.Object, _mapperMock.Object); // Passez-le au handler
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenDossierMedicalNotFound()
        {
            // Arrange
            var dossierDto = new DossierMedicalDTO
            {
                Id = Guid.NewGuid(),
                Allergies = "Aucune",
                MaladiesChroniques = "Asthme",
                MedicamentsActuels = "Ventoline",
                AntécédentsFamiliaux = "Aucun",
                AntécédentsPersonnels = "Opération appendicite",
                GroupeSanguin = "O+"
            };
            var command = new UpdateDossierMedicalCommand(dossierDto);
            _dossierMedicalRepositoryMock.Setup(r => r.GetDossierMedicalByIdAsync(dossierDto.Id))
                .ReturnsAsync((DossierMedical?)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            var ex = await act.Should().ThrowAsync<Exception>();
            ex.Which.Message.Should().Be("Dossier médical not found");
            _dossierMedicalRepositoryMock.Verify(r => r.UpdateDossierMedicalAsync(It.IsAny<DossierMedical>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldUpdateDossierMedicalAndCallEvent_WhenValid()
        {
            // Arrange
            var dossierDto = new DossierMedicalDTO
            {
                Id = Guid.NewGuid(),
                Allergies = "Aucune",
                MaladiesChroniques = "Asthme",
                MedicamentsActuels = "Ventoline",
                AntécédentsFamiliaux = "Aucun",
                AntécédentsPersonnels = "Opération appendicite",
                GroupeSanguin = "O+"
            };

            var dossierMedicalMock = new Mock<DossierMedical>();
            dossierMedicalMock.SetupAllProperties();
            dossierMedicalMock.Object.Id = dossierDto.Id;

            var command = new UpdateDossierMedicalCommand(dossierDto);

            _dossierMedicalRepositoryMock.Setup(r => r.GetDossierMedicalByIdAsync(dossierDto.Id))
                .ReturnsAsync(dossierMedicalMock.Object);
            _dossierMedicalRepositoryMock.Setup(r => r.UpdateDossierMedicalAsync(dossierMedicalMock.Object))
                .Returns(Task.CompletedTask);

            // Simule le mapping des propriétés du DTO vers l'entité existante
            _mapperMock.Setup(m => m.Map<DossierMedicalDTO, DossierMedical>(dossierDto, dossierMedicalMock.Object))
                .Callback<DossierMedicalDTO, DossierMedical>((src, dest) =>
                {
                    dest.Allergies = src.Allergies;
                    dest.MaladiesChroniques = src.MaladiesChroniques;
                    dest.MedicamentsActuels = src.MedicamentsActuels;
                    dest.AntécédentsFamiliaux = src.AntécédentsFamiliaux;
                    dest.AntécédentsPersonnels = src.AntécédentsPersonnels;
                    dest.GroupeSanguin = src.GroupeSanguin;
                })
                .Returns(dossierMedicalMock.Object);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            dossierMedicalMock.Object.Allergies.Should().Be(dossierDto.Allergies);
            dossierMedicalMock.Object.MaladiesChroniques.Should().Be(dossierDto.MaladiesChroniques);
            dossierMedicalMock.Object.MedicamentsActuels.Should().Be(dossierDto.MedicamentsActuels);
            dossierMedicalMock.Object.AntécédentsFamiliaux.Should().Be(dossierDto.AntécédentsFamiliaux);
            dossierMedicalMock.Object.AntécédentsPersonnels.Should().Be(dossierDto.AntécédentsPersonnels);
            dossierMedicalMock.Object.GroupeSanguin.Should().Be(dossierDto.GroupeSanguin);
            dossierMedicalMock.Verify(d => d.ModifierDossierMedicalEvent(), Times.Once);
            _dossierMedicalRepositoryMock.Verify(r => r.UpdateDossierMedicalAsync(dossierMedicalMock.Object), Times.Once);
        }
    }
}
