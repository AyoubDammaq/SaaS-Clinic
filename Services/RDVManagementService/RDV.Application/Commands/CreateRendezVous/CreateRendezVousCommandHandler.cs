using AutoMapper;
using MediatR;
using RDV.Domain.Entities;
using RDV.Domain.Enums;
using RDV.Domain.Interfaces;

namespace RDV.Application.Commands.CreateRendezVous
{
    public class CreateRendezVousCommandHandler : IRequestHandler<CreateRendezVousCommand>
    {
        private readonly IRendezVousRepository _rendezVousRepository;
        private readonly IMapper _mapper;
        public CreateRendezVousCommandHandler(IRendezVousRepository rendezVousRepository, IMapper mapper)
        {
            _rendezVousRepository = rendezVousRepository;
            _mapper = mapper;
        }
        public async Task Handle(CreateRendezVousCommand request, CancellationToken cancellationToken)
        {
            if (request.rendezVous == null)
            {
                throw new ArgumentNullException(nameof(request.rendezVous), "Le rendez-vous ne peut pas être nul.");
            }

            var entity = _mapper.Map<RendezVous>(request.rendezVous);

            // 🔒 Règle métier : empêcher les doubles réservations
            bool dejaPris = await _rendezVousRepository
                .ExisteRendezVousPourMedecinEtDate(request.rendezVous.MedecinId, request.rendezVous.DateHeure);

            if (dejaPris)
            {
                throw new InvalidOperationException("Un rendez-vous existe déjà à cette heure pour ce médecin.");
            }

            entity.Statut = RDVstatus.EN_ATTENTE;

            entity.CreerRendezVousEvent();

            await _rendezVousRepository.CreateRendezVousAsync(entity);
        }
    }
}
