import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "@/hooks/useAuth";
import { DossierMedical, Patient } from "@/types/patient";
import { PatientsList } from "@/components/patients/PatientsList";
import { PatientProfile } from "@/components/patients/PatientProfile";
import { PatientForm } from "@/components/patients/PatientForm";
import { PatientSettings } from "@/components/patients/PatientSettings";
import { usePatients } from "@/hooks/usePatients";
import { Button } from "@/components/ui/button";
import { FileText, Settings, User } from "lucide-react";
import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card";
import { Tabs, TabsList, TabsTrigger, TabsContent } from "@/components/ui/tabs";
import { useTranslation } from "@/hooks/useTranslation";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
  DialogFooter,
} from "@/components/ui/dialog";
import {
  Pagination,
  PaginationContent,
  PaginationItem,
  PaginationNext,
  PaginationPrevious,
} from "@/components/ui/pagination";
import { cn } from "@/lib/utils";

type PatientFormValues = Omit<Patient, "id" | "dateCreation">;

function PatientsPage() {
  const { t } = useTranslation("patients");
  const { user } = useAuth();
  const navigate = useNavigate();
  const {
    patients,
    filteredPatients,
    isLoading,
    permissions,
    searchTerm,
    setSearchTerm,
    handleAddPatient,
    handleUpdatePatient,
    handleDeletePatient,
  } = usePatients();

  const [isFormOpen, setIsFormOpen] = useState(false);
  const [selectedPatient, setSelectedPatient] = useState<Patient | null>(null);
  const [patientData, setPatientData] = useState<Patient | null>(null);
  const [activeTab, setActiveTab] = useState("profile");
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const [patientToDelete, setPatientToDelete] = useState<string | null>(null);
  const [currentPage, setCurrentPage] = useState(1);

  // Pagination constants
  const ITEMS_PER_PAGE = 10;
  const totalPages = Math.ceil(filteredPatients.length / ITEMS_PER_PAGE);
  const startIndex = (currentPage - 1) * ITEMS_PER_PAGE;
  const paginatedPatients = filteredPatients.slice(
    startIndex,
    startIndex + ITEMS_PER_PAGE
  );

  // Reset currentPage when filteredPatients changes
  useEffect(() => {
    if (filteredPatients.length === 0) {
      setCurrentPage(1);
    } else if (currentPage > totalPages && totalPages > 0) {
      setCurrentPage(totalPages);
    }
  }, [filteredPatients, totalPages, currentPage]);

  // Load patient data if user is a patient
  useEffect(() => {
    if (user?.role === "Patient" && patients.length > 0) {
      const foundPatient =
        patients.find((p) => p.email === user.email) || patients[0];
      setPatientData(foundPatient);
    }
  }, [user, patients]);

  // Navigate to medical record
  const handleViewMedicalRecord = (patientId: string) => {
    navigate(`/medical-record/${patientId}`);
  };

  // Handle form submission
  const handleFormSubmit = async (
    data: PatientFormValues
  ): Promise<Patient> => {
    try {
      if (selectedPatient) {
        const payload = { ...data, id: selectedPatient.id };
        const updatedPatient = await handleUpdatePatient(
          selectedPatient.id,
          payload
        );
        setSelectedPatient(null);
        setIsFormOpen(false);
        return updatedPatient; // <-- retourne le patient mis à jour
      } else {
        const newPatient = await handleAddPatient(data); // <-- doit retourner le patient créé
        setIsFormOpen(false);
        return newPatient; // <-- retourne le patient créé
      }
    } catch (error) {
      console.error("Error submitting patient data:", error);
      throw error; // pour que le Promise soit rejected si erreur
    }
  };

  // Handle edit patient
  const handleEditPatient = (patient: Patient) => {
    setSelectedPatient(patient);
    setIsFormOpen(true);
  };

  // Handle delete patient with confirmation
  const handleDeletePatientConfirm = async () => {
    if (patientToDelete) {
      try {
        await handleDeletePatient(patientToDelete);
        setIsDeleteDialogOpen(false);
        setPatientToDelete(null);
      } catch (error) {
        console.error("Error deleting patient:", error);
      }
    }
  };

  const handleDeletePatientRequest = (patientId: string) => {
    setPatientToDelete(patientId);
    setIsDeleteDialogOpen(true);
  };

  // Role-specific patient view
  const renderPatientView = () => {
    if (!user) return null;

    if (user.role === "Patient") {
      if (!patientData) {
        return (
          <div className="flex justify-center items-center h-40">
            <p className="text-muted-foreground">
              {t("loading_patient_profile")}
            </p>
          </div>
        );
      }

      const patientProfileData = {
        id: patientData.id,
        name: `${patientData.prenom} ${patientData.nom}`,
        email: patientData.email,
        phone: patientData.telephone,
        dateOfBirth: patientData.dateNaissance,
        gender: patientData.sexe,
        address: patientData.adresse,
      };

      return (
        <Tabs value={activeTab} onValueChange={setActiveTab} className="w-full">
          <TabsList className="mb-6 grid grid-cols-3 w-full md:w-auto">
            <TabsTrigger
              value="profile"
              className="flex items-center gap-2"
              aria-label={t("profile")}
            >
              <User className="h-4 w-4" />
              <span className="hidden md:inline">{t("profile")}</span>
            </TabsTrigger>
            <TabsTrigger
              value="medical"
              className="flex items-center gap-2"
              aria-label={t("medical_record")}
            >
              <FileText className="h-4 w-4" />
              <span className="hidden md:inline">{t("medical_record")}</span>
            </TabsTrigger>
            <TabsTrigger
              value="settings"
              className="flex items-center gap-2"
              aria-label={t("settings")}
            >
              <Settings className="h-4 w-4" />
              <span className="hidden md:inline">{t("settings")}</span>
            </TabsTrigger>
          </TabsList>

          <TabsContent value="profile" className="mt-0">
            <PatientProfile
              patient={patientProfileData}
              onEditPatient={(patient) => {
                const updatedPatient: Partial<Patient> = {
                  id: patient.id,
                  nom: patient.nom.split(" ")[1] || patientData?.nom || "",
                  prenom:
                    patient.nom.split(" ")[0] || patientData?.prenom || "",
                  email: patient.email,
                  telephone: patient.telephone,
                  dateNaissance: patient.dateNaissance,
                  sexe: patient.sexe as "M" | "F",
                  adresse: patient.adresse,
                };
                return handleUpdatePatient(patient.id, updatedPatient);
              }}
            />
          </TabsContent>

          <TabsContent value="medical" className="mt-0">
            <Card>
              <CardHeader>
                <CardTitle>{t("medical_record")}</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="flex flex-col items-start gap-4">
                  <p>{t("view_medical_history_description")}</p>
                  <Button
                    onClick={() => handleViewMedicalRecord(patientData.id)}
                    className="gap-2"
                    aria-label={t("view_medical_record")}
                  >
                    <FileText className="h-4 w-4" />
                    {t("view_medical_record")}
                  </Button>
                </div>
              </CardContent>
            </Card>
          </TabsContent>

          <TabsContent value="settings" className="mt-0">
            <PatientSettings />
          </TabsContent>
        </Tabs>
      );
    }

    async function fetchMedicalRecord(
      patientId: string
    ): Promise<DossierMedical | null> {
      try {
        const medicalRecord: DossierMedical = await fetch(
          `medical-records/${patientId}`
        ).then((res) => res.json());
        return medicalRecord;
      } catch (error) {
        console.error("Failed to fetch medical record:", error);
        throw error;
      }
    }

    return (
      <>
        <PatientsList
          userRole={user.role}
          patients={patients}
          filteredPatients={paginatedPatients} // Use paginatedPatients instead of filteredPatients
          searchTerm={searchTerm}
          setSearchTerm={setSearchTerm}
          isLoading={isLoading}
          permissions={permissions}
          onAddPatient={() => {
            setSelectedPatient(null);
            setIsFormOpen(true);
          }}
          onEditPatient={handleEditPatient}
          onDeletePatient={handleDeletePatientRequest}
          fetchMedicalRecord={fetchMedicalRecord}
        />
        {totalPages > 1 && (
          <Pagination>
            <PaginationContent>
              <PaginationItem>
                <PaginationPrevious
                  onClick={() =>
                    setCurrentPage((prev) => Math.max(prev - 1, 1))
                  }
                  className={cn(
                    currentPage <= 1 && "pointer-events-none opacity-50"
                  )}
                />
              </PaginationItem>
              <PaginationItem className="flex items-center">
                <span className="text-sm">
                  {t("page")} {currentPage} {t("of")} {totalPages}
                </span>
              </PaginationItem>
              <PaginationItem>
                <PaginationNext
                  onClick={() =>
                    setCurrentPage((prev) => Math.min(prev + 1, totalPages))
                  }
                  className={cn(
                    currentPage >= totalPages &&
                      "pointer-events-none opacity-50"
                  )}
                />
              </PaginationItem>
            </PaginationContent>
          </Pagination>
        )}
      </>
    );
  };

  return (
    <div className="space-y-6 pb-8">
      <div className="flex flex-col gap-2">
        <h1 className="text-3xl font-bold tracking-tight">
          {user?.role === "Patient" ? t("patient_portal") : t("patients")}
        </h1>
        <p className="text-muted-foreground">
          {user?.role === "Patient"
            ? t("manage_patient_profile_description")
            : t("manage_patients_description")}
        </p>
      </div>
      {renderPatientView()}

      {/* Patient Form Modal */}
      <PatientForm
        isOpen={isFormOpen}
        onClose={() => {
          setIsFormOpen(false);
          setSelectedPatient(null);
        }}
        onSubmit={handleFormSubmit}
        initialData={selectedPatient || undefined}
        isLoading={isLoading}
      />

      {/* Confirm Delete Dialog */}
      <Dialog open={isDeleteDialogOpen} onOpenChange={setIsDeleteDialogOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>{t("confirmDelete")}</DialogTitle>
            <DialogDescription>
              {t("confirmDeleteDescription")}
            </DialogDescription>
          </DialogHeader>
          <DialogFooter>
            <Button
              variant="outline"
              onClick={() => setIsDeleteDialogOpen(false)}
            >
              {t("cancel")}
            </Button>
            <Button variant="destructive" onClick={handleDeletePatientConfirm}>
              {t("confirm")}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}

export default PatientsPage;
