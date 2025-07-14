
import { useState } from "react";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { CalendarDays, Clock } from "lucide-react";
import { Appointment, AppointmentActions } from "./AppointmentActions";
import { AppointmentStatusBadge } from "./AppointmentStatusBadge";
import { 
  Pagination,
  PaginationContent,
  PaginationItem,
  PaginationNext,
  PaginationPrevious
} from "@/components/ui/pagination";
import { useTranslation } from "@/hooks/useTranslation";
import { cn } from "@/lib/utils";

interface AppointmentTableProps {
  appointments: Appointment[];
  userRole: string;
  onCancel?: (appointmentId: string) => void;
  onCancelByDoctor?: (appointment: Appointment) => void;
  onComplete?: (appointmentId: string) => void;
  onReschedule?: (appointment: Appointment) => void;
  onViewDetails?: (appointment: Appointment) => void;
  onAddNotes?: (appointment: Appointment) => void;
  onRowClick?: (appointment: Appointment) => void;
  isPast?: boolean;
  onConfirm?: (appointmentId: string) => void;
}

const ITEMS_PER_PAGE = 5;

export const AppointmentTable = ({
  appointments,
  userRole,
  onCancel,
  onCancelByDoctor,
  onComplete,
  onReschedule,
  onViewDetails,
  onAddNotes,
  onRowClick,
  isPast = false,
  onConfirm,
}: AppointmentTableProps) => {
  const { t } = useTranslation('appointments');
  const tCommon = useTranslation('common').t;
  const [currentPage, setCurrentPage] = useState(1);
  const [hoveredRow, setHoveredRow] = useState<string | null>(null);
  
  // Calculate pagination
  const totalPages = Math.ceil(appointments.length / ITEMS_PER_PAGE);
  const startIndex = (currentPage - 1) * ITEMS_PER_PAGE;
  const paginatedAppointments = appointments.slice(
    startIndex,
    startIndex + ITEMS_PER_PAGE
  );
  
  // Handle row click
  const handleRowClick = (appointment: Appointment) => {
    
    if (onRowClick) {
      onRowClick(appointment);
    } else if (onViewDetails) {
      onViewDetails(appointment);
    }
  };

  return (
    <div className="space-y-4">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>{t('dateTime')}</TableHead>
            {userRole !== 'Patient' && <TableHead>{tCommon('patient')}</TableHead>}
            {userRole !== 'Doctor' && <TableHead>{tCommon('doctor')}</TableHead>}
            <TableHead>{t('reason')}</TableHead>
            <TableHead>{tCommon('status')}</TableHead>
            <TableHead>{tCommon('Actions')}</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {paginatedAppointments.length > 0 ? (
            paginatedAppointments.map((appointment) => (
              <TableRow 
                key={appointment.id}
                className={cn(
                  "cursor-pointer transition-colors", 
                  hoveredRow === appointment.id && "bg-muted"
                )}
                onMouseEnter={() => setHoveredRow(appointment.id)}
                onMouseLeave={() => setHoveredRow(null)}
                onClick={() => handleRowClick(appointment)}
              >
                <TableCell>
                  <div className="flex flex-col">
                    <div className="flex items-center">
                      <CalendarDays className="mr-1 h-3.5 w-3.5 text-muted-foreground" />
                      <span>{appointment.dateHeure}</span>
                    </div>
                    <div className="flex items-center text-muted-foreground">
                      <Clock className="mr-1 h-3.5 w-3.5" />
                      <span>{appointment.time} ({appointment.duration} {t('min')})</span>
                    </div>
                  </div>
                </TableCell>
                {userRole !== 'Patient' && (
                  <TableCell className="font-medium">{appointment.patientName}</TableCell>
                )}
                {userRole !== 'Doctor' && (
                  <TableCell>{appointment.doctorName}</TableCell>
                )}
                <TableCell>{appointment.reason}</TableCell>
                <TableCell>
                  <AppointmentStatusBadge status={appointment.status} />
                </TableCell>
                <TableCell onClick={(e) => e.stopPropagation()}>
                  <AppointmentActions
                    appointment={appointment}
                    userRole={userRole}
                    onCancel={onCancel}
                    onComplete={onComplete}
                    onReschedule={onReschedule}
                    onViewDetails={onViewDetails}
                    onAddNotes={onAddNotes}
                    onConfirm={onConfirm}
                    onCancelByDoctor= {onCancelByDoctor}
                  />
                </TableCell>
              </TableRow>
            ))
          ) : (
            <TableRow>
              <TableCell 
                colSpan={userRole === 'Patient' || userRole === 'Doctor' ? 5 : 6} 
                className="text-center py-8 text-muted-foreground"
              >
                {isPast ? t('noPastAppointments') : t('noUpcomingAppointments')}
              </TableCell>
            </TableRow>
          )}
        </TableBody>
      </Table>
      
      {/* Pagination */}
      {totalPages > 1 && (
        <Pagination>
          <PaginationContent>
            <PaginationItem>
              <PaginationPrevious 
                onClick={() => setCurrentPage(prev => Math.max(prev - 1, 1))}
                className={cn(currentPage <= 1 && "pointer-events-none opacity-50")}
              />
            </PaginationItem>
            <PaginationItem className="flex items-center text-sm">
              {t('page')} {currentPage} {t('of')} {totalPages}
            </PaginationItem>
            <PaginationItem>
              <PaginationNext
                onClick={() => setCurrentPage(prev => Math.min(prev + 1, totalPages))}
                className={cn(currentPage >= totalPages && "pointer-events-none opacity-50")}
              />
            </PaginationItem>
          </PaginationContent>
        </Pagination>
      )}
    </div>
  );
};
