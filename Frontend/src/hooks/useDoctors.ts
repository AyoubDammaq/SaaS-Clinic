import { useState, useEffect, useCallback } from "react";
import { toast } from "sonner";
import { useAuth } from "@/hooks/useAuth";
import { doctorService } from "@/services/doctorService";
import { Doctor, DoctorDto } from "@/types/doctor";

interface UseDoctorsState {
  doctors: Doctor[];
  doctor: Doctor;
  filteredDoctors: Doctor[];
  isLoading: boolean;
  isSubmitting: boolean;
  permissions: {
    canCreate: boolean;
    canEdit: boolean;
    canDelete: boolean;
    canView: boolean;
  };
  searchTerm: string;
  setSearchTerm: (term: string) => void;
  addDoctor: (data: DoctorDto) => Promise<Doctor>;
  updateDoctor: (id: string, data: DoctorDto) => Promise<void>;
  deleteDoctor: (id: string) => Promise<void>;
  assignDoctorToClinic: (
    medecinId: string,
    cliniqueId: string
  ) => Promise<void>;
  unassignDoctorFromClinic: (medecinId: string) => Promise<void>;
  refetchDoctors: () => Promise<void>;
  fetchDoctors: () => Promise<void>;
  fetchDoctorById: (id: string) => Promise<Doctor | null>;
  setDoctors: React.Dispatch<React.SetStateAction<Doctor[]>>;
  linkUserToDoctor: (userId: string, doctorId: string) => Promise<void>;
}

export function useDoctors(): UseDoctorsState {
  const { user } = useAuth();
  const [doctors, setDoctors] = useState<Doctor[]>([]);
  const [filteredDoctors, setFilteredDoctors] = useState<Doctor[]>([]);
  const [doctor, setDoctor] = useState<Doctor>();
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [searchTerm, setSearchTerm] = useState("");
  const [permissions, setPermissions] = useState({
    canCreate: false,
    canEdit: false,
    canDelete: false,
    canView: false,
  });

  // Check permissions based on user role
  useEffect(() => {
    if (user) {
      const canCreate =
        user.role === "SuperAdmin" || user.role === "ClinicAdmin";
      const canEdit =
        user.role === "SuperAdmin" ||
        user.role === "ClinicAdmin" ||
        user.role === "Doctor";
      const canDelete = user.role === "SuperAdmin";
      const canView = true; // All authenticated users can view doctors

      setPermissions({ canCreate, canEdit, canDelete, canView });
    }
  }, [user]);

  // Fetch doctors list
  const fetchDoctors = useCallback(async () => {
    if (!user) return;

    setIsLoading(true);
    try {
      // Get all doctors
      const data = await doctorService.getDoctors();

      // Filter doctors based on user role
      let filteredData = [...data];

      if (user.role === "ClinicAdmin") {
        if (user.cliniqueId) {
          filteredData = data.filter(
            (d) => d.cliniqueId === null || d.cliniqueId === user.cliniqueId
          );
        } else {
          filteredData = data.filter((d) => d.cliniqueId === null); // fallback
        }
      } else if (user.role === "Doctor") {
        if (user.cliniqueId) {
          filteredData = data.filter((d) => d.cliniqueId === user.cliniqueId);
        }
      }

      // SuperAdmin can see all doctors
      // Patients can see all doctors too

      setDoctors(filteredData);
      setFilteredDoctors(filteredData);
    } catch (error) {
      console.error("Error fetching doctors:", error);
      toast.error("Failed to load doctors");
    } finally {
      setIsLoading(false);
    }
  }, [user]);

  // Filter doctors based on search term
  useEffect(() => {
    if (doctors.length === 0) return;

    const results = doctors.filter(
      (doctor) =>
        `${doctor.prenom} ${doctor.nom}`
          .toLowerCase()
          .includes(searchTerm.toLowerCase()) ||
        doctor.specialite.toLowerCase().includes(searchTerm.toLowerCase())
    );
    setFilteredDoctors(results);
  }, [searchTerm, doctors]);

  // Initial data loading
  useEffect(() => {
    fetchDoctors();
  }, [fetchDoctors]);

  // Add a new doctor
  const addDoctor = async (data: DoctorDto): Promise<Doctor> => {
    setIsSubmitting(true);
    try {
      const newDoctor = await doctorService.createDoctor(data);
      setDoctors((prev) => [...prev, newDoctor]);
      toast.success("Doctor added successfully");
      return newDoctor;
    } catch (error) {
      console.error("Error adding doctor:", error);
      toast.error("Failed to add doctor");
      throw error;
    } finally {
      setIsSubmitting(false);
    }
  };

  // Update an existing doctor
  const updateDoctor = async (id: string, data: DoctorDto) => {
    setIsSubmitting(true);
    try {
      const updatedDoctor = await doctorService.updateDoctor(id, data);
      setDoctors((prev) =>
        prev.map((doctor) => (doctor.id === id ? updatedDoctor : doctor))
      );
    } catch (error) {
      console.error("Error updating doctor:", error);
      throw error;
    } finally {
      setIsSubmitting(false);
    }
  };

  const fetchDoctorById = async (id: string): Promise<Doctor | null> => {
    try {
      const fetchedDoctor = await doctorService.getDoctorById(id);
      setDoctor(fetchedDoctor); // Assuming setDoctor updates some state
      return fetchedDoctor;
    } catch (error) {
      console.error("Error fetching doctor:", error);
      return null; // Return null instead of throwing to handle errors gracefully
    }
  };

  const refetchDoctors = async () => {
    try {
      const data = await doctorService.getDoctors();
      setDoctors(data);
      setFilteredDoctors(data);
    } catch (error) {
      console.error("Error refetching doctors:", error);
      toast.error("Failed to refresh doctors list.");
    }
  };

  // Delete a doctor
  const deleteDoctor = async (id: string) => {
    if (!id) {
      console.error("Invalid doctor ID:", id);
      toast.error("Failed to delete doctor: Invalid ID.");
      return;
    }

    setIsLoading(true);
    try {
      await doctorService.deleteDoctor(id);
      setDoctors((prev) => prev.filter((doctor) => doctor.id !== id));
      toast.success("Doctor deleted successfully");
    } catch (error) {
      console.error("Error deleting doctor:", error);
      toast.error("Failed to delete doctor. Please try again.");
    } finally {
      setIsLoading(false);
    }
  };

  // Assign doctor to clinic
  const assignDoctorToClinic = async (
    medecinId: string,
    cliniqueId: string
  ) => {
    setIsSubmitting(true);
    try {
      await doctorService.assignDoctorToClinic(medecinId, cliniqueId);
      toast.success("Médecin attribué à la clinique avec succès");
      await refetchDoctors();
    } catch (error) {
      console.error(
        "Erreur lors de l’attribution du médecin à la clinique :",
        error
      );
      toast.error("Échec de l’attribution du médecin");
      throw error;
    } finally {
      setIsSubmitting(false);
    }
  };

  // Unassign doctor from clinic
  const unassignDoctorFromClinic = async (medecinId: string) => {
    setIsSubmitting(true);
    try {
      await doctorService.unassignDoctorFromClinic(medecinId);
      toast.success("Médecin désabonné de la clinique avec succès");
      await refetchDoctors();
    } catch (error) {
      console.error(
        "Erreur lors du désabonnement du médecin de la clinique :",
        error
      );
      toast.error("Échec du désabonnement du médecin");
      throw error;
    } finally {
      setIsSubmitting(false);
    }
  };

  // Link user to doctor
  const linkUserToDoctor = async (userId: string, doctorId: string) => {
    setIsSubmitting(true);
    try {
      await doctorService.linkUserToDoctor({ userId, doctorId });
      toast.success("User linked to doctor successfully");
    } catch (error) {
      console.error("Error linking user to doctor:", error);
      toast.error("Failed to link user to doctor");
      throw error;
    } finally {
      setIsSubmitting(false);
    }
  };

  return {
    doctors,
    doctor,
    filteredDoctors,
    isLoading,
    isSubmitting,
    permissions,
    searchTerm,
    setSearchTerm,
    addDoctor,
    updateDoctor,
    deleteDoctor,
    assignDoctorToClinic,
    unassignDoctorFromClinic,
    refetchDoctors: fetchDoctors,
    fetchDoctors,
    fetchDoctorById,
    setDoctors,
    linkUserToDoctor,
  };
}
