import { useState, useEffect, useRef } from "react";
import { useAuth } from "@/hooks/useAuth";
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
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Badge } from "@/components/ui/badge";
import { DoctorForm } from "@/components/doctors/DoctorForm";
import { DoctorFilters } from "@/components/doctors/DoctorFilters";
import { DoctorProfile } from "@/components/doctors/DoctorProfile";
import { useDoctors } from "@/hooks/useDoctors";
import { toast } from "sonner";
import { useCliniques } from "@/hooks/useCliniques";
import { AssignDoctorModal } from "@/components/doctors/AssignDoctorModal";
import { ConfirmUnassignDialog } from "@/components/doctors/ConfirmUnassignDialog";
import { AppointmentForm } from "@/components/appointments/AppointmentForm";
import { useAppointments } from "@/hooks/useAppointments";
import { useDisponibilite } from "@/hooks/useDisponibilites";
import { useTranslation } from "@/hooks/useTranslation";
import { ConfirmDeleteDialog } from "@/components/consultations/ConfirmDeleteDialog";
import {
  Pagination,
  PaginationContent,
  PaginationItem,
  PaginationNext,
  PaginationPrevious,
} from "@/components/ui/pagination";
import { cn } from "@/lib/utils";

export interface Doctor {
  id: string;
  prenom: string;
  nom: string;
  specialite: string;
  email: string;
  telephone: string;
  cliniqueId?: string;
  photoUrl?: string;
  dateCreation: string;
}

function DoctorsPage() {
  const { user } = useAuth();
  const { t: tCommon } = useTranslation("common");
  const { t } = useTranslation("doctors");
  const {
    doctors,
    isLoading,
    isSubmitting,
    fetchDoctors,
    deleteDoctor,
    assignDoctorToClinic,
    unassignDoctorFromClinic,
  } = useDoctors();
  const { getAvailableDoctors } = useDisponibilite();
  const { cliniques } = useCliniques();
  const { handleAddAppointment } = useAppointments();

  const [searchTerm, setSearchTerm] = useState("");
  const [filteredDoctors, setFilteredDoctors] = useState<Doctor[]>([]);
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [selectedDoctor, setSelectedDoctor] = useState<Doctor | null>(null);
  const [selectedDoctorForProfile, setSelectedDoctorForProfile] =
    useState<Doctor | null>(null);
  const [filters, setFilters] = useState({
    specialty: null as string | null,
    clinicId: null as string | null,
    assignedStatus: "all" as "all" | "assigned" | "unassigned",
  });
  const [availabilityFilters, setAvailabilityFilters] = useState({
    date: null as string | null,
    heureDebut: null as string | null,
    heureFin: null as string | null,
  });
  const [availableDoctorIds, setAvailableDoctorIds] = useState<string[] | null>(
    null
  );
  const [isAssignModalOpen, setIsAssignModalOpen] = useState(false);
  const [doctorToAssign, setDoctorToAssign] = useState<Doctor | null>(null);
  const [selectedClinicId, setSelectedClinicId] = useState<string | null>(null);
  const [isAppointmentFormOpen, setIsAppointmentFormOpen] = useState(false);
  const [doctorForAppointment, setDoctorForAppointment] =
    useState<Doctor | null>(null);
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const [doctorIdToDelete, setDoctorIdToDelete] = useState<string | null>(null);
  const hasFetchedDoctorsRef = useRef(false);
  const [currentPage, setCurrentPage] = useState(1);

  // Define specialty values and translation keys
  const specialtyValues = [
    "General Practitioner",
    "Pediatrician",
    "Cardiologist",
    "Dermatologist",
    "Neurologist",
    "Psychiatrist",
    "Ophthalmologist",
    "Gynecologist",
    "Orthopedist",
    "Dentist",
  ];
  const specialtyKeys = [
    "generalPractitioner",
    "pediatrician",
    "cardiologist",
    "dermatologist",
    "neurologist",
    "psychiatrist",
    "ophthalmologist",
    "gynecologist",
    "orthopedist",
    "dentist",
  ];

  // Pagination constants
  const ITEMS_PER_PAGE = 7; // Match ClinicsPage.tsx
  const totalPages = Math.ceil(filteredDoctors.length / ITEMS_PER_PAGE);
  const startIndex = (currentPage - 1) * ITEMS_PER_PAGE;

  const paginatedDoctors = filteredDoctors.slice(
    startIndex,
    startIndex + ITEMS_PER_PAGE
  );

  // Reset currentPage when filters change or filteredDoctors is updated
  useEffect(() => {
    if (filteredDoctors.length === 0) {
      setCurrentPage(1);
    } else if (currentPage > totalPages && totalPages > 0) {
      setCurrentPage(totalPages);
    }
  }, [filteredDoctors, totalPages, currentPage]);

  useEffect(() => {
    if (!hasFetchedDoctorsRef.current) {
      fetchDoctors().catch(() => toast.error(t("errorFetchingDoctors")));
      hasFetchedDoctorsRef.current = true;
    }
  }, [fetchDoctors, t]);

  useEffect(() => {
    const { date, heureDebut, heureFin } = availabilityFilters;
    if (!date || !heureDebut || !heureFin) {
      setAvailableDoctorIds(null);
      return;
    }

    const fetchAvailableDoctors = async () => {
      try {
        const availableDoctors = await getAvailableDoctors(
          date,
          heureDebut,
          heureFin
        );

        // Extraire uniquement les IDs
        const ids = availableDoctors.map((doctor) => doctor.id);
        console.log("Available Doctor IDs (UUIDs):", ids);

        setAvailableDoctorIds(ids);
      } catch (error) {
        console.error(t("errorFetchingDoctors"), error);
        toast.error(t("errorFetchingDoctors"));
        setAvailableDoctorIds([]);
      }
    };

    fetchAvailableDoctors();
  }, [availabilityFilters, getAvailableDoctors, t]);

  useEffect(() => {
    const results = doctors.filter((doctor) => {
      // Apply availability filter if set
      if (
        availableDoctorIds !== null &&
        !availableDoctorIds.includes(doctor.id)
      ) {
        return false;
      }

      const matchesSearch =
        `${doctor.prenom} ${doctor.nom}`
          .toLowerCase()
          .includes(searchTerm.toLowerCase()) ||
        doctor.specialite.toLowerCase().includes(searchTerm.toLowerCase()) ||
        doctor.email.toLowerCase().includes(searchTerm.toLowerCase());

      const matchesSpecialty =
        !filters.specialty || doctor.specialite === filters.specialty;
      const matchesClinic =
        !filters.clinicId || doctor.cliniqueId === filters.clinicId;

      const matchesAssignedStatus = (() => {
        if (filters.assignedStatus === "all") return true;
        if (user.role === "SuperAdmin") {
          return filters.assignedStatus === "assigned"
            ? !!doctor.cliniqueId
            : !doctor.cliniqueId;
        }
        if (user.role === "ClinicAdmin") {
          return filters.assignedStatus === "assigned"
            ? doctor.cliniqueId === user.cliniqueId
            : !doctor.cliniqueId;
        }
        return true;
      })();

      // For patients, only show doctors with a cliniqueId (assigned to a clinic)
      const matchesPatientRequirement =
        user.role === "Patient" ? !!doctor.cliniqueId : true;

      return (
        matchesSearch &&
        matchesSpecialty &&
        matchesClinic &&
        matchesAssignedStatus &&
        matchesPatientRequirement
      );
    });

    setFilteredDoctors(results);
    setCurrentPage(1); // Reset to page 1 when search or filters change
  }, [searchTerm, doctors, filters, user, availableDoctorIds]);

  const clinicsForForm = cliniques.map((c) => ({
    id: c.id,
    name: c.nom,
  }));

  const handleAvailabilityFilterChange = (filters: {
    date: string | null;
    heureDebut: string | null;
    heureFin: string | null;
  }) => {
    setAvailabilityFilters(filters);
    setCurrentPage(1); // Reset to page 1 when availability filters change
  };

  const handleFilterChange = (newFilters: {
    specialty: string | null;
    clinicId: string | null;
    assignedStatus: "all" | "assigned" | "unassigned";
  }) => {
    setFilters(newFilters);
    setCurrentPage(1); // Reset to page 1 when filters change
  };

  const handleFormSubmit = async (data: Doctor) => {
    try {
      setSelectedDoctor(null);
      setIsFormOpen(false);
      await fetchDoctors();
      toast.success(t("doctorAddSuccess"));
    } catch (error) {
      console.error(t("errorAddingDoctor"), error);
      toast.error(t("errorAddingDoctor"));
    }
  };

  const handleEditDoctor = (doctor: Doctor) => {
    setSelectedDoctor(doctor);
    setIsFormOpen(true);
  };

  const handleAssignDoctor = async (
    doctorId: string,
    clinicId: string | null | undefined
  ) => {
    if (!clinicId) {
      toast.error(t("errorAddingDoctor"));
      return;
    }
    try {
      await assignDoctorToClinic(doctorId, clinicId);
      await fetchDoctors();
      setIsAssignModalOpen(false);
      setDoctorToAssign(null);
      setSelectedClinicId(null);
      toast.success(t("doctorAddSuccess"));
    } catch (error) {
      console.error(t("errorAddingDoctor"), error);
      toast.error(t("errorAddingDoctor"));
    }
  };

  const handleUnassignDoctor = async (doctorId: string) => {
    try {
      await unassignDoctorFromClinic(doctorId);
      await fetchDoctors();
      toast.success(t("doctorDeleteSuccess"));
    } catch (error) {
      console.error(t("errorDeletingDoctor"), error);
      toast.error(t("errorDeletingDoctor"));
    }
  };

  const handleDeleteDoctor = async (id: string) => {
    try {
      if (user.role === "SuperAdmin") {
        await deleteDoctor(id);
        toast.success(t("doctorDeleteSuccess"));
      } else if (user.role === "ClinicAdmin") {
        await unassignDoctorFromClinic(id);
        toast.success(t("doctorDeleteSuccess"));
      } else {
        toast.error(t("errorDeletingDoctor"));
        return;
      }
      await fetchDoctors();
    } catch (error) {
      console.error(t("errorDeletingDoctor"), error);
      toast.error(t("errorDeletingDoctor"));
    }
  };

  const handleOpenDeleteDialog = (id: string) => {
    setDoctorIdToDelete(id);
    setIsDeleteDialogOpen(true);
  };

  const handleConfirmDelete = () => {
    if (doctorIdToDelete) {
      handleDeleteDoctor(doctorIdToDelete);
      setIsDeleteDialogOpen(false);
      setDoctorIdToDelete(null);
    }
  };

  if (!user) {
    return <div className="p-8 text-center">{t("loginRequired")}</div>;
  }

  if (isLoading) {
    return <div className="p-8 text-center">{t("loadingDoctors")}</div>;
  }

  if (user.role === "Doctor" || selectedDoctorForProfile) {
    const doctorToShow =
      selectedDoctorForProfile ||
      (user.role === "Doctor"
        ? doctors.find((d) => d.email === user.email) || null
        : null);

    if (!doctorToShow) {
      return <div className="p-8 text-center">{t("doctorDataNotFound")}</div>;
    }

    return (
      <div className="space-y-6 pb-8">
        <div className="flex flex-col gap-2">
          <h1 className="text-3xl font-bold tracking-tight">
            {t("doctorDetails")}
          </h1>
          <p className="text-muted-foreground">{t("manageProfile")}</p>
        </div>
        <DoctorProfile
          doctor={doctorToShow}
          onEdit={handleEditDoctor}
          clinics={clinicsForForm}
          userRole={user.role}
        />
        {user.role !== "Doctor" && (
          <div className="mt-4">
            <Button
              variant="outline"
              onClick={() => setSelectedDoctorForProfile(null)}
            >
              {t("backToDoctorsList")}
            </Button>
          </div>
        )}
        <DoctorForm
          isOpen={isFormOpen}
          onClose={() => setIsFormOpen(false)}
          onSubmit={handleFormSubmit}
          initialData={selectedDoctor ? { ...selectedDoctor } : undefined}
          clinics={clinicsForForm}
        />
      </div>
    );
  }

  const doctorsOfClinic = filteredDoctors.filter(
    (doctor) => doctor.cliniqueId === user.cliniqueId
  );
  const availableDoctors = filteredDoctors.filter(
    (doctor) => !doctor.cliniqueId
  );

  const renderDoctorRow = (doctor: Doctor) => {
    const specialtyIndex = specialtyValues.findIndex(
      (value) => value.toLowerCase() === doctor.specialite.toLowerCase()
    );
    const translatedSpecialty =
      specialtyIndex !== -1
        ? t(specialtyKeys[specialtyIndex])
        : t("unknownSpecialty");

    return (
      <TableRow
        key={doctor.id}
        className="cursor-pointer hover:bg-muted/60"
        onClick={() => setSelectedDoctorForProfile(doctor)}
      >
        <TableCell>
          <div className="flex items-center gap-3">
            <Avatar className="h-8 w-8">
              <AvatarFallback className="bg-clinic-500 text-white">
                {doctor.prenom[0]}
                {doctor.nom[0]}
              </AvatarFallback>
            </Avatar>
            <div className="font-medium">
              {doctor.prenom} {doctor.nom}
            </div>
          </div>
        </TableCell>
        <TableCell>{doctor.email}</TableCell>
        <TableCell>
          <Badge
            variant="outline"
            className="bg-blue-50 text-blue-600 border-blue-200"
          >
            {translatedSpecialty}
          </Badge>
        </TableCell>
        <TableCell>{doctor.telephone}</TableCell>
        <TableCell
          onClick={async (e) => {
            e.stopPropagation();
            if (user.role === "SuperAdmin") {
              setDoctorToAssign(doctor);
              setSelectedClinicId(doctor.cliniqueId ?? null);
              setIsAssignModalOpen(true);
            } else if (user.role === "ClinicAdmin") {
              if (!user.cliniqueId) {
                toast.error(t("errorAddingDoctor"));
                return;
              }
              if (doctor.cliniqueId === user.cliniqueId) {
                toast.info(t("doctorAddSuccess"));
                return;
              }
              try {
                await handleAssignDoctor(doctor.id, user.cliniqueId);
              } catch (error) {
                console.error(t("errorAddingDoctor"), error);
              }
            }
          }}
          className="cursor-pointer underline text-blue-600 hover:text-blue-800"
          title={
            user.role === "SuperAdmin"
              ? t("assignToClinic")
              : doctor.cliniqueId === user.cliniqueId
              ? t("doctorAddSuccess")
              : t("assignToClinic")
          }
        >
          {doctor.cliniqueId ? (
            <Badge className="bg-green-100 text-green-800 border-green-200">
              {cliniques.find((c) => c.id === doctor.cliniqueId)?.nom}
            </Badge>
          ) : (
            <Badge className="bg-red-100 text-red-800 border-red-200">
              {t("noClinicAssigned")}
            </Badge>
          )}
        </TableCell>
        <TableCell>
          {user.role === "Patient" ? (
            <Button
              size="sm"
              variant="outline"
              className="text-blue-600"
              onClick={(e) => {
                e.stopPropagation();
                setDoctorForAppointment(doctor);
                setIsAppointmentFormOpen(true);
              }}
            >
              {t("takeAppointment")}
            </Button>
          ) : user.role === "ClinicAdmin" &&
            doctor.cliniqueId !== user.cliniqueId ? null : (
            <div className="flex items-center gap-2">
              <Button
                size="sm"
                variant="ghost"
                onClick={(e) => {
                  e.stopPropagation();
                  handleEditDoctor(doctor);
                }}
                aria-label={t("editDoctor")}
              >
                <FileEdit className="h-4 w-4" />
              </Button>
              {user.role !== "ClinicAdmin" && (
                <Button
                  size="sm"
                  variant="ghost"
                  className="text-red-500"
                  onClick={(e) => {
                    e.stopPropagation();
                    handleOpenDeleteDialog(doctor.id);
                  }}
                  aria-label={t("deleteDoctor")}
                >
                  <Trash2 className="h-4 w-4" />
                </Button>
              )}
              {doctor.cliniqueId && (
                <ConfirmUnassignDialog
                  onConfirm={() => handleUnassignDoctor(doctor.id)}
                />
              )}
            </div>
          )}
        </TableCell>
      </TableRow>
    );
  };

  return (
    <div className="space-y-6 pb-8">
      <div className="flex flex-col gap-2">
        <h1 className="text-3xl font-bold tracking-tight">
          {t("doctorsList")}
        </h1>
        <p className="text-muted-foreground">{t("doctorDetails")}</p>
      </div>
      <Card>
        <CardContent className="p-6">
          <div className="flex items-center justify-between mb-4">
            <div className="relative w-full max-w-sm">
              <Search className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
              <Input
                placeholder={t("search")}
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="pl-8"
                aria-label={t("search")}
              />
            </div>
            {user.role !== "Patient" && (
              <Button
                className="ml-2"
                onClick={() => {
                  setSelectedDoctor(null);
                  setIsFormOpen(true);
                }}
                aria-label={t("addDoctor")}
              >
                <Plus className="mr-1 h-4 w-4" /> {t("addDoctor")}
              </Button>
            )}
          </div>
          <DoctorFilters
            specialties={Array.from(new Set(doctors.map((d) => d.specialite)))}
            clinics={clinicsForForm}
            userRole={user.role as "SuperAdmin" | "ClinicAdmin"}
            onFilterChange={handleFilterChange}
            onAvailabilityFilterChange={handleAvailabilityFilterChange}
          />
          <div className="rounded-md border">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>{t("fullName")}</TableHead>
                  <TableHead>{t("email")}</TableHead>
                  <TableHead>{t("specialty")}</TableHead>
                  <TableHead>{t("phone")}</TableHead>
                  <TableHead>{t("clinic")}</TableHead>
                  <TableHead>
                    {user.role !== "Patient"
                      ? t("actions")
                      : t("takeAppointment")}
                  </TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {user.role === "ClinicAdmin" ? (
                  <>
                    <TableRow>
                      <TableCell colSpan={6} className="bg-muted font-semibold">
                        {t("doctorsOfClinic")}
                      </TableCell>
                    </TableRow>
                    {doctorsOfClinic.length > 0 ? (
                      doctorsOfClinic.map((doctor) => renderDoctorRow(doctor))
                    ) : (
                      <TableRow>
                        <TableCell
                          colSpan={6}
                          className="text-center text-muted-foreground"
                        >
                          {t("noDoctorsInClinic")}
                        </TableCell>
                      </TableRow>
                    )}
                    <TableRow>
                      <TableCell colSpan={6} className="bg-muted font-semibold">
                        {t("availableDoctors")}
                      </TableCell>
                    </TableRow>
                    {availableDoctors.length > 0 ? (
                      availableDoctors.map((doctor) => renderDoctorRow(doctor))
                    ) : (
                      <TableRow>
                        <TableCell
                          colSpan={6}
                          className="text-center text-muted-foreground"
                        >
                          {t("noAvailableDoctors")}
                        </TableCell>
                      </TableRow>
                    )}
                  </>
                ) : paginatedDoctors.length > 0 ? (
                  paginatedDoctors.map((doctor) => renderDoctorRow(doctor))
                ) : (
                  <TableRow>
                    <TableCell colSpan={6} className="text-center py-4">
                      {t("noDoctorsFound")}
                    </TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
            {user.role !== "ClinicAdmin" && totalPages > 1 && (
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
                      {tCommon("page")} {currentPage} {tCommon("of")}{" "}
                      {totalPages}
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
          </div>
        </CardContent>
      </Card>
      <DoctorForm
        isOpen={isFormOpen}
        onClose={() => setIsFormOpen(false)}
        onSubmit={handleFormSubmit}
        initialData={selectedDoctor ? { ...selectedDoctor } : undefined}
        clinics={clinicsForForm}
      />
      <AssignDoctorModal
        isOpen={isAssignModalOpen}
        onClose={() => setIsAssignModalOpen(false)}
        onConfirm={() => {
          if (doctorToAssign && selectedClinicId) {
            handleAssignDoctor(doctorToAssign.id, selectedClinicId);
            setIsAssignModalOpen(false);
          }
        }}
        selectedClinicId={selectedClinicId}
        setSelectedClinicId={setSelectedClinicId}
        clinics={clinicsForForm}
        isSubmitting={isSubmitting}
      />
      <ConfirmDeleteDialog
        isOpen={isDeleteDialogOpen}
        onClose={() => setIsDeleteDialogOpen(false)}
        onConfirm={handleConfirmDelete}
        title={t("confirmDeleteDoctorTitle")}
        message={t("confirmDeleteDoctorMessage")}
      />
      {isAppointmentFormOpen && doctorForAppointment && (
        <AppointmentForm
          isOpen={isAppointmentFormOpen}
          onClose={() => {
            setIsAppointmentFormOpen(false);
            setDoctorForAppointment(null);
          }}
          onSubmit={async (data) => {
            try {
              await handleAddAppointment(data);
            } catch (error) {
              toast.error(t("errorAddingAvailability"));
              console.error(t("errorAddingAvailability"), error);
            } finally {
              setIsAppointmentFormOpen(false);
              setDoctorForAppointment(null);
            }
          }}
          doctors={[doctorForAppointment]}
          patientId={user.patientId}
        />
      )}
    </div>
  );
}

export default DoctorsPage;
