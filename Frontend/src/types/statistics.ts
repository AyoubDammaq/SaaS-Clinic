// statistics.ts

export interface ActiviteMedecinDTO {
  medecinId: string;
  nomComplet: string;
  nombreConsultations: number;
  nombreRendezVous: number;
}

export interface ComparaisonCliniqueDTO {
  cliniqueId: string;
  nom: string;
  nombreMedecins: number;
  nombrePatients: number;
  nombreConsultations: number;
  nombreRendezVous: number;
}

export interface ConsultationStatsDto {
  totalConsultations: number;
  consultationsParJour: Record<string, number>; // or { [key: string]: number }
}

export interface DashboardStatsDTO {
  consultationsJour: number;
  totalPatients: number;
  nouveauxPatients: number;
  nouveauxPatientsParMois: StatistiqueDTO[];
  nombreFactures: number;
  totalFacturesPayees: number;
  totalFacturesImpayees: number;
  paiementsPayes: number;
  paiementsImpayes: number;
  paiementsEnAttente: number;
  RendezvousStats?: StatistiqueDTO[];
  doctorsBySpecialty?: StatistiqueDTO[];
  totalClinics: number;
  nouvellesCliniques: number;
  totalMedecins: number;
  nouveauxMedecins: number;
  WeeklyAppointmentStatsByDoctor?: AppointmentDayStat[];
  WeeklyAppointmentStatsByClinic?: AppointmentDayStat[];
}

export interface DoctorStatsDTO {
  cle: string;
  nombre: number;
}

export interface FactureStatsDTO {
  cle: string;
  nombre: number;
}

export interface RendezVousStatDTO {
  date: string; // ISO format (e.g., "2025-07-29")
  totalRendezVous: number;
  confirmes: number;
  annules: number;
  enAttente: number;
}

export interface StatistiqueCliniqueDTO {
  cliniqueId: string;
  nom: string;
  nombreMedecins: number;
  nombreRendezVous: number;
  nombreConsultations: number;
  nombrePatients: number;
}

export interface StatistiqueDTO {
  cle: string;
  nombre: number;
}

export interface StatistiquesFactureDto {
  nombreTotal: number;
  nombrePayees: number;
  nombreImpayees: number;
  nombrePartiellementPayees: number;
  montantTotal: number;
  montantTotalPaye: number;
  nombreParClinique: Record<string, number>; // Key is CliniqueId (GUID), value is number
}

export interface AppointmentDayStat {
  jour: string; // ex: "Lundi", "Mardi", etc.
  scheduled: number;
  pending: number;
  cancelled: number;
}

export interface RecentActivity {
  id: string;
  type: "appointment";
  title: string;
  description: string;
  time: string;
  seen: boolean;
}

export type BarChartData = {
  name: string;
  value: number;
}[];

export interface RevenuTrend {
  current: number;
  previous: number;
  percentageChange: number;
  isPositive: boolean;
}
