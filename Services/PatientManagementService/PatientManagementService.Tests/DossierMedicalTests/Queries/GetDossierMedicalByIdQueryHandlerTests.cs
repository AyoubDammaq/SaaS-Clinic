using FluentAssertions;
using Moq;
using PatientManagementService.Application.DossierMedicalService.Queries.GetDocumentById;
using PatientManagementService.Domain.Interfaces;
using PatientManagementService.Domain.Entities;
using PatientManagementService.Application.DossierMedicalService.Queries.GetDossierMedicalById;

namespace PatientManagementService.Tests.DossierMedicalTests.Queries
{
    public class GetDossierMedicalByIdQueryHandlerTests
    {
        private readonly Mock<IDossierMedicalRepository> _dossierMedicalRepositoryMock;
        private readonly GetDossierMedicalByIdQueryHandler _handler;

        public GetDossierMedicalByIdQueryHandlerTests()
        {
            _dossierMedicalRepositoryMock = new Mock<IDossierMedicalRepository>();
            _handler = new GetDossierMedicalByIdQueryHandler(_dossierMedicalRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnDossierMedical_WhenDossierMedicalExists()
        {
            // Arrange
            var dossierId = Guid.NewGuid();
            var dossier = new DossierMedical
            {
                Id = dossierId,
                Allergies = "Aucune",
                MaladiesChroniques = "Asthme",
                GroupeSanguin = "O+"
            };
            _dossierMedicalRepositoryMock.Setup(r => r.GetDossierMedicalByIdAsync(dossierId)).ReturnsAsync(dossier);
            var query = new GetDossierMedicalByIdQuery(dossierId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(dossier);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenDossierMedicalNotFound()
        {
            // Arrange
            var dossierId = Guid.NewGuid();
            _dossierMedicalRepositoryMock.Setup(r => r.GetDossierMedicalByIdAsync(dossierId)).ReturnsAsync((DossierMedical?)null);
            var query = new GetDossierMedicalByIdQuery(dossierId);

            // Act
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            var ex = await act.Should().ThrowAsync<Exception>();
            ex.Which.Message.Should().Be("Dossier médical not found");
        }
    }
}
