import { CalendarCheck, Clock } from "lucide-react";
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

// Status translation mapping
const statusTranslations: Record<AppointmentStatusEnum, string> = {
  [AppointmentStatusEnum.CONFIRME]: "Confirmé",
  [AppointmentStatusEnum.EN_ATTENTE]: "En attente",
  [AppointmentStatusEnum.ANNULE]: "Annulé",
};

export function AppointmentList({ appointments }: AppointmentListProps) {
  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h3 className="text-lg font-medium">Rendez-vous à venir</h3>
      </div>

      {appointments.length === 0 ? (
        <div className="text-center text-muted-foreground">
          Aucun rendez-vous à venir
        </div>
      ) : (
        <div className="border rounded-lg overflow-hidden">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Patient</TableHead>
                <TableHead className="hidden md:table-cell">Médecin</TableHead>
                <TableHead>Date & Heure</TableHead>
                <TableHead>Statut</TableHead>
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
        </div>
      )}
    </div>
  );
}

function StatusBadge({ status }: { status: AppointmentStatusEnum }) {
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
