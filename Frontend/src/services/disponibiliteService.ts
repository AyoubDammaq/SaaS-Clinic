import { API_ENDPOINTS } from '@/config/api';
import { api } from '@/utils/apiClient';
import { Disponibilite } from '@/types/disponibilite';

type UUID = string;

export const disponibiliteService = {
  // Get all availabilities
  async getAllAvailabilities(): Promise<Disponibilite[]> {
    const response = await api.get<Disponibilite[]>(API_ENDPOINTS.Disponibilite.GET_ALL);
    return response;
  },

  // Get availabilities by doctor
  async getAvailabilitiesByDoctor(doctorId: UUID): Promise<Disponibilite[]> {
    const response = await api.get<Disponibilite[]>(API_ENDPOINTS.Disponibilite.GET_BY_DOCTOR(doctorId));
    return response;
  },

  // Add Disponibilite for a doctor
  async addDisponibilite(Disponibilite: Omit<Disponibilite, 'id'>): Promise<Disponibilite> {
    const response = await api.post<Disponibilite>(API_ENDPOINTS.Disponibilite.ADD(), Disponibilite);
    return response;
  },

  // Update an Disponibilite
  async updateDisponibilite(DisponibiliteId: UUID, updatedData: Partial<Disponibilite>): Promise<Disponibilite> {
    const response = await api.put<Disponibilite>(API_ENDPOINTS.Disponibilite.UPDATE(DisponibiliteId), updatedData);
    return response;
  },

  // Delete an Disponibilite
  async deleteDisponibilite(DisponibiliteId: UUID): Promise<void> {
    try {
      await api.delete<void>(API_ENDPOINTS.Disponibilite.DELETE(DisponibiliteId));
    } catch (error) {
      console.error("Error deleting Disponibilite:", error);
      throw error;
    }
  },

  // Delete all availabilities of a doctor
  async deleteAvailabilitiesByDoctor(doctorId: UUID): Promise<void> {
    try {
      await api.delete<void>(API_ENDPOINTS.Disponibilite.DELETE_BY_DOCTOR(doctorId));
    } catch (error) {
      console.error("Error deleting doctor availabilities:", error);
      throw error;
    }
  },

  // Get Disponibilite by doctor and day
  async getDisponibiliteByDay(doctorId: UUID, day: string): Promise<Disponibilite[]> {
    const response = await api.get<Disponibilite[]>(API_ENDPOINTS.Disponibilite.GET_BY_DAY(doctorId, day));
    return response;
  },

  // Get doctors available on a date (with optional time range)
  async getAvailableDoctors(date: string, heureDebut?: string, heureFin?: string): Promise<UUID[]> {
    const url = API_ENDPOINTS.Disponibilite.GET_AVAILABLE_DOCTORS(date, heureDebut, heureFin);
    const response = await api.get<UUID[]>(url);
    return response;
  },

  // Check if a doctor is available at a specific dateTime
  async isDoctorAvailable(doctorId: UUID, dateTime: string): Promise<boolean> {
    const response = await api.get<boolean>(API_ENDPOINTS.Disponibilite.IS_AVAILABLE(doctorId, dateTime));
    return response;
  },

  // Get total available time for a doctor between two dates
  async getTotalAvailableTime(doctorId: UUID, start: string, end: string): Promise<number> {
    const response = await api.get<number>(API_ENDPOINTS.Disponibilite.TOTAL_AVAILABLE_TIME(doctorId, start, end));
    return response;
  },

  // Get availabilities for a doctor in a specific interval
  async getAvailabilitiesInInterval(doctorId: UUID, start: string, end: string): Promise<Disponibilite[]> {
    const response = await api.get<Disponibilite[]>(API_ENDPOINTS.Disponibilite.GET_IN_INTERVAL(doctorId, start, end));
    return response;
  },
};
