using Clinic.API.Controllers;
using Clinic.Application.Commands.AjouterClinique;
using Clinic.Application.Commands.ModifierClinique;
using Clinic.Application.Commands.SupprimerClinique;
using Clinic.Application.DTOs;
using Clinic.Application.Queries.GetNombreCliniques;
using Clinic.Application.Queries.GetNombreNouvellesCliniquesDuMois;
using Clinic.Application.Queries.GetNombreNouvellesCliniquesParMois;
using Clinic.Application.Queries.GetStatistiquesDesCliniques;
using Clinic.Application.Queries.ListerClinique;
using Clinic.Application.Queries.ObtenirCliniqueParId;
using Clinic.Application.Queries.RechercherCliniqueParAdresse;
using Clinic.Application.Queries.RechercherCliniqueParNom;
using Clinic.Application.Queries.RechercherCliniqueParStatut;
using Clinic.Application.Queries.RechercherCliniqueParType;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Clinic.Tests
{
    public class CliniqueControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CliniqueController _controller;

        public CliniqueControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new CliniqueController(_mediatorMock.Object);
        }

        [Fact]
        public async Task AjouterClinique_ReturnsCreatedAtAction_OnSuccess()
        {
            var dto = new CliniqueDto { Nom = "Test", Adresse = "A", NumeroTelephone = "1", Email = "a@a.fr", TypeClinique = TypeClinique.Publique, Statut = StatutClinique.Active };
            var clinique = new Clinique { Id = Guid.NewGuid(), Nom = "Test" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<AjouterCliniqueCommand>(), default)).ReturnsAsync(clinique);

            var result = await _controller.AjouterClinique(dto);

            var created = result.Result as CreatedAtActionResult;
            created.Should().NotBeNull();
            created!.Value.Should().Be(clinique);
            created.ActionName.Should().Be(nameof(_controller.ObtenirClinique));
        }

        [Fact]
        public async Task AjouterClinique_Returns500_OnException()
        {
            var dto = new CliniqueDto { Nom = "Test", Adresse = "A", NumeroTelephone = "1", Email = "a@a.fr", TypeClinique = TypeClinique.Publique, Statut = StatutClinique.Active };
            _mediatorMock.Setup(m => m.Send(It.IsAny<AjouterCliniqueCommand>(), default)).ThrowsAsync(new Exception("fail"));

            var result = await _controller.AjouterClinique(dto);

            var obj = result.Result as ObjectResult;
            obj.Should().NotBeNull();
            obj!.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task ModifierClinique_ReturnsNoContent_OnSuccess()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateCliniqueCommand>(), default)).ReturnsAsync(new Clinique());
            var clinique = new Clinique { Id = Guid.NewGuid(), Nom = "Test" };

            var result = await _controller.ModifierClinique(clinique.Id, clinique);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task ModifierClinique_ReturnsNotFound_OnKeyNotFound()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateCliniqueCommand>(), default)).ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.ModifierClinique(Guid.NewGuid(), new Clinique());

            var notFound = result as NotFoundObjectResult;
            notFound.Should().NotBeNull();
            notFound!.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task SupprimerClinique_ReturnsNoContent_OnSuccess()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<SupprimerCliniqueCommand>(), default)).ReturnsAsync(true);

            var result = await _controller.SupprimerClinique(Guid.NewGuid());

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task SupprimerClinique_ReturnsNotFound_OnFalse()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<SupprimerCliniqueCommand>(), default)).ReturnsAsync(false);

            var result = await _controller.SupprimerClinique(Guid.NewGuid());

            var notFound = result as NotFoundObjectResult;
            notFound.Should().NotBeNull();
            notFound!.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task ObtenirClinique_ReturnsOk_OnSuccess()
        {
            var clinique = new Clinique { Id = Guid.NewGuid(), Nom = "Test" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<ObtenirCliniqueParIdQuery>(), default)).ReturnsAsync(clinique);

            var result = await _controller.ObtenirClinique(clinique.Id);

            var ok = result.Result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.Value.Should().Be(clinique);
        }

        [Fact]
        public async Task ObtenirClinique_ReturnsNotFound_OnNull()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<ObtenirCliniqueParIdQuery>(), default)).ReturnsAsync((Clinique?)null);

            var result = await _controller.ObtenirClinique(Guid.NewGuid());

            var notFound = result.Result as NotFoundObjectResult;
            notFound.Should().NotBeNull();
            notFound!.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task ListerCliniques_ReturnsOk()
        {
            var cliniques = new List<Clinique> { new Clinique { Id = Guid.NewGuid(), Nom = "A" } };
            _mediatorMock.Setup(m => m.Send(It.IsAny<ListerCliniquesQuery>(), default)).ReturnsAsync(cliniques);

            var result = await _controller.ListerCliniques();

            var ok = result.Result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.Value.Should().BeEquivalentTo(cliniques);
        }

        [Fact]
        public async Task ListerCliniquesParNom_ReturnsOk()
        {
            var cliniques = new List<Clinique> { new Clinique { Id = Guid.NewGuid(), Nom = "A" } };
            _mediatorMock.Setup(m => m.Send(It.IsAny<RechercherCliniqueParNomQuery>(), default)).ReturnsAsync(cliniques);

            var result = await _controller.ListerCliniquesParNom("A");

            var ok = result.Result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.Value.Should().BeEquivalentTo(cliniques);
        }

        [Fact]
        public async Task ListerCliniquesParAdresse_ReturnsOk()
        {
            var cliniques = new List<Clinique> { new Clinique { Id = Guid.NewGuid(), Adresse = "Rue" } };
            _mediatorMock.Setup(m => m.Send(It.IsAny<RechercherCliniqueParAdresseQuery>(), default)).ReturnsAsync(cliniques);

            var result = await _controller.ListerCliniquesParAdresse("Rue");

            var ok = result.Result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.Value.Should().BeEquivalentTo(cliniques);
        }

        [Fact]
        public async Task ListerCliniquesParType_ReturnsOk()
        {
            var cliniques = new List<Clinique> { new Clinique { Id = Guid.NewGuid(), TypeClinique = TypeClinique.Publique } };
            _mediatorMock.Setup(m => m.Send(It.IsAny<RechercherCliniqueParTypeQuery>(), default)).ReturnsAsync(cliniques);

            var result = await _controller.ListerCliniquesParType(TypeClinique.Publique);

            var ok = result.Result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.Value.Should().BeEquivalentTo(cliniques);
        }

        [Fact]
        public async Task ListerCliniquesParStatut_ReturnsOk()
        {
            var cliniques = new List<Clinique> { new Clinique { Id = Guid.NewGuid(), Statut = StatutClinique.Active } };
            _mediatorMock.Setup(m => m.Send(It.IsAny<RechercherCliniqueParStatusQuery>(), default)).ReturnsAsync(cliniques);

            var result = await _controller.ListerCliniquesParStatut(StatutClinique.Active);

            var ok = result.Result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.Value.Should().BeEquivalentTo(cliniques);
        }

        [Fact]
        public async Task GetNombreDeCliniques_ReturnsOk()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetNombreCliniquesQuery>(), default)).ReturnsAsync(5);

            var result = await _controller.GetNombreDeCliniques();

            var ok = result.Result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.Value.Should().Be(5);
        }

        [Fact]
        public async Task GetNombreNouvellesCliniquesDuMois_ReturnsOk()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetNombreNouvellesCliniquesDuMoisQuery>(), default)).ReturnsAsync(2);

            var result = await _controller.GetNombreNouvellesCliniquesDuMois();

            var ok = result.Result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.Value.Should().Be(2);
        }

        [Fact]
        public async Task GetNombreNouvellesCliniquesParMois_ReturnsOk()
        {
            var stats = new List<StatistiqueDTO> { new StatistiqueDTO { Cle = "Janvier", Nombre = 1 } };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetNombreNouvellesCliniquesParMoisQuery>(), default)).ReturnsAsync(stats);

            var result = await _controller.GetNombreNouvellesCliniquesParMois();

            var ok = result.Result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.Value.Should().BeEquivalentTo(stats);
        }

        [Fact]
        public async Task GetStatistiquesDesCliniques_ReturnsOk()
        {
            var stat = new StatistiqueCliniqueDTO { CliniqueId = Guid.NewGuid(), Nom = "Test" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetStatistiquesDesCliniquesQuery>(), default)).ReturnsAsync(stat);

            var result = await _controller.GetStatistiquesDesCliniques(stat.CliniqueId);

            var ok = result.Result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.Value.Should().Be(stat);
        }

        [Fact]
        public async Task GetStatistiquesDesCliniques_ReturnsNotFound_OnNull()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetStatistiquesDesCliniquesQuery>(), default)).ReturnsAsync((StatistiqueCliniqueDTO?)null);

            var result = await _controller.GetStatistiquesDesCliniques(Guid.NewGuid());

            var notFound = result.Result as NotFoundObjectResult;
            notFound.Should().NotBeNull();
            notFound!.StatusCode.Should().Be(404);
        }
    }
}
