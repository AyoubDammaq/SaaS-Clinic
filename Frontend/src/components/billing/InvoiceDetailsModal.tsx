import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Separator } from "@/components/ui/separator";
import { Card, CardContent } from "@/components/ui/card";
import {
  Download,
  CreditCard,
  Calendar,
  User,
  FileText,
  Euro,
  Clock,
  CheckCircle,
  Hospital,
} from "lucide-react";
import {
  Facture,
  FactureStatus,
  mapFactureStatus,
  ModePaiement,
} from "@/types/billing";
import { InvoiceStatusBadge } from "./InvoiceStatusBadge";
import { format } from "date-fns";
import { fr } from "date-fns/locale";
import { useDoctors } from "@/hooks/useDoctors";
import { usePatients } from "@/hooks/usePatients";
import { useCliniques } from "@/hooks/useCliniques";
import { useConsultations } from "@/hooks/useConsultations";
import { useMemo, useEffect } from "react";
import {
  Consultation,
  ConsultationType,
  consultationTypes,
} from "@/types/consultation";
import { Doctor } from "@/types/doctor";
import { useTranslation } from "@/hooks/useTranslation";

interface InvoiceDetailsModalProps {
  invoice: Facture | null;
  isOpen: boolean;
  onClose: () => void;
  onPayInvoice?: (invoice: Facture) => void;
  onDownloadPDF?: (invoiceId: string) => void;
  showPayButton?: boolean;
}

export function InvoiceDetailsModal({
  invoice,
  isOpen,
  onClose,
  onPayInvoice,
  onDownloadPDF,
  showPayButton = true,
}: InvoiceDetailsModalProps) {
  const { t } = useTranslation("billing");
  const tCommon = useTranslation("common").t;
  const { doctors, isLoading: doctorsLoading } = useDoctors();
  const { patients, isLoading: patientsLoading } = usePatients();
  const { cliniques, isLoading: clinicsLoading } = useCliniques();
  const { consultations, isLoading: consultationsLoading } = useConsultations();

  const isLoading =
    doctorsLoading || patientsLoading || clinicsLoading || consultationsLoading;

  const patientMap = useMemo(() => {
    const map = new Map<string, string>();
    patients.forEach((p) => map.set(p.id, `${p.prenom} ${p.nom}`));
    return map;
  }, [patients]);

  const clinicMap = useMemo(() => {
    const map = new Map<string, string>();
    cliniques.forEach((c) => map.set(c.id, c.nom));
    return map;
  }, [cliniques]);

  const consultationMap = useMemo(() => {
    const map = new Map<string, Consultation>();
    consultations.forEach((c) => map.set(c.id, c));
    return map;
  }, [consultations]);

  const medecinMap = useMemo(() => {
    const map = new Map<string, string>();
    doctors.forEach((m: Doctor) => map.set(m.id, `${m.prenom} ${m.nom}`));
    return map;
  }, [doctors]);

  useEffect(() => {
    if (isOpen && !isLoading) {
      console.log("Consultation Map:", consultationMap);
      console.log(
        "Invoice:",
        invoice,
        "Consultation:",
        consultationMap.get(invoice?.consultationId)
      );
    }
  }, [isOpen, isLoading, consultationMap, invoice]);

  if (!invoice || isLoading) return null;

  const formatCurrency = (amount: number, currency = "EUR") =>
    new Intl.NumberFormat("fr-FR", {
      style: "currency",
      currency,
    }).format(amount);

  const formatDate = (dateString: string) =>
    format(new Date(dateString), "dd MMMM yyyy", { locale: fr });

  const formatDateTime = (dateString: string) =>
    format(new Date(dateString), "dd MMMM yyyy à HH:mm", { locale: fr });

  const getStatusLabel = (status: FactureStatus) => {
    switch (status) {
      case FactureStatus.PAYEE:
        return t("paid");
      case FactureStatus.PARTIELLEMENT_PAYEE:
        return t("partiallyPaid");
      case FactureStatus.IMPAYEE:
        return t("unpaid");
      case FactureStatus.ANNULEE:
        return t("cancelled");
      default:
        return status;
    }
  };

  const getStatusColor = (status: FactureStatus) => {
    switch (status) {
      case FactureStatus.PAYEE:
        return "text-green-600";
      case FactureStatus.PARTIELLEMENT_PAYEE:
        return "text-yellow-600";
      case FactureStatus.IMPAYEE:
        return "text-red-600";
      case FactureStatus.ANNULEE:
        return "text-gray-600";
      default:
        return "text-gray-600";
    }
  };

  const getStatusIcon = (status: FactureStatus) => {
    switch (status) {
      case FactureStatus.PAYEE:
        return <CheckCircle className="h-5 w-5 text-green-600" />;
      case FactureStatus.PARTIELLEMENT_PAYEE:
        return <Clock className="h-5 w-5 text-yellow-600" />;
      case FactureStatus.IMPAYEE:
        return <Clock className="h-5 w-5 text-red-600" />;
      case FactureStatus.ANNULEE:
        return <FileText className="h-5 w-5 text-gray-600" />;
      default:
        return <FileText className="h-5 w-5 text-gray-600" />;
    }
  };

  const mappedStatus = mapFactureStatus(invoice.status);
  const consultation = consultationMap.get(invoice.consultationId);
  const medecinName = consultation
    ? medecinMap.get(consultation.medecinId) || t("unknownDoctor")
    : t("unknownDoctor");

  const getConsultationTypeLabel = (
    type: ConsultationType | undefined
  ): string => {
    if (!type) return t("unknownType");
    switch (type) {
      case ConsultationType.ConsultationGenerale:
        return t("consultationType.general");
      case ConsultationType.ConsultationSpecialiste:
        return t("consultationType.specialist");
      case ConsultationType.ConsultationUrgence:
        return t("consultationType.emergency");
      case ConsultationType.ConsultationSuivi:
        return t("consultationType.followUp");
      case ConsultationType.ConsultationLaboratoire:
        return t("consultationType.laboratory");
      default:
        return `${t("unknownType")} (${type})`;
    }
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <FileText className="h-5 w-5" />
            {t("invoice")} #{invoice.id.substring(0, 8).toUpperCase()}
          </DialogTitle>
          <DialogDescription>
            {t("invoiceDetailsDescription")}
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-6">
          <Card className="bg-muted/30">
            <CardContent className="pt-6">
              <div className="flex justify-between items-start mb-4">
                <div className="space-y-2">
                  <div className="flex items-center gap-2">
                    {getStatusIcon(mappedStatus)}
                    <span
                      className={`font-medium ${getStatusColor(mappedStatus)}`}
                    >
                      {getStatusLabel(mappedStatus)}
                    </span>
                    <InvoiceStatusBadge status={mappedStatus} />
                  </div>
                </div>
                <div className="text-right">
                  <div className="text-3xl font-bold">
                    {formatCurrency(invoice.montantTotal)}
                  </div>
                  <div className="text-sm text-muted-foreground">
                    {t("totalAmount")}
                  </div>
                </div>
              </div>
            </CardContent>
          </Card>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <Card>
              <CardContent className="pt-4">
                <div className="space-y-3">
                  <div className="flex items-center gap-2 text-sm font-medium">
                    <User className="h-4 w-4" />
                    {t("patientInfo")}
                  </div>
                  <div className="space-y-1">
                    <div className="text-sm text-muted-foreground">
                      {t("patient")}
                    </div>
                    <div className="font-medium">
                      {patientMap.get(invoice.patientId) || t("unknownPatient")}
                    </div>
                  </div>
                  <div className="space-y-1">
                    <div className="text-sm text-muted-foreground">
                      {t("consultationDate")}
                    </div>
                    <div className="font-medium">
                      {consultation
                        ? formatDate(consultation.dateConsultation)
                        : t("unknownDate")}
                    </div>
                  </div>
                  <div className="space-y-1">
                    <div className="flex items-center gap-2 text-sm font-medium">
                      <Hospital className="h-4 w-4" />
                      {t("clinic")}
                    </div>
                    <div className="font-medium">
                      {clinicMap.get(invoice.clinicId) || t("unknownClinic")}
                    </div>
                  </div>
                </div>
              </CardContent>
            </Card>

            <Card>
              <CardContent className="pt-4">
                <div className="space-y-3">
                  <div className="flex items-center gap-2 text-sm font-medium">
                    <Calendar className="h-4 w-4" />
                    {t("importantDates")}
                  </div>
                  <div className="space-y-1">
                    <div className="text-sm text-muted-foreground">
                      {t("issueDate")}
                    </div>
                    <div className="font-medium">
                      {formatDate(invoice.dateEmission)}
                    </div>
                  </div>
                  {invoice.paiement?.datePaiement && (
                    <div className="space-y-1">
                      <div className="text-sm text-muted-foreground">
                        {t("paymentDate")}
                      </div>
                      <div className="font-medium text-green-600">
                        {formatDateTime(invoice.paiement.datePaiement)}
                      </div>
                    </div>
                  )}
                </div>
              </CardContent>
            </Card>
          </div>

          {consultation && (
            <Card>
              <CardContent className="pt-4">
                <div className="space-y-3">
                  <div className="flex items-center gap-2 text-sm font-medium">
                    <FileText className="h-4 w-4" />
                    {t("consultationDetails")}
                  </div>
                  <div className="space-y-1">
                    <div className="font-medium">
                      {`${getConsultationTypeLabel(
                        consultation.type
                      )} - ${medecinName}`}
                    </div>
                  </div>
                </div>
              </CardContent>
            </Card>
          )}

          {invoice.paiement && (
            <Card>
              <CardContent className="pt-4">
                <div className="space-y-3">
                  <div className="flex items-center gap-2 text-sm font-medium">
                    <CreditCard className="h-4 w-4" />
                    {t("paymentInfo")}
                  </div>
                  <div className="space-y-1">
                    <div className="text-sm text-muted-foreground">
                      {t("amountPaid")}
                    </div>
                    <div className="font-medium">
                      {formatCurrency(invoice.paiement.montant)}
                    </div>
                  </div>
                  <div className="space-y-1">
                    <div className="text-sm text-muted-foreground">
                      {t("paymentMethod")}
                    </div>
                    <div className="font-medium">
                      {invoice.paiement.mode === ModePaiement.CarteBancaire &&
                        t("creditCard")}
                      {invoice.paiement.mode === ModePaiement.Virement &&
                        t("bankTransfer")}
                      {invoice.paiement.mode === ModePaiement.Especes &&
                        t("cashOnSite")}
                      {invoice.paiement.mode === ModePaiement.Chèque &&
                        t("check")}
                      {invoice.paiement.mode === ModePaiement.Mobile &&
                        t("mobilePayment")}
                    </div>
                  </div>
                </div>
              </CardContent>
            </Card>
          )}

          <Separator />

          <div className="flex flex-col sm:flex-row gap-3 justify-end">
            <Button
              variant="outline"
              onClick={() => onDownloadPDF?.(invoice.id)}
              className="flex items-center gap-2"
            >
              <Download className="h-4 w-4" />
              {t("downloadPDF")}
            </Button>

            {showPayButton &&
              invoice.status === FactureStatus.IMPAYEE &&
              onPayInvoice && (
                <Button
                  onClick={() => onPayInvoice(invoice)}
                  className="flex items-center gap-2"
                >
                  <CreditCard className="h-4 w-4" />
                  {t("payInvoice")}
                </Button>
              )}

            <Button variant="secondary" onClick={onClose}>
              {tCommon("close")}
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
}
