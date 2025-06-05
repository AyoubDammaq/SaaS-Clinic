using Clinic.Domain.Interfaces;
using MediatR;

namespace Clinic.Application.Commands.SupprimerClinique
{
    public class SupprimerCliniqueCommandHandler : IRequestHandler<SupprimerCliniqueCommand, bool>
    {
        private readonly ICliniqueRepository _repository;

        public SupprimerCliniqueCommandHandler(ICliniqueRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(SupprimerCliniqueCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
                throw new ArgumentException("L'identifiant de la clinique est requis.", nameof(request.Id));

            var clinique = await _repository.GetByIdAsync(request.Id);
            if (clinique == null)
                throw new InvalidOperationException("Clinique non trouvée.");

            clinique.SupprimerCliniqueEvent();

            await _repository.DeleteAsync(request.Id);
            return true;
        }
    }
}
