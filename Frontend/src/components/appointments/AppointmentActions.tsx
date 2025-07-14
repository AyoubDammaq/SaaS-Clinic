import { Button } from "@/components/ui/button";
import { XCircle, CheckCircle, CalendarPlus } from "lucide-react";
import { AppointmentStatus } from "./AppointmentStatusBadge";
import { useTranslation } from "@/hooks/useTranslation";

export interface Appointment {
  id: string;
  patientName: string;
  patientId: string;
  doctorName: string;
  medecinId: string;
  dateHeure: string;
  time: string;
  duration: number;
  reason: string;
  status: AppointmentStatus;
  notes?: string;
}

interface AppointmentActionsProps {
  appointment: Appointment;
  userRole: string;
  onCancel?: (appointmentId: string) => void;
  onCancelByDoctor?: (appointment: Appointment) => void;
  onComplete?: (appointmentId: string) => void;
  onReschedule?: (appointment: Appointment) => void;
  onViewDetails?: (appointment: Appointment) => void;
  onAddNotes?: (appointment: Appointment) => void;
  onConfirm?: (appointmentId: string) => void;
}

export const AppointmentActions = ({
  appointment,
  userRole,
  onCancel,
  onCancelByDoctor,
  onComplete,
  onReschedule,
  onViewDetails,
  onAddNotes,
  onConfirm,
}: AppointmentActionsProps) => {
  const { t } = useTranslation("appointments");
  const tCommon = useTranslation("common").t;
  const canEdit = appointment.status === "EN_ATTENTE";

  if (userRole === "Patient") {
    return (
      <div className="flex gap-2">
        {appointment.status !== "ANNULE" && (
          <Button
            size="sm"
            variant="ghost"
            className="text-red-500 hover:text-red-600 hover:bg-red-50"
            onClick={() => onCancel && onCancel(appointment.id)}
          >
            <XCircle className="h-4 w-4 mr-1" /> {tCommon("cancel")}
          </Button>
        )}
        {canEdit && (
          <Button
            size="sm"
            variant="ghost"
            className="hover:bg-blue-50 hover:text-blue-600"
            onClick={() => onReschedule && onReschedule(appointment)}
          >
            <CalendarPlus className="h-4 w-4 mr-1" />
            {t("reschedule")}
          </Button>
        )}
      </div>
    );
  }

  if (userRole === "Doctor") {
    return (
      <div className="flex gap-2">
        {appointment.status === "EN_ATTENTE" && onConfirm && (
          <Button
            size="sm"
            variant="ghost"
            className="text-blue-500 hover:text-blue-600 hover:bg-blue-50"
            onClick={() => onConfirm(appointment.id)}
          >
            <CheckCircle className="h-4 w-4 mr-1" /> {t("confirm")}
          </Button>
        )}
        {appointment.status === "CONFIRME" && (
          <Button
            size="sm"
            variant="ghost"
            className="text-green-500 hover:text-green-600 hover:bg-green-50"
            onClick={() => onComplete && onComplete(appointment.id)}
          >
            <CheckCircle className="h-4 w-4 mr-1" /> {t("complete")}
          </Button>
        )}
        {appointment.status !== "ANNULE" && (
          <Button
            size="sm"
            variant="ghost"
            className="text-red-500 hover:text-red-600 hover:bg-red-50"
            onClick={() => onCancelByDoctor && onCancelByDoctor(appointment)}
          >
            <XCircle className="h-4 w-4 mr-1" /> {tCommon("cancel")}
          </Button>
        )}
      </div>
    );
  }

  return (
    <div className="flex gap-2">
      {canEdit && (
        <Button
          size="sm"
          variant="ghost"
          className="text-red-500 hover:text-red-600 hover:bg-red-50"
          onClick={() => onCancel && onCancel(appointment.id)}
        >
          <XCircle className="h-4 w-4 mr-1" /> {tCommon("cancel")}
        </Button>
      )}
    </div>
  );
};
