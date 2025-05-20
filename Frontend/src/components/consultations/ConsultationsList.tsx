
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
  consultations: Consultation[];
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
}

export function ConsultationsList({
  consultations,
  filteredConsultations,
  searchTerm,
  setSearchTerm,
  isLoading,
  permissions,
  onAddConsultation,
  onEditConsultation,
  onDeleteConsultation
}: ConsultationsListProps) {
  const navigate = useNavigate();

  const handleRowClick = (consultation: Consultation) => {
    navigate(`/consultations/${consultation.id}`);
  };

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
      return format(new Date(dateString), 'PPP');
    } catch (error) {
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
              <TableHead>Time</TableHead>
              <TableHead>Patient</TableHead>
              <TableHead>Doctor</TableHead>
              <TableHead>Reason</TableHead>
              <TableHead>Status</TableHead>
              {(permissions.canEdit || permissions.canDelete) && <TableHead>Actions</TableHead>}
            </TableRow>
          </TableHeader>
          <TableBody>
            {filteredConsultations.length === 0 ? (
              <TableRow>
                <TableCell colSpan={7} className="h-24 text-center">
                  No consultations found
                </TableCell>
              </TableRow>
            ) : (
              filteredConsultations.map((consultation) => (
                <TableRow 
                  key={consultation.id} 
                  className="cursor-pointer hover:bg-muted/60"
                  onClick={() => handleRowClick(consultation)}
                >
                  <TableCell>{formatDate(consultation.dateConsultation)}</TableCell>
                  <TableCell>{consultation.heureDebut} - {consultation.heureFin}</TableCell>
                  <TableCell className="font-medium">{consultation.patientId}</TableCell>
                  <TableCell>{consultation.medecinId}</TableCell>
                  <TableCell>{consultation.raison}</TableCell>
                  <TableCell>{getStatusBadge(consultation.statut)}</TableCell>
                  {(permissions.canEdit || permissions.canDelete) && (
                    <TableCell onClick={(e) => e.stopPropagation()}>
                      <div className="flex items-center gap-2">
                        {permissions.canEdit && (
                          <Button size="sm" variant="ghost" onClick={(e) => {
                            e.stopPropagation();
                            onEditConsultation(consultation);
                          }}>
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
              ))
            )}
          </TableBody>
        </Table>
      </div>
    </div>
  );
}
