using Clinic.Domain.Interfaces;
using MediatR;

namespace Clinic.Application.Queries.GetNombreNouvellesCliniquesDuMois
{
    public class GetNombreNouvellesCliniquesDuMoisQueryHandler : IRequestHandler<GetNombreNouvellesCliniquesDuMoisQuery, int>
    {
        private readonly ICliniqueRepository _repository;
        public GetNombreNouvellesCliniquesDuMoisQueryHandler(ICliniqueRepository repository)
        {
            _repository = repository;
        }
        public async Task<int> Handle(GetNombreNouvellesCliniquesDuMoisQuery request, CancellationToken cancellationToken)
        {
            var nombre = await _repository.GetNombreNouvellesCliniquesDuMoisAsync();
            if (nombre < 0)
                throw new InvalidOperationException("Le nombre de nouvelles cliniques ne peut pas être négatif.");
            return nombre;
        }
    } 
}
