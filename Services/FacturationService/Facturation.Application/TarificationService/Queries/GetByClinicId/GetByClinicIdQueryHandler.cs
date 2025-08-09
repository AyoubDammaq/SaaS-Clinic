using AutoMapper;
using Facturation.Application.DTOs;
using Facturation.Domain.Interfaces;
using MediatR;

namespace Facturation.Application.TarificationService.Queries.GetByClinicId
{
    public class GetByClinicIdQueryHandler : IRequestHandler<GetByClinicIdQuery, IEnumerable<TarifConsultationDto>>
    {
        private readonly ITarificationConsultationRepository _tarificationConsultationRepository;
        private readonly IMapper _mapper;

        public GetByClinicIdQueryHandler(
            ITarificationConsultationRepository tarificationConsultationRepository,
            IMapper mapper)
        {
            _tarificationConsultationRepository = tarificationConsultationRepository ?? throw new ArgumentNullException(nameof(tarificationConsultationRepository));
            _mapper = mapper;
        }

        public async Task<IEnumerable<TarifConsultationDto>> Handle(GetByClinicIdQuery request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var tarifs = await _tarificationConsultationRepository.GetByClinicIdAsync(request.cliniqueId);
            return _mapper.Map<IEnumerable<TarifConsultationDto>>(tarifs);
        }
    }
}
