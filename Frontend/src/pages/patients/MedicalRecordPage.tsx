import { useParams, useNavigate } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { CreateMedicalRecordModal } from "@/components/patients/CreateMedicalRecordForm";
import { ArrowLeft, User } from "lucide-react";
import {
  MedicalRecordView,
  MedicalRecord as MedicalRecordViewType,
  MedicalRecordDocument,
} from "@/components/patients/MedicalRecordView";
import { useMedicalRecord } from "@/hooks/useMedicalRecord";
import { patientService } from "@/services/patientService";
import { useState, useEffect, useMemo } from "react";
import { toast } from "sonner";
import { useTranslation } from "@/hooks/useTranslation";
import { DossierMedicalDTO } from "@/types/patient";
import { Patient, Document, DossierMedical } from "@/types/patient";
import { Consultation } from "@/types/consultation";
import { ConsultationHistory } from "@/components/patients/ConsultationHistory";
import { ConsultationDetails } from "@/components/consultations/ConsultationDetails";
import { useDoctors } from "@/hooks/useDoctors";
import { useAuth } from "@/hooks/useAuth";
import { differenceInYears } from "date-fns";

function SimplifiedPatientProfile({ patient }: { patient: Patient }) {
  const { t } = useTranslation("patients");

  const calculateAge = (dateOfBirth: string) => {
    try {
      const date = new Date(dateOfBirth);
      if (isNaN(date.getTime())) throw new Error("Invalid date");
      return differenceInYears(new Date(), date);
    } catch {
      return t("unknown_age") || "Âge inconnu";
    }
  };

  return (
    <Card className="mb-6">
      <CardContent className="pt-6">
        <div className="flex items-center gap-4">
          <User className="h-8 w-8 text-primary" />
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div aria-label={t("patient_name") || "Nom du patient"}>
              <span className="text-sm text-muted-foreground">{t("name") || "Nom"}: </span>
              <span className="font-medium">{`${patient.prenom} ${patient.nom}`}</span>
            </div>
            <div aria-label={t("patient_age") || "Âge du patient"}>
              <span className="text-sm text-muted-foreground">{t("age") || "Âge"}: </span>
              <span className="font-medium">{calculateAge(patient.dateNaissance)}</span>
            </div>
            <div aria-label={t("patient_gender") || "Sexe du patient"}>
              <span className="text-sm text-muted-foreground">{t("gender") || "Sexe"}: </span>
              <span className="font-medium">
                {patient.sexe === "M" ? t("male") || "Homme" : patient.sexe === "F" ? t("female") || "Femme" : t("other") || "Autre"}
              </span>
            </div>
            <div aria-label={t("patient_email") || "Email du patient"}>
              <span className="text-sm text-muted-foreground">{t("email") || "Email"}: </span>
              <span className="font-medium">{patient.email}</span>
            </div>
            <div aria-label={t("patient_phone") || "Téléphone du patient"}>
              <span className="text-sm text-muted-foreground">{t("phone") || "Téléphone"}: </span>
              <span className="font-medium">{patient.telephone}</span>
            </div>
            {patient.adresse && (
              <div aria-label={t("patient_address") || "Adresse du patient"}>
                <span className="text-sm text-muted-foreground">{t("address") || "Adresse"}: </span>
                <span className="font-medium">{patient.adresse}</span>
              </div>
            )}
          </div>
        </div>
      </CardContent>
    </Card>
  );
}

function MedicalRecordPage() {
  const { user } = useAuth();
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [patient, setPatient] = useState<Patient | null>(null);
  const [isLoadingPatient, setIsLoadingPatient] = useState(true);
  const [patientCache, setPatientCache] = useState<{ [key: string]: Patient }>({});
  const { t } = useTranslation("patients");
  const stableT = useMemo(() => t, [t]); // Stabiliser la fonction t
  const [selectedConsultation, setSelectedConsultation] = useState<Consultation | null>(null);
  const [isDetailsOpen, setIsDetailsOpen] = useState(false);
  const { doctors, isLoading: isLoadingDoctors } = useDoctors();

  const selectedDoctor = doctors.find((doc) => doc.id === selectedConsultation?.medecinId);
  const doctorName = selectedDoctor ? `${selectedDoctor.prenom} ${selectedDoctor.nom}` : "";

  const {
    medicalRecord,
    isLoading: isLoadingRecord,
    isSubmitting,
    fetchMedicalRecord,
    createMedicalRecord,
    updateMedicalRecord,
    addDocument,
    deleteDocument,
  } = useMedicalRecord(id || "");

  // Récupérer les données du patient
  useEffect(() => {
    const fetchPatient = async () => {
      if (!id) {
        toast.error(stableT("errors.no_patient_id") || "Aucun ID de patient fourni");
        navigate("/patients");
        return;
      }

      // Vérifier le cache
      if (patientCache[id]) {
        setPatient(patientCache[id]);
        setIsLoadingPatient(false);
        return;
      }

      setIsLoadingPatient(true);
      try {
        const patientData = await patientService.getPatientById(id);
        if (patientData) {
          setPatient(patientData);
          setPatientCache((prev) => ({ ...prev, [id]: patientData }));
          // Appeler fetchMedicalRecord après patient
          await fetchMedicalRecord(id);
        } else {
          toast.error(stableT("errors.patient_not_found") || "Patient non trouvé");
          navigate("/patients");
        }
      } catch (error: any) {
        console.error("Erreur lors de la récupération du patient:", error);
        if (error.response?.status === 429) {
          toast.error(stableT("errors.rate_limit_exceeded") || "Trop de requêtes, veuillez réessayer plus tard");
        } else {
          toast.error(stableT("errors.load_patient_failed") || "Échec du chargement des données du patient");
        }
        navigate("/patients");
      } finally {
        setIsLoadingPatient(false);
      }
    };

    fetchPatient();
  }, [id, navigate, stableT, patientCache, fetchMedicalRecord]);

  if (isLoadingPatient || isLoadingRecord) {
    return (
      <Card>
        <CardContent className="pt-6">
          <div className="flex justify-center items-center h-40">
            <p className="text-muted-foreground">{stableT("loading", "common") || "Chargement..."}</p>
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
            <p className="text-muted-foreground">{stableT("patient_not_found") || "Patient introuvable"}</p>
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
      allergies: medicalRecord.allergies || "",
      chronicDiseases: medicalRecord.maladiesChroniques || "",
      currentMedications: medicalRecord.medicamentsActuels || "",
      bloodType: medicalRecord.groupeSanguin || "",
      creationDate: medicalRecord.dateCreation || "",
      personalHistory: medicalRecord.antécédentsPersonnels || "",
      familyHistory: medicalRecord.antécédentsFamiliaux || "",
      documents: (medicalRecord.documents || []).map((doc) => ({
        id: doc.id,
        nom: doc.nom || "",
        type: doc.type,
        dateCreation: doc.dateCreation || "",
        url: doc.url || "",
      })),
    };
  };

  const mapViewDocumentToAppDocument = (
    viewDocument: Omit<MedicalRecordDocument, "id" | "dateCreation">
  ): Omit<Document, "id"> => {
    return {
      nom: viewDocument.nom,
      type: viewDocument.type,
      url: viewDocument.url || "",
      dateCreation: new Date().toISOString(),
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
      antécédentsFamiliaux: data.familyHistory,
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
      toast.success(stableT("success.medical_record_created") || "Dossier médical créé avec succès");
    } catch (error) {
      console.error("Failed to create medical record:", error);
      toast.error(stableT("errors.create_medical_record_failed") || "Échec de la création du dossier médical");
    }
  };

  const handleViewConsultationDetails = (consultation: Consultation) => {
    setSelectedConsultation(consultation);
    setIsDetailsOpen(true);
  };

  return (
    <div className="space-y-6 pb-8">
      {/* Section du profil simplifié */}
      {["SuperAdmin", "ClinicAdmin", "Doctor"].includes(user.role) && patient && (
        <SimplifiedPatientProfile patient={patient} />
      )}

      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">
            {stableT("patientRecord") || "Dossier médical"}
          </h1>
          <p className="text-muted-foreground">
            {stableT("manage_medical_info") || "Consulter et gérer les informations médicales du patient"}
          </p>
        </div>
        <Button variant="outline" onClick={() => navigate("/patients")}>
          <ArrowLeft className="mr-2 h-4 w-4" />
          {stableT("back_to_patients") || "Retour aux patients"}
        </Button>
      </div>

      <MedicalRecordView
        userRole={user.role}
        patient={{
          name: `${patient.prenom} ${patient.nom}`,
          email: patient.email,
          phone: patient.telephone,
          dateOfBirth: patient.dateNaissance,
          gender: patient.sexe,
          address: patient.adresse,
          id: patient.id,
        }}
        medicalRecord={mapMedicalRecordToView()}
        isLoading={isLoadingRecord}
        isSubmitting={isSubmitting}
        updateMedicalRecord={(data) => {
          return updateMedicalRecord(mapViewUpdateToAppUpdate(data));
        }}
        addDocument={(documentData) => {
          return addDocument(mapViewDocumentToAppDocument(documentData));
        }}
        deleteDocument={deleteDocument}
        onCreateMedicalRecord={handleCreateMedicalRecord}
        isCreating={isSubmitting}
      />
      {medicalRecord && (
        <ConsultationHistory
          patientId={patient.id}
          patientName={`${patient.prenom} ${patient.nom}`}
          fallbackDoctorName={doctorName}
          onViewConsultationDetails={handleViewConsultationDetails}
        />
      )}
      {selectedConsultation && (
        <ConsultationDetails
          isOpen={isDetailsOpen}
          onClose={() => setIsDetailsOpen(false)}
          consultation={selectedConsultation}
          patientName={`${patient.prenom} ${patient.nom}`}
          doctorName={doctorName}
        />
      )}
    </div>
  );
}

export default MedicalRecordPage;