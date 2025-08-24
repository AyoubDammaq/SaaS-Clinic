import { useEffect, useState } from "react";
import { useAuth } from "@/hooks/useAuth";
import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
  CardFooter,
} from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Calendar, RefreshCcw } from "lucide-react";
import {
  ConsultationPaymentForm,
  PaymentFormValues,
} from "@/components/billing/ConsultationPaymentForm";
import { ConsultationInvoiceTable } from "@/components/billing/ConsultationInvoiceTable";
import { SearchBar } from "@/components/billing/SearchBar";
import { EmptyState } from "@/components/billing/EmptyState";
import { PaginationControls } from "@/components/ui/pagination-controls";
import { BillingStats } from "@/components/billing/BillingStats";
import { InvoiceDetailsModal } from "@/components/billing/InvoiceDetailsModal";
import { useBillingSystem } from "@/hooks/useBillingSystem";
import { Facture, ModePaiement, PayInvoiceRequest } from "@/types/billing";
import { LoadingSpinner } from "@/components/ui/loading-spinner";
import {
  InvoiceFilters,
  InvoiceFilterState,
} from "@/components/billing/InvoiceFilters";
import { useTranslation } from "@/hooks/useTranslation";
import { mapFactureStatus } from "@/types/billing";
import { usePaymentNotifications } from "@/components/billing/PaymentStatusNotification";
import { toast } from "sonner";

interface BackendValidationError {
  message: string;
  errors?: Record<string, string[]>;
}

function BillingPage() {
  const { user } = useAuth();
  const clinicId = user?.role === "ClinicAdmin" ? user.cliniqueId : undefined;
  const {
    invoices,
    isLoading,
    isSubmitting,
    searchTerm,
    setSearchTerm,
    currentPage,
    setCurrentPage,
    totalPages,
    filteredCount,
    totalCount,
    permissions,
    fetchInvoices,
    payInvoice,
    downloadInvoicePDF,
    viewInvoiceDetails,
    billingStats,
    isStatsLoading,
    statsError,
    fetchBillingStats,
  } = useBillingSystem(clinicId);

  const { t } = useTranslation("billing");
  const tCommon = useTranslation("common").t;
  const { showErrorPayment } = usePaymentNotifications();

  const [invoiceToPay, setInvoiceToPay] = useState<Facture | null>(null);
  const [invoiceToView, setInvoiceToView] = useState<Facture | null>(null);
  const [isDetailsModalOpen, setIsDetailsModalOpen] = useState(false);
  const [filters, setFilters] = useState<InvoiceFilterState>({
    status: "",
    dateRange: { from: undefined, to: undefined },
    amountRange: { min: "", max: "" },
    searchTerm: "",
  });
  const [isExpanded, setIsExpanded] = useState(false);

  useEffect(() => {
    if (clinicId) {
      fetchBillingStats(clinicId);
    }
  }, [clinicId, fetchBillingStats]);

  // Filtrer les factures en fonction du rôle de l'utilisateur
  const roleFilteredInvoices = invoices.filter((invoice) => {
    if (user?.role === "Patient" && user.patientId) {
      return invoice.patientId === user.patientId;
    } else if (user?.role === "ClinicAdmin" && user.cliniqueId) {
      return invoice.clinicId === user.cliniqueId;
    } else if (user?.role === "SuperAdmin") {
      return true; // SuperAdmin voit toutes les factures
    }
    return false; // Par défaut, ne rien afficher si le rôle n'est pas reconnu
  });

  // Get page title and description based on user role
  const getPageContent = () => {
    if (user?.role === "Patient") {
      return {
        title: t("myInvoices"),
        description: t("viewPayInvoices"),
      };
    } else {
      return {
        title: t("billingManagement"),
        description: t("managePatientInvoices"),
      };
    }
  };

  const { title, description } = getPageContent();

  const handlePayInvoice = (invoice: Facture) => {
    setInvoiceToPay(invoice);
  };

  const handleViewDetails = async (invoice: Facture) => {
    try {
      const details = await viewInvoiceDetails(invoice.id);
      if (details) {
        setInvoiceToView(details);
        setIsDetailsModalOpen(true);
      }
    } catch (error) {
      console.error("Error fetching invoice details:", error);
    }
  };

  const handlePaymentSubmit = async (data: PaymentFormValues) => {
    if (!invoiceToPay) return false;
    try {
      console.log("Invoice to pay:", {
        id: invoiceToPay.id,
        montantTotal: invoiceToPay.montantTotal,
        montantPaye: invoiceToPay.montantPaye,
        status: invoiceToPay.status,
        calculatedMontant: invoiceToPay.montantTotal - invoiceToPay.montantPaye,
      });
      if (invoiceToPay.status === "PAYEE") {
        throw new Error(t("invoiceAlreadyPaid"));
      }
      const montant = invoiceToPay.montantTotal - invoiceToPay.montantPaye;
      if (montant <= 0) {
        throw new Error(t("noAmountToPay"));
      }
      const payInvoiceData: PayInvoiceRequest & { montant: number } = {
        MoyenPaiement: data.paymentMethod as ModePaiement,
        montant,
        CardDetails:
          data.paymentMethod === ModePaiement.CarteBancaire
            ? {
                CardNumber: data.cardNumber!.replace(/\s/g, ""),
                ExpiryDate: data.expiryDate!,
                Cvv: data.cvv!,
                CardholderName: data.cardholderName!,
              }
            : undefined,
      };
      console.log("Payment payload:", payInvoiceData);
      const success = await payInvoice(invoiceToPay.id, payInvoiceData);
      if (success) {
        toast.success(t("paymentSuccess"));
        return true;
      }
      return false;
    } catch (error: unknown) {
      const apiError = error as BackendValidationError;
      console.error("Payment error:", apiError.message);
      if (apiError.errors) {
        console.error("Détails validation :", apiError.errors);
      }
      toast.error(apiError.message || t("paymentError"));
      return false;
    }
  };

  const closePaymentForm = () => {
    setInvoiceToPay(null);
  };

  const closeDetailsModal = () => {
    setIsDetailsModalOpen(false);
    setInvoiceToView(null);
  };

  const handleDownloadPDF = async (invoiceId: string) => {
    await downloadInvoicePDF(invoiceId);
  };

  const handleFiltersChange = (newFilters: InvoiceFilterState) => {
    setFilters(newFilters);
    fetchInvoices(newFilters);
    setCurrentPage(1);
  };

  const handleToggleExpanded = () => {
    setIsExpanded(!isExpanded);
  };

  // Appliquer les filtres supplémentaires (statut, date, montant, recherche)
  const filteredInvoices = roleFilteredInvoices.filter((invoice) => {
    const matchesStatus =
      !filters.status || mapFactureStatus(invoice.status) === filters.status;
    const matchesDate =
      (!filters.dateRange.from ||
        new Date(invoice.dateEmission) >= filters.dateRange.from) &&
      (!filters.dateRange.to ||
        new Date(invoice.dateEmission) <= filters.dateRange.to);
    const matchesAmount =
      (!filters.amountRange.min ||
        invoice.montantTotal >= Number(filters.amountRange.min)) &&
      (!filters.amountRange.max ||
        invoice.montantTotal <= Number(filters.amountRange.max));
    const matchesSearch =
      !filters.searchTerm ||
      invoice.id.toLowerCase().includes(filters.searchTerm.toLowerCase()) ||
      (invoice.patientId &&
        invoice.patientId
          .toLowerCase()
          .includes(filters.searchTerm.toLowerCase()));

    return matchesStatus && matchesDate && matchesAmount && matchesSearch;
  });

  return (
    <div className="space-y-6 pb-8">
      <div className="flex flex-col gap-2">
        <h1 className="text-3xl font-bold tracking-tight">{title}</h1>
        <p className="text-muted-foreground" id="billing-description">
          {description}
        </p>
      </div>

      {/* Billing Statistics */}
      {user.role == "ClinicAdmin" && (
        <BillingStats
          stats={billingStats}
          isLoading={isStatsLoading}
          error={statsError}
        />
      )}

      <InvoiceFilters
        filters={filters}
        onFiltersChange={handleFiltersChange}
        isExpanded={isExpanded}
        onToggleExpanded={handleToggleExpanded}
      />

      <Card className="card-hover">
        <CardHeader>
          <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
            <div>
              <CardTitle>{t("consultationInvoices")}</CardTitle>
              <CardDescription>
                {user?.role === "Patient"
                  ? t("myRecentInvoices")
                  : t("recentInvoices")}
              </CardDescription>
            </div>
            <div className="flex flex-col sm:flex-row items-center gap-2 w-full md:w-auto">
              <Button
                variant="outline"
                size="icon"
                onClick={() => fetchInvoices(filters)}
                aria-label={tCommon("refreshInvoices")}
                className="shrink-0"
              >
                <RefreshCcw className="h-4 w-4" />
              </Button>
            </div>
          </div>
        </CardHeader>
        <CardContent>
          {isLoading ? (
            <LoadingSpinner fullPage text={t("loadingInvoices")} />
          ) : (
            <>
              <ConsultationInvoiceTable
                invoices={filteredInvoices}
                showPatientColumn={permissions.canViewAllInvoices}
                onPayInvoice={handlePayInvoice}
                onDownloadPDF={handleDownloadPDF}
                onViewDetails={handleViewDetails}
              />

              {filteredCount === 0 && totalCount > 0 && (
                <EmptyState
                  title={t("noInvoicesFound")}
                  description={t("noMatchingInvoices")}
                  action={{
                    label: tCommon("clearSearch"),
                    onClick: () => {
                      setFilters({
                        status: "",
                        dateRange: { from: undefined, to: undefined },
                        amountRange: { min: "", max: "" },
                        searchTerm: "",
                      });
                      fetchInvoices();
                    },
                  }}
                />
              )}

            </>
          )}
        </CardContent>
        <CardFooter className="border-t pt-4 flex flex-col sm:flex-row justify-between gap-4">
          <div className="text-sm text-muted-foreground">
            {isLoading ? (
              <span> </span>
            ) : (
              `${t("showing")} ${filteredInvoices.length} ${t(
                "of"
              )} ${filteredCount} ${t("invoice(s)")}`
            )}
          </div>

          {totalPages > 1 && (
            <PaginationControls
              currentPage={currentPage}
              totalPages={totalPages}
              onPageChange={setCurrentPage}
            />
          )}

          {/* <div className="flex items-center gap-2 ml-auto">
            {permissions.canViewAllInvoices && (
              <Button variant="outline" size="sm" className="btn-press">
                <Calendar className="mr-1 h-4 w-4" aria-hidden="true" />
                {t("paymentHistory")}
              </Button>
            )}
          </div> */}
        </CardFooter>
      </Card>

      {/* Payment Form */}
      {invoiceToPay && (
        <ConsultationPaymentForm
          isOpen={!!invoiceToPay}
          onClose={closePaymentForm}
          onSubmit={handlePaymentSubmit}
          invoice={invoiceToPay}
        />
      )}

      {/* Invoice Details Modal */}
      {invoiceToView && (
        <InvoiceDetailsModal
          invoice={invoiceToView}
          isOpen={isDetailsModalOpen}
          onClose={closeDetailsModal}
          onPayInvoice={() => setInvoiceToPay(invoiceToView)}
          onDownloadPDF={handleDownloadPDF}
          showPayButton={permissions.canPayInvoices}
        />
      )}
    </div>
  );
}

export default BillingPage;
