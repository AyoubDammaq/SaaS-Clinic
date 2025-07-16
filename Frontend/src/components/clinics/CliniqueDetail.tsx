import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { ArrowLeft } from 'lucide-react';
import { Clinique, StatistiqueCliniqueDTO, TypeClinique, StatutClinique } from '@/types/clinic';
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
} from 'recharts';

interface CliniqueDetailProps {
  clinique: Clinique;
  statistics: StatistiqueCliniqueDTO | null;
  isLoadingStats: boolean;
  onBack: () => void;
}

export function CliniqueDetail({ clinique, statistics, isLoadingStats, onBack }: CliniqueDetailProps) {
  if (!clinique) return null;

    // Helper to get enum label from value
  const getTypeCliniqueLabel = (value: number) =>
    TypeClinique[value as unknown as keyof typeof TypeClinique];
  const getStatutCliniqueLabel = (value: number) =>
    StatutClinique[value as unknown as keyof typeof StatutClinique];

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h2 className="text-2xl font-bold">{clinique.nom}</h2>
        <Button variant="outline" size="sm" onClick={onBack}>
          <ArrowLeft className="mr-2 h-4 w-4" />
          Retour
        </Button>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Informations générales</CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <p className="text-sm font-semibold text-muted-foreground">Adresse</p>
              <p>{clinique.adresse}</p>
            </div>
            <div>
              <p className="text-sm font-semibold text-muted-foreground">Contact</p>
              <p>Téléphone: {clinique.numeroTelephone}</p>
              <p>Email: {clinique.email}</p>
              {clinique.siteWeb && (
                <p>
                  Site web:{" "}
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
                <p className="text-sm font-semibold text-muted-foreground">Description</p>
                <p>{clinique.description}</p>
              </div>
            )}
            <div>
              <p className="text-sm font-semibold text-muted-foreground">Type</p>
              <p>{getTypeCliniqueLabel(clinique.typeClinique)}</p>
            </div>
            <div>
              <p className="text-sm font-semibold text-muted-foreground">Statut</p>
              <p>{getStatutCliniqueLabel(clinique.statut)}</p>
            </div>
            <div>
              <p className="text-sm font-semibold text-muted-foreground">Date de création</p>
              <p>{clinique.dateCreation}</p>
            </div>
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Statistiques</CardTitle>
        </CardHeader>
        <CardContent>
          {isLoadingStats ? (
            <div className="h-40 flex items-center justify-center">
              <p className="text-muted-foreground">Chargement des statistiques...</p>
            </div>
          ) : statistics ? (
            <div className="space-y-6">
              <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
                <Card>
                  <CardContent className="pt-6">
                    <div className="text-center">
                      <p className="text-lg font-semibold">{statistics.nombreDocteurs}</p>
                      <p className="text-sm text-muted-foreground">Médecins</p>
                    </div>
                  </CardContent>
                </Card>
                <Card>
                  <CardContent className="pt-6">
                    <div className="text-center">
                      <p className="text-lg font-semibold">{statistics.nombrePatients}</p>
                      <p className="text-sm text-muted-foreground">Patients</p>
                    </div>
                  </CardContent>
                </Card>
                <Card>
                  <CardContent className="pt-6">
                    <div className="text-center">
                      <p className="text-lg font-semibold">{statistics.nombreConsultations}</p>
                      <p className="text-sm text-muted-foreground">Consultations</p>
                    </div>
                  </CardContent>
                </Card>
              </div>

              <div>
                <h3 className="text-lg font-semibold mb-3">Consultations par mois</h3>
                <div className="h-64 w-full">
                  <ResponsiveContainer width="100%" height="100%">
                    <BarChart data={statistics.nombreConsultationsParMois}>
                      <CartesianGrid strokeDasharray="3 3" />
                      <XAxis dataKey="mois" />
                      <YAxis />
                      <Tooltip />
                      <Legend />
                      <Bar dataKey="valeur" name="Consultations" fill="#0ea5e9" />
                    </BarChart>
                  </ResponsiveContainer>
                </div>
              </div>

              <div>
                <h3 className="text-lg font-semibold mb-3">Nouveaux patients par mois</h3>
                <div className="h-64 w-full">
                  <ResponsiveContainer width="100%" height="100%">
                    <BarChart data={statistics.nombreNouveauxPatientsParMois}>
                      <CartesianGrid strokeDasharray="3 3" />
                      <XAxis dataKey="mois" />
                      <YAxis />
                      <Tooltip />
                      <Legend />
                      <Bar dataKey="valeur" name="Nouveaux patients" fill="#22c55e" />
                    </BarChart>
                  </ResponsiveContainer>
                </div>
              </div>

              <div>
                <h3 className="text-lg font-semibold mb-3">Revenus par mois (€)</h3>
                <div className="h-64 w-full">
                  <ResponsiveContainer width="100%" height="100%">
                    <BarChart data={statistics.revenusParMois}>
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
              <p className="text-muted-foreground mb-4">Les statistiques ne sont pas disponibles pour cette clinique</p>
              <Button variant="outline">Charger les statistiques</Button>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
