using Clinic.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Clinic.Application.Commands.SupprimerClinique
{
    public class SupprimerCliniqueCommandHandler : IRequestHandler<SupprimerCliniqueCommand, bool>
    {
        private readonly ICliniqueRepository _repository;
        private readonly ILogger<SupprimerCliniqueCommandHandler> _logger;

        public SupprimerCliniqueCommandHandler(ICliniqueRepository repository, ILogger<SupprimerCliniqueCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
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

            _logger.LogInformation(
                "[AUDIT] Clinique supprimée : Id={Id}, Nom={Nom}, SuppriméeLe={Date}",
                clinique.Id,
                clinique.Nom,
                DateTime.UtcNow
            );

            return true;
        }
    }
}
