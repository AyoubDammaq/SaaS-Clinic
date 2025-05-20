
import { useState } from 'react';
import { useAuth } from '@/contexts/AuthContext';
import { 
  Table, TableBody, TableCell, TableHead, TableHeader, TableRow 
} from '@/components/ui/table';
import { 
  Card, CardContent, CardDescription, CardHeader, CardTitle 
} from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { 
  Search, Plus, FileEdit, FileText, User, Calendar, Clock, Trash2
} from 'lucide-react';
import { Badge } from '@/components/ui/badge';
import { Input } from '@/components/ui/input';
import { ConsultationForm } from '@/components/consultations/ConsultationForm';
import { toast } from 'sonner';

type ConsultationStatus = 'Completed' | 'Pending' | 'Cancelled';

interface Consultation {
  id: string;
  patientName: string;
  patientId: string;
  doctorName: string;
  doctorId: string;
  date: string;
  time: string;
  duration: number;
  reason: string;
  status: ConsultationStatus;
  notes?: string;
  prescription?: string;
}

interface Patient {
  id: string;
  name: string;
}

interface Doctor {
  id: string;
  name: string;
  specialty: string;
}

const mockPatients: Patient[] = [
  { id: '1', name: 'John Doe' },
  { id: '2', name: 'Jane Smith' },
  { id: '3', name: 'Robert Brown' },
  { id: '4', name: 'Emily Davis' },
  { id: '5', name: 'Michael Wilson' },
];

const mockDoctors: Doctor[] = [
  { id: '1', name: 'Dr. Sarah Johnson', specialty: 'Cardiology' },
  { id: '2', name: 'Dr. James Wilson', specialty: 'Neurology' },
  { id: '3', name: 'Dr. Emily Harris', specialty: 'Pediatrics' },
  { id: '4', name: 'Dr. Michael Brown', specialty: 'General Practice' },
  { id: '5', name: 'Dr. Lisa Chen', specialty: 'Dermatology' },
];

const mockConsultations: Consultation[] = [
  {
    id: '1',
    patientName: 'John Doe',
    patientId: '1',
    doctorName: 'Dr. Michael Brown',
    doctorId: '4',
    date: '2025-04-28',
    time: '09:15',
    duration: 30,
    reason: 'Regular check-up',
    status: 'Completed',
    notes: 'Patient presented with mild symptoms. Blood pressure normal.',
    prescription: 'Vitamin D supplements, once daily'
  },
  {
    id: '2',
    patientName: 'Jane Smith',
    patientId: '2',
    doctorName: 'Dr. Sarah Johnson',
    doctorId: '1',
    date: '2025-04-29',
    time: '14:30',
    duration: 45,
    reason: 'Follow-up consultation',
    status: 'Completed',
    notes: 'Patient recovering well from procedure. No complications.',
    prescription: 'Continue with current medication'
  },
  {
    id: '3',
    patientName: 'Robert Brown',
    patientId: '3',
    doctorName: 'Dr. James Wilson',
    doctorId: '2',
    date: '2025-04-30',
    time: '11:00',
    duration: 30,
    reason: 'Diagnosis review',
    status: 'Pending'
  },
  {
    id: '4',
    patientName: 'Emily Davis',
    patientId: '4',
    doctorName: 'Dr. Lisa Chen',
    doctorId: '5',
    date: '2025-05-02',
    time: '10:15',
    duration: 60,
    reason: 'New patient consultation',
    status: 'Pending'
  },
  {
    id: '5',
    patientName: 'Michael Wilson',
    patientId: '5',
    doctorName: 'Dr. Emily Harris',
    doctorId: '3',
    date: '2025-04-27',
    time: '16:00',
    duration: 30,
    reason: 'Treatment discussion',
    status: 'Cancelled'
  }
];

function ConsultationsPage() {
  const { user } = useAuth();
  const [searchTerm, setSearchTerm] = useState('');
  const [consultations, setConsultations] = useState<Consultation[]>(mockConsultations);
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [currentConsultation, setCurrentConsultation] = useState<Consultation | null>(null);
  
  // Filter consultations based on user role and search term
  const getFilteredConsultations = () => {
    if (!user) return [];
    
    let filtered = [...consultations];
    
    // Filter by role
    if (user.role === 'Patient') {
      filtered = filtered.filter(consultation => 
        consultation.patientId === user.id || 
        consultation.patientName.includes(user.name)
      );
    } else if (user.role === 'Doctor') {
      filtered = filtered.filter(consultation => 
        consultation.doctorId === user.id || 
        consultation.doctorName.includes(user.name)
      );
    }
    
    // Apply search term
    if (searchTerm) {
      filtered = filtered.filter(consultation => 
        consultation.patientName.toLowerCase().includes(searchTerm.toLowerCase()) ||
        consultation.doctorName.toLowerCase().includes(searchTerm.toLowerCase()) ||
        consultation.reason.toLowerCase().includes(searchTerm.toLowerCase())
      );
    }
    
    return filtered;
  };

  const handleCreateConsultation = () => {
    setCurrentConsultation(null);
    setIsFormOpen(true);
  };

  const handleEditConsultation = (consultation: Consultation) => {
    setCurrentConsultation(consultation);
    setIsFormOpen(true);
  };

  const handleDeleteConsultation = (id: string) => {
    if (window.confirm('Are you sure you want to delete this consultation?')) {
      setConsultations(consultations.filter(c => c.id !== id));
      toast.success('Consultation deleted successfully');
    }
  };

  const handleSubmitConsultation = (data: any) => {
    if (currentConsultation) {
      // Edit existing consultation
      const updatedConsultations = consultations.map(c => 
        c.id === currentConsultation.id ? {
          ...c,
          patientId: data.patientId,
          patientName: mockPatients.find(p => p.id === data.patientId)?.name || '',
          doctorId: data.doctorId,
          doctorName: mockDoctors.find(d => d.id === data.doctorId)?.name || '',
          date: data.date,
          time: data.time,
          reason: data.reason,
          notes: data.notes || c.notes,
        } : c
      );
      setConsultations(updatedConsultations);
    } else {
      // Create new consultation
      const newConsultation: Consultation = {
        id: (consultations.length + 1).toString(),
        patientId: data.patientId,
        patientName: mockPatients.find(p => p.id === data.patientId)?.name || '',
        doctorId: data.doctorId,
        doctorName: mockDoctors.find(d => d.id === data.doctorId)?.name || '',
        date: data.date,
        time: data.time,
        duration: 30, // Default duration
        reason: data.reason,
        status: 'Pending',
        notes: data.notes
      };
      setConsultations([...consultations, newConsultation]);
    }
  };

  const filteredConsultations = getFilteredConsultations();

  const renderStatusBadge = (status: ConsultationStatus) => {
    let variant = '';
    switch (status) {
      case 'Completed':
        variant = 'text-green-600 bg-green-50 border-green-200';
        break;
      case 'Pending':
        variant = 'text-blue-600 bg-blue-50 border-blue-200';
        break;
      case 'Cancelled':
        variant = 'text-red-600 bg-red-50 border-red-200';
        break;
      default:
        variant = '';
    }
    
    return (
      <Badge variant="outline" className={variant}>
        {status}
      </Badge>
    );
  };

  return (
    <div className="space-y-6 pb-8">
      <div className="flex flex-col gap-2">
        <h1 className="text-3xl font-bold tracking-tight">Consultations</h1>
        <p className="text-muted-foreground">
          {user?.role === 'Patient' 
            ? 'View your consultation history and medical notes'
            : user?.role === 'Doctor'
            ? 'Manage patient consultations and medical records'
            : 'Oversee clinic consultations and medical history'}
        </p>
      </div>

      <div className="flex items-center justify-between mb-4">
        <div className="relative w-full max-w-sm">
          <Search className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input
            placeholder="Search consultations..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="pl-8"
          />
        </div>
        {(user?.role === 'Doctor' || user?.role === 'ClinicAdmin') && (
          <Button className="ml-2" onClick={handleCreateConsultation}>
            <Plus className="mr-1 h-4 w-4" /> 
            {user?.role === 'Doctor' ? 'New Consultation' : 'Schedule Consultation'}
          </Button>
        )}
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Medical Consultations</CardTitle>
          <CardDescription>
            {user?.role === 'Patient' 
              ? 'Your consultation history with doctors'
              : 'Patient consultation records'}
          </CardDescription>
        </CardHeader>
        <CardContent>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Date & Time</TableHead>
                {user?.role !== 'Patient' && <TableHead>Patient</TableHead>}
                {user?.role !== 'Doctor' && <TableHead>Doctor</TableHead>}
                <TableHead>Reason</TableHead>
                <TableHead>Status</TableHead>
                <TableHead>Actions</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {filteredConsultations.map((consultation) => (
                <TableRow key={consultation.id}>
                  <TableCell>
                    <div className="flex flex-col">
                      <div className="flex items-center">
                        <Calendar className="mr-1 h-3.5 w-3.5 text-muted-foreground" />
                        <span>{consultation.date}</span>
                      </div>
                      <div className="flex items-center text-muted-foreground">
                        <Clock className="mr-1 h-3.5 w-3.5" />
                        <span>{consultation.time} ({consultation.duration} min)</span>
                      </div>
                    </div>
                  </TableCell>
                  {user?.role !== 'Patient' && (
                    <TableCell className="font-medium">
                      <div className="flex items-center gap-2">
                        <User className="h-4 w-4 text-muted-foreground" />
                        {consultation.patientName}
                      </div>
                    </TableCell>
                  )}
                  {user?.role !== 'Doctor' && (
                    <TableCell>{consultation.doctorName}</TableCell>
                  )}
                  <TableCell>{consultation.reason}</TableCell>
                  <TableCell>
                    {renderStatusBadge(consultation.status)}
                  </TableCell>
                  <TableCell>
                    <div className="flex gap-2">
                      {user?.role === 'Doctor' && consultation.status === 'Pending' && (
                        <Button size="sm" variant="outline">
                          <Plus className="mr-1 h-4 w-4" /> Add Notes
                        </Button>
                      )}
                      {consultation.status === 'Completed' && (
                        <Button size="sm" variant="outline">
                          <FileText className="mr-1 h-4 w-4" /> View Notes
                        </Button>
                      )}
                      {(user?.role === 'ClinicAdmin' || user?.role === 'SuperAdmin') && (
                        <>
                          <Button 
                            size="sm" 
                            variant="ghost" 
                            onClick={() => handleEditConsultation(consultation)}
                          >
                            <FileEdit className="h-4 w-4" />
                          </Button>
                          <Button 
                            size="sm" 
                            variant="ghost" 
                            className="text-red-500 hover:text-red-700"
                            onClick={() => handleDeleteConsultation(consultation.id)}
                          >
                            <Trash2 className="h-4 w-4" />
                          </Button>
                        </>
                      )}
                    </div>
                  </TableCell>
                </TableRow>
              ))}
              {filteredConsultations.length === 0 && (
                <TableRow>
                  <TableCell colSpan={user?.role === 'Patient' || user?.role === 'Doctor' ? 5 : 6} className="text-center py-8 text-muted-foreground">
                    No consultations found
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </CardContent>
      </Card>

      <ConsultationForm
        isOpen={isFormOpen}
        onClose={() => setIsFormOpen(false)}
        onSubmit={handleSubmitConsultation}
        initialData={currentConsultation ? {
          patientId: currentConsultation.patientId,
          doctorId: currentConsultation.doctorId,
          date: currentConsultation.date,
          time: currentConsultation.time,
          reason: currentConsultation.reason,
          notes: currentConsultation.notes
        } : undefined}
        patients={mockPatients}
        doctors={mockDoctors}
      />
    </div>
  );
}

export default ConsultationsPage;
