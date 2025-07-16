import { useState, useEffect } from "react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Eye, Calendar, User, Stethoscope } from "lucide-react";
import { format } from "date-fns";
import { fr } from "date-fns/locale";
import { Consultation } from "@/types/consultation";
import { consultationService } from "@/services/consultationService";
import { toast } from "sonner";

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
  const [consultations, setConsultations] = useState<Consultation[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const fetchConsultationHistory = async () => {
      if (!patientId) return;

      setIsLoading(true);
      try {
        const patientConsultations = await consultationService.getConsultationsByPatientId(patientId);

        const sortedConsultations = [...patientConsultations].sort((a, b) =>
          new Date(b.dateConsultation).getTime() - new Date(a.dateConsultation).getTime()
        );

        setConsultations(sortedConsultations);
      } catch (error) {
        console.error("Error fetching consultation history:", error);
        toast.error("Erreur lors du chargement de l'historique des consultations");
      } finally {
        setIsLoading(false);
      }
    };

    fetchConsultationHistory();
  }, [patientId]);

  const handleViewDetails = (consultation: Consultation) => {
    onViewConsultationDetails?.(consultation);
  };

  return (
    <Card>
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          <Stethoscope className="h-5 w-5" />
          🗂️ Historique des Consultations
          {!isLoading && consultations.length > 0 && ` (${consultations.length})`}
        </CardTitle>
      </CardHeader>

      <CardContent>
        {isLoading ? (
          <div className="flex justify-center items-center h-20">
            <p className="text-muted-foreground">Chargement...</p>
          </div>
        ) : consultations.length === 0 ? (
          <div className="flex justify-center items-center h-20">
            <p className="text-muted-foreground">Aucune consultation trouvée pour ce patient</p>
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
                        {format(new Date(consultation.dateConsultation), "dd MMM yyyy", { locale: fr })}
                      </div>
                    </div>

                    <div className="flex items-center gap-1 text-sm">
                      <User className="h-4 w-4 text-muted-foreground" />
                      <span className="font-medium">Dr. {fallbackDoctorName || "Non spécifié"}</span>
                    </div>

                    <div className="text-sm">
                      <span className="font-medium">Diagnostic :</span>{" "}
                      {consultation.diagnostic || "Non spécifié"}
                    </div>

                    {consultation.notes && (
                      <div className="text-sm text-muted-foreground">
                        <span className="font-medium">Notes :</span>{" "}
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
                  >
                    <Eye className="mr-1 h-4 w-4" />
                    Voir plus
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
