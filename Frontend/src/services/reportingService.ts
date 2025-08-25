import { API_ENDPOINTS } from "@/config/api";
import { api } from "@/utils/apiClient";
import { toast } from "sonner";

import {
  StatistiqueDTO,
  ActiviteMedecinDTO,
  ComparaisonCliniqueDTO,
  DashboardStatsDTO,
  AppointmentDayStat,
  RevenuTrend,
} from "@/types/statistics"; // à ajuster selon vos types réels
import { StatistiqueCliniqueDTO } from "@/types/clinic";
import { endOfMonth, isWithinInterval, parseISO, startOfMonth } from "date-fns";
import { Consultation } from "@/types/consultation";
import { Paiement, RecentPaiementDto } from "@/types/billing";

export const reportingService = {
  // Consultation count
  getConsultationCount: async (start: string, end: string): Promise<number> => {
    try {
      return await api.get<number>(
        API_ENDPOINTS.REPORTS.COUNT_CONSULTATIONS(start, end)
      );
    } catch (error) {
      toast.error("Échec lors de la récupération du nombre de consultations");
      throw error;
    }
  },

  // Appointment stats
  getRendezvousStats: async (
    start: string,
    end: string
  ): Promise<StatistiqueDTO[]> => {
    try {
      return await api.get<StatistiqueDTO[]>(
        API_ENDPOINTS.REPORTS.STATS_RENDEZVOUS(start, end)
      );
    } catch (error) {
      toast.error("Échec lors de la récupération des statistiques de RDV");
      throw error;
    }
  },

  // New patients
  getNewPatientsCount: async (start: string, end: string): Promise<number> => {
    try {
      return await api.get<number>(
        API_ENDPOINTS.REPORTS.COUNT_NEW_PATIENTS(start, end)
      );
    } catch (error) {
      toast.error("Échec lors de la récupération des nouveaux patients");
      throw error;
    }
  },

  // Doctors by specialty / clinic
  getDoctorsBySpecialty: async (): Promise<StatistiqueDTO[]> => {
    try {
      return await api.get(API_ENDPOINTS.REPORTS.COUNT_DOCTORS_BY_SPECIALTY);
    } catch (error) {
      toast.error("Échec lors de la récupération des médecins par spécialité");
      throw error;
    }
  },

  getDoctorsByClinic: async (): Promise<StatistiqueDTO[]> => {
    try {
      return await api.get(API_ENDPOINTS.REPORTS.COUNT_DOCTORS_BY_CLINIC);
    } catch (error) {
      toast.error("Échec lors de la récupération des médecins par clinique");
      throw error;
    }
  },

  getDoctorsBySpecialtyInClinic: async (
    cliniqueId: string
  ): Promise<StatistiqueDTO[]> => {
    try {
      return await api.get(
        API_ENDPOINTS.REPORTS.COUNT_DOCTORS_BY_SPECIALTY_IN_CLINIC(cliniqueId)
      );
    } catch (error) {
      toast.error(
        "Échec lors de la récupération des médecins par spécialité pour la clinique"
      );
      throw error;
    }
  },

  // Billing stats
  getFacturesByStatus: async (): Promise<StatistiqueDTO[]> => {
    try {
      return await api.get(API_ENDPOINTS.REPORTS.COUNT_FACTURES_BY_STATUS);
    } catch (error) {
      toast.error("Échec lors de la récupération des factures par statut");
      throw error;
    }
  },

  getFacturesByClinic: async (): Promise<StatistiqueDTO[]> => {
    try {
      return await api.get(API_ENDPOINTS.REPORTS.COUNT_FACTURES_BY_CLINIC);
    } catch (error) {
      toast.error("Échec lors de la récupération des factures par clinique");
      throw error;
    }
  },

  getFacturesByStatusAndClinic: async (): Promise<StatistiqueDTO[]> => {
    try {
      return await api.get(
        API_ENDPOINTS.REPORTS.COUNT_FACTURES_BY_STATUS_AND_CLINIC
      );
    } catch (error) {
      toast.error(
        "Échec lors de la récupération des factures par statut et clinique"
      );
      throw error;
    }
  },

  getFacturesByStatusInClinic: async (
    cliniqueId: string
  ): Promise<StatistiqueDTO[]> => {
    try {
      return await api.get(
        API_ENDPOINTS.REPORTS.COUNT_FACTURES_BY_STATUS_IN_CLINIC(cliniqueId)
      );
    } catch (error) {
      toast.error(
        "Échec lors de la récupération des factures pour cette clinique"
      );
      throw error;
    }
  },

  // Clinics stats
  getClinicStats: async (
    cliniqueId: string
  ): Promise<StatistiqueCliniqueDTO> => {
    try {
      return await api.get(API_ENDPOINTS.REPORTS.GET_CLINIC_STATS(cliniqueId));
    } catch (error) {
      toast.error("Échec lors de la récupération des stats de la clinique");
      throw error;
    }
  },

  getTotalClinics: async (): Promise<number> => {
    try {
      return await api.get(API_ENDPOINTS.REPORTS.COUNT_CLINICS);
    } catch (error) {
      toast.error("Échec lors de la récupération du nombre de cliniques");
      throw error;
    }
  },

  getNewClinics: async (): Promise<number> => {
    try {
      return await api.get(API_ENDPOINTS.REPORTS.COUNT_NEW_CLINICS);
    } catch (error) {
      toast.error("Échec lors de la récupération des nouvelles cliniques");
      throw error;
    }
  },

  getNewClinicsByMonth: async (): Promise<StatistiqueDTO[]> => {
    try {
      return await api.get(API_ENDPOINTS.REPORTS.COUNT_NEW_CLINICS_BY_MONTH);
    } catch (error) {
      toast.error("Échec lors de la récupération des nouvelles cliniques");
      throw error;
    }
  },

  // Doctor activities
  getDoctorActivities: async (
    medecinId: string
  ): Promise<ActiviteMedecinDTO[]> => {
    try {
      return await api.get(API_ENDPOINTS.REPORTS.DOCTOR_ACTIVITIES(medecinId));
    } catch (error) {
      toast.error("Échec lors de la récupération des activités du médecin");
      throw error;
    }
  },

  // Payment amount by status
  getPaymentAmount: async (
    statut: string,
    start: string,
    end: string
  ): Promise<number> => {
    try {
      return await api.get(
        API_ENDPOINTS.REPORTS.PAYMENT_AMOUNT(statut, start, end)
      );
    } catch (error) {
      toast.error("Échec lors de la récupération des paiements");
      throw error;
    }
  },

  // Facture statistics
  getFactureStats: async (
    start: string,
    end: string
  ): Promise<StatistiqueDTO[]> => {
    try {
      return await api.get(API_ENDPOINTS.REPORTS.FACTURE_STATS(start, end));
    } catch (error) {
      toast.error("Échec lors de la récupération des statistiques de factures");
      throw error;
    }
  },

  // Clinic comparison
  compareClinics: async (): Promise<ComparaisonCliniqueDTO[]> => {
    try {
      return await api.get(API_ENDPOINTS.REPORTS.COMPARE_CLINICS);
    } catch (error) {
      toast.error("Échec lors de la comparaison des cliniques");
      throw error;
    }
  },

  // Dashboard
  getDashboardStats: async (
    start: string,
    end: string,
    patientId?: string,
    medecinId?: string,
    cliniqueId?: string
  ): Promise<DashboardStatsDTO> => {
    try {
      const url = API_ENDPOINTS.REPORTS.DASHBOARD_STATS(
        start,
        end,
        patientId,
        medecinId,
        cliniqueId
      );
      return await api.get<DashboardStatsDTO>(url);
    } catch (error) {
      toast.error(
        "Échec lors du chargement des statistiques du tableau de bord"
      );
      throw error;
    }
  },

  downloadDashboardPdf: async (start: string, end: string): Promise<Blob> => {
    const url = API_ENDPOINTS.REPORTS.DASHBOARD_PDF(start, end);
    try {
      const response = await fetch(url, {
        method: "GET",
        headers: {
          Authorization: `Bearer ${localStorage.getItem("accessToken")}`, // si nécessaire
        },
      });

      if (!response.ok) {
        toast.error("Échec lors du téléchargement du rapport PDF");
        throw new Error("Failed to download dashboard PDF");
      }

      return response.blob();
    } catch (error) {
      toast.error("Échec lors du téléchargement du rapport PDF");
      throw error;
    }
  },

  downloadDashboardExcel: async (start: string, end: string): Promise<Blob> => {
    const url = API_ENDPOINTS.REPORTS.DASHBOARD_EXCEL(start, end);
    try {
      const response = await fetch(url, {
        method: "GET",
        headers: {
          Authorization: `Bearer ${localStorage.getItem("accessToken")}`, // si nécessaire
        },
      });

      if (!response.ok) {
        toast.error("Échec lors du téléchargement du rapport Excel");
        throw new Error("Failed to download dashboard Excel");
      }

      return response.blob();
    } catch (error) {
      toast.error("Échec lors du téléchargement du rapport Excel");
      throw error;
    }
  },

  getWeeklyAppointmentsStatsByDoctor: async (
    medecinId: string,
    start: string,
    end: string
  ): Promise<AppointmentDayStat[]> => {
    try {
      return await api.get<AppointmentDayStat[]>(
        API_ENDPOINTS.REPORTS.WEEKLY_APPOINTMENTS_STATS_BY_DOCTOR(
          medecinId,
          start,
          end
        )
      );
    } catch (error) {
      toast.error("Échec lors de la récupération du planning hebdomadaire");
      throw error;
    }
  },

  getWeeklyAppointmentsStatsByClinic: async (
    cliniqueId: string,
    start: string,
    end: string
  ): Promise<AppointmentDayStat[]> => {
    try {
      return await api.get<AppointmentDayStat[]>(
        API_ENDPOINTS.REPORTS.WEEKLY_APPOINTMENTS_STATS_BY_CLINIC(
          cliniqueId,
          start,
          end
        )
      );
    } catch (error) {
      toast.error("Échec lors de la récupération du planning hebdomadaire");
      throw error;
    }
  },

  getNewPatientsCountByDoctor: async (
    medecinId: string,
    start: string,
    end: string
  ): Promise<number> => {
    try {
      return await api.get<number>(
        API_ENDPOINTS.CONSULTATIONS.GET_NOUVEAUX_PATIENTS_COUNT_BY_DOCTOR(
          medecinId,
          start,
          end
        )
      );
    } catch (error) {
      toast.error("Échec lors de la récupération des nouveaux patients");
      throw error;
    }
  },

  getNewPatientsCountByClinic: async (
    cliniqueId: string,
    start: string,
    end: string
  ): Promise<number> => {
    try {
      return await api.get<number>(
        API_ENDPOINTS.CONSULTATIONS.GET_NOUVEAUX_PATIENTS_COUNT_BY_CLINIC(
          cliniqueId,
          start,
          end
        )
      );
    } catch (error) {
      toast.error("Échec lors de la récupération des nouveaux patients");
      throw error;
    }
  },

  async getConsultationsCountCurrentMonthByDoctor(
    medecinId: string
  ): Promise<number> {
    // Appel API
    const response = await api.get<Consultation[]>(
      API_ENDPOINTS.CONSULTATIONS.GET_BY_DOCTOR_ID(medecinId)
    );

    // Si api.get renvoie un objet avec une propriété `data` contenant la liste
    const consultations = response;

    const start = startOfMonth(new Date());
    const end = endOfMonth(new Date());

    // Filtrer et compter les consultations du mois courant
    const count = consultations.filter((consult) => {
      // dateConsultation est la bonne propriété pour la date dans Consultation
      const consultationDate = parseISO(consult.dateConsultation);
      return isWithinInterval(consultationDate, { start, end });
    }).length;

    return count;
  },

  async getConsultationsCountCurrentMonthByClinic(
    cliniqueId: string
  ): Promise<number> {
    // Appel API
    const response = await api.get<Consultation[]>(
      API_ENDPOINTS.CONSULTATIONS.GET_BY_CLINIC_ID(cliniqueId)
    );

    // Si api.get renvoie un objet avec une propriété `data` contenant la liste
    const consultations = response;

    const start = startOfMonth(new Date());
    const end = endOfMonth(new Date());

    // Filtrer et compter les consultations du mois courant
    const count = consultations.filter((consult) => {
      // dateConsultation est la bonne propriété pour la date dans Consultation
      const consultationDate = parseISO(consult.dateConsultation);
      return isWithinInterval(consultationDate, { start, end });
    }).length;

    return count;
  },

  async getAppointmentsCountCurrentDayByDoctor(
    medecinId: string
  ): Promise<number> {
    const response = await api.get<Consultation[]>(
      API_ENDPOINTS.APPOINTMENTS.GET_BY_DOCTOR_ID(medecinId)
    );

    // Si response est null/undefined, on remplace par un tableau vide
    const consultations = response ?? [];

    if (consultations.length === 0) {
      return 0; // liste vide => count = 0
    }

    const today = new Date();
    const start = new Date(
      today.getFullYear(),
      today.getMonth(),
      today.getDate()
    );
    const end = new Date(
      today.getFullYear(),
      today.getMonth(),
      today.getDate() + 1
    );

    const count = consultations.filter((consult) => {
      if (!consult?.dateConsultation) return false; // ignore si date non définie
      const consultationDate = parseISO(consult.dateConsultation);
      return isWithinInterval(consultationDate, { start, end });
    }).length;

    return count;
  },

  async getAppointmentsCountCurrentDayByClinic(
    cliniqueId: string
  ): Promise<number> {
    // Appel API
    const response = await api.get<Consultation[]>(
      API_ENDPOINTS.CONSULTATIONS.GET_BY_CLINIC_ID(cliniqueId)
    );
    // Si api.get renvoie un objet avec une propriété `data` contenant la liste
    const consultations = response;
    const today = new Date();
    const start = new Date(
      today.getFullYear(),
      today.getMonth(),
      today.getDate()
    );
    const end = new Date(
      today.getFullYear(),
      today.getMonth(),
      today.getDate() + 1
    );
    // Filtrer et compter les consultations du jour
    const count = consultations.filter((consult) => {
      // dateConsultation est la bonne propriété pour la date dans Consultation
      const consultationDate = parseISO(consult.dateConsultation);
      return isWithinInterval(consultationDate, { start, end });
    }).length;
    return count;
  },

  async getCountRendezVousByClinicToday(
    cliniqueId: string,
    date: Date
  ): Promise<number> {
    try {
      const formattedDate = date.toISOString().split("T")[0]; // format 'yyyy-MM-dd'
      return await api.get<number>(
        API_ENDPOINTS.APPOINTMENTS.COUNT_BY_CLINIC(cliniqueId, formattedDate)
      );
    } catch (error) {
      toast.error("Échec lors de la récupération des RDV pour la clinique");
      throw error;
    }
  },

  async countConsultationsByPatient(
    patientId: string,
    startDate: string,
    endDate: string
  ): Promise<number> {
    try {
      return await api.get<number>(
        API_ENDPOINTS.CONSULTATIONS.COUNT_BY_PATIENT_ID(
          patientId,
          startDate,
          endDate
        )
      );
    } catch (error) {
      toast.error("Échec lors de la récupération des consultations du patient");
      throw error;
    }
  },

  async countConsultationsByDoctor(
    doctorId: string,
    startDate: string,
    endDate: string
  ): Promise<number> {
    try {
      return await api.get<number>(
        API_ENDPOINTS.CONSULTATIONS.COUNT_BY_DOCTOR_ID(
          doctorId,
          startDate,
          endDate
        )
      );
    } catch (error) {
      toast.error("Échec lors de la récupération des consultations du médecin");
      throw error;
    }
  },

  async countConsultationsByClinic(
    cliniqueId: string,
    startDate: string,
    endDate: string
  ): Promise<number> {
    try {
      return await api.get<number>(
        API_ENDPOINTS.CONSULTATIONS.COUNT_BY_CLINIC_ID(
          cliniqueId,
          startDate,
          endDate
        )
      );
    } catch (error) {
      toast.error(
        "Échec lors de la récupération des consultations de la clinique"
      );
      throw error;
    }
  },

  async getRecentPaymentsByPatient(
    patientId: string
  ): Promise<RecentPaiementDto> {
    try {
      return await api.get<RecentPaiementDto>(
        API_ENDPOINTS.BILLING.PAIEMENT.DERNIER_PAIEMENT_PATIENT(patientId)
      );
    } catch (error) {
      toast.error(
        "Échec lors de la récupération des paiements récents du patient"
      );
      throw error;
    }
  },
  async getPendingAppointmentsCountByDoctor(
    medecinId: string
  ): Promise<number> {
    try {
      return await api.get<number>(
        API_ENDPOINTS.APPOINTMENTS.COUNT_PENDING_BY_DOCTOR(medecinId)
      );
    } catch (error) {
      toast.error(
        "Échec lors de la récupération des RDV en attente du médecin"
      );
      throw error;
    }
  },

  async getPendingAppointmentsCountByClinic(
    cliniqueId: string
  ): Promise<number> {
    try {
      return await api.get<number>(
        API_ENDPOINTS.APPOINTMENTS.COUNT_PENDING_BY_CLINIC(cliniqueId)
      );
    } catch (error) {
      toast.error(
        "Échec lors de la récupération des RDV en attente de la clinique"
      );
      throw error;
    }
  },

  async getRevenusMensuelByClinic(cliniqueId: string): Promise<number> {
    try {
      return await api.get<number>(
        API_ENDPOINTS.BILLING.FACTURE.REVENU_MENSUEL(cliniqueId)
      );
    } catch (error) {
      toast.error(
        "Échec lors de la récupération des revenus mensuels de la clinique"
      );
      throw error;
    }
  },

  async getRevenuMensuelTrendByClinic(
    cliniqueId: string
  ): Promise<RevenuTrend> {
    try {
      return await api.get<RevenuTrend>(
        API_ENDPOINTS.BILLING.FACTURE.REVENU_MENSUEL_TREND(cliniqueId)
      );
    } catch (error) {
      toast.error(
        "Échec lors de la récupération de la tendance des revenus mensuels"
      );
      throw error;
    }
  },

  async getAllDashboardData(
    start: string,
    end: string,
    medecinId?: string,
    cliniqueId?: string
  ) {
    try {
      const [
        consultationCount,
        newPatientsCount,
        rendezvousStats,
        dashboardStats,
        doctorsBySpecialty,
        totalClinics,
        weeklyAppointmentStatsByDoctor,
        weeklyAppointmentStatsByClinic,
      ] = await Promise.all([
        this.getConsultationCount(start, end),
        this.getNewPatientsCount(start, end),
        this.getRendezvousStats(start, end),
        this.getDashboardStats(start, end),
        this.getDoctorsBySpecialty(),
        this.getTotalClinics(),
        medecinId
          ? this.getWeeklyAppointmentsStatsByDoctor(medecinId, start, end)
          : Promise.resolve([]),
        cliniqueId
          ? this.getWeeklyAppointmentsStatsByClinic(cliniqueId, start, end)
          : Promise.resolve([]),
      ]);

      return {
        consultationCount,
        newPatientsCount,
        rendezvousStats,
        dashboardStats,
        doctorsBySpecialty,
        totalClinics,
        weeklyAppointmentStatsByDoctor,
        weeklyAppointmentStatsByClinic,
      };
    } catch (err) {
      throw new Error(
        "Erreur lors de la récupération des données du tableau de bord"
      );
    }
  },
};
