
import { useState, useEffect, useCallback } from 'react';
import { toast } from 'sonner';
import { useAuth } from '@/contexts/AuthContext';
import { patientService } from '@/services/patientService';
import { Patient } from '@/types/patient';

interface UsePatientState {
  patients: Patient[];
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
  handleAddPatient: (data: Omit<Patient, 'id' | 'dateCreation'>) => Promise<Patient>;
  handleUpdatePatient: (id: string, data: Partial<Omit<Patient, 'id' | 'dateCreation'>>) => Promise<void>;
  handleDeletePatient: (id: string) => Promise<void>;
  refetchPatients: () => Promise<void>;
  linkUserToPatient: (userId: string, patientId: string) => Promise<void>;
}

export function usePatients(): UsePatientState {
  const { user } = useAuth();
  const [patients, setPatients] = useState<Patient[]>([]);
  const [filteredPatients, setFilteredPatients] = useState<Patient[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [permissions, setPermissions] = useState({
    canCreate: false,
    canEdit: false,
    canDelete: false,
    canView: false,
  });

  // Vérifier les permissions en fonction du rôle de l'utilisateur
  useEffect(() => {
    if (user) {
      const canCreate = user.role === 'ClinicAdmin' || user.role === 'Doctor';
      const canEdit = user.role === 'ClinicAdmin' || user.role === 'Doctor';
      const canDelete = user.role === 'ClinicAdmin';
      const canView = user.role === 'SuperAdmin' || user.role === 'ClinicAdmin' || user.role === 'Doctor' || user.role === 'Patient';
      
      setPermissions({ canCreate, canEdit, canDelete, canView });
    }
  }, [user]);

  // Récupérer la liste des patients
  const fetchPatients = useCallback(async () => {
    if (!user) return;
    
    setIsLoading(true);
    try {
      // Obtenir tous les patients
      const data = await patientService.getPatients();
      
      // Filtrer les patients en fonction du rôle de l'utilisateur
      let filteredData = [...data];
      
      if (user.role === 'Patient') {
        // Un patient ne peut voir que son propre dossier
        filteredData = data.filter(p => p.email === user.email);
      } else if (user.role === 'Doctor' || user.role === 'ClinicAdmin') {
        // Un médecin ou admin de clinique ne peut voir que les patients de sa clinique
        if (user.clinicId) {
          filteredData = data.filter(p => p.clinicId === user.clinicId);
        }
      }
      // Un SuperAdmin peut voir tous les patients
      
      setPatients(filteredData);
      setFilteredPatients(filteredData);
    } catch (error) {
      console.error("Erreur lors de la récupération des patients:", error);
      toast.error("Échec du chargement des patients");
    } finally {
      setIsLoading(false);
    }
  }, [user]);
  
  // Filtrer les patients en fonction du terme de recherche
  useEffect(() => {
    if (patients.length === 0) return;
    
    const results = patients.filter(patient => 
      `${patient.prenom} ${patient.nom}`.toLowerCase().includes(searchTerm.toLowerCase()) || 
      patient.email.toLowerCase().includes(searchTerm.toLowerCase())
    );
    setFilteredPatients(results);
  }, [searchTerm, patients]);

  // Chargement initial des données
  useEffect(() => {
    fetchPatients();
  }, [fetchPatients]);

  // Ajouter un nouveau patient
  const handleAddPatient = async (data: Omit<Patient, "id" | "dateCreation">): Promise<Patient>  => {
    setIsSubmitting(true);
    try {
      const newPatient = await patientService.createPatient(data);
      setPatients(prev => [...prev, newPatient]);
      toast.success("Patient ajouté avec succès");
      return newPatient;
    } catch (error) {
      console.error("Erreur lors de l'ajout du patient:", error);
      toast.error("Échec de l'ajout du patient");
      throw error;
    } finally {
      setIsSubmitting(false);
    }
  };

  // Mettre à jour un patient existant
  const handleUpdatePatient = async (id: string, data: Partial<Omit<Patient, "id" | "dateCreation">>) => {
    setIsSubmitting(true);
    try {
      await patientService.updatePatient(id, data);
      setPatients(prev => 
        prev.map(patient => patient.id === id ? { ...patient, ...data } : patient)
      );
      toast.success("Patient mis à jour avec succès");
    } catch (error) {
      console.error("Erreur lors de la mise à jour du patient:", error);
      toast.error("Échec de la mise à jour du patient");
      throw error;
    } finally {
      setIsSubmitting(false);
    }
  };

  // Supprimer un patient
  const handleDeletePatient = async (id: string) => {
    if (!window.confirm("Êtes-vous sûr de vouloir supprimer ce patient ?")) {
      return;
    }

    setIsLoading(true);
    try {
      await patientService.deletePatient(id);
      setPatients(prev => prev.filter(patient => patient.id !== id));
      toast.success("Patient supprimé avec succès");
    } catch (error) {
      console.error("Erreur lors de la suppression du patient:", error);
      toast.error("Échec de la suppression du patient");
      throw error;
    } finally {
      setIsLoading(false);
    }
  };

  // Lier un utilisateur à un patient
  const linkUserToPatient = async (userId: string, patientId: string) => {
    try {
      await patientService.linkUserToPatient({ userId, patientId });
      toast.success("Utilisateur lié au patient avec succès");
    } catch (error) {
      console.error("Erreur lors de la liaison de l'utilisateur au patient:", error);
      toast.error("Échec de la liaison de l'utilisateur au patient");
      throw error;
    }
  };

  return {
    patients,
    filteredPatients,
    isLoading,
    isSubmitting,
    permissions,
    searchTerm,
    setSearchTerm,
    handleAddPatient,
    handleUpdatePatient,
    handleDeletePatient,
    refetchPatients: fetchPatients,
    linkUserToPatient,
  };
}
