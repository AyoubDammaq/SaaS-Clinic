import { CalendarCheck, Clock, User, MapPin } from "lucide-react";
import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
} from "@/components/ui/card";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Badge } from "@/components/ui/badge";
import { cn } from "@/lib/utils";
import { AppointmentStatusEnum } from "@/types/rendezvous";
import { useTranslation } from "@/hooks/useTranslation";

// Interface for Appointment
interface Appointment {
  id: string;
  patientName: string;
  doctorName: string;
  date: string;
  time: string;
  status: AppointmentStatusEnum;
}

interface AppointmentListProps {
  appointments: Appointment[];
}

export function AppointmentList({ appointments }: AppointmentListProps) {
  const { t } = useTranslation("dashboard");

  if (appointments.length === 0) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>
            {t("upcoming_appointments_title") || "Upcoming Appointments"}
          </CardTitle>
          <CardDescription>
            {t("no_upcoming_appointments") || "No upcoming appointments"}
          </CardDescription>
        </CardHeader>
      </Card>
    );
  }

  return (
    <Card>
      <CardHeader>
        <CardTitle>
          {t("upcoming_appointments_title") || "Upcoming Appointments"}
        </CardTitle>
        <CardDescription>
          {t("upcoming_appointments_description") ||
            "List of your upcoming appointments"}
        </CardDescription>
      </CardHeader>
      <CardContent>
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead className="w-[200px]">
                <div className="flex items-center gap-2">
                  <User className="w-4 h-4 text-muted-foreground" />
                  {t("table_header_patient") || "Patient"}
                </div>
              </TableHead>
              <TableHead className="hidden md:table-cell w-[200px]">
                <div className="flex items-center gap-2">
                  <User className="w-4 h-4 text-muted-foreground" />
                  {t("table_header_doctor") || "Doctor"}
                </div>
              </TableHead>
              <TableHead className="w-[250px]">
                <div className="flex items-center gap-2">
                  <CalendarCheck className="w-4 h-4 text-muted-foreground" />
                  {t("table_header_date_time") || "Date & Time"}
                </div>
              </TableHead>
              <TableHead className="w-[150px]">
                <div className="flex items-center gap-2">
                  <MapPin className="w-4 h-4 text-muted-foreground" />
                  {t("table_header_status") || "Status"}
                </div>
              </TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {appointments.map((appointment) => (
              <TableRow key={appointment.id}>
                <TableCell className="font-medium">
                  {appointment.patientName}
                </TableCell>
                <TableCell className="hidden md:table-cell">
                  {appointment.doctorName}
                </TableCell>
                <TableCell>
                  <div className="flex flex-col space-y-1">
                    <div className="flex items-center text-sm">
                      <CalendarCheck className="mr-2 h-3.5 w-3.5 text-muted-foreground" />
                      <span>{appointment.date}</span>
                    </div>
                    <div className="flex items-center text-sm">
                      <Clock className="mr-2 h-3.5 w-3.5 text-muted-foreground" />
                      <span>{appointment.time}</span>
                    </div>
                  </div>
                </TableCell>
                <TableCell>
                  <StatusBadge status={appointment.status} />
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </CardContent>
    </Card>
  );
}

function StatusBadge({ status }: { status: AppointmentStatusEnum }) {
  const { t } = useTranslation("dashboard");
  // Status translation mapping
  const statusTranslations = {
    [AppointmentStatusEnum.CONFIRME]: t("status_confirmed") || "Confirmed",
    [AppointmentStatusEnum.EN_ATTENTE]: t("status_pending") || "Pending",
    [AppointmentStatusEnum.ANNULE]: t("status_cancelled") || "Cancelled",
  };
  return (
    <Badge
      variant="outline"
      className={cn(
        "text-xs",
        status === AppointmentStatusEnum.CONFIRME &&
          "border-blue-200 bg-blue-50 text-blue-700",
        status === AppointmentStatusEnum.EN_ATTENTE &&
          "border-green-200 bg-green-50 text-green-700",
        status === AppointmentStatusEnum.ANNULE &&
          "border-red-200 bg-red-50 text-red-700"
      )}
    >
      {statusTranslations[status]}
    </Badge>
  );
}
