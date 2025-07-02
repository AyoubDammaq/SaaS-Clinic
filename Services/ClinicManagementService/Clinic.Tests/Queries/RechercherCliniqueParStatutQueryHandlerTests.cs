using Clinic.Application.Queries.RechercherCliniqueParStatut;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;
using Clinic.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Clinic.Tests.Queries
{
    public class RechercherCliniqueParStatutQueryHandlerTests
    {
        private readonly Mock<ICliniqueRepository> _repositoryMock = new();
        private readonly RechercherCliniqueParStatutQueryHandler _handler;

        public RechercherCliniqueParStatutQueryHandlerTests()
        {
            _handler = new RechercherCliniqueParStatutQueryHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WithStatut_ShouldReturnCliniques()
        {
            var cliniques = new List<Clinique?>
            {
                new Clinique { Id = Guid.NewGuid(), Statut = StatutClinique.Active },
                null,
                new Clinique { Id = Guid.NewGuid(), Statut = StatutClinique.Active }
            };
            _repositoryMock.Setup(r => r.GetByStatusAsync(StatutClinique.Active)).ReturnsAsync(cliniques);
            var query = new RechercherCliniqueParStatusQuery(StatutClinique.Active);

            var result = (await _handler.Handle(query, CancellationToken.None)).ToList();

            result.Should().HaveCount(2);
            result.All(c => c != null).Should().BeTrue();
            _repositoryMock.Verify(r => r.GetByStatusAsync(StatutClinique.Active), Times.Once);
        }
    }
}
