import { useState, useEffect, useRef } from "react";
import { useAuth } from "@/hooks/useAuth";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Search, Plus, FileEdit, Trash2, X } from "lucide-react";
import { Card, CardContent } from "@/components/ui/card";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Badge } from "@/components/ui/badge";
import { DoctorForm } from "@/components/doctors/DoctorForm";
import { DoctorFilters } from "@/components/doctors/DoctorFilters";
import { DoctorProfile } from "@/components/doctors/DoctorProfile";
import { useDoctors } from "@/hooks/useDoctors";
import { toast } from "sonner";
import { useCliniques } from "@/hooks/useCliniques";
import { AssignDoctorModal } from "@/components/doctors/AssignDoctorModal";
import { ConfirmUnassignDialog } from "@/components/doctors/ConfirmUnassignDialog";
import { AppointmentForm } from "@/components/appointments/AppointmentForm";
import { useAppointments } from "@/hooks/useAppointments";
import { useDisponibilite } from "@/hooks/useDisponibilites";

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
  const {
    doctors,
    isLoading,
    isSubmitting,
    fetchDoctors,
    deleteDoctor,
    assignDoctorToClinic,
    unassignDoctorFromClinic,
  } = useDoctors();
  const { getAvailableDoctors } = useDisponibilite();
  const { cliniques } = useCliniques();
  const { handleAddAppointment } = useAppointments();

  const [searchTerm, setSearchTerm] = useState("");
  const [filteredDoctors, setFilteredDoctors] = useState<Doctor[]>([]);
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [selectedDoctor, setSelectedDoctor] = useState<Doctor | null>(null);
  const [selectedDoctorForProfile, setSelectedDoctorForProfile] =
    useState<Doctor | null>(null);
  const [filters, setFilters] = useState({
    specialty: null as string | null,
    clinicId: null as string | null,
    assignedStatus: "all" as "all" | "assigned" | "unassigned",
  });
  const [availabilityFilters, setAvailabilityFilters] = useState({
    date: null as string | null,
    heureDebut: null as string | null,
    heureFin: null as string | null,
  });

  const [isAssignModalOpen, setIsAssignModalOpen] = useState(false);
  const [doctorToAssign, setDoctorToAssign] = useState<Doctor | null>(null);
  const [selectedClinicId, setSelectedClinicId] = useState<string | null>(null);
  const [isAppointmentFormOpen, setIsAppointmentFormOpen] = useState(false);
  const [doctorForAppointment, setDoctorForAppointment] =
    useState<Doctor | null>(null);
  const hasFetchedDoctorsRef = useRef(false);

  useEffect(() => {
    if (!hasFetchedDoctorsRef.current) {
      fetchDoctors().catch(() => toast.error("Failed to fetch doctors"));
      hasFetchedDoctorsRef.current = true;
    }
  }, [fetchDoctors]);

  useEffect(() => {
    const { date, heureDebut, heureFin } = availabilityFilters;
    if (!date || !heureDebut || !heureFin) return;

    const fetchAvailableDoctors = async () => {
      try {
        const availableDoctorIds = await getAvailableDoctors(
          date,
          heureDebut,
          heureFin
        );
        const filtered = doctors.filter((doc) =>
          availableDoctorIds.includes(doc.id)
        );
        setFilteredDoctors(filtered);
      } catch (error) {
        console.error(
          "Erreur lors du filtrage des médecins disponibles",
          error
        );
        toast.error("Erreur de récupération des médecins disponibles.");
      }
    };

    fetchAvailableDoctors();
  }, [availabilityFilters, doctors, getAvailableDoctors]);

  useEffect(() => {
    const results = doctors.filter((doctor) => {
      const matchesSearch =
        `${doctor.prenom} ${doctor.nom}`
          .toLowerCase()
          .includes(searchTerm.toLowerCase()) ||
        doctor.specialite.toLowerCase().includes(searchTerm.toLowerCase()) ||
        doctor.email.toLowerCase().includes(searchTerm.toLowerCase());

      const matchesSpecialty =
        !filters.specialty || doctor.specialite === filters.specialty;
      const matchesClinic =
        !filters.clinicId || doctor.cliniqueId === filters.clinicId;

      const matchesAssignedStatus = (() => {
        if (filters.assignedStatus === "all") return true;
        if (user.role === "SuperAdmin") {
          return filters.assignedStatus === "assigned"
            ? !!doctor.cliniqueId
            : !doctor.cliniqueId;
        }
        if (user.role === "ClinicAdmin") {
          return filters.assignedStatus === "assigned"
            ? doctor.cliniqueId === user.cliniqueId
            : !doctor.cliniqueId;
        }
        return true;
      })();

      return (
        matchesSearch &&
        matchesSpecialty &&
        matchesClinic &&
        matchesAssignedStatus
      );
    });

    setFilteredDoctors(results);
  }, [searchTerm, doctors, filters, user]);

  const clinicsForForm = cliniques.map((c) => ({
    id: c.id,
    name: c.nom,
  }));

  const handleAvailabilityFilterChange = (filters: {
    date: string | null;
    heureDebut: string | null;
    heureFin: string | null;
  }) => {
    setAvailabilityFilters(filters);
  };

  const handleFilterChange = (newFilters: {
    specialty: string | null;
    clinicId: string | null;
    assignedStatus: "all" | "assigned" | "unassigned";
  }) => {
    setFilters(newFilters);
  };

  const handleFormSubmit = async (data: Doctor) => {
    try {
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

  const handleAssignDoctor = async (
    doctorId: string,
    clinicId: string | null | undefined
  ) => {
    if (!clinicId) {
      toast.error("Clinic ID is invalid.");
      return;
    }

    try {
      await assignDoctorToClinic(doctorId, clinicId);
      await fetchDoctors();
      setIsAssignModalOpen(false);
      setDoctorToAssign(null);
      setSelectedClinicId(null);

      const message =
        user.role === "ClinicAdmin"
          ? "Médecin assigné à votre clinique avec succès"
          : "Médecin assigné à la clinique avec succès";
      toast.success(message);
    } catch (error) {
      console.error("Error assigning doctor:", error);
      toast.error("Échec de l’assignation du médecin.");
    }
  };

  const handleUnassignDoctor = async (doctorId: string) => {
    try {
      await unassignDoctorFromClinic(doctorId);
      await fetchDoctors();
      toast.success("Médecin désassigné avec succès");
    } catch (error) {
      console.error("Erreur lors de la désassignation:", error);
      toast.error("Erreur lors de la désassignation du médecin");
    }
  };

  const handleDeleteDoctor = async (id: string) => {
    try {
      if (user.role === "SuperAdmin") {
        await deleteDoctor(id);
        toast.success("Médecin supprimé avec succès");
      } else if (user.role === "ClinicAdmin") {
        await unassignDoctorFromClinic(id);
        toast.success("Médecin désabonné de la clinique avec succès");
      } else {
        toast.error("Vous n'avez pas les droits pour effectuer cette action.");
        return;
      }
      await fetchDoctors();
    } catch (error) {
      console.error("Erreur lors de la suppression:", error);
      toast.error("Une erreur est survenue lors de la suppression.");
    }
  };

  if (!user) {
    return (
      <div className="p-8 text-center">Please log in to access this page.</div>
    );
  }

  const currentDoctor =
    user.role === "Doctor"
      ? doctors.find((d) => d.email === user.email) || null
      : null;

  if (isLoading) {
    return <div className="p-8 text-center">Loading doctors...</div>;
  }

  if (user.role === "Doctor" || selectedDoctorForProfile) {
    const doctorToShow = selectedDoctorForProfile || currentDoctor;

    if (!doctorToShow) {
      return <div className="p-8 text-center">Doctor data not found.</div>;
    }

    return (
      <div className="space-y-6 pb-8">
        <div className="flex flex-col gap-2">
          <h1 className="text-3xl font-bold tracking-tight">Doctor Profile</h1>
          <p className="text-muted-foreground">
            Manage your professional information and settings
          </p>
        </div>

        <DoctorProfile
          doctor={doctorToShow}
          onEdit={handleEditDoctor}
          clinics={clinicsForForm}
          userRole={user.role}
        />

        {user.role !== "Doctor" && (
          <div className="mt-4">
            <Button
              variant="outline"
              onClick={() => setSelectedDoctorForProfile(null)}
            >
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

  const doctorsOfClinic = filteredDoctors.filter(
    (doctor) => doctor.cliniqueId === user.cliniqueId
  );

  const availableDoctors = filteredDoctors.filter(
    (doctor) => !doctor.cliniqueId
  );

  const renderDoctorRow = (doctor: Doctor) => (
    <TableRow
      key={doctor.id}
      className="cursor-pointer hover:bg-muted/60"
      onClick={() => {
        setSelectedDoctorForProfile(doctor);
      }}
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
        <Badge
          variant="outline"
          className="bg-blue-50 text-blue-600 border-blue-200"
        >
          {doctor.specialite}
        </Badge>
      </TableCell>
      <TableCell>{doctor.telephone}</TableCell>
      <TableCell
        onClick={async (e) => {
          e.stopPropagation();

          if (user.role === "SuperAdmin") {
            // SuperAdmin → ouvrir modal pour choisir une clinique
            setDoctorToAssign(doctor);
            setSelectedClinicId(doctor.cliniqueId ?? null);
            setIsAssignModalOpen(true);
          } else if (user.role === "ClinicAdmin") {
            // ClinicAdmin → assigner directement à sa propre clinique
            if (!user.cliniqueId) {
              toast.error("Aucune clinique associée à cet administrateur.");
              return;
            }

            try {
              await handleAssignDoctor(doctor.id, user.cliniqueId);
            } catch (error) {
              console.error("Erreur d’assignation par ClinicAdmin :", error);
            }
          }
        }}
        className="cursor-pointer underline text-blue-600 hover:text-blue-800"
        title={
          user.role === "SuperAdmin"
            ? "Assigner à une clinique"
            : "Assigner à ma clinique"
        }
      >
        {doctor.cliniqueId ? (
          <Badge className="bg-green-100 text-green-800 border-green-200">
            {cliniques.find((c) => c.id === doctor.cliniqueId)?.nom}
          </Badge>
        ) : (
          <Badge className="bg-red-100 text-red-800 border-red-200">
            Non assigné
          </Badge>
        )}
      </TableCell>

      <TableCell>
        {user.role === "Patient" ? (
          <Button
            size="sm"
            variant="outline"
            className="text-blue-600"
            onClick={(e) => {
              e.stopPropagation();
              setDoctorForAppointment(doctor);
              setIsAppointmentFormOpen(true);
            }}
          >
            Prendre rendez-vous
          </Button>
        ) : (
          <div className="flex items-center gap-2">
            <Button
              size="sm"
              variant="ghost"
              onClick={(e) => {
                e.stopPropagation();
                handleEditDoctor(doctor);
              }}
              aria-label="Edit Doctor"
            >
              <FileEdit className="h-4 w-4" />
            </Button>
            <Button
              size="sm"
              variant="ghost"
              className="text-red-500"
              onClick={(e) => {
                e.stopPropagation();
                handleDeleteDoctor(doctor.id);
              }}
              aria-label="Delete Doctor"
            >
              <Trash2 className="h-4 w-4" />
            </Button>
            {doctor.cliniqueId && (
              <ConfirmUnassignDialog
                onConfirm={() => handleUnassignDoctor(doctor.id)}
              />
            )}
          </div>
        )}
      </TableCell>
    </TableRow>
  );

  return (
    <div className="space-y-6 pb-8">
      <div className="flex flex-col gap-2">
        <h1 className="text-3xl font-bold tracking-tight">Doctors</h1>
        <p className="text-muted-foreground">
          Manage and view doctors information
        </p>
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
            {user.role !== "Patient" && (
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
            )}
          </div>

          <DoctorFilters
            specialties={Array.from(new Set(doctors.map((d) => d.specialite)))}
            clinics={clinicsForForm}
            userRole={user.role as "SuperAdmin" | "ClinicAdmin"}
            onFilterChange={handleFilterChange}
            onAvailabilityFilterChange={handleAvailabilityFilterChange}
          />

          <div className="rounded-md border">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Doctor</TableHead>
                  <TableHead>Email</TableHead>
                  <TableHead>Specialization</TableHead>
                  <TableHead>Contact</TableHead>
                  <TableHead>Associated Clinic</TableHead>
                  <TableHead>
                    {user.role !== "Patient" ? "Actions" : "Prendre RDV"}
                  </TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {user.role === "ClinicAdmin" ? (
                  <>
                    {/* Section 1: Médecins de la clinique */}
                    <TableRow>
                      <TableCell colSpan={6} className="bg-muted font-semibold">
                        Médecins de la clinique
                      </TableCell>
                    </TableRow>
                    {doctorsOfClinic.length > 0 ? (
                      doctorsOfClinic.map((doctor) => renderDoctorRow(doctor))
                    ) : (
                      <TableRow>
                        <TableCell
                          colSpan={6}
                          className="text-center text-muted-foreground"
                        >
                          Aucun médecin assigné à cette clinique.
                        </TableCell>
                      </TableRow>
                    )}

                    {/* Section 2: Médecins disponibles */}
                    <TableRow>
                      <TableCell colSpan={6} className="bg-muted font-semibold">
                        Médecins disponibles
                      </TableCell>
                    </TableRow>
                    {availableDoctors.length > 0 ? (
                      availableDoctors.map((doctor) => renderDoctorRow(doctor))
                    ) : (
                      <TableRow>
                        <TableCell
                          colSpan={6}
                          className="text-center text-muted-foreground"
                        >
                          Aucun médecin disponible pour l’assignation.
                        </TableCell>
                      </TableRow>
                    )}
                  </>
                ) : // Default rendering for SuperAdmin and others
                filteredDoctors.length > 0 ? (
                  (user.role === "Patient"
                    ? filteredDoctors.filter((doctor) => !!doctor.cliniqueId)
                    : filteredDoctors
                  ).map((doctor) => renderDoctorRow(doctor))
                ) : (
                  <TableRow>
                    <TableCell
                      colSpan={user.role === "Patient" ? 6 : 6}
                      className="text-center py-4"
                    >
                      No doctors found matching your search criteria
                    </TableCell>
                  </TableRow>
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
      <AssignDoctorModal
        isOpen={isAssignModalOpen}
        onClose={() => setIsAssignModalOpen(false)}
        onConfirm={() => {
          if (doctorToAssign && selectedClinicId) {
            handleAssignDoctor(doctorToAssign.id, selectedClinicId);
            setIsAssignModalOpen(false);
          }
        }}
        selectedClinicId={selectedClinicId}
        setSelectedClinicId={setSelectedClinicId}
        clinics={clinicsForForm}
        isSubmitting={isSubmitting}
      />
      {isAppointmentFormOpen && doctorForAppointment && (
        <AppointmentForm
          isOpen={isAppointmentFormOpen}
          onClose={() => {
            setIsAppointmentFormOpen(false);
            setDoctorForAppointment(null);
          }}
          onSubmit={async (data) => {
            try {
              await handleAddAppointment(data);
              toast.success("Rendez-vous enregistré !");
            } catch (error) {
              toast.error("Erreur lors de la création du rendez-vous");
              console.error("Erreur de création de rendez-vous:", error);
            } finally {
              setIsAppointmentFormOpen(false);
              setDoctorForAppointment(null);
            }
          }}
          doctors={[doctorForAppointment]}
          patientId={user.patientId}
        />
      )}
    </div>
  );
}

export default DoctorsPage;
