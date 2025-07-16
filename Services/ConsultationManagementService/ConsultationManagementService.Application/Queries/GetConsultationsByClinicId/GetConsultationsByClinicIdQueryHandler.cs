using AutoMapper;
using ConsultationManagementService.Application.DTOs;
using ConsultationManagementService.Repositories;
using MediatR;

namespace ConsultationManagementService.Application.Queries.GetConsultationsByClinicId
{
    public class GetConsultationsByClinicIdQueryHandler : IRequestHandler<GetConsultationsByClinicIdQuery, List<ConsultationDTO>>
    {
        private readonly IConsultationRepository _consultationRepository;
        private readonly IMapper _mapper;
        public GetConsultationsByClinicIdQueryHandler(IConsultationRepository consultationRepository, IMapper mapper)
        {
            _consultationRepository = consultationRepository;
            _mapper = mapper;
        }
        public async Task<List<ConsultationDTO>> Handle(GetConsultationsByClinicIdQuery request, CancellationToken cancellationToken)
        {
            if (request.ClinicId == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant de la clinique ne peut pas être vide.", nameof(request.ClinicId));
            }
            var consultations = await _consultationRepository.GetConsultationsByClinicIdAsync(request.ClinicId);
            return _mapper.Map<List<ConsultationDTO>>(consultations);
        }
    }
}
