using Clinic.Domain.Entities;
using Clinic.Domain.Interfaces;
using MediatR;

namespace Clinic.Application.Commands.ModifierClinique
{
    public class ModifierCliniqueCommandHandler : IRequestHandler<ModifierCliniqueCommand, Clinique>
    {
        private readonly ICliniqueRepository _repository;

        public ModifierCliniqueCommandHandler(ICliniqueRepository repository)
        {
            _repository = repository;
        }

        public async Task<Clinique> Handle(ModifierCliniqueCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
                throw new ArgumentException("L'identifiant de la clinique est requis.", nameof(request.Id));

            var clinique = request.Clinique ?? throw new ArgumentNullException(nameof(request.Clinique));
            clinique.Id = request.Id;

            await _repository.UpdateAsync(clinique);
            return clinique;
        }
    }
}
