
import { useNavigate } from 'react-router-dom';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { Search, Plus, FileEdit, Trash2 } from 'lucide-react';
import { Card, CardContent } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Consultation } from '@/types/consultation';
import { format } from 'date-fns';

interface ConsultationsListProps {
  filteredConsultations: Consultation[];
  searchTerm: string;
  setSearchTerm: (term: string) => void;
  isLoading: boolean;
  permissions: {
    canCreate: boolean;
    canEdit: boolean;
    canDelete: boolean;
  };
  onAddConsultation: () => void;
  onEditConsultation: (consultation: Consultation) => void;
  onDeleteConsultation: (id: string) => void;
  onViewConsultationDetails: (consultation: Consultation) => void;
  doctors: { id: string; prenom: string; nom: string }[];
  patients: { id: string; prenom: string; nom: string }[];
}

export function ConsultationsList({
  filteredConsultations,
  searchTerm,
  setSearchTerm,
  isLoading,
  permissions,
  onAddConsultation,
  onEditConsultation,
  onDeleteConsultation,
  onViewConsultationDetails,
  doctors,
  patients,
}: ConsultationsListProps) {
  const getStatusBadge = (status: string) => {
    switch (status) {
      case 'Programmée':
        return <Badge variant="outline" className="bg-blue-50 text-blue-700 border-blue-200">Scheduled</Badge>;
      case 'Terminée':
        return <Badge variant="outline" className="bg-green-50 text-green-700 border-green-200">Completed</Badge>;
      case 'Annulée':
        return <Badge variant="outline" className="bg-red-50 text-red-700 border-red-200">Canceled</Badge>;
      default:
        return <Badge variant="outline">{status}</Badge>;
    }
  };

  const formatDate = (dateString: string) => {
    try {
      return format(new Date(dateString), 'PPpp'); // Ex: Jul 16, 2025 at 14:30
    } catch {
      return dateString;
    }
  };

  if (isLoading) {
    return (
      <Card>
        <CardContent className="pt-6">
          <div className="flex justify-center items-center h-40">
            <p className="text-muted-foreground">Loading consultations...</p>
          </div>
        </CardContent>
      </Card>
    );
  }

  return (
    <div>
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
        {permissions.canCreate && (
          <Button className="ml-2" onClick={onAddConsultation}>
            <Plus className="mr-1 h-4 w-4" /> Add Consultation
          </Button>
        )}
      </div>

      <div className="rounded-md border">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Date</TableHead>
              <TableHead>Patient</TableHead>
              <TableHead>Doctor</TableHead>
              <TableHead>Diagnostic</TableHead>
              <TableHead>Status</TableHead>
              {(permissions.canEdit || permissions.canDelete) && <TableHead>Actions</TableHead>}
            </TableRow>
          </TableHeader>
          <TableBody>
            {filteredConsultations.length === 0 ? (
              <TableRow>
                <TableCell colSpan={6} className="h-24 text-center">
                  No consultations found
                </TableCell>
              </TableRow>
            ) : (
              filteredConsultations.map((consultation) => {
                const patient = patients.find((p) => p.id === consultation.patientId);
                const doctor = doctors.find((d) => d.id === consultation.medecinId);

                return (
                  <TableRow
                    key={consultation.id}
                    className="cursor-pointer hover:bg-muted/50"
                    onClick={() => onViewConsultationDetails(consultation)}
                  >
                    <TableCell>{formatDate(consultation.dateConsultation)}</TableCell>
                    <TableCell>{patient ? `${patient.prenom} ${patient.nom}` : "N/A"}</TableCell>
                    <TableCell>{doctor ? `${doctor.prenom} ${doctor.nom}` : "N/A"}</TableCell>
                    <TableCell>{consultation.diagnostic || "—"}</TableCell>
                    {(permissions.canEdit || permissions.canDelete) && (
                      <TableCell onClick={(e) => e.stopPropagation()}>
                        <div className="flex items-center gap-2">
                          {permissions.canEdit && (
                            <Button
                              size="sm"
                              variant="ghost"
                              onClick={(e) => {
                                e.stopPropagation();
                                onEditConsultation(consultation);
                              }}
                            >
                              <FileEdit className="h-4 w-4" />
                            </Button>
                          )}
                          {permissions.canDelete && (
                            <Button
                              size="sm"
                              variant="ghost"
                              className="text-red-500"
                              onClick={(e) => {
                                e.stopPropagation();
                                onDeleteConsultation(consultation.id);
                              }}
                            >
                              <Trash2 className="h-4 w-4" />
                            </Button>
                          )}
                        </div>
                      </TableCell>
                    )}
                  </TableRow>
                );
              })
            )}
          </TableBody>
        </Table>
      </div>
    </div>
  );
}
