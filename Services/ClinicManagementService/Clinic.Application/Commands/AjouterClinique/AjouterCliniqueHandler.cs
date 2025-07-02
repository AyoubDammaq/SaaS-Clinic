using AutoMapper;
using Clinic.Domain.Entities;
using Clinic.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Clinic.Application.Commands.AjouterClinique
{
    public class AjouterCliniqueHandler : IRequestHandler<AjouterCliniqueCommand, Clinique>
    {
        private readonly ICliniqueRepository _repository;
        private readonly ILogger<AjouterCliniqueHandler> _logger;
        private readonly IMapper _mapper;

        public AjouterCliniqueHandler(ICliniqueRepository repository, ILogger<AjouterCliniqueHandler> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Clinique> Handle(AjouterCliniqueCommand request, CancellationToken cancellationToken)
        {
            var dto = request.CliniqueDto;

            if (string.IsNullOrWhiteSpace(dto.Nom))
                throw new ArgumentException("Nom requis");
            if (string.IsNullOrWhiteSpace(dto.Adresse))
                throw new ArgumentException("Adresse requise");

            var clinique = _mapper.Map<Clinique>(dto);

            clinique.AjouterCliniqueEvent();

            await _repository.AddAsync(clinique);

            _logger.LogInformation(
                "[AUDIT] Clinique créée : Id={Id}, Nom={Nom}, Adresse={Adresse}, CrééeLe={Date}",
                clinique.Id,
                clinique.Nom,
                clinique.Adresse,
                DateTime.UtcNow
            );

            return clinique;
        }
    }
}
