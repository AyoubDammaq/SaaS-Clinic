import { useState, useEffect, useRef } from 'react';
import { useAuth } from "@/hooks/useAuth";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { Search, Plus, FileEdit, Trash2 } from 'lucide-react';
import { Card, CardContent } from '@/components/ui/card';
import { Avatar, AvatarFallback } from '@/components/ui/avatar';
import { Badge } from '@/components/ui/badge';
import { DoctorForm } from '@/components/doctors/DoctorForm';
import { DoctorFilters } from '@/components/doctors/DoctorFilters';
import { DoctorProfile } from '@/components/doctors/DoctorProfile';
import { useDoctors } from '@/hooks/useDoctors';
import { toast } from 'sonner';
import { useCliniques } from '@/hooks/useCliniques';


export interface Doctor {
  id: string;
  prenom: string;
  nom: string;
  specialite: string;
  email: string;
  telephone: string;
  cliniqueId?: string;
  photoUrl?: string;
  dateCreation: string;
}

function DoctorsPage() {
  const { user } = useAuth();
  const { doctors, isLoading, fetchDoctors, setDoctors, deleteDoctor } = useDoctors(); // Ensure setDoctors is available
  const [searchTerm, setSearchTerm] = useState('');
  const [filteredDoctors, setFilteredDoctors] = useState<Doctor[]>([]);
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [selectedDoctor, setSelectedDoctor] = useState<Doctor | null>(null);
  const [selectedDoctorForProfile, setSelectedDoctorForProfile] = useState<Doctor | null>(null);
  const [filters, setFilters] = useState({ specialty: null as string | null, clinicId: null as string | null });
  const { cliniques } = useCliniques();
  const hasFetchedRef = useRef(false);

  useEffect(() => {
    if (!hasFetchedRef.current) {
      fetchDoctors().catch(() => toast.error("Failed to fetch doctors"));
      hasFetchedRef.current = true;
    }
  }, [fetchDoctors]);

  useEffect(() => {
    const results = doctors.filter((doctor) => {
      const matchesSearch =
        `${doctor.prenom} ${doctor.nom}`.toLowerCase().includes(searchTerm.toLowerCase()) ||
        doctor.specialite.toLowerCase().includes(searchTerm.toLowerCase()) ||
        doctor.email.toLowerCase().includes(searchTerm.toLowerCase());

      const matchesSpecialty = !filters.specialty || doctor.specialite === filters.specialty;
      const matchesClinic = !filters.clinicId || doctor.cliniqueId === filters.clinicId;

      return matchesSearch && matchesSpecialty && matchesClinic;
    });

    setFilteredDoctors(results);
  }, [searchTerm, doctors, filters]);

    const clinicsForForm = cliniques.map(c => ({
      id: c.id,
      name: c.nom,
    }));


  const handleFormSubmit = async (data: Doctor) => {
    try {
      // if (selectedDoctor) {
      //   const updatedDoctors = doctors.map((doctor) =>
      //     doctor.id === selectedDoctor.id ? { ...doctor, ...data } : doctor
      //   );
      //   setDoctors(updatedDoctors);
      //   toast.success('Doctor updated successfully');
      // } else {
      //   const newDoctor = {
      //     id: (doctors.length + 1).toString(),
      //     ...data,
      //     patients: 0,
      //   };
      // setDoctors([...doctors, newDoctor]);
      // toast.success('Doctor added successfully');
      // }
      // setSelectedDoctor(null);
      // setIsFormOpen(false);
      // await fetchDoctors();
      setSelectedDoctor(null);
      setIsFormOpen(false);
      await fetchDoctors();
    } catch (error) {
      console.error("Error submitting doctor form:", error);
      toast.error("Failed to save doctor. Please try again.");
    }
  };

  const handleEditDoctor = (doctor: Doctor) => {
    console.log("Editing doctor:", doctor);
    setSelectedDoctor(doctor);
    setIsFormOpen(true);
  };

  const handleDeleteDoctor = async (id: string) => {
    try {
      if (user.role === 'SuperAdmin') {
        await deleteDoctor(id);
        toast.success('Médecin supprimé avec succès');
      } else if (user.role === 'ClinicAdmin') {
        await unassignDoctorFromClinic(id);
        toast.success('Médecin désabonné de la clinique avec succès');
      } else {
        toast.error("Vous n'avez pas les droits pour effectuer cette action.");
        return;
      }

      // Mettre à jour la liste après suppression/désabonnement
      await fetchDoctors();
    } catch (error) {
      console.error('Erreur lors de la suppression ou désabonnement du médecin:', error);
      toast.error("Une erreur est survenue lors de la suppression.");
    }
  };


  const handleFilterChange = (newFilters: { specialty: string | null; clinicId: string | null }) => {
    setFilters(newFilters);
  };

  if (!user) {
    return <div className="p-8 text-center">Please log in to access this page.</div>;
  }

  const currentDoctor = user.role === 'Doctor' ? doctors.find((d) => d.email === user.email) || null : null;

  if (isLoading) {
    return <div className="p-8 text-center">Loading doctors...</div>;
  }

  if (user.role === 'Doctor' || selectedDoctorForProfile) {
    const doctorToShow = selectedDoctorForProfile || currentDoctor;

    if (!doctorToShow) {
      return <div className="p-8 text-center">Doctor data not found.</div>;
    }

    return (
      <div className="space-y-6 pb-8">
        <div className="flex flex-col gap-2">
          <h1 className="text-3xl font-bold tracking-tight">Doctor Profile</h1>
          <p className="text-muted-foreground">Manage your professional information and settings</p>
        </div>

        <DoctorProfile doctor={doctorToShow} onEdit={handleEditDoctor} clinics={clinicsForForm} />

        {user.role !== 'Doctor' && (
          <div className="mt-4">
            <Button variant="outline" onClick={() => setSelectedDoctorForProfile(null)}>
              Back to Doctors List
            </Button>
          </div>
        )}

        <DoctorForm
          isOpen={isFormOpen}
          onClose={() => setIsFormOpen(false)}
          onSubmit={handleFormSubmit}
          initialData={selectedDoctor ? { ...selectedDoctor } : undefined}
          clinics={clinicsForForm}
        />
      </div>
    );
  }

  return (
    <div className="space-y-6 pb-8">
      <div className="flex flex-col gap-2">
        <h1 className="text-3xl font-bold tracking-tight">Doctors</h1>
        <p className="text-muted-foreground">Manage and view doctors information</p>
      </div>

      <Card>
        <CardContent className="p-6">
          <div className="flex items-center justify-between mb-4">
            <div className="relative w-full max-w-sm">
              <Search className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
              <Input
                placeholder="Search doctors..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="pl-8"
                aria-label="Search doctors"
              />
            </div>
            <Button
              className="ml-2"
              onClick={() => {
                setSelectedDoctor(null);
                setIsFormOpen(true);
              }}
              aria-label="Add Doctor"
            >
              <Plus className="mr-1 h-4 w-4" /> Add Doctor
            </Button>
          </div>

          <DoctorFilters specialties={Array.from(new Set(doctors.map((d) => d.specialite)))} clinics={clinicsForForm} onFilterChange={handleFilterChange} />

          <div className="rounded-md border">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Doctor</TableHead>
                  <TableHead>Email</TableHead>
                  <TableHead>Specialization</TableHead>
                  <TableHead>Contact</TableHead>
                  <TableHead>Associated Clinic</TableHead>
                  <TableHead>Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {filteredDoctors.length === 0 ? (
                  <TableRow>
                    <TableCell colSpan={6} className="text-center py-4">
                      No doctors found matching your search criteria
                    </TableCell>
                  </TableRow>
                ) : (
                  filteredDoctors.map((doctor) => (
                    <TableRow
                      key={doctor.id}
                      className="cursor-pointer hover:bg-muted/60"
                      onClick={() => setSelectedDoctorForProfile(doctor)}
                    >
                      <TableCell>
                        <div className="flex items-center gap-3">
                          <Avatar className="h-8 w-8">
                            <AvatarFallback className="bg-clinic-500 text-white">
                              {doctor.prenom[0]}
                              {doctor.nom[0]}
                            </AvatarFallback>
                          </Avatar>
                          <div className="font-medium">
                            {doctor.prenom} {doctor.nom}
                          </div>
                        </div>
                      </TableCell>
                      <TableCell>{doctor.email}</TableCell>
                      <TableCell>
                        <Badge variant="outline" className="bg-blue-50 text-blue-600 border-blue-200">
                          {doctor.specialite}
                        </Badge>
                      </TableCell>
                      <TableCell>{doctor.telephone}</TableCell>
                      <TableCell>
                        {doctor.cliniqueId
                          ? cliniques.find((c) => c.id === doctor.cliniqueId)?.nom
                          : <span className="text-muted-foreground">None</span>}
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-2">
                          <Button size="sm" variant="ghost" onClick={(e) => { e.stopPropagation(); handleEditDoctor(doctor); }} aria-label="Edit Doctor">
                            <FileEdit className="h-4 w-4" />
                          </Button>
                          <Button
                            size="sm"
                            variant="ghost"
                            className="text-red-500"
                            onClick={(e) => { e.stopPropagation(); handleDeleteDoctor(doctor.id); }}
                            aria-label="Delete Doctor"
                          >
                            <Trash2 className="h-4 w-4" />
                          </Button>
                        </div>
                      </TableCell>
                    </TableRow>
                  ))
                )}
              </TableBody>
            </Table>
          </div>
        </CardContent>
      </Card>

      <DoctorForm
        isOpen={isFormOpen}
        onClose={() => setIsFormOpen(false)}
        onSubmit={handleFormSubmit}
        initialData={selectedDoctor ? { ...selectedDoctor } : undefined}
        clinics={clinicsForForm}
      />
    </div>
  );
}

export default DoctorsPage;

function unassignDoctorFromClinic(id: string) {
  throw new Error('Function not implemented.');
}
