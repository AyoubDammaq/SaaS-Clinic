import { useState } from "react";
import { Search, CalendarRange, Filter, X, CalendarPlus2 } from "lucide-react";
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
import { AppointmentStatus } from "./AppointmentStatusBadge";
import { Badge } from "@/components/ui/badge";
import { useTranslation } from "@/hooks/useTranslation";
import { Doctor } from "@/types/doctor";
import { Patient } from "@/types/patient";
import { Clinique } from "@/types/clinic";

interface AppointmentFiltersProps {
  searchTerm: string;
  onSearchChange: (value: string) => void;
  dateFilter: Date | null;
  onDateFilterChange: (date: Date | null) => void;
  statusFilter: AppointmentStatus | "all";
  onStatusFilterChange: (status: AppointmentStatus | "all") => void;
  clinicFilter: string | "all";
  onClinicFilterChange: (clinicId: string | "all") => void;
  doctorFilter: string | "all";
  onDoctorFilterChange: (doctorId: string | "all") => void;
  patientFilter: string | "all";
  onPatientFilterChange: (patientId: string | "all") => void;
  onCreateAppointment?: () => void;
  userRole?: string;
  clinics: Clinique[];
  doctors: Doctor[];
  patients: Patient[];
}

export const AppointmentFilters = ({
  searchTerm,
  onSearchChange,
  dateFilter,
  onDateFilterChange,
  statusFilter,
  onStatusFilterChange,
  clinicFilter,
  onClinicFilterChange,
  doctorFilter,
  onDoctorFilterChange,
  patientFilter,
  onPatientFilterChange,
  onCreateAppointment,
  userRole,
  clinics,
  doctors,
  patients,
}: AppointmentFiltersProps) => {
  const { t } = useTranslation("appointments");
  const tCommon = useTranslation("common").t;
  const [filtersExpanded, setFiltersExpanded] = useState(false);

  const hasActiveFilters =
    dateFilter ||
    statusFilter !== "all" ||
    clinicFilter !== "all" ||
    doctorFilter !== "all" ||
    patientFilter !== "all";

  const clearFilters = () => {
    onDateFilterChange(null);
    onStatusFilterChange("all");
    onClinicFilterChange("all");
    onDoctorFilterChange("all");
    onPatientFilterChange("all");
  };

  // Determine which filters to show based on user role
  const showClinicFilter = userRole === "Patient" || userRole === "SuperAdmin";
  const showDoctorFilter =
    userRole === "Patient" ||
    userRole === "ClinicAdmin" ||
    userRole === "SuperAdmin";
  const showPatientFilter =
    userRole === "Doctor" ||
    userRole === "ClinicAdmin" ||
    userRole === "SuperAdmin";
  const showStatusFilter =
    userRole === "Patient" ||
    userRole === "Doctor" ||
    userRole === "ClinicAdmin" ||
    userRole === "SuperAdmin";
  const showDateFilter =
    userRole === "Patient" ||
    userRole === "ClinicAdmin" ||
    userRole === "SuperAdmin" ||
    userRole === "Doctor";

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap gap-3 items-center">
        <div className="relative flex-grow max-w-sm">
          <Search className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input
            placeholder={t("searchAppointments")}
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

          {(userRole === "Patient" || userRole === "ClinicAdmin") && (
            <Button onClick={onCreateAppointment}>
              <CalendarPlus2 className="mr-1 h-4 w-4" />
              {userRole === "Patient"
                ? t("bookAppointment")
                : t("createAppointment")}
            </Button>
          )}
        </div>
      </div>

      {filtersExpanded && (
        <div className="bg-accent/30 p-3 rounded-md flex flex-wrap gap-4 items-end">
          {showClinicFilter && (
            <div className="space-y-2 min-w-[200px]">
              <Label htmlFor="clinic-filter">{tCommon("clinic")}</Label>
              <Select value={clinicFilter} onValueChange={onClinicFilterChange}>
                <SelectTrigger id="clinic-filter">
                  <SelectValue placeholder={t("allClinics")} />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">{t("allClinics")}</SelectItem>
                  {clinics.map((clinic) => (
                    <SelectItem key={clinic.id} value={clinic.id}>
                      {clinic.nom}
                    </SelectItem>
                  ))}
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

          {showStatusFilter && (
            <div className="space-y-2 min-w-[200px]">
              <Label htmlFor="status-filter">{tCommon("status")}</Label>
              <Select
                value={statusFilter}
                onValueChange={(value) =>
                  onStatusFilterChange(value as AppointmentStatus | "all")
                }
              >
                <SelectTrigger id="status-filter">
                  <SelectValue placeholder={t("allStatuses")} />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">{t("allStatuses")}</SelectItem>
                  <SelectItem value="CONFIRME">{t("scheduled")}</SelectItem>
                  <SelectItem value="EN_ATTENTE">{t("pending")}</SelectItem>
                  <SelectItem value="ANNULE">{t("cancelled")}</SelectItem>
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
          {statusFilter !== "all" && showStatusFilter && (
            <Badge variant="secondary" className="flex gap-1 items-center">
              {statusFilter}
              <X
                className="h-3 w-3 ml-1 cursor-pointer"
                onClick={() => onStatusFilterChange("all")}
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
