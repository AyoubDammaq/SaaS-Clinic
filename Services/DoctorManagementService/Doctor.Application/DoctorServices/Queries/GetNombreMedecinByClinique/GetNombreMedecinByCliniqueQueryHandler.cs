using Doctor.Application.DTOs;
using Doctor.Domain.Interfaces;
using MediatR;

namespace Doctor.Application.DoctorServices.Queries.GetNombreMedecinByClinique
{
    public class GetNombreMedecinByCliniqueQueryHandler : IRequestHandler<GetNombreMedecinByCliniqueQuery, IEnumerable<StatistiqueMedecinDTO>>
    {
        private readonly IMedecinRepository _medecinRepository;
        public GetNombreMedecinByCliniqueQueryHandler(IMedecinRepository medecinRepository)
        {
            _medecinRepository = medecinRepository;
        }
        public async Task<IEnumerable<StatistiqueMedecinDTO>> Handle(GetNombreMedecinByCliniqueQuery request, CancellationToken cancellationToken)
        {
            var statistiques = await _medecinRepository.GetNombreMedecinByCliniqueAsync();

            return statistiques.Select(s => new StatistiqueMedecinDTO
            {
                Cle = s.Cle,
                Nombre = s.Nombre
            });
        }
    }
}
