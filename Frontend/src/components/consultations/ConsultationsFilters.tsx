import { useState } from "react";
import { Search, CalendarRange, Filter, X, Plus } from "lucide-react";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import {
  Popover,
  PopoverTrigger,
  PopoverContent,
} from "@/components/ui/popover";
import { Calendar } from "@/components/ui/calendar";
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { format } from "date-fns";
import { cn } from "@/lib/utils";
import { Badge } from "@/components/ui/badge";
import { useTranslation } from "@/hooks/useTranslation";
import { Doctor } from "@/types/doctor";
import { Patient } from "@/types/patient";
import { Clinique } from "@/types/clinic";
import { ConsultationType, consultationTypes } from "@/types/consultation";

interface ConsultationsFiltersProps {
  searchTerm: string;
  onSearchChange: (value: string) => void;
  dateFilter: Date | null;
  onDateFilterChange: (date: Date | null) => void;
  typeFilter: ConsultationType | "all";
  onTypeFilterChange: (type: ConsultationType | "all") => void;
  clinicFilter: string | "all";
  onClinicFilterChange: (clinicId: string | "all") => void;
  doctorFilter: string | "all";
  onDoctorFilterChange: (doctorId: string | "all") => void;
  patientFilter: string | "all";
  onPatientFilterChange: (patientId: string | "all") => void;
  onCreateConsultation?: () => void;
  userRole?: string;
  clinics: Clinique[];
  doctors: Doctor[];
  patients: Patient[];
}

export const ConsultationsFilters = ({
  searchTerm,
  onSearchChange,
  dateFilter,
  onDateFilterChange,
  typeFilter,
  onTypeFilterChange,
  clinicFilter,
  onClinicFilterChange,
  doctorFilter,
  onDoctorFilterChange,
  patientFilter,
  onPatientFilterChange,
  onCreateConsultation,
  userRole,
  clinics,
  doctors,
  patients,
}: ConsultationsFiltersProps) => {
  const { t } = useTranslation("consultations");
  const tCommon = useTranslation("common").t;
  const [filtersExpanded, setFiltersExpanded] = useState(false);

  const hasActiveFilters =
    dateFilter ||
    typeFilter !== "all" ||
    clinicFilter !== "all" ||
    doctorFilter !== "all" ||
    patientFilter !== "all";

  const clearFilters = () => {
    onDateFilterChange(null);
    onTypeFilterChange("all");
    onClinicFilterChange("all");
    onDoctorFilterChange("all");
    onPatientFilterChange("all");
  };

  // Déterminer quels filtres afficher selon le rôle de l'utilisateur
  const showClinicFilter = userRole === "Patient" || userRole === "SuperAdmin";
  const showDoctorFilter =
    userRole === "Patient" ||
    userRole === "ClinicAdmin" ||
    userRole === "SuperAdmin";
  const showPatientFilter =
    userRole === "Doctor" ||
    userRole === "ClinicAdmin" ||
    userRole === "SuperAdmin";
  const showTypeFilter =
    userRole === "Patient" ||
    userRole === "Doctor" ||
    userRole === "ClinicAdmin" ||
    userRole === "SuperAdmin";
  const showDateFilter =
    userRole === "Patient" ||
    userRole === "Doctor" ||
    userRole === "ClinicAdmin" ||
    userRole === "SuperAdmin";

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap gap-3 items-center">
        <div className="relative flex-grow max-w-sm">
          <Search className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input
            placeholder={t("searchConsultationsPlaceholder")}
            value={searchTerm}
            onChange={(e) => onSearchChange(e.target.value)}
            className="pl-8"
          />
        </div>

        <div className="flex gap-2">
          <Button
            variant="outline"
            size="icon"
            onClick={() => setFiltersExpanded(!filtersExpanded)}
            className={cn(hasActiveFilters && "border-primary text-primary")}
          >
            <Filter className="h-4 w-4" />
          </Button>

          {showDateFilter && (
            <Popover>
              <PopoverTrigger asChild>
                <Button
                  variant="outline"
                  size="icon"
                  className={cn(dateFilter && "border-primary text-primary")}
                >
                  <CalendarRange className="h-4 w-4" />
                </Button>
              </PopoverTrigger>
              <PopoverContent className="w-auto p-0" align="end">
                <Calendar
                  mode="single"
                  selected={dateFilter || undefined}
                  onSelect={onDateFilterChange}
                  initialFocus
                  className={cn("p-3 pointer-events-auto")}
                />
                {dateFilter && (
                  <div className="border-t p-3 flex justify-between">
                    <div className="text-sm font-medium">
                      {format(dateFilter, "PPP")}
                    </div>
                    <Button
                      variant="ghost"
                      size="sm"
                      onClick={() => onDateFilterChange(null)}
                    >
                      <X className="h-4 w-4 mr-1" /> {tCommon("clear")}
                    </Button>
                  </div>
                )}
              </PopoverContent>
            </Popover>
          )}

          {onCreateConsultation && (
            <Button onClick={onCreateConsultation}>
              <Plus className="mr-1 h-4 w-4" />
              {userRole === "Doctor"
                ? t("createConsultation")
                : t("scheduleConsultation")}
            </Button>
          )}
        </div>
      </div>

      {filtersExpanded && (
        <div className="bg-accent/30 p-3 rounded-md flex flex-wrap gap-4 items-end">
          {showClinicFilter && (
            <div className="space-y-2 min-w-[200px]">
              <Label htmlFor="clinic-filter">{t("clinic")}</Label>
              <Select value={clinicFilter} onValueChange={onClinicFilterChange}>
                <SelectTrigger id="clinic-filter">
                  <SelectValue placeholder={t("allClinics")} />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">{t("allClinics")}</SelectItem>
                  {clinics.length === 0 ? (
                    <SelectItem disabled value="">
                      {tCommon("noClinicsAvailable")}
                    </SelectItem>
                  ) : (
                    clinics.map((clinic) => (
                      <SelectItem key={clinic.id} value={clinic.id}>
                        {clinic.nom}
                      </SelectItem>
                    ))
                  )}
                </SelectContent>
              </Select>
            </div>
          )}

          {showDoctorFilter && (
            <div className="space-y-2 min-w-[200px]">
              <Label htmlFor="doctor-filter">{tCommon("doctor")}</Label>
              <Select value={doctorFilter} onValueChange={onDoctorFilterChange}>
                <SelectTrigger id="doctor-filter">
                  <SelectValue placeholder={t("allDoctors")} />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">{t("allDoctors")}</SelectItem>
                  {doctors.map((doctor) => (
                    <SelectItem key={doctor.id} value={doctor.id}>
                      {`Dr. ${doctor.prenom} ${doctor.nom}`}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          )}

          {showPatientFilter && (
            <div className="space-y-2 min-w-[200px]">
              <Label htmlFor="patient-filter">{tCommon("patient")}</Label>
              <Select
                value={patientFilter}
                onValueChange={onPatientFilterChange}
              >
                <SelectTrigger id="patient-filter">
                  <SelectValue placeholder={t("allPatients")} />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">{t("allPatients")}</SelectItem>
                  {patients.map((patient) => (
                    <SelectItem key={patient.id} value={patient.id}>
                      {`${patient.prenom} ${patient.nom}`}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          )}

          {showTypeFilter && (
            <div className="space-y-2 min-w-[200px]">
              <Label htmlFor="type-filter">{t("typeConsultation")}</Label>
              <Select
                value={typeFilter.toString()}
                onValueChange={(value) =>
                  onTypeFilterChange(
                    value === "all"
                      ? "all"
                      : (parseInt(value) as ConsultationType)
                  )
                }
              >
                <SelectTrigger id="type-filter">
                  <SelectValue placeholder={t("allTypes")} />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">{t("allTypes")}</SelectItem>
                  {Object.entries(consultationTypes).map(([value, label]) => (
                    <SelectItem key={value} value={value}>
                      {t(label)}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          )}

          {hasActiveFilters && (
            <Button
              variant="ghost"
              size="sm"
              onClick={clearFilters}
              className="ml-auto"
            >
              <X className="h-4 w-4 mr-1" /> {tCommon("clearFilters")}
            </Button>
          )}
        </div>
      )}

      {hasActiveFilters && !filtersExpanded && (
        <div className="flex flex-wrap items-center gap-2">
          <span className="text-sm text-muted-foreground">
            {t("activeFilters")}:
          </span>
          {dateFilter && showDateFilter && (
            <Badge variant="secondary" className="flex gap-1 items-center">
              <CalendarRange className="h-3 w-3" />
              {format(dateFilter, "PP")}
              <X
                className="h-3 w-3 ml-1 cursor-pointer"
                onClick={() => onDateFilterChange(null)}
              />
            </Badge>
          )}
          {typeFilter !== "all" && showTypeFilter && (
            <Badge variant="secondary" className="flex gap-1 items-center">
              {t(consultationTypes[typeFilter])}
              <X
                className="h-3 w-3 ml-1 cursor-pointer"
                onClick={() => onTypeFilterChange("all")}
              />
            </Badge>
          )}
          {clinicFilter !== "all" && showClinicFilter && (
            <Badge variant="secondary" className="flex gap-1 items-center">
              {clinics.find((c) => c.id === clinicFilter)?.nom || clinicFilter}
              <X
                className="h-3 w-3 ml-1 cursor-pointer"
                onClick={() => onClinicFilterChange("all")}
              />
            </Badge>
          )}
          {doctorFilter !== "all" && showDoctorFilter && (
            <Badge variant="secondary" className="flex gap-1 items-center">
              {doctors.find((d) => d.id === doctorFilter)?.nom || doctorFilter}
              <X
                className="h-3 w-3 ml-1 cursor-pointer"
                onClick={() => onDoctorFilterChange("all")}
              />
            </Badge>
          )}
          {patientFilter !== "all" && showPatientFilter && (
            <Badge variant="secondary" className="flex gap-1 items-center">
              {patients.find((p) => p.id === patientFilter)?.nom ||
                patientFilter}
              <X
                className="h-3 w-3 ml-1 cursor-pointer"
                onClick={() => onPatientFilterChange("all")}
              />
            </Badge>
          )}
          <Button
            variant="ghost"
            size="sm"
            onClick={clearFilters}
            className="h-7 px-2"
          >
            {tCommon("clearAll")}
          </Button>
        </div>
      )}
    </div>
  );
};
