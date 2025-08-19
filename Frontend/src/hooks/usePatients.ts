import { useState, useMemo } from "react";
import { toast } from "sonner";
import { useAuth } from "@/hooks/useAuth";
import { patientService } from "@/services/patientService";
import { Patient } from "@/types/patient";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";

interface UsePatientState {
  patients: Patient[];
  patient: Patient;
  filteredPatients: Patient[];
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
  handleAddPatient: (
    data: Omit<Patient, "id" | "dateCreation">
  ) => Promise<Patient>;
  handleUpdatePatient: (
    id: string,
    data: Partial<Omit<Patient, "id" | "dateCreation">>
  ) => Promise<void>;
  handleDeletePatient: (id: string) => Promise<void>;
  refetchPatients: () => void;
  fetchPatients: () => void;
  fetchPatientById: (id: string) => Promise<Patient | null>;
  linkUserToPatient: (userId: string, patientId: string) => Promise<void>;
}

export function usePatients(): UsePatientState {
  const { user } = useAuth();
  const queryClient = useQueryClient();

  const [patient, setPatient] = useState<Patient>();
  const [searchTerm, setSearchTerm] = useState("");

  // Définir les permissions en fonction du rôle
  const permissions = useMemo(() => {
    if (!user) {
      return {
        canCreate: false,
        canEdit: false,
        canDelete: false,
        canView: false,
      };
    }
    return {
      canCreate: user.role === "ClinicAdmin" || user.role === "Doctor",
      canEdit: user.role === "ClinicAdmin" || user.role === "Doctor",
      canDelete: user.role === "ClinicAdmin",
      canView:
        user.role === "SuperAdmin" ||
        user.role === "ClinicAdmin" ||
        user.role === "Doctor" ||
        user.role === "Patient",
    };
  }, [user]);

  // Fetch des patients avec React Query
  const {
    data: patients = [],
    isLoading,
    refetch: refetchPatients,
  } = useQuery<Patient[]>({
    queryKey: ["patients", user?.id],
    queryFn: async () => {
      const data = await patientService.getPatients();
      if (!user) return data;

      // Appliquer les restrictions
      if (user.role === "Patient" && user.patientId) {
        return data.filter((p) => p.id === user.patientId);
      } else if (user.role === "Doctor" || user.role === "ClinicAdmin") {
        // Si tu veux filtrer par clinique
        // return data.filter((p) => p.clinicId === user.cliniqueId);
        return data;
      }
      return data; // SuperAdmin voit tout
    },
    enabled: !!user, // seulement si l'utilisateur est défini
  });

  // Filtrage par recherche
  const filteredPatients = useMemo(() => {
    if (!patients) return [];
    if (!searchTerm) return patients;
    return patients.filter(
      (patient) =>
        `${patient.prenom} ${patient.nom}`
          .toLowerCase()
          .includes(searchTerm.toLowerCase()) ||
        patient.email.toLowerCase().includes(searchTerm.toLowerCase())
    );
  }, [patients, searchTerm]);

  const fetchPatientById = async (id: string): Promise<Patient | null> => {
    try {
      const fetchedPatient = await patientService.getPatientById(id);
      setPatient(fetchedPatient); // Assuming setDoctor updates some state
      return fetchedPatient;
    } catch (error) {
      console.error("Error fetching patient:", error);
      return null; // Return null instead of throwing to handle errors gracefully
    }
  };

  // --- Mutations ---
  const addMutation = useMutation({
    mutationFn: (data: Omit<Patient, "id" | "dateCreation">) =>
      patientService.createPatient(data),
    onSuccess: (newPatient) => {
      queryClient.setQueryData<Patient[]>(
        ["patients", user?.id],
        (old = []) => [...old, newPatient]
      );
      toast.success("Patient ajouté avec succès");
    },
    onError: () => {
      toast.error("Échec de l'ajout du patient");
    },
  });

  const updateMutation = useMutation({
    mutationFn: ({
      id,
      data,
    }: {
      id: string;
      data: Partial<Omit<Patient, "id" | "dateCreation">>;
    }) => patientService.updatePatient(id, data),
    onSuccess: (_, { id, data }) => {
      queryClient.setQueryData<Patient[]>(["patients", user?.id], (old = []) =>
        old.map((p) => (p.id === id ? { ...p, ...data } : p))
      );
      toast.success("Patient mis à jour avec succès");
    },
    onError: () => {
      toast.error("Échec de la mise à jour du patient");
    },
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => patientService.deletePatient(id),
    onSuccess: (_, id) => {
      queryClient.setQueryData<Patient[]>(["patients", user?.id], (old = []) =>
        old.filter((p) => p.id !== id)
      );
      toast.success("Patient supprimé avec succès");
    },
    onError: () => {
      toast.error("Échec de la suppression du patient");
    },
  });

  const linkMutation = useMutation({
    mutationFn: ({
      userId,
      patientId,
    }: {
      userId: string;
      patientId: string;
    }) => patientService.linkUserToPatient({ userId, patientId }),
    onSuccess: () => {
      toast.success("Utilisateur lié au patient avec succès");
    },
    onError: () => {
      toast.error("Échec de la liaison de l'utilisateur au patient");
    },
  });

  return {
    patients,
    patient,
    filteredPatients,
    isLoading,
    isSubmitting:
      addMutation.isPending ||
      updateMutation.isPending ||
      deleteMutation.isPending ||
      linkMutation.isPending,
    permissions,
    searchTerm,
    setSearchTerm,
    handleAddPatient: addMutation.mutateAsync,
    handleUpdatePatient: (id, data) => updateMutation.mutateAsync({ id, data }),
    handleDeletePatient: deleteMutation.mutateAsync,
    refetchPatients,
    fetchPatients: refetchPatients, // alias
    fetchPatientById,
    linkUserToPatient: (userId, patientId) =>
      linkMutation.mutateAsync({ userId, patientId }),
  };
}
