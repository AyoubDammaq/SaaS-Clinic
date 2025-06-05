using Clinic.Domain.Entities;
using Clinic.Domain.Interfaces;
using MediatR;

namespace Clinic.Application.Commands.AjouterClinique
{
    public class AjouterCliniqueHandler : IRequestHandler<AjouterCliniqueCommand, Clinique>
    {
        private readonly ICliniqueRepository _repository;

        public AjouterCliniqueHandler(ICliniqueRepository repository)
        {
            _repository = repository;
        }

        public async Task<Clinique> Handle(AjouterCliniqueCommand request, CancellationToken cancellationToken)
        {
            var dto = request.CliniqueDto;

            if (string.IsNullOrWhiteSpace(dto.Nom))
                throw new ArgumentException("Nom requis");
            if (string.IsNullOrWhiteSpace(dto.Adresse))
                throw new ArgumentException("Adresse requise");

            var clinique = new Clinique
            {
                Nom = dto.Nom,
                Adresse = dto.Adresse,
                NumeroTelephone = dto.NumeroTelephone,
                Email = dto.Email,
                SiteWeb = dto.SiteWeb,
                Description = dto.Description,
                TypeClinique = dto.TypeClinique,
                Statut = dto.Statut
            };
            clinique.AjouterCliniqueEvent();

            await _repository.AddAsync(clinique);
            return clinique;
        }
    }
}
