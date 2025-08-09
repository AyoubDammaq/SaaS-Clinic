using AutoMapper;
using Facturation.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Facturation.Application.TarificationService.Commands.Update
{
    public class UpdateCommandHandler : IRequestHandler<UpdateCommand>
    {
        private readonly ITarificationConsultationRepository _tarificationConsultationRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateCommandHandler> _logger;
        public UpdateCommandHandler(ITarificationConsultationRepository tarificationConsultationRepository, IMapper mapper, ILogger<UpdateCommandHandler> logger)
        {
            _tarificationConsultationRepository = tarificationConsultationRepository;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task Handle(UpdateCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (request.request == null)
                throw new ArgumentNullException(nameof(request.request));
            var tarifConsultation = await _tarificationConsultationRepository.GetByIdAsync(request.request.Id);
            if (tarifConsultation == null)
            {
                _logger.LogWarning("Aucun tarif trouvé pour l'ID {Id}", request.request.Id);
                throw new KeyNotFoundException($"Aucun tarif trouvé pour l'ID {request.request.Id}");
            }
            _mapper.Map(request.request, tarifConsultation);
            await _tarificationConsultationRepository.UpdateAsync(tarifConsultation);
            _logger.LogInformation("Tarif mis à jour : {Id} - {Prix} $", tarifConsultation.Id, tarifConsultation.Prix);
        }
    }
}
