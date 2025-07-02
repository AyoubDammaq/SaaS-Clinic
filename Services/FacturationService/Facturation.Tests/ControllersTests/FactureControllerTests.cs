using Facturation.API.Controllers;
using Facturation.Application.DTOs;
using Facturation.Application.FactureService.Commands.ExportToPdf;
using Facturation.Application.FactureService.Queries.GetAllFactures;
using Facturation.Application.FactureService.Queries.GetFactureById;
using Facturation.Application.FactureService.Queries.GetNombreDeFactureByStatus;
using Facturation.Application.FactureService.Queries.GetNombreDeFactureParClinique;
using Facturation.Application.FactureService.Queries.GetNombreDeFacturesByStatusDansUneClinique;
using Facturation.Application.FactureService.Queries.GetNombreDeFacturesByStatusParClinique;
using Facturation.Application.FactureService.Queries.GetStatistiquesFacturesParPeriode;
using Facturation.Domain.Enums;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Facturation.Tests.ControllersTests
{
    public class FactureControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly FactureController _controller;

        public FactureControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new FactureController(_mediatorMock.Object);
        }

        [Fact]
        public async Task GetFactureById_Should_ReturnOk_When_FactureExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var response = new GetFacturesResponse { PatientId = Guid.NewGuid() };
            _mediatorMock.Setup(m => m.Send(It.Is<GetFactureByIdQuery>(q => q.id == id), default))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetFactureById(id);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be(response);
        }

        [Fact]
        public async Task GetFactureById_Should_Return500_When_Exception()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetFactureByIdQuery>(), default))
                .ThrowsAsync(new Exception("fail"));

            // Act
            var result = await _controller.GetFactureById(id);

            // Assert
            var status = result as ObjectResult;
            status.Should().NotBeNull();
            status!.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task AddFacture_Should_Return201_When_Success()
        {
            // Arrange
            var request = new CreateFactureRequest
            {
                PatientId = Guid.NewGuid(),
                ConsultationId = Guid.NewGuid(),
                ClinicId = Guid.NewGuid(),
                MontantTotal = 100m
            };

            // Act
            var result = await _controller.AddFacture(request);

            // Assert
            var status = result as ObjectResult;
            status.Should().NotBeNull();
            status!.StatusCode.Should().Be(201);
        }

        [Fact]
        public async Task AddFacture_Should_ReturnBadRequest_When_Null()
        {
            // Act
            var result = await _controller.AddFacture(default!);

            // Assert
            var bad = result as BadRequestObjectResult;
            bad.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateFacture_Should_ReturnNoContent_When_Success()
        {
            // Arrange
            var request = new UpdateFactureRequest
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                ConsultationId = Guid.NewGuid(),
                ClinicId = Guid.NewGuid(),
                MontantTotal = 100m
            };

            // Act
            var result = await _controller.UpdateFacture(request);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteFacture_Should_ReturnNoContent_When_FactureExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var response = new GetFacturesResponse { PatientId = Guid.NewGuid() };
            _mediatorMock.Setup(m => m.Send(It.Is<GetFactureByIdQuery>(q => q.id == id), default))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.DeleteFacture(id);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteFacture_Should_ReturnNotFound_When_FactureNotExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.Is<GetFactureByIdQuery>(q => q.id == id), default))
                .ReturnsAsync((GetFacturesResponse?)null);

            // Act
            var result = await _controller.DeleteFacture(id);

            // Assert
            var notFound = result as NotFoundObjectResult;
            notFound.Should().NotBeNull();
        }

        [Fact]
        public async Task GetAllFactures_Should_ReturnOk()
        {
            // Arrange
            var list = new List<GetFacturesResponse>();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllFacturesQuery>(), default))
                .ReturnsAsync(list);

            // Act
            var result = await _controller.GetAllFactures();

            // Assert
            var ok = result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.Value.Should().Be(list);
        }

        [Fact]
        public async Task GetAllFacturesByRangeOfDate_Should_ReturnBadRequest_When_StartDateAfterEndDate()
        {
            // Arrange
            var start = DateTime.Now;
            var end = start.AddDays(-1);

            // Act
            var result = await _controller.GetAllFacturesByRangeOfDate(start, end);

            // Assert
            var bad = result as BadRequestObjectResult;
            bad.Should().NotBeNull();
        }

        [Fact]
        public async Task ExportFacture_Should_ReturnFile_When_FactureExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var facture = new GetFacturesResponse
            {
                PatientId = Guid.NewGuid(),
                ConsultationId = Guid.NewGuid(),
                ClinicId = Guid.NewGuid(),
                DateEmission = DateTime.UtcNow,
                MontantTotal = 100m,
                Status = FactureStatus.PAYEE
            };
            _mediatorMock.Setup(m => m.Send(It.Is<GetFactureByIdQuery>(q => q.id == id), default))
                .ReturnsAsync(facture);
            _mediatorMock.Setup(m => m.Send(It.IsAny<ExportToPdfCommand>(), default))
                .ReturnsAsync(new byte[] { 1, 2, 3, 4 });

            // Act
            var result = await _controller.ExportFacture(id);

            // Assert
            result.Should().BeOfType<FileContentResult>();
        }

        [Fact]
        public async Task ExportFacture_Should_ReturnNotFound_When_FactureNotExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.Is<GetFactureByIdQuery>(q => q.id == id), default))
                .ReturnsAsync((GetFacturesResponse?)null);

            // Act
            var result = await _controller.ExportFacture(id);

            // Assert
            var notFound = result as NotFoundObjectResult;
            notFound.Should().NotBeNull();
        }

        [Fact]
        public async Task GetStatistiques_Should_ReturnOk()
        {
            // Arrange
            var debut = new DateTime(2024, 1, 1);
            var fin = new DateTime(2024, 1, 31);
            var stats = new StatistiquesFacturesDto
            {
                NombreTotal = 5,
                NombrePayees = 2,
                NombreImpayees = 2,
                NombrePartiellementPayees = 1,
                MontantTotal = 1000,
                MontantTotalPaye = 700,
                NombreParClinique = new Dictionary<Guid, int>()
            };
            _mediatorMock.Setup(m => m.Send(It.Is<GetStatistiquesFacturesParPeriodeQuery>(q => q.DateDebut == debut && q.DateFin == fin), default))
                .ReturnsAsync(stats);

            // Act
            var result = await _controller.GetStatistiques(debut, fin);

            // Assert
            var ok = result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.Value.Should().Be(stats);
        }

        [Fact]
        public async Task GetNombreDeFactureByStatus_Should_ReturnOk()
        {
            // Arrange
            var stats = new List<FactureStatsDTO>();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetNombreDeFactureByStatusQuery>(), default))
                .ReturnsAsync((IEnumerable<FactureStatsDTO>)stats);

            // Act
            var result = await _controller.GetNombreDeFactureByStatus();

            // Assert
            var ok = result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.Value.Should().Be(stats);
        }

        [Fact]
        public async Task GetNombreDeFactureParClinique_Should_ReturnOk()
        {
            // Arrange
            var stats = new List<FactureStatsDTO>();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetNombreDeFactureParCliniqueQuery>(), default))
                .ReturnsAsync((IEnumerable<FactureStatsDTO>)stats);

            // Act
            var result = await _controller.GetNombreDeFactureParClinique();

            // Assert
            var ok = result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.Value.Should().Be(stats);
        }

        [Fact]
        public async Task GetNombreDeFacturesByStatusParClinique_Should_ReturnOk()
        {
            // Arrange
            var stats = new List<FactureStatsDTO>();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetNombreDeFacturesByStatusParCliniqueQuery>(), default))
                .ReturnsAsync((IEnumerable<FactureStatsDTO>)stats);

            // Act
            var result = await _controller.GetNombreDeFacturesByStatusParClinique();

            // Assert
            var ok = result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.Value.Should().Be(stats);
        }

        [Fact]
        public async Task GetNombreDeFacturesByStatusDansUneClinique_Should_ReturnOk()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var stats = new List<FactureStatsDTO>();
            _mediatorMock.Setup(m => m.Send(It.Is<GetNombreDeFacturesByStatusDansUneCliniqueQuery>(q => q.cliniqueId == clinicId), default))
                .ReturnsAsync((IEnumerable<FactureStatsDTO>)stats);

            // Act
            var result = await _controller.GetNombreDeFacturesByStatusDansUneClinique(clinicId);

            // Assert
            var ok = result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.Value.Should().Be(stats);
        }
    }
}
