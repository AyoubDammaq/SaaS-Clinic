using Clinic.Domain.Interfaces;
using MediatR;

namespace Clinic.Application.Queries.GetNombreNouveauxCliniques
{
    public class GetNombreNouveauxCliniquesQueryHandler : IRequestHandler<GetNombreNouveauxCliniquesQuery, int>
    {
        private readonly ICliniqueRepository _cliniqueRepository;
        public GetNombreNouveauxCliniquesQueryHandler(ICliniqueRepository cliniqueRepository)
        {
            _cliniqueRepository = cliniqueRepository;
        }
        public async Task<int> Handle(GetNombreNouveauxCliniquesQuery request, CancellationToken cancellationToken)
        {
            return await _cliniqueRepository.GetNombreNouveauxCliniquesAsync(request.DateDebut, request.DateFin);
        }
    }
}
