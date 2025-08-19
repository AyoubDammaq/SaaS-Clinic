import { useState, useMemo, useCallback, useEffect } from "react";
import {
  AddTarificationRequest,
  BillingStatsDto,
  Facture,
  FactureStatus,
  PayInvoiceRequest,
  TarifConsultation,
  UpdateTarificationRequest,
} from "@/types/billing";
import { useAuth } from "@/hooks/useAuth";
import { toast } from "sonner";
import { billingService } from "@/services/billingService";

interface UseBillingOptions {
  initialInvoices?: Facture[];
  itemsPerPage?: number;
  autoFetch?: boolean;
}

export function useBilling({
  initialInvoices = [],
  itemsPerPage = 5,
  autoFetch = true,
}: UseBillingOptions = {}) {
  const { user } = useAuth();

  // États pour factures
  const [invoices, setInvoices] = useState<Facture[]>(initialInvoices);
  const [searchTerm, setSearchTerm] = useState("");
  const [currentPage, setCurrentPage] = useState(1);
  const [sortField, setSortField] = useState<keyof Facture>("dateEmission");
  const [sortDirection, setSortDirection] = useState<"asc" | "desc">("desc");
  const [selectedInvoice, setSelectedInvoice] = useState<Facture | null>(null);
  const [isPaymentFormOpen, setIsPaymentFormOpen] = useState(false);
  const [billingStats, setBillingStats] = useState<BillingStatsDto | null>(
    null
  );
  const [isStatsLoading, setIsStatsLoading] = useState(false);
  const [statsError, setStatsError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // États pour tarifications
  const [tarifications, setTarifications] = useState<TarifConsultation[]>([]);
  const [isTarifsLoading, setIsTarifsLoading] = useState(false);
  const [tarifsError, setTarifsError] = useState<string | null>(null);

  // Fetch invoices selon rôle utilisateur
  const fetchInvoices = useCallback(async () => {
    if (!autoFetch) return;

    setIsLoading(true);
    setError(null);

    try {
      let fetchedInvoices: Facture[] = [];

      if (user?.role === "Patient") {
        fetchedInvoices = await billingService.filterFactures({
          patientId: user.patientId,
        });
      } else if (user?.role === "ClinicAdmin") {
        fetchedInvoices = await billingService.filterFactures({
          clinicId: user.cliniqueId,
        });
      } else {
        fetchedInvoices = await billingService.getAllFactures();
      }

      setInvoices(fetchedInvoices);
    } catch (err) {
      console.error("Failed to fetch invoices:", err);
      setError("Failed to load invoices. Please try again.");
      toast.error("Failed to load invoices");
    } finally {
      setIsLoading(false);
    }
  }, [autoFetch, user]);

  const getFacturesByClinicId = useCallback(async (cliniqueId: string) => {
    setIsLoading(true);
    setError(null);

    try {
      const factures = await billingService.getFacturesByClinicId(cliniqueId);
      setInvoices(factures);
    } catch (err) {
      console.error(
        "Erreur lors du chargement des factures par clinique :",
        err
      );
      setError("Erreur lors du chargement des factures de la clinique.");
      toast.error("Impossible de charger les factures de cette clinique");
    } finally {
      setIsLoading(false);
    }
  }, []);

  const handleDeleteFacture = useCallback(async (id: string) => {
    const confirmed = window.confirm(
      "Êtes-vous sûr de vouloir supprimer cette facture ?"
    );
    if (!confirmed) return;

    try {
      await billingService.deleteFacture(id);
      setInvoices((prev) => prev.filter((inv) => inv.id !== id));
      toast.success("Facture supprimée avec succès");
    } catch (error) {
      console.error("Erreur lors de la suppression de la facture:", error);
      toast.error("Échec de la suppression de la facture");
    }
  }, []);

  // Filtrage, tri et pagination factures
  const filteredInvoices = useMemo(() => {
    if (!searchTerm.trim()) return invoices;
    const lowerTerm = searchTerm.toLowerCase();

    return invoices.filter(
      (inv) =>
        inv.id.toLowerCase().includes(lowerTerm) ||
        (inv.paiement &&
          inv.paiement.factureId.toLowerCase().includes(lowerTerm)) ||
        inv.status.toLowerCase().includes(lowerTerm)
    );
  }, [invoices, searchTerm]);

  const sortedInvoices = useMemo(() => {
    return [...filteredInvoices].sort((a, b) => {
      let cmp = 0;

      if (sortField === "montantTotal" || sortField === "montantPaye") {
        cmp = (a[sortField] ?? 0) - (b[sortField] ?? 0);
      } else if (sortField === "dateEmission") {
        cmp =
          new Date(a.dateEmission).getTime() -
          new Date(b.dateEmission).getTime();
      } else if (
        typeof a[sortField] === "string" &&
        typeof b[sortField] === "string"
      ) {
        cmp = (a[sortField] as string).localeCompare(b[sortField] as string);
      }

      return sortDirection === "desc" ? -cmp : cmp;
    });
  }, [filteredInvoices, sortField, sortDirection]);

  const totalPages = Math.max(
    1,
    Math.ceil(sortedInvoices.length / itemsPerPage)
  );
  const paginatedInvoices = useMemo(() => {
    const start = (currentPage - 1) * itemsPerPage;
    return sortedInvoices.slice(start, start + itemsPerPage);
  }, [sortedInvoices, currentPage, itemsPerPage]);

  const handleSort = useCallback(
    (field: keyof Facture) => {
      if (field === sortField) {
        setSortDirection((d) => (d === "asc" ? "desc" : "asc"));
      } else {
        setSortField(field);
        setSortDirection("asc");
      }
    },
    [sortField]
  );

  const fetchBillingStats = useCallback(async (clinicId: string) => {
    setIsStatsLoading(true);
    setStatsError(null);

    try {
      const stats = await billingService.getBillingStats(clinicId);
      setBillingStats(stats);
    } catch (error) {
      console.error("Erreur chargement statistiques :", error);
      setStatsError("Impossible de charger les statistiques de facturation.");
      toast.error("Impossible de charger les statistiques.");
    } finally {
      setIsStatsLoading(false);
    }
  }, []);

  useEffect(() => {
    setCurrentPage(1);
  }, [searchTerm, sortField, sortDirection]);

  useEffect(() => {
    fetchInvoices();
  }, [fetchInvoices]);

  // Paiement
  const handlePayNow = useCallback((invoice: Facture) => {
    setSelectedInvoice(invoice);
    setIsPaymentFormOpen(true);
  }, []);

  const closePaymentForm = useCallback(() => {
    setIsPaymentFormOpen(false);
    setSelectedInvoice(null);
  }, []);

  const handlePaymentSubmit = useCallback(
    async (
      invoiceId: string,
      data: PayInvoiceRequest & { montant: number }
    ) => {
      if (!selectedInvoice) return;

      try {
        await billingService.payerFacture(selectedInvoice.id, data);

        setInvoices((prev) =>
          prev.map((inv) =>
            inv.id === selectedInvoice.id
              ? {
                  ...inv,
                  status: FactureStatus.PAYEE,
                  montantPaye: inv.montantTotal,
                }
              : inv
          )
        );

        closePaymentForm();

        toast.success("Payment successfully processed");
      } catch (error) {
        console.error("Payment error:", error);
        toast.error("Payment processing failed. Please try again.");
      }
    },
    [selectedInvoice, closePaymentForm]
  );

  const handleDownload = useCallback((invoice: Facture) => {
    toast.info(`Downloading invoice ${invoice.id}...`);
    // Implémenter export PDF si besoin
  }, []);

  // Permissions
  const userPermissions = useMemo(
    () => ({
      canPayInvoices: !!user?.role,
      canViewAllInvoices:
        user?.role === "SuperAdmin" || user?.role === "ClinicAdmin",
      canGenerateReports:
        user?.role === "SuperAdmin" || user?.role === "ClinicAdmin",
    }),
    [user?.role]
  );

  const refreshInvoices = useCallback(() => {
    fetchInvoices();
  }, [fetchInvoices]);

  // -- Gestion des Tarifications --

  const fetchTarifications = useCallback(async (clinicId: string) => {
    setIsTarifsLoading(true);
    setTarifsError(null);
    try {
      const data = await billingService.getConsultationPricing(clinicId);
      setTarifications(data);
    } catch (error) {
      console.error("Erreur chargement tarifications:", error);
      setTarifsError("Impossible de charger les tarifications.");
      toast.error("Impossible de charger les tarifications.");
    } finally {
      setIsTarifsLoading(false);
    }
  }, []);

  const refreshTarifications = useCallback(
    (clinicId: string) => {
      fetchTarifications(clinicId);
    },
    [fetchTarifications]
  );

  const addTarification = useCallback(
    async (data: AddTarificationRequest) => {
      try {
        await billingService.addTarification(data);
        toast.success("Tarification ajoutée avec succès");
        refreshInvoices();
      } catch (error) {
        console.error("Erreur ajout tarification :", error);
        toast.error("Impossible d'ajouter la tarification");
      }
    },
    [refreshInvoices]
  );

  const updateTarification = useCallback(
    async (data: UpdateTarificationRequest) => {
      try {
        await billingService.updateTarification(data);
        toast.success("Tarification mise à jour avec succès");
        refreshInvoices();
      } catch (error) {
        console.error("Erreur mise à jour tarification :", error);
        toast.error("Impossible de mettre à jour la tarification");
      }
    },
    [refreshInvoices]
  );

  return {
    invoices: paginatedInvoices,
    isLoading,
    error,
    filteredCount: filteredInvoices.length,
    totalCount: invoices.length,
    currentPage,
    totalPages,
    searchTerm,
    setSearchTerm,
    setCurrentPage,
    sortField,
    sortDirection,
    handleSort,
    selectedInvoice,
    handleDeleteFacture,
    isPaymentFormOpen,
    handlePayNow,
    handlePaymentSubmit,
    closePaymentForm,
    handleDownload,
    refreshInvoices,
    getFacturesByClinicId,
    billingStats,
    isStatsLoading,
    statsError,
    fetchBillingStats,
    // Tarifications
    tarifications,
    isTarifsLoading,
    tarifsError,
    fetchTarifications,
    refreshTarifications,
    addTarification,
    updateTarification,
    userPermissions,
  };
}
