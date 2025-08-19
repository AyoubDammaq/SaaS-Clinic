import { useAuth } from "@/hooks/useAuth";
import { SuperAdminProfile } from "@/components/admins/SuperAdminProfile";
import { ClinicAdminProfile } from "@/components/admins/ClinicAdminProfile";
import { DoctorProfile } from "@/components/doctors/DoctorProfile";
import { PatientProfile } from "@/components/patients/PatientProfile";
import { useDoctors } from "@/hooks/useDoctors";
import { usePatients } from "@/hooks/usePatients";
import { useCliniques } from "@/hooks/useCliniques";
import { useState } from "react";
import { toast } from "sonner";
import { DoctorForm } from "@/components/doctors/DoctorForm";
import { Patient } from "@/types/patient";
import { PatientForm } from "@/components/patients/PatientForm";
import { useTranslation } from "@/hooks/useTranslation";

export default function ProfilePage() {
  const { user } = useAuth();
  const { t } = useTranslation("profil");
  const { doctors, fetchDoctors } = useDoctors();
  const { patients, fetchPatients } = usePatients();
  const { cliniques } = useCliniques();

  const [selectedDoctor, setSelectedDoctor] = useState(null);
  const [selectedPatient, setSelectedPatient] = useState<Patient | null>(null);
  const [isFormOpen, setIsFormOpen] = useState(false);

  if (!user) {
    return (
      <div className="flex h-screen items-center justify-center">
        <p>
          {t("must_be_logged_in") ||
            "Vous devez être connecté pour voir votre profil."}
        </p>
      </div>
    );
  }

  const handleEditDoctor = (doctor) => {
    console.log("Edit doctor:", doctor);
    setSelectedDoctor(doctor);
    setIsFormOpen(true);
  };

  const handleEditPatient = (patient: Patient) => {
    setSelectedPatient(patient);
    setIsFormOpen(true);
  };

  const handleCloseForm = () => {
    setIsFormOpen(false);
    setSelectedDoctor(null);
    setSelectedPatient(null);
  };

  const handleFormSubmit = async (updatedDoctor) => {
    try {
      if (selectedDoctor) {
        await fetchDoctors(); // Recharge la liste des docteurs après update
        toast.success(
          t("profile_updated_success") || "Profil mis à jour avec succès !"
        );
      } else if (selectedPatient) {
        await fetchPatients(); // Recharge la liste des patients après update
        toast.success(
          t("profile_updated_success") || "Profil mis à jour avec succès !"
        );
      }
      handleCloseForm();
    } catch (error) {
      toast.error(t("update_error") || "Erreur lors de la mise à jour.");
      console.error(error);
    }
  };

  const clinicsForForm = cliniques.map((c) => ({
    id: c.id,
    name: c.nom,
  }));

  const renderProfile = () => {
    switch (user.role) {
      case "SuperAdmin":
        return <SuperAdminProfile />;

      case "ClinicAdmin":
        return <ClinicAdminProfile />;

      case "Doctor": {
        const currentDoctor = doctors.find((doc) => doc.id === user.medecinId);
        if (!currentDoctor) {
          return (
            <div className="text-center py-8">
              <p className="text-muted-foreground">
                {t("doctor_profile_not_found") || "Profil médecin non trouvé."}
              </p>
            </div>
          );
        }

        const doctorForProfile = {
          ...currentDoctor,
          patients: 0,
        };

        return (
          <DoctorProfile
            doctor={doctorForProfile}
            onEdit={handleEditDoctor}
            clinics={clinicsForForm}
            userRole="Doctor"
          />
        );
      }

      case "Patient": {
        console.log("Patients", patients);
        if (!patients || patients.length === 0) {
          return (
            <div className="text-center py-8">
              <p className="text-muted-foreground">
                Chargement du profil patient...
              </p>
            </div>
          );
        }

        const currentPatient = patients.find(
          (pat) =>
            pat.id.trim().toLowerCase() === user.patientId.trim().toLowerCase()
        );

        if (!currentPatient) {
          return (
            <div className="text-center py-8">
              <p className="text-muted-foreground">
                {t("patient_profile_not_found") || "Profil patient non trouvé."}
              </p>
            </div>
          );
        }

        const patientForProfile = {
          id: currentPatient.id,
          name: `${currentPatient.prenom} ${currentPatient.nom}`,
          email: currentPatient.email,
          phone: currentPatient.telephone,
          dateOfBirth: currentPatient.dateNaissance,
          gender: currentPatient.sexe,
          address: currentPatient.adresse,
          lastVisit: undefined,
        };

        return (
          <PatientProfile
            patient={patientForProfile}
            onEditPatient={handleEditPatient}
          />
        );
      }

      default:
        return (
          <div className="text-center py-8">
            <p className="text-muted-foreground">
              {t("unknown_profile_type") || "Type de profil non reconnu."}
            </p>
          </div>
        );
    }
  };

  return (
    <div className="container mx-auto p-6">
      <div className="mb-6">
        <h1 className="text-3xl font-bold tracking-tight">
          {t("my_profile") || "Mon Profil"}
        </h1>
        <p className="text-muted-foreground">
          {t("manage_personal_info") ||
            "Gérez vos informations personnelles et vos préférences"}
        </p>
      </div>

      {renderProfile()}

      {/* --- DoctorForm modal --- */}
      {isFormOpen && selectedDoctor && (
        <DoctorForm
          isOpen={isFormOpen}
          onClose={handleCloseForm}
          onSubmit={handleFormSubmit}
          initialData={selectedDoctor}
          clinics={clinicsForForm}
        />
      )}

      {/* --- PatientForm modal --- */}
      {isFormOpen && selectedPatient && (
        <PatientForm
          isOpen={isFormOpen}
          onClose={handleCloseForm}
          onSubmit={handleFormSubmit}
          initialData={selectedPatient}
        />
      )}
    </div>
  );
}
