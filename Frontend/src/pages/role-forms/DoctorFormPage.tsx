
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '@/contexts/AuthContext';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { useTranslation } from '@/hooks/useTranslation';
import { doctorService } from '@/services/doctorService';
import { toast } from 'sonner';

function DoctorFormPage() {
  const { user } = useAuth();
  const navigate = useNavigate();
  const { t } = useTranslation('doctors');
  
  const [formData, setFormData] = useState({
    prenom: '',
    nom: '',
    specialite: '',
    telephone: '',
    photoUrl: ''
  });
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  
  // List of possible specialties
  const specialties = [
    'Cardiologie', 
    'Dermatologie', 
    'Endocrinologie', 
    'Gastroentérologie', 
    'Neurologie',
    'Obstétrique', 
    'Ophtalmologie', 
    'Pédiatrie', 
    'Psychiatrie', 
    'Médecin généraliste'
  ];

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { id, value } = e.target;
    setFormData(prev => ({ ...prev, [id]: value }));
  };

  const handleSpecialtyChange = (value: string) => {
    setFormData(prev => ({ ...prev, specialite: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsSubmitting(true);
    setError(null);
    
    try {
      if (!user) {
        throw new Error('User not authenticated');
      }
      
      // Split the name from user object (if available)
      const nameParts = user.name.split(' ');
      const prenom = formData.prenom || (nameParts.length > 0 ? nameParts[0] : '');
      const nom = formData.nom || (nameParts.length > 1 ? nameParts.slice(1).join(' ') : '');
      
      // Create doctor profile
      await doctorService.createDoctor({
        prenom,
        nom,
        specialite: formData.specialite,
        email: user.email,
        telephone: formData.telephone,
        photoUrl: formData.photoUrl
      });
      
      toast.success('Doctor profile created successfully');
      navigate('/dashboard');
    } catch (err: any) {
      setError(err.message || 'Failed to create doctor profile');
      toast.error(err.message || 'Failed to create doctor profile');
    } finally {
      setIsSubmitting(false);
    }
  };
  
  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 dark:bg-gray-900 p-4">
      <div className="w-full max-w-md">
        <div className="text-center mb-8">
          <h1 className="text-2xl font-bold text-gray-900 dark:text-white">SaaS-Clinic</h1>
          <p className="text-gray-600 dark:text-gray-400 mt-1">Complete Your Doctor Profile</p>
        </div>
        
        <Card>
          <CardHeader>
            <CardTitle>Doctor Information</CardTitle>
            <CardDescription>
              Please provide your professional information
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
                <div className="grid grid-cols-2 gap-4">
                  <div className="grid gap-2">
                    <Label htmlFor="prenom">{t('firstName')}</Label>
                    <Input
                      id="prenom"
                      value={formData.prenom}
                      onChange={handleChange}
                      placeholder="John"
                      required
                    />
                  </div>
                  <div className="grid gap-2">
                    <Label htmlFor="nom">{t('lastName')}</Label>
                    <Input
                      id="nom"
                      value={formData.nom}
                      onChange={handleChange}
                      placeholder="Doe"
                      required
                    />
                  </div>
                </div>
                
                <div className="grid gap-2">
                  <Label htmlFor="specialite">{t('specialty')}</Label>
                  <Select
                    value={formData.specialite}
                    onValueChange={handleSpecialtyChange}
                    required
                  >
                    <SelectTrigger>
                      <SelectValue placeholder="Select your specialty" />
                    </SelectTrigger>
                    <SelectContent>
                      {specialties.map((specialty) => (
                        <SelectItem key={specialty} value={specialty}>
                          {specialty}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
                
                <div className="grid gap-2">
                  <Label htmlFor="telephone">{t('phone')}</Label>
                  <Input
                    id="telephone"
                    value={formData.telephone}
                    onChange={handleChange}
                    placeholder="+1 (555) 123-4567"
                    required
                  />
                </div>
                
                <div className="grid gap-2">
                  <Label htmlFor="photoUrl">Profile Photo URL (optional)</Label>
                  <Input
                    id="photoUrl"
                    value={formData.photoUrl}
                    onChange={handleChange}
                    placeholder="https://example.com/photo.jpg"
                  />
                </div>
                
                <Button type="submit" className="w-full" disabled={isSubmitting}>
                  {isSubmitting ? 'Creating Profile...' : 'Complete Registration'}
                </Button>
              </div>
            </form>
          </CardContent>
          <CardFooter className="flex justify-center">
            <div className="text-sm text-muted-foreground">
              Your profile information can be updated later
            </div>
          </CardFooter>
        </Card>
      </div>
    </div>
  );
}

export default DoctorFormPage;
