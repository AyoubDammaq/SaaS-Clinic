using Doctor.Application.DTOs;
using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;
using MediatR;

namespace Doctor.Application.DoctorServices.Queries.FilterDoctorsByName
{
    public class FilterDoctorsByNameQueryHandler : IRequestHandler<FilterDoctorsByNameQuery, IEnumerable<GetMedecinRequestDto>>
    {
        private readonly IMedecinRepository _medecinRepository;
        public FilterDoctorsByNameQueryHandler(IMedecinRepository medecinRepository)
        {
            _medecinRepository = medecinRepository;
        }
        public async Task<IEnumerable<GetMedecinRequestDto>> Handle(FilterDoctorsByNameQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.name) && string.IsNullOrWhiteSpace(request.prenom))
            {
                throw new ArgumentException("Le nom ou le prénom doit être spécifié.");
            }

            var medecins = await _medecinRepository.FilterByNameOrPrenomAsync(request.name, request.prenom, request.page, request.pageSize);

            return medecins.Select(m => new GetMedecinRequestDto
            {
                Id = m.Id,
                Prenom = m.Prenom,
                Nom = m.Nom,
                Specialite = m.Specialite,
                CliniqueId = m.CliniqueId,
                Email = m.Email,
                Telephone = m.Telephone,
                PhotoUrl = m.PhotoUrl,
                Disponibilites = m.Disponibilites.ToList()
            });
        }
    }
}
