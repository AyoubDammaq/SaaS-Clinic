
import { useState } from 'react';
import { Button } from '@/components/ui/button';
import { 
  Card, 
  CardContent 
} from '@/components/ui/card';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Label } from '@/components/ui/label';
import { Filter, X } from 'lucide-react';

interface DoctorFiltersProps {
  specialties: string[];
  clinics: { id: string; name: string }[];
  onFilterChange: (filters: {
    specialty: string | null;
    clinicId: string | null;
  }) => void;
}

export function DoctorFilters({ specialties, clinics, onFilterChange }: DoctorFiltersProps) {
  const [showFilters, setShowFilters] = useState(false);
  const [specialty, setSpecialty] = useState<string | null>(null);
  const [clinicId, setClinicId] = useState<string | null>(null);

  const handleSpecialtyChange = (value: string) => {
    const newValue = value === "all_specialties" ? null : value;
    setSpecialty(newValue);
    onFilterChange({ specialty: newValue, clinicId });
  };

  const handleClinicChange = (value: string) => {
    const newValue = value === "all_clinics" ? null : value;
    setClinicId(newValue);
    onFilterChange({ specialty, clinicId: newValue });
  };

  const clearFilters = () => {
    setSpecialty(null);
    setClinicId(null);
    onFilterChange({ specialty: null, clinicId: null });
  };

  return (
    <>
      <div className="flex justify-between items-center mb-4">
        <Button
          variant="outline"
          size="sm"
          className="flex items-center gap-2"
          onClick={() => setShowFilters(!showFilters)}
        >
          <Filter className="h-4 w-4" />
          Filters
        </Button>
        
        {(specialty || clinicId) && (
          <Button
            variant="ghost"
            size="sm"
            className="flex items-center gap-2 text-muted-foreground"
            onClick={clearFilters}
          >
            <X className="h-4 w-4" />
            Clear Filters
          </Button>
        )}
      </div>
      
      {showFilters && (
        <Card className="mb-6">
          <CardContent className="p-4">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label htmlFor="specialty-filter">Specialty</Label>
                <Select 
                  value={specialty || "all_specialties"} 
                  onValueChange={handleSpecialtyChange}
                >
                  <SelectTrigger id="specialty-filter">
                    <SelectValue placeholder="All Specialties" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all_specialties">All Specialties</SelectItem>
                    {specialties.map(specialty => (
                      <SelectItem key={specialty} value={specialty}>
                        {specialty}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
              
              <div className="space-y-2">
                <Label htmlFor="clinic-filter">Clinic</Label>
                <Select 
                  value={clinicId || "all_clinics"} 
                  onValueChange={handleClinicChange}
                >
                  <SelectTrigger id="clinic-filter">
                    <SelectValue placeholder="All Clinics" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all_clinics">All Clinics</SelectItem>
                    {clinics.map(clinic => (
                      <SelectItem key={clinic.id} value={clinic.id}>
                        {clinic.name}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
            </div>
          </CardContent>
        </Card>
      )}
    </>
  );
}
