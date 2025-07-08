
import { useState, useEffect, useCallback } from 'react';
import { toast } from 'sonner';
import { useAuth } from '@/contexts/AuthContext';
import { Clinique, StatistiqueCliniqueDTO, TypeClinique } from '@/types/clinic';
import { cliniqueService } from '@/services/cliniqueService';

interface UseCliniquesState {
  cliniques: Clinique[];
  filteredCliniques: Clinique[];
  isLoading: boolean;
  isSubmitting: boolean;
  permissions: {
    canCreate: boolean;
    canEdit: boolean;
    canDelete: boolean;
    canView: boolean;
  };
  selectedClinique: Clinique | null;
  statistics: StatistiqueCliniqueDTO | null;
  isLoadingStats: boolean;
  searchTerm: string;
  setSearchTerm: (term: string) => void;
  setSelectedClinique: (clinique: Clinique | null) => void;
  handleAddClinique: (data: Omit<Clinique, 'id' | 'dateCreation'>) => Promise<Clinique>;
  handleUpdateClinique: (id: string, data: Partial<Clinique>) => Promise<void>;
  handleDeleteClinique: (id: string) => Promise<void>;
  fetchCliniqueStatistics: (id: string) => Promise<void>;
  filterCliniquesByType: (type: TypeClinique) => Promise<void>;
  filterCliniquesByStatus: (status: 'Active' | 'Inactive') => Promise<void>;
  filterCliniquesByName: (name: string) => Promise<void>;
  filterCliniquesByAddress: (address: string) => Promise<void>;
  getTotalCliniqueCount: () => Promise<void>;
  getNewCliniqueThisMonth: () => Promise<void>;
  getNewCliniqueByMonth: () => Promise<void>;
  refetchCliniques: () => Promise<void>;
  linkUserToClinique: (userId: string, clinicId: string) => Promise<void>;
}

export function useCliniques(): UseCliniquesState {
  const { user } = useAuth();
  const [cliniques, setCliniques] = useState<Clinique[]>([]);
  const [filteredCliniques, setFilteredCliniques] = useState<Clinique[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedClinique, setSelectedClinique] = useState<Clinique | null>(null);
  const [statistics, setStatistics] = useState<StatistiqueCliniqueDTO | null>(null);
  const [isLoadingStats, setIsLoadingStats] = useState(false);
  const [permissions, setPermissions] = useState({
    canCreate: false,
    canEdit: false,
    canDelete: false,
    canView: false,
  });

  // Vérifier les permissions en fonction du rôle de l'utilisateur
  useEffect(() => {
    if (user) {
      const canCreate = user.role === 'SuperAdmin' || user.role === 'ClinicAdmin';
      const canEdit = user.role === 'SuperAdmin' || user.role === 'ClinicAdmin';
      const canDelete = user.role === 'SuperAdmin';
      const canView = true; // Tous les utilisateurs peuvent voir les cliniques

      setPermissions({ canCreate, canEdit, canDelete, canView });
    }
  }, [user]);

  // Récupérer la liste des cliniques
  const fetchCliniques = useCallback(async () => {
    setIsLoading(true);
    try {
      const data = await cliniqueService.getAllCliniques();
      setCliniques(data);
      setFilteredCliniques(data);
    } catch (error) {
      console.error("Erreur lors de la récupération des cliniques:", error);
      toast.error("Échec du chargement des cliniques");
    } finally {
      setIsLoading(false);
    }
  }, []);

  // Filtrer les cliniques en fonction du terme de recherche
  useEffect(() => {
    if (cliniques.length === 0) return;
    
    const results = cliniques.filter(clinique => 
      clinique.nom.toLowerCase().includes(searchTerm.toLowerCase()) || 
      clinique.adresse.toLowerCase().includes(searchTerm.toLowerCase())
    );
    setFilteredCliniques(results);
  }, [searchTerm, cliniques]);

  // Charger les données initiales
  useEffect(() => {
    fetchCliniques();
  }, [fetchCliniques]);

  // Récupérer les statistiques d'une clinique
  const fetchCliniqueStatistics = async (id: string) => {
    setIsLoadingStats(true);
    try {
      const data = await cliniqueService.getCliniqueStatistics(id);
      setStatistics(data);
    } catch (error) {
      console.error("Erreur lors de la récupération des statistiques:", error);
      toast.error("Échec du chargement des statistiques");
    } finally {
      setIsLoadingStats(false);
    }
  };

  // Ajouter une nouvelle clinique
  const handleAddClinique = async (data: Omit<Clinique, 'id' | 'dateCreation'>) : Promise<Clinique> => {
    setIsSubmitting(true);
    try {
      const newClinique = await cliniqueService.addClinique(data);
      setCliniques(prev => [...prev, newClinique]);
      toast.success("Clinique ajoutée avec succès");
      return newClinique;
    } catch (error) {
      console.error("Erreur lors de l'ajout de la clinique:", error);
      toast.error("Échec de l'ajout de la clinique");
      throw error;
    } finally {
      setIsSubmitting(false);
    }
  };

  // Mettre à jour une clinique existante
  const handleUpdateClinique = async (id: string, data: Partial<Clinique>) => {
    setIsSubmitting(true);
    try {
      await cliniqueService.updateClinique(id, data);
      setCliniques(prev => 
        prev.map(clinique => clinique.id === id ? { ...clinique, ...data } : clinique)
      );
      toast.success("Clinique mise à jour avec succès");
    } catch (error) {
      console.error("Erreur lors de la mise à jour de la clinique:", error);
      toast.error("Échec de la mise à jour de la clinique");
      throw error;
    } finally {
      setIsSubmitting(false);
    }
  };

  // Supprimer une clinique
  const handleDeleteClinique = async (id: string) => {
    if (!window.confirm("Êtes-vous sûr de vouloir supprimer cette clinique ?")) {
      return;
    }

    setIsLoading(true);
    try {
      await cliniqueService.deleteClinique(id);
      setCliniques(prev => prev.filter(clinique => clinique.id !== id));
      toast.success("Clinique supprimée avec succès");
    } catch (error) {
      console.error("Erreur lors de la suppression de la clinique:", error);
      toast.error("Échec de la suppression de la clinique");
      throw error;
    } finally {
      setIsLoading(false);
    }
  };

  const filterCliniquesByType = async (type: TypeClinique) => {
    setIsLoading(true);
    try {
      const data = await cliniqueService.getCliniqueByType(type);
      setFilteredCliniques(data);
    } catch (error) {
      toast.error("Échec du filtrage des cliniques par type");
    } finally {
      setIsLoading(false);
    }
  };

  const filterCliniquesByStatus = async (status: 'Active' | 'Inactive') => {
    setIsLoading(true);
    try {
      const data = await cliniqueService.getCliniqueByStatus(status);
      setFilteredCliniques(data);
    } finally {
      setIsLoading(false);
    }
  };

  const filterCliniquesByName = async (name: string) => {
    setIsLoading(true);
    try {
      const data = await cliniqueService.getCliniqueByName(name);
      setFilteredCliniques(data);
    } catch (error) {
      toast.error("Échec du filtrage des cliniques par nom");
    } finally {
      setIsLoading(false);
    }
  };

  const filterCliniquesByAddress = async (address: string) => {
    setIsLoading(true);
    try {
      const data = await cliniqueService.getCliniqueByAddress(address);
      setFilteredCliniques(data);
    } catch (error) {
      toast.error("Échec du filtrage des cliniques par adresse");
    } finally {
      setIsLoading(false);
    }
  };

  const getTotalCliniqueCount = async () => {
    setIsLoading(true);
    try {
      const count = await cliniqueService.getTotalCliniqueCount();
      toast.success(`Nombre total de cliniques: ${count}`);
    } catch (error) {
      toast.error("Échec de la récupération du nombre total de cliniques");
    } finally {
      setIsLoading(false);
    }
  };

  const getNewCliniqueThisMonth = async () => {
    setIsLoading(true);
    try {
      const count = await cliniqueService.getNewCliniqueThisMonth();
      toast.success(`Nouvelles cliniques ce mois-ci: ${count}`);
    } catch (error) {
      toast.error("Échec de la récupération des nouvelles cliniques ce mois-ci");
    } finally {
      setIsLoading(false);
    }
  };

  const getNewCliniqueByMonth = async () => {
    setIsLoading(true);
    try {
      const data = await cliniqueService.getNewCliniqueByMonth();
      toast.success(`Nouvelles cliniques par mois: ${JSON.stringify(data)}`);
    } catch (error) {
      toast.error("Échec de la récupération des nouvelles cliniques par mois");
    } finally {
      setIsLoading(false);
    }
  };
  
  // Lier un utilisateur à une clinique
  const linkUserToClinique = async (userId: string, clinicId: string) => {
    try {
      await cliniqueService.linkUserToClinique({ userId, clinicId });
      toast.success("Utilisateur lié à la clinique avec succès");
    } catch (error) {
      console.error("Erreur lors de la liaison de l'utilisateur à la clinique:", error);
      toast.error("Échec de la liaison de l'utilisateur à la clinique");
      throw error;
    }
  };

  return {
    cliniques,
    filteredCliniques,
    isLoading,
    isSubmitting,
    permissions,
    selectedClinique,
    statistics,
    isLoadingStats,
    searchTerm,
    setSearchTerm,
    setSelectedClinique,
    handleAddClinique,
    handleUpdateClinique,
    handleDeleteClinique,
    fetchCliniqueStatistics,
    filterCliniquesByType,
    filterCliniquesByStatus,
    filterCliniquesByName,
    filterCliniquesByAddress,
    getTotalCliniqueCount,
    getNewCliniqueThisMonth,
    getNewCliniqueByMonth,
    refetchCliniques: fetchCliniques,
    linkUserToClinique,
  };
}
