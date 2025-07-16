import { useNavigate } from 'react-router-dom';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { Search, Plus, FileEdit, Trash2 } from 'lucide-react';
import { Card, CardContent } from '@/components/ui/card';
import { Clinique } from '@/types/clinic';

interface CliniquesListProps {
  cliniques: Clinique[];
  filteredCliniques: Clinique[];
  searchTerm: string;
  setSearchTerm: (term: string) => void;
  isLoading: boolean;
  permissions: {
    canCreate: boolean;
    canEdit: boolean;
    canDelete: boolean;
  };
  onAddClinique: () => void;
  onEditClinique: (clinique: Clinique) => void;
  onDeleteClinique: (id: string) => void;
  onSelectClinique: (clinique: Clinique) => void;
}

export function CliniquesList({
  cliniques,
  filteredCliniques,
  searchTerm,
  setSearchTerm,
  isLoading,
  permissions,
  onAddClinique,
  onEditClinique,
  onDeleteClinique,
  onSelectClinique
}: CliniquesListProps) {
  const navigate = useNavigate();

  if (isLoading) {
    return (
      <Card>
        <CardContent className="pt-6">
          <div className="flex justify-center items-center h-40">
            <p className="text-muted-foreground">Chargement des cliniques...</p>
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
            placeholder="Rechercher des cliniques..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="pl-8"
          />
        </div>
        {permissions.canCreate && (
          <Button className="ml-2" onClick={onAddClinique}>
            <Plus className="mr-1 h-4 w-4" /> Ajouter une clinique
          </Button>
        )}
      </div>
      
      <div className="rounded-md border">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Nom</TableHead>
              <TableHead>Email</TableHead>
              <TableHead>Téléphone</TableHead>
              <TableHead>Adresse</TableHead>
              <TableHead>Date de création</TableHead>
              {(permissions.canEdit || permissions.canDelete) && <TableHead>Actions</TableHead>}
            </TableRow>
          </TableHeader>
          <TableBody>
            {filteredCliniques.length === 0 ? (
              <TableRow>
                <TableCell colSpan={6} className="h-24 text-center">
                  Aucune clinique trouvée
                </TableCell>
              </TableRow>
            ) : (
              filteredCliniques.map((clinique) => (
                <TableRow 
                  key={clinique.id} 
                  className="cursor-pointer hover:bg-muted/60"
                  onClick={() => onSelectClinique(clinique)}
                >
                  <TableCell className="font-medium">{clinique.nom}</TableCell>
                  <TableCell>{clinique.email}</TableCell>
                  <TableCell>{clinique.numeroTelephone}</TableCell>
                  <TableCell>{clinique.adresse}</TableCell>
                  <TableCell>{clinique.dateCreation}</TableCell>
                  {(permissions.canEdit || permissions.canDelete) && (
                    <TableCell onClick={(e) => e.stopPropagation()}>
                      <div className="flex items-center gap-2">
                        {permissions.canEdit && (
                          <Button size="sm" variant="ghost" onClick={(e) => {
                            e.stopPropagation();
                            onEditClinique(clinique);
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
                              onDeleteClinique(clinique.id);
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
