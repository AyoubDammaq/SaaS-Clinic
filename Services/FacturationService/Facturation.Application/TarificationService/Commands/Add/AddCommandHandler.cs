using AutoMapper;
using Facturation.Domain.Entities;
using Facturation.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Facturation.Application.TarificationService.Commands.Add
{
    public class AddCommandHandler : IRequestHandler<AddCommand>
    {
        private readonly ITarificationConsultationRepository _tarificationConsultationRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AddCommandHandler> _logger;
        public AddCommandHandler(ITarificationConsultationRepository tarificationConsultationRepository, IMapper mapper, ILogger<AddCommandHandler> logger)
        {
            _tarificationConsultationRepository = tarificationConsultationRepository;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task Handle(AddCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (request.request.Prix <= 0)
                throw new ArgumentException("Le prix doit être supérieur à zéro.", nameof(request.request.Prix));

            var tarif = _mapper.Map<TarifConsultation>(request.request);

            _logger.LogInformation("Ajout du tarif de consultation {TarifId} pour la clinique {ClinicId} et le type {ConsultationType}",
                tarif.Id, tarif.ClinicId, tarif.ConsultationType);
            await _tarificationConsultationRepository.AddAsync(tarif);

        }
    }
}
