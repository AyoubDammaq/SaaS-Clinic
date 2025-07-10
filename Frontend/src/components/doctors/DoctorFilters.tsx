import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Label } from "@/components/ui/label";
import { Filter, X } from "lucide-react";
import { Input } from "../ui/input";

interface DoctorFiltersProps {
  specialties: string[];
  clinics: { id: string; name: string }[];
  userRole: "SuperAdmin" | "ClinicAdmin" | "Patient";
  onFilterChange: (filters: {
    specialty: string | null;
    clinicId: string | null;
    assignedStatus: "all" | "assigned" | "unassigned";
  }) => void;
  onAvailabilityFilterChange?: (availability: {
    date: string | null;
    heureDebut: string | null;
    heureFin: string | null;
  }) => void;
}

export function DoctorFilters({
  specialties,
  clinics,
  userRole,
  onFilterChange,
  onAvailabilityFilterChange,
}: DoctorFiltersProps) {
  const [showFilters, setShowFilters] = useState(false);
  const [specialty, setSpecialty] = useState<string | null>(null);
  const [clinicId, setClinicId] = useState<string | null>(null);
  const [assignedStatus, setAssignedStatus] = useState<
    "all" | "assigned" | "unassigned"
  >("all");
  const [availabilityDate, setAvailabilityDate] = useState<string | null>(null);
  const [heureDebut, setHeureDebut] = useState<string | null>(null);
  const [heureFin, setHeureFin] = useState<string | null>(null);

  const handleSpecialtyChange = (value: string) => {
    const newValue = value === "all_specialties" ? null : value;
    setSpecialty(newValue);
    onFilterChange({ specialty: newValue, clinicId, assignedStatus });
  };

  const handleClinicChange = (value: string) => {
    const newValue = value === "all_clinics" ? null : value;
    setClinicId(newValue);
    onFilterChange({ specialty, clinicId: newValue, assignedStatus });
  };

  const handleAssignedStatusChange = (
    value: "all" | "assigned" | "unassigned"
  ) => {
    setAssignedStatus(value);
    onFilterChange({ specialty, clinicId, assignedStatus: value });
  };

  const handleAvailabilityChange = (
    date: string | null,
    debut: string | null,
    fin: string | null
  ) => {
    setAvailabilityDate(date);
    setHeureDebut(debut);
    setHeureFin(fin);
    onAvailabilityFilterChange?.({
      date,
      heureDebut: debut,
      heureFin: fin,
    });
  };

  const clearFilters = () => {
    setSpecialty(null);
    setClinicId(null);
    onFilterChange({ specialty: null, clinicId: null, assignedStatus: "all" });
  };

  return (
    <>
      <div className="flex justify-between items-center mb-4">
        <Button
          variant="outline"
          size="sm"
          className="flex items-center gap-2"
          onClick={() => setShowFilters(!showFilters)}
        >
          <Filter className="h-4 w-4" />
          Filters
        </Button>

        {(specialty || clinicId || assignedStatus !== "all") && (
          <Button
            variant="ghost"
            size="sm"
            className="flex items-center gap-2 text-muted-foreground"
            onClick={clearFilters}
          >
            <X className="h-4 w-4" />
            Clear Filters
          </Button>
        )}
      </div>

      {showFilters && (
        <Card className="mb-6">
          <CardContent className="p-4">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label htmlFor="specialty-filter">Specialty</Label>
                <Select
                  value={specialty || "all_specialties"}
                  onValueChange={handleSpecialtyChange}
                >
                  <SelectTrigger id="specialty-filter">
                    <SelectValue placeholder="All Specialties" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all_specialties">
                      All Specialties
                    </SelectItem>
                    {specialties.map((specialty) => (
                      <SelectItem key={specialty} value={specialty}>
                        {specialty}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>

              <div className="space-y-2">
                <Label htmlFor="clinic-filter">Clinic</Label>
                <Select
                  value={clinicId || "all_clinics"}
                  onValueChange={handleClinicChange}
                >
                  <SelectTrigger id="clinic-filter">
                    <SelectValue placeholder="All Clinics" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all_clinics">All Clinics</SelectItem>
                    {clinics.map((clinic) => (
                      <SelectItem key={clinic.id} value={clinic.id}>
                        {clinic.name}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>

              {userRole !== "Patient" && (
                <div className="space-y-2">
                  <Label htmlFor="assigned-status-filter">
                    {userRole === "SuperAdmin"
                      ? "Affectation"
                      : "Disponibilité"}
                  </Label>
                  <Select
                    value={assignedStatus}
                    onValueChange={handleAssignedStatusChange}
                  >
                    <SelectTrigger id="assigned-status-filter">
                      <SelectValue placeholder="Tous" />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="all">Tous</SelectItem>
                      <SelectItem value="assigned">
                        {userRole === "SuperAdmin"
                          ? "Assignés"
                          : "Médecins de la clinique"}
                      </SelectItem>
                      <SelectItem value="unassigned">
                        {userRole === "SuperAdmin"
                          ? "Non assignés"
                          : "Médecins disponibles"}
                      </SelectItem>
                    </SelectContent>
                  </Select>
                </div>
              )}

              <div className="space-y-2">
                <Label>Disponibilité à la date</Label>
                <Input
                  type="date"
                  value={availabilityDate || ""}
                  onChange={(e) =>
                    handleAvailabilityChange(
                      e.target.value || null,
                      heureDebut,
                      heureFin
                    )
                  }
                />
              </div>
              <div className="space-y-2">
                <Label>Heure de début</Label>
                <Input
                  type="time"
                  value={heureDebut || ""}
                  onChange={(e) =>
                    handleAvailabilityChange(
                      availabilityDate,
                      e.target.value || null,
                      heureFin
                    )
                  }
                />
              </div>
              <div className="space-y-2">
                <Label>Heure de fin</Label>
                <Input
                  type="time"
                  value={heureFin || ""}
                  onChange={(e) =>
                    handleAvailabilityChange(
                      availabilityDate,
                      heureDebut,
                      e.target.value || null
                    )
                  }
                />
              </div>
            </div>
          </CardContent>
        </Card>
      )}
    </>
  );
}
