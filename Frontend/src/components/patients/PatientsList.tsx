import { useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { differenceInYears, format, parseISO } from "date-fns";
import { fr } from "date-fns/locale"; // Pour le français
import { enUS } from "date-fns/locale"; // Pour l'anglais
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Search, Plus, FileEdit, Trash2, X } from "lucide-react";
import { Card, CardContent } from "@/components/ui/card";
import { Patient } from "@/types/patient";
import { DossierMedical } from "@/types/patient";
import { MedicalRecord } from "@/components/patients/MedicalRecordView";
import { UserRole } from "@/types/auth";
import { useTranslation } from "@/hooks/useTranslation";

interface PatientsListProps {
  userRole: UserRole;
  patients: Patient[];
  filteredPatients: Patient[];
  searchTerm: string;
  setSearchTerm: (term: string) => void;
  isLoading: boolean;
  permissions: {
    canCreate: boolean;
    canEdit: boolean;
    canDelete: boolean;
  };
  onAddPatient: () => void;
  onEditPatient: (patient: Patient) => void;
  onDeletePatient: (id: string) => void;
  fetchMedicalRecord: (patientId: string) => Promise<DossierMedical | null>;
}

export function PatientsList({
  userRole,
  patients,
  filteredPatients,
  searchTerm,
  setSearchTerm,
  isLoading,
  permissions,
  onAddPatient,
  onEditPatient,
  onDeletePatient,
  fetchMedicalRecord,
}: PatientsListProps) {
  const navigate = useNavigate();
  const { t, language } = useTranslation("patients");
  const tCommon = useTranslation("common").t;
  const [modalPatient, setModalPatient] = useState<Patient | null>(null);
  const [modalMedicalRecord, setModalMedicalRecord] =
    useState<DossierMedical | null>(null);
  const [modalLoading, setModalLoading] = useState(false);
  const [modalError, setModalError] = useState<string | null>(null);
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [currentPage, setCurrentPage] = useState(1);
  const patientsPerPage = 10;

  const totalPages = Math.ceil(filteredPatients.length / patientsPerPage);

  // Met à jour la page actuelle si les résultats changent
  useEffect(() => {
    if (currentPage > totalPages) {
      setCurrentPage(1);
    }
  }, [filteredPatients, totalPages, currentPage]);

  const paginatedPatients = useMemo(() => {
    const startIndex = (currentPage - 1) * patientsPerPage;
    const endIndex = startIndex + patientsPerPage;
    return filteredPatients.slice(startIndex, endIndex);
  }, [filteredPatients, currentPage]);

  // Calculate age from date of birth
  const calculateAge = (dateOfBirth: string) => {
    return differenceInYears(new Date(), new Date(dateOfBirth));
  };

  // Déterminer le locale en fonction de la langue
  const locale = language === "fr" ? fr : enUS;

  // Formater la date avec heure
  const formatDateWithTime = (dateStr: string) => {
    return format(parseISO(dateStr), "PPP 'à' HH:mm", { locale });
  };

  // Handle click on a patient row to view their medical record
  const handlePatientClick = (patient: Patient) => {
    navigate(`/medical-record/${patient.id}`);
  };

  // Close the modal and reset states
  const closeModal = () => {
    setModalPatient(null);
    setModalMedicalRecord(null);
    setModalError(null);
    setModalLoading(false);
    setIsFormOpen(false);
  };

  // Navigate to create medical record page
  const handleCreateMedicalRecord = () => {
    if (modalPatient) {
      setIsFormOpen(true); // Open the modal
    }
  };

  // Navigate to medical record details page
  const handleViewMedicalRecordPage = (modalPatient: Patient) => {
    if (modalPatient) {
      closeModal();
      navigate(`/medical-records/${modalPatient.id}`);
    }
  };

  // Map gender to translated string
  const getGenderLabel = (gender: string) => {
    switch (gender) {
      case "M":
        return t("male");
      case "F":
        return t("female");
      default:
        return t("other");
    }
  };

  const mapDossierMedicalToMedicalRecord = (
    dossier: DossierMedical
  ): MedicalRecord => {
    return {
      id: dossier.id,
      patientId: dossier.patientId,
      allergies: dossier.allergies || "",
      chronicDiseases: dossier.maladiesChroniques || "", // Map "maladiesChroniques"
      currentMedications: dossier.medicamentsActuels || "", // Map "medicamentsActuels"
      bloodType: dossier.groupeSanguin || "", // Map "groupeSanguin"
      creationDate: dossier.dateCreation || "", // Map "dateCreation"
      personalHistory: dossier.antécédentsPersonnels || "", // Map "antécédentsPersonnels"
      familyHistory: dossier.antécédentsFamiliaux || "", // Map "antécédentsFamiliaux"
      documents: (dossier.documents || []).map((doc) => ({
        id: doc.id,
        nom: doc.nom || "",
        type: doc.type,
        dateCreation: doc.dateCreation || "",
        url: doc.url || "",
      })),
    };
  };

  if (isLoading) {
    return (
      <Card>
        <CardContent className="pt-6">
          <div className="flex justify-center items-center h-40">
            <p className="text-muted-foreground">Chargement des patients...</p>
          </div>
        </CardContent>
      </Card>
    );
  }

  return (
    <>
      <div>
        <div className="flex items-center justify-between mb-4">
          <div className="relative w-full max-w-sm">
            <Search className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
            <Input
              placeholder={t("search_patients_placeholder")}
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="pl-8"
            />
          </div>
          {permissions.canCreate && (
            <Button className="ml-2" onClick={onAddPatient}>
              <Plus className="mr-1 h-4 w-4" /> {t("add_patient")}
            </Button>
          )}
        </div>

        <div className="rounded-md border">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>{t("full_name")}</TableHead>
                <TableHead>{t("email")}</TableHead>
                <TableHead>{t("phone")}</TableHead>
                <TableHead>{t("age")}</TableHead>
                <TableHead>{t("gender")}</TableHead>
                {userRole !== "Doctor" && (
                  <TableHead>{t("registration_date")}</TableHead>
                )}
                {userRole !== "Doctor" &&
                  (permissions.canEdit || permissions.canDelete) && (
                    <TableHead>{t("actions")}</TableHead>
                  )}
              </TableRow>
            </TableHeader>
            <TableBody>
              {filteredPatients.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={7} className="h-24 text-center">
                    {t("no_patients_found")}
                  </TableCell>
                </TableRow>
              ) : (
                paginatedPatients.map((patient) => (
                  <TableRow
                    key={patient.id}
                    className="cursor-pointer hover:bg-muted/60"
                    onClick={() => handlePatientClick(patient)}
                  >
                    <TableCell className="font-medium">
                      {patient.prenom} {patient.nom}
                    </TableCell>
                    <TableCell>{patient.email}</TableCell>
                    <TableCell>{patient.telephone}</TableCell>
                    <TableCell>{calculateAge(patient.dateNaissance)}</TableCell>
                    <TableCell>{getGenderLabel(patient.sexe)}</TableCell>
                    {userRole !== "Doctor" && (
                      <TableCell>
                        {formatDateWithTime(patient.dateCreation)}
                      </TableCell>
                    )}
                    {userRole !== "Doctor" &&
                      (permissions.canEdit || permissions.canDelete) && (
                        <TableCell onClick={(e) => e.stopPropagation()}>
                          <div className="flex items-center gap-2">
                            {permissions.canEdit && (
                              <Button
                                size="sm"
                                variant="ghost"
                                onClick={(e) => {
                                  e.stopPropagation();
                                  onEditPatient(patient);
                                }}
                              >
                                <FileEdit className="h-4 w-4" />
                              </Button>
                            )}
                            {permissions.canDelete && (
                              <Button
                                size="sm"
                                variant="ghost"
                                className="text-red-500"
                                onClick={(e) => {
                                  e.stopPropagation();
                                  onDeletePatient(patient.id);
                                }}
                              >
                                <Trash2 className="h-4 w-4" />
                              </Button>
                            )}
                          </div>
                        </TableCell>
                      )}
                  </TableRow>
                ))
              )}
            </TableBody>
          </Table>
          {filteredPatients.length > patientsPerPage && (
            <div className="flex justify-center items-center gap-4 mt-4">
              <Button
                size="sm"
                variant="outline"
                onClick={() => setCurrentPage((prev) => Math.max(prev - 1, 1))}
                disabled={currentPage === 1}
              >
                {tCommon("previous")}
              </Button>
              <span className="text-sm text-muted-foreground">
                {t("page")} {currentPage} / {totalPages}
              </span>
              <Button
                size="sm"
                variant="outline"
                onClick={() =>
                  setCurrentPage((prev) =>
                    prev < totalPages ? prev + 1 : prev
                  )
                }
                disabled={currentPage === totalPages}
              >
                {tCommon("next")}
              </Button>
            </div>
          )}
        </div>
      </div>
    </>
  );
}
