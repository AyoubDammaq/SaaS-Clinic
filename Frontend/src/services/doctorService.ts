import { API_ENDPOINTS } from "@/config/api";
import { api } from "@/utils/apiClient";
import {
  Doctor,
  DoctorDto,
  AttribuerMedecinDto,
  SpecialiteStatistique,
  CliniqueStatistique,
  LinkUserToDoctorDto,
} from "@/types/doctor";

type UUID = string;

export const doctorService = {
  // Get all doctors
  async getDoctors(): Promise<Doctor[]> {
    const response = await api.get<Doctor[]>(API_ENDPOINTS.DOCTORS.GET_ALL);
    return response;
  },

  // Get doctor by ID
  async getDoctorById(id: UUID): Promise<Doctor> {
    const response = await api.get<Doctor>(API_ENDPOINTS.DOCTORS.GET_BY_ID(id));
    return response;
  },

  // Add a new doctor
  async createDoctor(doctorData: DoctorDto): Promise<Doctor> {
    const response = await api.post<Doctor>(
      API_ENDPOINTS.DOCTORS.CREATE,
      doctorData
    );
    return response;
  },

  // Update a doctor
  async updateDoctor(id: UUID, doctorData: DoctorDto): Promise<Doctor> {
    const response = await api.put<Doctor>(
      API_ENDPOINTS.DOCTORS.UPDATE(id),
      doctorData
    );
    return response;
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

  // Filter doctors by specialty
  async filterDoctorsBySpeciality(specialite: string): Promise<Doctor[]> {
    const response = await api.get<Doctor[]>(
      API_ENDPOINTS.DOCTORS.FILTER_BY_SPECIALTY(specialite)
    );
    return response;
  },

  // Filter doctors by name or first name
  async filterDoctorsByName(nom?: string, prenom?: string): Promise<Doctor[]> {
    const response = await api.get<Doctor[]>(
      API_ENDPOINTS.DOCTORS.FILTER_BY_NAME(nom, prenom)
    );
    return response;
  },

  // Get doctors by clinic
  async getDoctorsByClinic(cliniqueId: UUID): Promise<Doctor[]> {
    const response = await api.get<Doctor[]>(
      API_ENDPOINTS.DOCTORS.BY_CLINIC(cliniqueId)
    );
    return response;
  },

  // Assign a doctor to a clinic
  async assignDoctorToClinic(medecinId: UUID, cliniqueId: UUID): Promise<void> {
    const data: AttribuerMedecinDto = { medecinId, cliniqueId };
    await api.post<void>(API_ENDPOINTS.DOCTORS.ATTRIBUER, data);
  },

  // Unassign a doctor from a clinic
  async unassignDoctorFromClinic(medecinId: UUID): Promise<void> {
    await api.post<void>(API_ENDPOINTS.DOCTORS.DESABONNER(medecinId));
  },

  // Get doctor count by speciality
  async getDoctorCountBySpeciality(): Promise<SpecialiteStatistique[]> {
    const response = await api.get<SpecialiteStatistique[]>(
      API_ENDPOINTS.DOCTORS.STATS_BY_SPECIALTY
    );
    return response;
  },

  // Get doctor count by clinic
  async getDoctorCountByClinic(): Promise<CliniqueStatistique[]> {
    const response = await api.get<CliniqueStatistique[]>(
      API_ENDPOINTS.DOCTORS.STATS_BY_CLINIC
    );
    return response;
  },

  // Get doctor count by speciality in a clinic
  async getDoctorCountBySpecialityInClinic(
    cliniqueId: UUID
  ): Promise<SpecialiteStatistique[]> {
    const response = await api.get<SpecialiteStatistique[]>(
      API_ENDPOINTS.DOCTORS.STATS_BY_SPECIALTY_IN_CLINIC(cliniqueId)
    );
    return response;
  },

  // Get doctor IDs by clinic
  async getDoctorIdsByClinic(cliniqueId: UUID): Promise<UUID[]> {
    const response = await api.get<UUID[]>(
      API_ENDPOINTS.DOCTORS.IDS_BY_CLINIQUE(cliniqueId)
    );
    return response;
  },

  // Get doctor activities
  async getDoctorActivities(medecinId: UUID): Promise<unknown[]> {
    const response = await api.get<unknown[]>(
      API_ENDPOINTS.DOCTORS.ACTIVITES(medecinId)
    );
    return response;
  },

  // Link user to doctor
  async linkUserToDoctor({
    userId,
    doctorId,
  }: LinkUserToDoctorDto): Promise<void> {
    const data = { userId, doctorId };
    await api.post<void>(API_ENDPOINTS.DOCTORS.LINK_USER, data);
    console.log("[doctorService] User linked to doctor successfully:", data);
  },
};
