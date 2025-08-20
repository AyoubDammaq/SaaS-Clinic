using FluentAssertions;
using Moq;
using PatientManagementService.Application.DossierMedicalService.Queries.GetDossierMedicalByPatientId;
using PatientManagementService.Domain.Entities;
using PatientManagementService.Domain.Interfaces;

namespace PatientManagementService.Tests.DossierMedicalTests.Queries
{
    public class GetDossierMedicalByPatientIdQueryHandlerTests
    {
        private readonly Mock<IDossierMedicalRepository> _dossierMedicalRepositoryMock;
        private readonly GetDossierMedicalByPatientIdQueryHandler _handler;

        public GetDossierMedicalByPatientIdQueryHandlerTests()
        {
            _dossierMedicalRepositoryMock = new Mock<IDossierMedicalRepository>();
            _handler = new GetDossierMedicalByPatientIdQueryHandler(_dossierMedicalRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnDossierMedical_WhenDossierMedicalExistsForPatient()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var dossier = new DossierMedical
            {
                Id = Guid.NewGuid(),
                PatientId = patientId,
                Allergies = "Aucune",
                MaladiesChroniques = "Asthme",
                GroupeSanguin = "O+"
            };
            _dossierMedicalRepositoryMock.Setup(r => r.GetDossierMedicalByPatientIdAsync(patientId)).ReturnsAsync(dossier);
            var query = new GetDossierMedicalByPatientIdQuery(patientId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(dossier);
        }

        [Fact]
        public async Task Handle_ShouldReturnNull_WhenDossierMedicalNotFoundForPatient()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            _dossierMedicalRepositoryMock.Setup(r => r.GetDossierMedicalByPatientIdAsync(patientId)).ReturnsAsync((DossierMedical?)null);
            var query = new GetDossierMedicalByPatientIdQuery(patientId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}
