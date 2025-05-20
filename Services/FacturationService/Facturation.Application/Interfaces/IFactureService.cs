using Facturation.Application.DTOs;
using Facturation.Domain.Entities;
using Facturation.Domain.Enums;
using Facturation.Domain.ValueObjects;

namespace Facturation.Application.Interfaces
{
    public interface IFactureService
    {
        Task<GetFacturesResponse> GetFactureByIdAsync(Guid id);
        Task<IEnumerable<GetFacturesResponse>> GetAllFacturesAsync();
        Task AddFactureAsync(CreateFactureRequest request);
        Task UpdateFactureAsync(UpdateFactureRequest request); 
        Task DeleteFactureAsync(Guid id);
        Task<IEnumerable<GetFacturesResponse>> GetAllFacturesByRangeOfDateAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<GetFacturesResponse>> GetAllFacturesByStateAsync(FactureStatus status);
        Task<IEnumerable<GetFacturesResponse>> GetAllFacturesByPatientIdAsync(Guid patientId);
        Task<IEnumerable<GetFacturesResponse>> GetAllFacturesByClinicIdAsync(Guid clinicId);
        Task<byte[]> ExportToPdfAsync(Facture facture);

        Task<IEnumerable<FactureStatsDTO>> GetNombreDeFactureByStatus();
        Task<IEnumerable<FactureStatsDTO>> GetNombreDeFactureParClinique();
        Task<IEnumerable<FactureStatsDTO>> GetNombreDeFacturesByStatusParClinique();
        Task<IEnumerable<FactureStatsDTO>> GetNombreDeFacturesByStatusDansUneClinique(Guid cliniqueId);
    }
}
