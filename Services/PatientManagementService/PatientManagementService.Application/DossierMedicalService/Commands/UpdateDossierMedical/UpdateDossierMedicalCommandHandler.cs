using AutoMapper;
using MediatR;
using PatientManagementService.Domain.Interfaces;

namespace PatientManagementService.Application.DossierMedicalService.Commands.UpdateDossierMedical
{
    public class UpdateDossierMedicalCommandHandler : IRequestHandler<UpdateDossierMedicalCommand>
    {
        private readonly IDossierMedicalRepository _dossierMedicalRepository;
        private readonly IMapper _mapper;
        public UpdateDossierMedicalCommandHandler(IDossierMedicalRepository dossierMedicalRepository, IMapper mapper)
        {
            _dossierMedicalRepository = dossierMedicalRepository;
            _mapper = mapper;
        }
        public async Task Handle(UpdateDossierMedicalCommand request, CancellationToken cancellationToken)
        {
            var existingDossier = await _dossierMedicalRepository.GetDossierMedicalByIdAsync(request.dossierMedical.Id)
                ?? throw new Exception("Dossier médical not found");

            _mapper.Map(request.dossierMedical, existingDossier);

            existingDossier.ModifierDossierMedicalEvent();

            await _dossierMedicalRepository.UpdateDossierMedicalAsync(existingDossier);
        }
    }
}
