using Clinic.Application.Queries.ListerClinique;
using Clinic.Domain.Entities;
using Clinic.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Clinic.Tests.Queries
{
    public class ListerCliniquesQueryHandlerTests
    {
        private readonly Mock<ICliniqueRepository> _repositoryMock;
        private readonly ListerCliniquesQueryHandler _handler;

        public ListerCliniquesQueryHandlerTests()
        {
            _repositoryMock = new Mock<ICliniqueRepository>();
            _handler = new ListerCliniquesQueryHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WithCliniques_ShouldReturnList()
        {
            // Arrange
            var cliniques = new List<Clinique>
                {
                    new Clinique { Id = Guid.NewGuid(), Nom = "Clinique 1" },
                    new Clinique { Id = Guid.NewGuid(), Nom = "Clinique 2" }
                };
            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(cliniques);
            var query = new ListerCliniquesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result[0].Nom.Should().Be("Clinique 1");
            result[1].Nom.Should().Be("Clinique 2");
            _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNullFromRepository_ShouldReturnEmptyList()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync((List<Clinique>?)null);
            var query = new ListerCliniquesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        }
    }
}
