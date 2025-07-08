
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { Search, Plus, FileEdit, Trash2 } from 'lucide-react';
import { Card, CardContent } from '@/components/ui/card';
import { Doctor } from '@/types/doctor';
import { useAuth } from "@/hooks/useAuth";

interface DoctorsListProps {
  doctors: Doctor[];
  filteredDoctors: Doctor[];
  searchTerm: string;
  setSearchTerm: (term: string) => void;
  isLoading: boolean;
  permissions: {
    canCreate: boolean;
    canEdit: boolean;
    canDelete: boolean;
  };
  onAddDoctor: () => void;
  onEditDoctor: (doctor: Doctor) => void;
  onDeleteDoctor: (id: string) => void;
}

export function DoctorsList({
  doctors,
  filteredDoctors,
  searchTerm,
  setSearchTerm,
  isLoading,
  permissions,
  onAddDoctor,
  onEditDoctor,
  onDeleteDoctor
}: DoctorsListProps) {
  const navigate = useNavigate();
  const { user } = useAuth();

  const handleRowClick = (doctor: Doctor) => {
    navigate(`/doctors/${doctor.id}`);
  };

  if (isLoading) {
    return (
      <Card>
        <CardContent className="pt-6">
          <div className="flex justify-center items-center h-40">
            <p className="text-muted-foreground">Loading doctors...</p>
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
            placeholder="Search doctors..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="pl-8"
          />
        </div>
        {permissions.canCreate && (
          <Button className="ml-2" onClick={onAddDoctor}>
            <Plus className="mr-1 h-4 w-4" /> Add Doctor
          </Button>
        )}
      </div>
      
      <div className="rounded-md border">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Name</TableHead>
              <TableHead>Specialty</TableHead>
              <TableHead>Email</TableHead>
              <TableHead>Phone</TableHead>
              <TableHead>Clinic</TableHead>
              {(permissions.canEdit || permissions.canDelete) && <TableHead>Actions</TableHead>}
            </TableRow>
          </TableHeader>
          <TableBody>
            {filteredDoctors.length === 0 ? (
              <TableRow>
                <TableCell colSpan={6} className="h-24 text-center">
                  No doctors found
                </TableCell>
              </TableRow>
            ) : (
              filteredDoctors.map((doctor) => (
                <TableRow 
                  key={doctor.id} 
                  className="cursor-pointer hover:bg-muted/60"
                  onClick={() => handleRowClick(doctor)}
                >
                  <TableCell className="font-medium">{doctor.prenom} {doctor.nom}</TableCell>
                  <TableCell>{doctor.specialite}</TableCell>
                  <TableCell>{doctor.email}</TableCell>
                  <TableCell>{doctor.telephone}</TableCell>
                  <TableCell>{doctor.cliniqueId ? 'Assigned' : 'Not assigned'}</TableCell>
                  {(permissions.canEdit || permissions.canDelete) && (
                    <TableCell onClick={(e) => e.stopPropagation()}>
                      <div className="flex items-center gap-2">
                        {permissions.canEdit && (
                          <Button size="sm" variant="ghost" onClick={(e) => {
                            e.stopPropagation();
                            onEditDoctor(doctor);
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
                              onDeleteDoctor(doctor.id);
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
