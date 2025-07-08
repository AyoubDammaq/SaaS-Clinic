import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from "@/hooks/useAuth";
import { DossierMedical, Patient } from '@/types/patient';
import { PatientsList } from '@/components/patients/PatientsList';
import { PatientProfile } from '@/components/patients/PatientProfile';
import { PatientForm } from '@/components/patients/PatientForm';
import { PatientSettings } from '@/components/patients/PatientSettings';
import { usePatients } from '@/hooks/usePatients';
import { Button } from '@/components/ui/button';
import { FileText, Settings, User } from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';

type PatientFormValues = Omit<Patient, 'id' | 'dateCreation'>;

function PatientsPage() {
  const { user } = useAuth();
  const navigate = useNavigate();
  const {
    patients,
    filteredPatients,
    isLoading,
    permissions,
    searchTerm,
    setSearchTerm,
    handleAddPatient,
    handleUpdatePatient,
    handleDeletePatient
  } = usePatients();
  
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [selectedPatient, setSelectedPatient] = useState<Patient | null>(null);
  const [patientData, setPatientData] = useState<Patient | null>(null);
  const [activeTab, setActiveTab] = useState("profile");

  // Load patient data if user is a patient
  useEffect(() => {
    if (user?.role === 'Patient' && patients.length > 0) {
      // In a real app, we'd fetch the patient profile for the current user
      // Here we're mocking it by finding a patient with matching email
      const foundPatient = patients.find(p => p.email === user.email) || patients[0];
      setPatientData(foundPatient);
    }
  }, [user, patients]);

  // Navigate to medical record
  const handleViewMedicalRecord = (patientId: string) => {
    window.location.href = `medical-record/${patientId}`;
  };

  // Handle form submission
  const handleFormSubmit = async (data: PatientFormValues) => {
    try {
      if (selectedPatient) {
        // Update existing patient
        const payload = { ...data, id: selectedPatient.id };
        await handleUpdatePatient(selectedPatient.id, payload);
      } else {
        // Add new patient
        await handleAddPatient(data);
      }
      setSelectedPatient(null);
      setIsFormOpen(false);
    } catch (error) {
      console.error("Error submitting patient data:", error);
    }
  };

  // Handle edit patient
  const handleEditPatient = (patient: Patient) => {
    setSelectedPatient(patient);
    setIsFormOpen(true);
  };

  // Role-specific patient view
  const renderPatientView = () => {
    if (!user) return null;
    
    // For Patients - Show only their own information with tabs
    if (user.role === 'Patient') {
      if (!patientData) {
        return (
          <div className="flex justify-center items-center h-40">
            <p className="text-muted-foreground">Loading patient profile...</p>
          </div>
        );
      }
      
      const patientProfileData = {
        id: patientData.id,
        name: `${patientData.prenom} ${patientData.nom}`,
        email: patientData.email,
        phone: patientData.telephone,
        dateOfBirth: patientData.dateNaissance,
        gender: patientData.sexe,
        address: patientData.adresse
      };
      
      return (
        <Tabs 
          value={activeTab} 
          onValueChange={setActiveTab}
          className="w-full"
        >
          <TabsList className="mb-6 grid grid-cols-3 w-full md:w-auto">
            <TabsTrigger value="profile" className="flex items-center gap-2">
              <User className="h-4 w-4" />
              <span className="hidden md:inline">Profile</span>
            </TabsTrigger>
            <TabsTrigger value="medical" className="flex items-center gap-2">
              <FileText className="h-4 w-4" />
              <span className="hidden md:inline">Medical Record</span>
            </TabsTrigger>
            <TabsTrigger value="settings" className="flex items-center gap-2">
              <Settings className="h-4 w-4" />
              <span className="hidden md:inline">Settings</span>
            </TabsTrigger>
          </TabsList>

          <TabsContent value="profile" className="mt-0">
            <PatientProfile 
              patient={patientProfileData} 
              onEditPatient={(patient) => {
                // Map the patient profile data back to our Patient type format
                const updatedPatient: Partial<Patient> = {
                  id: patient.id,
                  nom: patient.name.split(' ')[1] || patientData?.nom || '',
                  prenom: patient.name.split(' ')[0] || patientData?.prenom || '',
                  email: patient.email,
                  telephone: patient.phone,
                  dateNaissance: patient.dateOfBirth,
                  sexe: patient.gender as "M" | "F", // Force the type to match our defined enum
                  adresse: patient.address
                };
                handleUpdatePatient(patient.id, updatedPatient);
              }}
            />
          </TabsContent>

          <TabsContent value="medical" className="mt-0">
            <Card>
              <CardHeader>
                <CardTitle>Medical Record</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="flex flex-col items-start gap-4">
                  <p>View your complete medical history, allergies, medications, and more.</p>
                  <Button 
                    onClick={() => handleViewMedicalRecord(patientData.id)}
                    className="gap-2"
                  >
                    <FileText className="h-4 w-4" />
                    View Medical Record
                  </Button>
                </div>
              </CardContent>
            </Card>
          </TabsContent>

          <TabsContent value="settings" className="mt-0">
            <PatientSettings />
          </TabsContent>
        </Tabs>
      );
    }
    
    async function fetchMedicalRecord(patientId: string): Promise<DossierMedical | null> {
      try {
        // Simulate fetching the medical record for the selected patient
        const medicalRecord: DossierMedical = await fetch(`medical-records/${patientId}`).then(res => res.json());
        return medicalRecord;
      } catch (error) {
        console.error("Failed to fetch medical record:", error);
        throw error;
      }
    }

    // For ClinicAdmin and Doctor - Show full patient list with actions
    return (
      <PatientsList 
        patients={patients}
        filteredPatients={filteredPatients}
        searchTerm={searchTerm}
        setSearchTerm={setSearchTerm}
        isLoading={isLoading}
        permissions={permissions}
        onAddPatient={() => { setSelectedPatient(null); setIsFormOpen(true); }}
        onEditPatient={handleEditPatient}
        onDeletePatient={handleDeletePatient}
        fetchMedicalRecord={fetchMedicalRecord}
      />
    );
  };

  return (
    <div className="space-y-6 pb-8">
      <div className="flex flex-col gap-2">
        <h1 className="text-3xl font-bold tracking-tight">
          {user?.role === 'Patient' ? 'Patient Portal' : 'Patients'}
        </h1>
        <p className="text-muted-foreground">
          {user?.role === 'Patient' 
            ? 'Manage your patient profile and view your medical information' 
            : 'Manage and view patient information'}
        </p>
      </div>
      {renderPatientView()}
      
      {/* Patient Form Modal */}
      <PatientForm 
        isOpen={isFormOpen}
        onClose={() => {
          setIsFormOpen(false);
          setSelectedPatient(null);
        }}
        onSubmit={handleFormSubmit}
        initialData={selectedPatient || undefined}
        isLoading={isLoading}
      />
    </div>
  );
}

export default PatientsPage;
