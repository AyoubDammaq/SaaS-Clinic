using ClinicManagementService.Data;
using ClinicManagementService.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementService.Service
{
    public class CliniqueService : ICliniqueService
    {
        private readonly CliniqueDbContext _context;
        private readonly Guid _tenantId;

        public CliniqueService(CliniqueDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            // Récupération du TenantId depuis le contexte HTTP
            _tenantId = httpContextAccessor.HttpContext?.Items["TenantId"] is Guid tenantId
                ? tenantId
                : throw new UnauthorizedAccessException("Tenant non identifié");
        }

        public async Task<Clinique> AjouterCliniqueAsync(Clinique clinique)
        {
            clinique.Id = Guid.NewGuid();
            clinique.TenantId = _tenantId;

            _context.Cliniques.Add(clinique);
            await _context.SaveChangesAsync();

            return clinique;
        }

        public async Task<Clinique> ModifierCliniqueAsync(Guid id, Clinique clinique)
        {
            var cliniqueExistante = await _context.Cliniques.FindAsync(id);

            if (cliniqueExistante == null)
                throw new KeyNotFoundException("Clinique non trouvée");

            cliniqueExistante.Nom = clinique.Nom;
            cliniqueExistante.Adresse = clinique.Adresse;
            cliniqueExistante.NumeroTelephone = clinique.NumeroTelephone;
            cliniqueExistante.Email = clinique.Email;

            await _context.SaveChangesAsync();
            return cliniqueExistante;
        }

        public async Task<bool> SupprimerCliniqueAsync(Guid id)
        {
            var clinique = await _context.Cliniques.FindAsync(id);

            if (clinique == null)
                return false;

            _context.Cliniques.Remove(clinique);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Clinique> ObtenirCliniqueParIdAsync(Guid id)
        {
            return await _context.Cliniques.FindAsync(id);
        }

        public async Task<IEnumerable<Clinique>> ListerCliniqueAsync()
        {
            return await _context.Cliniques.ToListAsync();
        }

    }
}
