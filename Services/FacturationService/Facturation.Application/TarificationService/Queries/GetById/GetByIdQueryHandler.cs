using AutoMapper;
using Facturation.Application.DTOs;
using Facturation.Domain.Entities;
using Facturation.Domain.Interfaces;
using MediatR;

namespace Facturation.Application.TarificationService.Queries.GetById
{
    public class GetByIdQueryHandler : IRequestHandler<GetByIdQuery, TarifConsultationDto?>
    {
        private readonly ITarificationConsultationRepository _tarificationConsultationRepository;
        private readonly IMapper _mapper;
        public GetByIdQueryHandler(ITarificationConsultationRepository tarificationConsultationRepository, IMapper mapper)
        {
            _tarificationConsultationRepository = tarificationConsultationRepository ?? throw new ArgumentNullException(nameof(tarificationConsultationRepository));
            _mapper = mapper;
        }
        public async Task<TarifConsultationDto?> Handle(GetByIdQuery request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            var tarifConsultation = await _tarificationConsultationRepository.GetByIdAsync(request.Id);
            if (tarifConsultation == null)
                throw new KeyNotFoundException($"Aucun tarif trouvé pour l'ID {request.Id}");

            return tarifConsultation == null ? null : _mapper.Map<TarifConsultationDto>(tarifConsultation);
        }
    }
}
