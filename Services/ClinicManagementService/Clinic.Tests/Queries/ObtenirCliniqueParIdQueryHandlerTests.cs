using Clinic.Application.Queries.ObtenirCliniqueParId;
using Clinic.Domain.Entities;
using Clinic.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Clinic.Tests.Queries
{
    public class ObtenirCliniqueParIdQueryHandlerTests
    {
        private readonly Mock<ICliniqueRepository> _repositoryMock;
        private readonly ObtenirCliniqueParIdQueryHandler _handler;

        public ObtenirCliniqueParIdQueryHandlerTests()
        {
            _repositoryMock = new Mock<ICliniqueRepository>();
            _handler = new ObtenirCliniqueParIdQueryHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidId_ShouldReturnClinique()
        {
            // Arrange
            var id = Guid.NewGuid();
            var clinique = new Clinique { Id = id, Nom = "Clinique Test" };
            _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(clinique);
            var query = new ObtenirCliniqueParIdQuery(id);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(id);
            result.Nom.Should().Be("Clinique Test");
            _repositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task Handle_WithEmptyId_ShouldThrowArgumentException()
        {
            // Arrange
            var query = new ObtenirCliniqueParIdQuery(Guid.Empty);

            // Act
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>();
            _repositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WithUnknownId_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Clinique?)null);
            var query = new ObtenirCliniqueParIdQuery(id);

            // Act
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>();
            _repositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once);
        }
    }
}
