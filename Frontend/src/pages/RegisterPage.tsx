import { useState, FormEvent } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from "@/hooks/useAuth";
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { useTranslation } from '@/hooks/useTranslation';
import { ThemeSwitcher } from '@/components/ui/theme-switcher';
import { LanguageSwitcher } from '@/components/ui/language-switcher';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { UserRole } from '@/types/auth';

function RegisterPage() {
  const [FullName, setFullName] = useState('');
  const [Email, setEmail] = useState('');
  const [Password, setPassword] = useState('');
  const [ConfirmPassword, setConfirmPassword] = useState('');
  const [Role, setRole] = useState<UserRole | ''>('');
  const [passwordError, setPasswordError] = useState('');
  const { register, isLoading, error } = useAuth();
  const navigate = useNavigate();
  const { t } = useTranslation();
  
  const validatePassword = () => {
    if (Password !== ConfirmPassword) {
      setPasswordError(t('passwordMismatch'));
      return false;
    }
    setPasswordError('');
    return true;
  };
  
  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    
    if (!validatePassword()) {
      return;
    }

    if (!Role) {
      // Show error for role selection
      return;
    }
    
    const success = await register(FullName, Email, Password, ConfirmPassword, Role as UserRole);
    if (success) {
      navigate('/role-form');
    }
  };
  
  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 dark:bg-gray-900 p-4">
      <div className="absolute top-4 right-4 flex gap-2">
        <LanguageSwitcher />
        <ThemeSwitcher />
      </div>
      <div className="w-full max-w-md">
        <div className="text-center mb-8">
          <div className="inline-block mb-2 w-12 h-12 bg-clinic-500 text-white rounded-lg flex items-center justify-center font-bold text-xl mx-auto">
            SC
          </div>
          <h1 className="text-2xl font-bold text-gray-900 dark:text-white">SaaS-Clinic</h1>
          <p className="text-gray-600 dark:text-gray-400 mt-1">Medical Management System</p>
        </div>
        
        <Card>
          <CardHeader>
            <CardTitle>{t('register')}</CardTitle>
            <CardDescription>
              {t('createAccount')}
            </CardDescription>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit}>
              {error && (
                <Alert variant="destructive" className="mb-4">
                  <AlertDescription>{error}</AlertDescription>
                </Alert>
              )}
              
              <div className="grid gap-4">
                <div className="grid gap-2">
                  <Label htmlFor="name">{t('name')}</Label>
                  <Input
                    id="name"
                    type="text"
                    placeholder={t('fullNamePlaceholder')}
                    value={FullName}
                    onChange={(e) => setFullName(e.target.value)}
                    required
                  />
                </div>
                
                <div className="grid gap-2">
                  <Label htmlFor="email">{t('email')}</Label>
                  <Input
                    id="email"
                    type="email"
                    placeholder="name@example.com"
                    value={Email}
                    onChange={(e) => setEmail(e.target.value)}
                    autoComplete="email"
                    required
                  />
                </div>
                
                <div className="grid gap-2">
                  <Label htmlFor="password">{t('password')}</Label>
                  <Input
                    id="password"
                    type="password"
                    placeholder="••••••••"
                    value={Password}
                    onChange={(e) => setPassword(e.target.value)}
                    autoComplete="new-password"
                    required
                  />
                </div>
                
                <div className="grid gap-2">
                  <Label htmlFor="confirmPassword">{t('confirmPassword')}</Label>
                  <Input
                    id="confirmPassword"
                    type="password"
                    placeholder="••••••••"
                    value={ConfirmPassword}
                    onChange={(e) => setConfirmPassword(e.target.value)}
                    autoComplete="new-password"
                    required
                  />
                  {passwordError && (
                    <p className="text-sm font-medium text-destructive">{passwordError}</p>
                  )}
                </div>
                
                <div className="grid gap-2">
                  <Label htmlFor="role">{t('selectRole')}</Label>
                  <Select 
                    value={Role} 
                    onValueChange={(value) => setRole(value as UserRole)}
                    required
                  >
                    <SelectTrigger className="w-full">
                      <SelectValue placeholder={t('selectRolePlaceholder')} />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="ClinicAdmin">{t('clinicAdmin')}</SelectItem>
                      <SelectItem value="Doctor">{t('doctor')}</SelectItem>
                      <SelectItem value="Patient">{t('patient')}</SelectItem>
                    </SelectContent>
                  </Select>
                </div>
                
                <Button type="submit" className="w-full" disabled={isLoading}>
                  {isLoading ? t('registering') : t('registerButton')}
                </Button>
              </div>
            </form>
          </CardContent>
          <CardFooter className="flex justify-center">
            <div className="text-sm text-muted-foreground">
              {t('alreadyHaveAccount')} <Link to="/login" className="text-primary font-medium">{t('login')}</Link>
            </div>
          </CardFooter>
        </Card>
      </div>
    </div>
  );
}

export default RegisterPage;