import { CalendarClock, User, MapPin } from "lucide-react";
import { Card, CardHeader, CardTitle, CardDescription, CardContent } from "@/components/ui/card";

interface Appointment {
  patientName: string;
  time: string; // format: "HH:mm"
  date: string; // format: "YYYY-MM-DD"
  commentaire: string;
}

interface NextAppointmentCardProps {
  appointment: Appointment | null;
}

export function NextAppointmentCard({ appointment }: NextAppointmentCardProps) {
  if (!appointment) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>Prochain Rendez-vous</CardTitle>
          <CardDescription>Aucun rendez-vous à venir</CardDescription>
        </CardHeader>
      </Card>
    );
  }

  return (
    <Card>
      <CardHeader>
        <CardTitle>Prochain Rendez-vous</CardTitle>
        <CardDescription>{appointment.date} à {appointment.time}</CardDescription>
      </CardHeader>
      <CardContent className="space-y-3">
        <div className="flex items-center gap-2 text-sm text-muted-foreground">
          <User className="w-4 h-4" />
          <span>Patient : {appointment.patientName}</span>
        </div>
        <div className="flex items-center gap-2 text-sm text-muted-foreground">
          <CalendarClock className="w-4 h-4" />
          <span>Heure : {appointment.time}</span>
        </div>
        <div className="flex items-center gap-2 text-sm text-muted-foreground">
          <MapPin className="w-4 h-4" />
          <span>Commentaire : {appointment.commentaire}</span>
        </div>
      </CardContent>
    </Card>
  );
}
