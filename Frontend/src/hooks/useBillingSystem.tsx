import { useState, useEffect, useCallback, useMemo } from "react";
import { toast } from "sonner";
import { useAuth } from "@/hooks/useAuth";
import { billingService } from "@/services/billingService";
import {
  CreateFactureRequest,
  PayInvoiceRequest,
  TarifConsultation,
  Facture,
  BillingStatsDto,
} from "@/types/billing";
import { ConsultationType } from "@/types/consultation";
import { useTranslation } from "./useTranslation";

interface UseBillingSystemState {
  invoices: Facture[];
  consultationTypes: ConsultationType[];
  consultationPricing: TarifConsultation[];
  billingStats: BillingStatsDto | null;
  isStatsLoading: boolean;
  statsError: string | null;
  isLoading: boolean;
  isSubmitting: boolean;
  searchTerm: string;
  setSearchTerm: (term: string) => void;
  currentPage: number;
  setCurrentPage: (page: number) => void;
  totalPages: number;
  filteredCount: number;
  totalCount: number;
  permissions: {
    canViewInvoices: boolean;
    canPayInvoices: boolean;
    canManagePricing: boolean;
    canViewAllInvoices: boolean;
  };
  fetchInvoices: (filters?: {
    status?: string;
    dateRange?: { from?: Date; to?: Date };
    amountRange?: { min?: string; max?: string };
    searchTerm?: string;
  }) => Promise<void>;
  fetchConsultationTypes: () => Promise<void>;
  fetchConsultationPricing: () => Promise<void>;
  createInvoice: (data: CreateFactureRequest) => Promise<Facture | null>;
  payInvoice: (invoiceId: string, data: PayInvoiceRequest) => Promise<boolean>;
  downloadInvoicePDF: (invoiceId: string) => Promise<void>;
  getConsultationPrice: (consultationType: ConsultationType) => number;
  viewInvoiceDetails: (invoiceId: string) => Promise<Facture | null>;
  fetchBillingStats: (clinicId: string) => Promise<BillingStatsDto | null>;
}

export function useBillingSystem(clinicId?: string): UseBillingSystemState {
  const { user } = useAuth();
  const { t } = useTranslation("billing");
  const [invoices, setInvoices] = useState<Facture[]>([]);
  const [consultationTypes, setConsultationTypes] = useState<
    ConsultationType[]
  >([]);
  const [consultationPricing, setConsultationPricing] = useState<
    TarifConsultation[]
  >([]);
  const [isLoading, setIsLoading] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [searchTerm, setSearchTerm] = useState("");
  const [currentPage, setCurrentPage] = useState(1);
  const itemsPerPage = 5;
  const [totalCount, setTotalCount] = useState(0);
  const [billingStats, setBillingStats] = useState<BillingStatsDto | null>(
    null
  );
  const [isStatsLoading, setIsStatsLoading] = useState(false);
  const [statsError, setStatsError] = useState<string | null>(null);
  const [hasFetched, setHasFetched] = useState(false);

  // Check permissions based on user role
  const permissions = {
    canViewInvoices: true,
    canPayInvoices:
      user?.role === "Patient" ||
      user?.role === "ClinicAdmin" ||
      user?.role === "SuperAdmin",
    canManagePricing:
      user?.role === "ClinicAdmin" || user?.role === "SuperAdmin",
    canViewAllInvoices:
      user?.role === "ClinicAdmin" || user?.role === "SuperAdmin",
  };

  // Fetch invoices based on user role
  const fetchInvoices = useCallback(
    async (filters?: {
      status?: string;
      dateRange?: { from?: Date; to?: Date };
      amountRange?: { min?: string; max?: string };
      searchTerm?: string;
    }) => {
      if (!user) return;

      setIsLoading(true);
      try {
        let invoiceData: Facture[] = [];
        const params = new URLSearchParams();

        if (user.role === "Patient") {
          params.append("patientId", user.patientId);
        } else if (
          (user.role === "ClinicAdmin" || user.role === "SuperAdmin") &&
          clinicId
        ) {
          params.append("clinicId", clinicId);
        } else if (user.role === "SuperAdmin") {
          // Pas de filtre spécifique pour SuperAdmin sur clinicId
        }

        if (filters?.status) params.append("status", filters.status);
        if (filters?.dateRange?.from)
          params.append("from", filters.dateRange.from.toISOString());
        if (filters?.dateRange?.to)
          params.append("to", filters.dateRange.to.toISOString());
        if (filters?.amountRange?.min)
          params.append("minAmount", filters.amountRange.min);
        if (filters?.amountRange?.max)
          params.append("maxAmount", filters.amountRange.max);
        if (filters?.searchTerm)
          params.append("searchTerm", filters.searchTerm);

        params.append("page", (currentPage - 1).toString());
        params.append("size", itemsPerPage.toString());

        invoiceData = await billingService.filterFactures({
          clinicId: clinicId,
          patientId: user.role === "Patient" ? user.patientId : undefined,
          status: filters?.status,
        });

        const validInvoices = invoiceData.filter(
          (invoice) => invoice.consultationId != null
        );
        setInvoices(validInvoices);
        setTotalCount(invoiceData.length); // À ajuster si l'API renvoie un total
      } catch (error: unknown) {
        console.error("[fetchInvoices] Error:", error);
        const errorMessage =
          error instanceof Error
            ? error.message
            : "Erreur lors du chargement des factures";
        toast.error(errorMessage);
      } finally {
        setIsLoading(false);
      }
    },
    [user, clinicId, currentPage]
  );

  // Fetch consultation types
  const fetchConsultationTypes = useCallback(async () => {
    try {
      const types = Object.keys(ConsultationType)
        .filter((key) => isNaN(Number(key)))
        .map((key) => ConsultationType[key as keyof typeof ConsultationType]);
      setConsultationTypes(types);
    } catch (error) {
      console.error("Error fetching consultation types:", error);
      toast.error("Erreur lors du chargement des types de consultation");
    }
  }, []);

  // Fetch consultation pricing for clinic
  const fetchConsultationPricing = useCallback(async () => {
    if (!clinicId) return;

    try {
      const pricing = await billingService.getConsultationPricing(clinicId);
      setConsultationPricing(pricing);
    } catch (error) {
      console.error("Error fetching consultation pricing:", error);
      toast.error("Erreur lors du chargement des tarifs");
    }
  }, [clinicId]);

  // Create invoice
  const createInvoice = async (
    data: CreateFactureRequest
  ): Promise<Facture | null> => {
    setIsSubmitting(true);
    console.log("[createInvoice] Request data:", data);
    try {
      await billingService.addFacture(data);
      await fetchInvoices();
      toast.success("Facture créée avec succès");
      const newInvoice = await billingService.getFacturesByConsultationId(
        data.consultationId
      );
      console.log(
        "[createInvoice] New invoice fetched by consultationId:",
        newInvoice
      );
      return newInvoice;
    } catch (error) {
      console.error("[createInvoice] Error:", error);
      toast.error("Erreur lors de la création de la facture");
      return null;
    } finally {
      setIsSubmitting(false);
    }
  };

  // Pay invoice
  const payInvoice = async (
    invoiceId: string,
    data: PayInvoiceRequest & { montant: number }
  ): Promise<boolean> => {
    setIsSubmitting(true);
    try {
      const success = await billingService.payerFacture(invoiceId, data);
      if (success) {
        await fetchInvoices();
        toast.success("Paiement effectué avec succès");
        return true;
      }
      toast.error("Paiement échoué : facture introuvable ou déjà payée");
      return false;
    } catch (error) {
      console.error("[payInvoice] Error:", error);
      const errorMessage =
        error instanceof Error ? error.message : "Erreur lors du paiement";
      toast.error(errorMessage);
      return false;
    } finally {
      setIsSubmitting(false);
    }
  };

  // Download invoice PDF
  const downloadInvoicePDF = async (invoiceId: string): Promise<void> => {
    console.log("[downloadInvoicePDF] invoiceId:", invoiceId);
    try {
      const blob = await billingService.exportFacturePdf(invoiceId);
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement("a");
      a.style.display = "none";
      a.href = url;
      a.download = `facture-${invoiceId}.pdf`;
      document.body.appendChild(a);
      a.click();
      window.URL.revokeObjectURL(url);
      document.body.removeChild(a);
      toast.success("PDF téléchargé avec succès");
    } catch (error) {
      console.error("[downloadInvoicePDF] Error:", error);
      toast.error("Erreur lors du téléchargement du PDF");
    }
  };

  // Get consultation price by type
  const getConsultationPrice = (consultationType: ConsultationType): number => {
    const pricing = consultationPricing.find(
      (p) => p.consultationType === consultationType
    );
    return pricing?.prix || 0;
  };

  // View invoice details
  const viewInvoiceDetails = async (id: string): Promise<Facture | null> => {
    if (!id) {
      console.error("[viewInvoiceDetails] invoiceId is undefined!");
      return null;
    }

    console.log("[viewInvoiceDetails] invoiceId:", id);

    try {
      const invoice = await billingService.getFactureById(id);
      console.log("[viewInvoiceDetails] fetched invoice:", invoice);
      return invoice;
    } catch (error) {
      console.error("[viewInvoiceDetails] Error fetching details:", error);
      toast.error("Erreur lors du chargement des détails de la facture");
      return null;
    }
  };

  const fetchBillingStats = useCallback(
    async (clinicId: string): Promise<BillingStatsDto | null> => {
      setIsStatsLoading(true);
      setStatsError(null);

      try {
        const stats = await billingService.getBillingStats(clinicId);
        setBillingStats(stats);
        return stats;
      } catch (error) {
        console.error("Erreur chargement statistiques :", error);
        setStatsError("Impossible de charger les statistiques de facturation.");
        toast.error("Impossible de charger les statistiques.");
        return null;
      } finally {
        setIsStatsLoading(false);
      }
    },
    []
  );

  // Filtrage local pour la recherche
  const filteredInvoices = useMemo(() => {
    console.log("[filteredInvoices] searchTerm:", searchTerm);
    if (!searchTerm.trim()) return invoices;
    const lowerTerm = searchTerm.toLowerCase();
    const result = invoices.filter(
      (inv) =>
        inv.id?.toLowerCase().includes(lowerTerm) ||
        inv.paiement?.factureId?.toLowerCase().includes(lowerTerm) ||
        inv.status?.toLowerCase().includes(lowerTerm)
    );
    console.log("[filteredInvoices] filtered results:", result);
    return result;
  }, [invoices, searchTerm]);

  // Pagination locale
  const totalPages = Math.max(
    1,
    Math.ceil(filteredInvoices.length / itemsPerPage)
  );
  const paginatedInvoices = useMemo(() => {
    const start = (currentPage - 1) * itemsPerPage;
    const slice = filteredInvoices.slice(start, start + itemsPerPage);
    console.log("[paginatedInvoices] page:", currentPage, "slice:", slice);
    return slice;
  }, [filteredInvoices, currentPage]);

  // Initial data loading
  useEffect(() => {
    if (user && !hasFetched) {
      console.log("[useEffect] Initial fetch triggered");
      fetchInvoices();
      fetchConsultationTypes();
      if (clinicId && permissions.canManagePricing) {
        fetchConsultationPricing();
      }
      setHasFetched(true);
    }
  }, [
    user,
    clinicId,
    fetchInvoices,
    fetchConsultationTypes,
    fetchConsultationPricing,
    permissions.canManagePricing,
    hasFetched,
  ]);

  return {
    invoices: paginatedInvoices,
    consultationTypes,
    consultationPricing,
    isLoading,
    isSubmitting,
    searchTerm,
    setSearchTerm,
    currentPage,
    setCurrentPage,
    totalPages,
    filteredCount: filteredInvoices.length,
    totalCount: invoices.length,
    permissions,
    fetchInvoices,
    fetchConsultationTypes,
    fetchConsultationPricing,
    createInvoice,
    payInvoice,
    downloadInvoicePDF,
    getConsultationPrice,
    viewInvoiceDetails,
    billingStats,
    isStatsLoading,
    statsError,
    fetchBillingStats,
  };
}
