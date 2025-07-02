using AutoMapper;
using ConsultationManagementService.Application.Commands.UpdateConsultation;
using ConsultationManagementService.Application.DTOs;
using ConsultationManagementService.Domain.Entities;
using ConsultationManagementService.Repositories;
using FluentAssertions;
using Moq;

namespace ConsultationManagementService.Tests.Commands
{
    public class UpdateConsultationCommandHandlerTests
    {
        private readonly Mock<IConsultationRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UpdateConsultationCommandHandler _handler;

        public UpdateConsultationCommandHandlerTests()
        {
            _repositoryMock = new Mock<IConsultationRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new UpdateConsultationCommandHandler(_repositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentNullException_When_ConsultationIsNull()
        {
            // Arrange
            var command = new UpdateConsultationCommand(null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentException_When_IdIsEmpty()
        {
            // Arrange
            var dto = new ConsultationDTO
            {
                Id = Guid.Empty,
                PatientId = Guid.NewGuid(),
                MedecinId = Guid.NewGuid(),
                DateConsultation = DateTime.Now,
                Diagnostic = "Test",
                Notes = "Notes"
            };
            var command = new UpdateConsultationCommand(dto);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*identifiant*");
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentException_When_PatientIdIsEmpty()
        {
            // Arrange
            var dto = new ConsultationDTO
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.Empty,
                MedecinId = Guid.NewGuid(),
                DateConsultation = DateTime.Now,
                Diagnostic = "Test",
                Notes = "Notes"
            };
            var command = new UpdateConsultationCommand(dto);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*patient*");
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentException_When_MedecinIdIsEmpty()
        {
            // Arrange
            var dto = new ConsultationDTO
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                MedecinId = Guid.Empty,
                DateConsultation = DateTime.Now,
                Diagnostic = "Test",
                Notes = "Notes"
            };
            var command = new UpdateConsultationCommand(dto);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*médecin*");
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentException_When_DateConsultationIsDefault()
        {
            // Arrange
            var dto = new ConsultationDTO
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                MedecinId = Guid.NewGuid(),
                DateConsultation = default,
                Diagnostic = "Test",
                Notes = "Notes"
            };
            var command = new UpdateConsultationCommand(dto);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*date*");
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentException_When_DiagnosticIsEmpty()
        {
            // Arrange
            var dto = new ConsultationDTO
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                MedecinId = Guid.NewGuid(),
                DateConsultation = DateTime.Now,
                Diagnostic = "",
                Notes = "Notes"
            };
            var command = new UpdateConsultationCommand(dto);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*diagnostic*");
        }

        [Fact]
        public async Task Handle_Should_CallRepository_When_DataIsValid()
        {
            // Arrange
            var dto = new ConsultationDTO
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                MedecinId = Guid.NewGuid(),
                DateConsultation = DateTime.Now,
                Diagnostic = "Test",
                Notes = "Notes",
                Documents = new List<DocumentMedicalDTO>()
            };
            var command = new UpdateConsultationCommand(dto);
            var entity = new Consultation
            {
                Id = dto.Id,
                PatientId = dto.PatientId,
                MedecinId = dto.MedecinId,
                DateConsultation = dto.DateConsultation,
                Diagnostic = dto.Diagnostic,
                Notes = dto.Notes,
                Documents = new List<DocumentMedical>()
            };

            _mapperMock.Setup(m => m.Map<Consultation>(dto)).Returns(entity);
            _repositoryMock.Setup(r => r.UpdateConsultationAsync(entity)).Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mapperMock.Verify(m => m.Map<Consultation>(dto), Times.Once);
            _repositoryMock.Verify(r => r.UpdateConsultationAsync(entity), Times.Once);
        }
    }
}
