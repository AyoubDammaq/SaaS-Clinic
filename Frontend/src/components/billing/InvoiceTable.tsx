import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Button } from "@/components/ui/button";
import { CreditCard, Download, FileText } from "lucide-react";
import { useState } from "react";
import { InvoiceStatusBadge } from "./InvoiceStatusBadge";
import { Facture, FactureStatus, mapFactureStatus } from "@/types/billing";
import { usePatients } from "@/hooks/usePatients";

interface InvoiceTableProps {
  invoices: Facture[];
  showPatientColumn: boolean;
  onPayNow: (invoice: Facture) => void;
  onDownload: (invoice: Facture) => void;
}

export function InvoiceTable({
  invoices,
  showPatientColumn,
  onPayNow,
  onDownload,
}: InvoiceTableProps) {
  const [hoveredRow, setHoveredRow] = useState<string | null>(null);
  const { patients, isLoading } = usePatients();

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
        <h3 className="text-lg font-medium">No invoices found</h3>
        <p className="text-sm text-muted-foreground mt-1 mb-4">
          You have no invoices at this time.
        </p>
      </div>
    );
  }

  if (isLoading) {
    return (
      <div className="flex justify-center items-center py-12">
        <p className="text-muted-foreground">Chargement des patients...</p>
      </div>
    );
  }

  if (patients.length === 0) {
    return (
      <div className="flex justify-center items-center py-12">
        <p className="text-muted-foreground">Aucun patient disponible</p>
      </div>
    );
  }

  // Fonction pour retrouver un patient par ID
  const getPatientName = (patientId: string) => {
    const patient = patients.find((p) => p.id === patientId);
    return patient ? `${patient.nom} ${patient.prenom}` : "Patient inconnu";
  };

  return (
    <div className="rounded-md border">
      <Table>
        <TableHeader>
          <TableRow>
            {showPatientColumn && <TableHead>Patient</TableHead>}
            <TableHead>Date Issued</TableHead>
            <TableHead>Total Amount</TableHead>
            <TableHead>Amount Paid</TableHead>
            <TableHead>Status</TableHead>
            <TableHead>Actions</TableHead>
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
              {showPatientColumn && (
                <TableCell>{getPatientName(invoice.patientId)}</TableCell>
              )}

              <TableCell>
                {new Date(invoice.dateEmission).toLocaleDateString()}
              </TableCell>

              <TableCell className="font-medium">
                $
                {invoice.montantTotal !== undefined &&
                invoice.montantTotal !== null
                  ? invoice.montantTotal.toFixed(2)
                  : "0.00"}
              </TableCell>

              <TableCell>
                $
                {invoice.montantPaye !== undefined &&
                invoice.montantPaye !== null
                  ? invoice.montantPaye.toFixed(2)
                  : "0.00"}
              </TableCell>

              <TableCell>
                <InvoiceStatusBadge status={mapFactureStatus(invoice.status)} />
              </TableCell>

              <TableCell>
                <div className="flex items-center gap-2">
                  {invoice.status !== FactureStatus.PAYEE && (
                    <Button
                      size="sm"
                      onClick={() => onPayNow(invoice)}
                      aria-label={`Pay invoice ${invoice.id}`}
                    >
                      <CreditCard className="mr-1 h-4 w-4" aria-hidden="true" />
                      Pay Now
                    </Button>
                  )}
                  <Button
                    size="icon"
                    variant="ghost"
                    onClick={() => onDownload(invoice)}
                    aria-label={`Download invoice ${invoice.id}`}
                  >
                    <Download className="h-4 w-4" aria-hidden="true" />
                  </Button>
                </div>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </div>
  );
}
