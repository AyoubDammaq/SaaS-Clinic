
import { useLanguage } from '@/contexts/LanguageContext';
import { resources, Resource, TranslationKey } from '@/i18n/locales';

export function useTranslation<R extends Resource = 'common'>(namespace: R = 'common' as R) {
  const { language } = useLanguage();
  
  const t = <K extends TranslationKey<R> | string>(key: K, ns?: Resource): string => {
    const currentNamespace = ns || namespace;
    const translations = resources[language as keyof typeof resources];
    
    // Try to find the translation in the specified namespace
    if (
      translations && 
      translations[currentNamespace] && 
      (translations[currentNamespace] as Record<string, string>)[key as string]
    ) {
      return (translations[currentNamespace] as Record<string, string>)[key as string];
    }
    
    // If not found in the specified namespace and the namespace isn't common, try common namespace
    if (
      currentNamespace !== 'common' && 
      translations && 
      translations.common && 
      (translations.common as Record<string, string>)[key as string]
    ) {
      return (translations.common as Record<string, string>)[key as string];
    }
    
    // Fall back to the key itself
    return key as string;
  };
  
  return { t, language };
}
