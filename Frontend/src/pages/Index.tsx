
import { useEffect } from "react";
import { useNavigate, Link } from "react-router-dom";
import { useAuth } from "@/hooks/useAuth";
import { Button } from "@/components/ui/button";
import { useTranslation } from "@/hooks/useTranslation";

const Index = () => {
  const navigate = useNavigate();
  const { isAuthenticated } = useAuth();
  const { t } = useTranslation();
  
  useEffect(() => {
    // If authenticated, go to dashboard
    if (isAuthenticated) {
      navigate('/dashboard');
    }
  }, [isAuthenticated, navigate]);

  return (
    <div className="flex min-h-screen items-center justify-center bg-gray-50 dark:bg-gray-900">
      <div className="text-center max-w-md px-4">
        <div className="w-16 h-16 bg-clinic-500 text-white rounded-lg flex items-center justify-center font-bold text-2xl mx-auto mb-4">
          SC
        </div>
        <h1 className="text-4xl font-bold mb-4">SaaS-Clinic</h1>
        <p className="text-xl text-gray-600 dark:text-gray-400 mb-8">
          Modern medical management platform for clinics and patients
        </p>
        
        <div className="flex flex-col sm:flex-row gap-4 justify-center">
          <Button asChild size="lg">
            <Link to="/login">{t('login')}</Link>
          </Button>
          <Button asChild variant="outline" size="lg">
            <Link to="/register">{t('register')}</Link>
          </Button>
        </div>
      </div>
    </div>
  );
};

export default Index;
