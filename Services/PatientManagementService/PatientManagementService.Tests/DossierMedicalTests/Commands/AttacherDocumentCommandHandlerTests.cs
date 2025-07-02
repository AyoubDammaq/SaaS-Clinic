using AutoMapper;
using FluentAssertions;
using Moq;
using PatientManagementService.Application.DossierMedicalService.Commands.AttacherDocument;
using PatientManagementService.Application.DTOs;
using PatientManagementService.Domain.Entities;
using PatientManagementService.Domain.Interfaces;

namespace PatientManagementService.Tests.DossierMedicalTests.Commands
{
    public class AttacherDocumentCommandHandlerTests
    {
        private readonly Mock<IDossierMedicalRepository> _dossierMedicalRepositoryMock;
        private readonly Mock<IMapper> _mapperMock; // Ajoutez ce mock
        private readonly AttacherDocumentCommandHandler _handler;

        public AttacherDocumentCommandHandlerTests()
        {
            _dossierMedicalRepositoryMock = new Mock<IDossierMedicalRepository>();
            _mapperMock = new Mock<IMapper>(); // Instanciez le mock
            _handler = new AttacherDocumentCommandHandler(_dossierMedicalRepositoryMock.Object, _mapperMock.Object); // Passez-le au handler
        }

        [Fact]
        public async Task Handle_ShouldThrowInvalidOperationException_WhenDossierMedicalNotFound()
        {
            // Arrange
            var dossierId = Guid.NewGuid();
            var documentDto = new CreateDocumentRequest { Nom = "doc.pdf", Url = "url", Type = "PDF" };
            var command = new AttacherDocumentCommand(dossierId, documentDto);
            _dossierMedicalRepositoryMock.Setup(r => r.GetDossierMedicalByIdAsync(dossierId))
                .ReturnsAsync((DossierMedical?)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            var ex = await act.Should().ThrowAsync<InvalidOperationException>();
            ex.Which.Message.Should().Be("Dossier médical not found.");
            _dossierMedicalRepositoryMock.Verify(r => r.AttacherDocumentAsync(It.IsAny<Guid>(), It.IsAny<Document>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldAttachDocument_WhenDossierMedicalExists()
        {
            // Arrange
            var dossierId = Guid.NewGuid();
            var dossierMedical = new DossierMedical { Id = dossierId };
            var documentDto = new CreateDocumentRequest { Nom = "doc.pdf", Url = "url", Type = "PDF" };
            var command = new AttacherDocumentCommand(dossierId, documentDto);

            _dossierMedicalRepositoryMock.Setup(r => r.GetDossierMedicalByIdAsync(dossierId))
                .ReturnsAsync(dossierMedical);
            _dossierMedicalRepositoryMock.Setup(r => r.AttacherDocumentAsync(dossierId, It.IsAny<Document>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Si le handler utilise AutoMapper pour mapper le DTO vers Document, ajoutez ce setup :
            _mapperMock.Setup(m => m.Map<Document>(documentDto))
                .Returns(new Document { Nom = documentDto.Nom, Url = documentDto.Url, Type = documentDto.Type });

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dossierMedicalRepositoryMock.Verify(r => r.AttacherDocumentAsync(dossierId, It.Is<Document>(
                d => d.Nom == documentDto.Nom &&
                     d.Url == documentDto.Url &&
                     d.Type == documentDto.Type
            )), Times.Once);
        }
    }
}
