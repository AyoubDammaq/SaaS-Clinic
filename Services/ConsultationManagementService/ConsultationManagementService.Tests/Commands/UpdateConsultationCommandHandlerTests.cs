using AutoMapper;
using ConsultationManagementService.Application.Commands.UpdateConsultation;
using ConsultationManagementService.Application.DTOs;
using ConsultationManagementService.Domain.Entities;
using ConsultationManagementService.Domain.Enums;
using ConsultationManagementService.Repositories;
using ConsultationManagementService.Application.Interfaces;
using FluentAssertions;
using Moq;

namespace ConsultationManagementService.Tests.Commands
{
    public class UpdateConsultationCommandHandlerTests
    {
        private readonly Mock<IConsultationRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IDoctorHttpClient> _doctorClientMock;
        private readonly UpdateConsultationCommandHandler _handler;

        public UpdateConsultationCommandHandlerTests()
        {
            _repositoryMock = new Mock<IConsultationRepository>();
            _mapperMock = new Mock<IMapper>();
            _doctorClientMock = new Mock<IDoctorHttpClient>();
            _handler = new UpdateConsultationCommandHandler(_repositoryMock.Object, _mapperMock.Object, _doctorClientMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentNullException_When_ConsultationIsNull()
        {
            var command = new UpdateConsultationCommand(null);
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentException_When_IdIsEmpty()
        {
            var dto = new ConsultationDTO
            {
                Id = Guid.Empty,
                PatientId = Guid.NewGuid(),
                MedecinId = Guid.NewGuid(),
                DateConsultation = DateTime.Now,
                Diagnostic = "Test",
                Notes = "Notes",
                Type = TypeConsultation.ConsultationGenerale
            };
            var command = new UpdateConsultationCommand(dto);
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*identifiant*");
        }

        [Fact]
        public async Task Handle_Should_ThrowInvalidOperationException_When_DoctorNotFound()
        {
            var dto = new ConsultationDTO
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                MedecinId = Guid.NewGuid(),
                DateConsultation = DateTime.Now,
                Diagnostic = "Test",
                Notes = "Notes",
                Type = TypeConsultation.ConsultationGenerale
            };
            var command = new UpdateConsultationCommand(dto);
            _doctorClientMock.Setup(x => x.GetDoctorById(dto.MedecinId)).ReturnsAsync((GetMedecinDto)null);

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage($"Le médecin avec l'id {dto.MedecinId} n'est pas assigné à une clinique.");
        }

        [Fact]
        public async Task Handle_Should_ThrowInvalidOperationException_When_DoctorHasNoClinic()
        {
            var dto = new ConsultationDTO
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                MedecinId = Guid.NewGuid(),
                DateConsultation = DateTime.Now,
                Diagnostic = "Test",
                Notes = "Notes",
                Type = TypeConsultation.ConsultationGenerale
            };
            var command = new UpdateConsultationCommand(dto);
            var medecin = new GetMedecinDto { Id = dto.MedecinId, CliniqueId = null };
            _doctorClientMock.Setup(x => x.GetDoctorById(dto.MedecinId)).ReturnsAsync(medecin);

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage($"Le médecin avec l'id {dto.MedecinId} n'est pas assigné à une clinique.");
        }

        [Fact]
        public async Task Handle_Should_ThrowInvalidOperationException_When_DoctorClinicIdIsEmpty()
        {
            var dto = new ConsultationDTO
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                MedecinId = Guid.NewGuid(),
                DateConsultation = DateTime.Now,
                Diagnostic = "Test",
                Notes = "Notes",
                Type = TypeConsultation.ConsultationGenerale
            };
            var command = new UpdateConsultationCommand(dto);
            var medecin = new GetMedecinDto { Id = dto.MedecinId, CliniqueId = Guid.Empty };
            _doctorClientMock.Setup(x => x.GetDoctorById(dto.MedecinId)).ReturnsAsync(medecin);

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage($"Le médecin avec l'id {dto.MedecinId} n'est pas assigné à une clinique.");
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentException_When_PatientIdIsEmpty()
        {
            var dto = new ConsultationDTO
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.Empty,
                MedecinId = Guid.NewGuid(),
                DateConsultation = DateTime.Now,
                Diagnostic = "Test",
                Notes = "Notes",
                Type = TypeConsultation.ConsultationGenerale
            };
            var command = new UpdateConsultationCommand(dto);
            var medecin = new GetMedecinDto { Id = dto.MedecinId, CliniqueId = Guid.NewGuid() };
            _doctorClientMock.Setup(x => x.GetDoctorById(dto.MedecinId)).ReturnsAsync(medecin);

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*patient*");
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentException_When_MedecinIdIsEmpty()
        {
            var dto = new ConsultationDTO
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                MedecinId = Guid.Empty,
                DateConsultation = DateTime.Now,
                Diagnostic = "Test",
                Notes = "Notes",
                Type = TypeConsultation.ConsultationGenerale
            };
            var command = new UpdateConsultationCommand(dto);
            var medecin = new GetMedecinDto { Id = Guid.NewGuid(), CliniqueId = Guid.NewGuid() };
            _doctorClientMock.Setup(x => x.GetDoctorById(dto.MedecinId)).ReturnsAsync(medecin);

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*médecin*");
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentException_When_ClinicIdIsEmpty()
        {
            var dto = new ConsultationDTO
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                MedecinId = Guid.NewGuid(),
                ClinicId = Guid.Empty,
                DateConsultation = DateTime.Now,
                Diagnostic = "Test",
                Notes = "Notes",
                Type = TypeConsultation.ConsultationGenerale
            };
            var command = new UpdateConsultationCommand(dto);
            var medecin = new GetMedecinDto { Id = dto.MedecinId, CliniqueId = Guid.NewGuid() };
            _doctorClientMock.Setup(x => x.GetDoctorById(dto.MedecinId)).ReturnsAsync(medecin);

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*clinique*");
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentException_When_DateConsultationIsDefault()
        {
            var dto = new ConsultationDTO
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                MedecinId = Guid.NewGuid(),
                ClinicId = Guid.NewGuid(),
                DateConsultation = default,
                Diagnostic = "Test",
                Notes = "Notes",
                Type = TypeConsultation.ConsultationGenerale
            };
            var command = new UpdateConsultationCommand(dto);
            var medecin = new GetMedecinDto { Id = dto.MedecinId, CliniqueId = Guid.NewGuid() };
            _doctorClientMock.Setup(x => x.GetDoctorById(dto.MedecinId)).ReturnsAsync(medecin);

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*date*");
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentException_When_DiagnosticIsEmpty()
        {
            var dto = new ConsultationDTO
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                MedecinId = Guid.NewGuid(),
                ClinicId = Guid.NewGuid(),
                DateConsultation = DateTime.Now,
                Diagnostic = "",
                Notes = "Notes",
                Type = TypeConsultation.ConsultationGenerale
            };
            var command = new UpdateConsultationCommand(dto);
            var medecin = new GetMedecinDto { Id = dto.MedecinId, CliniqueId = Guid.NewGuid() };
            _doctorClientMock.Setup(x => x.GetDoctorById(dto.MedecinId)).ReturnsAsync(medecin);

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*diagnostic*");
        }

        [Fact]
        public async Task Handle_Should_CallRepository_When_DataIsValid()
        {
            var dto = new ConsultationDTO
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                MedecinId = Guid.NewGuid(),
                ClinicId = Guid.NewGuid(),
                DateConsultation = DateTime.Now,
                Diagnostic = "Test",
                Notes = "Notes",
                Type = TypeConsultation.ConsultationGenerale,
                Documents = new List<DocumentMedicalDTO>()
            };
            var command = new UpdateConsultationCommand(dto);
            var medecin = new GetMedecinDto { Id = dto.MedecinId, CliniqueId = dto.ClinicId };
            var entity = new Consultation
            {
                Id = dto.Id,
                PatientId = dto.PatientId,
                MedecinId = dto.MedecinId,
                ClinicId = dto.ClinicId,
                DateConsultation = dto.DateConsultation,
                Diagnostic = dto.Diagnostic,
                Notes = dto.Notes,
                Type = dto.Type,
                Documents = new List<DocumentMedical>()
            };

            _doctorClientMock.Setup(x => x.GetDoctorById(dto.MedecinId)).ReturnsAsync(medecin);
            _mapperMock.Setup(m => m.Map<Consultation>(dto)).Returns(entity);
            _repositoryMock.Setup(r => r.UpdateConsultationAsync(entity)).Returns(Task.CompletedTask);

            await _handler.Handle(command, CancellationToken.None);

            _mapperMock.Verify(m => m.Map<Consultation>(dto), Times.Once);
            _repositoryMock.Verify(r => r.UpdateConsultationAsync(entity), Times.Once);
        }
    }
}

