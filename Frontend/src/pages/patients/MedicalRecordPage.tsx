
import { useParams, useNavigate } from 'react-router-dom';
import { Button } from '@/components/ui/button';
import { Card, CardContent } from '@/components/ui/card';
import { CreateMedicalRecordModal } from "@/components/patients/CreateMedicalRecordForm";
import { ArrowLeft } from 'lucide-react';
import { MedicalRecordView, MedicalRecord as MedicalRecordViewType, MedicalRecordDocument } from '@/components/patients/MedicalRecordView';
import { useMedicalRecord } from '@/hooks/useMedicalRecord';
import { patientService } from '@/services/patientService';
import { useState, useEffect } from 'react';
import { toast } from 'sonner';
import { useTranslation } from '@/hooks/useTranslation';
import { DossierMedicalDTO } from '@/types/patient';
import { Patient, Document, DossierMedical } from '@/types/patient';

function MedicalRecordPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [patient, setPatient] = useState<Patient | null>(null);
  const [isLoadingPatient, setIsLoadingPatient] = useState(true);
  const { t } = useTranslation('patients');
  
  const {
    medicalRecord,
    isLoading: isLoadingRecord,
    isSubmitting,
    fetchMedicalRecord,
    createMedicalRecord,
    updateMedicalRecord,
    addDocument,
    deleteDocument
  } = useMedicalRecord(id || "");

  // Récupérer les données du patient
  useEffect(() => {
    const fetchPatient = async () => {
      if (!id) return;
      
      setIsLoadingPatient(true);
      try {
        const patientData = await patientService.getPatientById(id);
        
        if (patientData) {
          setPatient(patientData);
        } else {
          toast.error("Patient not found");
          navigate('/patients');
        }
      } catch (error) {
        console.error("Erreur lors de la récupération du patient:", error);
        toast.error("Échec du chargement des données du patient");
      } finally {
        setIsLoadingPatient(false);
      }
    };
    
    fetchPatient();
  }, [id, navigate]);

  if (isLoadingPatient) {
    return (
      <Card>
        <CardContent className="pt-6">
          <div className="flex justify-center items-center h-40">
            <p className="text-muted-foreground">{t('loading', 'common')}</p>
          </div>
        </CardContent>
      </Card>
    );
  }

  if (!patient) {
    return (
      <Card>
        <CardContent className="pt-6">
          <div className="flex justify-center items-center h-40">
            <p className="text-muted-foreground">Patient introuvable</p>
          </div>
        </CardContent>
      </Card>
    );
  }

  // Create mapper functions to convert between the incompatible types
  const mapMedicalRecordToView = (): MedicalRecordViewType | null => {
    if (!medicalRecord) return null;

    return {
      id: medicalRecord.id,
      patientId: medicalRecord.patientId,
      allergies: medicalRecord.allergies || '',
      chronicDiseases: medicalRecord.maladiesChroniques || '',
      currentMedications: medicalRecord.medicamentsActuels || '',
      bloodType:  medicalRecord.groupeSanguin || '',
      creationDate: medicalRecord.dateCreation || '',
      personalHistory: medicalRecord.antécédentsPersonnels || '',
      familyHistory: medicalRecord.antécédentsFamiliaux || '',
      documents: (medicalRecord.documents || []).map(doc => ({
        id: doc.id,
        name: doc.nom || doc.name || '',
        type: doc.type,
        uploadDate: doc.dateCreation || doc.uploadDate || '',
        url: doc.url || ''
      }))
    };
  };

  const mapViewDocumentToAppDocument = (viewDocument: Omit<MedicalRecordDocument, "id">): Omit<Document, "id"> => {
    return {
      nom: viewDocument.name,
      type: viewDocument.type,
      dateCreation: viewDocument.uploadDate,
      contenu: viewDocument.url || '',
      url: viewDocument.url || ''
    };
  };

  const mapViewUpdateToAppUpdate = (
    data: Partial<Omit<MedicalRecordViewType, "id" | "patientId">>
  ): Partial<DossierMedical> => {
    return {
      allergies: data.allergies,
      maladiesChroniques: data.chronicDiseases,
      medicamentsActuels: data.currentMedications,
      groupeSanguin: data.bloodType,
      antécédentsPersonnels: data.personalHistory,
      antécédentsFamiliaux: data.familyHistory
    };
  };


  const handleCreateMedicalRecord = async (data: {
    allergies: string;
    chronicDiseases: string;
    currentMedications: string;
    bloodType: string;
    personalHistory: string;
    familyHistory: string;
  }) => {
    try {
      const payload: Partial<DossierMedicalDTO> = {
        patientId: patient.id,
        allergies: data.allergies,
        maladiesChroniques: data.chronicDiseases,
        medicamentsActuels: data.currentMedications,
        groupeSanguin: data.bloodType,
        antécédentsPersonnels: data.personalHistory,
        antécédentsFamiliaux: data.familyHistory,
      };

      await createMedicalRecord(payload);
      toast.success("Medical record created successfully");
    } catch (error) {
      console.error("Failed to create medical record:", error);
      toast.error("Failed to create medical record");
    }
  };


  return (
    <div className="space-y-6 pb-8">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">{t('patientRecord')}</h1>
          <p className="text-muted-foreground">
            Consulter et gérer les informations médicales du patient
          </p>
        </div>
        <Button variant="outline" onClick={() => navigate('/patients')}>
          <ArrowLeft className="mr-2 h-4 w-4" />
          Retour aux patients
        </Button>
      </div>
      
      <MedicalRecordView
        patient={{
          name: `${patient.prenom} ${patient.nom}`,
          email: patient.email,
          phone: patient.telephone,
          dateOfBirth: patient.dateNaissance,
          gender: patient.sexe,
          address: patient.adresse,
          id: patient.id
        }}
        medicalRecord={mapMedicalRecordToView()}
        isLoading={isLoadingRecord}
        isSubmitting={isSubmitting}
        updateMedicalRecord={(data) => {
          // Map the data back to the format expected by updateMedicalRecord
          return updateMedicalRecord(mapViewUpdateToAppUpdate(data));
        }}
        addDocument={(documentData) => {
          // Map the document data to the format expected by addDocument
          return addDocument(mapViewDocumentToAppDocument(documentData));
        }}
        deleteDocument={deleteDocument}
        onCreateMedicalRecord={handleCreateMedicalRecord}
        isCreating={isSubmitting}
      />
    </div>
  );
}

export default MedicalRecordPage;
