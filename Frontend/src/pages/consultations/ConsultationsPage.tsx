import { useState } from "react";
import { useAuth } from "@/hooks/useAuth";
import { useConsultations } from "@/hooks/useConsultations";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Search, Plus, FileEdit, Trash2 } from "lucide-react";
import { Input } from "@/components/ui/input";
import {
  ConsultationForm,
  ConsultationFormValues,
} from "@/components/consultations/ConsultationForm";
import { toast } from "sonner";
import { useDoctors } from "@/hooks/useDoctors";
import { usePatients } from "@/hooks/usePatients";
import { Consultation, ConsultationDTO } from "@/types/consultation";
import { ConsultationDetails } from "@/components/consultations/ConsultationDetails";

function ConsultationsPage() {
  const { user } = useAuth();
  const {
    filteredConsultations,
    addConsultation,
    updateConsultation,
    deleteConsultation,
    searchTerm,
    setSearchTerm,
    permissions,
    refetchConsultations,
  } = useConsultations();
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [currentConsultation, setCurrentConsultation] =
    useState<Consultation | null>(null);
  const [selectedConsultation, setSelectedConsultation] =
    useState<Consultation | null>(null);
  const [isDetailsOpen, setIsDetailsOpen] = useState(false);
  const { doctors } = useDoctors();
  const { patients } = usePatients();

  function splitDateTime(dateTime: string) {
    if (!dateTime) return { date: "", time: "" };
    const [datePart, timePart] = dateTime.split("T");
    const time = timePart ? timePart.slice(0, 5) : ""; // "HH:mm"
    return { date: datePart, time };
  }

  const { date, time } = currentConsultation
    ? splitDateTime(currentConsultation.dateConsultation)
    : { date: "", time: "" };

  const handleCreateConsultation = () => {
    setCurrentConsultation(null);
    setIsFormOpen(true);
  };

  const handleEditConsultation = (consultation: Consultation) => {
    setCurrentConsultation(consultation);
    setIsFormOpen(true);
  };

  const handleDeleteConsultation = async (id: string) => {
    try {
      await deleteConsultation(id);
    } catch (err) {
      toast.error("Erreur lors de la suppression.");
    }
  };

  const handleSubmitConsultation = async (data: ConsultationFormValues) => {
    try {
      const consultationDTO: ConsultationDTO = {
        patientId: data.patientId,
        medecinId: user?.role === "Doctor" ? user.medecinId! : data.medecinId,
        dateConsultation: data.date, // <-- rename here
        diagnostic: data.diagnostic,
        notes: data.notes || "",
      };

      if (currentConsultation) {
        await updateConsultation({
          ...consultationDTO,
          id: currentConsultation.id,
        });
      } else {
        await addConsultation(consultationDTO);
      }
      setIsFormOpen(false);
    } catch (err) {
      toast.error("Échec de l'enregistrement de la consultation.");
    }
  };

  const handleViewConsultationDetails = (consultation: Consultation) => {
    setSelectedConsultation(consultation);
    setIsDetailsOpen(true);
  };

  return (
    <div className="space-y-6 pb-8">
      <div className="flex flex-col gap-2">
        <h1 className="text-3xl font-bold tracking-tight">Consultations</h1>
        <p className="text-muted-foreground">
          {user?.role === "Patient"
            ? "View your consultation history and medical notes"
            : user?.role === "Doctor"
            ? "Manage patient consultations and medical records"
            : "Oversee clinic consultations and medical history"}
        </p>
      </div>

      <div className="flex items-center justify-between mb-4">
        <div className="relative w-full max-w-sm">
          <Search className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input
            placeholder="Search consultations..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="pl-8"
          />
        </div>
        {permissions.canCreate && (
          <Button className="ml-2" onClick={handleCreateConsultation}>
            <Plus className="mr-1 h-4 w-4" />
            {user?.role === "Doctor"
              ? "New Consultation"
              : "Schedule Consultation"}
          </Button>
        )}
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Medical Consultations</CardTitle>
          <CardDescription>
            {user?.role === "Patient"
              ? "Your consultation history with doctors"
              : "Patient consultation records"}
          </CardDescription>
        </CardHeader>
        <CardContent>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Date</TableHead>
                <TableHead>Diagnostic</TableHead>
                {user?.role !== "Patient" && <TableHead>Patient</TableHead>}
                {user?.role !== "Doctor" && <TableHead>Doctor</TableHead>}
                <TableHead>Actions</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {filteredConsultations.map((consultation) => (
                <TableRow
                  key={consultation.id}
                  onClick={() => handleViewConsultationDetails(consultation)}
                  className="cursor-pointer hover:bg-muted/50 transition"
                >
                  <TableCell>{consultation.dateConsultation}</TableCell>
                  <TableCell>{consultation.diagnostic}</TableCell>
                  {user?.role !== "Patient" && (
                    <TableCell>
                      {(() => {
                        const patient = patients.find(
                          (p) => p.id === consultation.patientId
                        );
                        return patient
                          ? `${patient.prenom} ${patient.nom}`
                          : "N/A";
                      })()}
                    </TableCell>
                  )}
                  {user?.role !== "Doctor" && (
                    <TableCell>
                      {(() => {
                        const doctor = doctors.find(
                          (d) => d.id === consultation.medecinId
                        );
                        return doctor
                          ? `${doctor.prenom} ${doctor.nom}`
                          : "N/A";
                      })()}
                    </TableCell>
                  )}

                  <TableCell>
                    {(permissions.canEdit || permissions.canDelete) && (
                      <div className="flex gap-2">
                        {permissions.canEdit && (
                          <Button
                            size="sm"
                            variant="ghost"
                            onClick={() => handleEditConsultation(consultation)}
                          >
                            <FileEdit className="h-4 w-4" />
                          </Button>
                        )}
                        {permissions.canDelete && (
                          <Button
                            size="sm"
                            variant="ghost"
                            className="text-red-500 hover:text-red-700"
                            onClick={() =>
                              handleDeleteConsultation(consultation.id)
                            }
                          >
                            <Trash2 className="h-4 w-4" />
                          </Button>
                        )}
                      </div>
                    )}
                  </TableCell>
                </TableRow>
              ))}
              {filteredConsultations.length === 0 && (
                <TableRow>
                  <TableCell
                    colSpan={5}
                    className="text-center py-8 text-muted-foreground"
                  >
                    No consultations found
                  </TableCell>
                </TableRow>
              )}
              {selectedConsultation &&
                (() => {
                  const selectedPatient = patients.find(
                    (p) => p.id === selectedConsultation.patientId
                  );
                  const selectedDoctor = doctors.find(
                    (d) => d.id === selectedConsultation.medecinId
                  );

                  return (
                    <ConsultationDetails
                      isOpen={isDetailsOpen}
                      onClose={() => setIsDetailsOpen(false)}
                      consultation={selectedConsultation}
                      patientName={
                        `${selectedPatient?.prenom || ""} ${
                          selectedPatient?.nom || ""
                        }`.trim() || "N/A"
                      }
                      doctorName={
                        `${selectedDoctor?.prenom || ""} ${
                          selectedDoctor?.nom || ""
                        }`.trim() || "N/A"
                      }
                    />
                  );
                })()}
            </TableBody>
          </Table>
        </CardContent>
      </Card>

      <ConsultationForm
        isOpen={isFormOpen}
        onClose={() => setIsFormOpen(false)}
        onSubmit={handleSubmitConsultation}
        initialData={
          currentConsultation
            ? {
                patientId: currentConsultation.patientId,
                medecinId: currentConsultation.medecinId,
                date,
                time,
                diagnostic: currentConsultation.diagnostic,
                notes: currentConsultation.notes,
              }
            : undefined
        }
        patients={patients}
        doctors={doctors}
        user={user!}
      />
    </div>
  );
}

export default ConsultationsPage;
