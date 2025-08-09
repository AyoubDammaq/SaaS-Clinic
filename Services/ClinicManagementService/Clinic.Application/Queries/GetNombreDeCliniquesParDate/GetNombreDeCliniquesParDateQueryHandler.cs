using Clinic.Domain.Interfaces;
using MediatR;

namespace Clinic.Application.Queries.GetNombreDeCliniquesParDate
{
    public class GetNombreDeCliniquesParDateQueryHandler : IRequestHandler<GetNombreDeCliniquesParDateQuery, int>
    {
        private readonly ICliniqueRepository _cliniqueRepository;
        public GetNombreDeCliniquesParDateQueryHandler(ICliniqueRepository cliniqueRepository)
        {
            _cliniqueRepository = cliniqueRepository;
        }
        public async Task<int> Handle(GetNombreDeCliniquesParDateQuery request, CancellationToken cancellationToken)
        {
            return await _cliniqueRepository.GetNombreDeCliniquesParDateAsync(request.startDate, request.endDate);
        }
    }
}
