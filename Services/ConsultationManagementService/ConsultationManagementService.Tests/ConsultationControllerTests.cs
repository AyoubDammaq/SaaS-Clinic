using ConsultationManagementService.Application.Commands.CreateConsultation;
using ConsultationManagementService.Application.Commands.DeleteConsultation;
using ConsultationManagementService.Application.Commands.DeleteDocumentMedical;
using ConsultationManagementService.Application.Commands.UpdateConsultation;
using ConsultationManagementService.Application.Commands.UploadDocumentMedical;
using ConsultationManagementService.Application.DTOs;
using ConsultationManagementService.Application.Queries.CountByMedecinIds;
using ConsultationManagementService.Application.Queries.GetAllConsultations;
using ConsultationManagementService.Application.Queries.GetConsultationById;
using ConsultationManagementService.Application.Queries.GetConsultationsByDoctorId;
using ConsultationManagementService.Application.Queries.GetConsultationsByPatientId;
using ConsultationManagementService.Application.Queries.GetDocumentMedicalById;
using ConsultationManagementService.Application.Queries.GetNombreConsultations;
using ConsultationManagementService.Controllers;
using ConsultationManagementService.Domain.Entities;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ConsultationManagementService.Tests
{
    public class ConsultationControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly ConsultationController _controller;

        public ConsultationControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new ConsultationController(_mediatorMock.Object);
        }

        [Fact]
        public async Task GetConsultationByIdAsync_Should_ReturnBadRequest_When_IdIsEmpty()
        {
            var result = await _controller.GetConsultationByIdAsync(Guid.Empty);
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetConsultationByIdAsync_Should_ReturnOk_When_ConsultationExists()
        {
            var id = Guid.NewGuid();
            var consultation = new Consultation { Id = id };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetConsultationByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(consultation);

            var result = await _controller.GetConsultationByIdAsync(id);

            result.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result.Result).Value.Should().Be(consultation);
        }

        [Fact]
        public async Task GetAllConsultationsAsync_Should_ReturnOk()
        {
            var consultations = new List<Consultation> { new Consultation(), new Consultation() };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllConsultationsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(consultations);

            var result = await _controller.GetAllConsultationsAsync(1, 10);

            result.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result.Result).Value.Should().BeEquivalentTo(consultations);
        }

        [Fact]
        public async Task CreateConsultationAsync_Should_ReturnBadRequest_When_DtoIsNull()
        {
            var result = await _controller.CreateConsultationAsync(null);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid consultation data", badRequest.Value);
        }

        [Fact]
        public async Task CreateConsultationAsync_Should_ReturnOk_When_Valid()
        {
            var dto = new ConsultationDTO
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                MedecinId = Guid.NewGuid(),
                DateConsultation = DateTime.Now,
                Diagnostic = "Test",
                Notes = "Notes"
            };
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateConsultationCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Unit.Value));

            var result = await _controller.CreateConsultationAsync(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Consultation créée avec succès", okResult.Value);
        }

        [Fact]
        public async Task UpdateConsultationAsync_Should_ReturnBadRequest_When_DtoIsNull()
        {
            var result = await _controller.UpdateConsultationAsync(null);
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task UpdateConsultationAsync_Should_ReturnNoContent_When_Valid()
        {
            var dto = new ConsultationDTO
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                MedecinId = Guid.NewGuid(),
                DateConsultation = DateTime.Now,
                Diagnostic = "Test",
                Notes = "Notes"
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateConsultationCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Unit.Value));

            var result = await _controller.UpdateConsultationAsync(dto);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteConsultationAsync_Should_ReturnBadRequest_When_IdIsEmpty()
        {
            var result = await _controller.DeleteConsultationAsync(Guid.Empty);
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task DeleteConsultationAsync_Should_ReturnNoContent_When_Deleted()
        {
            var id = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteConsultationCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _controller.DeleteConsultationAsync(id);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task GetConsultationsByPatientIdAsync_Should_ReturnBadRequest_When_IdIsEmpty()
        {
            var result = await _controller.GetConsultationsByPatientIdAsync(Guid.Empty);
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetConsultationsByPatientIdAsync_Should_ReturnOk_When_Valid()
        {
            var patientId = Guid.NewGuid();
            var consultations = new List<Consultation> { new Consultation(), new Consultation() };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetConsultationsByPatientIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(consultations);

            var result = await _controller.GetConsultationsByPatientIdAsync(patientId);

            result.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result.Result).Value.Should().BeEquivalentTo(consultations);
        }

        [Fact]
        public async Task GetConsultationsByDoctorIdAsync_Should_ReturnBadRequest_When_IdIsEmpty()
        {
            var result = await _controller.GetConsultationsByDoctorIdAsync(Guid.Empty);
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetConsultationsByDoctorIdAsync_Should_ReturnOk_When_Valid()
        {
            var doctorId = Guid.NewGuid();
            var consultations = new List<Consultation> { new Consultation(), new Consultation() };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetConsultationsByDoctorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(consultations);

            var result = await _controller.GetConsultationsByDoctorIdAsync(doctorId);

            result.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result.Result).Value.Should().BeEquivalentTo(consultations);
        }

        [Fact]
        public async Task GetDocumentMedicalByIdAsync_Should_ReturnBadRequest_When_IdIsEmpty()
        {
            var result = await _controller.GetDocumentMedicalByIdAsync(Guid.Empty);
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetDocumentMedicalByIdAsync_Should_ReturnOk_When_Valid()
        {
            var id = Guid.NewGuid();
            var document = new DocumentMedical { Id = id };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetDocumentMedicalByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(document);

            var result = await _controller.GetDocumentMedicalByIdAsync(id);

            result.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result.Result).Value.Should().Be(document);
        }

        [Fact]
        public async Task UploadDocumentMedicalAsync_Should_ReturnBadRequest_When_DtoIsNull()
        {
            var result = await _controller.UploadDocumentMedicalAsync(null);
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task UploadDocumentMedicalAsync_Should_ReturnOk_When_Valid()
        {
            var dto = new DocumentMedicalDTO
            {
                Id = Guid.NewGuid(),
                ConsultationId = Guid.NewGuid(),
                Type = "Ordonnance",
                FichierURL = "http://test.fr/file.pdf"
            };
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<UploadDocumentMedicalCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Unit.Value));

            var result = await _controller.UploadDocumentMedicalAsync(dto);

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteDocumentMedicalAsync_Should_ReturnBadRequest_When_IdIsEmpty()
        {
            var result = await _controller.DeleteDocumentMedicalAsync(Guid.Empty);
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task DeleteDocumentMedicalAsync_Should_ReturnNoContent_When_Deleted()
        {
            var id = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteDocumentMedicalCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _controller.DeleteDocumentMedicalAsync(id);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task GetNombreConsultations_Should_ReturnOk()
        {
            var start = new DateTime(2024, 1, 1);
            var end = new DateTime(2024, 12, 31);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetNombreConsultationsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(10);

            var result = await _controller.GetNombreConsultations(start, end);

            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().Be(10);
        }

        [Fact]
        public async Task GetCountByMedecinIds_Should_ReturnBadRequest_When_ListIsNullOrEmpty()
        {
            var result1 = await _controller.GetCountByMedecinIds(null);
            var result2 = await _controller.GetCountByMedecinIds(new List<Guid>());

            result1.Should().BeOfType<BadRequestObjectResult>();
            result2.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetCountByMedecinIds_Should_ReturnOk_When_Valid()
        {
            var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            _mediatorMock.Setup(m => m.Send(It.IsAny<CountByMedecinIdsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(5);

            var result = await _controller.GetCountByMedecinIds(ids);

            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().Be(5);
        }
    }
}
