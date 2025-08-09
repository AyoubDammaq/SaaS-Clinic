using AutoMapper;
using Facturation.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Facturation.Application.TarificationService.Commands.Delete
{
    public class DeleteCommandHandler : IRequestHandler<DeleteCommand>
    {
        private readonly ITarificationConsultationRepository _tarificationConsultationRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DeleteCommandHandler> _logger;
        public DeleteCommandHandler(ITarificationConsultationRepository tarificationConsultationRepository, IMapper mapper, ILogger<DeleteCommandHandler> logger)
        {
            _tarificationConsultationRepository = tarificationConsultationRepository;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task Handle(DeleteCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (request.Id == Guid.Empty)
                throw new ArgumentException("L'ID du tarif doit être spécifié.", nameof(request.Id));
            var tarif = await _tarificationConsultationRepository.GetByIdAsync(request.Id);
            if (tarif == null)
            {
                _logger.LogWarning("Aucun tarif trouvé avec l'ID {TarifId}", request.Id);
                return;
            }
            _logger.LogInformation("Suppression du tarif de consultation {TarifId} pour la clinique {ClinicId} et le type {ConsultationType}",
                tarif.Id, tarif.ClinicId, tarif.ConsultationType);

            await _tarificationConsultationRepository.DeleteAsync(request.Id);

        }
    }
}
