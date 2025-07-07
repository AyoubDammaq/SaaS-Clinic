// rendezVousService.ts
import { toast } from 'sonner';
import { API_ENDPOINTS } from '@/config/api';
import { api } from '@/utils/apiClient';
import { AnnulerMedecinRequest, CreateRendezVousRequest, RendezVous, UpdateRendezVousRequest } from '@/types/rendezvous';

export const rendezVousService = {
  // 📋 1. Get all rendez-vous
  getAll: async (): Promise<RendezVous[]> => {
    try {
      return await api.get<RendezVous[]>(API_ENDPOINTS.APPOINTMENTS.GET_ALL);
    } catch (error) {
      toast.error('Erreur lors du chargement des rendez-vous');
      throw error;
    }
  },

  // 🔍 2. Get rendez-vous by ID
  getById: async (id: string): Promise<RendezVous> => {
    try {
      return await api.get<RendezVous>(API_ENDPOINTS.APPOINTMENTS.GET_BY_ID(id));
    } catch (error) {
      toast.error('Erreur lors du chargement du rendez-vous');
      throw error;
    }
  },

  // 🆕 3. Create rendez-vous
  create: async (data: CreateRendezVousRequest): Promise<RendezVous> => {
    try {
      const response = await api.post<RendezVous>(API_ENDPOINTS.APPOINTMENTS.CREATE, data);
      toast.success('Rendez-vous créé avec succès');
      return response;
    } catch (error) {
      toast.error('Échec de la création du rendez-vous');
      throw error;
    }
  },

  // ✏️ 4. Update rendez-vous
  update: async (id: string, data: UpdateRendezVousRequest): Promise<RendezVous> => {
    try {
      const response = await api.put<RendezVous>(API_ENDPOINTS.APPOINTMENTS.UPDATE(id), data);
      toast.success('Rendez-vous mis à jour avec succès');
      return response;
    } catch (error) {
      toast.error('Échec de la mise à jour du rendez-vous');
      throw error;
    }
  },

  // ❌ 5. Cancel rendez-vous by patient (POST to /annuler/patient/{id})
  annulerParPatient: async (id: string): Promise<void> => {
    try {
      await api.post(API_ENDPOINTS.APPOINTMENTS.CANCEL_BY_PATIENT(id));
      toast.success('Rendez-vous annulé par le patient');
    } catch (error) {
      toast.error('Échec de l’annulation du rendez-vous');
      throw error;
    }
  },

  // ❌ 6. Cancel rendez-vous by doctor (with justification)
  annulerParMedecin: async (id: string, data: AnnulerMedecinRequest): Promise<void> => {
    try {
      await api.post(API_ENDPOINTS.APPOINTMENTS.CANCEL_BY_DOCTOR(id), data);
      toast.success('Rendez-vous annulé par le médecin');
    } catch (error) {
      toast.error('Échec de l’annulation du rendez-vous par le médecin');
      throw error;
    }
  },

  // ✅ 7. Confirm rendez-vous by doctor
  confirmer: async (id: string): Promise<void> => {
    try {
      await api.post(API_ENDPOINTS.APPOINTMENTS.CONFIRM_BY_DOCTOR(id));
      toast.success('Rendez-vous confirmé');
    } catch (error) {
      toast.error('Échec de la confirmation');
      throw error;
    }
  },

  // 📊 8. Get statistics for a period
  getStatsByPeriod: async (start: string, end: string): Promise<RendezVous[]> => {
    try {
      return await api.get<RendezVous[]>(API_ENDPOINTS.APPOINTMENTS.GET_STATS_PERIOD(start, end));
    } catch (error) {
      toast.error('Erreur lors de la récupération des statistiques');
      throw error;
    }
  },

  // 🔢 9. Count rendez-vous by medecin IDs
  getCountByMedecins: async (medecinIds: string[]): Promise<number> => {
    try {
      const query = medecinIds.map(id => `medecinIds=${id}`).join('&');
      return await api.get<number>(`${API_ENDPOINTS.APPOINTMENTS.BASE}/count?${query}`);
    } catch (error) {
      toast.error('Erreur lors du comptage des rendez-vous');
      throw error;
    }
  },

  // 🔢 10. Count distinct patients by medecin IDs
  getDistinctPatients: async (medecinIds: string[]): Promise<number> => {
    try {
      const query = medecinIds.map(id => `medecinIds=${id}`).join('&');
      return await api.get<number>(`${API_ENDPOINTS.APPOINTMENTS.BASE}/distinct/patients?${query}`);
    } catch (error) {
      toast.error('Erreur lors du comptage des patients');
      throw error;
    }
  },

  // 🔎 11. Filter by statut
  getByStatut: async (statut: string): Promise<RendezVous[]> => {
    try {
      return await api.get<RendezVous[]>(API_ENDPOINTS.APPOINTMENTS.GET_BY_STATUS(statut));
    } catch (error) {
      toast.error('Erreur lors du filtrage par statut');
      throw error;
    }
  },

  // 🔎 12. Filter by medecin ID
  getByMedecin: async (id: string): Promise<RendezVous[]> => {
    try {
      return await api.get<RendezVous[]>(API_ENDPOINTS.APPOINTMENTS.GET_BY_DOCTOR_ID(id));
    } catch (error) {
      toast.error('Erreur lors du filtrage par médecin');
      throw error;
    }
  },

  // 🔎 13. Filter by patient ID
  getByPatient: async (id: string): Promise<RendezVous[]> => {
    try {
      return await api.get<RendezVous[]>(API_ENDPOINTS.APPOINTMENTS.GET_BY_PATIENT_ID(id));
    } catch (error) {
      toast.error('Erreur lors du filtrage par patient');
      throw error;
    }
  },

  // 🔎 14. Filter by date (format yyyy-MM-dd)
  getByDate: async (date: string): Promise<RendezVous[]> => {
    try {
      return await api.get<RendezVous[]>(API_ENDPOINTS.APPOINTMENTS.GET_BY_DATE(date));
    } catch (error) {
      toast.error('Erreur lors du filtrage par date');
      throw error;
    }
  }
};

export default rendezVousService;
