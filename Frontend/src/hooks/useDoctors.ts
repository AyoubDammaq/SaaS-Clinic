
import { useState, useEffect, useCallback } from 'react';
import { toast } from 'sonner';
import { useAuth } from '@/contexts/AuthContext';
import { doctorService } from '@/services/doctorService';
import { Doctor, DoctorDto } from '@/types/doctor';

interface UseDoctorsState {
  doctors: Doctor[];
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
  addDoctor: (data: DoctorDto) => Promise<void>;
  updateDoctor: (id: string, data: DoctorDto) => Promise<void>;
  deleteDoctor: (id: string) => Promise<void>;
  refetchDoctors: () => Promise<void>;
  fetchDoctors: () => Promise<void>; 
  setDoctors: React.Dispatch<React.SetStateAction<Doctor[]>>;
}

export function useDoctors(): UseDoctorsState {
  const { user } = useAuth();
  const [doctors, setDoctors] = useState<Doctor[]>([]);
  const [filteredDoctors, setFilteredDoctors] = useState<Doctor[]>([]);
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
      const canCreate = user.role === 'SuperAdmin' || user.role === 'ClinicAdmin';
      const canEdit = user.role === 'SuperAdmin' || user.role === 'ClinicAdmin' || user.role === 'Doctor';
      const canDelete = user.role === 'SuperAdmin';
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
      
      if (user.role === 'ClinicAdmin' || user.role === 'Doctor') {
        // Clinic admins and doctors can only see doctors in their clinic
        if (user.clinicId) {
          filteredData = data.filter(d => d.cliniqueId === user.clinicId);
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
    
    const results = doctors.filter(doctor => 
      `${doctor.prenom} ${doctor.nom}`.toLowerCase().includes(searchTerm.toLowerCase()) || 
      doctor.specialite.toLowerCase().includes(searchTerm.toLowerCase())
    );
    setFilteredDoctors(results);
  }, [searchTerm, doctors]);

  // Initial data loading
  useEffect(() => {
    fetchDoctors();
  }, [fetchDoctors]);

  // Add a new doctor
  const addDoctor = async (data: DoctorDto) => {
    setIsSubmitting(true);
    try {
      const newDoctor = await doctorService.createDoctor(data);
      setDoctors(prev => [...prev, newDoctor]);
      toast.success("Doctor added successfully");
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
      setDoctors(prev => prev.map(doctor => (doctor.id === id ? updatedDoctor : doctor)));
      toast.success("Doctor updated successfully");
    } catch (error) {
      console.error("Error updating doctor:", error);
      toast.error("Failed to update doctor");
      throw error;
    } finally {
      setIsSubmitting(false);
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

    if (!window.confirm("Are you sure you want to delete this doctor?")) {
      return;
    }

    setIsLoading(true);
    try {
      await doctorService.deleteDoctor(id);
      setDoctors(prev => prev.filter(doctor => doctor.id !== id));
      toast.success("Doctor deleted successfully");
    } catch (error) {
      console.error("Error deleting doctor:", error);
      toast.error("Failed to delete doctor. Please try again.");
    } finally {
      setIsLoading(false);
    }
  };

  return {
    doctors,
    filteredDoctors,
    isLoading,
    isSubmitting,
    permissions,
    searchTerm,
    setSearchTerm,
    addDoctor,
    updateDoctor,
    deleteDoctor,
    refetchDoctors: fetchDoctors,
    fetchDoctors,
    setDoctors,
  };
}
