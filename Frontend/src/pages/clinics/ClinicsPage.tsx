import { useState } from 'react';
import { useAuth } from "@/hooks/useAuth";
import { 
  Card, CardContent, CardDescription, CardHeader, CardTitle 
} from '@/components/ui/card';
import { 
  Table, TableBody, TableCell, TableHead, TableHeader, TableRow 
} from '@/components/ui/table';
import { Button } from '@/components/ui/button';
import { 
  Search, Plus, FileEdit, Trash2, Users, User, Calendar, CreditCard, MapPin, Phone, Mail 
} from 'lucide-react';
import { Input } from '@/components/ui/input';
import { Badge } from '@/components/ui/badge';
import { useCliniques } from '@/hooks/useCliniques';
import { Clinique } from '@/types/clinic';
import { toast } from 'sonner';
import { ClinicForm } from '@/components/clinics/ClinicForm';
import { StatutClinique, TypeClinique } from '@/types/clinic';
import { CliniqueDetail } from '@/components/clinics/CliniqueDetail';
import { ClinicFilters } from '@/components/clinics/ClinicFilters';


type ClinicFormValues = Omit<Clinique, 'id' | 'dateCreation'>;

function ClinicsPage() {
  const { user } = useAuth();
  const {
    filteredCliniques,
    isLoading,
    permissions,
    searchTerm,
    setSearchTerm,
    handleAddClinique,
    handleUpdateClinique,
    handleDeleteClinique,
    refetchCliniques,
    statistics,
    isLoadingStats,
    fetchCliniqueStatistics,
    filterCliniquesByAddress,
  } = useCliniques();
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [addressSearch, setAddressSearch] = useState('');
  const [editingClinic, setEditingClinic] = useState<Clinique | null>(null);
  const [selectedClinic, setSelectedClinic] = useState<Clinique | null>(null);

  // Open modal for add
  const handleOpenAdd = () => {
    setEditingClinic(null);
    setIsFormOpen(true);
  };

  // Open modal for edit
  const handleOpenEdit = (clinic: Clinique) => {
    setEditingClinic(clinic);
    setIsFormOpen(true);
  };

  // Handle form submit (add or edit)
  const handleFormSubmit = async (data: ClinicFormValues) => {
    try {
      if (editingClinic) {
        await handleUpdateClinique(editingClinic.id, data);
        toast.success('Clinic updated successfully!');
      } else {
        await handleAddClinique(data);
        toast.success('Clinic added successfully!');
      }
      setIsFormOpen(false);
      setEditingClinic(null);
      refetchCliniques();
    } catch (error) {
      // Error handled in hooks
      console.error("Error submitting clinic form:", error);
      toast.error("Failed to save clinic. Please try again.");
    }
  };

  // Handle delete
  const handleDelete = async (clinic: Clinique) => {
    if (window.confirm(`Are you sure you want to delete "${clinic.nom}"?`)) {
      await handleDeleteClinique(clinic.id);
      refetchCliniques();
    }
  };

  const handleSearchByAddress = async (address: string) => {
    try {
      await filterCliniquesByAddress(address);
    } catch (error) {
      // Handle error (already handled in service with toast)
    }
  };

  const renderStatusBadge = (statut: number) => {
    const variant = statut === StatutClinique.Active
      ? 'text-green-600 bg-green-50 border-green-200'
      : 'text-red-600 bg-red-50 border-red-200';
    return (
      <Badge variant="outline" className={variant}>
        {statut === StatutClinique.Active ? "Active" : "Inactive"}
      </Badge>
    );
  };

    // Prepare type and status options for the filter
  const typeOptions = Object.values(TypeClinique)
    .filter((v) => typeof v === 'number')
    .map((value) => ({
      value: value as number,
      label: TypeClinique[value as number],
    }));

  const statusOptions = Object.values(StatutClinique)
    .filter((v) => typeof v === 'number')
    .map((value) => ({
      value: value as number,
      label: StatutClinique[value as number],
    }));

  // Filter state
  const [clinicFilters, setClinicFilters] = useState<{ typeClinique: number | null; statut: number | null }>({
    typeClinique: null,
    statut: null,
  });

  // Filtering logic
  const filteredCliniquesToShow = filteredCliniques.filter((clinic) => {
    const matchesType = clinicFilters.typeClinique === null || clinic.typeClinique === clinicFilters.typeClinique;
    const matchesStatus = clinicFilters.statut === null || clinic.statut === clinicFilters.statut;
    return matchesType && matchesStatus;
  });

    // Show details when a clinic is selected
  if (selectedClinic) {
    return (
      <CliniqueDetail
        clinique={selectedClinic}
        statistics={statistics}
        isLoadingStats={isLoadingStats}
        onBack={() => setSelectedClinic(null)}
      />
    );
  }

  // This page is only accessible to SuperAdmin
  if (user?.role !== 'SuperAdmin') {
    return (
      <div className="flex items-center justify-center h-full">
        <Card>
          <CardHeader>
            <CardTitle>Access Denied</CardTitle>
            <CardDescription>You do not have permission to access this page.</CardDescription>
          </CardHeader>
        </Card>
      </div>
    );
  }

  return (
    <div className="space-y-6 pb-8">
      <div className="flex flex-col gap-2">
        <h1 className="text-3xl font-bold tracking-tight">Clinics</h1>
        <p className="text-muted-foreground">
          Manage all clinics in the SaaS-Clinic platform
        </p>
      </div>

      <ClinicFilters
        types={typeOptions}
        statuses={statusOptions}
        onFilterChange={setClinicFilters}
      />

      <div className="flex items-center justify-between mb-4">
        <div className="relative w-full max-w-sm">
          <Search className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input
            placeholder="Search clinics..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="pl-8"
          />
        </div>
        <Button className="ml-2" onClick={handleOpenAdd}>
          <Plus className="mr-1 h-4 w-4" /> Add Clinic
        </Button>
      </div>

      {/* Address search input and button */}
      <div className="flex items-center justify-between mb-4">
        <div className="relative w-full max-w-sm">
          <MapPin className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input
            placeholder="Search by address..."
            value={addressSearch}
            onChange={e => {
              setAddressSearch(e.target.value);
              handleSearchByAddress(e.target.value); // Auto-filter as you type
            }}            
            className="pl-8"
          />
        </div>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>All Clinics</CardTitle>
          <CardDescription>List of all registered clinics</CardDescription>
        </CardHeader>
        <CardContent>
          {isLoading ? (
            <div className="text-center py-8 text-muted-foreground">
              Loading clinics...
            </div>
          ) : filteredCliniquesToShow.length === 0 ? (
            <div className="text-center py-8 text-muted-foreground">
              No clinics found
            </div>
          ) : (
            filteredCliniquesToShow.map((clinic) => (
              <div
                key={clinic.id}
                className="border rounded-lg p-4 mb-4 cursor-pointer hover:bg-muted/60"
                onClick={async () => {
                  setSelectedClinic(clinic);
                  await fetchCliniqueStatistics(clinic.id);
                }}
              >
                <div className="flex flex-col md:flex-row justify-between gap-4">
                  <div className="space-y-2">
                    <div className="flex items-center gap-2">
                      <h3 className="text-lg font-semibold">{clinic.nom}</h3>
                      {renderStatusBadge(clinic.statut)}
                      <Badge variant="secondary" className="ml-2">{clinic.typeClinique}</Badge>
                    </div>
                    <div className="flex items-center gap-1 text-muted-foreground text-sm">
                      <MapPin className="h-3.5 w-3.5" />
                      <span>{clinic.adresse}</span>
                    </div>
                    <div className="flex flex-col md:flex-row gap-4">
                      <div className="flex items-center gap-1 text-sm">
                        <Phone className="h-3.5 w-3.5 text-muted-foreground" />
                        <span>{clinic.numeroTelephone}</span>
                      </div>
                      <div className="flex items-center gap-1 text-sm">
                        <Mail className="h-3.5 w-3.5 text-muted-foreground" />
                        <span>{clinic.email}</span>
                      </div>
                      {clinic.siteWeb && (
                        <div className="flex items-center gap-1 text-sm">
                          <span className="text-muted-foreground">🌐</span>
                          <a href={clinic.siteWeb} target="_blank" rel="noopener noreferrer" className="underline">{clinic.siteWeb}</a>
                        </div>
                      )}
                    </div>
                  </div>
                  <div className="flex flex-wrap gap-2">
                    <Button variant="outline" size="sm" onClick={() => handleOpenEdit(clinic)}>
                      <FileEdit className="h-3.5 w-3.5 mr-1" /> Edit
                    </Button>
                    {clinic.statut === StatutClinique.Active && (
                      <Button
                        variant="outline"
                        size="sm"
                        className="text-red-500"
                        onClick={() => handleDelete(clinic)}
                      >
                        <Trash2 className="h-3.5 w-3.5 mr-1" /> Delete
                      </Button>
                    )}
                  </div>
                </div>
              </div>
            ))
          )}
        </CardContent>
      </Card>

      <ClinicForm
        isOpen={isFormOpen}
        onClose={() => {
          setIsFormOpen(false);
          setEditingClinic(null);
        }}
        onSubmit={handleFormSubmit}
        initialData={editingClinic ? {
          nom: editingClinic.nom,
          adresse: editingClinic.adresse,
          numeroTelephone: editingClinic.numeroTelephone,
          email: editingClinic.email,
          siteWeb: editingClinic.siteWeb,
          description: editingClinic.description,
          typeClinique: editingClinic.typeClinique?.toString(),
          statut: editingClinic.statut?.toString(),
        } : undefined}
      />
    </div>
  );
}

export default ClinicsPage;