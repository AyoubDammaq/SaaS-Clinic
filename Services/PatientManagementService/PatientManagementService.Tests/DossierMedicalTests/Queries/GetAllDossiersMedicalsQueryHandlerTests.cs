using FluentAssertions;
using Moq;
using PatientManagementService.Application.DossierMedicalService.Queries.GetAllDossiersMedicals;
using PatientManagementService.Domain.Entities;
using PatientManagementService.Domain.Interfaces;

namespace PatientManagementService.Tests.DossierMedicalTests.Queries
{
    public class GetAllDossiersMedicalsQueryHandlerTests
    {
        private readonly Mock<IDossierMedicalRepository> _dossierMedicalRepositoryMock;
        private readonly GetAllDossiersMedicalsQueryHandler _handler;

        public GetAllDossiersMedicalsQueryHandlerTests()
        {
            _dossierMedicalRepositoryMock = new Mock<IDossierMedicalRepository>();
            _handler = new GetAllDossiersMedicalsQueryHandler(_dossierMedicalRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnDossiersMedicals_WhenRepositoryReturnsList()
        {
            // Arrange
            var dossiers = new List<DossierMedical>
            {
                new DossierMedical { Id = Guid.NewGuid(), Allergies = "Aucune" },
                new DossierMedical { Id = Guid.NewGuid(), Allergies = "Pollen" }
            };
            _dossierMedicalRepositoryMock.Setup(r => r.GetAllDossiersMedicalsAsync()).ReturnsAsync(dossiers);

            // Act
            var result = await _handler.Handle(new GetAllDossiersMedicalsQuery(), CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(dossiers);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenRepositoryReturnsEmpty()
        {
            // Arrange
            var dossiers = new List<DossierMedical>();
            _dossierMedicalRepositoryMock.Setup(r => r.GetAllDossiersMedicalsAsync()).ReturnsAsync(dossiers);

            // Act
            var result = await _handler.Handle(new GetAllDossiersMedicalsQuery(), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
    }
}
