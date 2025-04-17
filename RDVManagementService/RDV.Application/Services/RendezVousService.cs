using RDV.Application.DTOs;
using RDV.Application.Interfaces;
using RDV.Domain.Entities;
using RDV.Domain.Enums;
using RDV.Domain.Interfaces;

namespace RDV.Application.Services
{
    public class RendezVousService : IRendezVousService
    {
        private readonly IRendezVousRepository _rendezVousRepository;

        public RendezVousService(IRendezVousRepository rendezVousRepository)
        {
            _rendezVousRepository = rendezVousRepository ?? throw new ArgumentNullException(nameof(rendezVousRepository));
        }

        public async Task<IEnumerable<RendezVousDTO>> GetAllRendezVousAsync()
        {
            var rendezVousList = await _rendezVousRepository.GetAllRendezVousAsync();
            var dtoList = rendezVousList.Select(r => new RendezVousDTO
            {
                Id = r.Id,
                PatientId = r.PatientId,
                MedecinId = r.MedecinId,
                DateHeure = r.DateHeure,
                Statut = r.Statut,
                Commentaire = r.Commentaire ?? string.Empty
            });

            return dtoList;
        }

        public async Task<RendezVous> GetRendezVousByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant du rendez-vous ne peut pas être vide.", nameof(id));
            }
            return await _rendezVousRepository.GetRendezVousByIdAsync(id);
        }

        public async Task CreateRendezVousAsync(RendezVous rendezVous)
        {
            if (rendezVous == null)
            {
                throw new ArgumentNullException(nameof(rendezVous), "Le rendez-vous ne peut pas être nul.");
            }
            await _rendezVousRepository.CreateRendezVousAsync(rendezVous);
        }

        public async Task UpdateRendezVousAsync(Guid id, RendezVous rendezVous)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant du rendez-vous ne peut pas être vide.", nameof(id));
            }
            if (rendezVous == null)
            {
                throw new ArgumentNullException(nameof(rendezVous), "Le rendez-vous ne peut pas être nul.");
            }
            await _rendezVousRepository.UpdateRendezVousAsync(id, rendezVous);
        }

        public async Task<bool> AnnulerRendezVousAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant du rendez-vous ne peut pas être vide.", nameof(id));
            }
            return await _rendezVousRepository.AnnulerRendezVousAsync(id);
        }

        public async Task<IEnumerable<RendezVous>> GetRendezVousByPatientIdAsync(Guid patientId)
        {
            if (patientId == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant du patient ne peut pas être vide.", nameof(patientId));
            }
            return await _rendezVousRepository.GetRendezVousByPatientIdAsync(patientId);
        }

        public async Task<IEnumerable<RendezVous>> GetRendezVousByMedecinIdAsync(Guid medecinId)
        {
            if (medecinId == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant du médecin ne peut pas être vide.", nameof(medecinId));
            }
            return await _rendezVousRepository.GetRendezVousByMedecinIdAsync(medecinId);
        }

        public async Task<IEnumerable<RendezVous>> GetRendezVousByDateAsync(DateTime date)
        {
            return await _rendezVousRepository.GetRendezVousByDateAsync(date);
        }

        public async Task<IEnumerable<RendezVous>> GetRendezVousByStatutAsync(RDVstatus statut)
        {
            return await _rendezVousRepository.GetRendezVousByStatutAsync(statut);
        }

        public async Task ConfirmerRendezVousParMedecin(Guid rendezVousId)
        {
            if (rendezVousId == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant du rendez-vous ne peut pas être vide.", nameof(rendezVousId));
            }
            await _rendezVousRepository.ConfirmerRendezVousParMedecin(rendezVousId);
        }

        public async Task AnnulerRendezVousParMedecin(Guid rendezVousId, string justification)
        {
            if (rendezVousId == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant du rendez-vous ne peut pas être vide.", nameof(rendezVousId));
            }
            if (string.IsNullOrWhiteSpace(justification))
            {
                throw new ArgumentException("La justification ne peut pas être vide.", nameof(justification));
            }
            await _rendezVousRepository.AnnulerRendezVousParMedecin(rendezVousId, justification);
        }
    }
}
