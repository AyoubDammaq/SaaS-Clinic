import { useState, useCallback } from "react";
import { reportingService } from "@/services/reportingService";
import { DashboardStatsDTO, RevenuTrend } from "@/types/statistics";
import { consultationService } from "@/services/consultationService";
import {
  endOfMonth,
  format,
  parseISO,
  startOfMonth,
  subMonths,
} from "date-fns";
import { Paiement, RecentPaiementDto } from "@/types/billing";

interface TrendDTO {
  value: number;
  isPositive: boolean;
}

interface UseReportingsState {
  dashboardStats: DashboardStatsDTO | null;
  newPatientsByDoctor: number | null;
  newPatientsByClinic: number | null;
  newPatientsTrendByClinic: TrendDTO | null;
  newPatientsTrendByDoctor: TrendDTO | null;
  consultationsCurrentMonthByDoctor: number | null;
  consultationsCurrentMonthByClinic: number | null;
  consultationTrendByDoctor: TrendDTO | null;
  consultationTrendByClinic: TrendDTO | null;
  rendezvousCountByDoctorToday: number | null;
  rendezvousCountByClinicToday: number | null;
  recentPayments: RecentPaiementDto | null;
  pendingAppointmentsByDoctor: number | null;
  pendingAppointmentsByClinic: number | null;
  revenusMensuel: number | null;
  revenusMensuelTrend: TrendDTO | null;
  loading: boolean;
  error: string | null;
  fetchDashboardStats: (
    start: string,
    end: string,
    patientId?: string,
    medecinId?: string,
    cliniqueId?: string
  ) => Promise<void>;
  fetchNewPatientsCountByDoctor: (
    medecinId: string,
    start: string,
    end: string
  ) => Promise<void>;
  fetchNewPatientsCountByClinic: (
    cliniqueId: string,
    start: string,
    end: string
  ) => Promise<void>;
  fetchNewPatientsTrendByClinic: (
    cliniqueId: string,
    start: string,
    end: string
  ) => Promise<void>;
  fetchNewPatientsTrendByDoctor: (
    medecinId: string,
    start: string,
    end: string
  ) => Promise<void>;
  fetchConsultationsCountCurrentMonthByDoctor: (
    medecinId: string
  ) => Promise<void>;
  fetchConsultationsCountCurrentMonthByClinic: (
    cliniqueId: string
  ) => Promise<void>;
  fetchConsultationsTrendCurrentMonthByDoctor: (
    medecinId: string
  ) => Promise<void>;
  fetchConsultationsTrendCurrentMonthByClinic: (
    cliniqueId: string
  ) => Promise<void>;
  fetchRendezvousCountByDoctorCurrentDay: (medecinId: string) => Promise<void>;
  fetchRendezvousCountByClinicCurrentDay: (cliniqueId: string) => Promise<void>;
  calculterNombreConsultationsByPatient: (
    patientId: string,
    startDate: string,
    endDate: string
  ) => Promise<number>;
  calculterNombreConsultationsByDoctor: (
    doctorId: string,
    startDate: string,
    endDate: string
  ) => Promise<number>;
  calculterNombreConsultationsByClinic: (
    cliniqueId: string,
    startDate: string,
    endDate: string
  ) => Promise<number>;
  fetchRecentPaymentsByPatient: (
    patientId: string
  ) => Promise<RecentPaiementDto | null>;
  countPendingAppointmentsByDoctor: (medecinId: string) => Promise<number>;
  countPendingAppointmentsByClinic: (cliniqueId: string) => Promise<number>;
  getRevenuMensuel: (cliniqueId: string) => Promise<number | null>;
  getRevenuMensuelTrend: (cliniqueId: string) => Promise<RevenuTrend | null>;
  getNewClinicsTrend: (start: string, end: string) => Promise<TrendDTO>;
  getNewPatientsTrend: (start: string, end: string) => Promise<TrendDTO>;
  getNewDoctorsTrend: (start: string, end: string) => Promise<TrendDTO>;
  getRevenueTrend: (start: string, end: string) => Promise<TrendDTO>;
}

export function useReportings(): UseReportingsState {
  const [dashboardStats, setDashboardStats] =
    useState<DashboardStatsDTO | null>(null);
  const [newPatientsByDoctor, setNewPatientsByDoctor] = useState<number | null>(
    null
  );
  const [newPatientsByClinic, setNewPatientsByClinic] = useState<number | null>(
    null
  );
  const [newPatientsTrendByClinic, setNewPatientsTrendByClinic] =
    useState<TrendDTO | null>(null);
  const [newPatientsTrendByDoctor, setNewPatientsTrendByDoctor] =
    useState<TrendDTO | null>(null);
  const [
    consultationsCurrentMonthByDoctor,
    setConsultationsCurrentMonthByDoctor,
  ] = useState<number | null>(null);
  const [
    consultationsCurrentMonthByClinic,
    setConsultationsCurrentMonthByClinic,
  ] = useState<number | null>(null);
  const [consultationTrendByDoctor, setConsultationTrendByDoctor] =
    useState<TrendDTO | null>(null);
  const [consultationTrendByClinic, setConsultationTrendByClinic] =
    useState<TrendDTO | null>(null);
  const [rendezvousCountByDoctorToday, setRendezvousCountByDoctorToday] =
    useState<number | null>(null);
  const [rendezvousCountByClinicToday, setRendezvousCountByClinicToday] =
    useState<number | null>(null);
  const [recentPayments, setRecentPayments] =
    useState<RecentPaiementDto | null>(null);
  const [pendingAppointmentsByDoctor, setPendingAppointmentsByDoctor] =
    useState<number | null>(null);
  const [pendingAppointmentsByClinic, setPendingAppointmentsByClinic] =
    useState<number | null>(null);
  const [revenusMensuel, setRevenusMensuel] = useState<number | null>(null);
  const [revenusMensuelTrend, setRevenusMensuelTrend] =
    useState<TrendDTO | null>(null);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  const fetchDashboardStats = useCallback(
    async (
      start: string,
      end: string,
      patientId?: string,
      medecinId?: string,
      cliniqueId?: string
    ) => {
      setLoading(true);
      try {
        const stats = await reportingService.getDashboardStats(
          start,
          end,
          patientId,
          medecinId,
          cliniqueId
        );
        console.log("Dashboard Stats:", stats);
        setDashboardStats(stats);
      } catch (error) {
        setError(
          "Erreur lors de la récupération des données du tableau de bord"
        );
      } finally {
        setLoading(false);
      }
    },
    []
  );

  const calculateTrend = useCallback(
    async (start: string, end: string, key: keyof DashboardStatsDTO) => {
      try {
        // Récupérer les stats actuelles (bien que déjà fetchées, on refetch pour cohérence)
        const currentStats = await reportingService.getDashboardStats(
          start,
          end
        );
        const currentValue = (currentStats[key] as number) ?? 0;

        // Calculer la période précédente (mois précédent)
        const prevStartDate = subMonths(parseISO(start), 1);
        const prevEndDate = subMonths(parseISO(end), 1);
        const prevStart = format(prevStartDate, "yyyy-MM-dd");
        const prevEnd = format(prevEndDate, "yyyy-MM-dd");

        const prevStats = await reportingService.getDashboardStats(
          prevStart,
          prevEnd
        );
        const prevValue = (prevStats[key] as number) ?? 0;

        let trendValue = 0;
        let isPositive = true;

        if (prevValue === 0) {
          if (currentValue > 0) {
            trendValue = 100; // Croissance "infinie" simplifiée à +100%
            isPositive = true;
          }
        } else {
          trendValue = Math.round(
            ((currentValue - prevValue) / prevValue) * 100
          );
          isPositive = trendValue >= 0;
        }

        return { value: Math.abs(trendValue), isPositive };
      } catch (error) {
        console.error("Erreur lors du calcul du trend:", error);
        return { value: 0, isPositive: true }; // Valeur par défaut en cas d'erreur
      }
    },
    []
  );

  const getNewClinicsTrend = useCallback(
    (start: string, end: string) =>
      calculateTrend(start, end, "nouvellesCliniques"),
    [calculateTrend]
  );

  const getNewPatientsTrend = useCallback(
    (start: string, end: string) =>
      calculateTrend(start, end, "nouveauxPatients"),
    [calculateTrend]
  );

  const getNewDoctorsTrend = useCallback(
    (start: string, end: string) =>
      calculateTrend(start, end, "nouveauxMedecins"),
    [calculateTrend]
  );

  // Optionnel : pour les revenus (ajustez la clé si c'est totalFacturesPayees ou autre)
  const getRevenueTrend = useCallback(
    (start: string, end: string) =>
      calculateTrend(start, end, "paiementsPayes"),
    [calculateTrend]
  );

  const fetchNewPatientsCountByDoctor = useCallback(
    async (medecinId: string, start: string, end: string) => {
      setLoading(true);
      try {
        const count = await reportingService.getNewPatientsCountByDoctor(
          medecinId,
          start,
          end
        );
        setNewPatientsByDoctor(count);
      } catch (error) {
        setError(
          "Erreur lors de la récupération des nouveaux patients par médecin"
        );
      } finally {
        setLoading(false);
      }
    },
    []
  );

  const fetchNewPatientsCountByClinic = useCallback(
    async (cliniqueId: string, start: string, end: string) => {
      setLoading(true);
      try {
        const count = await reportingService.getNewPatientsCountByClinic(
          cliniqueId,
          start,
          end
        );
        setNewPatientsByClinic(count);
      } catch (error) {
        setError(
          "Erreur lors de la récupération des nouveaux patients par clinique"
        );
      } finally {
        setLoading(false);
      }
    },
    []
  );

  const fetchNewPatientsTrendByClinic = useCallback(
    async (cliniqueId: string, start: string, end: string) => {
      setLoading(true);
      try {
        const now = new Date();
        const currentStart = startOfMonth(now);
        const currentEnd = endOfMonth(now);
        const prevMonth = subMonths(now, 1);
        const prevStart = startOfMonth(prevMonth);
        const prevEnd = endOfMonth(prevMonth);

        const [currentCount, prevCount] = await Promise.all([
          reportingService.getNewPatientsCountByClinic(
            cliniqueId,
            currentStart.toISOString(),
            currentEnd.toISOString()
          ),
          reportingService.getNewPatientsCountByClinic(
            cliniqueId,
            prevStart.toISOString(),
            prevEnd.toISOString()
          ),
        ]);

        if (prevCount === 0 && currentCount === 0) {
          setNewPatientsTrendByClinic({ value: 0, isPositive: true });
        } else if (prevCount === 0) {
          setNewPatientsTrendByClinic({ value: 100, isPositive: true });
        } else {
          const delta = currentCount - prevCount;
          const percentage = Math.abs(Math.round((delta / prevCount) * 100));
          setNewPatientsTrendByClinic({
            value: percentage,
            isPositive: delta >= 0,
          });
        }
      } catch (error) {
        setError("Erreur lors du calcul de la tendance des patients");
        setNewPatientsTrendByClinic(null);
      } finally {
        setLoading(false);
      }
    },
    []
  );

  const fetchNewPatientsTrendByDoctor = useCallback(
    async (medecinId: string, start: string, end: string) => {
      setLoading(true);
      try {
        const now = new Date();
        const currentStart = startOfMonth(now);
        const currentEnd = endOfMonth(now);
        const prevMonth = subMonths(now, 1);
        const prevStart = startOfMonth(prevMonth);
        const prevEnd = endOfMonth(prevMonth);

        const [currentCount, prevCount] = await Promise.all([
          reportingService.getNewPatientsCountByDoctor(
            medecinId,
            currentStart.toISOString(),
            currentEnd.toISOString()
          ),
          reportingService.getNewPatientsCountByDoctor(
            medecinId,
            prevStart.toISOString(),
            prevEnd.toISOString()
          ),
        ]);

        if (prevCount === 0 && currentCount === 0) {
          setNewPatientsTrendByDoctor({ value: 0, isPositive: true });
        } else if (prevCount === 0) {
          setNewPatientsTrendByDoctor({ value: 100, isPositive: true });
        } else {
          const delta = currentCount - prevCount;
          const percentage = Math.abs(Math.round((delta / prevCount) * 100));
          setNewPatientsTrendByDoctor({
            value: percentage,
            isPositive: delta >= 0,
          });
        }
      } catch (error) {
        setError(
          "Erreur lors du calcul de la tendance des patients par médecin"
        );
        setNewPatientsTrendByDoctor(null);
      } finally {
        setLoading(false);
      }
    },
    []
  );

  const fetchConsultationsCountCurrentMonthByDoctor = useCallback(
    async (medecinId: string) => {
      setLoading(true);
      try {
        const count =
          await reportingService.getConsultationsCountCurrentMonthByDoctor(
            medecinId
          );
        setConsultationsCurrentMonthByDoctor(count);
      } catch (error) {
        setError(
          "Erreur lors de la récupération du nombre de consultations du mois courant"
        );
      } finally {
        setLoading(false);
      }
    },
    []
  );

  const fetchConsultationsCountCurrentMonthByClinic = useCallback(
    async (cliniqueId: string) => {
      setLoading(true);
      try {
        const count =
          await reportingService.getConsultationsCountCurrentMonthByClinic(
            cliniqueId
          );
        setConsultationsCurrentMonthByClinic(count);
      } catch (error) {
        setError(
          "Erreur lors de la récupération du nombre de consultations du mois courant"
        );
      } finally {
        setLoading(false);
      }
    },
    []
  );

  const fetchConsultationsTrendCurrentMonthByDoctor = useCallback(
    async (medecinId: string) => {
      setLoading(true);
      try {
        const consultations =
          await consultationService.getConsultationsByDoctorId(medecinId);
        const now = new Date();
        const currentMonth = now.getMonth();
        const currentYear = now.getFullYear();
        const previousMonth = currentMonth === 0 ? 11 : currentMonth - 1;
        const previousYear = currentMonth === 0 ? currentYear - 1 : currentYear;

        const consultationsThisMonth = consultations.filter((c) => {
          const date = new Date(c.dateConsultation);
          return (
            date.getMonth() === currentMonth &&
            date.getFullYear() === currentYear
          );
        });

        const consultationsLastMonth = consultations.filter((c) => {
          const date = new Date(c.dateConsultation);
          return (
            date.getMonth() === previousMonth &&
            date.getFullYear() === previousYear
          );
        });

        const currentCount = consultationsThisMonth.length;
        const previousCount = consultationsLastMonth.length;

        setConsultationsCurrentMonthByDoctor(currentCount);

        const trendValue = previousCount
          ? Math.round(((currentCount - previousCount) / previousCount) * 100)
          : currentCount > 0
          ? 100
          : 0;

        const isPositive = currentCount >= previousCount;

        setConsultationTrendByDoctor({ value: trendValue, isPositive });
      } catch (error) {
        setError(
          "Erreur lors de la récupération des consultations du mois courant"
        );
      } finally {
        setLoading(false);
      }
    },
    []
  );

  const fetchConsultationsTrendCurrentMonthByClinic = useCallback(
    async (cliniqueId: string) => {
      setLoading(true);
      try {
        const consultations =
          await consultationService.getConsultationsByClinicId(cliniqueId);
        const now = new Date();
        const currentMonth = now.getMonth();
        const currentYear = now.getFullYear();
        const previousMonth = currentMonth === 0 ? 11 : currentMonth - 1;
        const previousYear = currentMonth === 0 ? currentYear - 1 : currentYear;

        const consultationsThisMonth = consultations.filter((c) => {
          const date = new Date(c.dateConsultation);
          return (
            date.getMonth() === currentMonth &&
            date.getFullYear() === currentYear
          );
        });

        const consultationsLastMonth = consultations.filter((c) => {
          const date = new Date(c.dateConsultation);
          return (
            date.getMonth() === previousMonth &&
            date.getFullYear() === previousYear
          );
        });

        const currentCount = consultationsThisMonth.length;
        const previousCount = consultationsLastMonth.length;

        setConsultationsCurrentMonthByClinic(currentCount);

        const trendValue = previousCount
          ? Math.round(((currentCount - previousCount) / previousCount) * 100)
          : currentCount > 0
          ? 100
          : 0;

        const isPositive = currentCount >= previousCount;

        setConsultationTrendByClinic({ value: trendValue, isPositive });
      } catch (error) {
        setError(
          "Erreur lors de la récupération des consultations du mois courant"
        );
      } finally {
        setLoading(false);
      }
    },
    []
  );

  const fetchRendezvousCountByDoctorCurrentDay = useCallback(
    async (medecinId: string) => {
      setLoading(true);
      try {
        const count =
          await reportingService.getAppointmentsCountCurrentDayByDoctor(
            medecinId
          );
        setRendezvousCountByDoctorToday(count);
      } catch (error) {
        setError(
          "Erreur lors de la récupération du nombre de rendez-vous du jour"
        );
      } finally {
        setLoading(false);
      }
    },
    []
  );

  const fetchRendezvousCountByClinicCurrentDay = useCallback(
    async (cliniqueId: string) => {
      setLoading(true);
      try {
        const count =
          await reportingService.getAppointmentsCountCurrentDayByClinic(
            cliniqueId
          );
        setRendezvousCountByClinicToday(count);
      } catch (error) {
        setError(
          "Erreur lors de la récupération du nombre de rendez-vous du jour"
        );
      } finally {
        setLoading(false);
      }
    },
    []
  );

  const calculterNombreConsultationsByPatient = useCallback(
    async (patientId: string, startDate: string, endDate: string) => {
      setLoading(true);
      try {
        const count = await reportingService.countConsultationsByPatient(
          patientId,
          startDate,
          endDate
        );
        return count;
      } catch (error) {
        setError("Erreur lors du comptage des consultations par patient");
        return 0;
      } finally {
        setLoading(false);
      }
    },
    []
  );

  const calculterNombreConsultationsByDoctor = useCallback(
    async (doctorId: string, startDate: string, endDate: string) => {
      setLoading(true);
      try {
        const count = await reportingService.countConsultationsByDoctor(
          doctorId,
          startDate,
          endDate
        );
        return count;
      } catch (error) {
        setError("Erreur lors du comptage des consultations par médecin");
        return 0;
      } finally {
        setLoading(false);
      }
    },
    []
  );

  const calculterNombreConsultationsByClinic = useCallback(
    async (cliniqueId: string, startDate: string, endDate: string) => {
      setLoading(true);
      try {
        const count = await reportingService.countConsultationsByClinic(
          cliniqueId,
          startDate,
          endDate
        );
        return count;
      } catch (error) {
        setError("Erreur lors du comptage des consultations par clinique");
        return 0;
      } finally {
        setLoading(false);
      }
    },
    []
  );

  const fetchRecentPaymentsByPatient = useCallback(
    async (patientId: string) => {
      setLoading(true);
      try {
        const payment = await reportingService.getRecentPaymentsByPatient(
          patientId
        );
        setRecentPayments(payment); // Single payment or null
        return payment;
      } catch (error) {
        setError(
          "Erreur lors de la récupération des paiements récents du patient"
        );
        setRecentPayments(null);
        return null;
      } finally {
        setLoading(false);
      }
    },
    []
  );

  const countPendingAppointmentsByDoctor = useCallback(
    async (medecinId: string) => {
      setLoading(true);
      try {
        const count =
          await reportingService.getPendingAppointmentsCountByDoctor(medecinId);
        setPendingAppointmentsByDoctor(count);
        return count;
      } catch (error) {
        setError("Erreur lors du comptage des rendez-vous en attente");
        return 0;
      } finally {
        setLoading(false);
      }
    },
    []
  );

  const countPendingAppointmentsByClinic = useCallback(
    async (cliniqueId: string) => {
      setLoading(true);
      try {
        const count =
          await reportingService.getPendingAppointmentsCountByClinic(
            cliniqueId
          );
        setPendingAppointmentsByClinic(count);
        return count;
      } catch (error) {
        setError("Erreur lors du comptage des rendez-vous en attente");
        return 0;
      } finally {
        setLoading(false);
      }
    },
    []
  );

  const getRevenuMensuel = useCallback(async (cliniqueId: string) => {
    setLoading(true);
    try {
      const revenu = await reportingService.getRevenusMensuelByClinic(
        cliniqueId
      );
      setRevenusMensuel(revenu);
      return revenu;
    } catch (error) {
      setError("Erreur lors de la récupération du revenu mensuel");
      setRevenusMensuel(null);
      return null;
    } finally {
      setLoading(false);
    }
  }, []);

  const getRevenuMensuelTrend = useCallback(async (cliniqueId: string) => {
    setLoading(true);
    try {
      const trend = await reportingService.getRevenuMensuelTrendByClinic(
        cliniqueId
      );
      setRevenusMensuel(trend.current);
      setRevenusMensuelTrend({
        value: Math.abs(trend.percentageChange),
        isPositive: trend.isPositive,
      });
      return trend;
    } catch (error) {
      setError("Erreur lors de la récupération du trend des revenus");
      setRevenusMensuel(0);
      setRevenusMensuelTrend({ value: 0, isPositive: true });
      return null;
    } finally {
      setLoading(false);
    }
  }, []);

  return {
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
    recentPayments,
    pendingAppointmentsByDoctor,
    pendingAppointmentsByClinic,
    revenusMensuel,
    revenusMensuelTrend,
    loading,
    error,
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
    calculterNombreConsultationsByDoctor,
    calculterNombreConsultationsByClinic,
    fetchRecentPaymentsByPatient,
    countPendingAppointmentsByDoctor,
    countPendingAppointmentsByClinic,
    getRevenuMensuel,
    getRevenuMensuelTrend,
    getNewClinicsTrend,
    getNewPatientsTrend,
    getNewDoctorsTrend,
    getRevenueTrend,
  };
}
