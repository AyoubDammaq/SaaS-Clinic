import en_auth from "./en/auth";
import en_common from "./en/common";
import en_appointments from "./en/appointments";
import en_patients from "./en/patients";
import en_consultations from "./en/consultations";
import en_billing from "./en/billing";
import en_doctors from "./en/doctors";
import en_pagination from "./en/pagination";
import en_clinics from "./en/clinics";
import en_profil from "./en/profile";
import en_dashboard from "./en/dashboard";
import en_notifications from "./en/notifications";
import en_users from "./en/users";

import fr_auth from "./fr/auth";
import fr_common from "./fr/common";
import fr_appointments from "./fr/appointments";
import fr_patients from "./fr/patients";
import fr_consultations from "./fr/consultations";
import fr_billing from "./fr/billing";
import fr_doctors from "./fr/doctors";
import fr_pagination from "./fr/pagination";
import fr_clinics from "./fr/clinics";
import fr_profil from "./fr/profile";
import fr_dashboard from "./fr/dashboard";
import fr_notifications from "./fr/notifications";
import fr_users from "./fr/users";

// Re-export the types
export * from "./types";

export const resources = {
  en: {
    auth: en_auth,
    common: en_common,
    appointments: en_appointments,
    patients: en_patients,
    consultations: en_consultations,
    billing: en_billing,
    doctors: en_doctors,
    pagination: en_pagination,
    clinics: en_clinics,
    profil: en_profil,
    dashboard: en_dashboard,
    notifications: en_notifications,
    users: en_users,
  },
  fr: {
    auth: fr_auth,
    common: fr_common,
    appointments: fr_appointments,
    patients: fr_patients,
    consultations: fr_consultations,
    billing: fr_billing,
    doctors: fr_doctors,
    pagination: fr_pagination,
    clinics: fr_clinics,
    profil: fr_profil,
    dashboard: fr_dashboard,
    notifications: fr_notifications,
    users: fr_users,
  },
};

export const availableLanguages = [
  { code: "en", label: "English" },
  { code: "fr", label: "Français" },
];

export const defaultLanguage = "en";
