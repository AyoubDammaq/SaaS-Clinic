import { Badge } from "@/components/ui/badge";
import { CheckCircle, Clock, XCircle } from "lucide-react";
import { cn } from "@/lib/utils";
import { useTranslation } from "@/hooks/useTranslation";

export type AppointmentStatus = "CONFIRME" | "EN_ATTENTE" | "ANNULE";

interface AppointmentStatusBadgeProps {
  status: AppointmentStatus;
  showIcon?: boolean;
}

const statusConfig: Record<AppointmentStatus, {
  labelKey: string;
  icon: JSX.Element;
  className: string;
}> = {
  CONFIRME: {
    labelKey: "scheduled",
    icon: <CheckCircle className="h-3.5 w-3.5 mr-1" />,
    className: "text-green-600 bg-green-50 border-green-200",
  },
  EN_ATTENTE: {
    labelKey: "pending",
    icon: <Clock className="h-3.5 w-3.5 mr-1" />,
    className: "text-blue-600 bg-blue-50 border-blue-200",
  },
  ANNULE: {
    labelKey: "cancelled",
    icon: <XCircle className="h-3.5 w-3.5 mr-1" />,
    className: "text-red-600 bg-red-50 border-red-200",
  },
};

export const AppointmentStatusBadge = ({
  status,
  showIcon = true,
}: AppointmentStatusBadgeProps) => {
  const { t } = useTranslation("appointments");

  const config = statusConfig[status];

  return (
    <Badge
      variant="outline"
      className={cn(config.className, "flex items-center font-medium")}
    >
      {showIcon && config.icon}
      <span>{t(config.labelKey)}</span>
    </Badge>
  );
};