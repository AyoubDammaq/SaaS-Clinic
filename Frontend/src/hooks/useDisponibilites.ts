
import { useState, useEffect, useCallback } from 'react';
import { toast } from 'sonner';
import { useAuth } from '@/contexts/AuthContext';
import { disponibiliteService } from '@/services/disponibiliteService';
import { Disponibilite, DisponibiliteDto } from '@/types/disponibilite';

interface UseDisponibilitesState {
  disponibilites: Disponibilite[];
  isLoading: boolean;
  isSubmitting: boolean;
  permissions: {
    canCreate: boolean;
    canEdit: boolean;
    canDelete: boolean;
    canView: boolean;
  };
  addDisponibilite: (doctorId: string, data: DisponibiliteDto) => Promise<void>;
  deleteDisponibilite: (disponibiliteId: string) => Promise<void>;
  getDisponibilitesByDoctorId: (doctorId: string) => Promise<Disponibilite[]>;
  getDisponibilitesByDoctorIdAndDate: (doctorId: string, date: Date) => Promise<Disponibilite[]>;
}

export function useDisponibilites(): UseDisponibilitesState {
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

  // Check permissions based on user role
  useEffect(() => {
    if (user) {
      const canCreate = user.role === 'SuperAdmin' || user.role === 'ClinicAdmin' || user.role === 'Doctor';
      const canEdit = user.role === 'SuperAdmin' || user.role === 'ClinicAdmin' || user.role === 'Doctor';
      const canDelete = user.role === 'SuperAdmin' || user.role === 'ClinicAdmin' || user.role === 'Doctor';
      const canView = true; // All authenticated users can view disponibilites
      
      setPermissions({ canCreate, canEdit, canDelete, canView });
    }
  }, [user]);

  // Add a new disponibilite
  const addDisponibilite = async (doctorId: string, data: DisponibiliteDto) => {
    setIsSubmitting(true);
    try {
      await disponibiliteService.addDisponibilite(doctorId, data);
      toast.success("Availability added successfully");
    } catch (error) {
      console.error("Error adding availability:", error);
      toast.error("Failed to add availability");
      throw error;
    } finally {
      setIsSubmitting(false);
    }
  };

  // Delete a disponibilite
  const deleteDisponibilite = async (disponibiliteId: string) => {
    setIsSubmitting(true);
    try {
      await disponibiliteService.deleteDisponibilite(disponibiliteId);
      toast.success("Availability deleted successfully");
    } catch (error) {
      console.error("Error deleting availability:", error);
      toast.error("Failed to delete availability");
      throw error;
    } finally {
      setIsSubmitting(false);
    }
  };

  // Get disponibilites by doctor ID
  const getDisponibilitesByDoctorId = async (doctorId: string): Promise<Disponibilite[]> => {
    setIsLoading(true);
    try {
      const disponibilites = await disponibiliteService.getDisponibilitiesByDoctorId(doctorId);
      setDisponibilites(disponibilites);
      return disponibilites;
    } catch (error) {
      console.error("Error fetching availabilities:", error);
      toast.error("Failed to load availabilities");
      throw error;
    } finally {
      setIsLoading(false);
    }
  };

  // Get disponibilites by doctor ID and date
  const getDisponibilitesByDoctorIdAndDate = async (doctorId: string, date: Date): Promise<Disponibilite[]> => {
    setIsLoading(true);
    try {
      const disponibilites = await disponibiliteService.getDisponibilitiesByDoctorIdAndDate(doctorId, date);
      return disponibilites;
    } catch (error) {
      console.error("Error fetching availabilities by date:", error);
      toast.error("Failed to load availabilities for the selected date");
      throw error;
    } finally {
      setIsLoading(false);
    }
  };

  return {
    disponibilites,
    isLoading,
    isSubmitting,
    permissions,
    addDisponibilite,
    deleteDisponibilite,
    getDisponibilitesByDoctorId,
    getDisponibilitesByDoctorIdAndDate
  };
}
