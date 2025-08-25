import { toast } from "sonner";
import { API_ENDPOINTS } from "@/config/api";
import { api } from "@/utils/apiClient";
import { Document as PatientDocument } from "@/types/patient";

export interface MedicalRecord {
  id: string;
  allergies: string;
  chronicDiseases: string;
  currentMedications: string;
  familyHistory: string;
  personalHistory: string;
  bloodType: string;
  documents: Document[];
  creationDate: string;
  patientId: string;
}

export interface Document {
  id: string;
  name: string;
  type: string;
  url: string;
  uploadDate: string;
}

// Mock API functions - These would be replaced with real API calls
export const medicalRecordService = {
  // Get medical record by patient ID
  getMedicalRecord: async (patientId: string): Promise<MedicalRecord> => {
    try {
      // In production, this would use the real API
      // return await api.get<MedicalRecord>(API_ENDPOINTS.MEDICAL_RECORDS.GET_BY_PATIENT(patientId));

      // Simulate API call
      await new Promise((resolve) => setTimeout(resolve, 800));

      // Mock medical record data
      const mockMedicalRecord: MedicalRecord = {
        id: `mr-${patientId}`,
        allergies: "Penicillin, Pollen",
        chronicDiseases: "Asthma, Hypertension",
        currentMedications: "Ventolin (2 puffs daily), Lisinopril 10mg",
        familyHistory: "Father: Diabetes, Mother: Breast cancer",
        personalHistory: "Appendectomy (2018), Fractured arm (2015)",
        bloodType: "O+",
        documents: [
          {
            id: "d1",
            name: "Blood Test Results",
            type: "PDF",
            url: "#",
            uploadDate: "2025-03-15",
          },
          {
            id: "d2",
            name: "X-Ray Report",
            type: "Image",
            url: "#",
            uploadDate: "2025-02-20",
          },
        ],
        creationDate: "2023-05-10",
        patientId,
      };

      return mockMedicalRecord;
    } catch (error) {
      console.error("Failed to fetch medical record:", error);
      throw error;
    }
  },

  // Update medical record
  updateMedicalRecord: async (
    id: string,
    data: Partial<Omit<MedicalRecord, "id" | "patientId">>
  ): Promise<MedicalRecord> => {
    try {
      // In production, this would use the real API
      // return await api.put<MedicalRecord>(API_ENDPOINTS.MEDICAL_RECORDS.UPDATE(id), data);

      // Simulate API call
      await new Promise((resolve) => setTimeout(resolve, 800));

      // This would be replaced with a real API call
      return {
        id,
        allergies: data.allergies || "Penicillin, Pollen",
        chronicDiseases: data.chronicDiseases || "Asthma, Hypertension",
        currentMedications:
          data.currentMedications ||
          "Ventolin (2 puffs daily), Lisinopril 10mg",
        familyHistory:
          data.familyHistory || "Father: Diabetes, Mother: Breast cancer",
        personalHistory:
          data.personalHistory || "Appendectomy (2018), Fractured arm (2015)",
        bloodType: data.bloodType || "O+",
        documents: data.documents || [],
        creationDate: "2023-05-10",
        patientId: "patient-id",
      };
    } catch (error) {
      console.error("Failed to update medical record:", error);
      throw error;
    }
  },

  // Add a document to medical record
  addDocument: async (
    medicalRecordId: string,
    document: Omit<Document, "id">
  ): Promise<Document> => {
    try {
      // In production, this would use the real API
      // return await api.post<Document>(API_ENDPOINTS.MEDICAL_RECORDS.ADD_DOCUMENT(medicalRecordId), document);

      // Simulate API call
      await new Promise((resolve) => setTimeout(resolve, 500));

      const newId = Math.floor(Math.random() * 1000).toString();

      return {
        id: newId,
        ...document,
      };
    } catch (error) {
      console.error("Failed to add document:", error);
      throw error;
    }
  },

  // Delete a document from medical record
  deleteDocument: async (documentId: string): Promise<boolean> => {
    try {
      // In production, this would use the real API
      // await api.delete(API_ENDPOINTS.MEDICAL_RECORDS.DELETE_DOCUMENT(documentId));

      // Simulate API call
      await new Promise((resolve) => setTimeout(resolve, 500));

      return true;
    } catch (error) {
      console.error("Failed to delete document:", error);
      throw error;
    }
  },
};
