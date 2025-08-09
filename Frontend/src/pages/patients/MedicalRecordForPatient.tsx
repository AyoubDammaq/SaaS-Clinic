import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { ArrowLeft } from "lucide-react";
import { MedicalRecordView, MedicalRecord as MedicalRecordViewType } from "@/components/patients/MedicalRecordView";
import { useMedicalRecord } from "@/hooks/useMedicalRecord";
import { patientService } from "@/services/patientService";
import { toast } from "sonner";
import { useAuth } from "@/hooks/useAuth";
import { ConsultationHistory } from "@/components/patients/ConsultationHistory";
import { ConsultationDetails } from "@/components/consultations/ConsultationDetails";
import { useDoctors } from "@/hooks/useDoctors";
import { Consultation } from "@/types/consultation";
import { useTranslation } from "@/hooks/useTranslation";
import { useQuery } from "@tanstack/react-query";
import { AxiosError } from "axios"; // Importer AxiosError

export default function MedicalRecordForPatient() {
  const { user } = useAuth();
  const navigate = useNavigate();
  const { t } = useTranslation("patients");

  // Utiliser useQuery avec typage pour AxiosError
  const {
    data: patient,
    isLoading: isLoadingPatient,
    error: patientError,
  } = useQuery({
    queryKey: ["patient", user?.patientId],
    queryFn: () => patientService.getPatientById(user?.patientId || ""),
    enabled: !!user?.patientId,
    staleTime: 5 * 60 * 1000,
    retry: (failureCount, error) => {
      // Typage explicite de error comme AxiosError
      const axiosError = error as AxiosError;
      if (axiosError.response?.status === 429) {
        toast.error(t("too_many_requests"));
        return false; // Ne pas réessayer en cas de 429
      }
      return failureCount < 3; // Réessayer 3 fois pour d'autres erreurs
    },
  });

  const {
    medicalRecord,
    isLoading: isLoadingRecord,
    isSubmitting,
    createMedicalRecord,
    updateMedicalRecord,
    addDocument,
    deleteDocument,
  } = useMedicalRecord(user?.patientId || "");

  const { doctors } = useDoctors();

  const [selectedConsultation, setSelectedConsultation] = useState<Consultation | null>(null);
  const [isDetailsOpen, setIsDetailsOpen] = useState(false);

  // Gestion des erreurs d'authentification ou de patient non trouvé
  if (!user?.id) {
    toast.error(t("user_not_authenticated"));
    navigate("/");
    return null;
  }

  if (patientError) {
    toast.error(t("loading_patient_error"));
    navigate("/");
    return null;
  }

  if (isLoadingPatient) {
    return (
      <Card>
        <CardContent className="pt-6 flex justify-center items-center h-40">
          <p className="text-muted-foreground">{t("loading_patient")}</p>
        </CardContent>
      </Card>
    );
  }

  if (!patient) {
    return (
      <Card>
        <CardContent className="pt-6 flex justify-center items-center h-40">
          <p className="text-muted-foreground">{t("patient_not_found")}</p>
          <Button onClick={() => navigate("/")}>{t("back")}</Button>
        </CardContent>
      </Card>
    );
  }

  // Mapping fonction du dossier médical (inchangé)
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

  const handleCreateMedicalRecord = async (data) => {
    try {
      const payload = {
        patientId: patient.id,
        allergies: data.allergies,
        maladiesChroniques: data.chronicDiseases,
        medicamentsActuels: data.currentMedications,
        groupeSanguin: data.bloodType,
        antécédentsPersonnels: data.personalHistory,
        antécédentsFamiliaux: data.familyHistory,
      };

      await createMedicalRecord(payload);
      toast.success(t("medical_record_created_success"));
    } catch (error) {
      console.error(error);
      toast.error(t("medical_record_creation_failed"));
    }
  };

  const selectedDoctor = doctors.find((doc) => doc.id === selectedConsultation?.medecinId);
  const doctorName = selectedDoctor ? `${selectedDoctor.prenom} ${selectedDoctor.nom}` : "";

  const handleViewConsultationDetails = (consultation) => {
    setSelectedConsultation(consultation);
    setIsDetailsOpen(true);
  };

  return (
    <div className="space-y-6 pb-8">
      <div className="flex items-center justify-between">
        <h1 className="text-3xl font-bold tracking-tight">{t("my_medical_record")}</h1>
        <Button variant="outline" onClick={() => navigate(-1)}>
          <ArrowLeft className="mr-2 h-4 w-4" /> {t("back")}
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
        updateMedicalRecord={updateMedicalRecord}
        addDocument={addDocument}
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