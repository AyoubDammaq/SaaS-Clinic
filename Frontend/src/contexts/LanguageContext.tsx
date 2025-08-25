
import { ReactNode, createContext, useContext, useState, useEffect } from 'react';

// Définir les langues supportées
export type Language = 'en' | 'fr';

// Définir le contexte
type LanguageContextType = {
  language: Language;
  setLanguage: (language: Language) => void;
};

const LanguageContext = createContext<LanguageContextType | undefined>(undefined);

// Provider component
export function LanguageProvider({ children }: { children: ReactNode }) {
  const [language, setLanguage] = useState<Language>(
    () => {
      // Try to get language from localStorage first
      const savedLanguage = localStorage.getItem('app-language') as Language;
      if (savedLanguage && ['en', 'fr'].includes(savedLanguage)) {
        return savedLanguage;
      }
      
      // If no saved language, try to detect browser language
      const navigatorLanguage = navigator.language.split('-')[0];
      if (navigatorLanguage === 'fr') {
        return 'fr';
      }
      
      // Default to English
      return 'en';
    }
  );
  
  const handleSetLanguage = (lang: Language) => {
    localStorage.setItem('app-language', lang);
    setLanguage(lang);
    document.documentElement.setAttribute('lang', lang);
  };

  // Set the lang attribute on document on mount
  useEffect(() => {
    document.documentElement.setAttribute('lang', language);
  }, [language]);

  return (
    <LanguageContext.Provider value={{ language, setLanguage: handleSetLanguage }}>
      {children}
    </LanguageContext.Provider>
  );
}

// Custom hook
// eslint-disable-next-line react-refresh/only-export-components
export function useLanguage() {
  const context = useContext(LanguageContext);
  if (context === undefined) {
    throw new Error('useLanguage must be used within a LanguageProvider');
  }
  
  return context;
}
