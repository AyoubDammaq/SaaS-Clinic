import { useEffect, useMemo, useState } from "react";
import { useAuth } from "@/hooks/useAuth";
import { useAppointments } from "@/hooks/useAppointments";
import { useDoctors } from "@/hooks/useDoctors";
import { usePatients } from "@/hooks/usePatients";
import { useReportings } from "@/hooks/useReportings"; // Importer le hook
import {
  LineChart,
  Line,
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
} from "recharts";
import {
  Calendar,
  CreditCard,
  Users,
  User,
  Activity,
  Building,
} from "lucide-react";
import { StatCard } from "@/components/dashboard/StatCard";
import { AppointmentList } from "@/components/dashboard/AppointmentList";
import { RecentActivityComponent } from "@/components/dashboard/RecentActivityComponent";
import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
} from "@/components/ui/card";
import { AppointmentStatusEnum, RendezVous } from "@/types/rendezvous";
import { Patient } from "@/types/patient";
import { Doctor } from "@/types/doctor";
import { format, isValid } from "date-fns"; // Pour formater les dates
import { toast } from "sonner";
import {
  AppointmentDayStat,
  StatistiqueDTO,
  RecentActivity,
  DashboardStatsDTO,
  BarChartData,
} from "@/types/statistics";
import { NextAppointmentCard } from "@/components/dashboard/NextAppointmentCard";
import { fr } from "date-fns/locale";
import { RecentPaiementDto } from "@/types/billing";
import { count } from "console";

// Interface Appointment (inchangée)
interface Appointment {
  id: string;
  patientName: string;
  doctorName: string;
  date: string;
  time: string;
  status: AppointmentStatusEnum;
  commentaire?: string;
}

// Fonction pour mapper RendezVous à Appointment (inchangée)
const mapRendezVousToAppointment = (
  rdv: RendezVous,
  patients: Patient[],
  doctors: Doctor[]
): Appointment => {
  const patient = patients.find((p) => p.id === rdv.patientId);
  const doctor = doctors.find((d) => d.id === rdv.medecinId);
  return {
    id: rdv.id,
    patientName:
      rdv.patientNom ||
      (patient ? `${patient.prenom} ${patient.nom}` : "Inconnu"),
    doctorName:
      rdv.medecinNom ||
      (doctor ? `Dr. ${doctor.prenom} ${doctor.nom}` : "Inconnu"),
    date: new Date(rdv.dateHeure).toLocaleDateString("fr-FR", {
      day: "2-digit",
      month: "2-digit",
      year: "numeric",
    }),
    time: new Date(rdv.dateHeure).toLocaleTimeString("fr-FR", {
      hour: "2-digit",
      minute: "2-digit",
      timeZone: "Europe/Paris",
    }),
    status: rdv.statut,
    commentaire: rdv.commentaire || "",
  };
};

// Générer les données pour le graphique des patients (basé sur rendezvousStats)
const generatePatientData = (stats: StatistiqueDTO[]) => {
  const monthlyData: { [key: string]: number } = {};
  const monthsFullName = [
    "January",
    "February",
    "March",
    "April",
    "May",
    "June",
    "July",
    "August",
    "September",
    "October",
    "November",
    "December",
  ];
  const monthsShort = [
    "Jan",
    "Feb",
    "Mar",
    "Apr",
    "May",
    "Jun",
    "Jul",
    "Aug",
    "Sep",
    "Oct",
    "Nov",
    "Dec",
  ];

  stats.forEach((stat) => {
    const monthIndex = monthsFullName.findIndex(
      (m) => m.toLowerCase() === stat.cle.toLowerCase()
    );
    if (monthIndex === -1) {
      console.warn(`Invalid month name in stat key: ${stat.cle}`);
      return;
    }
    const monthKey = monthsShort[monthIndex];
    monthlyData[monthKey] = (monthlyData[monthKey] || 0) + stat.nombre;
  });

  return Object.entries(monthlyData).map(([month, count]) => ({
    month,
    count,
  }));
};

function fillPatientData(
  rawData: { month: string; count: number }[]
): { month: string; count: number }[] {
  const months = [
    "Jan",
    "Feb",
    "Mar",
    "Apr",
    "May",
    "Jun",
    "Jul",
    "Aug",
    "Sep",
    "Oct",
    "Nov",
    "Dec",
  ];

  // Derniers 12 mois à partir de maintenant
  const now = new Date();
  const last12Months = Array.from({ length: 12 }).map((_, i) => {
    const d = new Date(now.getFullYear(), now.getMonth() - (11 - i), 1);
    return months[d.getMonth()];
  });

  return last12Months.map((m) => {
    const existing = rawData.find((item) => item.month === m);
    return existing || { month: m, count: 0 };
  });
}

// Générer les données pour le graphique des rendez-vous (basé sur rendezvousStats)
const generateAppointmentData = (stats: StatistiqueDTO[]) => {
  const days = ["Lun", "Mar", "Mer", "Jeu", "Ven", "Sam", "Dim"];
  const currentWeekStart = new Date();
  currentWeekStart.setDate(
    currentWeekStart.getDate() - currentWeekStart.getDay()
  );

  return days.map((day, index) => {
    const dayDate = new Date(currentWeekStart);
    dayDate.setDate(dayDate.getDate() + index);
    const dateStr = format(dayDate, "yyyy-MM-dd");

    const statsForDay = stats.filter((s) => s.cle === dateStr);

    return {
      name: day,
      scheduled: statsForDay
        .filter((s) => s.cle === "SCHEDULED")
        .reduce((acc, cur) => acc + cur.nombre, 0),
      pending: statsForDay
        .filter((s) => s.cle === "PENDING")
        .reduce((acc, cur) => acc + cur.nombre, 0),
      cancelled: statsForDay
        .filter((s) => s.cle === "CANCELLED")
        .reduce((acc, cur) => acc + cur.nombre, 0),
    };
  });
};

// Générer les activités récentes (inchangée)
const generateRecentActivities = (
  appointments: RendezVous[],
  patients: Patient[],
  doctors: Doctor[]
) => {
  return appointments.slice(0, 4).map((rdv, index) => {
    const patient = patients.find((p) => p.id === rdv.patientId);
    const doctor = doctors.find((d) => d.id === rdv.medecinId);

    const patientName =
      rdv.patientNom ||
      (patient ? `${patient.prenom} ${patient.nom}` : "Inconnu");
    const doctorName =
      rdv.medecinNom ||
      (doctor ? `Dr. ${doctor.prenom} ${doctor.nom}` : "Inconnu");

    return {
      id: rdv.id,
      type: "appointment" as const,
      title:
        rdv.statut === AppointmentStatusEnum.CONFIRME
          ? "Rendez-vous confirmé"
          : "Nouveau rendez-vous",
      description: `${patientName} ${
        rdv.statut === AppointmentStatusEnum.CONFIRME
          ? "a confirmé"
          : "a planifié"
      } un rendez-vous avec ${doctorName}`,
      time: `${index + 1} heure${index > 0 ? "s" : ""} ago`,
      seen: rdv.statut === AppointmentStatusEnum.CONFIRME,
    };
  });
};

const normalizeWeeklyStats = (stats: AppointmentDayStat[]) => {
  const daysOfWeek = [
    "Monday",
    "Tuesday",
    "Wednesday",
    "Thursday",
    "Friday",
    "Saturday",
    "Sunday",
  ];

  // Si stats est undefined, retourner une liste vide ou remplie de 0
  if (!Array.isArray(stats)) {
    return daysOfWeek.map((day) => ({
      jour: day,
      scheduled: 0,
      pending: 0,
      cancelled: 0,
    }));
  }

  return daysOfWeek.map((day) => {
    const stat = stats.find((s) => s.jour === day);
    return {
      jour: day,
      scheduled: stat?.scheduled || 0,
      pending: stat?.pending || 0,
      cancelled: stat?.cancelled || 0,
    };
  });
};


function DashboardPage() {
  const { user } = useAuth();
  const { filteredAppointments, isLoading, setSearchTerm } = useAppointments();
  const { doctors } = useDoctors();
  const { patients } = usePatients();
  const {
    dashboardStats,
    newPatientsByDoctor,
    newPatientsByClinic,
    newPatientsTrendByClinic,
    newPatientsTrendByDoctor,
    consultationsCurrentMonthByDoctor,
    consultationsCurrentMonthByClinic,
    consultationTrendByDoctor,
    consultationTrendByClinic,
    rendezvousCountByDoctorToday,
    rendezvousCountByClinicToday,
    pendingAppointmentsByDoctor,
    pendingAppointmentsByClinic,
    revenusMensuel,
    revenusMensuelTrend,
    loading: statsLoading,
    error: statsError,
    fetchDashboardStats,
    fetchNewPatientsCountByDoctor,
    fetchNewPatientsCountByClinic,
    fetchNewPatientsTrendByClinic,
    fetchNewPatientsTrendByDoctor,
    fetchConsultationsCountCurrentMonthByDoctor,
    fetchConsultationsCountCurrentMonthByClinic,
    fetchConsultationsTrendCurrentMonthByDoctor,
    fetchConsultationsTrendCurrentMonthByClinic,
    fetchRendezvousCountByDoctorCurrentDay,
    fetchRendezvousCountByClinicCurrentDay,
    calculterNombreConsultationsByPatient,
    fetchRecentPaymentsByPatient,
    countPendingAppointmentsByDoctor,
    countPendingAppointmentsByClinic,
    getRevenuMensuel,
    getRevenuMensuelTrend,
    getNewClinicsTrend,
    getNewPatientsTrend,
    getNewDoctorsTrend,
    getRevenueTrend,
  } = useReportings();

  const [clinicTrend, setClinicTrend] = useState<{
    value: number;
    isPositive: boolean;
  } | null>(null);
  const [patientTrend, setPatientTrend] = useState<{
    value: number;
    isPositive: boolean;
  } | null>(null);
  const [doctorTrend, setDoctorTrend] = useState<{
    value: number;
    isPositive: boolean;
  } | null>(null);
  const [revenueTrend, setRevenueTrend] = useState<{
    value: number;
    isPositive: boolean;
  } | null>(null);

  const [recentPayments, setRecentPayments] =
    useState<RecentPaiementDto | null>(null);
  const [nombreConsultations, setNombreConsultations] = useState<number | null>(
    null
  );

  const [dateRange, setDateRange] = useState({
    startDate: format(
      new Date(new Date().setMonth(new Date().getMonth() - 1)),
      "yyyy-MM-dd"
    ),
    endDate: format(new Date(), "yyyy-MM-dd"),
  });

  useEffect(() => {
    if (user?.role === "Patient" && user.patientId) {
      const { startDate, endDate } = dateRange;
      calculterNombreConsultationsByPatient(user.patientId, startDate, endDate)
        .then((count) => setNombreConsultations(count))
        .catch((error) => {
          toast.error("Erreur lors du chargement du nombre de consultations");
          setNombreConsultations(0);
        });
      fetchRecentPaymentsByPatient(user.patientId)
        .then((payment) => {
          console.log("Recent Payment:", payment);
          // Si payment est null ou vide (statut 204), définir recentPayments à null
          setRecentPayments(payment || null);
        })
        .catch((error) => {
          console.error("Error fetching recent payments:", error);
          setRecentPayments(null); // En cas d'erreur, définir à null
        });
    }
  }, [
    user,
    dateRange,
    calculterNombreConsultationsByPatient,
    fetchRecentPaymentsByPatient,
  ]);

  useEffect(() => {
    if (user) {
      const { startDate, endDate } = dateRange;
      const patientId = user.role === "Patient" ? user.patientId : undefined;
      const medecinId = user.role === "Doctor" ? user.medecinId : undefined;
      const cliniqueId =
        user.role === "ClinicAdmin" ? user.cliniqueId : undefined;

      fetchDashboardStats(startDate, endDate, patientId, medecinId, cliniqueId)
        .then((data) => {
          if (user.role === "Doctor" && medecinId) {
            fetchNewPatientsCountByDoctor(medecinId, startDate, endDate);
            fetchNewPatientsTrendByDoctor(medecinId, startDate, endDate);
            fetchConsultationsCountCurrentMonthByDoctor(medecinId);
            fetchConsultationsTrendCurrentMonthByDoctor(medecinId);
            fetchRendezvousCountByDoctorCurrentDay(medecinId);
            countPendingAppointmentsByDoctor(medecinId);
          }

          if (user.role === "ClinicAdmin" && user.cliniqueId) {
            fetchNewPatientsCountByClinic(user.cliniqueId, startDate, endDate);
            fetchConsultationsCountCurrentMonthByClinic(user.cliniqueId);
            fetchConsultationsTrendCurrentMonthByClinic(user.cliniqueId);
            fetchRendezvousCountByClinicCurrentDay(user.cliniqueId);
            fetchNewPatientsTrendByClinic(user.cliniqueId, startDate, endDate);
            countPendingAppointmentsByClinic(user.cliniqueId);
            getRevenuMensuel(user.cliniqueId);
            getRevenuMensuelTrend(user.cliniqueId);
          }

          if (user.role === "SuperAdmin") {
            getNewClinicsTrend(startDate, endDate).then(setClinicTrend);
            getNewPatientsTrend(startDate, endDate).then(setPatientTrend);
            getNewDoctorsTrend(startDate, endDate).then(setDoctorTrend);
            getRevenueTrend(startDate, endDate).then(setRevenueTrend);
          }
        })
        .catch((error) => {
          toast.error("Erreur lors du chargement des statistiques");
        });
    }
  }, [
    user,
    dateRange,
    fetchDashboardStats,
    fetchNewPatientsCountByDoctor,
    fetchNewPatientsCountByClinic,
    fetchNewPatientsTrendByClinic,
    fetchNewPatientsTrendByDoctor,
    fetchConsultationsCountCurrentMonthByDoctor,
    fetchConsultationsCountCurrentMonthByClinic,
    fetchConsultationsTrendCurrentMonthByDoctor,
    fetchConsultationsTrendCurrentMonthByClinic,
    fetchRendezvousCountByDoctorCurrentDay,
    fetchRendezvousCountByClinicCurrentDay,
    calculterNombreConsultationsByPatient,
    fetchRecentPaymentsByPatient,
    countPendingAppointmentsByDoctor,
    countPendingAppointmentsByClinic,
    getRevenuMensuel,
    getRevenuMensuelTrend,
    getNewClinicsTrend,
    getNewPatientsTrend,
    getNewDoctorsTrend,
    getRevenueTrend,
  ]);

  useEffect(() => {
    setSearchTerm("");
  }, [setSearchTerm]);

  const roleFilteredAppointments = useMemo(() => {
    if (!user) return [];
    let filtered = [...filteredAppointments];
    if (user.role === "ClinicAdmin") {
      const doctorIds = doctors
        .filter((d) => d.cliniqueId === user.cliniqueId)
        .map((d) => d.id);
      filtered = filtered.filter((rdv) => doctorIds.includes(rdv.medecinId));
    } else if (user.role === "Doctor") {
      filtered = filtered.filter((rdv) => rdv.medecinId === user.medecinId);
    } else if (user.role === "Patient") {
      filtered = filtered.filter((rdv) => rdv.patientId === user.patientId);
    }
    const now = new Date();
    filtered = filtered.filter((rdv) => {
      const appointmentDate = new Date(rdv.dateHeure);
      return appointmentDate >= now;
    });
    return filtered;
  }, [user, filteredAppointments, doctors]);

  const sortedAppointments = useMemo(() => {
    return roleFilteredAppointments
      .sort(
        (a, b) =>
          new Date(a.dateHeure).getTime() - new Date(b.dateHeure).getTime()
      )
      .map((rdv) => mapRendezVousToAppointment(rdv, patients, doctors))
      .slice(0, 6);
  }, [roleFilteredAppointments, patients, doctors]);

  const patientData = useMemo(
    () => generatePatientData(dashboardStats?.nouveauxPatientsParMois || []),
    [dashboardStats]
  );

  const appointmentDataForDoctor = useMemo(
    () =>
      normalizeWeeklyStats(
        dashboardStats?.WeeklyAppointmentStatsByDoctor || []
      ),
    [dashboardStats]
  );

  const appointmentDataForClinic = useMemo(
    () =>
      normalizeWeeklyStats(
        dashboardStats?.WeeklyAppointmentStatsByClinic || []
      ),
    [dashboardStats]
  );

  const recentActivities = useMemo(
    () => generateRecentActivities(roleFilteredAppointments, patients, doctors),
    [roleFilteredAppointments, patients, doctors]
  );

  const doctorsBySpecialtyChart = useMemo(() => {
    const specialties = dashboardStats?.doctorsBySpecialty || [];
    return {
      type: "bar",
      data: specialties.map((stat) => ({
        name: stat.cle || "Inconnu",
        value: stat.nombre || 0,
      })),
    };
  }, [dashboardStats]);

  const dashboardContent = useMemo(() => {
    if (!user) return null;
    switch (user.role) {
      case "SuperAdmin":
        return (
          <SuperAdminDashboard
            patientData={patientData}
            dashboardStats={dashboardStats}
            doctorsBySpecialtyChart={doctorsBySpecialtyChart}
            newPatientsCount={dashboardStats?.totalPatients || 0}
            clinicTrend={clinicTrend}
            patientTrend={patientTrend}
            doctorTrend={doctorTrend}
            revenueTrend={revenueTrend}
          />
        );
      case "ClinicAdmin":
        return (
          <ClinicAdminDashboard
            appointments={sortedAppointments}
            appointmentData={appointmentDataForClinic}
            recentActivities={recentActivities}
            dashboardStats={rendezvousCountByClinicToday}
            consultationCount={consultationsCurrentMonthByClinic}
            newPatientsByClinic={newPatientsByClinic}
            newPatientsTrend={newPatientsTrendByClinic}
            consultationsTrend={
              consultationTrendByClinic || { value: 0, isPositive: true }
            }
            pendingAppointmentsByClinic={pendingAppointmentsByClinic}
            revenusMensuel={revenusMensuel}
            revenusMensuelTrend={revenusMensuelTrend}
          />
        );
      case "Doctor":
        return (
          <DoctorDashboard
            appointments={sortedAppointments}
            appointmentData={appointmentDataForDoctor}
            dashboardStats={rendezvousCountByDoctorToday}
            consultationCount={consultationsCurrentMonthByDoctor}
            newPatientsByDoctor={newPatientsByDoctor}
            newPatientsTrend={
              newPatientsTrendByDoctor || { value: 0, isPositive: true }
            }
            consultationsTrend={
              consultationTrendByDoctor || { value: 0, isPositive: true }
            }
            pendingAppointmentsByDoctor={pendingAppointmentsByDoctor}
          />
        );
      case "Patient":
        return (
          <PatientDashboard
            appointments={sortedAppointments}
            nombreConsultations={nombreConsultations || 0}
            recentActivities={recentActivities}
            recentPayments={recentPayments}
          />
        );
      default:
        return <div>Rôle inconnu</div>;
    }
  }, [
    user,
    sortedAppointments,
    patientData,
    appointmentDataForDoctor,
    appointmentDataForClinic,
    rendezvousCountByDoctorToday,
    rendezvousCountByClinicToday,
    recentActivities,
    dashboardStats,
    doctorsBySpecialtyChart,
    consultationsCurrentMonthByDoctor,
    consultationsCurrentMonthByClinic,
    newPatientsByDoctor,
    newPatientsByClinic,
    newPatientsTrendByClinic,
    newPatientsTrendByDoctor,
    consultationTrendByDoctor,
    consultationTrendByClinic,
    nombreConsultations,
    recentPayments,
    pendingAppointmentsByDoctor,
    pendingAppointmentsByClinic,
    revenusMensuel,
    revenusMensuelTrend,
    clinicTrend,
    patientTrend,
    doctorTrend,
    revenueTrend,
  ]);

  useEffect(() => {
    if (statsError) {
      toast.error(statsError);
    }
  }, [statsError]);

  return (
    <div className="dashboard-container space-y-6 pb-8">
      {isLoading || statsLoading ? (
        <div>Chargement...</div>
      ) : (
        <>
          <div className="flex flex-col gap-2">
            <h1 className="text-3xl font-bold tracking-tight">
              Tableau de bord
            </h1>
            <p className="text-muted-foreground">Bienvenue, {user?.name}</p>
          </div>
          {dashboardContent}
        </>
      )}
    </div>
  );
}

function SuperAdminDashboard({
  patientData,
  dashboardStats,
  doctorsBySpecialtyChart,
  newPatientsCount,
  clinicTrend,
  patientTrend,
  doctorTrend,
  revenueTrend,
}: {
  patientData: { month: string; count: number }[];
  dashboardStats: DashboardStatsDTO | null;
  doctorsBySpecialtyChart: { type: string; data: BarChartData };
  newPatientsCount: number;
  clinicTrend?: { value: number; isPositive: boolean } | null;
  patientTrend?: { value: number; isPositive: boolean } | null;
  doctorTrend?: { value: number; isPositive: boolean } | null;
  revenueTrend?: { value: number; isPositive: boolean } | null;
}) {
  if (!dashboardStats || !patientData || !doctorsBySpecialtyChart) {
    return <div>Chargement des données...</div>;
  }
  return (
    <div className="space-y-6">
      <div className="grid gap-4 grid-cols-1 sm:grid-cols-2 lg:grid-cols-4">
        <StatCard
          title="Total Cliniques"
          value={`${dashboardStats?.totalClinics?.toString() || 0}`}
          icon={<Building className="h-4 w-4" />}
          trend={clinicTrend ?? { value: 0, isPositive: true }}
        />
        <StatCard
          title="Total Patients"
          value={(newPatientsCount ?? 0).toString()}
          icon={<Users className="h-4 w-4" />}
          trend={patientTrend ?? { value: 0, isPositive: true }}
        />
        <StatCard
          title="Total Doctors"
          value={
            dashboardStats?.doctorsBySpecialty
              ?.reduce((sum, stat) => sum + stat.nombre, 0)
              ?.toString() || "0"
          }
          icon={<User className="h-4 w-4" />}
          trend={doctorTrend ?? { value: 0, isPositive: true }}
        />
        <StatCard
          title="Revenus Totaux"
          value={`$${dashboardStats.totalFacturesPayees ?? 0}`}
          icon={<CreditCard className="h-4 w-4" />}
          trend={revenueTrend ?? { value: 0, isPositive: true }}
        />
      </div>

      <div className="grid gap-6 grid-cols-1 lg:grid-cols-2">
        <Card>
          <CardHeader>
            <CardTitle>Clinic Growth</CardTitle>
            <CardDescription>
              New patient registrations across all clinics
            </CardDescription>
          </CardHeader>
          <CardContent className="h-80">
            <ResponsiveContainer width="100%" height="100%">
              <LineChart data={fillPatientData(patientData || [])}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="month" />
                <YAxis />
                <Tooltip />
                <Line
                  type="monotone"
                  dataKey="count"
                  stroke="#0073b6"
                  strokeWidth={2}
                />
              </LineChart>
            </ResponsiveContainer>
          </CardContent>
        </Card>
        <Card>
          <CardHeader>
            <CardTitle>Répartition des Médecins</CardTitle>
            <CardDescription>
              Nombre de médecins par spécialité enregistrés sur la plateforme
            </CardDescription>
          </CardHeader>
          <CardContent className="h-80">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart
                data={doctorsBySpecialtyChart.data || []}
                margin={{ top: 20, right: 30, left: 20, bottom: 50 }}
              >
                <CartesianGrid strokeDasharray="3 3" stroke="#ccc" />
                <XAxis
                  dataKey="name"
                  angle={-20}
                  textAnchor="end"
                  interval={0}
                  tick={{ fontSize: 12 }}
                />
                <YAxis allowDecimals={false} />
                <Tooltip
                  formatter={(value) => [`${value} médecin(s)`, "Total"]}
                />
                <Bar
                  dataKey="value"
                  fill="#0073b6"
                  radius={[6, 6, 0, 0]}
                  barSize={40}
                  label={{ position: "top", fill: "#333", fontSize: 12 }}
                />
              </BarChart>
            </ResponsiveContainer>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}

function ClinicAdminDashboard({
  appointments,
  appointmentData,
  recentActivities,
  dashboardStats,
  consultationCount,
  newPatientsByClinic,
  newPatientsTrend = { value: 0, isPositive: true },
  consultationsTrend,
  pendingAppointmentsByClinic = 0,
  revenusMensuel = 0.0,
  revenusMensuelTrend = { value: 0, isPositive: true },
}) {
  return (
    <div className="space-y-6">
      <div className="grid gap-4 grid-cols-1 sm:grid-cols-2 lg:grid-cols-4">
        <StatCard
          title="Nouveaux Patients"
          value={newPatientsByClinic?.toString() || "0"}
          icon={<Users className="h-4 w-4" />}
          trend={newPatientsTrend || { value: 0, isPositive: true }}
        />
        <StatCard
          title="Consultations"
          value={consultationCount?.toString() || "0"}
          icon={<Activity className="h-4 w-4" />}
          trend={consultationsTrend || { value: 0, isPositive: true }}
        />
        <StatCard
          title="Rendez-vous Aujourd'hui"
          value={dashboardStats || "0"}
          icon={<Calendar className="h-4 w-4" />}
          description={`${
            pendingAppointmentsByClinic?.toString() || "0"
          } en attente de confirmation`}
        />
        <StatCard
          title="Revenus Mensuels"
          value={`$${revenusMensuel || 0}`}
          icon={<CreditCard className="h-4 w-4" />}
          trend={revenusMensuelTrend} // Use the actual trend data
        />
      </div>

      <div className="grid gap-6 grid-cols-1 lg:grid-cols-2">
        <AppointmentList appointments={appointments} />
        <RecentActivityComponent activities={recentActivities} />
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Activité Hebdomadaire</CardTitle>
          <CardDescription>
            Statistiques des rendez-vous pour votre clinique
          </CardDescription>
        </CardHeader>
        <CardContent className="h-80">
          <ResponsiveContainer width="100%" height="100%">
            <BarChart data={appointmentData}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="jour" />
              <YAxis />
              <Tooltip />
              <Bar dataKey="scheduled" fill="#0D98BA" />
              <Bar dataKey="pending" fill="#98FF98" />
              <Bar dataKey="cancelled" fill="#FF0000" />
            </BarChart>
          </ResponsiveContainer>
        </CardContent>
      </Card>
    </div>
  );
}

function DoctorDashboard({
  appointments,
  appointmentData,
  dashboardStats,
  consultationCount,
  newPatientsByDoctor,
  newPatientsTrend = { value: 0, isPositive: true },
  consultationsTrend = { value: 0, isPositive: true },
  pendingAppointmentsByDoctor,
}) {
  return (
    <div className="space-y-6">
      <div className="grid gap-4 grid-cols-1 sm:grid-cols-2 lg:grid-cols-3">
        <StatCard
          title="Rendez-vous Aujourd'hui"
          value={dashboardStats || "0"}
          icon={<Calendar className="h-4 w-4" />}
          description={`${
            pendingAppointmentsByDoctor?.toString() || "0"
          } en attente de confirmation`}
        />
        <StatCard
          title="Nouveaux Patients"
          value={newPatientsByDoctor?.toString() || "0"}
          icon={<Users className="h-4 w-4" />}
          trend={newPatientsTrend}
          description="Ce mois"
        />
        <StatCard
          title="Consultations Terminées"
          value={consultationCount?.toString() || "0"}
          icon={<Activity className="h-4 w-4" />}
          trend={consultationsTrend || { value: 0, isPositive: true }}
          description="Ce mois"
        />
      </div>

      <NextAppointmentCard appointment={appointments?.[0] || null} />

      <AppointmentList appointments={appointments} />

      <Card>
        <CardHeader>
          <CardTitle>Planning Hebdomadaire</CardTitle>
          <CardDescription>
            Votre charge de rendez-vous pour la semaine
          </CardDescription>
        </CardHeader>
        <CardContent className="h-80">
          <ResponsiveContainer width="100%" height="100%">
            <BarChart data={appointmentData}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="jour" />
              <YAxis />
              <Tooltip />
              <Bar dataKey="scheduled" fill="#0D98BA" />
              <Bar dataKey="pending" fill="#98FF98" />
              <Bar dataKey="cancelled" fill="#FF0000" />
            </BarChart>
          </ResponsiveContainer>
        </CardContent>
      </Card>
    </div>
  );
}

function PatientDashboard({
  nombreConsultations,
  appointments,
  recentActivities,
  recentPayments,
}: {
  nombreConsultations: number;
  appointments: Appointment[];
  recentActivities: RecentActivity[];
  recentPayments: RecentPaiementDto | null;
}) {
  const totalPayments =
    recentPayments && typeof recentPayments.montant === "number"
      ? recentPayments.montant
      : 0;
  const latestPaymentDate = recentPayments?.datePaiement
    ? isValid(new Date(recentPayments.datePaiement))
      ? format(new Date(recentPayments.datePaiement), "d MMMM yyyy", {
          locale: fr,
        })
      : "Date invalide"
    : "Aucun paiement";

  return (
    <div className="space-y-6">
      <div className="grid gap-4 grid-cols-1 sm:grid-cols-2 lg:grid-cols-3">
        <StatCard
          title="Rendez-vous à venir"
          value={appointments
            .filter((rdv) => rdv.status !== AppointmentStatusEnum.ANNULE)
            .length.toString()}
          icon={<Calendar className="h-4 w-4" />}
          description={
            appointments[0]
              ? `Prochain : ${appointments[0].date}, ${appointments[0].time}`
              : "Aucun rendez-vous à venir"
          }
        />
        <StatCard
          title="Consultations Terminées"
          value={nombreConsultations?.toString() || "0"}
          icon={<Activity className="h-4 w-4" />}
          description="Depuis l'inscription"
        />
        <StatCard
          title="Paiements Récents"
          value={`€${totalPayments.toFixed(2)}`}
          icon={<CreditCard className="h-4 w-4" />}
          description={`Dernier paiement : ${latestPaymentDate}`}
        />
      </div>

      <div className="grid gap-6 grid-cols-1 lg:grid-cols-2">
        <AppointmentList appointments={appointments} />
        <RecentActivityComponent activities={recentActivities} />
      </div>
    </div>
  );
}

export default DashboardPage;
