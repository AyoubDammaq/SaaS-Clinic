using Microsoft.EntityFrameworkCore;
using RDVManagementService.Data;
using RDVManagementService.Models;

namespace RDVManagementService.Repositories
{
    public class RendezVousRepository : IRendezVousRepository
    {
        private readonly RendezVousDbContext _context;

        public RendezVousRepository(RendezVousDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RendezVous>> GetAllRendezVousAsync()
        {
            return await _context.RendezVous.ToListAsync();
        }

        public async Task<RendezVous> GetRendezVousByIdAsync(Guid id)
        {
            return await _context.RendezVous.FindAsync(id);
        }

        public async Task CreateRendezVousAsync(RendezVousDTO rendezVous)
        {
            var rendezVousEntity = new RendezVous
            {
                Id = rendezVous.Id,
                PatientId = rendezVous.PatientId,
                MedecinId = rendezVous.MedecinId,
                DateHeure = rendezVous.DateHeure,
                Statut = RDVstatus.EN_ATTENTE,
                Commentaire = rendezVous.Commentaire
            };
            await _context.RendezVous.AddAsync(rendezVousEntity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRendezVousAsync(Guid id, RendezVousDTO rendezVous)
        {
            var existingRendezVous = await _context.RendezVous.FindAsync(id);
            if (existingRendezVous == null)
            {
                throw new Exception("Rendez-vous non trouvé");
            }
            existingRendezVous.PatientId = rendezVous.PatientId;
            existingRendezVous.MedecinId = rendezVous.MedecinId;
            existingRendezVous.DateHeure = rendezVous.DateHeure;
            existingRendezVous.Commentaire = rendezVous.Commentaire;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AnnulerRendezVousAsync(Guid id)
        {
            var rendezVous = await _context.RendezVous.FindAsync(id);
            if (rendezVous == null)
            {
                throw new Exception("Rendez-vous non trouvé");
            }
            rendezVous.Statut = RDVstatus.ANNULE;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<RendezVous>> GetRendezVousByPatientIdAsync(Guid patientId)
        {
            return await _context.RendezVous.Where(r => r.PatientId == patientId).ToListAsync();
        }

        public async Task<IEnumerable<RendezVous>> GetRendezVousByMedecinIdAsync(Guid medecinId)
        {
            return await _context.RendezVous.Where(r => r.MedecinId == medecinId).ToListAsync();
        }

        public async Task<IEnumerable<RendezVous>> GetRendezVousByDateAsync(DateTime date)
        {
            return await _context.RendezVous.Where(r => r.DateHeure.Date == date.Date).ToListAsync();
        }

        public async Task<IEnumerable<RendezVous>> GetRendezVousByStatutAsync(RDVstatus statut)
        {
            return await _context.RendezVous.Where(r => r.Statut == statut).ToListAsync();
        }

        public async Task ConfirmerRendezVousParMedecin(Guid rendezVousId)
        {
            var rendezVous = await _context.RendezVous.FindAsync(rendezVousId);
            if (rendezVous == null)
            {
                throw new Exception("Rendez-vous non trouvé");
            }
            rendezVous.Statut = RDVstatus.CONFIRME;
            await _context.SaveChangesAsync();
        }

        public async Task AnnulerRendezVousParMedecin(Guid rendezVousId, string justification)
        {
            var rendezVous = await _context.RendezVous.FindAsync(rendezVousId);
            if (rendezVous == null)
            {
                throw new Exception("Rendez-vous non trouvé");
            }   
            rendezVous.Statut = RDVstatus.ANNULE;
            rendezVous.Commentaire = justification;
            await _context.SaveChangesAsync();
        }
        
    }

}
