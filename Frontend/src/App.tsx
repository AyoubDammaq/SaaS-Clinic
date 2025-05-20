
import { Routes, Route } from 'react-router-dom';
import { Toaster } from '@/components/ui/sonner';
import DashboardPage from '@/pages/DashboardPage';
import LoginPage from '@/pages/LoginPage';
import RegisterPage from '@/pages/RegisterPage';
import RoleFormRouter from '@/pages/role-forms/RoleFormRouter';
import Index from '@/pages/Index';
import NotFound from '@/pages/NotFound';
import PatientsPage from '@/pages/patients/PatientsPage';
import MedicalRecordPage from '@/pages/patients/MedicalRecordPage';
import AppointmentsPage from '@/pages/appointments/AppointmentsPage';
import ConsultationsPage from '@/pages/consultations/ConsultationsPage';
import DoctorsPage from '@/pages/doctors/DoctorsPage';
import ClinicsPage from '@/pages/clinics/ClinicsPage';
import BillingPage from '@/pages/billing/BillingPage';
import SettingsPage from '@/pages/settings/SettingsPage';
import PlaceholderPage from '@/pages/PlaceholderPage';
import NotificationsPage from '@/pages/notifications/NotificationsPage';
import { PrivateRoute } from '@/components/PrivateRoute';
import { AuthProvider } from '@/contexts/AuthContext';
import { ThemeProvider } from '@/components/theme-provider';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { LanguageProvider } from '@/contexts/LanguageContext';

function App() {
  const queryClient = new QueryClient();

  return (
    <QueryClientProvider client={queryClient}>
      <LanguageProvider>
        <AuthProvider>
          <ThemeProvider defaultTheme="system" storageKey="vite-ui-theme">
            <div className="min-h-screen flex flex-col">
              <Routes>
                <Route path="/" element={<Index />} />
                <Route path="/login" element={<LoginPage />} />
                <Route path="/register" element={<RegisterPage />} />
                <Route path="/role-form" element={<RoleFormRouter />} />
                
                {/* Private routes wrapped by PrivateRoute */}
                <Route element={<PrivateRoute />}>
                  <Route path="/dashboard" element={<DashboardPage />} />
                  <Route path="/patients" element={<PatientsPage />} />
                  <Route path="/medical-record/:id" element={<MedicalRecordPage />} />
                  <Route path="/appointments" element={<AppointmentsPage />} />
                  <Route path="/consultations" element={<ConsultationsPage />} />
                  <Route path="/doctors" element={<DoctorsPage />} />
                  <Route path="/clinics" element={<ClinicsPage />} />
                  <Route path="/billing" element={<BillingPage />} />
                  <Route path="/settings" element={<SettingsPage />} />
                  <Route path="/notifications" element={<NotificationsPage />} />
                  <Route path="/reports" element={<PlaceholderPage title="Reports" />} />
                  <Route path="/analytics" element={<PlaceholderPage title="Analytics" />} />
                  <Route path="/profile" element={<PlaceholderPage title="Profile" />} />
                </Route>
                
                <Route path="*" element={<NotFound />} />
              </Routes>
              <Toaster />
            </div>
          </ThemeProvider>
        </AuthProvider>
      </LanguageProvider>
    </QueryClientProvider>
  );
}

export default App;
