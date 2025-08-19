import { useState, useEffect } from "react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Eye, Calendar, User, Stethoscope } from "lucide-react";
import { format } from "date-fns";
import { fr, enUS } from "date-fns/locale";
import { Consultation } from "@/types/consultation";
import { consultationService } from "@/services/consultationService";
import { toast } from "sonner";
import { useTranslation } from "@/hooks/useTranslation";
import { debounce } from "lodash";

interface ConsultationHistoryProps {
  patientId: string;
  patientName: string;
  fallbackDoctorName: string;
  onViewConsultationDetails?: (consultation: Consultation) => void;
}

export function ConsultationHistory({
  patientId,
  fallbackDoctorName,
  onViewConsultationDetails,
}: ConsultationHistoryProps) {
  const { t, language } = useTranslation("patients");
  const [consultations, setConsultations] = useState<Consultation[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  // Sélectionner la locale en fonction de la langue active
  const locale = language === "fr" ? fr : enUS;

  useEffect(() => {
    if (!patientId) return;

    const debouncedFetch = debounce(async () => {
      setIsLoading(true);
      try {
        const patientConsultations =
          await consultationService.getConsultationsByPatientId(patientId);

        const sortedConsultations = [...patientConsultations].sort(
          (a, b) =>
            new Date(b.dateConsultation).getTime() -
            new Date(a.dateConsultation).getTime()
        );

        setConsultations(sortedConsultations);
      } catch (error) {
        console.error("Error fetching consultation history:", error);
        toast.error(t("errors.load_consultation_history_failed"));
      } finally {
        setIsLoading(false);
      }
    }, 500);

    debouncedFetch();

    return () => {
      debouncedFetch.cancel();
    };
  }, [patientId, t]);

  const handleViewDetails = (consultation: Consultation) => {
    onViewConsultationDetails?.(consultation);
  };

  return (
    <Card>
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          <Stethoscope className="h-5 w-5" />
          {t("consultation_history")}
          {!isLoading &&
            consultations.length > 0 &&
            ` (${consultations.length})`}
        </CardTitle>
      </CardHeader>

      <CardContent>
        {isLoading ? (
          <div className="flex justify-center items-center h-20">
            <p className="text-muted-foreground">{t("loading")}</p>
          </div>
        ) : consultations.length === 0 ? (
          <div className="flex justify-center items-center h-20">
            <p className="text-muted-foreground">
              {t("no_consultations_found")}
            </p>
          </div>
        ) : (
          <div className="space-y-4">
            {consultations.map((consultation) => (
              <div
                key={consultation.id}
                className="border rounded-lg p-4 hover:bg-muted/50 transition-colors"
              >
                <div className="flex items-start justify-between">
                  <div className="space-y-2 flex-1">
                    <div className="flex items-center gap-4 flex-wrap text-sm text-muted-foreground">
                      <div className="flex items-center gap-1">
                        <Calendar className="h-4 w-4" />
                        {format(
                          new Date(consultation.dateConsultation),
                          "dd MMM yyyy",
                          { locale }
                        )}
                      </div>
                    </div>

                    <div className="flex items-center gap-1 text-sm">
                      <User className="h-4 w-4 text-muted-foreground" />
                      <span className="font-medium">
                        {t("doctor")} {fallbackDoctorName || t("not_specified")}
                      </span>
                    </div>

                    <div className="text-sm">
                      <span className="font-medium">{t("diagnosis")}:</span>{" "}
                      {consultation.diagnostic || t("not_specified")}
                    </div>

                    {consultation.notes && (
                      <div className="text-sm text-muted-foreground">
                        <span className="font-medium">{t("notes")}:</span>{" "}
                        {consultation.notes.length > 100
                          ? `${consultation.notes.substring(0, 100)}...`
                          : consultation.notes}
                      </div>
                    )}
                  </div>

                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => handleViewDetails(consultation)}
                    className="ml-4"
                    aria-label={t("view_more")}
                  >
                    <Eye className="mr-1 h-4 w-4" />
                    {t("view_more")}
                  </Button>
                </div>
              </div>
            ))}
          </div>
        )}
      </CardContent>
    </Card>
  );
}
