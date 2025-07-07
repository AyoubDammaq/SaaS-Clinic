
import { useState, useEffect, useCallback } from 'react';
import { dossierMedicalService } from '@/services/patientService';
import { DossierMedical, DossierMedicalDTO, Document, CreateDocumentRequest } from '@/types/patient';
import { toast } from 'sonner';
import { ApiResponse } from '@/types/response';
import { MedicalRecordDocument } from '@/components/patients/MedicalRecordView';

interface UseMedicalRecordResult {
  medicalRecord: DossierMedical | null;
  isLoading: boolean;
  isSubmitting: boolean;
  error: string | null;
  fetchMedicalRecord: (patientId: string) => Promise<DossierMedical | null>;
  createMedicalRecord: (data: Partial<DossierMedicalDTO>) => Promise<void>;
  updateMedicalRecord: (data: Partial<DossierMedical>) => Promise<void>;
  addDocument: (document: Omit<Document, 'id'>) => Promise<void>;
  deleteDocument: (documentId: string) => Promise<void>;
}

export function useMedicalRecord(patientId: string): UseMedicalRecordResult {
  const [dossierMedicalDTO, setDossierMedicalDTO] = useState<DossierMedicalDTO | null>(null);
  const [medicalRecord, setMedicalRecord] = useState<DossierMedical | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Fonction pour charger le dossier médical
  const fetchMedicalRecord = useCallback(async (patientId: string): Promise<DossierMedical | null> => {
    setIsLoading(true);
    setError(null);

    try {
      const dossier  = await dossierMedicalService.getDossierMedicalByPatientId(patientId);
      console.log("Fetched dossier medical:", dossier);

      if (dossier && dossier.id) {
        setMedicalRecord(dossier);
        return dossier;
      } else {
        setMedicalRecord(null);
        return null;
      }
    } catch (err) {
      console.error('Failed to fetch medical record:', err);
      setError('Échec du chargement du dossier médical');
      toast.error('Échec du chargement du dossier médical');
      return null;
    } finally {
      setIsLoading(false);
    }
  }, []);



  // Charger les données initiales
  useEffect(() => {
    fetchMedicalRecord(patientId);
  }, [patientId, fetchMedicalRecord]);

  // Add the createMedicalRecord method
  const createMedicalRecord = async (data: Partial<DossierMedicalDTO>): Promise<void> => {
    if (!patientId) {
      toast.error("Patient ID is required to create a medical record");
      return;
    }

    setIsSubmitting(true);
    setError(null);

    try {
      // Map the frontend data to the backend DTO structure
    const dossierMedicalDTO = {
      patientId,
      allergies: data.allergies || "",
      maladiesChroniques: data.maladiesChroniques || "",
      medicamentsActuels: data.medicamentsActuels || "",
      antécédentsFamiliaux: data.antécédentsFamiliaux || "",
      antécédentsPersonnels: data.antécédentsPersonnels || "",
      groupeSanguin: data.groupeSanguin || "",
    };

    // Call the service to create the medical record
    const newMedicalRecord = await dossierMedicalService.createDossierMedical(dossierMedicalDTO);

      setDossierMedicalDTO(newMedicalRecord); // Set the newly created medical record

      // Ajoute cette ligne pour rafraîchir le state local :
      await fetchMedicalRecord(patientId);
      
      toast.success("Dossier médical créé avec succès");
    } catch (err) {
      console.error("Failed to create medical record:", err);
      setError("Failed to create medical record");
      toast.error("Échec de la création du dossier médical");
      throw err;
    } finally {
      setIsSubmitting(false);
    }
  };

  // Mettre à jour le dossier médical
  const updateMedicalRecord = async (data: Partial<DossierMedical>): Promise<void> => {
    if (!medicalRecord) return;
    
    setIsSubmitting(true);
    setError(null);
    
    try {
      if (medicalRecord.id) {
        await dossierMedicalService.updateDossierMedical({
          ...medicalRecord,
          ...data,
        });
        
        setMedicalRecord(prev => prev ? { ...prev, ...data } : null);
        toast.success('Dossier médical mis à jour avec succès');
      }
    } catch (err) {
      console.error('Failed to update medical record:', err);
      setError('Failed to update medical record');
      toast.error('Échec de la mise à jour du dossier médical');
      throw err;
    } finally {
      setIsSubmitting(false);
    }
  };

  // Ajouter un document au dossier médical
  const addDocument = async (document: CreateDocumentRequest): Promise<void> => {
    if (!medicalRecord) {
      console.warn("[addDocument] medicalRecord is null");
      return;
    }

    console.log("[addDocument] Medical Record ID:", medicalRecord.id);
    console.log("[addDocument] Document to send:", document);
    
    setIsSubmitting(true);
    setError(null);
    
    try {
      const newDocument = await dossierMedicalService.addDocumentToDossier(
        medicalRecord.id,
        document
      );
      console.log("[addDocument] Document returned from backend:", newDocument);
      
      setMedicalRecord(prev => {
        if (!prev) return null;
        
        return {
          ...prev,
          documents: [...prev.documents, newDocument]
        };
      });
      
      toast.success('Document ajouté avec succès');
    } catch (err) {
      console.error("[addDocument] Failed to add document:", err);
      setError('Failed to add document');
      toast.error('Échec de l\'ajout du document');
      throw err;
    } finally {
      setIsSubmitting(false);
    }
  };

  // Supprimer un document du dossier médical
  const deleteDocument = async (documentId: string): Promise<void> => {
    if (!medicalRecord) return;
    
    setIsSubmitting(true);
    setError(null);
    
    try {
      await dossierMedicalService.deleteDocument(documentId);
      
      setMedicalRecord(prev => {
        if (!prev) return null;
        
        return {
          ...prev,
          documents: prev.documents.filter(doc => doc.id !== documentId)
        };
      });
      
      toast.success('Document supprimé avec succès');
    } catch (err) {
      console.error('Failed to delete document:', err);
      setError('Failed to delete document');
      toast.error('Échec de la suppression du document');
      throw err;
    } finally {
      setIsSubmitting(false);
    }
  };

  return {
    medicalRecord,
    isLoading,
    isSubmitting,
    error,
    fetchMedicalRecord,
    createMedicalRecord,
    updateMedicalRecord,
    addDocument,
    deleteDocument,
  };
}
