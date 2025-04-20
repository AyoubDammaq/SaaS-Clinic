using Facturation.Application.DTOs;
using Facturation.Domain.Entities;
using Facturation.Domain.Enums;

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
    }
}
