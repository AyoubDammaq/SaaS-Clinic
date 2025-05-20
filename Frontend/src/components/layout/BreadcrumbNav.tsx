
import { useLocation } from 'react-router-dom';
import { ChevronRight, Home } from 'lucide-react';
import { Link } from 'react-router-dom';
import { cn } from '@/lib/utils';
import { useTranslation } from '@/hooks/useTranslation';

export function BreadcrumbNav() {
  const location = useLocation();
  const { t } = useTranslation();
  
  // Skip breadcrumbs on home/dashboard
  if (location.pathname === '/' || location.pathname === '/dashboard') {
    return null;
  }
  
  // Generate breadcrumb segments from path
  const pathSegments = location.pathname
    .split('/')
    .filter(segment => segment !== '');
  
  // Function to generate readable title from path segment
  const getReadableTitle = (segment: string, index: number) => {
    // Handle special cases with dynamic IDs
    if (segment.match(/^[0-9a-fA-F-]+$/) && index > 0) {
      const previousSegment = pathSegments[index - 1];
      
      // For medical records pages
      if (previousSegment === 'medical-record') {
        return t('patientRecord', 'patients');
      }
      
      return "Details";
    }
    
    // Map path segments to readable titles with translations
    const segmentMap: Record<string, string> = {
      'dashboard': "Dashboard",
      'patients': "Patients",
      'appointments': "Appointments",
      'consultations': "Consultations",
      'doctors': "Doctors",
      'clinics': "Clinics",
      'billing': "Billing",
      'settings': "Settings",
      'notifications': "Notifications",
      'medical-record': "Medical Record",
      'reports': "Reports",
      'analytics': "Analytics",
      'profile': "Profile"
    };
    
    return segmentMap[segment] || segment.charAt(0).toUpperCase() + segment.slice(1);
  };
  
  const getPathForSegment = (index: number) => {
    return '/' + pathSegments.slice(0, index + 1).join('/');
  };

  return (
    <nav aria-label="Breadcrumb" className="mb-4 flex items-center text-sm text-muted-foreground animate-fade-in">
      <Link 
        to="/dashboard" 
        className="flex items-center hover:text-foreground transition-colors"
        aria-label="Dashboard"
      >
        <Home className="h-4 w-4" />
      </Link>
      
      {pathSegments.map((segment, index) => (
        <div key={index} className="flex items-center">
          <ChevronRight className="h-4 w-4 mx-1" />
          {index === pathSegments.length - 1 ? (
            <span 
              className={cn("font-medium text-foreground")}
              aria-current="page"
            >
              {getReadableTitle(segment, index)}
            </span>
          ) : (
            <Link 
              to={getPathForSegment(index)} 
              className="hover:text-foreground transition-colors"
            >
              {getReadableTitle(segment, index)}
            </Link>
          )}
        </div>
      ))}
    </nav>
  );
}
