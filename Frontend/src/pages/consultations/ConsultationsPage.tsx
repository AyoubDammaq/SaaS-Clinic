import { useEffect, useMemo, useState } from "react";
import { useAuth } from "@/hooks/useAuth";
import { useConsultations } from "@/hooks/useConsultations";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { CalendarDays, Clock, FileEdit, Trash2 } from "lucide-react";
import {
  ConsultationForm,
  ConsultationFormValues,
} from "@/components/consultations/ConsultationForm";
import { toast } from "sonner";
import { useDoctors } from "@/hooks/useDoctors";
import { usePatients } from "@/hooks/usePatients";
import {
  Consultation,
  ConsultationDTO,
  ConsultationType,
  consultationTypes,
} from "@/types/consultation";
import { ConsultationDetails } from "@/components/consultations/ConsultationDetails";
import { format, parseISO } from "date-fns";
import { ConsultationsFilters } from "@/components/consultations/ConsultationsFilters";
import { useTranslation } from "@/hooks/useTranslation";
import { useCliniques } from "@/hooks/useCliniques";
import { doctorService } from "@/services/doctorService";
import { fr } from "date-fns/locale";
import {
  Pagination,
  PaginationContent,
  PaginationItem,
  PaginationNext,
  PaginationPrevious,
} from "@/components/ui/pagination";
import { cn } from "@/lib/utils";
import { Badge } from "@/components/ui/badge";
import { ConfirmDeleteDialog } from "@/components/consultations/ConfirmDeleteDialog";

function ConsultationsPage() {
  const { t } = useTranslation("consultations");
  const tCommon = useTranslation("common").t;
  const { user } = useAuth();
  const {
    filteredConsultations,
    addConsultation,
    updateConsultation,
    deleteConsultation,
    permissions,
    refetchConsultations,
  } = useConsultations();
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [currentConsultation, setCurrentConsultation] =
    useState<Consultation | null>(null);
  const [selectedConsultation, setSelectedConsultation] =
    useState<Consultation | null>(null);
  const [isDetailsOpen, setIsDetailsOpen] = useState(false);
  const [currentPage, setCurrentPage] = useState(1);
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const [consultationToDelete, setConsultationToDelete] = useState<
    string | null
  >(null);

  // États pour les filtres
  const [searchTerm, setSearchTerm] = useState("");
  const [dateFilter, setDateFilter] = useState<Date | null>(null);
  const [typeFilter, setTypeFilter] = useState<ConsultationType | "all">("all");
  const [doctorFilter, setDoctorFilter] = useState<string | "all">("all");
  const [patientFilter, setPatientFilter] = useState<string | "all">("all");
  const [clinicFilter, setClinicFilter] = useState<string | "all">("all");

  const { doctors } = useDoctors();
  const { patients } = usePatients();
  const { cliniques } = useCliniques();

  const [cliniqueIdFromDoctor, setCliniqueIdFromDoctor] = useState<
    string | null
  >(null);

  // Color mapping for consultation types
  const consultationTypeColors: Record<ConsultationType, string> = {
    [ConsultationType.ConsultationGenerale]: "bg-blue-100 text-blue-800",
    [ConsultationType.ConsultationSpecialiste]: "bg-green-100 text-green-800",
    [ConsultationType.ConsultationUrgence]: "bg-red-100 text-red-800",
    [ConsultationType.ConsultationSuivi]: "bg-purple-100 text-purple-800",
    [ConsultationType.ConsultationLaboratoire]: "bg-yellow-100 text-yellow-800",
  };

  useEffect(() => {
    const fetchDoctor = async () => {
      if (user?.role === "Doctor" && user?.medecinId) {
        try {
          const doctor = await doctorService.getDoctorById(user.medecinId);
          setCliniqueIdFromDoctor(doctor.cliniqueId ?? null);
        } catch (error) {
          console.error("Erreur lors du chargement du médecin", error);
          setCliniqueIdFromDoctor(null);
        }
      }
    };

    fetchDoctor();
  }, [user]);

  // Vérifier si l'utilisateur peut créer une consultation
  const canCreateConsultation =
    permissions.canCreate &&
    (user?.role !== "Doctor" || !!cliniqueIdFromDoctor);

  // Optimisation des recherches avec useMemo
  const patientMap = useMemo(() => {
    const map = new Map();
    patients.forEach((p) => map.set(p.id, `${p.prenom} ${p.nom}`));
    return map;
  }, [patients]);

  const doctorMap = useMemo(() => {
    const map = new Map();
    doctors.forEach((d) => map.set(d.id, `${d.prenom} ${d.nom}`));
    return map;
  }, [doctors]);

  const clinicMap = useMemo(() => {
    const map = new Map();
    cliniques.forEach((c) => map.set(c.id, c.nom));
    return map;
  }, [cliniques]);

  // Filtrer les consultations selon le rôle de l'utilisateur et les critères
  const getFilteredConsultations = () => {
    if (!user) return [];

    let filtered = [...filteredConsultations];

    // Restrictions par rôle
    if (user.role === "ClinicAdmin" && user.cliniqueId) {
      filtered = filtered.filter((c) => c.clinicId === user.cliniqueId);
    } else if (user.role === "Doctor" && user.medecinId) {
      filtered = filtered.filter((c) => c.medecinId === user.medecinId);
    } else if (user.role === "Patient" && user.patientId) {
      filtered = filtered.filter((c) => c.patientId === user.patientId);
    }

    // Filtre par recherche
    if (searchTerm) {
      const term = searchTerm.toLowerCase();
      filtered = filtered.filter(
        (consultation) =>
          consultation.diagnostic.toLowerCase().includes(term) ||
          patientMap
            .get(consultation.patientId)
            ?.toLowerCase()
            .includes(term) ||
          doctorMap.get(consultation.medecinId)?.toLowerCase().includes(term) ||
          clinicMap.get(consultation.clinicId)?.toLowerCase().includes(term)
      );
    }

    // Filtre par date
    if (dateFilter) {
      const formattedFilterDate = format(dateFilter, "yyyy-MM-dd");
      filtered = filtered.filter((consultation) =>
        consultation.dateConsultation.startsWith(formattedFilterDate)
      );
    }

    // Filtre par type
    if (typeFilter !== "all") {
      filtered = filtered.filter(
        (consultation) => consultation.type === typeFilter
      );
    }

    // Filtre par médecin
    if (doctorFilter !== "all") {
      filtered = filtered.filter(
        (consultation) => consultation.medecinId === doctorFilter
      );
    }

    // Filtre par patient
    if (patientFilter !== "all") {
      filtered = filtered.filter(
        (consultation) => consultation.patientId === patientFilter
      );
    }

    // Filtre par clinique
    if (clinicFilter !== "all") {
      filtered = filtered.filter(
        (consultation) => consultation.clinicId === clinicFilter
      );
    }

    // Trier par date (décroissant)
    filtered.sort(
      (a, b) =>
        new Date(b.dateConsultation).getTime() -
        new Date(a.dateConsultation).getTime()
    );

    return filtered;
  };

  const filtred = getFilteredConsultations();

  // Pagination
  const ITEMS_PER_PAGE = 5;
  const totalPages = Math.ceil(filtred.length / ITEMS_PER_PAGE);
  const startIndex = (currentPage - 1) * ITEMS_PER_PAGE;
  const paginatedConsultations = filtred.slice(
    startIndex,
    startIndex + ITEMS_PER_PAGE
  );

  const handleCreateConsultation = () => {
    setCurrentConsultation(null);
    setIsFormOpen(true);
  };

  const handleEditConsultation = (consultation: Consultation) => {
    setCurrentConsultation(consultation);
    setIsFormOpen(true);
  };

  const handleDeleteConsultation = (id: string) => {
    setConsultationToDelete(id);
    setIsDeleteDialogOpen(true);
  };

  const handleConfirmDeleteConsultation = async () => {
    if (consultationToDelete) {
      try {
        await deleteConsultation(consultationToDelete);
      } catch (error) {
        // Error toast is handled in the hook
      }
    }
    setIsDeleteDialogOpen(false);
    setConsultationToDelete(null);
  };

  const handleSubmitConsultation = async (
    data: ConsultationFormValues & { date: string }
  ) => {
    try {
      const consultationDTO: ConsultationDTO = {
        patientId: data.patientId,
        medecinId: user?.role === "Doctor" ? user.medecinId! : data.medecinId,
        type: data.type,
        dateConsultation: data.date,
        diagnostic: data.diagnostic,
        notes: data.notes || "",
      };

      if (currentConsultation) {
        await updateConsultation({
          ...consultationDTO,
          id: currentConsultation.id,
        });
      } else {
        await addConsultation(consultationDTO);
        toast.success(t("consultationAddSuccess"));
      }
      setIsFormOpen(false);
    } catch (err) {
      toast.error(
        currentConsultation
          ? t("errorUpdatingConsultation")
          : t("errorAddingConsultation")
      );
    }
  };

  const handleViewConsultationDetails = (consultation: Consultation) => {
    setSelectedConsultation(consultation);
    setIsDetailsOpen(true);
  };

  // Déterminer si la colonne Actions doit être affichée
  const showActionsColumn =
    user?.role !== "Patient" && user?.role !== "SuperAdmin";

  return (
    <div className="space-y-6 pb-8">
      <div className="flex flex-col gap-2">
        <h1 className="text-3xl font-bold tracking-tight">Consultations</h1>
        <p className="text-muted-foreground">
          {user?.role === "Patient"
            ? t("patientDescription")
            : user?.role === "Doctor"
            ? t("doctorDescription")
            : t("adminDescription")}
        </p>
      </div>

      <ConsultationsFilters
        searchTerm={searchTerm}
        onSearchChange={setSearchTerm}
        dateFilter={dateFilter}
        onDateFilterChange={setDateFilter}
        typeFilter={typeFilter}
        onTypeFilterChange={setTypeFilter}
        clinicFilter={clinicFilter}
        onClinicFilterChange={setClinicFilter}
        doctorFilter={doctorFilter}
        onDoctorFilterChange={setDoctorFilter}
        patientFilter={patientFilter}
        onPatientFilterChange={setPatientFilter}
        onCreateConsultation={
          canCreateConsultation ? handleCreateConsultation : undefined
        }
        userRole={user?.role}
        clinics={cliniques}
        doctors={doctors}
        patients={patients}
      />

      <Card>
        <CardHeader>
          <CardTitle>{t("medicalConsultationsTitle")}</CardTitle>
          <CardDescription>
            {user?.role === "Patient"
              ? t("patientConsultationsDescription")
              : t("adminConsultationsDescription")}
          </CardDescription>
        </CardHeader>
        <CardContent>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>{t("date")}</TableHead>
                <TableHead>{t("reason")}</TableHead>
                <TableHead>{t("typeConsultation")}</TableHead>
                {user?.role !== "Patient" && (
                  <TableHead>{t("patient")}</TableHead>
                )}
                {user?.role !== "Doctor" && (
                  <TableHead>{t("doctor")}</TableHead>
                )}
                {(user?.role === "Patient" || user?.role === "SuperAdmin") && (
                  <TableHead>{t("clinic")}</TableHead>
                )}
                {showActionsColumn && <TableHead>{t("actions")}</TableHead>}
              </TableRow>
            </TableHeader>
            <TableBody>
              {paginatedConsultations.map((consultation) => (
                <TableRow
                  key={consultation.id}
                  onClick={() => handleViewConsultationDetails(consultation)}
                  className="cursor-pointer hover:bg-muted/50 transition"
                >
                  <TableCell>
                    <div className="flex flex-col">
                      <div className="flex items-center">
                        <CalendarDays className="mr-1 h-3.5 w-3.5 text-muted-foreground" />
                        <span>
                          {format(
                            parseISO(consultation.dateConsultation),
                            "EEE, dd/MM/yyyy",
                            {
                              locale: fr,
                            }
                          )}
                        </span>
                      </div>
                      <div className="flex items-center text-muted-foreground">
                        <Clock className="mr-1 h-3.5 w-3.5" />
                        <span>
                          {format(
                            parseISO(consultation.dateConsultation),
                            "HH:mm"
                          )}
                        </span>
                      </div>
                    </div>
                  </TableCell>
                  <TableCell>{consultation.diagnostic}</TableCell>
                  <TableCell>
                    <Badge
                      className={cn(consultationTypeColors[consultation.type])}
                    >
                      {t(consultationTypes[consultation.type])}
                    </Badge>
                  </TableCell>
                  {user?.role !== "Patient" && (
                    <TableCell>
                      {patientMap.get(consultation.patientId) || "N/A"}
                    </TableCell>
                  )}
                  {user?.role !== "Doctor" && (
                    <TableCell>
                      {doctorMap.get(consultation.medecinId) || "N/A"}
                    </TableCell>
                  )}
                  {(user?.role === "Patient" ||
                    user?.role === "SuperAdmin") && (
                    <TableCell>
                      {clinicMap.get(consultation.clinicId) || "N/A"}
                    </TableCell>
                  )}
                  {showActionsColumn && (
                    <TableCell>
                      {(permissions.canEdit || permissions.canDelete) && (
                        <div className="flex gap-2">
                          {permissions.canEdit && (
                            <Button
                              size="sm"
                              variant="ghost"
                              onClick={(e) => {
                                e.stopPropagation();
                                handleEditConsultation(consultation);
                              }}
                            >
                              <FileEdit className="h-4 w-4" />
                            </Button>
                          )}
                          {permissions.canDelete && (
                            <Button
                              size="sm"
                              variant="ghost"
                              className="text-red-500 hover:text-red-700"
                              onClick={(e) => {
                                e.stopPropagation();
                                handleDeleteConsultation(consultation.id);
                              }}
                            >
                              <Trash2 className="h-4 w-4" />
                            </Button>
                          )}
                        </div>
                      )}
                    </TableCell>
                  )}
                </TableRow>
              ))}
              {paginatedConsultations.length === 0 && (
                <TableRow>
                  <TableCell
                    colSpan={
                      3 +
                      (user?.role !== "Patient" ? 1 : 0) +
                      (user?.role !== "Doctor" ? 1 : 0) +
                      (user?.role === "Patient" || user?.role === "SuperAdmin"
                        ? 1
                        : 0) +
                      (showActionsColumn ? 1 : 0)
                    }
                    className="text-center py-8 text-muted-foreground"
                  >
                    {t("noConsultationsFound")}
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
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
                <PaginationItem className="flex items-center text-sm">
                  {tCommon("page")} {currentPage} {tCommon("of")} {totalPages}
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
          {selectedConsultation && (
            <ConsultationDetails
              isOpen={isDetailsOpen}
              onClose={() => setIsDetailsOpen(false)}
              consultation={selectedConsultation}
              patientName={
                patientMap.get(selectedConsultation.patientId) || "N/A"
              }
              doctorName={
                doctorMap.get(selectedConsultation.medecinId) || "N/A"
              }
            />
          )}
        </CardContent>
      </Card>

      {user && (
        <ConsultationForm
          isOpen={isFormOpen}
          onClose={() => setIsFormOpen(false)}
          onSubmit={handleSubmitConsultation}
          initialData={
            currentConsultation
              ? {
                  patientId: currentConsultation.patientId,
                  medecinId: currentConsultation.medecinId,
                  type: currentConsultation.type,
                  diagnostic: currentConsultation.diagnostic,
                  notes: currentConsultation.notes,
                }
              : undefined
          }
          patients={patients}
          doctors={doctors}
          user={user}
        />
      )}

      {isDeleteDialogOpen && (
        <ConfirmDeleteDialog
          isOpen={isDeleteDialogOpen}
          onClose={() => {
            setIsDeleteDialogOpen(false);
            setConsultationToDelete(null);
          }}
          onConfirm={handleConfirmDeleteConsultation}
          title={t("confirmDelete")}
          message={t("confirmDeleteDescription")}
        />
      )}
    </div>
  );
}

export default ConsultationsPage;
