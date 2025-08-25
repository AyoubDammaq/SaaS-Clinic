import { useState, useEffect, useCallback } from "react";
import { toast } from "sonner";
import { useAuth } from "@/hooks/useAuth";
import { disponibiliteService } from "@/services/disponibiliteService";
import { CreneauDisponibleDto, Disponibilite } from "@/types/disponibilite";
import { Doctor } from "@/types/doctor";

type UUID = string;

interface UseDisponibiliteState {
  disponibilites: Disponibilite[];
  isLoading: boolean;
  isSubmitting: boolean;
  permissions: {
    canCreate: boolean;
    canEdit: boolean;
    canDelete: boolean;
    canView: boolean;
  };
  addDisponibilite: (data: Omit<Disponibilite, "id">) => Promise<void>;
  updateDisponibilite: (
    disponibiliteId: UUID,
    data: Partial<Disponibilite>
  ) => Promise<void>;
  deleteDisponibilite: (disponibiliteId: UUID) => Promise<void>;
  deleteAvailabilitiesByDoctor: (doctorId: UUID) => Promise<void>;
  getAvailabilitiesByDoctor: (doctorId: UUID) => Promise<Disponibilite[]>;
  getDisponibiliteByDay: (
    doctorId: UUID,
    day: string
  ) => Promise<Disponibilite[]>;
  getAvailableDoctors: (
    date: string,
    startTime?: string,
    endTime?: string
  ) => Promise<Doctor[]>;
  getAvailableSlots: (
    doctorId: UUID,
    date: string
  ) => Promise<CreneauDisponibleDto[]>;
  isDoctorAvailable: (doctorId: UUID, dateTime: string) => Promise<boolean>;
  getTotalAvailableTime: (
    doctorId: UUID,
    start: string,
    end: string
  ) => Promise<number>;
  getAvailabilitiesInInterval: (
    doctorId: UUID,
    start: string,
    end: string
  ) => Promise<Disponibilite[]>;
}

export function useDisponibilite(): UseDisponibiliteState {
  const { user } = useAuth();
  const [disponibilites, setDisponibilites] = useState<Disponibilite[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const [permissions, setPermissions] = useState({
    canCreate: false,
    canEdit: false,
    canDelete: false,
    canView: false,
  });

  useEffect(() => {
    if (user) {
      const canCreate = ["ClinicAdmin"].includes(user.role);
      const canEdit = canCreate;
      const canDelete = canCreate;
      const canView = true;
      setPermissions({ canCreate, canEdit, canDelete, canView });
    }
  }, [user]);

  const addDisponibilite = async (
    data: Omit<Disponibilite, "id">
  ): Promise<void> => {
    console.log("Adding Disponibilite:", data);
    setIsSubmitting(true);
    try {
      await disponibiliteService.addDisponibilite(data);
      await getAvailabilitiesByDoctor(data.medecinId);
      toast.success("Disponibilite added successfully");
    } catch (error) {
      console.error("Error adding Disponibilite:", error);
      toast.error("Failed to add Disponibilite");
      throw error;
    } finally {
      setIsSubmitting(false);
    }
  };

  const updateDisponibilite = async (
    disponibiliteId: UUID,
    data: Partial<Disponibilite>
  ) => {
    setIsSubmitting(true);
    try {
      await disponibiliteService.updateDisponibilite(disponibiliteId, data);
      toast.success("Disponibilite updated successfully");
    } catch (error) {
      console.error("Error updating Disponibilite:", error);
      toast.error("Failed to update Disponibilite");
      throw error;
    } finally {
      setIsSubmitting(false);
    }
  };

  const deleteDisponibilite = async (disponibiliteId: UUID) => {
    setIsSubmitting(true);
    try {
      await disponibiliteService.deleteDisponibilite(disponibiliteId);
      toast.success("Disponibilite deleted successfully");
    } catch (error) {
      console.error("Error deleting Disponibilite:", error);
      toast.error("Failed to delete Disponibilite");
      throw error;
    } finally {
      setIsSubmitting(false);
    }
  };

  const deleteAvailabilitiesByDoctor = async (doctorId: UUID) => {
    setIsSubmitting(true);
    try {
      await disponibiliteService.deleteAvailabilitiesByDoctor(doctorId);
      toast.success("All doctor availabilities deleted");
    } catch (error) {
      console.error("Error deleting doctor availabilities:", error);
      toast.error("Failed to delete availabilities");
      throw error;
    } finally {
      setIsSubmitting(false);
    }
  };

  const getAvailabilitiesByDoctor = useCallback(
    async (doctorId: UUID): Promise<Disponibilite[]> => {
      setIsLoading(true);
      try {
        const result = await disponibiliteService.getAvailabilitiesByDoctor(
          doctorId
        );
        setDisponibilites(result);
        return result;
      } catch (error) {
        console.error("Error fetching availabilities:", error);
        toast.error("Failed to load availabilities");
        throw error;
      } finally {
        setIsLoading(false);
      }
    },
    []
  );

  const getDisponibiliteByDay = async (
    doctorId: UUID,
    day: string
  ): Promise<Disponibilite[]> => {
    setIsLoading(true);
    try {
      return await disponibiliteService.getDisponibiliteByDay(doctorId, day);
    } catch (error) {
      console.error("Error fetching availabilities by day:", error);
      toast.error("Failed to load availabilities for the selected day");
      throw error;
    } finally {
      setIsLoading(false);
    }
  };

  const getAvailableDoctors = async (
    date: string,
    heureDebut?: string,
    heureFin?: string
  ): Promise<Doctor[]> => {
    try {
      return await disponibiliteService.getAvailableDoctors(
        date,
        heureDebut,
        heureFin
      );
    } catch (error) {
      console.error("Error fetching available doctors:", error);
      toast.error("Failed to fetch available doctors");
      throw error;
    }
  };

  // Check if a doctor is available at a specific dateTime
  // This function checks if a doctor is available at a specific dateTime
  const getAvailableSlots = useCallback(
    async (doctorId: UUID, date: string): Promise<CreneauDisponibleDto[]> => {
      try {
        return await disponibiliteService.getAvailableSlots(doctorId, date);
      } catch (error) {
        console.error("Error fetching available slots:", error);
        toast.error("Failed to fetch available slots");
        throw error;
      }
    },
    []
  );

  const isDoctorAvailable = async (
    doctorId: UUID,
    dateTime: string
  ): Promise<boolean> => {
    try {
      return await disponibiliteService.isDoctorAvailable(doctorId, dateTime);
    } catch (error) {
      console.error("Error checking doctor Disponibilite:", error);
      toast.error("Failed to check doctor Disponibilite");
      throw error;
    }
  };

  const getTotalAvailableTime = async (
    doctorId: UUID,
    start: string,
    end: string
  ): Promise<number> => {
    try {
      return await disponibiliteService.getTotalAvailableTime(
        doctorId,
        start,
        end
      );
    } catch (error) {
      console.error("Error fetching total available time:", error);
      toast.error("Failed to get total available time");
      throw error;
    }
  };

  const getAvailabilitiesInInterval = async (
    doctorId: UUID,
    start: string,
    end: string
  ): Promise<Disponibilite[]> => {
    try {
      return await disponibiliteService.getAvailabilitiesInInterval(
        doctorId,
        start,
        end
      );
    } catch (error) {
      console.error("Error fetching availabilities in interval:", error);
      toast.error("Failed to get availabilities for the interval");
      throw error;
    }
  };

  return {
    disponibilites,
    isLoading,
    isSubmitting,
    permissions,
    addDisponibilite,
    updateDisponibilite,
    deleteDisponibilite,
    deleteAvailabilitiesByDoctor,
    getAvailabilitiesByDoctor,
    getDisponibiliteByDay,
    getAvailableDoctors,
    getAvailableSlots,
    isDoctorAvailable,
    getTotalAvailableTime,
    getAvailabilitiesInInterval,
  };
}
