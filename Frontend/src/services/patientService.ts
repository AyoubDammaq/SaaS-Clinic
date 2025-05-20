import { toast } from 'sonner';
import { API_ENDPOINTS } from '@/config/api';
import { api } from '@/utils/apiClient';
import { Patient, PatientDTO, DossierMedical, DossierMedicalDTO, Document, PatientStatistique } from '@/types/patient';
import { ApiResponse } from '@/types/response';

export const patientService = {
  // Get all patients
  getPatients: async (): Promise<Patient[]> => {
    try {
      const response = await api.get<Patient[]>(API_ENDPOINTS.PATIENTS.GET_ALL);
      return response;
    } catch (error) {
      console.error('Failed to fetch patients:', error);
      toast.error('Failed to fetch patients');
      throw error;
    }
  },

  // Get a patient by ID
  getPatientById: async (id: string): Promise<Patient | null> => {
    try {
      const response = await api.get<Patient>(API_ENDPOINTS.PATIENTS.GET_BY_ID(id));
      return response;
    } catch (error) {
      console.error(`Failed to fetch patient with ID ${id}:`, error);
      toast.error('Failed to fetch patient');
      throw error;
    }
  },

  // Search patients by name or first name
  searchPatientsByName: async (nom?: string, prenom?: string): Promise<Patient[]> => {
    try {
      const query = new URLSearchParams();
      if (nom) query.append('name', nom);
      if (prenom) query.append('lastname', prenom);

      const response = await api.get<Patient[]>(`${API_ENDPOINTS.PATIENTS.BASE}/search?${query.toString()}`);
      return response;
    } catch (error) {
      console.error('Failed to search patients:', error);
      toast.error('Failed to search patients');
      throw error;
    }
  },

  // Create a new patient
  createPatient: async (patient: Omit<Patient, 'id' | 'dateCreation'>): Promise<Patient> => {
    try {
      const response = await api.post<Patient>(API_ENDPOINTS.PATIENTS.CREATE, patient);
      toast.success('Patient created successfully');
      return response;
    } catch (error) {
      console.error('Failed to create patient:', error);
      toast.error('Failed to create patient');
      throw error;
    }
  },

  // Update an existing patient
  updatePatient: async (id: string, patient: Partial<Patient>): Promise<void> => {
    try {
      await api.put(API_ENDPOINTS.PATIENTS.UPDATE(id), patient);
      toast.success('Patient updated successfully');
    } catch (error) {
      console.error(`Failed to update patient with ID ${id}:`, error);
      toast.error('Failed to update patient');
      throw error;
    }
  },

  // Delete a patient
  deletePatient: async (id: string): Promise<void> => {
    try {
      await api.delete(API_ENDPOINTS.PATIENTS.DELETE(id));
      toast.success('Patient deleted successfully');
    } catch (error) {
      console.error(`Failed to delete patient with ID ${id}:`, error);
      toast.error('Failed to delete patient');
      throw error;
    }
  },
};

export const dossierMedicalService = {
  // Get medical record by patient ID
  getDossierMedicalByPatientId: async (patientId: string): Promise<ApiResponse<DossierMedical> | null> => {
    try {
      const response = await api.get<ApiResponse<DossierMedical>>(API_ENDPOINTS.MEDICAL_RECORDS.GET_BY_PATIENT(patientId));
      return response; 
    } catch (error) {
      console.error(`Failed to fetch medical record for patient ID ${patientId}:`, error);
      toast.error('Failed to fetch medical record');
      return null;
    }
  },

  // Create a new medical record
  createDossierMedical: async (dossier: DossierMedicalDTO): Promise<DossierMedicalDTO> => {
    try {
      const response = await api.post<DossierMedicalDTO>(API_ENDPOINTS.MEDICAL_RECORDS.BASE, dossier);
      toast.success('Medical record created successfully');
      return response;
    } catch (error) {
      console.error('Failed to create medical record:', error);
      toast.error('Failed to create medical record');
      throw error;
    }
  },

  // Update an existing medical record
  updateDossierMedical: async (dossier: DossierMedicalDTO): Promise<void> => {
    try {
      await api.put(API_ENDPOINTS.MEDICAL_RECORDS.UPDATE(), dossier);
      toast.success('Medical record updated successfully');
    } catch (error) {
      console.error('Failed to update medical record:', error);
      toast.error('Failed to update medical record');
      throw error;
    }
  },

  // Add a document to a medical record
  addDocumentToDossier: async (dossierId: string, document: Omit<Document, 'id'>): Promise<Document> => {
    try {
      const response = await api.post<Document>(API_ENDPOINTS.MEDICAL_RECORDS.ADD_DOCUMENT(dossierId), document);
      toast.success('Document added successfully');
      return response;
    } catch (error) {
      console.error(`Failed to add document to medical record ${dossierId}:`, error);
      toast.error('Failed to add document');
      throw error;
    }
  },

  // Delete a document from a medical record
  deleteDocument: async (documentId: string): Promise<void> => {
    try {
      await api.delete(API_ENDPOINTS.MEDICAL_RECORDS.DELETE_DOCUMENT(documentId));
      toast.success('Document deleted successfully');
    } catch (error) {
      console.error(`Failed to delete document with ID ${documentId}:`, error);
      toast.error('Failed to delete document');
      throw error;
    }
  },
};

export const patientStatistiqueService = {
  // Get patient statistics for a given period
  getPatientStatistiques: async (dateDebut: string, dateFin: string): Promise<PatientStatistique> => {
    try {
      const response = await api.get<PatientStatistique>(
        `${API_ENDPOINTS.PATIENTS.BASE}/statistiques?dateDebut=${dateDebut}&dateFin=${dateFin}`
      );
      return response;
    } catch (error) {
      console.error('Failed to fetch patient statistics:', error);
      toast.error('Failed to fetch patient statistics');
      throw error;
    }
  },
};