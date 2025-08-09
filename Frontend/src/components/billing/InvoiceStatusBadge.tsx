import { Badge } from "@/components/ui/badge";
import { CheckCircle, AlertTriangle, Clock } from "lucide-react";
import { cn } from "@/lib/utils";
import { FactureStatus } from "@/types/billing";
import { useTranslation } from "@/hooks/useTranslation";

interface InvoiceStatusBadgeProps {
  status: FactureStatus;
  size?: "sm" | "md" | "lg";
}

export function InvoiceStatusBadge({
  status,
  size = "md",
}: InvoiceStatusBadgeProps) {
  const { t } = useTranslation('billing');

  const getStatusConfig = () => {
    switch (status) {
      case FactureStatus.PAYEE:
        return {
          variant: "bg-emerald-100 text-emerald-800 border-emerald-200",
          icon: (
            <CheckCircle
              className={cn("mr-1", size === "sm" ? "h-3 w-3" : "h-3.5 w-3.5")}
              aria-hidden="true"
            />
          ),
          label: t('paid'),
          ariaLabel: `${t('paymentStatus')}: ${t('paid')}`,
        };
      case FactureStatus.IMPAYEE:
        return {
          variant: "bg-red-100 text-red-800 border-red-200",
          icon: (
            <AlertTriangle
              className={cn("mr-1", size === "sm" ? "h-3 w-3" : "h-3.5 w-3.5")}
              aria-hidden="true"
            />
          ),
          label: t('unpaid'),
          ariaLabel: `${t('paymentStatus')}: ${t('unpaid')}`,
        };
      case FactureStatus.PARTIELLEMENT_PAYEE:
        return {
          variant: "bg-amber-50 text-amber-600 border-amber-200",
          icon: (
            <Clock
              className={cn("mr-1", size === "sm" ? "h-3 w-3" : "h-3.5 w-3.5")}
              aria-hidden="true"
            />
          ),
          label: t('partiallyPaid'),
          ariaLabel: `${t('paymentStatus')}: ${t('partiallyPaid')}`,
        };
      case FactureStatus.ANNULEE:
        return {
          variant: "bg-gray-100 text-gray-600 border-gray-300",
          icon: null,
          label: t('cancelled'),
          ariaLabel: `${t('paymentStatus')}: ${t('cancelled')}`,
        };
      default:
        return {
          variant: "",
          icon: null,
          label: status,
          ariaLabel: `${t('paymentStatus')}: ${status}`,
        };
    }
  };

  const { variant, icon, label, ariaLabel } = getStatusConfig();
  const sizeClass = {
    sm: "text-xs py-0.5 px-1.5",
    md: "text-sm py-0.5 px-2",
    lg: "py-1 px-2.5",
  }[size];

  return (
    <Badge
      variant="outline"
      className={cn(
        variant,
        "flex items-center font-medium transition-colors",
        sizeClass
      )}
      aria-label={ariaLabel}
    >
      {icon}
      <span>{label}</span>
    </Badge>
  );
}