using AutoMapper;
using Facturation.Application.DTOs;
using Facturation.Domain.Interfaces;
using MediatR;

namespace Facturation.Application.TarificationService.Queries.GetAll
{
    public class GetAllQueryHandler : IRequestHandler<GetAllQuery, IEnumerable<TarifConsultationDto>>
    {
        private readonly ITarificationConsultationRepository _tarificationConsultationRepository;
        private readonly IMapper _mapper;

        public GetAllQueryHandler(ITarificationConsultationRepository tarificationConsultationRepository, IMapper mapper)
        {
            _tarificationConsultationRepository = tarificationConsultationRepository ?? throw new ArgumentNullException(nameof(tarificationConsultationRepository));
            _mapper = mapper;
        }
        public async Task<IEnumerable<TarifConsultationDto>> Handle(GetAllQuery request, CancellationToken cancellationToken)
        {
            var tarifs = await _tarificationConsultationRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<TarifConsultationDto>>(tarifs);
        }
    }
}
