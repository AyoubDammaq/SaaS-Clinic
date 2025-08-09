import { useLanguage } from '@/contexts/LanguageContext';
import { resources, Resource, TranslationKey } from '@/i18n/locales';
import { useCallback } from 'react';

export function useTranslation<R extends Resource = 'common'>(namespace: R = 'common' as R) {
  const { language } = useLanguage();

  const t = useCallback(<K extends TranslationKey<R> | string>(key: K, ns?: Resource): string => {
    const currentNamespace = ns || namespace;
    const translations = resources[language as keyof typeof resources];
    
    if (
      translations && 
      translations[currentNamespace] && 
      (translations[currentNamespace] as Record<string, string>)[key as string]
    ) {
      return (translations[currentNamespace] as Record<string, string>)[key as string];
    }
    
    if (
      currentNamespace !== 'common' && 
      translations && 
      translations.common && 
      (translations.common as Record<string, string>)[key as string]
    ) {
      return (translations.common as Record<string, string>)[key as string];
    }
    
    return key as string;
  }, [language, namespace]);

  return { t, language };
}
