import { useState } from 'react';
import { Button } from '@/components/ui/button';
import { Card, CardContent } from '@/components/ui/card';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Label } from '@/components/ui/label';
import { Filter, X } from 'lucide-react';
import { TypeClinique, StatutClinique } from '@/types/clinic';

interface ClinicFiltersProps {
  types: { value: number; label: string }[];
  statuses: { value: number; label: string }[];
  onFilterChange: (filters: { typeClinique: number | null; statut: number | null }) => void;
}

export function ClinicFilters({ types, statuses, onFilterChange }: ClinicFiltersProps) {
  const [showFilters, setShowFilters] = useState(false);
  const [typeClinique, setTypeClinique] = useState<number | null>(null);
  const [statut, setStatut] = useState<number | null>(null);

  const handleTypeChange = (value: string) => {
    const newValue = value === "all_types" ? null : Number(value);
    setTypeClinique(newValue);
    onFilterChange({ typeClinique: newValue, statut });
  };

  const handleStatusChange = (value: string) => {
    const newValue = value === "all_statuses" ? null : Number(value);
    setStatut(newValue);
    onFilterChange({ typeClinique, statut: newValue });
  };

  const clearFilters = () => {
    setTypeClinique(null);
    setStatut(null);
    onFilterChange({ typeClinique: null, statut: null });
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
        {(typeClinique !== null || statut !== null) && (
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
                <Label htmlFor="type-filter">Clinic Type</Label>
                <Select
                  value={typeClinique !== null ? typeClinique.toString() : "all_types"}
                  onValueChange={handleTypeChange}
                >
                  <SelectTrigger id="type-filter">
                    <SelectValue placeholder="All Types" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all_types">All Types</SelectItem>
                    {types.map(type => (
                      <SelectItem key={type.value} value={type.value.toString()}>
                        {type.label}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
              <div className="space-y-2">
                <Label htmlFor="status-filter">Status</Label>
                <Select
                  value={statut !== null ? statut.toString() : "all_statuses"}
                  onValueChange={handleStatusChange}
                >
                  <SelectTrigger id="status-filter">
                    <SelectValue placeholder="All Statuses" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all_statuses">All Statuses</SelectItem>
                    {statuses.map(status => (
                      <SelectItem key={status.value} value={status.value.toString()}>
                        {status.label}
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