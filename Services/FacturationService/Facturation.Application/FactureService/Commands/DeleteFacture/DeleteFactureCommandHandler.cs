using Facturation.Domain.Interfaces;
using MediatR;

namespace Facturation.Application.FactureService.Commands.DeleteFacture
{
    public class DeleteFactureCommandHandler : IRequestHandler<DeleteFactureCommand>
    {
        private readonly IFactureRepository _factureRepository;
        public DeleteFactureCommandHandler(IFactureRepository factureRepository)
        {
            _factureRepository = factureRepository;
        }
        public async Task Handle(DeleteFactureCommand request, CancellationToken cancellationToken)
        {
            if (request.id == Guid.Empty)
                throw new ArgumentException("L'identifiant de la facture ne peut pas être vide.", nameof(request.id));

            var existingFacture = await _factureRepository.GetFactureByIdAsync(request.id);
            if (existingFacture == null)
                throw new KeyNotFoundException($"Aucune facture trouvée avec l'identifiant {request.id}.");

            await _factureRepository.DeleteFactureAsync(request.id);
        }
    }
}
