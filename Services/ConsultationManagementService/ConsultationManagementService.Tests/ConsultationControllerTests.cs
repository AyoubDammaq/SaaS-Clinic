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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Xunit;

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
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Invalid consultation ID", badRequest.Value);
        }

        [Fact]
        public async Task GetConsultationByIdAsync_Should_ReturnOk_When_ConsultationExists()
        {
            var id = Guid.NewGuid();
            var consultation = new Consultation { Id = id };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetConsultationByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(consultation);

            var result = await _controller.GetConsultationByIdAsync(id);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(consultation, okResult.Value);
        }

        [Fact]
        public async Task GetAllConsultationsAsync_Should_ReturnOk()
        {
            var consultations = new List<Consultation> { new Consultation(), new Consultation() };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllConsultationsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(consultations);

            var result = await _controller.GetAllConsultationsAsync(1, 10);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(consultations, okResult.Value);
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
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid consultation data", badRequest.Value);
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

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteConsultationAsync_Should_ReturnBadRequest_When_IdIsEmpty()
        {
            var result = await _controller.DeleteConsultationAsync(Guid.Empty);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid consultation ID", badRequest.Value);
        }

        [Fact]
        public async Task DeleteConsultationAsync_Should_ReturnNoContent_When_Deleted()
        {
            var id = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteConsultationCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _controller.DeleteConsultationAsync(id);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteConsultationAsync_Should_ReturnNotFound_When_NotDeleted()
        {
            var id = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteConsultationCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _controller.DeleteConsultationAsync(id);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Consultation not found", notFound.Value);
        }

        [Fact]
        public async Task GetConsultationsByPatientIdAsync_Should_ReturnBadRequest_When_IdIsEmpty()
        {
            var result = await _controller.GetConsultationsByPatientIdAsync(Guid.Empty);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Invalid patient ID", badRequest.Value);
        }

        [Fact]
        public async Task GetConsultationsByPatientIdAsync_Should_ReturnOk_When_Valid()
        {
            var patientId = Guid.NewGuid();
            var consultations = new List<Consultation> { new Consultation(), new Consultation() };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetConsultationsByPatientIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(consultations);

            var result = await _controller.GetConsultationsByPatientIdAsync(patientId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(consultations, okResult.Value);
        }

        [Fact]
        public async Task GetConsultationsByDoctorIdAsync_Should_ReturnBadRequest_When_IdIsEmpty()
        {
            var result = await _controller.GetConsultationsByDoctorIdAsync(Guid.Empty);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Invalid doctor ID", badRequest.Value);
        }

        [Fact]
        public async Task GetConsultationsByDoctorIdAsync_Should_ReturnOk_When_Valid()
        {
            var doctorId = Guid.NewGuid();
            var consultations = new List<Consultation> { new Consultation(), new Consultation() };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetConsultationsByDoctorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(consultations);

            var result = await _controller.GetConsultationsByDoctorIdAsync(doctorId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(consultations, okResult.Value);
        }

        [Fact]
        public async Task GetDocumentMedicalByIdAsync_Should_ReturnBadRequest_When_IdIsEmpty()
        {
            var result = await _controller.GetDocumentMedicalByIdAsync(Guid.Empty);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Invalid document ID", badRequest.Value);
        }

        [Fact]
        public async Task GetDocumentMedicalByIdAsync_Should_ReturnOk_When_Valid()
        {
            var id = Guid.NewGuid();
            var document = new DocumentMedical { Id = id };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetDocumentMedicalByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(document);

            var result = await _controller.GetDocumentMedicalByIdAsync(id);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(document, okResult.Value);
        }

        [Fact]
        public async Task UploadDocumentMedicalAsync_Should_ReturnBadRequest_When_FileIsNull()
        {
            var result = await _controller.UploadDocumentMedicalAsync(Guid.NewGuid(), "Ordonnance", null);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Aucun fichier reçu.", badRequest.Value);
        }

        [Fact]
        public async Task DeleteDocumentMedicalAsync_Should_ReturnBadRequest_When_IdIsEmpty()
        {
            var result = await _controller.DeleteDocumentMedicalAsync(Guid.Empty);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid document ID", badRequest.Value);
        }

        [Fact]
        public async Task DeleteDocumentMedicalAsync_Should_ReturnNoContent_When_Deleted()
        {
            var id = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteDocumentMedicalCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _controller.DeleteDocumentMedicalAsync(id);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteDocumentMedicalAsync_Should_ReturnNotFound_When_NotDeleted()
        {
            var id = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteDocumentMedicalCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _controller.DeleteDocumentMedicalAsync(id);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Document médical not found", notFound.Value);
        }

        [Fact]
        public async Task GetNombreConsultations_Should_ReturnOk()
        {
            var start = new DateTime(2024, 1, 1);
            var end = new DateTime(2024, 12, 31);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetNombreConsultationsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(10);

            var result = await _controller.GetNombreConsultations(start, end);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(10, okResult.Value);
        }

        [Fact]
        public async Task GetCountByMedecinIds_Should_ReturnBadRequest_When_ListIsNullOrEmpty()
        {
            var result1 = await _controller.GetCountByMedecinIds(null);
            var badRequest1 = Assert.IsType<BadRequestObjectResult>(result1);
            Assert.Equal("La liste des identifiants de médecins ne peut pas être vide.", badRequest1.Value);

            var result2 = await _controller.GetCountByMedecinIds(new List<Guid>());
            var badRequest2 = Assert.IsType<BadRequestObjectResult>(result2);
            Assert.Equal("La liste des identifiants de médecins ne peut pas être vide.", badRequest2.Value);
        }

        [Fact]
        public async Task GetCountByMedecinIds_Should_ReturnOk_When_Valid()
        {
            var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            _mediatorMock.Setup(m => m.Send(It.IsAny<CountByMedecinIdsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(5);

            var result = await _controller.GetCountByMedecinIds(ids);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(5, okResult.Value);
        }
    }
}

