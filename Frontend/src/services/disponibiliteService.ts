
import { API_ENDPOINTS } from '@/config/api';
import { api } from '@/utils/apiClient';
import { Disponibilite, DisponibiliteDto } from '@/types/disponibilite';

export const disponibiliteService = {
  // Add availability for a doctor
  async addDisponibilite(medecinId: string, disponibilite: DisponibiliteDto): Promise<void> {
    await api.post<void>(`${API_ENDPOINTS.DOCTORS.BASE}/${medecinId}/disponibilites`, disponibilite);
  },

  // Delete an availability
  async deleteDisponibilite(disponibiliteId: string): Promise<void> {
    await api.delete<void>(`${API_ENDPOINTS.DOCTORS.BASE}/disponibilites/${disponibiliteId}`);
  },

  // Get all availabilities for a doctor
  async getDisponibilitiesByDoctorId(medecinId: string): Promise<Disponibilite[]> {
    const response = await api.get<Disponibilite[]>(`${API_ENDPOINTS.DOCTORS.BASE}/disponibilites/${medecinId}`);
    return response;
  },

  // Get all availabilities
  async getAllDisponibilites(): Promise<Disponibilite[]> {
    const response = await api.get<Disponibilite[]>(`${API_ENDPOINTS.DOCTORS.BASE}/disponibilites`);
    return response;
  },

  // Get doctor availabilities by date
  async getDisponibilitiesByDoctorIdAndDate(medecinId: string, date: Date): Promise<Disponibilite[]> {
    const formattedDate = date.toISOString().split('T')[0];
    const response = await api.get<Disponibilite[]>(
      `${API_ENDPOINTS.DOCTORS.BASE}/disponibilites/${medecinId}/disponible?date=${formattedDate}`
    );
    return response;
  },

  // Get availabilities by date and time range
  async getDisponibilitesByDateAndTime(date: Date, heureDebut?: string, heureFin?: string): Promise<Disponibilite[]> {
    const formattedDate = date.toISOString().split('T')[0];
    let url = `${API_ENDPOINTS.DOCTORS.BASE}/disponibilites/${formattedDate}/medecin`;
    
    if (heureDebut || heureFin) {
      url += '?';
      if (heureDebut) url += `heureDebut=${heureDebut}&`;
      if (heureFin) url += `heureFin=${heureFin}`;
    }
    
    const response = await api.get<Disponibilite[]>(url);
    return response;
  }
};
