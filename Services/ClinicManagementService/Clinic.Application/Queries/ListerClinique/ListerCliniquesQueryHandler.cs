using Clinic.Domain.Entities;
using Clinic.Domain.Interfaces;
using MediatR;

namespace Clinic.Application.Queries.ListerClinique
{
    public class ListerCliniquesQueryHandler : IRequestHandler<ListerCliniquesQuery, List<Clinique>>
    {
        private readonly ICliniqueRepository _repository;

        public ListerCliniquesQueryHandler(ICliniqueRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Clinique>> Handle(ListerCliniquesQuery request, CancellationToken cancellationToken)
        {
            var cliniques = await _repository.GetAllAsync();
            return cliniques ?? new List<Clinique>();
        }
    }
}
