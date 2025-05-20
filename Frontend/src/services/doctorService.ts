
import { API_ENDPOINTS } from '@/config/api';
import { api } from '@/utils/apiClient';
import { 
  Doctor, 
  DoctorDto, 
  AttribuerMedecinDto, 
  SpecialiteStatistique, 
  CliniqueStatistique 
} from '@/types/doctor';

type UUID = string;

export const doctorService = {
  // Get all doctors
  async getDoctors(): Promise<Doctor[]> {
    const response = await api.get<Doctor[]>(API_ENDPOINTS.DOCTORS.GET_ALL);
    return response; // Return the response directly
  },

  // Get doctor by ID
  async getDoctorById(id: UUID): Promise<Doctor> {
    const response = await api.get<Doctor>(API_ENDPOINTS.DOCTORS.GET_BY_ID(id));
    return response; // Corrigez pour retourner response.data
  },
  
  // Add a new doctor
  async createDoctor(doctorData: DoctorDto): Promise<Doctor> {
    const response = await api.post<Doctor>(API_ENDPOINTS.DOCTORS.BASE, doctorData);
    return response; // Corrigez pour retourner response.data
  },

  // Update a doctor
  async updateDoctor(id: UUID, doctorData: DoctorDto): Promise<Doctor> {
    const response = await api.put<Doctor>(API_ENDPOINTS.DOCTORS.UPDATE(id), doctorData);
    return response; // Corrigez pour retourner response.data
  },

  // Delete a doctor
  async deleteDoctor(id: UUID): Promise<void> {
    try {
      await api.delete<void>(API_ENDPOINTS.DOCTORS.DELETE(id));
    } catch (error) {
      console.error("Error deleting doctor in backend:", error);
      throw error;
    }
  },

  // Filter doctors by speciality
  async filterDoctorsBySpeciality(specialite: string): Promise<Doctor[]> {
    const response = await api.get<Doctor[]>(`${API_ENDPOINTS.DOCTORS.BASE}/filter/specialite?specialite=${encodeURIComponent(specialite)}`);
    return response; // Corrigez pour retourner response.data
  },
  
  // Filter doctors by name or first name
  async filterDoctorsByName(nom?: string, prenom?: string): Promise<Doctor[]> {
    let url = `${API_ENDPOINTS.DOCTORS.BASE}/filter/name?`;
    if (nom) url += `name=${encodeURIComponent(nom)}&`;
    if (prenom) url += `prenom=${encodeURIComponent(prenom)}`;
    
    const response = await api.get<Doctor[]>(url);
    return response; // Corrigez pour retourner response.data
  },

  // Get doctors by clinic
  async getDoctorsByClinic(cliniqueId: UUID): Promise<Doctor[]> {
    const response = await api.get<Doctor[]>(`${API_ENDPOINTS.DOCTORS.BASE}/clinique/${cliniqueId}`);
    return response; // Corrigez pour retourner response.data
  },

  // Assign a doctor to a clinic
  async assignDoctorToClinic(medecinId: UUID, cliniqueId: UUID): Promise<void> {
    const data: AttribuerMedecinDto = { medecinId, cliniqueId };
    await api.post<void>(`${API_ENDPOINTS.DOCTORS.BASE}/attribuer`, data);
  },

  // Unassign a doctor from a clinic
  async unassignDoctorFromClinic(medecinId: UUID): Promise<void> {
    await api.delete<void>(`${API_ENDPOINTS.DOCTORS.BASE}/desabonner/${medecinId}`);
  },

  // Get doctor count by speciality
  async getDoctorCountBySpeciality(): Promise<SpecialiteStatistique[]> {
    const response = await api.get<SpecialiteStatistique[]>(`${API_ENDPOINTS.DOCTORS.BASE}/statistiques/specialite`);
    return response; // Corrigez pour retourner response.data
  },

  // Get doctor count by clinic
  async getDoctorCountByClinic(): Promise<CliniqueStatistique[]> {
    const response = await api.get<CliniqueStatistique[]>(`${API_ENDPOINTS.DOCTORS.BASE}/statistiques/clinique`);
    return response; // Corrigez pour retourner response.data
  },

  // Get doctor count by speciality in a clinic
  async getDoctorCountBySpecialityInClinic(cliniqueId: UUID): Promise<SpecialiteStatistique[]> {
    const response = await api.get<SpecialiteStatistique[]>(`${API_ENDPOINTS.DOCTORS.BASE}/statistiques/specialite/clinique/${cliniqueId}`);
    return response; // Corrigez pour retourner response.data
  },

  // Get doctor IDs by clinic
  async getDoctorIdsByClinic(cliniqueId: UUID): Promise<UUID[]> {
    const response = await api.get<UUID[]>(`${API_ENDPOINTS.DOCTORS.BASE}/medecinsIds/clinique/${cliniqueId}`);
    return response; // Corrigez pour retourner response.data
  },

  // Get doctor activities
  async getDoctorActivities(medecinId: UUID): Promise<unknown[]> { // Remplacez unknown[] par un type approprié si disponible
    const response = await api.get<unknown[]>(`${API_ENDPOINTS.DOCTORS.BASE}/activites/${medecinId}`);
    return response; // Corrigez pour retourner response.data
  }
};