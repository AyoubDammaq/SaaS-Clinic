import { useState, useEffect, useCallback } from "react";
import { toast } from "sonner";
import { useAuth } from "@/hooks/useAuth";
import { consultationService } from "@/services/consultationService";
import { billingService } from "@/services/billingService";
import {
  Consultation,
  ConsultationDTO,
  DocumentMedicalDTO,
} from "@/types/consultation";

interface UseConsultationsState {
  consultations: Consultation[];
  filteredConsultations: Consultation[];
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
  addConsultation: (data: ConsultationDTO) => Promise<void>;
  updateConsultation: (data: ConsultationDTO) => Promise<void>;
  deleteConsultation: (id: string) => Promise<void>;
  uploadDocumentMedical: (consultationId: string, file: File) => Promise<void>;
  deleteDocumentMedical: (id: string) => Promise<void>;
  refetchConsultations: () => Promise<void>;
}

export function useConsultations(): UseConsultationsState {
  const { user } = useAuth();
  const { id, role, patientId, medecinId, cliniqueId } = user || {};
  const [consultations, setConsultations] = useState<Consultation[]>([]);
  const [filteredConsultations, setFilteredConsultations] = useState<
    Consultation[]
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

  // 🔐 Définir les permissions utilisateur
  useEffect(() => {
    if (user) {
      const canCreate = user.role === "Doctor";
      const canEdit = user.role === "Doctor" || user.role === "ClinicAdmin";
      const canDelete = user.role === "Doctor" || user.role === "ClinicAdmin";
      const canView = true; // Tout le monde peut voir

      setPermissions({ canCreate, canEdit, canDelete, canView });
    }
  }, [user]);

  // 📥 Charger les consultations en fonction du rôle utilisateur
  const fetchConsultations = useCallback(async () => {
    if (!user) return;

    setIsLoading(true);
    try {
      let consultationData: Consultation[] = [];

      if (role === "Patient" && patientId) {
        consultationData = await consultationService.getConsultationsByPatientId(patientId);
      } else if (role === "Doctor" && medecinId) {
        consultationData = await consultationService.getConsultationsByDoctorId(medecinId);
      } else if (role === "ClinicAdmin" && cliniqueId) {
        consultationData = await consultationService.getConsultationsByClinicId(cliniqueId);
      } else {
        consultationData = await consultationService.getAllConsultations();
      }

      setConsultations(consultationData);
      setFilteredConsultations(consultationData);
    } catch (error) {
      console.error("Error fetching consultations:", error);
      toast.error("Échec du chargement des consultations");
    } finally {
      setIsLoading(false);
    }
  }, [user, role, patientId, medecinId, cliniqueId]);

  useEffect(() => {
    if (user) {
      fetchConsultations();
    }
  }, [user, fetchConsultations]);

  // 🔎 Filtrage en fonction du champ de recherche
  useEffect(() => {
    if (!consultations.length) return;

    const results = consultations.filter(
      (consultation) =>
        consultation.diagnostic
          .toLowerCase()
          .includes(searchTerm.toLowerCase()) ||
        consultation.dateConsultation.includes(searchTerm)
    );
    setFilteredConsultations(results);
  }, [searchTerm, consultations]);

  // ➕ Ajouter une consultation
  const addConsultation = async (data: ConsultationDTO) => {
    setIsSubmitting(true);
    try {
      await consultationService.createConsultation(data);
      await fetchConsultations();
      toast.success("Consultation ajoutée avec succès");
    } catch (error) {
      console.error("Erreur lors de l'ajout:", error);
      toast.error("Échec de l'ajout de la consultation");
      throw error;
    } finally {
      setIsSubmitting(false);
    }
  };

  // 📝 Modifier une consultation
  const updateConsultation = async (data: ConsultationDTO) => {
    setIsSubmitting(true);
    try {
      await consultationService.updateConsultation(data);
      await fetchConsultations();
      toast.success("Consultation mise à jour avec succès");
    } catch (error) {
      console.error("Erreur lors de la mise à jour:", error);
      toast.error("Échec de la mise à jour de la consultation");
      throw error;
    } finally {
      setIsSubmitting(false);
    }
  };

  // ❌ Supprimer une consultation
  const deleteConsultation = async (id: string) => {
    setIsLoading(true);
    try {
      await consultationService.deleteConsultation(id);
      const facture = await billingService.getFacturesByConsultationId(id);
      await billingService.deleteFacture(facture.id);
      setConsultations((prev) => prev.filter((c) => c.id !== id));
      toast.success("Consultation supprimée avec succès");
    } catch (error) {
      console.error("Erreur lors de la suppression:", error);
      toast.error("Échec de la suppression de la consultation");
      throw error;
    } finally {
      setIsLoading(false);
    }
  };

  const uploadDocumentMedical = async (consultationId: string, file: File) => {
    setIsSubmitting(true);
    try {
      const formData = new FormData();
      formData.append("consultationId", consultationId);
      formData.append("file", file);
      formData.append("type", file.type); // optionnel
      // fichierURL et dateAjout seront générés côté serveur

      await consultationService.uploadDocumentMedical(formData);

      toast.success("Document médical téléchargé avec succès");
    } catch (error) {
      console.error(
        "Erreur lors du téléchargement du document médical:",
        error
      );
      toast.error("Échec du téléchargement du document médical");
      throw error;
    } finally {
      setIsSubmitting(false);
    }
  };

  // 📄 Supprimer un document médical
  const deleteDocumentMedical = async (id: string) => {
    setIsLoading(true);
    try {
      await consultationService.deleteDocumentMedical(id);
      toast.success("Document médical supprimé avec succès");
    } catch (error) {
      console.error(
        "Erreur lors de la suppression du document médical:",
        error
      );
      toast.error("Échec de la suppression du document médical");
      throw error;
    } finally {
      setIsLoading(false);
    }
  };

  return {
    consultations,
    filteredConsultations,
    isLoading,
    isSubmitting,
    permissions,
    searchTerm,
    setSearchTerm,
    addConsultation,
    updateConsultation,
    deleteConsultation,
    uploadDocumentMedical,
    deleteDocumentMedical,
    refetchConsultations: fetchConsultations,
  };
}
