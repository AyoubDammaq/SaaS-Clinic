
import { Badge } from "@/components/ui/badge";
import { CheckCircle, AlertTriangle, Clock } from "lucide-react";
import { cn } from "@/lib/utils";
import { InvoiceStatus } from "@/types/billing";

interface InvoiceStatusBadgeProps {
  status: InvoiceStatus;
  size?: 'sm' | 'md' | 'lg';
}

export function InvoiceStatusBadge({ status, size = 'md' }: InvoiceStatusBadgeProps) {
  const getStatusConfig = () => {
    switch (status) {
      case 'Paid':
        return {
          variant: "bg-emerald-100 text-emerald-800 border-emerald-200",
          icon: <CheckCircle className={cn("mr-1", size === 'sm' ? "h-3 w-3" : "h-3.5 w-3.5")} aria-hidden="true" />,
          label: "Paid",
          ariaLabel: "Payment status: Paid"
        };
      case 'Pending':
        return {
          variant: "bg-amber-50 text-amber-600 border-amber-200",
          icon: <Clock className={cn("mr-1", size === 'sm' ? "h-3 w-3" : "h-3.5 w-3.5")} aria-hidden="true" />,
          label: "Pending",
          ariaLabel: "Payment status: Pending"
        };
      case 'Overdue':
        return {
          variant: "bg-red-100 text-red-800 border-red-200",
          icon: <AlertTriangle className={cn("mr-1", size === 'sm' ? "h-3 w-3" : "h-3.5 w-3.5")} aria-hidden="true" />,
          label: "Overdue",
          ariaLabel: "Payment status: Overdue"
        };
      default:
        return {
          variant: "",
          icon: null,
          label: status,
          ariaLabel: `Payment status: ${status}`
        };
    }
  };

  const { variant, icon, label, ariaLabel } = getStatusConfig();
  const sizeClass = {
    sm: "text-xs py-0.5 px-1.5",
    md: "text-sm py-0.5 px-2",
    lg: "py-1 px-2.5"
  }[size];

  return (
    <Badge 
      variant="outline" 
      className={cn(variant, "flex items-center font-medium transition-colors", sizeClass)}
      aria-label={ariaLabel}
    >
      {icon}
      <span>{label}</span>
    </Badge>
  );
}
