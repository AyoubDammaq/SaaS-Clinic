using MediatR;
using RDV.Domain.Interfaces;

namespace RDV.Application.Commands.CreateRendezVous
{
    public class CreateRendezVousCommandHandler : IRequestHandler<CreateRendezVousCommand>
    {
        private readonly IRendezVousRepository _rendezVousRepository;
        public CreateRendezVousCommandHandler(IRendezVousRepository rendezVousRepository)
        {
            _rendezVousRepository = rendezVousRepository;
        }
        public async Task Handle(CreateRendezVousCommand request, CancellationToken cancellationToken)
        {
            if (request.rendezVous == null)
            {
                throw new ArgumentNullException(nameof(request.rendezVous), "Le rendez-vous ne peut pas être nul.");
            }

            request.rendezVous.CreerRendezVousEvent();

            await _rendezVousRepository.CreateRendezVousAsync(request.rendezVous);
        }
    }
}
