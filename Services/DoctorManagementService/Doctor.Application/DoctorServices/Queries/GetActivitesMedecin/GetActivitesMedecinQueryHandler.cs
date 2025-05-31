using Doctor.Application.DTOs;
using Doctor.Domain.Interfaces;
using MediatR;

namespace Doctor.Application.DoctorServices.Queries.GetActivitesMedecin
{
    public class GetActivitesMedecinQueryHandler : IRequestHandler<GetActivitesMedecinQuery, IEnumerable<ActiviteMedecinDTO>>
    {
        private readonly IMedecinRepository _medecinRepository;
        public GetActivitesMedecinQueryHandler(IMedecinRepository medecinRepository)
        {
            _medecinRepository = medecinRepository;
        }
        public async Task<IEnumerable<ActiviteMedecinDTO>> Handle(GetActivitesMedecinQuery request, CancellationToken cancellationToken)
        {
            if (request.medecinId == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant du médecin est invalide.", nameof(request.medecinId));
            }
            var activites = await _medecinRepository.GetActivitesMedecinAsync(request.medecinId);

            return activites.Select(a => new ActiviteMedecinDTO
            {
                MedecinId = a.MedecinId,
                NomComplet = a.NomComplet,
                NombreConsultations = a.NombreConsultations,
                NombreRendezVous = a.NombreRendezVous,
            });
        }
    }
}
