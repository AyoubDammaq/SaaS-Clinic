using Doctor.Application.DoctorServices.Queries.GetMedecinsIdsByCliniqueId;
using Doctor.Domain.Interfaces;
using Moq;

namespace Doctor.Tests.DoctorTests.Queries
{
    public class GetMedecinsIdsByCliniqueIdQueryHandlerTests
    {
        private readonly Mock<IMedecinRepository> _medecinRepositoryMock;
        private readonly GetMedecinsIdsByCliniqueIdQueryHandler _handler;

        public GetMedecinsIdsByCliniqueIdQueryHandlerTests()
        {
            _medecinRepositoryMock = new Mock<IMedecinRepository>();
            _handler = new GetMedecinsIdsByCliniqueIdQueryHandler(_medecinRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenCliniqueIdIsEmpty()
        {
            // Arrange
            var query = new GetMedecinsIdsByCliniqueIdQuery(Guid.Empty);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldReturnIds_WhenMedecinsExist()
        {
            // Arrange
            var cliniqueId = Guid.NewGuid();
            var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var query = new GetMedecinsIdsByCliniqueIdQuery(cliniqueId);

            _medecinRepositoryMock
                .Setup(r => r.GetMedecinsIdsByCliniqueId(cliniqueId))
                .ReturnsAsync(ids);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ids, result);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoMedecinFound()
        {
            // Arrange
            var cliniqueId = Guid.NewGuid();
            var query = new GetMedecinsIdsByCliniqueIdQuery(cliniqueId);

            _medecinRepositoryMock
                .Setup(r => r.GetMedecinsIdsByCliniqueId(cliniqueId))
                .ReturnsAsync(new List<Guid>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
