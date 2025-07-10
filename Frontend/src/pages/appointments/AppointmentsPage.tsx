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
import { ConsultationForm } from "@/components/consultations/ConsultationForm";
import { useAppointments } from "@/hooks/useAppointments";
import { useDoctors } from "@/hooks/useDoctors";
import { usePatients } from "@/hooks/usePatients";
import {
  AppointmentFormData,
  AppointmentStatusEnum,
  RendezVous,
} from "@/types/rendezvous";
import { ConsultationDTO } from "@/types/consultation";
import { Patient } from "@/types/patient";
import { Doctor } from "@/types/doctor";

interface Appointment {
  id: string;
  patientName: string;
  patientId: string;
  doctorName: string;
  medecinId: string;
  dateHeure: string;
  time: string;
  duration: number;
  reason: string;
  status: keyof typeof AppointmentStatusEnum;
  notes?: string;
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
    dateHeure: rdv.dateHeure,
    time: new Date(rdv.dateHeure).toLocaleTimeString([], {
      hour: "2-digit",
      minute: "2-digit",
    }),
    duration: 30,
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
    notes: rdv.justificationAnnulation || "",
  };
};

function AppointmentsPage() {
  const { user } = useAuth();
  const { doctors } = useDoctors();
  const { patients } = usePatients();
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

  // Filter appointments based on user role, search term, date and status
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

    filtered = filtered.filter(
      (appointment) =>
        statusFilter === "all" || appointment.status === statusFilter
    );

    const today = new Date();
    today.setHours(0, 0, 0, 0);

    const upcoming = filtered.filter((appointment) => {
      const appointmentDate = new Date(appointment.dateHeure);
      return appointmentDate >= today || appointment.status === "CONFIRME";
    });

    const past = filtered.filter((appointment) => {
      const appointmentDate = new Date(appointment.dateHeure);
      return appointmentDate < today && appointment.status !== "CONFIRME";
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
    // Open the cancel confirmation dialog
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
        refetchAppointments(); // <-- Important pour rafraîchir la liste
      } catch (error) {
        toast.error(t("appointmentError"));
      }
    }
  };

  const handleComplete = (appointmentId: string) => {
    // Here would be the API call to mark the appointment as completed
    console.log(`Completing appointment ${appointmentId}`);
    toast.success(t("appointmentCompleted"));
  };

  const handleReschedule = (appointment: Appointment) => {
    // Open form with the appointment data for rescheduling
    setSelectedAppointment(appointment);
    setIsFormOpen(true);
  };

  const handleViewDetails = (appointment: Appointment) => {
    // Open the details dialog
    setViewingAppointment(appointment);
    setIsDetailsOpen(true);
  };

  const handleAddNotes = (appointment: Appointment) => {
    // Here would be opening a notes modal for the doctor to add notes
    console.log(`Adding notes to appointment ${appointment.id}`);
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
    // Only allow doctors to create consultations from appointments
    if (user?.role === "Doctor" && appointment.status === "CONFIRME") {
      setAppointmentForConsultation(appointment);
      setIsConsultationFormOpen(true);
    }
  };

  const handleConsultationFormSubmit = (data: ConsultationDTO) => {
    // Here would be the API call to create a consultation
    console.log("Creating consultation from appointment:", data);

    // Show success message
    toast.success("Consultation created successfully");

    // Close the form
    setIsConsultationFormOpen(false);
    setAppointmentForConsultation(null);
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
        onCreateAppointment={handleCreateAppointment}
        userRole={user?.role}
      />

      <div className="space-y-6">
        <Card>
          <CardHeader className="py-4">
            <CardTitle>{t("upcomingAppointments")}</CardTitle>
            <CardDescription>
              {tCommon("manage")} {t("upcomingAppointments").toLowerCase()}
            </CardDescription>
          </CardHeader>
          <CardContent>
            <AppointmentTable
              appointments={upcoming}
              userRole={user?.role || ""}
              onCancel={handleCancel}
              onComplete={handleComplete}
              onReschedule={handleReschedule}
              onViewDetails={handleViewDetails}
              onAddNotes={handleAddNotes}
              onRowClick={
                user?.role === "Doctor" ? handleCreateConsultation : undefined
              }
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

      {/* Appointment Form Dialog */}
      <AppointmentForm
        isOpen={isFormOpen}
        onClose={() => setIsFormOpen(false)}
        onSubmit={handleFormSubmit}
        initialData={selectedAppointment}
        doctors={doctors}
        patients={patients}
        patientId={user?.role === "Patient" ? user.patientId : undefined}
      />

      {/* Appointment Details Dialog */}
      <Dialog open={isDetailsOpen} onOpenChange={setIsDetailsOpen}>
        <DialogContent className="sm:max-w-[500px]">
          <DialogHeader>
            <DialogTitle>{t("appointmentDetails")}</DialogTitle>
          </DialogHeader>
          {viewingAppointment && (
            <div className="space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <p className="text-sm font-medium text-muted-foreground">
                    {tCommon("patient")}
                  </p>
                  <p>{viewingAppointment.patientName}</p>
                </div>
                <div>
                  <p className="text-sm font-medium text-muted-foreground">
                    {tCommon("doctor")}
                  </p>
                  <p>{viewingAppointment.doctorName}</p>
                </div>
              </div>
              <div className="grid grid-cols-3 gap-4">
                <div>
                  <p className="text-sm font-medium text-muted-foreground">
                    {t("date")}
                  </p>
                  <p>{viewingAppointment.dateHeure}</p>
                </div>
                <div>
                  <p className="text-sm font-medium text-muted-foreground">
                    {t("time")}
                  </p>
                  <p>{viewingAppointment.time}</p>
                </div>
                <div>
                  <p className="text-sm font-medium text-muted-foreground">
                    {t("durationMinutes")}
                  </p>
                  <p>
                    {viewingAppointment.duration} {t("min")}
                  </p>
                </div>
              </div>
              <div>
                <p className="text-sm font-medium text-muted-foreground">
                  {t("reason")}
                </p>
                <p>{viewingAppointment.reason}</p>
              </div>
              {viewingAppointment.notes && (
                <div>
                  <p className="text-sm font-medium text-muted-foreground">
                    {t("additionalNotes")}
                  </p>
                  <p>{viewingAppointment.notes}</p>
                </div>
              )}
              <DialogFooter>
                {viewingAppointment.status === "CONFIRME" &&
                  user?.role !== "Patient" && (
                    <Button
                      variant="outline"
                      className="mr-auto"
                      onClick={() => {
                        handleCreateConsultation(viewingAppointment);
                        setIsDetailsOpen(false);
                      }}
                    >
                      {tConsultations("createConsultation")}
                    </Button>
                  )}
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

      {/* Cancel Appointment Confirmation Dialog */}
      <Dialog open={isCancelDialogOpen} onOpenChange={setIsCancelDialogOpen}>
        <DialogContent className="sm:max-w-[425px]">
          <DialogHeader>
            <DialogTitle>{t("cancelAppointment")}</DialogTitle>
            <DialogDescription>
              {tCommon("confirmActionCannotBeUndone")}
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

      {/* Consultation Form from Appointment */}
      {appointmentForConsultation && (
        <ConsultationForm
          isOpen={isConsultationFormOpen}
          onClose={() => setIsConsultationFormOpen(false)}
          onSubmit={handleConsultationFormSubmit}
          initialData={{
            patientId: appointmentForConsultation.patientId,
            doctorId: appointmentForConsultation.medecinId,
            date: appointmentForConsultation.dateHeure,
            time: appointmentForConsultation.time,
            reason: `${tConsultations("consultationFromAppointment")}: ${
              appointmentForConsultation.reason
            }`,
          }}
          patients={patients}
          doctors={doctors}
        />
      )}
    </div>
  );
}

export default AppointmentsPage;
