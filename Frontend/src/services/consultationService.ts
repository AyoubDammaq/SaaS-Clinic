
import { API_ENDPOINTS } from '@/config/api';
import { api } from '@/utils/apiClient';
import { Consultation, ConsultationDTO, DocumentMedical, DocumentMedicalDTO } from '@/types/consultation';

export const consultationService = {
  // Get consultation by ID
  async getConsultationById(id: string): Promise<Consultation> {
    const response = await api.get<Consultation>(`${API_ENDPOINTS.CONSULTATIONS.GET_BY_ID(id)}`);
    return response;
  },

  // Get all consultations
  async getAllConsultations(): Promise<Consultation[]> {
    const response = await api.get<Consultation[]>(`${API_ENDPOINTS.CONSULTATIONS.GET_ALL}`);
    return response;
  },

  // Create a new consultation
  async createConsultation(consultationData: ConsultationDTO): Promise<void> {
    await api.post<void>(`${API_ENDPOINTS.CONSULTATIONS.CREATE}`, consultationData);
  },

  // Update a consultation
  async updateConsultation(consultationData: ConsultationDTO): Promise<void> {
    await api.put<void>(`${API_ENDPOINTS.CONSULTATIONS.UPDATE(consultationData.id!)}`, consultationData);
  },

  // Delete a consultation
  async deleteConsultation(id: string): Promise<void> {
    await api.delete<void>(`${API_ENDPOINTS.CONSULTATIONS.DELETE(id)}`);
  },

  // Get consultations by patient ID
  async getConsultationsByPatientId(patientId: string): Promise<Consultation[]> {
    const response = await api.get<Consultation[]>(`${API_ENDPOINTS.CONSULTATIONS.BASE}/Patient/${patientId}`);
    return response;
  },

  // Get consultations by doctor ID
  async getConsultationsByDoctorId(doctorId: string): Promise<Consultation[]> {
    const response = await api.get<Consultation[]>(`${API_ENDPOINTS.CONSULTATIONS.BASE}/Doctor/${doctorId}`);
    return response;
  },

  // Get medical document by ID
  async getDocumentMedicalById(id: string): Promise<DocumentMedical> {
    const response = await api.get<DocumentMedical>(`${API_ENDPOINTS.CONSULTATIONS.BASE}/Document/${id}`);
    return response;
  },

  // Upload a medical document
  async uploadDocumentMedical(documentData: DocumentMedicalDTO): Promise<void> {
    await api.post<void>(`${API_ENDPOINTS.CONSULTATIONS.BASE}/Document`, documentData);
  },

  // Delete a medical document
  async deleteDocumentMedical(id: string): Promise<void> {
    await api.delete<void>(`${API_ENDPOINTS.CONSULTATIONS.BASE}/Document/${id}`);
  },

  // Get number of consultations in a date range
  async getConsultationCount(startDate: Date, endDate: Date): Promise<number> {
    const start = startDate.toISOString().split('T')[0];
    const end = endDate.toISOString().split('T')[0];
    const response = await api.get<number>(`${API_ENDPOINTS.CONSULTATIONS.BASE}/count?start=${start}&end=${end}`);
    return response;
  },

  // Get consultation count by doctor IDs
  async getConsultationCountByDoctorIds(doctorIds: string[]): Promise<number> {
    const queryString = doctorIds.map(id => `medecinIds=${id}`).join('&');
    const response = await api.get<number>(`${API_ENDPOINTS.CONSULTATIONS.BASE}/countByMedecinIds?${queryString}`);
    return response;
  }
};
