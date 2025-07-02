using Clinic.Application.Queries.GetNombreCliniques;
using Clinic.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Clinic.Tests.Queries
{
    public class GetNombreCliniquesQueryHandlerTests
    {
        private readonly Mock<ICliniqueRepository> _repositoryMock;
        private readonly GetNombreCliniquesQueryHandler _handler;

        public GetNombreCliniquesQueryHandlerTests()
        {
            _repositoryMock = new Mock<ICliniqueRepository>();
            _handler = new GetNombreCliniquesQueryHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WithPositiveNumber_ShouldReturnNumber()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetNombreCliniquesAsync()).ReturnsAsync(5);
            var query = new GetNombreCliniquesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().Be(5);
            _repositoryMock.Verify(r => r.GetNombreCliniquesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_WithZero_ShouldReturnZero()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetNombreCliniquesAsync()).ReturnsAsync(0);
            var query = new GetNombreCliniquesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().Be(0);
            _repositoryMock.Verify(r => r.GetNombreCliniquesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNegativeNumber_ShouldThrowInvalidOperationException()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetNombreCliniquesAsync()).ReturnsAsync(-1);
            var query = new GetNombreCliniquesQuery();

            // Act
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
            _repositoryMock.Verify(r => r.GetNombreCliniquesAsync(), Times.Once);
        }
    }
}
