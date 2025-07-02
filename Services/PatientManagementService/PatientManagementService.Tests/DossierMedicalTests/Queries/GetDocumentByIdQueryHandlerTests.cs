using FluentAssertions;
using Moq;
using PatientManagementService.Application.DossierMedicalService.Queries.GetDocumentById;
using PatientManagementService.Domain.Interfaces;
using PatientManagementService.Domain.Entities;

namespace PatientManagementService.Tests.DossierMedicalTests.Queries
{
    public class GetDocumentByIdQueryHandlerTests
    {
        private readonly Mock<IDossierMedicalRepository> _dossierMedicalRepositoryMock;
        private readonly GetDocumentByIdQueryHandler _handler;

        public GetDocumentByIdQueryHandlerTests()
        {
            _dossierMedicalRepositoryMock = new Mock<IDossierMedicalRepository>();
            _handler = new GetDocumentByIdQueryHandler(_dossierMedicalRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnDocument_WhenDocumentExists()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var document = new Document
            {
                Id = documentId,
                Nom = "doc.pdf",
                Url = "url",
                Type = "PDF"
            };
            _dossierMedicalRepositoryMock.Setup(r => r.GetDocumentByIdAsync(documentId)).ReturnsAsync(document);
            var query = new GetDocumentByIdQuery(documentId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(document);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenDocumentNotFound()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            _dossierMedicalRepositoryMock.Setup(r => r.GetDocumentByIdAsync(documentId)).ReturnsAsync((Document?)null);
            var query = new GetDocumentByIdQuery(documentId);

            // Act
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            var ex = await act.Should().ThrowAsync<Exception>();
            ex.Which.Message.Should().Be("Document not found");
        }
    }
}
