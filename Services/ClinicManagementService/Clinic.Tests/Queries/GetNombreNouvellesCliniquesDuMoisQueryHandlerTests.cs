using Clinic.Application.Queries.GetNombreNouvellesCliniquesDuMois;
using Clinic.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Clinic.Tests.Queries
{
    public class GetNombreNouvellesCliniquesDuMoisQueryHandlerTests
    {
        private readonly Mock<ICliniqueRepository> _repositoryMock;
        private readonly GetNombreNouvellesCliniquesDuMoisQueryHandler _handler;

        public GetNombreNouvellesCliniquesDuMoisQueryHandlerTests()
        {
            _repositoryMock = new Mock<ICliniqueRepository>();
            _handler = new GetNombreNouvellesCliniquesDuMoisQueryHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WithPositiveNumber_ShouldReturnNumber()
        {
            _repositoryMock.Setup(r => r.GetNombreNouvellesCliniquesDuMoisAsync()).ReturnsAsync(3);
            var query = new GetNombreNouvellesCliniquesDuMoisQuery();

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Should().Be(3);
            _repositoryMock.Verify(r => r.GetNombreNouvellesCliniquesDuMoisAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_WithZero_ShouldReturnZero()
        {
            _repositoryMock.Setup(r => r.GetNombreNouvellesCliniquesDuMoisAsync()).ReturnsAsync(0);
            var query = new GetNombreNouvellesCliniquesDuMoisQuery();

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Should().Be(0);
            _repositoryMock.Verify(r => r.GetNombreNouvellesCliniquesDuMoisAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNegativeNumber_ShouldThrowInvalidOperationException()
        {
            _repositoryMock.Setup(r => r.GetNombreNouvellesCliniquesDuMoisAsync()).ReturnsAsync(-1);
            var query = new GetNombreNouvellesCliniquesDuMoisQuery();

            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

            await act.Should().ThrowAsync<InvalidOperationException>();
            _repositoryMock.Verify(r => r.GetNombreNouvellesCliniquesDuMoisAsync(), Times.Once);
        }
    }
}
