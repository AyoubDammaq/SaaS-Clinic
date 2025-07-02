using Clinic.Domain.Interfaces;
using MediatR;

namespace Clinic.Application.Queries.GetNombreCliniques
{
    public class GetNombreCliniquesQueryHandler : IRequestHandler<GetNombreCliniquesQuery, int>
    {
        private readonly ICliniqueRepository _repository;

        public GetNombreCliniquesQueryHandler(ICliniqueRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> Handle(GetNombreCliniquesQuery request, CancellationToken cancellationToken)
        {
            var nombre = await _repository.GetNombreCliniquesAsync();
            if (nombre < 0)
                throw new InvalidOperationException("Le nombre de cliniques ne peut pas être négatif.");
            return nombre;
        }
    }
}
