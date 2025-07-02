using AutoMapper;
using ConsultationManagementService.Application.Commands.UploadDocumentMedical;
using ConsultationManagementService.Application.DTOs;
using ConsultationManagementService.Domain.Entities;
using ConsultationManagementService.Repositories;
using FluentAssertions;
using Moq;

namespace ConsultationManagementService.Tests.Commands
{
    public class UploadDocumentMedicalCommandHandlerTests
    {
        private readonly Mock<IConsultationRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UploadDocumentMedicalCommandHandler _handler;

        public UploadDocumentMedicalCommandHandlerTests()
        {
            _repositoryMock = new Mock<IConsultationRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new UploadDocumentMedicalCommandHandler(_repositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentNullException_When_DocumentIsNull()
        {
            // Arrange
            var command = new UploadDocumentMedicalCommand(null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentException_When_ConsultationIdIsEmpty()
        {
            // Arrange
            var dto = new DocumentMedicalDTO
            {
                Id = Guid.NewGuid(),
                ConsultationId = Guid.Empty,
                Type = "Ordonnance",
                FichierURL = "http://test.fr/file.pdf"
            };
            var command = new UploadDocumentMedicalCommand(dto);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*consultation*");
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentException_When_TypeIsEmpty()
        {
            // Arrange
            var dto = new DocumentMedicalDTO
            {
                Id = Guid.NewGuid(),
                ConsultationId = Guid.NewGuid(),
                Type = "",
                FichierURL = "http://test.fr/file.pdf"
            };
            var command = new UploadDocumentMedicalCommand(dto);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*type*");
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentException_When_FichierUrlIsEmpty()
        {
            // Arrange
            var dto = new DocumentMedicalDTO
            {
                Id = Guid.NewGuid(),
                ConsultationId = Guid.NewGuid(),
                Type = "Ordonnance",
                FichierURL = ""
            };
            var command = new UploadDocumentMedicalCommand(dto);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*URL*");
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentException_When_ConsultationDoesNotExist()
        {
            // Arrange
            var dto = new DocumentMedicalDTO
            {
                Id = Guid.NewGuid(),
                ConsultationId = Guid.NewGuid(),
                Type = "Ordonnance",
                FichierURL = "http://test.fr/file.pdf"
            };
            var command = new UploadDocumentMedicalCommand(dto);

            _repositoryMock.Setup(r => r.ExistsAsync(dto.ConsultationId)).ReturnsAsync(false);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*n'existe pas*");
        }

        [Fact]
        public async Task Handle_Should_CallRepository_When_DataIsValid()
        {
            // Arrange
            var dto = new DocumentMedicalDTO
            {
                Id = Guid.NewGuid(),
                ConsultationId = Guid.NewGuid(),
                Type = "Ordonnance",
                FichierURL = "http://test.fr/file.pdf"
            };
            var command = new UploadDocumentMedicalCommand(dto);
            var entity = new DocumentMedical
            {
                Id = dto.Id,
                ConsultationId = dto.ConsultationId,
                Type = dto.Type,
                FichierURL = dto.FichierURL,
                DateAjout = DateTime.Now
            };

            _repositoryMock.Setup(r => r.ExistsAsync(dto.ConsultationId)).ReturnsAsync(true);
            _mapperMock.Setup(m => m.Map<DocumentMedical>(dto)).Returns(entity);
            _repositoryMock.Setup(r => r.UploadDocumentMedicalAsync(entity)).Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(r => r.ExistsAsync(dto.ConsultationId), Times.Once);
            _mapperMock.Verify(m => m.Map<DocumentMedical>(dto), Times.Once);
            _repositoryMock.Verify(r => r.UploadDocumentMedicalAsync(entity), Times.Once);
        }
    }
}
