﻿using DoctorManagementService.Data;
using DoctorManagementService.Models;
using Microsoft.EntityFrameworkCore;

namespace DoctorManagementService.Repositories
{
    public class MedecinRepository : IMedecinRepository
    {
        private readonly MedecinDbContext _context;
        public MedecinRepository(MedecinDbContext context)
        {
            _context = context;
        }
        public async Task<Medecin> GetByIdAsync(Guid id)
        {
            return await _context.Medecins
                .FirstOrDefaultAsync(m => m.Id == id);
        }
        public async Task<List<Medecin>> GetAllAsync()
        {
            return await _context.Medecins.ToListAsync();
        }
        public async Task AddAsync(Medecin medecin)
        {
            await _context.Medecins.AddAsync(medecin);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Medecin medecin)
        {
            _context.Medecins.Update(medecin);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var medecin = await GetByIdAsync(id);
            if (medecin != null)
            {
                _context.Medecins.Remove(medecin);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<Medecin>> FilterAsync(string specialite)
        {
            return await _context.Medecins
                .Where(m => (string.IsNullOrEmpty(specialite) || m.Specialite == specialite))
                .ToListAsync();
        }

    }
}
