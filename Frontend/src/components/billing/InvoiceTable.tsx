
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { CreditCard, Download, FileText } from "lucide-react";
import { useState } from "react";
import { InvoiceStatusBadge } from "./InvoiceStatusBadge";
import { Invoice } from "@/types/billing";

interface InvoiceTableProps {
  invoices: Invoice[];
  showPatientColumn: boolean;
  onPayNow: (invoice: Invoice) => void;
  onDownload: (invoice: Invoice) => void;
}

export function InvoiceTable({ invoices, showPatientColumn, onPayNow, onDownload }: InvoiceTableProps) {
  const [hoveredRow, setHoveredRow] = useState<string | null>(null);

  if (invoices.length === 0) {
    return (
      <div className="flex flex-col items-center justify-center py-12 text-center" role="alert" aria-live="polite">
        <FileText className="h-12 w-12 text-muted-foreground mb-4" aria-hidden="true" />
        <h3 className="text-lg font-medium">No invoices found</h3>
        <p className="text-sm text-muted-foreground mt-1 mb-4">
          You have no invoices at this time.
        </p>
      </div>
    );
  }

  return (
    <div className="rounded-md border">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>Invoice #</TableHead>
            {showPatientColumn && <TableHead>Patient</TableHead>}
            <TableHead>Date</TableHead>
            <TableHead>Due Date</TableHead>
            <TableHead>Description</TableHead>
            <TableHead>Amount</TableHead>
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
              <TableCell className="font-medium">{invoice.id}</TableCell>
              {showPatientColumn && <TableCell>{invoice.patient}</TableCell>}
              <TableCell>{invoice.date}</TableCell>
              <TableCell>{invoice.dueDate}</TableCell>
              <TableCell>{invoice.description}</TableCell>
              <TableCell className="font-medium">${invoice.amount.toFixed(2)}</TableCell>
              <TableCell>
                <InvoiceStatusBadge status={invoice.status} />
              </TableCell>
              <TableCell>
                <div className="flex items-center gap-2">
                  {invoice.status !== 'Paid' && (
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
