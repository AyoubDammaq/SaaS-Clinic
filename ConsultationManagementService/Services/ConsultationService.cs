using ConsultationManagementService.DTOs;
using ConsultationManagementService.Models;
using ConsultationManagementService.Repositories;

namespace ConsultationManagementService.Services
{
    public class ConsultationService : IConsultationService 
    {
        public readonly IConsultationRepository _consultationRepository;

        public ConsultationService(IConsultationRepository consultationRepository)
        {
            _consultationRepository = consultationRepository;
        }

        public async Task<Consultation?> GetConsultationByIdAsync(Guid id)
        {
            return await _consultationRepository.GetConsultationByIdAsync(id);
        }

        public async Task<IEnumerable<Consultation>> GetAllConsultationsAsync()
        {
            return await _consultationRepository.GetAllConsultationsAsync();
        }

        public async Task CreateConsultationAsync(ConsultationDTO consultation)
        {
            await _consultationRepository.CreateConsultationAsync(consultation);
        }

        public async Task UpdateConsultationAsync(ConsultationDTO consultation)
        {
            await _consultationRepository.UpdateConsultationAsync(consultation);
        }

        public async Task<bool> DeleteConsultationAsync(Guid id)
        {
            return await _consultationRepository.DeleteConsultationAsync(id);
        }

        public async Task<DocumentMedical?> GetDocumentMedicalByIdAsync(Guid id)
        {
            return await _consultationRepository.GetDocumentMedicalByIdAsync(id);
        }

        public async Task UploadDocumentMedicalAsync(DocumentMedicalDTO documentMedical)
        {
            await _consultationRepository.UploadDocumentMedicalAsync(documentMedical);
        }

        public async Task<bool> DeleteDocumentMedicalAsync(Guid id)
        {
            return await _consultationRepository.DeleteDocumentMedicalAsync(id);
        }
    }
}
