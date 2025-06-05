using MediatR;
using PatientManagementService.Domain.Interfaces;

namespace PatientManagementService.Application.DossierMedicalService.Commands.DeleteDossierMedical
{
    public class DeleteDossierMedicalCommandHandler : IRequestHandler<DeleteDossierMedicalCommand>
    {
        private readonly IDossierMedicalRepository _dossierMedicalRepository;

        public DeleteDossierMedicalCommandHandler(IDossierMedicalRepository repository)
        {
            _dossierMedicalRepository = repository;
        }

        public async Task Handle(DeleteDossierMedicalCommand request, CancellationToken cancellationToken)
        {
            var dossierMedical = await _dossierMedicalRepository.GetDossierMedicalByIdAsync(request.dossierMedicalId) ?? throw new Exception("Dossier médical not found");

            dossierMedical.SupprimerDossierMedicalEvent();

            await _dossierMedicalRepository.DeleteDossierMedicalAsync(request.dossierMedicalId);
        }
    }
}
