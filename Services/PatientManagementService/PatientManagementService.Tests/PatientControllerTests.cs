using PatientManagementService.API.Controllers;
using MediatR;
using Moq;
using PatientManagementService.Domain.Entities;
using PatientManagementService.Application.PatientService.Queries.GetAllPatients;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using PatientManagementService.Application.PatientService.Queries.GetPatientById;
using PatientManagementService.Application.DTOs;
using PatientManagementService.Application.PatientService.Commands.AddPatient;
using PatientManagementService.Application.PatientService.Commands.UpdatePatient;
using PatientManagementService.Application.PatientService.Commands.DeletePatient;
using PatientManagementService.Application.PatientService.Queries.GetPatientsByName;
using PatientManagementService.Application.PatientService.Queries.GetStatistiques;


namespace PatientManagementService.Tests
{
    public class PatientControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly PatientsController _controller;

        public PatientControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new PatientsController(_mediatorMock.Object);
        }

        [Fact]
        public async Task GetAllPatients_ReturnsOk_WhenSuccess()
        {
            var patients = new List<Patient> { new Patient { Id = Guid.NewGuid(), Nom = "Test", Prenom = "User", DateNaissance = DateTime.Now, Sexe = "M" } };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllPatientsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(patients);

            var result = await _controller.GetAllPatients();

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(patients);
        }

        [Fact]
        public async Task GetAllPatients_Returns500_OnException()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllPatientsQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Erreur"));

            var result = await _controller.GetAllPatients();

            var status = result.Result as ObjectResult;
            status.Should().NotBeNull();
            status!.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task GetPatientById_ReturnsOk_WhenFound()
        {
            var id = Guid.NewGuid();
            var patient = new Patient { Id = id, Nom = "Test", Prenom = "User", DateNaissance = DateTime.Now, Sexe = "F" };
            _mediatorMock.Setup(m => m.Send(It.Is<GetPatientByIdQuery>(q => q.Equals(new GetPatientByIdQuery(id))), It.IsAny<CancellationToken>()))
                .ReturnsAsync(patient);

            var result = await _controller.GetPatientById(id);

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(patient);
        }

        [Fact]
        public async Task GetPatientById_ReturnsNotFound_WhenNull()
        {
            var id = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPatientByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Patient?)null);

            var result = await _controller.GetPatientById(id);

            var notFound = result.Result as NotFoundObjectResult;
            notFound.Should().NotBeNull();
        }

        [Fact]
        public async Task GetPatientById_Returns500_OnException()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPatientByIdQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Erreur"));

            var result = await _controller.GetPatientById(Guid.NewGuid());

            var status = result.Result as ObjectResult;
            status.Should().NotBeNull();
            status!.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task AddPatient_ReturnsBadRequest_WhenModelStateInvalid()
        {
            _controller.ModelState.AddModelError("Nom", "Requis");
            var dto = new CreatePatientDTO(); // Utilisez CreatePatientDTO

            var result = await _controller.AddPatient(dto);

            var badRequest = result as BadRequestObjectResult;
            badRequest.Should().NotBeNull();
        }

        [Fact]
        public async Task AddPatient_ReturnsCreatedAtAction_WhenSuccess()
        {
            var dto = new CreatePatientDTO
            {
                Nom = "Test",
                Prenom = "User",
                DateNaissance = DateTime.Now,
                Sexe = "M",
                Adresse = "1 rue",
                Telephone = "0102030404",
                Email = "test@email.com"
            };

            var createdPatient = new PatientDTO
            {
                Id = Guid.NewGuid(),
                Nom = dto.Nom,
                Prenom = dto.Prenom,
                DateNaissance = dto.DateNaissance,
                Sexe = dto.Sexe,
                Adresse = dto.Adresse,
                Telephone = dto.Telephone,
                Email = dto.Email
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<AddPatientCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdPatient);

            var result = await _controller.AddPatient(dto);

            var created = result as CreatedAtActionResult;
            created.Should().NotBeNull();
            created!.Value.Should().BeEquivalentTo(createdPatient);
        }


        [Fact]
        public async Task AddPatient_Returns500_OnException()
        {
            var dto = new CreatePatientDTO { Nom = "Test", Prenom = "User", DateNaissance = DateTime.Now, Sexe = "M", Adresse = "1 rue", Telephone = "0102030404", Email = "test@email.com" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<AddPatientCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Erreur"));

            var result = await _controller.AddPatient(dto);

            var status = result as ObjectResult;
            status.Should().NotBeNull();
            status!.StatusCode.Should().Be(500);
        }


        [Fact]
        public async Task UpdatePatient_ReturnsBadRequest_WhenIdMismatch()
        {
            var dto = new PatientDTO { Id = Guid.NewGuid() };
            var result = await _controller.UpdatePatient(Guid.NewGuid(), dto);

            var badRequest = result as BadRequestObjectResult;
            badRequest.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdatePatient_ReturnsNotFound_WhenPatientNotFound()
        {
            var id = Guid.NewGuid();
            var dto = new PatientDTO { Id = id };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPatientByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Patient?)null);

            var result = await _controller.UpdatePatient(id, dto);

            var notFound = result as NotFoundObjectResult;
            notFound.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdatePatient_ReturnsNoContent_WhenSuccess()
        {
            var id = Guid.NewGuid();
            var dto = new PatientDTO { Id = id };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPatientByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Patient { Id = id });
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<UpdatePatientCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true); // Le handler retourne un bool

            var result = await _controller.UpdatePatient(id, dto);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UpdatePatient_Returns500_OnException()
        {
            var id = Guid.NewGuid();
            var dto = new PatientDTO { Id = id };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPatientByIdQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Erreur"));

            var result = await _controller.UpdatePatient(id, dto);

            var status = result as ObjectResult;
            status.Should().NotBeNull();
            status!.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task DeletePatient_ReturnsNotFound_WhenPatientNotFound()
        {
            var id = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPatientByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Patient?)null);

            var result = await _controller.DeletePatient(id);

            var notFound = result as NotFoundObjectResult;
            notFound.Should().NotBeNull();
        }

        [Fact]
        public async Task DeletePatient_ReturnsNoContent_WhenSuccess()
        {
            var id = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPatientByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Patient { Id = id });
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<DeletePatientCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true); // Le handler retourne un bool

            var result = await _controller.DeletePatient(id);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeletePatient_Returns500_OnException()
        {
            var id = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPatientByIdQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Erreur"));

            var result = await _controller.DeletePatient(id);

            var status = result as ObjectResult;
            status.Should().NotBeNull();
            status!.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task SearchPatients_ReturnsOk_WhenSuccess()
        {
            var patients = new List<Patient> { new Patient { Id = Guid.NewGuid(), Nom = "Test", Prenom = "User", DateNaissance = DateTime.Now, Sexe = "M" } };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPatientsByNameQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(patients);

            var result = await _controller.SearchPatients("Test", "User");

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(patients);
        }

        [Fact]
        public async Task SearchPatients_Returns500_OnException()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPatientsByNameQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Erreur"));

            var result = await _controller.SearchPatients("Test", "User");

            var status = result.Result as ObjectResult;
            status.Should().NotBeNull();
            status!.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task GetStatistiques_ReturnsBadRequest_WhenDateDebutAfterDateFin()
        {
            var dateDebut = DateTime.Now;
            var dateFin = dateDebut.AddDays(-1);

            var result = await _controller.GetStatistiques(dateDebut, dateFin);

            var badRequest = result.Result as BadRequestObjectResult;
            badRequest.Should().NotBeNull();
        }

        [Fact]
        public async Task GetStatistiques_ReturnsOk_WhenSuccess()
        {
            var dateDebut = DateTime.Now;
            var dateFin = dateDebut.AddDays(1);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetStatistiquesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(5);

            var result = await _controller.GetStatistiques(dateDebut, dateFin);

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be(5);
        }
    }
}
