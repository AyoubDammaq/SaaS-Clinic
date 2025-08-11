/**
 * API configuration for the application
 * Contains all the endpoints used in the application
 */

// Base API URL - would be replaced with real API URL in production
const API_BASE_URL = import.meta.env.VITE_API_URL;

// API Gateway base path
const GATEWAY_BASE = `${API_BASE_URL}/gateway`;

// API endpoints configuration
export const API_ENDPOINTS = {
  // Auth endpoints
  AUTH: {
    LOGIN: `${GATEWAY_BASE}/auth/login`,
    REGISTER: `${GATEWAY_BASE}/auth/register`,
    LOGOUT: `${GATEWAY_BASE}/auth/logout`,
    REFRESH_TOKEN: `${GATEWAY_BASE}/auth/refresh-token`,
    FORGOT_PASSWORD: `${GATEWAY_BASE}/auth/forgot-password`,
    RESET_PASSWORD: `${GATEWAY_BASE}/auth/reset-password`,
    CHANGE_PASSWORD: `${GATEWAY_BASE}/auth/change-password`,
    GET_ALL_USERS: `${GATEWAY_BASE}/auth/users`,
    GET_USER_BY_ID: (id: string) => `${GATEWAY_BASE}/auth/users/${id}`,
    DELETE_USER: (id: string) => `${GATEWAY_BASE}/auth/users/${id}`,
    CHANGE_ROLE: `${GATEWAY_BASE}/auth/change-role`,
    LINK_PROFILE: `${GATEWAY_BASE}/auth/link-profile`,
  },

  // Patient endpoints
  PATIENTS: {
    BASE: `${GATEWAY_BASE}/patients`,
    GET_ALL: `${GATEWAY_BASE}/patients`,
    GET_BY_ID: (id: string) => `${GATEWAY_BASE}/patients/${id}`,
    CREATE: `${GATEWAY_BASE}/patients`,
    UPDATE: (id: string) => `${GATEWAY_BASE}/patients/${id}`,
    DELETE: (id: string) => `${GATEWAY_BASE}/patients/${id}`,
    LINK_USER: `${GATEWAY_BASE}/patients/link-user/patient`,
  },

  // Medical records endpoints
  MEDICAL_RECORDS: {
    BASE: `${GATEWAY_BASE}/medical-records`,
    GET_BY_PATIENT: (patientId: string) =>
      `${GATEWAY_BASE}/medical-records/${patientId}`,
    UPDATE: () => `${GATEWAY_BASE}/medical-records`,
    ADD: () => `${GATEWAY_BASE}/medical-records`,
    ADD_DOCUMENT: (recordId: string) =>
      `${GATEWAY_BASE}/medical-records/${recordId}/documents`,
    GET_DOCUMENT: (documentId: string) =>
      `${GATEWAY_BASE}/medical-records/documents/${documentId}`,
    DELETE_DOCUMENT: (documentId: string) =>
      `${GATEWAY_BASE}/medical-records/documents/${documentId}`,
    GET_ALL: () => `${GATEWAY_BASE}/medical-records/dossiers-medicals`,
  },

  // Doctor endpoints
  DOCTORS: {
    BASE: `${GATEWAY_BASE}/doctors`, // Updated to match C# controller
    GET_ALL: `${GATEWAY_BASE}/doctors`,
    GET_BY_ID: (id: string) => `${GATEWAY_BASE}/doctors/${id}`,
    CREATE: `${GATEWAY_BASE}/doctors`,
    UPDATE: (id: string) => `${GATEWAY_BASE}/doctors/${id}`,
    DELETE: (id: string) => `${GATEWAY_BASE}/doctors/${id}`,
    FILTER_BY_SPECIALTY: (specialite: string) =>
      `${GATEWAY_BASE}/doctors/filter/specialite?specialite=${encodeURIComponent(
        specialite
      )}`,
    FILTER_BY_NAME: (nom?: string, prenom?: string) =>
      `${GATEWAY_BASE}/doctors/filter/name?name=${encodeURIComponent(
        nom ?? ""
      )}&prenom=${encodeURIComponent(prenom ?? "")}`,
    BY_CLINIC: (cliniqueId: string) =>
      `${GATEWAY_BASE}/doctors/clinique/${cliniqueId}`,
    ATTRIBUER: `${GATEWAY_BASE}/doctors/attribuer`,
    DESABONNER: (medecinId: string) =>
      `${GATEWAY_BASE}/doctors/desabonner/${medecinId}`,
    STATS_BY_SPECIALTY: `${GATEWAY_BASE}/doctors/statistiques/specialite`,
    STATS_BY_CLINIC: `${GATEWAY_BASE}/doctors/statistiques/clinique`,
    STATS_BY_SPECIALTY_IN_CLINIC: (cliniqueId: string) =>
      `${GATEWAY_BASE}/doctors/statistiques/specialite/clinique/${cliniqueId}`,
    IDS_BY_CLINIQUE: (cliniqueId: string) =>
      `${GATEWAY_BASE}/doctors/medecinsIds/clinique/${cliniqueId}`,
    ACTIVITES: (medecinId: string) =>
      `${GATEWAY_BASE}/doctors/activites/${medecinId}`,
    LINK_USER: `${GATEWAY_BASE}/doctors/link-user/doctor`,
  },

  // Appointment endpoints
  APPOINTMENTS: {
    BASE: `${GATEWAY_BASE}/appointments`,
    GET_ALL: `${GATEWAY_BASE}/appointments`,
    GET_BY_ID: (id: string) => `${GATEWAY_BASE}/appointments/${id}`,
    CREATE: `${GATEWAY_BASE}/appointments`,
    UPDATE: (id: string) => `${GATEWAY_BASE}/appointments/${id}`,
    CANCEL_BY_PATIENT: (id: string) =>
      `${GATEWAY_BASE}/appointments/annuler/patient/${id}`,
    CANCEL_BY_DOCTOR: (id: string) =>
      `${GATEWAY_BASE}/appointments/annuler/medecin/${id}`,
    CONFIRM_BY_DOCTOR: (id: string) =>
      `${GATEWAY_BASE}/appointments/confirmer/${id}`,
    GET_BY_PATIENT_ID: (patientId: string) =>
      `${GATEWAY_BASE}/appointments/patient/${patientId}`,
    GET_BY_DOCTOR_ID: (medecinId: string) =>
      `${GATEWAY_BASE}/appointments/medecin/${medecinId}`,
    GET_BY_DATE: (date: string) => `${GATEWAY_BASE}/appointments/date/${date}`,
    GET_BY_STATUS: (statut: string) =>
      `${GATEWAY_BASE}/appointments/statut/${statut}`,
    GET_STATS_PERIOD: (start: string, end: string) =>
      `${GATEWAY_BASE}/appointments/period?start=${start}&end=${end}`,
    COUNT_BY_DOCTORS: (medecinIds: string) =>
      `${GATEWAY_BASE}/appointments/count?medecinIds=${medecinIds}`,
    COUNT_PENDING_BY_DOCTOR: (medecinId: string) =>
      `${GATEWAY_BASE}/appointments/count/pending/doctor/${medecinId}`,
    COUNT_PENDING_BY_CLINIC: (cliniqueId: string) =>
      `${GATEWAY_BASE}/appointments/count/pending/clinic/${cliniqueId}`,
    COUNT_DISTINCT_PATIENTS: (medecinIds: string) =>
      `${GATEWAY_BASE}/appointments/distinct/patients?medecinIds=${medecinIds}`,
    COUNT_BY_CLINIC: (cliniqueId: string, date: string) =>
      `${GATEWAY_BASE}/appointments/count/by-clinic/${cliniqueId}/date?date=${date}`,
  },

  // Consultation endpoints
  CONSULTATIONS: {
    BASE: `${GATEWAY_BASE}/consultations`, // Updated to match C# controller
    GET_ALL: `${GATEWAY_BASE}/consultations`,
    GET_BY_ID: (id: string) => `${GATEWAY_BASE}/consultations/${id}`,
    CREATE: `${GATEWAY_BASE}/consultations`,
    UPDATE: (id: string) => `${GATEWAY_BASE}/consultations`,
    DELETE: (id: string) => `${GATEWAY_BASE}/consultations/${id}`,
    GET_BY_PATIENT_ID: (patientId: string) =>
      `${GATEWAY_BASE}/consultations/patient/${patientId}`,
    GET_BY_DOCTOR_ID: (doctorId: string) =>
      `${GATEWAY_BASE}/consultations/doctor/${doctorId}`,
    GET_BY_CLINIC_ID: (clinicId: string) =>
      `${GATEWAY_BASE}/consultations/by-clinic?clinicId=${clinicId}`,
    GET_DOCUMENT_BY_ID: (id: string) =>
      `${GATEWAY_BASE}/consultations/document/${id}`,
    UPLOAD_DOCUMENT: `${GATEWAY_BASE}/consultations/document`,
    DELETE_DOCUMENT: (id: string) =>
      `${GATEWAY_BASE}/consultations/document/${id}`,
    COUNT_BETWEEN_DATES: (start: string, end: string) =>
      `${GATEWAY_BASE}/consultations/count?start=${start}&end=${end}`,
    COUNT_BY_DOCTOR_IDS: (ids: string[]) =>
      `${GATEWAY_BASE}/consultations/countByMedecinIds?${ids
        .map((id) => `medecinIds=${id}`)
        .join("&")}`,
    GET_NOUVEAUX_PATIENTS_COUNT_BY_DOCTOR: (
      medecinId: string,
      start: string,
      end: string
    ) =>
      `${GATEWAY_BASE}/consultations/nouveaux-patients-count-by-doctor/${medecinId}?startDate=${start}&endDate=${end}`,
    GET_NOUVEAUX_PATIENTS_COUNT_BY_CLINIC: (
      cliniqueId: string,
      start: string,
      end: string
    ) =>
      `${GATEWAY_BASE}/consultations/nouveaux-patients-count-by-clinic/${cliniqueId}?startDate=${start}&endDate=${end}`,
    COUNT_BY_PATIENT_ID: (
      patientId: string,
      startDate: string,
      endDate: string
    ) =>
      `${GATEWAY_BASE}/consultations/count-consultation-by-patient/${patientId}?startDate=${startDate}&endDate=${endDate}`,
    COUNT_BY_DOCTOR_ID: (
      doctorId: string,
      startDate: string,
      endDate: string
    ) =>
      `${GATEWAY_BASE}/consultations/count-consultation-by-doctor/${doctorId}?startDate=${startDate}&endDate=${endDate}`,
    COUNT_BY_CLINIC_ID: (
      cliniqueId: string,
      startDate: string,
      endDate: string
    ) =>
      `${GATEWAY_BASE}/consultations/count-consultation--by-clinic/${cliniqueId}?startDate=${startDate}&endDate=${endDate}`,
  },

  // Billing endpoints
  BILLING: {
    BASE: `${GATEWAY_BASE}/billing`,

    // Facture endpoints
    FACTURE: {
      BASE: `${GATEWAY_BASE}/billing`,
      GET_ALL: `${GATEWAY_BASE}/billing`,
      GET_BY_ID: (id: string) => `${GATEWAY_BASE}/billing/${id}`,
      ADD: `${GATEWAY_BASE}/billing`,
      UPDATE: `${GATEWAY_BASE}/billing`,
      DELETE: (id: string) => `${GATEWAY_BASE}/billing/${id}`,
      BY_CLINIC: (cliniqueId: string) =>
        `${GATEWAY_BASE}/billing/clinic/${cliniqueId}`,
      BY_PATIENT_ID: (patientId: string) =>
        `${GATEWAY_BASE}/billing/patient/${patientId}`,
      BY_CONSULTATION_ID: (consultationId: string) =>
        `${GATEWAY_BASE}/billing/consultation/${consultationId}`,
      GET_BY_DATE_RANGE: (start: string, end: string) =>
        `${GATEWAY_BASE}/billing/range?startDate=${start}&endDate=${end}`,
      FILTER: `${GATEWAY_BASE}/billing/filtrer`,
      EXPORT: (id: string) => `${GATEWAY_BASE}/billing//export/${id}`,
      STATS: {
        BY_STATUS: `${GATEWAY_BASE}/billing/stats/status`,
        BY_CLINIC: `${GATEWAY_BASE}/billing/stats/clinic`,
        BY_STATUS_CLINIC: `${GATEWAY_BASE}/billing/stats/status/clinic`,
        BY_STATUS_IN_CLINIC: (clinicId: string) =>
          `${GATEWAY_BASE}/billing/stats/status/clinic/${clinicId}`,
      },
      STATS_BY_PERIOD: (start: string, end: string) =>
        `${GATEWAY_BASE}/billing/statistiques?debut=${start}&fin=${end}`,
      REVENU_MENSUEL: (cliniqueId: string) =>
        `${GATEWAY_BASE}/billing/revenus-mensuels/${cliniqueId}`,
      REVENU_MENSUEL_TREND: (cliniqueId: string) =>
        `${GATEWAY_BASE}/billing/revenu-mensuel-trend/${cliniqueId}`,
    },

    // Paiement endpoints
    PAIEMENT: {
      BASE: `${GATEWAY_BASE}/paiement`,
      PAYER: (factureId: string) =>
        `${GATEWAY_BASE}/paiement/payer/${factureId}`,
      RECU: (factureId: string) => `${GATEWAY_BASE}/paiement/recu/${factureId}`,
      DERNIER_PAIEMENT_PATIENT: (patientId: string) =>
        `${GATEWAY_BASE}/paiement/GetDernierPaiementByPatientId/${patientId}`,
    },

    // Tarification endpoints
    TARIFICATION: {
      BASE: `${GATEWAY_BASE}/tarification`,
      GET_ALL: `${GATEWAY_BASE}/tarification/GetAllTarifications`,
      ADD: `${GATEWAY_BASE}/tarification/AddTarification`,
      UPDATE: `${GATEWAY_BASE}/tarification/UpdateTarification`,
      DELETE: (id: string) =>
        `${GATEWAY_BASE}/tarification/DeleteTarification/${id}`,
      GET_BY_ID: (id: string) =>
        `${GATEWAY_BASE}/tarification/GetTarificationById/${id}`,
      GET_BY_CLINIC_ID: (clinicId: string) =>
        `${GATEWAY_BASE}/tarification/GetTarificationByClinicId/${clinicId}`,
    },
  },

  // Clinic endpoints
  CLINICS: {
    BASE: `${GATEWAY_BASE}/clinics`,
    GET_ALL: `${GATEWAY_BASE}/clinics`,
    GET_BY_ID: (id: string) => `${GATEWAY_BASE}/clinics/${id}`,
    CREATE: `${GATEWAY_BASE}/clinics`,
    UPDATE: (id: string) => `${GATEWAY_BASE}/clinics/${id}`,
    DELETE: (id: string) => `${GATEWAY_BASE}/clinics/${id}`,
    LINK_USER: `${GATEWAY_BASE}/clinics/link-user/clinic`,

    // Search filters
    GET_BY_NAME: (name: string) => `${GATEWAY_BASE}/clinics/nom/${name}`,
    GET_BY_ADDRESS: (address: string) =>
      `${GATEWAY_BASE}/clinics/adresse/${address}`,
    GET_BY_TYPE: (type: string) => `${GATEWAY_BASE}/clinics/type/${type}`, // enum value as string
    GET_BY_STATUS: (status: string) =>
      `${GATEWAY_BASE}/clinics/statut/${status}`, // enum value as string

    // Statistics
    GET_TOTAL_COUNT: `${GATEWAY_BASE}/clinics/nombre-cliniques`,
    GET_NEW_THIS_MONTH: `${GATEWAY_BASE}/clinics/nouvelles-cliniques-mois`,
    GET_NEW_BY_MONTH: `${GATEWAY_BASE}/clinics/nouvelles-cliniques-par-mois`,
    GET_STATISTICS: (id: string) =>
      `${GATEWAY_BASE}/clinics/statistiques/${id}`,
  },

  // Reports endpoints
  REPORTS: {
    BASE: `${GATEWAY_BASE}/reports`,

    // Consultation stats
    COUNT_CONSULTATIONS: (start: string, end: string) =>
      `${GATEWAY_BASE}/reports/consultations/count?start=${start}&end=${end}`,

    // Appointment stats
    STATS_RENDEZVOUS: (start: string, end: string) =>
      `${GATEWAY_BASE}/reports/rendezvous/stats?start=${start}&end=${end}`,
    WEEKLY_APPOINTMENTS_STATS_BY_DOCTOR: (
      medecinId: string,
      start: string,
      end: string
    ) =>
      `${GATEWAY_BASE}/reports/rendezvous/weekly-stats/by-doctor/${medecinId}?start=${start}&end=${end}`,
    WEEKLY_APPOINTMENTS_STATS_BY_CLINIC: (
      cliniqueId: string,
      start: string,
      end: string
    ) =>
      `${GATEWAY_BASE}/reports/rendezvous/weekly-stats/by-clinic/${cliniqueId}?start=${start}&end=${end}`,

    // New patients
    COUNT_NEW_PATIENTS: (start: string, end: string) =>
      `${GATEWAY_BASE}/reports/patients/nouveaux/count?start=${start}&end=${end}`,

    // Doctors statistics
    COUNT_DOCTORS_BY_SPECIALTY: `${GATEWAY_BASE}/reports/medecins/specialites/count`,
    COUNT_DOCTORS_BY_CLINIC: `${GATEWAY_BASE}/reports/medecins/cliniques/count`,
    COUNT_DOCTORS_BY_SPECIALTY_IN_CLINIC: (cliniqueId: string) =>
      `${GATEWAY_BASE}/reports/medecins/specialites/cliniques/${cliniqueId}/count`,

    // Billing stats
    COUNT_FACTURES_BY_STATUS: `${GATEWAY_BASE}/reports/factures/status/count`,
    COUNT_FACTURES_BY_CLINIC: `${GATEWAY_BASE}/reports/factures/cliniques/count`,
    COUNT_FACTURES_BY_STATUS_AND_CLINIC: `${GATEWAY_BASE}/reports/factures/status/cliniques/count`,
    COUNT_FACTURES_BY_STATUS_IN_CLINIC: (cliniqueId: string) =>
      `${GATEWAY_BASE}/reports/factures/status/cliniques/${cliniqueId}/count`,

    // Clinics stats
    COUNT_CLINICS: `${GATEWAY_BASE}/reports/cliniques/count`,
    COUNT_NEW_CLINICS: `${GATEWAY_BASE}/reports/cliniques/nouveaux/count`,
    COUNT_NEW_CLINICS_BY_MONTH: `${GATEWAY_BASE}/reports/cliniques/nouveaux/mois`,
    GET_CLINIC_STATS: (cliniqueId: string) =>
      `${GATEWAY_BASE}/reports/cliniques/${cliniqueId}/stats`,

    // Doctor activities
    DOCTOR_ACTIVITIES: (medecinId: string) =>
      `${GATEWAY_BASE}/reports/medecins/${medecinId}/activites`,

    // Payments stats
    PAYMENT_AMOUNT: (statut: string, start: string, end: string) =>
      `${GATEWAY_BASE}/reports/paiements/montant?statut=${statut}&start=${start}&end=${end}`,

    // Invoice statistics
    FACTURE_STATS: (start: string, end: string) =>
      `${GATEWAY_BASE}/reports/factures/stats?start=${start}&end=${end}`,

    // Clinic comparison
    COMPARE_CLINICS: `${GATEWAY_BASE}/reports/cliniques/comparaison`,

    // Dashboard
    DASHBOARD_STATS: (
      start: string,
      end: string,
      patientId?: string,
      medecinId?: string,
      cliniqueId?: string
    ) =>
      `${GATEWAY_BASE}/reports/dashboard?start=${start}&end=${end}${
        patientId ? `&patientId=${patientId}` : ""
      }${medecinId ? `&medecinId=${medecinId}` : ""}${
        cliniqueId ? `&cliniqueId=${cliniqueId}` : ""
      }`,
    DASHBOARD_PDF: (start: string, end: string) =>
      `${GATEWAY_BASE}/reports/dashboard/pdf?start=${start}&end=${end}`,
    DASHBOARD_EXCEL: (start: string, end: string) =>
      `${GATEWAY_BASE}/reports/dashboard/excel?start=${start}&end=${end}`,
  },

  // Notifications endpoints
  NOTIFICATIONS: {
    BASE: `${GATEWAY_BASE}/notifications`,
    GET_ALL: `${GATEWAY_BASE}/notifications`,
    GET_BY_ID: (id: string) => `${GATEWAY_BASE}/notifications/${id}`,
    SEND: `${GATEWAY_BASE}/notifications/send`,
    MARK_AS_SENT: `${GATEWAY_BASE}/notifications/mark-as-sent`,
    GET_BY_RECIPIENT_ID: (recipientId: string) =>
      `${GATEWAY_BASE}/notifications/recipient/${recipientId}`,
    DELETE: (notificationId: string) =>
      `${GATEWAY_BASE}/notifications/${notificationId}`,
    MARK_AS_READ: (id: string) =>
      `${GATEWAY_BASE}/notifications/mark-as-read/${id}`,
    MARK_ALL_AS_READ: (recipientId: string) =>
      `${GATEWAY_BASE}/notifications/mark-all-as-read/${recipientId}`,
  },

  // Availability endpoints
  Disponibilite: {
    BASE: `${GATEWAY_BASE}/availibility`,
    GET_ALL: `${GATEWAY_BASE}/availibility`,
    GET_BY_DOCTOR: (doctorId: string) =>
      `${GATEWAY_BASE}/availibility/medecins/${doctorId}`,
    ADD: () => `${GATEWAY_BASE}/availibility`,
    UPDATE: (disponibiliteId: string) =>
      `${GATEWAY_BASE}/availibility/${disponibiliteId}`,
    DELETE: (disponibiliteId: string) =>
      `${GATEWAY_BASE}/availibility/${disponibiliteId}`,
    DELETE_BY_DOCTOR: (doctorId: string) =>
      `${GATEWAY_BASE}/availibility/medecins/${doctorId}`,
    GET_BY_DAY: (doctorId: string, day: string) =>
      `${GATEWAY_BASE}/availibility/medecins/${doctorId}/jour/${day}`,
    GET_AVAILABLE_DOCTORS: (
      date: string,
      heureDebut?: string,
      heureFin?: string
    ) => {
      let url = `${GATEWAY_BASE}/availibility/medecins-disponibles?date=${date}`;
      if (heureDebut) url += `&heureDebut=${heureDebut}`;
      if (heureFin) url += `&heureFin=${heureFin}`;
      return url;
    },
    GET_AVAILABLE_SLOTS: (doctorId: string, date: string) =>
      `${GATEWAY_BASE}/availibility/medecins/${doctorId}/creneaux-disponibles?date=${encodeURIComponent(
        date
      )}`,
    IS_AVAILABLE: (doctorId: string, dateTime: string) =>
      `${GATEWAY_BASE}/availibility/disponibilites/${doctorId}/disponible`,
    TOTAL_AVAILABLE_TIME: (doctorId: string, start: string, end: string) =>
      `${GATEWAY_BASE}/availibility/medecins/${doctorId}/total-temps-disponible?dateDebut=${start}&dateFin=${end}`,
    GET_IN_INTERVAL: (doctorId: string, start: string, end: string) =>
      `${GATEWAY_BASE}/availibility/medecins/${doctorId}/intervalle?start=${start}&end=${end}`,
  },
};

// HTTP methods
export enum HTTP_METHODS {
  GET = "GET",
  POST = "POST",
  PUT = "PUT",
  PATCH = "PATCH",
  DELETE = "DELETE",
}

// Default request headers
export const DEFAULT_HEADERS = {
  "Content-Type": "application/json",
  Accept: "application/json",
};
