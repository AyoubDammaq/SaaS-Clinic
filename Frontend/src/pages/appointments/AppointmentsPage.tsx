import { useState } from "react";
import { useAuth } from "@/hooks/useAuth";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { AppointmentFilters } from "@/components/appointments/AppointmentFilters";
import { AppointmentTable } from "@/components/appointments/AppointmentTable";
import { AppointmentForm } from "@/components/appointments/AppointmentForm";
import { AppointmentStatus } from "@/components/appointments/AppointmentStatusBadge";
import { cn } from "@/lib/utils";
import { useTranslation } from "@/hooks/useTranslation";
import { format } from "date-fns";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { toast } from "sonner";
import { useAppointments } from "@/hooks/useAppointments";
import { useDoctors } from "@/hooks/useDoctors";
import { usePatients } from "@/hooks/usePatients";
import { useCliniques } from "@/hooks/useCliniques";
import {
  AppointmentFormData,
  AppointmentStatusEnum,
  RendezVous,
} from "@/types/rendezvous";
import { ConsultationDTO } from "@/types/consultation";
import { Patient } from "@/types/patient";
import { Doctor } from "@/types/doctor";
import { CancelByDoctorDialog } from "@/components/appointments/CancelByDoctorDialog";
import { CreateFromAppointment } from "@/components/consultations/CreateFromAppointment";
import { useConsultations } from "@/hooks/useConsultations";
import {
  User,
  Calendar,
  Clock,
  Stethoscope,
  ClipboardList,
  X,
} from "lucide-react";

interface Appointment {
  id: string;
  patientName: string;
  patientId: string;
  doctorName: string;
  medecinId: string;
  clinicId: string;
  dateHeure: string;
  time: string;
  reason: string;
  status: keyof typeof AppointmentStatusEnum;
}

const mapRendezVousToAppointment = (
  rdv: RendezVous,
  patients: Patient[],
  doctors: Doctor[]
): Appointment => {
  const patient = patients.find((p) => p.id === rdv.patientId);
  const doctor = doctors.find((d) => d.id === rdv.medecinId);
  return {
    id: rdv.id,
    patientId: rdv.patientId,
    medecinId: rdv.medecinId,
    clinicId: doctor?.cliniqueId ?? "unknown",
    dateHeure: rdv.dateHeure,
    time: new Date(rdv.dateHeure).toLocaleTimeString([], {
      hour: "2-digit",
      minute: "2-digit",
    }),
    reason: rdv.commentaire ?? "—",
    status: AppointmentStatusEnum[
      rdv.statut
    ] as keyof typeof AppointmentStatusEnum,
    patientName:
      rdv.patientNom ||
      (patient ? `${patient.prenom} ${patient.nom}` : "Inconnu"),
    doctorName:
      rdv.medecinNom ||
      (doctor ? `Dr. ${doctor.prenom} ${doctor.nom}` : "Inconnu"),
  };
};

function AppointmentsPage() {
  const { user } = useAuth();
  const { doctors } = useDoctors();
  const { patients } = usePatients();
  const { cliniques } = useCliniques();
  const { addConsultation } = useConsultations();
  const { t } = useTranslation("appointments");
  const tConsultations = useTranslation("consultations").t;
  const tCommon = useTranslation("common").t;

  const {
    filteredAppointments,
    isLoading,
    isSubmitting,
    permissions,
    searchTerm,
    setSearchTerm,
    handleAddAppointment,
    handleUpdateAppointment,
    handleCancelAppointment,
    handleCancelAppointmentByDoctor,
    handleConfirmAppointment,
    refetchAppointments,
  } = useAppointments();

  const mappedAppointments = filteredAppointments.map((rdv) =>
    mapRendezVousToAppointment(rdv, patients, doctors)
  );

  // State for filters
  const [dateFilter, setDateFilter] = useState<Date | null>(null);
  const [statusFilter, setStatusFilter] = useState<AppointmentStatus | "all">(
    "all"
  );
  const [clinicFilter, setClinicFilter] = useState<string | "all">("all");
  const [doctorFilter, setDoctorFilter] = useState<string | "all">("all");
  const [patientFilter, setPatientFilter] = useState<string | "all">("all");

  // State for appointment form
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [selectedAppointment, setSelectedAppointment] =
    useState<Appointment | null>(null);

  // State for appointment details dialog
  const [isDetailsOpen, setIsDetailsOpen] = useState(false);
  const [viewingAppointment, setViewingAppointment] =
    useState<Appointment | null>(null);

  // State for cancel confirmation dialog
  const [isCancelDialogOpen, setIsCancelDialogOpen] = useState(false);
  const [appointmentToCancel, setAppointmentToCancel] = useState<string | null>(
    null
  );

  // State for consultation form
  const [isConsultationFormOpen, setIsConsultationFormOpen] = useState(false);
  const [appointmentForConsultation, setAppointmentForConsultation] =
    useState<Appointment | null>(null);

  // State for cancel by doctor dialog
  const [doctorCancelDialogOpen, setDoctorCancelDialogOpen] = useState(false);
  const [appointmentToCancelByDoctor, setAppointmentToCancelByDoctor] =
    useState<Appointment | null>(null);

  // Filter appointments based on user role, search term, date, status, clinic, doctor, and patient
  const getFilteredAppointments = () => {
    if (!user) return { upcoming: [], past: [] };

    let filtered = [...mappedAppointments];

    if (user.role === "ClinicAdmin") {
      const doctorIds = doctors
        .filter((d) => d.cliniqueId === user.cliniqueId)
        .map((d) => d.id);
      filtered = filtered.filter((a) => doctorIds.includes(a.medecinId));
    } else if (user.role === "Doctor") {
      filtered = filtered.filter((a) => a.medecinId === user.medecinId);
    } else if (user.role === "Patient") {
      filtered = filtered.filter((a) => a.patientId === user.patientId);
    }

    if (searchTerm) {
      const term = searchTerm.toLowerCase();
      filtered = filtered.filter(
        (appointment) =>
          appointment.patientName?.toLowerCase().includes(term) ||
          appointment.doctorName?.toLowerCase().includes(term) ||
          appointment.reason?.toLowerCase().includes(term)
      );
    }

    if (dateFilter) {
      const formattedFilterDate = format(dateFilter, "yyyy-MM-dd");
      filtered = filtered.filter((appointment) => {
        const appointmentDate = new Date(appointment.dateHeure);
        const formattedAppointmentDate = format(appointmentDate, "yyyy-MM-dd");
        return formattedAppointmentDate === formattedFilterDate;
      });
    }

    if (clinicFilter !== "all") {
      filtered = filtered.filter(
        (appointment) => appointment.clinicId === clinicFilter
      );
    }

    if (doctorFilter !== "all") {
      filtered = filtered.filter(
        (appointment) => appointment.medecinId === doctorFilter
      );
    }

    if (patientFilter !== "all") {
      filtered = filtered.filter(
        (appointment) => appointment.patientId === patientFilter
      );
    }

    filtered = filtered.filter(
      (appointment) =>
        statusFilter === "all" || appointment.status === statusFilter
    );

    const now = new Date();

    const upcoming = filtered.filter((appointment) => {
      const appointmentDate = new Date(appointment.dateHeure);
      return appointmentDate >= now;
    });

    const past = filtered.filter((appointment) => {
      const appointmentDate = new Date(appointment.dateHeure);
      return appointmentDate < now;
    });

    upcoming.sort(
      (a, b) =>
        new Date(a.dateHeure).getTime() - new Date(b.dateHeure).getTime()
    );
    past.sort(
      (a, b) =>
        new Date(b.dateHeure).getTime() - new Date(a.dateHeure).getTime()
    );

    return { upcoming, past };
  };

  const { upcoming, past } = getFilteredAppointments();

  // Handlers for appointment actions
  const handleCancel = (appointmentId: string) => {
    setAppointmentToCancel(appointmentId);
    setIsCancelDialogOpen(true);
  };

  const confirmCancelAppointment = async () => {
    if (appointmentToCancel) {
      try {
        await handleCancelAppointment(
          appointmentToCancel,
          user?.role === "Doctor" ? "doctor" : "patient"
        );

        toast.success(t("appointmentCancelled"));
        setIsCancelDialogOpen(false);
        setAppointmentToCancel(null);
        refetchAppointments();
      } catch (error) {
        toast.error(t("appointmentError"));
      }
    }
  };

  const handleCancelByDoctor = (appointment: Appointment) => {
    setAppointmentToCancelByDoctor(appointment);
    setDoctorCancelDialogOpen(true);
  };

  const confirmDoctorCancellation = async (justification: string) => {
    if (appointmentToCancelByDoctor) {
      try {
        await handleCancelAppointmentByDoctor(
          appointmentToCancelByDoctor.id,
          justification
        );
        toast.success(t("appointmentCancelledByDoctor"));
        setDoctorCancelDialogOpen(false);
        setAppointmentToCancelByDoctor(null);
        refetchAppointments();
      } catch (error) {
        toast.error(t("appointmentError"));
      }
    }
  };

  const handleComplete = (appointmentId: string) => {
    console.log(`Completing appointment ${appointmentId}`);
    toast.success(t("appointmentCompleted"));
  };

  const handleReschedule = (appointment: Appointment) => {
    setSelectedAppointment(appointment);
    setIsFormOpen(true);
  };

  const handleViewDetails = (appointment: Appointment) => {
    setViewingAppointment(appointment);
    setIsDetailsOpen(true);
  };

  const handleConfirm = async (appointmentId: string) => {
    try {
      await handleConfirmAppointment(appointmentId);
      toast.success(t("appointmentConfirmed"));
      refetchAppointments();
    } catch (error) {
      toast.error(t("appointmentError"));
    }
  };

  const handleCreateAppointment = () => {
    setSelectedAppointment(null);
    setIsFormOpen(true);
  };

  const handleFormSubmit = async (data: AppointmentFormData) => {
    if (selectedAppointment) {
      try {
        await handleUpdateAppointment(selectedAppointment.id, data);
        toast.success(t("appointmentUpdated"));
      } catch (error) {
        console.error("[AppointmentsPage] Update failed:", error);
        toast.error(t("appointmentError"));
      }
    } else {
      try {
        await handleAddAppointment(data);
        toast.success(t("appointmentCreated"));
      } catch (error) {
        console.error("[AppointmentsPage] Creation failed:", error);
        toast.error(t("appointmentError"));
      }
    }
    setIsFormOpen(false);
    refetchAppointments();
  };

  const handleCreateConsultation = (appointment: Appointment) => {
    if (user?.role === "Doctor" && appointment.status === "CONFIRME") {
      setAppointmentForConsultation(appointment);
      setIsConsultationFormOpen(true);
    }
  };

  const handleConsultationFormSubmit = async (data: ConsultationDTO) => {
    try {
      await addConsultation(data);
      toast.success("Consultation créée avec succès");
      setIsConsultationFormOpen(false);
      setAppointmentForConsultation(null);
    } catch (error) {
      console.error("Erreur lors de la création de la consultation :", error);
      toast.error("Échec de la création de la consultation");
    }
  };

  return (
    <div className="space-y-6 pb-8">
      <div className="flex flex-col gap-2">
        <h1 className="text-3xl font-bold tracking-tight">
          {t("appointments")}
        </h1>
        <p className="text-muted-foreground">
          {user?.role === "Patient"
            ? t("manageYourAppointments")
            : user?.role === "Doctor"
            ? t("managePatientAppointments")
            : t("manageAllAppointments")}
        </p>
      </div>

      <AppointmentFilters
        searchTerm={searchTerm}
        onSearchChange={setSearchTerm}
        dateFilter={dateFilter}
        onDateFilterChange={setDateFilter}
        statusFilter={statusFilter}
        onStatusFilterChange={setStatusFilter}
        clinicFilter={clinicFilter}
        onClinicFilterChange={setClinicFilter}
        doctorFilter={doctorFilter}
        onDoctorFilterChange={setDoctorFilter}
        patientFilter={patientFilter}
        onPatientFilterChange={setPatientFilter}
        onCreateAppointment={handleCreateAppointment}
        userRole={user?.role}
        clinics={cliniques}
        doctors={doctors}
        patients={patients}
      />

      <div className="space-y-6">
        <Card>
          <CardHeader className="py-4">
            <CardTitle>{t("upcomingAppointments")}</CardTitle>
            <CardDescription>
              {tCommon("Manage")} {t("upcomingAppointments").toLowerCase()}
            </CardDescription>
          </CardHeader>
          <CardContent>
            <AppointmentTable
              appointments={upcoming}
              userRole={user?.role || ""}
              onCancel={handleCancel}
              onCancelByDoctor={handleCancelByDoctor}
              onComplete={handleComplete}
              onReschedule={handleReschedule}
              onViewDetails={handleViewDetails}
              onRowClick={
                user?.role === "Doctor" ? handleCreateConsultation : undefined
              }
              onConfirm={handleConfirm}
              isPast={false}
            />
          </CardContent>
        </Card>

        <Card>
          <CardHeader className={cn("py-4", past.length === 0 && "border-b-0")}>
            <CardTitle>{t("pastAppointments")}</CardTitle>
            <CardDescription>
              {tCommon("view")} {t("pastAppointments").toLowerCase()}
            </CardDescription>
          </CardHeader>
          <CardContent>
            <AppointmentTable
              appointments={past}
              userRole={user?.role || ""}
              onViewDetails={handleViewDetails}
              isPast={true}
            />
          </CardContent>
        </Card>
      </div>

      <AppointmentForm
        isOpen={isFormOpen}
        onClose={() => setIsFormOpen(false)}
        onSubmit={handleFormSubmit}
        initialData={selectedAppointment}
        doctors={doctors}
        patients={patients}
        patientId={user?.role === "Patient" ? user.patientId : undefined}
      />

      <Dialog open={isDetailsOpen} onOpenChange={setIsDetailsOpen}>
        <DialogContent className="sm:max-w-[500px]">
          <DialogHeader>
            <DialogTitle className="flex items-center gap-2">
              <ClipboardList className="w-5 h-5 text-primary" />
              {t("appointmentDetails")}
            </DialogTitle>
          </DialogHeader>

          {viewingAppointment && (
            <div className="space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div className="flex flex-col gap-1">
                  <div className="flex items-center gap-2 text-sm font-medium text-muted-foreground">
                    <User className="w-4 h-4" />
                    {tCommon("patient")}
                  </div>
                  <p>{viewingAppointment.patientName}</p>
                </div>
                <div className="flex flex-col gap-1">
                  <div className="flex items-center gap-2 text-sm font-medium text-muted-foreground">
                    <Stethoscope className="w-4 h-4" />
                    {tCommon("doctor")}
                  </div>
                  <p>{viewingAppointment.doctorName}</p>
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div className="flex flex-col gap-1">
                  <div className="flex items-center gap-2 text-sm font-medium text-muted-foreground">
                    <Calendar className="w-4 h-4" />
                    {t("date")}
                  </div>
                  <p>
                    {new Date(viewingAppointment.dateHeure).toLocaleDateString(
                      "fr-FR",
                      {
                        weekday: "short",
                        day: "2-digit",
                        month: "2-digit",
                        year: "numeric",
                      }
                    )}
                  </p>
                </div>
                <div className="flex flex-col gap-1">
                  <div className="flex items-center gap-2 text-sm font-medium text-muted-foreground">
                    <Clock className="w-4 h-4" />
                    {t("time")}
                  </div>
                  <p>{viewingAppointment.time}</p>
                </div>
              </div>

              <div className="flex flex-col gap-1">
                <div className="flex items-center gap-2 text-sm font-medium text-muted-foreground">
                  <ClipboardList className="w-4 h-4" />
                  {t("reason")}
                </div>
                <p>{viewingAppointment.reason}</p>
              </div>

              <DialogFooter>
                <Button
                  variant="outline"
                  onClick={() => setIsDetailsOpen(false)}
                >
                  {tCommon("close")}
                </Button>
              </DialogFooter>
            </div>
          )}
        </DialogContent>
      </Dialog>

      <Dialog open={isCancelDialogOpen} onOpenChange={setIsCancelDialogOpen}>
        <DialogContent className="sm:max-w-[425px]">
          <DialogHeader>
            <DialogTitle>{t("cancelAppointment")}</DialogTitle>
            <DialogDescription>
              {t("confirmActionCannotBeUndone")}
            </DialogDescription>
          </DialogHeader>
          <DialogFooter>
            <Button
              variant="outline"
              onClick={() => setIsCancelDialogOpen(false)}
            >
              {tCommon("cancel")}
            </Button>
            <Button variant="destructive" onClick={confirmCancelAppointment}>
              {t("cancelAppointment")}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {appointmentForConsultation && (
        <CreateFromAppointment
          appointment={{
            id: appointmentForConsultation.id,
            patientId: appointmentForConsultation.patientId,
            medecinId: appointmentForConsultation.medecinId,
            date: appointmentForConsultation.dateHeure.split("T")[0],
            raison: appointmentForConsultation.reason,
            status: appointmentForConsultation.status,
          }}
          open={isConsultationFormOpen}
          onOpenChange={(open) => {
            setIsConsultationFormOpen(open);
            if (!open) setAppointmentForConsultation(null);
          }}
          onCreateConsultation={handleConsultationFormSubmit}
        />
      )}

      <CancelByDoctorDialog
        isOpen={doctorCancelDialogOpen}
        onClose={() => {
          setDoctorCancelDialogOpen(false);
          setAppointmentToCancelByDoctor(null);
        }}
        onSubmit={confirmDoctorCancellation}
      />
    </div>
  );
}

export default AppointmentsPage;
