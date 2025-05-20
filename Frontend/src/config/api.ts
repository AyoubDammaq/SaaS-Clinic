
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
  },
  
  // Patient endpoints
  PATIENTS: {
    BASE: `${GATEWAY_BASE}/patients`,
    GET_ALL: `${GATEWAY_BASE}/patients`,
    GET_BY_ID: (id: string) => `${GATEWAY_BASE}/patients/${id}`,
    CREATE: `${GATEWAY_BASE}/patients`,
    UPDATE: (id: string) => `${GATEWAY_BASE}/patients/${id}`,
    DELETE: (id: string) => `${GATEWAY_BASE}/patients/${id}`,
  },
  
  // Medical records endpoints
  MEDICAL_RECORDS: {
    BASE: `${GATEWAY_BASE}/medical-records`,
    GET_BY_PATIENT: (patientId: string) => `${GATEWAY_BASE}/medical-records/${patientId}`,
    UPDATE: () => `${GATEWAY_BASE}/medical-records`,
    ADD: () => `${GATEWAY_BASE}/medical-records`,
    ADD_DOCUMENT: (recordId: string) => `${GATEWAY_BASE}/medical-records/${recordId}/documents`,
    GET_DOCUMENT: (documentId: string) => `${GATEWAY_BASE}/medical-records/documents/${documentId}`,
    DELETE_DOCUMENT: (documentId: string) => `${GATEWAY_BASE}/medical-records/documents/${documentId}`,
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
    GET_AVAILABILITY: (doctorId: string) => `${GATEWAY_BASE}/disponibilite/disponibilites/${doctorId}`,
    SET_AVAILABILITY: (doctorId: string) => `${GATEWAY_BASE}/disponibilite/${doctorId}/disponibilites`,
  },
  
  // Appointment endpoints
  APPOINTMENTS: {
    BASE: `${GATEWAY_BASE}/appointments`,
    GET_ALL: `${GATEWAY_BASE}/appointments`,
    GET_BY_ID: (id: string) => `${GATEWAY_BASE}/appointments/${id}`,
    CREATE: `${GATEWAY_BASE}/appointments`,
    UPDATE: (id: string) => `${GATEWAY_BASE}/appointments/${id}`,
    CANCEL: (id: string) => `${GATEWAY_BASE}/appointments/${id}/cancel`,
    RESCHEDULE: (id: string) => `${GATEWAY_BASE}/appointments/${id}/reschedule`,
  },
  
  // Consultation endpoints
  CONSULTATIONS: {
    BASE: `${GATEWAY_BASE}/consultation`, // Updated to match C# controller
    GET_ALL: `${GATEWAY_BASE}/consultation`,
    GET_BY_ID: (id: string) => `${GATEWAY_BASE}/consultation/${id}`,
    CREATE: `${GATEWAY_BASE}/consultation`,
    UPDATE: (id: string) => `${GATEWAY_BASE}/consultation`,
    DELETE: (id: string) => `${GATEWAY_BASE}/consultation/${id}`,
  },
  
  // Billing endpoints
  BILLING: {
    BASE: `${GATEWAY_BASE}/billing`,
    GET_INVOICES: `${GATEWAY_BASE}/billing/invoices`,
    GET_INVOICE_BY_ID: (id: string) => `${GATEWAY_BASE}/billing/invoices/${id}`,
    PROCESS_PAYMENT: `${GATEWAY_BASE}/billing/payment/process`,
    DOWNLOAD_INVOICE: (id: string) => `${GATEWAY_BASE}/billing/invoices/${id}/download`,
  },
  
  // Clinic endpoints
  CLINICS: {
    BASE: `${GATEWAY_BASE}/clinics`,
    GET_ALL: `${GATEWAY_BASE}/clinics`,
    GET_BY_ID: (id: string) => `${GATEWAY_BASE}/clinics/${id}`,
    CREATE: `${GATEWAY_BASE}/clinics`,
    UPDATE: (id: string) => `${GATEWAY_BASE}/clinics/${id}`,
    DELETE: (id: string) => `${GATEWAY_BASE}/clinics/${id}`,

    // Search filters
    GET_BY_NAME: (name: string) => `${GATEWAY_BASE}/clinics/nom/${name}`,
    GET_BY_ADDRESS: (address: string) => `${GATEWAY_BASE}/clinics/adresse/${address}`,
    GET_BY_TYPE: (type: string) => `${GATEWAY_BASE}/clinics/type/${type}`, // enum value as string
    GET_BY_STATUS: (status: string) => `${GATEWAY_BASE}/clinics/statut/${status}`, // enum value as string

    // Statistics
    GET_TOTAL_COUNT: `${GATEWAY_BASE}/clinics/nombre-cliniques`,
    GET_NEW_THIS_MONTH: `${GATEWAY_BASE}/clinics/nouvelles-cliniques-mois`,
    GET_NEW_BY_MONTH: `${GATEWAY_BASE}/clinics/nouvelles-cliniques-par-mois`,
    GET_STATISTICS: (id: string) => `${GATEWAY_BASE}/clinics/statistiques/${id}`,
  },
  
  // Reports endpoints
  REPORTS: {
    BASE: `${GATEWAY_BASE}/reports`,
    GET_ALL: `${GATEWAY_BASE}/reports`,
    GET_BY_ID: (id: string) => `${GATEWAY_BASE}/reports/${id}`,
  },
  
  // Notifications endpoints
  NOTIFICATIONS: {
    BASE: `${GATEWAY_BASE}/notifications`,
    GET_ALL: `${GATEWAY_BASE}/notifications`,
    GET_BY_ID: (id: string) => `${GATEWAY_BASE}/notifications/${id}`,
    MARK_AS_READ: (id: string) => `${GATEWAY_BASE}/notifications/${id}/read`,
    MARK_ALL_AS_READ: `${GATEWAY_BASE}/notifications/read-all`,
  },
  
  // Availability endpoints
  AVAILABILITY: {
    BASE: `${GATEWAY_BASE}/disponibilite`,
    GET_ALL: `${GATEWAY_BASE}/disponibilite/disponibilites`,
    GET_BY_DOCTOR: (doctorId: string) => `${GATEWAY_BASE}/disponibilite/disponibilites/${doctorId}`,
    ADD: (doctorId: string) => `${GATEWAY_BASE}/disponibilite/${doctorId}/disponibilites`,
    DELETE: (id: string) => `${GATEWAY_BASE}/disponibilite/disponibilites/${id}`,
    GET_BY_DATE: (doctorId: string, date: string) => 
      `${GATEWAY_BASE}/disponibilite/disponibilites/${doctorId}/disponible?date=${date}`,
  }
};

// HTTP methods
export enum HTTP_METHODS {
  GET = 'GET',
  POST = 'POST',
  PUT = 'PUT',
  PATCH = 'PATCH',
  DELETE = 'DELETE'
}

// Default request headers
export const DEFAULT_HEADERS = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
};
