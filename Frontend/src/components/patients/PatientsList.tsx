
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { differenceInYears } from 'date-fns';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { Search, Plus, FileEdit, Trash2, X } from 'lucide-react';
import { Card, CardContent } from '@/components/ui/card';
import { Patient } from '@/types/patient';
import { toast } from 'sonner';
import { DossierMedical } from '@/types/patient';
import { CreateMedicalRecordModal } from '@/components/patients/CreateMedicalRecordForm';
import { MedicalRecordView } from '@/components/patients/MedicalRecordView';
import { MedicalRecord } from '@/components/patients/MedicalRecordView';

interface PatientsListProps {
  patients: Patient[];
  filteredPatients: Patient[];
  searchTerm: string;
  setSearchTerm: (term: string) => void;
  isLoading: boolean;
  permissions: {
    canCreate: boolean;
    canEdit: boolean;
    canDelete: boolean;
  };
  onAddPatient: () => void;
  onEditPatient: (patient: Patient) => void;
  onDeletePatient: (id: string) => void;
  fetchMedicalRecord: (patientId: string) => Promise<DossierMedical | null>;
}

export function PatientsList({
  patients,
  filteredPatients,
  searchTerm,
  setSearchTerm,
  isLoading,
  permissions,
  onAddPatient,
  onEditPatient,
  onDeletePatient,
  fetchMedicalRecord,
}: PatientsListProps) {
  const navigate = useNavigate();

  const [modalPatient, setModalPatient] = useState<Patient | null>(null);
  const [modalMedicalRecord, setModalMedicalRecord] = useState<DossierMedical | null>(null);
  const [modalLoading, setModalLoading] = useState(false);
  const [modalError, setModalError] = useState<string | null>(null);
  const [isFormOpen, setIsFormOpen] = useState(false);

  // Calculate age from date of birth
  const calculateAge = (dateOfBirth: string) => {
    return differenceInYears(new Date(), new Date(dateOfBirth));
  };

  // Handle click on a patient row to view their medical record
  const handlePatientClick = (patient: Patient) => {
      navigate(`/medical-record/${patient.id}`);
  };

  // Close the modal and reset states
  const closeModal = () => {
    setModalPatient(null);
    setModalMedicalRecord(null);
    setModalError(null);
    setModalLoading(false);
    setIsFormOpen(false);
  };

    // Navigate to create medical record page
  const handleCreateMedicalRecord = () => {
    if (modalPatient) {
      setIsFormOpen(true); // Open the modal
    }
  };

  // Navigate to medical record details page
  const handleViewMedicalRecordPage = (modalPatient: Patient) => {
    if (modalPatient) {
      closeModal();
      navigate(`/medical-records/${modalPatient.id}`);
    }
  };

  const mapDossierMedicalToMedicalRecord = (dossier: DossierMedical): MedicalRecord => {
    return {
      id: dossier.id,
      patientId: dossier.patientId,
      allergies: dossier.allergies || '',
      chronicDiseases: dossier.maladiesChroniques || '', // Map "maladiesChroniques"
      currentMedications: dossier.medicamentsActuels || '', // Map "medicamentsActuels"
      bloodType: dossier.groupeSanguin || '', // Map "groupeSanguin"
      creationDate: dossier.dateCreation || '', // Map "dateCreation"
      personalHistory: dossier.antécédentsPersonnels || '', // Map "antécédentsPersonnels"
      familyHistory: dossier.antécédentsFamiliaux || '', // Map "antécédentsFamiliaux"
      documents: (dossier.documents || []).map((doc) => ({
        id: doc.id,
        nom: doc.nom || '',
        type: doc.type,
        dateCreation: doc.dateCreation || '',
        url: doc.url || '',
      })),
    };
  };

  if (isLoading) {
    return (
      <Card>
        <CardContent className="pt-6">
          <div className="flex justify-center items-center h-40">
            <p className="text-muted-foreground">Chargement des patients...</p>
          </div>
        </CardContent>
      </Card>
    );
  }


  return (
    <>
      <div>
        {/* ... existing search, add button and table ... */}
        <div className="flex items-center justify-between mb-4">
          <div className="relative w-full max-w-sm">
            <Search className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
            <Input
              placeholder="Rechercher des patients..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="pl-8"
            />
          </div>
          {permissions.canCreate && (
            <Button className="ml-2" onClick={onAddPatient}>
              <Plus className="mr-1 h-4 w-4" /> Ajouter un patient
            </Button>
          )}
        </div>

        <div className="rounded-md border">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Nom complet</TableHead>
                <TableHead>Email</TableHead>
                <TableHead>Téléphone</TableHead>
                <TableHead>Âge</TableHead>
                <TableHead>Sexe</TableHead>
                <TableHead>Date d'inscription</TableHead>
                {(permissions.canEdit || permissions.canDelete) && <TableHead>Actions</TableHead>}
              </TableRow>
            </TableHeader>
            <TableBody>
              {filteredPatients.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={7} className="h-24 text-center">
                    Aucun patient trouvé
                  </TableCell>
                </TableRow>
              ) : (
                filteredPatients.map((patient) => (
                  <TableRow
                    key={patient.id}
                    className="cursor-pointer hover:bg-muted/60"
                    onClick={() => handlePatientClick(patient)}
                  >
                    <TableCell className="font-medium">
                      {patient.prenom} {patient.nom}
                    </TableCell>
                    <TableCell>{patient.email}</TableCell>
                    <TableCell>{patient.telephone}</TableCell>
                    <TableCell>{calculateAge(patient.dateNaissance)}</TableCell>
                    <TableCell>
                      {patient.sexe === 'M' ? 'Homme' : patient.sexe === 'F' ? 'Femme' : 'Autre'}
                    </TableCell>
                    <TableCell>{patient.dateCreation}</TableCell>
                    {(permissions.canEdit || permissions.canDelete) && (
                      <TableCell onClick={(e) => e.stopPropagation()}>
                        <div className="flex items-center gap-2">
                          {permissions.canEdit && (
                            <Button
                              size="sm"
                              variant="ghost"
                              onClick={(e) => {
                                e.stopPropagation();
                                onEditPatient(patient);
                              }}
                            >
                              <FileEdit className="h-4 w-4" />
                            </Button>
                          )}
                          {permissions.canDelete && (
                            <Button
                              size="sm"
                              variant="ghost"
                              className="text-red-500"
                              onClick={(e) => {
                                e.stopPropagation();
                                onDeletePatient(patient.id);
                              }}
                            >
                              <Trash2 className="h-4 w-4" />
                            </Button>
                          )}
                        </div>
                      </TableCell>
                    )}
                  </TableRow>
                ))
              )}
            </TableBody>
          </Table>
        </div>
      </div>
    </>
  );
}