
import { CalendarCheck, Clock, User } from "lucide-react";
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

type AppointmentStatus = 'CONFIRME' | 'ANNULE' | 'EN_ATTENTE';

interface Appointment {
  id: string;
  patientName: string;
  doctorName: string;
  date: string;
  time: string;
  status: AppointmentStatus;
}

interface AppointmentListProps {
  appointments: Appointment[];
}

export function AppointmentList({ appointments }: AppointmentListProps) {
  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h3 className="text-lg font-medium">Upcoming Appointments</h3>
      </div>
      
      <div className="border rounded-lg overflow-hidden">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Patient</TableHead>
              <TableHead className="hidden md:table-cell">Doctor</TableHead>
              <TableHead>Date & Time</TableHead>
              <TableHead>Status</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {appointments.map((appointment) => (
              <TableRow key={appointment.id}>
                <TableCell className="font-medium">{appointment.patientName}</TableCell>
                <TableCell className="hidden md:table-cell">{appointment.doctorName}</TableCell>
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
    </div>
  );
}

function StatusBadge({ status }: { status: AppointmentStatus }) {
  return (
    <Badge 
      variant="outline"
      className={cn(
        "text-xs",
        status === 'CONFIRME' && "border-blue-200 bg-blue-50 text-blue-700",
        status === 'EN_ATTENTE' && "border-green-200 bg-green-50 text-green-700",
        status === 'ANNULE' && "border-red-200 bg-red-50 text-red-700",
      )}
    >
      {status}
    </Badge>
  );
}
