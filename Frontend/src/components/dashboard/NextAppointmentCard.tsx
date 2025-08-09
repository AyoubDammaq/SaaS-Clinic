import { CalendarClock, User, MapPin } from "lucide-react";
import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
} from "@/components/ui/card";
import { useTranslation } from "@/hooks/useTranslation";

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
  const { t } = useTranslation("dashboard");
  if (!appointment) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>
            {t("next_appointment_title") || "Next Appointment"}
          </CardTitle>
          <CardDescription>
            {t("no_upcoming_appointment") || "No upcoming appointment"}
          </CardDescription>
        </CardHeader>
      </Card>
    );
  }

  return (
    <Card>
      <CardHeader>
        <CardTitle>{t("next_appointment_title") || "Next Appointment"}</CardTitle>
        <CardDescription>
          {t("appointment_date_time", { values: { date: appointment.date, time: appointment.time } }) ||
            `${appointment.date} at ${appointment.time}`}
        </CardDescription>
      </CardHeader>
      <CardContent className="space-y-3">
        <div className="flex items-center gap-2 text-sm text-muted-foreground">
          <User className="w-4 h-4" />
          <span>
            {t("appointment_patient_label", { values: { patientName: appointment.patientName } }) ||
              `Patient: ${appointment.patientName}`}
          </span>
        </div>
        <div className="flex items-center gap-2 text-sm text-muted-foreground">
          <CalendarClock className="w-4 h-4" />
          <span>
            {t("appointment_time_label", { values: { time: appointment.time } }) ||
              `Time: ${appointment.time}`}
          </span>
        </div>
        <div className="flex items-center gap-2 text-sm text-muted-foreground">
          <MapPin className="w-4 h-4" />
          <span>
            {t("appointment_comment_label", { values: { commentaire: appointment.commentaire } }) ||
              `Comment: ${appointment.commentaire}`}
          </span>
        </div>
      </CardContent>
    </Card>
  );
}
