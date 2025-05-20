
import en_auth from './en/auth';
import en_common from './en/common';
import en_appointments from './en/appointments';
import en_patients from './en/patients';
import en_consultations from './en/consultations';
import en_billing from './en/billing';
import en_doctors from './en/doctors';

import fr_auth from './fr/auth';
import fr_common from './fr/common';
import fr_appointments from './fr/appointments';
import fr_patients from './fr/patients';
import fr_consultations from './fr/consultations';
import fr_billing from './fr/billing';
import fr_doctors from './fr/doctors';

// Re-export the types
export * from './types';

export const resources = {
  en: {
    auth: en_auth,
    common: en_common,
    appointments: en_appointments,
    patients: en_patients,
    consultations: en_consultations,
    billing: en_billing,
    doctors: en_doctors,
  },
  fr: {
    auth: fr_auth,
    common: fr_common,
    appointments: fr_appointments,
    patients: fr_patients,
    consultations: fr_consultations,
    billing: fr_billing,
    doctors: fr_doctors,
  },
};

export const availableLanguages = [
  { code: 'en', label: 'English' },
  { code: 'fr', label: 'Français' },
];

export const defaultLanguage = 'en';
