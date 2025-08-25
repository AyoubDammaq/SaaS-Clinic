// This file is kept for backward compatibility
// It delegates to the new modular i18n system
import {
  resources,
  Resource,
  TranslationValues,
  TranslationKey,
} from "./locales";
import { Language } from "@/contexts/LanguageContext";

// Re-export TranslationValues type
export type { TranslationValues, TranslationKey };

export function getTranslation<R extends Resource = "common">(
  key: string,
  language: string
): string {
  const lang = language as Language;

  // Check each namespace
  for (const namespace of Object.keys(resources.en) as Resource[]) {
    if (
      resources[lang] &&
      resources[lang][namespace] &&
      (resources[lang][namespace] as Record<string, string>)[key as string]
    ) {
      return (resources[lang][namespace] as Record<string, string>)[
        key as string
      ];
    }
  }

  // Fall back to English
  for (const namespace of Object.keys(resources.en) as Resource[]) {
    if (
      resources.en[namespace] &&
      (resources.en[namespace] as Record<string, string>)[key as string]
    ) {
      return (resources.en[namespace] as Record<string, string>)[key as string];
    }
  }

  // Last resort: return the key itself
  return key as string;
}
