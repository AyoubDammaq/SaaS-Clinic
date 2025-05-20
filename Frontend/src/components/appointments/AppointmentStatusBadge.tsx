
import { Badge } from "@/components/ui/badge";
import { CheckCircle, Clock, XCircle, AlertTriangle } from "lucide-react";
import { cn } from "@/lib/utils";
import { useTranslation } from "@/hooks/useTranslation";

export type AppointmentStatus = 'Scheduled' | 'Completed' | 'Cancelled' | 'No-show';

interface AppointmentStatusBadgeProps {
  status: AppointmentStatus;
  showIcon?: boolean;
}

export const AppointmentStatusBadge = ({ status, showIcon = true }: AppointmentStatusBadgeProps) => {
  const { t } = useTranslation('appointments');
  
  const getStatusConfig = () => {
    switch (status) {
      case 'Scheduled':
        return {
          variant: "text-blue-600 bg-blue-50 border-blue-200",
          icon: <Clock className="h-3.5 w-3.5 mr-1" />,
          label: t('scheduled')
        };
      case 'Completed':
        return {
          variant: "text-green-600 bg-green-50 border-green-200",
          icon: <CheckCircle className="h-3.5 w-3.5 mr-1" />,
          label: t('completed')
        };
      case 'Cancelled':
        return {
          variant: "text-red-600 bg-red-50 border-red-200",
          icon: <XCircle className="h-3.5 w-3.5 mr-1" />,
          label: t('cancelled')
        };
      case 'No-show':
        return {
          variant: "text-amber-600 bg-amber-50 border-amber-200",
          icon: <AlertTriangle className="h-3.5 w-3.5 mr-1" />,
          label: t('noShow')
        };
      default:
        return {
          variant: "",
          icon: null,
          label: status
        };
    }
  };
  
  const { variant, icon, label } = getStatusConfig();
  
  return (
    <Badge variant="outline" className={cn(variant, "flex items-center font-medium")}>
      {showIcon && icon}
      <span>{label}</span>
    </Badge>
  );
};
