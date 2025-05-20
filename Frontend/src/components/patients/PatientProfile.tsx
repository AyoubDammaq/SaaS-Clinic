
import { useState } from 'react';
import { differenceInYears } from 'date-fns';
import { Card, CardHeader, CardTitle, CardDescription, CardContent } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { PatientForm } from '@/components/patients/PatientForm';
import { useTranslation } from '@/hooks/useTranslation';

interface PatientProfileProps {
  patient: {
    id: string;
    name: string;
    email: string;
    phone: string;
    dateOfBirth: string;
    gender: "M" | "F" | "Autre";
    address?: string;
    lastVisit?: string;
  };
  onEditPatient: (patient: any) => void;
}

export function PatientProfile({ patient, onEditPatient }: PatientProfileProps) {
  const [isFormOpen, setIsFormOpen] = useState(false);
  const { t } = useTranslation();

  // Calculate age from date of birth
  const calculateAge = (dateOfBirth: string) => {
    return differenceInYears(new Date(), new Date(dateOfBirth));
  };

  return (
    <>
      <Card>
        <CardHeader>
          <CardTitle>My Profile</CardTitle>
          <CardDescription>Your personal information</CardDescription>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <div className="text-sm text-muted-foreground">{t('name')}</div>
                <div className="font-medium">{patient.name}</div>
              </div>
              <div>
                <div className="text-sm text-muted-foreground">{t('email')}</div>
                <div className="font-medium">{patient.email}</div>
              </div>
              <div>
                <div className="text-sm text-muted-foreground">Phone</div>
                <div className="font-medium">{patient.phone}</div>
              </div>
              <div>
                <div className="text-sm text-muted-foreground">Age</div>
                <div className="font-medium">{calculateAge(patient.dateOfBirth)}</div>
              </div>
              <div>
                <div className="text-sm text-muted-foreground">Gender</div>
                <div className="font-medium">{patient.gender}</div>
              </div>
              <div>
                <div className="text-sm text-muted-foreground">Date of Birth</div>
                <div className="font-medium">{patient.dateOfBirth}</div>
              </div>
              <div>
                <div className="text-sm text-muted-foreground">Last Visit</div>
                <div className="font-medium">{patient.lastVisit || 'N/A'}</div>
              </div>
              {patient.address && (
                <div>
                  <div className="text-sm text-muted-foreground">Address</div>
                  <div className="font-medium">{patient.address}</div>
                </div>
              )}
            </div>
            <Button variant="outline" onClick={() => setIsFormOpen(true)}>Edit Profile</Button>
          </div>
        </CardContent>
      </Card>

      <PatientForm 
        isOpen={isFormOpen}
        onClose={() => setIsFormOpen(false)}
        onSubmit={(data) => {
          onEditPatient({ ...patient, ...data });
          setIsFormOpen(false);
        }}
        initialData={patient}
      />
    </>
  );
}
