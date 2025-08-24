// ClinicsPage.tsx
import { useState } from "react";
import { useAuth } from "@/hooks/useAuth";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import {
  Search,
  Plus,
  FileEdit,
  Trash2,
  MapPin,
  Phone,
  Mail,
} from "lucide-react";
import { Input } from "@/components/ui/input";
import { Badge } from "@/components/ui/badge";
import { useCliniques } from "@/hooks/useCliniques";
import { Clinique, TypeClinique, StatutClinique } from "@/types/clinic";
import { toast } from "sonner";
import { ClinicForm } from "@/components/clinics/ClinicForm";
import { CliniqueDetail } from "@/components/clinics/CliniqueDetail";
import { ClinicFilters } from "@/components/clinics/ClinicFilters";
import { ConfirmDeleteDialog } from "@/components/consultations/ConfirmDeleteDialog";
import { useTranslation } from "@/hooks/useTranslation";
import {
  Pagination,
  PaginationContent,
  PaginationItem,
  PaginationNext,
  PaginationPrevious,
} from "@/components/ui/pagination";
import { cn } from "@/lib/utils";

type ClinicFormValues = Omit<Clinique, "id" | "dateCreation">;

function ClinicsPage() {
  const { t } = useTranslation("clinics");
  const tCommon = useTranslation("common").t;
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
  const [addressSearch, setAddressSearch] = useState("");
  const [editingClinic, setEditingClinic] = useState<Clinique | null>(null);
  const [selectedClinic, setSelectedClinic] = useState<Clinique | null>(null);
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false); // État pour le dialogue
  const [clinicToDelete, setClinicToDelete] = useState<Clinique | null>(null); // Clinique à supprimer
  const [currentPage, setCurrentPage] = useState(1); // État pour la page actuelle

  const getTypeCliniqueLabel = (value: number) => {
    const key =
      TypeClinique[
        value as unknown as keyof typeof TypeClinique as string
      ]?.toLowerCase();
    return t(key) || t("unknown");
  };

  // Filtrer les cliniques pour les Doctor
  const doctorClinic =
    user?.role === "Doctor" ? filteredCliniques[0] || null : null;

  // Open modal for add
  const handleOpenAdd = () => {
    setEditingClinic(null);
    setIsFormOpen(true);
  };

  // Open modal for edit
  const handleOpenEdit = (clinic: Clinique, event: React.MouseEvent) => {
    event.stopPropagation(); // Empêche la propagation au parent
    setEditingClinic(clinic);
    setIsFormOpen(true);
  };

  // Handle form submit (add or edit)
  const handleFormSubmit = async (
    data: Omit<ClinicFormValues, "typeClinique" | "statut"> & {
      typeClinique: number;
      statut: number;
    }
  ): Promise<Clinique> => {
    if (editingClinic) {
      const updated = await handleUpdateClinique(editingClinic.id, data);
      setIsFormOpen(false);
      setEditingClinic(null);
      refetchCliniques();
      return updated;
    } else {
      const created = await handleAddClinique(data);
      setIsFormOpen(false);
      refetchCliniques();
      return created;
    }
  };

  // Open delete confirmation dialog
  const handleOpenDeleteDialog = (
    clinic: Clinique,
    event: React.MouseEvent
  ) => {
    event.stopPropagation();
    setClinicToDelete(clinic);
    setIsDeleteDialogOpen(true);
  };

  // Handle delete confirmation
  const handleConfirmDelete = async () => {
    if (clinicToDelete) {
      await handleDeleteClinique(clinicToDelete.id);
      refetchCliniques();
      setIsDeleteDialogOpen(false);
      setClinicToDelete(null);
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
    const variant =
      statut === StatutClinique.Active
        ? "text-green-600 bg-green-50 border-green-200"
        : "text-red-600 bg-red-50 border-red-200";
    return (
      <Badge variant="outline" className={variant}>
        {statut === StatutClinique.Active ? t("active") : t("inactive")}
      </Badge>
    );
  };

  const typeOptions = Object.values(TypeClinique)
    .filter((v) => typeof v === "number")
    .map((value) => ({
      value: value as number,
      label: TypeClinique[value as number],
    }));

  const statusOptions = Object.values(StatutClinique)
    .filter((v) => typeof v === "number")
    .map((value) => ({
      value: value as number,
      label: StatutClinique[value as number],
    }));

  const [clinicFilters, setClinicFilters] = useState<{
    typeClinique: number | null;
    statut: number | null;
  }>({
    typeClinique: null,
    statut: null,
  });

  const filteredCliniquesToShow = filteredCliniques.filter((clinic) => {
    const matchesType =
      clinicFilters.typeClinique === null ||
      clinic.typeClinique === clinicFilters.typeClinique;
    const matchesStatus =
      clinicFilters.statut === null || clinic.statut === clinicFilters.statut;
    return matchesType && matchesStatus;
  });

  // Constantes pour la pagination
  const ITEMS_PER_PAGE = 5;
  const totalPages = Math.ceil(filteredCliniques.length / ITEMS_PER_PAGE);
  const startIndex = (currentPage - 1) * ITEMS_PER_PAGE;
  const paginatedCliniques = filteredCliniquesToShow.slice(
    startIndex,
    startIndex + ITEMS_PER_PAGE
  );

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

  if (
    !["SuperAdmin", "Doctor", "Patient", "ClinicAdmin"].includes(user?.role)
  ) {
    return (
      <div className="flex items-center justify-center h-full">
        <Card>
          <CardHeader>
            <CardTitle>{t("access_denied")}</CardTitle>
            <CardDescription>{t("access_denied_description")}</CardDescription>
          </CardHeader>
        </Card>
      </div>
    );
  }

  // Si l'utilisateur est un Doctor, afficher directement les détails de sa clinique
  if (user?.role === "Doctor") {
    return (
      <div className="space-y-6 pb-8">
        {isLoading ? (
          <Card>
            <CardContent className="text-center py-8">
              <p className="text-muted-foreground">{t("loading_clinics")}</p>
            </CardContent>
          </Card>
        ) : doctorClinic ? (
          <CliniqueDetail
            clinique={doctorClinic}
            statistics={statistics}
            isLoadingStats={isLoadingStats}
            onBack={() => setSelectedClinic(null)}
          />
        ) : (
          <Card>
            <CardContent className="text-center py-8">
              <p className="text-muted-foreground">
                {t("doctor_no_clinic_assigned")}
              </p>
            </CardContent>
          </Card>
        )}
      </div>
    );
  }

  return (
    <div className="space-y-6 pb-8">
      <div className="flex flex-col gap-2">
        <h1 className="text-3xl font-bold tracking-tight">{t("clinics")}</h1>
        <p className="text-muted-foreground">
          {t("manage_clinics_description")}
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
            placeholder={t("search_clinics")}
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="pl-8"
          />
        </div>
        {permissions.canCreate && (
          <Button className="ml-2" onClick={handleOpenAdd}>
            <Plus className="mr-1 h-4 w-4" /> {t("add_clinic")}
          </Button>
        )}
      </div>

      <div className="flex items-center justify-between mb-4">
        <div className="relative w-full max-w-sm">
          <MapPin className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input
            placeholder={t("search_by_address")}
            value={addressSearch}
            onChange={(e) => {
              setAddressSearch(e.target.value);
              handleSearchByAddress(e.target.value);
            }}
            className="pl-8"
          />
        </div>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>{t("all_clinics")}</CardTitle>
          <CardDescription>{t("all_clinics_description")}</CardDescription>
        </CardHeader>
        <CardContent>
          {isLoading ? (
            <div className="text-center py-8 text-muted-foreground">
              {t("loading_clinics")}
            </div>
          ) : filteredCliniquesToShow.length === 0 ? (
            <div className="text-center py-8 text-muted-foreground">
              {t("no_clinics_found")}
            </div>
          ) : (
            paginatedCliniques.map((clinic) => (
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
                      <Badge variant="secondary" className="ml-2">
                        {getTypeCliniqueLabel(clinic.typeClinique)}
                      </Badge>
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
                          <a
                            href={clinic.siteWeb}
                            target="_blank"
                            rel="noopener noreferrer"
                            className="underline"
                          >
                            {clinic.siteWeb}
                          </a>
                        </div>
                      )}
                    </div>
                  </div>
                  {(permissions.canEdit || permissions.canDelete) && (
                    <div className="flex flex-wrap gap-2">
                      {permissions.canEdit && (
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={(e) => handleOpenEdit(clinic, e)}
                        >
                          <FileEdit className="h-3.5 w-3.5 mr-1" /> {t("edit")}
                        </Button>
                      )}
                      {permissions.canDelete &&
                        clinic.statut === StatutClinique.Active && (
                          <Button
                            variant="outline"
                            size="sm"
                            className="text-red-500"
                            onClick={(e) => handleOpenDeleteDialog(clinic, e)}
                          >
                            <Trash2 className="h-3.5 w-3.5 mr-1" />{" "}
                            {t("delete")}
                          </Button>
                        )}
                    </div>
                  )}
                </div>
              </div>
            ))
          )}
          {totalPages > 1 && (
            <Pagination>
              <PaginationContent>
                <PaginationItem>
                  <PaginationPrevious
                    onClick={() =>
                      setCurrentPage((prev) => Math.max(prev - 1, 1))
                    }
                    className={cn(
                      currentPage <= 1 && "pointer-events-none opacity-50"
                    )}
                  />
                </PaginationItem>
                <PaginationItem className="flex items-center">
                  <span className="text-sm">
                    {tCommon("page")} {currentPage} {tCommon("of")} {totalPages}
                  </span>
                </PaginationItem>
                <PaginationItem>
                  <PaginationNext
                    onClick={() =>
                      setCurrentPage((prev) => Math.min(prev + 1, totalPages))
                    }
                    className={cn(
                      currentPage >= totalPages &&
                        "pointer-events-none opacity-50"
                    )}
                  />
                </PaginationItem>
              </PaginationContent>
            </Pagination>
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
        initialData={
          editingClinic
            ? {
                nom: editingClinic.nom,
                adresse: editingClinic.adresse,
                numeroTelephone: editingClinic.numeroTelephone,
                email: editingClinic.email,
                siteWeb: editingClinic.siteWeb,
                description: editingClinic.description,
                typeClinique: editingClinic.typeClinique?.toString(),
                statut: editingClinic.statut?.toString(),
              }
            : undefined
        }
      />

      <ConfirmDeleteDialog
        isOpen={isDeleteDialogOpen}
        onClose={() => {
          setIsDeleteDialogOpen(false);
          setClinicToDelete(null);
        }}
        onConfirm={handleConfirmDelete}
        title={t("confirm_delete_title")}
        message={t("confirm_delete_message")}
      />
    </div>
  );
}

export default ClinicsPage;
