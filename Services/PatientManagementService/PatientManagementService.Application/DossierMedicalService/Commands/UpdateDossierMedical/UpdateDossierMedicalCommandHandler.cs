using MediatR;
using PatientManagementService.Domain.Interfaces;

namespace PatientManagementService.Application.DossierMedicalService.Commands.UpdateDossierMedical
{
    public class UpdateDossierMedicalCommandHandler : IRequestHandler<UpdateDossierMedicalCommand>
    {
        private readonly IDossierMedicalRepository _dossierMedicalRepository;
        public UpdateDossierMedicalCommandHandler(IDossierMedicalRepository dossierMedicalRepository)
        {
            _dossierMedicalRepository = dossierMedicalRepository;
        }
        public async Task Handle(UpdateDossierMedicalCommand request, CancellationToken cancellationToken)
        {
            var existingDossier = await _dossierMedicalRepository.GetDossierMedicalByIdAsync(request.dossierMedical.Id)
                ?? throw new Exception("Dossier médical not found");

            existingDossier.Allergies = request.dossierMedical.Allergies;
            existingDossier.MaladiesChroniques = request.dossierMedical.MaladiesChroniques;
            existingDossier.MedicamentsActuels = request.dossierMedical.MedicamentsActuels;
            existingDossier.AntécédentsFamiliaux = request.dossierMedical.AntécédentsFamiliaux;
            existingDossier.AntécédentsPersonnels = request.dossierMedical.AntécédentsPersonnels;
            existingDossier.GroupeSanguin = request.dossierMedical.GroupeSanguin;

            await _dossierMedicalRepository.UpdateDossierMedicalAsync(existingDossier);
        }

    }
}
