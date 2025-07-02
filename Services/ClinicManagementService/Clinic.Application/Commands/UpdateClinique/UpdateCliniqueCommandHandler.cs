using Clinic.Domain.Entities;
using Clinic.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Clinic.Application.Commands.ModifierClinique
{
    public class UpdateCliniqueCommandHandler : IRequestHandler<UpdateCliniqueCommand, Clinique>
    {
        private readonly ICliniqueRepository _repository;
        private readonly ILogger<UpdateCliniqueCommandHandler> _logger;

        public UpdateCliniqueCommandHandler(ICliniqueRepository repository, ILogger<UpdateCliniqueCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Clinique> Handle(UpdateCliniqueCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
                throw new ArgumentException("L'identifiant de la clinique est requis.", nameof(request.Id));

            var clinique = request.Clinique ?? throw new ArgumentNullException(nameof(request.Clinique));
            clinique.Id = request.Id;

            clinique.ModifierCliniqueEvent();

            await _repository.UpdateAsync(clinique);

            _logger.LogInformation(
                "[AUDIT] Clinique modifiée : Id={Id}, Nom={Nom}, ModifiéeLe={Date}",
                clinique.Id,
                clinique.Nom,
                DateTime.UtcNow
            );

            return clinique;
        }
    }
}
