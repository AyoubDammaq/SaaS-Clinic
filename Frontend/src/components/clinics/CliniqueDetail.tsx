//CliniqueDetail.tsx
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { ArrowLeft } from "lucide-react";
import {
  Clinique,
  StatistiqueCliniqueDTO,
  StatistiqueDTO,
  TypeClinique,
  StatutClinique,
} from "@/types/clinic";
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
} from "recharts";
import { useTranslation } from "@/hooks/useTranslation";
import { useAuth } from "@/hooks/useAuth";

interface CliniqueDetailProps {
  clinique: Clinique;
  statistics: StatistiqueCliniqueDTO | null;
  isLoadingStats: boolean;
  onBack: () => void;
}

export function CliniqueDetail({
  clinique,
  statistics,
  isLoadingStats,
  onBack,
}: CliniqueDetailProps) {
  const { t } = useTranslation("clinics");
  const { user } = useAuth();

  if (!clinique) return null;

  // Helper to get enum label from value
  const getTypeCliniqueLabel = (value: number) => {
    const key =
      TypeClinique[
        value as unknown as keyof typeof TypeClinique as string
      ]?.toLowerCase();
    return t(key) || t("unknown");
  };

  const getStatutCliniqueLabel = (value: number) => {
    const key =
      StatutClinique[
        value as unknown as keyof typeof StatutClinique as string
      ]?.toLowerCase();
    return t(key) || t("unknown");
  };

  // Transforme le mois en number pour Recharts
  const formatStatsForChart = (
    data: Record<number, number> | null | undefined
  ) => {
    const result = [];
    for (let month = 1; month <= 12; month++) {
      result.push({
        mois: month,
        valeur: data?.[month] ?? 0, // si pas de valeur, mettre 0
      });
    }
    return result;
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h2 className="text-2xl font-bold">{clinique.nom}</h2>
        {user?.role !== "Doctor" && (
          <Button variant="outline" size="sm" onClick={onBack}>
            <ArrowLeft className="mr-2 h-4 w-4" />
            {t("back")}
          </Button>
        )}
      </div>

      <Card>
        <CardHeader>
          <CardTitle>{t("general_information")}</CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <p className="text-sm font-semibold text-muted-foreground">
                {t("address")}
              </p>
              <p>{clinique.adresse}</p>
            </div>
            <div>
              <p className="text-sm font-semibold text-muted-foreground">
                {t("contact")}
              </p>
              <p>
                {t("phone")}: {clinique.numeroTelephone}
              </p>
              <p>
                {t("email")}: {clinique.email}
              </p>
              {clinique.siteWeb && (
                <p>
                  {t("website")}:{" "}
                  <a
                    href={clinique.siteWeb}
                    target="_blank"
                    rel="noopener noreferrer"
                    className="underline text-blue-600"
                  >
                    {clinique.siteWeb}
                  </a>
                </p>
              )}
            </div>
            {clinique.description && (
              <div className="col-span-1 md:col-span-2">
                <p className="text-sm font-semibold text-muted-foreground">
                  {t("description")}
                </p>
                <p>{clinique.description}</p>
              </div>
            )}
            <div>
              <p className="text-sm font-semibold text-muted-foreground">
                {t("type")}
              </p>
              <p>{getTypeCliniqueLabel(clinique.typeClinique)}</p>
            </div>
            <div>
              <p className="text-sm font-semibold text-muted-foreground">
                {t("status")}
              </p>
              <p>{getStatutCliniqueLabel(clinique.statut)}</p>
            </div>
            <div>
              <p className="text-sm font-semibold text-muted-foreground">
                {t("creation_date")}
              </p>
              <p>{clinique.dateCreation}</p>
            </div>
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>{t("statistics")}</CardTitle>
        </CardHeader>
        <CardContent>
          {isLoadingStats ? (
            <div className="h-40 flex items-center justify-center">
              <p className="text-muted-foreground">{t("loading_statistics")}</p>
            </div>
          ) : statistics ? (
            <div className="space-y-6">
              <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
                <Card>
                  <CardContent className="pt-6">
                    <div className="text-center">
                      <p className="text-lg font-semibold">
                        {statistics.nombreMedecins}
                      </p>
                      <p className="text-sm text-muted-foreground">
                        {t("doctors")}
                      </p>
                    </div>
                  </CardContent>
                </Card>
                <Card>
                  <CardContent className="pt-6">
                    <div className="text-center">
                      <p className="text-lg font-semibold">
                        {statistics.nombrePatients}
                      </p>
                      <p className="text-sm text-muted-foreground">
                        {t("patients")}
                      </p>
                    </div>
                  </CardContent>
                </Card>
                <Card>
                  <CardContent className="pt-6">
                    <div className="text-center">
                      <p className="text-lg font-semibold">
                        {statistics.nombreConsultations}
                      </p>
                      <p className="text-sm text-muted-foreground">
                        {t("consultations")}
                      </p>
                    </div>
                  </CardContent>
                </Card>
              </div>

              <div>
                <h3 className="text-lg font-semibold mb-3">
                  {t("consultations_per_month")}
                </h3>
                <div className="h-64 w-full">
                  <ResponsiveContainer width="100%" height="100%">
                    <BarChart
                      data={formatStatsForChart(
                        statistics.nombreConsultationsParMois
                      )}
                    >
                      <CartesianGrid strokeDasharray="3 3" />
                      <XAxis dataKey="mois" />
                      <YAxis />
                      <Tooltip />
                      <Legend />
                      <Bar
                        dataKey="valeur"
                        name="Consultations"
                        fill="#0ea5e9"
                      />
                    </BarChart>
                  </ResponsiveContainer>
                </div>
              </div>

              <div>
                <h3 className="text-lg font-semibold mb-3">
                  {t("new_patients_per_month")}
                </h3>
                <div className="h-64 w-full">
                  <ResponsiveContainer width="100%" height="100%">
                    <BarChart
                      data={formatStatsForChart(
                        statistics.nombreNouveauxPatientsParMois
                      )}
                    >
                      <CartesianGrid strokeDasharray="3 3" />
                      <XAxis dataKey="mois" />
                      <YAxis />
                      <Tooltip />
                      <Legend />
                      <Bar
                        dataKey="valeur"
                        name="Nouveaux patients"
                        fill="#22c55e"
                      />
                    </BarChart>
                  </ResponsiveContainer>
                </div>
              </div>

              <div>
                <h3 className="text-lg font-semibold mb-3">
                  {t("revenue_per_month")} (€)
                </h3>
                <div className="h-64 w-full">
                  <ResponsiveContainer width="100%" height="100%">
                    <BarChart
                      data={formatStatsForChart(statistics.revenusParMois)}
                    >
                      <CartesianGrid strokeDasharray="3 3" />
                      <XAxis dataKey="mois" />
                      <YAxis />
                      <Tooltip />
                      <Legend />
                      <Bar dataKey="valeur" name="Revenus" fill="#7c3aed" />
                    </BarChart>
                  </ResponsiveContainer>
                </div>
              </div>
            </div>
          ) : (
            <div className="h-40 flex flex-col items-center justify-center">
              <p className="text-muted-foreground mb-4">
                {t("statistics_unavailable")}
              </p>
              <Button variant="outline">{t("load_statistics")}</Button>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
