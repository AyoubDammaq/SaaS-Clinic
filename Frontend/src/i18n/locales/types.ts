
import { resources } from "./index";

// Define the available resources/namespaces
export type Resource = keyof typeof resources.en;

// Utility type to get keys from a specific namespace
export type TranslationKey<R extends Resource = 'common'> = keyof (typeof resources)['en'][R];

// For backward compatibility
export type TranslationValues = typeof resources.en.common & 
                               typeof resources.en.auth & 
                               typeof resources.en.appointments & 
                               typeof resources.en.billing &
                               typeof resources.en.patients &
                               typeof resources.en.consultations &
                               typeof resources.en.doctors &
                               typeof resources.en.pagination;
