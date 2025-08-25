import { API_ENDPOINTS } from "@/config/api";
import { api } from "@/utils/apiClient";
import {
  Consultation,
  ConsultationDTO,
  DocumentMedical,
  DocumentMedicalDTO,
} from "@/types/consultation";

export const consultationService = {
  // 🔍 Get consultation by ID
  async getConsultationById(id: string): Promise<Consultation> {
    const response = await api.get<Consultation>(
      API_ENDPOINTS.CONSULTATIONS.GET_BY_ID(id)
    );
    return response;
  },

  // 📄 Get all consultations
  async getAllConsultations(): Promise<Consultation[]> {
    const response = await api.get<Consultation[]>(
      API_ENDPOINTS.CONSULTATIONS.GET_ALL
    );
    return response;
  },

  // ➕ Create a new consultation
  async createConsultation(consultationData: ConsultationDTO): Promise<void> {
    await api.post<void>(API_ENDPOINTS.CONSULTATIONS.CREATE, consultationData);
  },

  // ✏️ Update a consultation
  async updateConsultation(consultationData: ConsultationDTO): Promise<void> {
    await api.put<void>(
      API_ENDPOINTS.CONSULTATIONS.UPDATE(consultationData.id!),
      consultationData
    );
  },

  // ❌ Delete a consultation
  async deleteConsultation(id: string): Promise<void> {
    await api.delete<void>(API_ENDPOINTS.CONSULTATIONS.DELETE(id));
  },

  // 👨‍⚕️ Get consultations by doctor ID
  async getConsultationsByDoctorId(doctorId: string): Promise<Consultation[]> {
    const response = await api.get<Consultation[]>(
      API_ENDPOINTS.CONSULTATIONS.GET_BY_DOCTOR_ID(doctorId)
    );
    return response;
  },

  // 🧑‍ Get consultations by patient ID
  async getConsultationsByPatientId(
    patientId: string
  ): Promise<Consultation[]> {
    const response = await api.get<Consultation[]>(
      API_ENDPOINTS.CONSULTATIONS.GET_BY_PATIENT_ID(patientId)
    );
    return response;
  },

  // 🏥 Get consultations by clinic ID
  async getConsultationsByClinicId(clinicId: string): Promise<Consultation[]> {
    const response = await api.get<Consultation[]>(
      API_ENDPOINTS.CONSULTATIONS.GET_BY_CLINIC_ID(clinicId)
    );
    return response;
  },

  // 📎 Get medical document by ID
  async getDocumentMedicalById(id: string): Promise<DocumentMedical> {
    const response = await api.get<DocumentMedical>(
      API_ENDPOINTS.CONSULTATIONS.GET_DOCUMENT_BY_ID(id)
    );
    return response;
  },

  // 📤 Upload a medical document
  async uploadDocumentMedical(formData: FormData): Promise<void> {
    await api.post<void>(API_ENDPOINTS.CONSULTATIONS.UPLOAD_DOCUMENT, formData);
  },

  // 🗑️ Delete a medical document
  async deleteDocumentMedical(id: string): Promise<void> {
    await api.delete<void>(API_ENDPOINTS.CONSULTATIONS.DELETE_DOCUMENT(id));
  },

  // 📊 Get number of consultations in a date range
  async getConsultationCount(startDate: Date, endDate: Date): Promise<number> {
    const start = startDate.toISOString();
    const end = endDate.toISOString();
    const response = await api.get<number>(
      API_ENDPOINTS.CONSULTATIONS.COUNT_BETWEEN_DATES(start, end)
    );
    return response;
  },

  // 📈 Get consultation count by doctor IDs
  async getConsultationCountByDoctorIds(doctorIds: string[]): Promise<number> {
    const response = await api.get<number>(
      API_ENDPOINTS.CONSULTATIONS.COUNT_BY_DOCTOR_IDS(doctorIds)
    );
    return response;
  },
};
