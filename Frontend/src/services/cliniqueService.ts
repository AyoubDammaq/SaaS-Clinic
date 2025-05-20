
import { API_ENDPOINTS } from '@/config/api';
import { api } from '@/utils/apiClient';
import { Clinique, StatistiqueCliniqueDTO, StatistiqueDTO, TypeClinique } from '@/types/clinic';
import { toast } from 'sonner';

export const cliniqueService = {

  // Get all clinics
  getAllCliniques: async (): Promise<Clinique[]> => {
    try {
      return await api.get<Clinique[]>(API_ENDPOINTS.CLINICS.GET_ALL);
    } catch (error) {
      console.error('Échec lors de la récupération des cliniques:', error);
      toast.error('Échec lors de la récupération des cliniques');
      throw error;
    }
  },

  // Get a clinic by ID
  getCliniqueById: async (id: string): Promise<Clinique | null> => {
    try {
      return await api.get<Clinique>(API_ENDPOINTS.CLINICS.GET_BY_ID(id));
    } catch (error) {
      console.error(`Échec lors de la récupération de la clinique ${id}:`, error);
      toast.error(`Échec lors de la récupération de la clinique`);
      throw error;
    }
  },

  // Add a new clinic
  addClinique: async (clinique: Omit<Clinique, 'id' | 'dateCreation'>): Promise<Clinique> => {
    try {
      return await api.post<Clinique>(API_ENDPOINTS.CLINICS.CREATE, clinique);
    } catch (error) {
      console.error('Échec lors de l\'ajout de la clinique:', error);
      toast.error('Échec lors de l\'ajout de la clinique');
      throw error;
    }
  },

  // Update a clinic
  updateClinique: async (id: string, clinique: Partial<Clinique>): Promise<void> => {
    try {
      await api.put(API_ENDPOINTS.CLINICS.UPDATE(id), clinique);
    } catch (error) {
      console.error(`Échec lors de la mise à jour de la clinique ${id}:`, error);
      toast.error(`Échec lors de la mise à jour de la clinique`);
      throw error;
    }
  },

  // Delete a clinic
  deleteClinique: async (id: string): Promise<void> => {
    try {
      await api.delete(API_ENDPOINTS.CLINICS.DELETE(id));
    } catch (error) {
      console.error(`Échec lors de la suppression de la clinique ${id}:`, error);
      toast.error(`Échec lors de la suppression de la clinique`);
      throw error;
    }
  },

  // Get statistics for a clinic
  getCliniqueStatistics: async (id: string): Promise<StatistiqueCliniqueDTO> => {
    try {
      return await api.get<StatistiqueCliniqueDTO>(`${API_ENDPOINTS.CLINICS.BASE}/statistiques/${id}`);
    } catch (error) {
      console.error(`Échec lors de la récupération des statistiques pour la clinique ${id}:`, error);
      toast.error(`Échec lors de la récupération des statistiques de la clinique`);
      throw error;
    }
  },

  // Filter clinics by type
  getCliniqueByType: async (type: TypeClinique): Promise<Clinique[]> => {
    try {
      return await api.get<Clinique[]>(API_ENDPOINTS.CLINICS.GET_BY_TYPE(String(type)));
    } catch (error) {
      console.error(`Échec lors de la récupération des cliniques de type ${type}:`, error);
      toast.error(`Échec lors de la récupération des cliniques de type ${type}`);
      throw error;
    }
  },

  // Filter clinics by name
  getCliniqueByName: async (name: string): Promise<Clinique[]> => {
    try {
      return await api.get<Clinique[]>(API_ENDPOINTS.CLINICS.GET_BY_NAME(name));
    } catch (error) {
      console.error(`Échec lors de la récupération des cliniques avec le nom ${name}:`, error);
      toast.error(`Échec lors de la récupération des cliniques avec le nom ${name}`);
      throw error;
    }
  },

  // Filter clinics by address
  getCliniqueByAddress: async (address: string): Promise<Clinique[]> => {
    try {
      return await api.get<Clinique[]>(API_ENDPOINTS.CLINICS.GET_BY_ADDRESS(address));
    } catch (error) {
      console.error(`Échec lors de la récupération des cliniques avec l'adresse ${address}:`, error);
      toast.error(`Échec lors de la récupération des cliniques avec l'adresse ${address}`);
      throw error;
    }
  },

  // Filter clinics by status
  getCliniqueByStatus: async (status: 'Active' | 'Inactive'): Promise<Clinique[]> => {
    try {
      return await api.get<Clinique[]>(API_ENDPOINTS.CLINICS.GET_BY_STATUS(status));
    } catch (error) {  
      console.error(`Échec lors de la récupération des cliniques avec le statut ${status}:`, error);
      toast.error(`Échec lors de la récupération des cliniques avec le statut ${status}`);
      throw error;
    }
  },

  // Get total count of clinics
  getTotalCliniqueCount: async (): Promise<number> => {
    try {
      return await api.get<number>(API_ENDPOINTS.CLINICS.GET_TOTAL_COUNT);
    } catch (error) {
      console.error('Échec lors de la récupération du nombre total de cliniques:', error);
      toast.error('Échec lors de la récupération du nombre total de cliniques');
      throw error;
    }
  },

    // Get new clinics for the current month
  getNewCliniqueThisMonth: async (): Promise<Clinique[]> => {
    try {
      return await api.get<Clinique[]>(API_ENDPOINTS.CLINICS.GET_NEW_THIS_MONTH);
    } catch (error) {
      console.error('Échec lors de la récupération des nouvelles cliniques du mois:', error);
      toast.error('Échec lors de la récupération des nouvelles cliniques du mois');
      throw error;
    }
  },

  // Get new clinics by month
  getNewCliniqueByMonth: async (): Promise<StatistiqueDTO[]> => {
    try {
      return await api.get<StatistiqueDTO[]>(API_ENDPOINTS.CLINICS.GET_NEW_BY_MONTH);
    } catch (error) {
      console.error('Échec lors de la récupération des nouvelles cliniques par mois:', error);
      toast.error('Échec lors de la récupération des nouvelles cliniques par mois');
      throw error;
    }
  },

  // Get statistics for a specific month
  // getCliniqueStatisticsByMonth: async (id: string, month: string): Promise<StatistiqueDTO[]> => {
  //   try {
  //     return await api.get<StatistiqueDTO[]>(`${API_ENDPOINTS.CLINICS.BASE}/statistiques/${id}/mois/${month}`);
  //   } catch (error) {
  //     console.error(`Échec lors de la récupération des statistiques pour la clinique ${id} pour le mois ${month}:`, error);
  //     toast.error(`Échec lors de la récupération des statistiques de la clinique pour le mois ${month}`);
  //     throw error;
  //   }
  // },



  // Get clinic statistics by month
  // getCliniqueStatisticsByMonth: async (id: string): Promise<StatistiqueDTO[]> => {
  //   try {
  //     return await api.get<StatistiqueDTO[]>(`${API_ENDPOINTS.CLINICS.BASE}/statistiques/${id}/mois`);
  //   } catch (error) {
  //     console.error(`Échec lors de la récupération des statistiques par mois pour la clinique ${id}:`, error);
  //     toast.error(`Échec lors de la récupération des statistiques par mois de la clinique`);
  //     throw error;
  //   }
  // }


  // Get clinic statistics by year
  // getCliniqueStatisticsByYear: async (id: string): Promise<StatistiqueDTO[]> => {
  //   try {
  //     return await api.get<StatistiqueDTO[]>(`${API_ENDPOINTS.CLINICS.BASE}/statistiques/${id}/annee`);
  //   } catch (error) {
  //     console.error(`Échec lors de la récupération des statistiques par année pour la clinique ${id}:`, error);
  //     toast.error(`Échec lors de la récupération des statistiques par année de la clinique`);
  //     throw error;
  //   }
  // }
};
