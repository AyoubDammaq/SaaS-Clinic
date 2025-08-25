import { useLanguage } from "@/contexts/LanguageContext";
import { resources, Resource, TranslationKey } from "@/i18n/locales";
import { useCallback } from "react";

export function useTranslation<R extends Resource = "common">(
  namespace: R = "common" as R
) {
  const { language } = useLanguage();

  const t = useCallback(
    <K extends TranslationKey<R> | string>(
      key: K,
      options?: { ns?: Resource; values?: Record<string, string | number> }
    ): string => {
      const currentNamespace = options?.ns || namespace;
      const translations = resources[language as keyof typeof resources];

      let translation: string | undefined;

      if (translations && translations[currentNamespace]) {
        translation = (
          translations[currentNamespace] as Record<string, string>
        )[key as string];
      }

      if (
        !translation &&
        currentNamespace !== "common" &&
        translations &&
        translations.common
      ) {
        translation = (translations.common as Record<string, string>)[
          key as string
        ];
      }

      if (
        !translation &&
        language !== "en" &&
        resources.en &&
        resources.en[currentNamespace]
      ) {
        translation = (
          resources.en[currentNamespace] as Record<string, string>
        )[key as string];
      }

      if (!translation) {
        return key as string;
      }

      if (options?.values) {
        return Object.entries(options.values).reduce(
          (str, [k, v]) => str.replace(new RegExp(`{{${k}}}`, "g"), String(v)),
          translation
        );
      }

      return translation;
    },
    [language, namespace]
  );

  return { t, language };
}
