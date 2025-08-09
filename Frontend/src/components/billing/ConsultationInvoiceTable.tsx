import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Button } from "@/components/ui/button";
import { CreditCard, Download, Eye, FileText } from "lucide-react";
import { useMemo, useState } from "react";
import { InvoiceStatusBadge } from "./InvoiceStatusBadge";
import { Facture, FactureStatus, mapFactureStatus } from "@/types/billing";
import { format } from "date-fns";
import { fr } from "date-fns/locale";
import { usePatients } from "@/hooks/usePatients";
import { useTranslation } from "@/hooks/useTranslation";
import { useAuth } from "@/hooks/useAuth";

interface ConsultationInvoiceTableProps {
  invoices: Facture[];
  showPatientColumn?: boolean;
  onPayInvoice: (invoice: Facture) => void;
  onDownloadPDF: (invoiceId: string) => void;
  onViewDetails?: (invoice: Facture) => void;
}

export function ConsultationInvoiceTable({
  invoices,
  showPatientColumn = false,
  onPayInvoice,
  onDownloadPDF,
  onViewDetails,
}: ConsultationInvoiceTableProps) {
  const { t } = useTranslation("billing");
  const tCommon = useTranslation("common").t;
  const [hoveredRow, setHoveredRow] = useState<string | null>(null);
  const { patients } = usePatients();
  const { user } = useAuth();

  const showActionsColumn =
    user?.role !== "Doctor" && user?.role !== "SuperAdmin";

  const patientMap = useMemo(() => {
    const map = new Map<string, string>();
    patients.forEach((p) => map.set(p.id, `${p.prenom} ${p.nom}`));
    return map;
  }, [patients]);

  if (invoices.length === 0) {
    return (
      <div
        className="flex flex-col items-center justify-center py-12 text-center"
        role="alert"
        aria-live="polite"
      >
        <FileText
          className="h-12 w-12 text-muted-foreground mb-4"
          aria-hidden="true"
        />
        <h3 className="text-lg font-medium">{t("noInvoicesFound")}</h3>
        <p className="text-sm text-muted-foreground mt-1">
          {t("noInvoicesYet")}
        </p>
      </div>
    );
  }

  const formatCurrency = (amount: number, currency: string = "EUR") => {
    return new Intl.NumberFormat("fr-FR", {
      style: "currency",
      currency: currency,
    }).format(amount);
  };

  const formatDate = (dateString: string) => {
    return format(new Date(dateString), "dd/MM/yyyy", { locale: fr });
  };

  const calculateDueDate = (emissionDate: string) => {
    const date = new Date(emissionDate);
    date.setDate(date.getDate() + 30);
    return format(date, "dd/MM/yyyy", { locale: fr });
  };

  return (
    <div className="rounded-md border">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>{t("invoiceNumber")}</TableHead>
            {showPatientColumn && <TableHead>{t("patient")}</TableHead>}
            <TableHead>{t("issueDate")}</TableHead>
            <TableHead>{t("dueDate")}</TableHead>
            <TableHead>{t("totalAmount")}</TableHead>
            <TableHead>{t("amountPaid")}</TableHead>
            <TableHead>{t("status")}</TableHead>
            {showActionsColumn && <TableHead>{t("actions")}</TableHead>}
          </TableRow>
        </TableHeader>
        <TableBody>
          {invoices.map((invoice) => (
            <TableRow
              key={invoice.id}
              onMouseEnter={() => setHoveredRow(invoice.id)}
              onMouseLeave={() => setHoveredRow(null)}
              className={hoveredRow === invoice.id ? "bg-muted/50" : ""}
            >
              <TableCell className="font-medium">
                {typeof invoice.id === "string"
                  ? invoice.id.substring(0, 8).toUpperCase()
                  : t("unknownId")}
              </TableCell>
              {showPatientColumn && (
                <TableCell>
                  {patientMap.get(invoice.patientId) || t("unknownPatient")}
                </TableCell>
              )}
              <TableCell>{formatDate(invoice.dateEmission)}</TableCell>
              <TableCell>{calculateDueDate(invoice.dateEmission)}</TableCell>
              <TableCell className="font-medium">
                {formatCurrency(invoice.montantTotal)}
              </TableCell>
              <TableCell>{formatCurrency(invoice.montantPaye)}</TableCell>
              <TableCell>
                <InvoiceStatusBadge status={mapFactureStatus(invoice.status)} />
              </TableCell>
              {showActionsColumn && (
                <TableCell>
                  <div className="flex items-center gap-2">
                    {(mapFactureStatus(invoice.status) ===
                      FactureStatus.IMPAYEE ||
                      mapFactureStatus(invoice.status) ===
                        FactureStatus.PARTIELLEMENT_PAYEE) && (
                      <Button
                        size="sm"
                        onClick={(event) => {
                          event.stopPropagation();
                          onPayInvoice(invoice);
                        }}
                        aria-label={`${t("payInvoice")} ${invoice.id}`}
                      >
                        <CreditCard
                          className="mr-1 h-4 w-4"
                          aria-hidden="true"
                        />
                        {t("pay")}
                      </Button>
                    )}
                    <Button
                      size="icon"
                      variant="ghost"
                      onClick={() => onDownloadPDF(invoice.id)}
                      aria-label={`${t("downloadPDF")} ${invoice.id}`}
                    >
                      <Download className="h-4 w-4" aria-hidden="true" />
                    </Button>
                    {onViewDetails && (
                      <Button
                        size="icon"
                        variant="ghost"
                        onClick={(event) => {
                          event.stopPropagation();
                          onViewDetails(invoice);
                        }}
                        aria-label={`${t("viewDetails")} ${invoice.id}`}
                      >
                        <Eye className="h-4 w-4" aria-hidden="true" />
                      </Button>
                    )}
                  </div>
                </TableCell>
              )}
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </div>
  );
}
