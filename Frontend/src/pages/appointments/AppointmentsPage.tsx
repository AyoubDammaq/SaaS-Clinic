import { useState } from 'react';
import { useAuth } from '@/contexts/AuthContext';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { AppointmentFilters } from '@/components/appointments/AppointmentFilters';
import { AppointmentTable } from '@/components/appointments/AppointmentTable';
import { AppointmentForm } from '@/components/appointments/AppointmentForm';
import { AppointmentStatus } from '@/components/appointments/AppointmentStatusBadge';
import { cn } from '@/lib/utils';
import { useTranslation } from '@/hooks/useTranslation';
import { format } from 'date-fns';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle, DialogFooter } from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { toast } from 'sonner';
import { ConsultationForm } from '@/components/consultations/ConsultationForm';

interface Appointment {
  id: string;
  patientName: string;
  patientId: string;
  doctorName: string;
  doctorId: string;
  date: string;
  time: string;
  duration: number; // minutes
  reason: string;
  status: AppointmentStatus;
  notes?: string;
}

// Mock data for doctors and patients
const mockDoctors = [
  { id: '1', name: 'Dr. Sarah Johnson', specialty: 'Cardiology' },
  { id: '2', name: 'Dr. James Wilson', specialty: 'Dermatology' },
  { id: '3', name: 'Dr. Emily Harris', specialty: 'Pediatrics' },
  { id: '4', name: 'Dr. Michael Brown', specialty: 'General Medicine' }
];

const mockPatients = [
  { id: '1', name: 'John Doe' },
  { id: '2', name: 'Jane Smith' },
  { id: '3', name: 'Robert Brown' },
  { id: '4', name: 'Emily Davis' },
  { id: '5', name: 'Michael Wilson' }
];

const mockAppointments: Appointment[] = [
  {
    id: '1',
    patientName: 'John Doe',
    patientId: '1',
    doctorName: 'Dr. Michael Brown',
    doctorId: '4',
    date: '2025-04-30',
    time: '09:00',
    duration: 30,
    reason: 'Annual check-up',
    status: 'Scheduled'
  },
  {
    id: '2',
    patientName: 'Jane Smith',
    patientId: '2',
    doctorName: 'Dr. Sarah Johnson',
    doctorId: '1',
    date: '2025-04-30',
    time: '10:30',
    duration: 45,
    reason: 'Follow-up appointment',
    status: 'Scheduled'
  },
  {
    id: '3',
    patientName: 'Robert Brown',
    patientId: '3',
    doctorName: 'Dr. Michael Brown',
    doctorId: '4',
    date: '2023-04-29',
    time: '14:15',
    duration: 30,
    reason: 'Consultation',
    status: 'Completed',
    notes: 'Patient is feeling better, prescribed medication refill.'
  },
  {
    id: '4',
    patientName: 'Emily Davis',
    patientId: '4',
    doctorName: 'Dr. James Wilson',
    doctorId: '2',
    date: '2025-05-02',
    time: '11:00',
    duration: 60,
    reason: 'Initial consultation',
    status: 'Scheduled'
  },
  {
    id: '5',
    patientName: 'Michael Wilson',
    patientId: '5',
    doctorName: 'Dr. Emily Harris',
    doctorId: '3',
    date: '2023-04-28',
    time: '15:30',
    duration: 30,
    reason: 'Check-up',
    status: 'Cancelled'
  },
  {
    id: '6',
    patientName: 'John Doe',
    patientId: '1',
    doctorName: 'Dr. Sarah Johnson',
    doctorId: '1',
    date: '2023-03-15',
    time: '09:45',
    duration: 30,
    reason: 'Prescription renewal',
    status: 'Completed'
  },
  {
    id: '7',
    patientName: 'Jane Smith',
    patientId: '2',
    doctorName: 'Dr. Emily Harris',
    doctorId: '3',
    date: '2023-04-10',
    time: '14:00',
    duration: 45,
    reason: 'Child vaccination',
    status: 'Completed'
  },
  {
    id: '8',
    patientName: 'Robert Brown',
    patientId: '3',
    doctorName: 'Dr. James Wilson',
    doctorId: '2',
    date: '2023-03-22',
    time: '11:30',
    duration: 60,
    reason: 'Skin condition evaluation',
    status: 'No-show'
  }
];

function AppointmentsPage() {
  const { user } = useAuth();
  const { t } = useTranslation('appointments');
  const tConsultations = useTranslation('consultations').t;
  const tCommon = useTranslation('common').t;
  
  // State for filters
  const [searchTerm, setSearchTerm] = useState('');
  const [dateFilter, setDateFilter] = useState<Date | null>(null);
  const [statusFilter, setStatusFilter] = useState<AppointmentStatus | 'all'>('all');
  
  // State for appointment form
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [selectedAppointment, setSelectedAppointment] = useState<Appointment | null>(null);
  
  // State for appointment details dialog
  const [isDetailsOpen, setIsDetailsOpen] = useState(false);
  const [viewingAppointment, setViewingAppointment] = useState<Appointment | null>(null);
  
  // State for cancel confirmation dialog
  const [isCancelDialogOpen, setIsCancelDialogOpen] = useState(false);
  const [appointmentToCancel, setAppointmentToCancel] = useState<string | null>(null);
  
  // State for consultation form
  const [isConsultationFormOpen, setIsConsultationFormOpen] = useState(false);
  const [appointmentForConsultation, setAppointmentForConsultation] = useState<Appointment | null>(null);

  // Filter appointments based on user role, search term, date and status
  const getFilteredAppointments = () => {
    if (!user) return { upcoming: [], past: [] };
    
    let filtered = [...mockAppointments];
    
    // Filter by role
    if (user.role === 'Patient') {
      filtered = filtered.filter(appointment => 
        appointment.patientId === user.id || 
        appointment.patientName.includes(user.name)
      );
    } else if (user.role === 'Doctor') {
      filtered = filtered.filter(appointment => 
        appointment.doctorId === user.id || 
        appointment.doctorName.includes(user.name)
      );
    }
    
    // Apply search term filter
    if (searchTerm) {
      filtered = filtered.filter(appointment => 
        appointment.patientName.toLowerCase().includes(searchTerm.toLowerCase()) ||
        appointment.doctorName.toLowerCase().includes(searchTerm.toLowerCase()) ||
        appointment.reason.toLowerCase().includes(searchTerm.toLowerCase())
      );
    }
    
    // Apply date filter
    if (dateFilter) {
      const formattedDateFilter = format(dateFilter, 'yyyy-MM-dd');
      filtered = filtered.filter(appointment => appointment.date === formattedDateFilter);
    }
    
    // Apply status filter
    if (statusFilter !== 'all') {
      filtered = filtered.filter(appointment => appointment.status === statusFilter);
    }
    
    // Split into upcoming and past appointments
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    
    const upcoming = filtered.filter(appointment => {
      const appointmentDate = new Date(appointment.date);
      return appointmentDate >= today || appointment.status === 'Scheduled';
    });
    
    const past = filtered.filter(appointment => {
      const appointmentDate = new Date(appointment.date);
      return appointmentDate < today && appointment.status !== 'Scheduled';
    });
    
    // Sort upcoming by date (ascending)
    upcoming.sort((a, b) => {
      const dateA = new Date(a.date + 'T' + a.time);
      const dateB = new Date(b.date + 'T' + b.time);
      return dateA.getTime() - dateB.getTime();
    });
    
    // Sort past by date (descending)
    past.sort((a, b) => {
      const dateA = new Date(a.date + 'T' + a.time);
      const dateB = new Date(b.date + 'T' + b.time);
      return dateB.getTime() - dateA.getTime();
    });
    
    return { upcoming, past };
  };

  const { upcoming, past } = getFilteredAppointments();

  // Handlers for appointment actions
  const handleCancel = (appointmentId: string) => {
    // Open the cancel confirmation dialog
    setAppointmentToCancel(appointmentId);
    setIsCancelDialogOpen(true);
  };
  
  const confirmCancelAppointment = () => {
    if (appointmentToCancel) {
      // Here would be the API call to cancel the appointment
      console.log(`Cancelling appointment ${appointmentToCancel}`);
      
      // Show success message
      toast.success(t('appointmentCancelled'));
      
      // Close the dialog
      setIsCancelDialogOpen(false);
      setAppointmentToCancel(null);
    }
  };

  const handleComplete = (appointmentId: string) => {
    // Here would be the API call to mark the appointment as completed
    console.log(`Completing appointment ${appointmentId}`);
    toast.success(t('appointmentCompleted'));
  };

  const handleReschedule = (appointment: Appointment) => {
    // Open form with the appointment data for rescheduling
    setSelectedAppointment(appointment);
    setIsFormOpen(true);
  };

  const handleViewDetails = (appointment: Appointment) => {
    // Open the details dialog
    setViewingAppointment(appointment);
    setIsDetailsOpen(true);
  };

  const handleAddNotes = (appointment: Appointment) => {
    // Here would be opening a notes modal for the doctor to add notes
    console.log(`Adding notes to appointment ${appointment.id}`);
  };

  const handleCreateAppointment = () => {
    setSelectedAppointment(null);
    setIsFormOpen(true);
  };

  const handleFormSubmit = (data: any) => {
    // Here would be the API call to create or update an appointment
    console.log("Submitting appointment data:", data);
    
    // Show success message
    if (selectedAppointment) {
      toast.success(t('appointmentUpdated'));
    } else {
      toast.success(t('appointmentCreated'));
    }
    
    // Close the form
    setIsFormOpen(false);
  };
  
  const handleCreateConsultation = (appointment: Appointment) => {
    // Only allow doctors to create consultations from appointments
    if (user?.role === 'Doctor' && appointment.status === 'Scheduled') {
      setAppointmentForConsultation(appointment);
      setIsConsultationFormOpen(true);
    }
  };
  
  const handleConsultationFormSubmit = (data: any) => {
    // Here would be the API call to create a consultation
    console.log("Creating consultation from appointment:", data);
    
    // Show success message
    toast.success("Consultation created successfully");
    
    // Close the form
    setIsConsultationFormOpen(false);
    setAppointmentForConsultation(null);
  };

  return (
    <div className="space-y-6 pb-8">
      <div className="flex flex-col gap-2">
        <h1 className="text-3xl font-bold tracking-tight">{t('appointments')}</h1>
        <p className="text-muted-foreground">
          {user?.role === 'Patient' 
            ? t('manageYourAppointments') 
            : user?.role === 'Doctor'
            ? t('managePatientAppointments')
            : t('manageAllAppointments')}
        </p>
      </div>

      <AppointmentFilters
        searchTerm={searchTerm}
        onSearchChange={setSearchTerm}
        dateFilter={dateFilter}
        onDateFilterChange={setDateFilter}
        statusFilter={statusFilter}
        onStatusFilterChange={setStatusFilter}
        onCreateAppointment={handleCreateAppointment}
        userRole={user?.role}
      />

      <div className="space-y-6">
        <Card>
          <CardHeader className="py-4">
            <CardTitle>{t('upcomingAppointments')}</CardTitle>
            <CardDescription>{tCommon('manage')} {t('upcomingAppointments').toLowerCase()}</CardDescription>
          </CardHeader>
          <CardContent>
            <AppointmentTable
              appointments={upcoming}
              userRole={user?.role || ''}
              onCancel={handleCancel}
              onComplete={handleComplete}
              onReschedule={handleReschedule}
              onViewDetails={handleViewDetails}
              onAddNotes={handleAddNotes}
              onRowClick={user?.role === 'Doctor' ? handleCreateConsultation : undefined}
            />
          </CardContent>
        </Card>

        <Card>
          <CardHeader className={cn("py-4", past.length === 0 && "border-b-0")}>
            <CardTitle>{t('pastAppointments')}</CardTitle>
            <CardDescription>{tCommon('view')} {t('pastAppointments').toLowerCase()}</CardDescription>
          </CardHeader>
          <CardContent>
            <AppointmentTable
              appointments={past}
              userRole={user?.role || ''}
              onViewDetails={handleViewDetails}
              isPast={true}
            />
          </CardContent>
        </Card>
      </div>

      {/* Appointment Form Dialog */}
      <AppointmentForm
        isOpen={isFormOpen}
        onClose={() => setIsFormOpen(false)}
        onSubmit={handleFormSubmit}
        initialData={selectedAppointment}
        doctors={mockDoctors}
        patients={mockPatients}
        patientId={user?.role === 'Patient' ? user.id : undefined}
      />
      
      {/* Appointment Details Dialog */}
      <Dialog open={isDetailsOpen} onOpenChange={setIsDetailsOpen}>
        <DialogContent className="sm:max-w-[500px]">
          <DialogHeader>
            <DialogTitle>{t('appointmentDetails')}</DialogTitle>
          </DialogHeader>
          {viewingAppointment && (
            <div className="space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <p className="text-sm font-medium text-muted-foreground">{tCommon('patient')}</p>
                  <p>{viewingAppointment.patientName}</p>
                </div>
                <div>
                  <p className="text-sm font-medium text-muted-foreground">{tCommon('doctor')}</p>
                  <p>{viewingAppointment.doctorName}</p>
                </div>
              </div>
              <div className="grid grid-cols-3 gap-4">
                <div>
                  <p className="text-sm font-medium text-muted-foreground">{t('date')}</p>
                  <p>{viewingAppointment.date}</p>
                </div>
                <div>
                  <p className="text-sm font-medium text-muted-foreground">{t('time')}</p>
                  <p>{viewingAppointment.time}</p>
                </div>
                <div>
                  <p className="text-sm font-medium text-muted-foreground">{t('durationMinutes')}</p>
                  <p>{viewingAppointment.duration} {t('min')}</p>
                </div>
              </div>
              <div>
                <p className="text-sm font-medium text-muted-foreground">{t('reason')}</p>
                <p>{viewingAppointment.reason}</p>
              </div>
              {viewingAppointment.notes && (
                <div>
                  <p className="text-sm font-medium text-muted-foreground">{t('additionalNotes')}</p>
                  <p>{viewingAppointment.notes}</p>
                </div>
              )}
              <DialogFooter>
                {viewingAppointment.status === 'Scheduled' && user?.role !== 'Patient' && (
                  <Button variant="outline" className="mr-auto" onClick={() => {
                    handleCreateConsultation(viewingAppointment);
                    setIsDetailsOpen(false);
                  }}>
                    {tConsultations('createConsultation')}
                  </Button>
                )}
                <Button variant="outline" onClick={() => setIsDetailsOpen(false)}>
                  {tCommon('close')}
                </Button>
              </DialogFooter>
            </div>
          )}
        </DialogContent>
      </Dialog>
      
      {/* Cancel Appointment Confirmation Dialog */}
      <Dialog open={isCancelDialogOpen} onOpenChange={setIsCancelDialogOpen}>
        <DialogContent className="sm:max-w-[425px]">
          <DialogHeader>
            <DialogTitle>{t('cancelAppointment')}</DialogTitle>
            <DialogDescription>
              {tCommon('confirmActionCannotBeUndone')}
            </DialogDescription>
          </DialogHeader>
          <DialogFooter>
            <Button variant="outline" onClick={() => setIsCancelDialogOpen(false)}>
              {tCommon('cancel')}
            </Button>
            <Button variant="destructive" onClick={confirmCancelAppointment}>
              {t('cancelAppointment')}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
      
      {/* Consultation Form from Appointment */}
      {appointmentForConsultation && (
        <ConsultationForm
          isOpen={isConsultationFormOpen}
          onClose={() => setIsConsultationFormOpen(false)}
          onSubmit={handleConsultationFormSubmit}
          initialData={{
            patientId: appointmentForConsultation.patientId,
            doctorId: appointmentForConsultation.doctorId,
            date: appointmentForConsultation.date,
            time: appointmentForConsultation.time,
            reason: `${tConsultations('consultationFromAppointment')}: ${appointmentForConsultation.reason}`
          }}
          patients={mockPatients}
          doctors={mockDoctors}
        />
      )}
    </div>
  );
}

export default AppointmentsPage;
