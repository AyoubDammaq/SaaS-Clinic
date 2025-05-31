using Doctor.Application.DTOs;
using Doctor.Domain.Interfaces;
using MediatR;

namespace Doctor.Application.DoctorServices.Queries.GetNombreMedecinBySpecialite
{
    public class GetNombreMedecinBySpecialiteQueryHandler : IRequestHandler<GetNombreMedecinBySpecialiteQuery, IEnumerable<StatistiqueMedecinDTO>>
    {
        private readonly IMedecinRepository _medecinRepository;
        public GetNombreMedecinBySpecialiteQueryHandler(IMedecinRepository medecinRepository)
        {
            _medecinRepository = medecinRepository;
        }
        public async Task<IEnumerable<StatistiqueMedecinDTO>> Handle(GetNombreMedecinBySpecialiteQuery request, CancellationToken cancellationToken)
        {
            var statistiques = await _medecinRepository.GetNombreMedecinBySpecialiteAsync();

            return statistiques.Select(s => new StatistiqueMedecinDTO
            {
                Cle = s.Cle,
                Nombre = s.Nombre
            });
        }
    }
}
