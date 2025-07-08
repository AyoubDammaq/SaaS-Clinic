
import { useState, useEffect, useCallback } from 'react';
import { toast } from 'sonner';
import { useAuth } from "@/hooks/useAuth";
import { consultationService } from '@/services/consultationService';
import { Consultation, ConsultationDTO } from '@/types/consultation';

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
  refetchConsultations: () => Promise<void>;
}

export function useConsultations(): UseConsultationsState {
  const { user } = useAuth();
  const [consultations, setConsultations] = useState<Consultation[]>([]);
  const [filteredConsultations, setFilteredConsultations] = useState<Consultation[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
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
      const canView = true; // All authenticated users can view consultations they have access to
      
      setPermissions({ canCreate, canEdit, canDelete, canView });
    }
  }, [user]);

  // Fetch consultations list
  const fetchConsultations = useCallback(async () => {
    if (!user) return;
    
    setIsLoading(true);
    try {
      let consultationData: Consultation[] = [];
      
      // Fetch consultations based on user role
      if (user.role === 'Patient') {
        // Patients can only see their own consultations
        consultationData = await consultationService.getConsultationsByPatientId(user.id);
      } else if (user.role === 'Doctor') {
        // Doctors can see consultations they are assigned to
        consultationData = await consultationService.getConsultationsByDoctorId(user.id);
      } else if (user.role === 'ClinicAdmin' && user.cliniqueId) {
        // Clinic admins can see all consultations in their clinic
        // This would require an additional endpoint, for now we'll use getAllConsultations
        // and filter client-side
        const allConsultations = await consultationService.getAllConsultations();
        // We would need to filter by clinic, which might require additional data about doctors
        consultationData = allConsultations;
      } else {
        // SuperAdmin can see all consultations
        consultationData = await consultationService.getAllConsultations();
      }
      
      setConsultations(consultationData);
      setFilteredConsultations(consultationData);
    } catch (error) {
      console.error("Error fetching consultations:", error);
      toast.error("Failed to load consultations");
    } finally {
      setIsLoading(false);
    }
  }, [user]);
  
  // Filter consultations based on search term
  useEffect(() => {
    if (consultations.length === 0) return;
    
    const results = consultations.filter(consultation => 
      consultation.raison.toLowerCase().includes(searchTerm.toLowerCase()) ||
      consultation.dateConsultation.includes(searchTerm)
    );
    setFilteredConsultations(results);
  }, [searchTerm, consultations]);

  // Initial data loading
  useEffect(() => {
    fetchConsultations();
  }, [fetchConsultations]);

  // Add a new consultation
  const addConsultation = async (data: ConsultationDTO) => {
    setIsSubmitting(true);
    try {
      await consultationService.createConsultation(data);
      await fetchConsultations(); // Refetch to get the updated list with IDs
      toast.success("Consultation added successfully");
    } catch (error) {
      console.error("Error adding consultation:", error);
      toast.error("Failed to add consultation");
      throw error;
    } finally {
      setIsSubmitting(false);
    }
  };

  // Update an existing consultation
  const updateConsultation = async (data: ConsultationDTO) => {
    if (!data.id) {
      toast.error("Consultation ID is required for updates");
      return;
    }
    
    setIsSubmitting(true);
    try {
      await consultationService.updateConsultation(data);
      setConsultations(prev => 
        prev.map(consultation => consultation.id === data.id ? { ...consultation, ...data } : consultation)
      );
      toast.success("Consultation updated successfully");
    } catch (error) {
      console.error("Error updating consultation:", error);
      toast.error("Failed to update consultation");
      throw error;
    } finally {
      setIsSubmitting(false);
    }
  };

  // Delete a consultation
  const deleteConsultation = async (id: string) => {
    if (!window.confirm("Are you sure you want to delete this consultation?")) {
      return;
    }

    setIsLoading(true);
    try {
      await consultationService.deleteConsultation(id);
      setConsultations(prev => prev.filter(consultation => consultation.id !== id));
      toast.success("Consultation deleted successfully");
    } catch (error) {
      console.error("Error deleting consultation:", error);
      toast.error("Failed to delete consultation");
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
    refetchConsultations: fetchConsultations
  };
}
