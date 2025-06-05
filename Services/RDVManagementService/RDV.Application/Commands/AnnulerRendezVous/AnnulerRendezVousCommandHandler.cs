using MediatR;
using RDV.Domain.Interfaces;

namespace RDV.Application.Commands.AnnulerRendezVous
{
    public class AnnulerRendezVousCommandHandler : IRequestHandler<AnnulerRendezVousCommand, bool>
    {
        private readonly IRendezVousRepository _rendezVousRepository;
        public AnnulerRendezVousCommandHandler(IRendezVousRepository repository)
        {
            _rendezVousRepository = repository;
        }
        public async Task<bool> Handle(AnnulerRendezVousCommand request, CancellationToken cancellationToken)
        {
            if (request.id == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant du rendez-vous ne peut pas être vide.", nameof(request.id));
            }

            var rendezvous = await _rendezVousRepository.GetRendezVousByIdAsync(request.id);
            rendezvous.AnnulerRendezVousEvent();

            return await _rendezVousRepository.AnnulerRendezVousAsync(request.id);
        }
    }
}
