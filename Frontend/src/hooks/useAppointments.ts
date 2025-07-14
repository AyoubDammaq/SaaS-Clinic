import { useState, useEffect, useCallback } from "react";
import { toast } from "sonner";
import { useAuth } from "@/hooks/useAuth";
import { rendezVousService } from "@/services/rendezVousService";
import {
  RendezVous,
  CreateRendezVousRequest,
  UpdateRendezVousRequest,
  AppointmentStatusEnum,
} from "@/types/rendezvous";

interface UseAppointmentsState {
  appointments: RendezVous[];
  filteredAppointments: RendezVous[];
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
  handleAddAppointment: (data: CreateRendezVousRequest) => Promise<void>;
  handleUpdateAppointment: (
    id: string,
    data: UpdateRendezVousRequest
  ) => Promise<void>;
  handleCancelAppointment: (
    id: string,
    by: "patient" | "doctor",
    justification?: string
  ) => Promise<void>;
  handleCancelAppointmentByDoctor: (
    id: string,
    justification: string
  ) => Promise<void>;
  handleConfirmAppointment: (id: string) => Promise<void>;
  refetchAppointments: () => Promise<void>;
}

export function useAppointments(): UseAppointmentsState {
  const { user } = useAuth();

  const [appointments, setAppointments] = useState<RendezVous[]>([]);
  const [filteredAppointments, setFilteredAppointments] = useState<
    RendezVous[]
  >([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [searchTerm, setSearchTerm] = useState("");
  const [permissions, setPermissions] = useState({
    canCreate: false,
    canEdit: false,
    canDelete: false,
    canView: false,
  });

  // Définir les permissions selon le rôle
  useEffect(() => {
    if (user) {
      const role = user.role;
      setPermissions({
        canCreate: role === "Doctor" || role === "ClinicAdmin",
        canEdit: role === "Doctor",
        canDelete: role === "ClinicAdmin",
        canView: ["SuperAdmin", "ClinicAdmin", "Doctor", "Patient"].includes(
          role
        ),
      });
    }
  }, [user]);

  // Charger les rendez-vous selon le rôle
  const fetchAppointments = useCallback(async () => {
    if (!user) return;
    setIsLoading(true);

    try {
      let data: RendezVous[] = [];

      // if (user.role === "Doctor") {
      //   data = await rendezVousService.getByMedecin(user.id);
      // } else if (user.role === "Patient") {
      //   data = await rendezVousService.getByPatient(user.id);
      // } else if (user.role === "ClinicAdmin" && user.clinicId) {
      //   const all = await rendezVousService.getAll();
      //   // TODO: adapter ce filtre selon la structure réelle, par exemple filtrer par clinicId sur l'objet rendez-vous
      //   //data = all.filter(rdv => rdv.clinicId === user.clinicId);
      //   data = all; // Pour l'instant, on récupère tous les rendez-vous
      // } else {
      //   data = await rendezVousService.getAll();
      // }

      data = await rendezVousService.getAll();
      setAppointments(data);
      setFilteredAppointments(data);
    } catch (error) {
      console.error("Erreur lors du chargement des rendez-vous:", error);
      toast.error("Échec du chargement des rendez-vous");
    } finally {
      setIsLoading(false);
    }
  }, [user]);

  // Filtrer les rendez-vous en fonction du terme de recherche
  useEffect(() => {
    if (appointments.length === 0) {
      setFilteredAppointments([]);
      return;
    }

    const term = searchTerm.toLowerCase();

    const results = appointments.filter((rdv) => {
      const patientMatch =
        rdv.patientNom?.toLowerCase().includes(term) ?? false;
      const medecinMatch =
        rdv.medecinNom?.toLowerCase().includes(term) ?? false;
      const statutMatch = AppointmentStatusEnum[rdv.statut]
        ?.toLowerCase()
        .includes(term);

      return patientMatch || medecinMatch || statutMatch;
    });

    setFilteredAppointments(results);
  }, [searchTerm, appointments]);

  useEffect(() => {
    fetchAppointments();
  }, [fetchAppointments]);

  // Ajouter un rendez-vous
  const handleAddAppointment = async (data: CreateRendezVousRequest) => {
    setIsSubmitting(true);
    try {
      const newRdv = await rendezVousService.create(data);
      setAppointments((prev) => [...prev, newRdv]);
      toast.success("Rendez-vous ajouté avec succès");
    } catch (error) {
      console.error("Erreur lors de la création du rendez-vous:", error);
      toast.error("Échec de la création du rendez-vous");
    } finally {
      setIsSubmitting(false);
    }
  };

  // Mettre à jour un rendez-vous
  const handleUpdateAppointment = async (
    id: string,
    data: UpdateRendezVousRequest
  ) => {
    setIsSubmitting(true);
    try {
      const updated = await rendezVousService.update(id, data);
      setAppointments((prev) => prev.map((r) => (r.id === id ? updated : r)));
      toast.success("Rendez-vous mis à jour avec succès");
    } catch (error) {
      console.error("Erreur lors de la mise à jour du rendez-vous:", error);
      toast.error("Échec de la mise à jour");
    } finally {
      setIsSubmitting(false);
    }
  };

  // Annuler un rendez-vous (patient ou médecin)
  const handleCancelAppointment = async (
    id: string,
    by: "patient" | "doctor",
    justification?: string
  ) => {
    setIsSubmitting(true);
    try {
      if (by === "doctor" && justification) {
        await rendezVousService.annulerParMedecin(id, { justification });
      } else {
        await rendezVousService.annulerParPatient(id);
      }
      setAppointments((prev) =>
        prev.map((r) =>
          r.id === id ? { ...r, statut: AppointmentStatusEnum.ANNULE } : r
        )
      );
      toast.success("Rendez-vous annulé");
    } catch (error) {
      console.error("Erreur lors de l'annulation du rendez-vous:", error);
      toast.error("Échec de l’annulation du rendez-vous");
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleCancelAppointmentByDoctor = async (
    id: string,
    justification: string
  ) => {
    setIsSubmitting(true);
    try {
      await rendezVousService.annulerParMedecin(id, { justification });
      setAppointments((prev) =>
        prev.map((r) =>
          r.id === id ? { ...r, statut: AppointmentStatusEnum.ANNULE } : r
        )
      );
      toast.success("Rendez-vous annulé par le médecin");
    } catch (error) {
      console.error("Erreur lors de l'annulation du rendez-vous par le médecin:", error);
      toast.error("Échec de l’annulation du rendez-vous par le médecin");
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleConfirmAppointment = async (id: string) => {
    setIsSubmitting(true);
    try {
      await rendezVousService.confirmer(id);
      setAppointments((prev) =>
        prev.map((r) =>
          r.id === id ? { ...r, statut: AppointmentStatusEnum.CONFIRME } : r
        )
      );
      toast.success("Rendez-vous confirmé");
    } catch (error) {
      console.error("Erreur lors de la confirmation du rendez-vous:", error);
      toast.error("Échec de la confirmation du rendez-vous");
    }
  };

  return {
    appointments,
    filteredAppointments,
    isLoading,
    isSubmitting,
    permissions,
    searchTerm,
    setSearchTerm,
    handleAddAppointment,
    handleUpdateAppointment,
    handleCancelAppointment,
    handleCancelAppointmentByDoctor,
    handleConfirmAppointment,
    refetchAppointments: fetchAppointments,
  };
}
