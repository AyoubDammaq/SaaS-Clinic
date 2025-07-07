import { Button } from "@/components/ui/button";
import {
  FileEdit,
  XCircle,
  CheckCircle,
  Eye,
  CalendarPlus,
} from "lucide-react";
import { AppointmentStatus } from "./AppointmentStatusBadge";
import { useTranslation } from "@/hooks/useTranslation";

export interface Appointment {
  id: string;
  patientName: string;
  patientId: string;
  doctorName: string;
  doctorId: string;
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
  onComplete?: (appointmentId: string) => void;
  onReschedule?: (appointment: Appointment) => void;
  onViewDetails?: (appointment: Appointment) => void;
  onAddNotes?: (appointment: Appointment) => void;
}

export const AppointmentActions = ({
  appointment,
  userRole,
  onCancel,
  onComplete,
  onReschedule,
  onViewDetails,
  onAddNotes,
}: AppointmentActionsProps) => {
  const { t } = useTranslation("appointments");
  const tCommon = useTranslation("common").t;
  const canEdit =
    appointment.status === "CONFIRME" || appointment.status === "EN_ATTENTE";

  if (userRole === "Patient") {
    return (
      <div className="flex gap-2">
        {canEdit && appointment.status !== "ANNULE" && (
          <Button
            size="sm"
            variant="ghost"
            className="text-red-500 hover:text-red-600 hover:bg-red-50"
            onClick={() => onCancel && onCancel(appointment.id)}
          >
            <XCircle className="h-4 w-4 mr-1" /> {tCommon("cancel")}
          </Button>
        )}
        <Button
          size="sm"
          variant="ghost"
          className="hover:bg-blue-50 hover:text-blue-600"
          onClick={() =>
            appointment.status === "CONFIRME"
              ? onReschedule && onReschedule(appointment)
              : onViewDetails && onViewDetails(appointment)
          }
        >
          {appointment.status === "CONFIRME" ? (
            <CalendarPlus className="h-4 w-4 mr-1" />
          ) : (
            <Eye className="h-4 w-4 mr-1" />
          )}
          {appointment.status === "CONFIRME"
            ? t("reschedule")
            : tCommon("view")}
        </Button>
      </div>
    );
  }

  if (userRole === "Doctor") {
    return (
      <div className="flex gap-2">
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
        <Button
          size="sm"
          variant="ghost"
          className="hover:bg-blue-50 hover:text-blue-600"
          onClick={() => onViewDetails && onViewDetails(appointment)}
        >
          <Eye className="h-4 w-4 mr-1" /> {tCommon("view")}
        </Button>
      </div>
    );
  }

  return (
    <div className="flex gap-2">
      <Button
        size="sm"
        variant="ghost"
        className="hover:bg-blue-50 hover:text-blue-600"
        onClick={() => onViewDetails && onViewDetails(appointment)}
      >
        <Eye className="h-4 w-4 mr-1" /> {tCommon("view")}
      </Button>
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
