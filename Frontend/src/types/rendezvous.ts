export enum AppointmentStatusEnum {
  CONFIRME = 0,
  ANNULE = 1,
  EN_ATTENTE = 2,
}

export interface RendezVous {
  id: string;
  patientId: string;
  medecinId: string;
  dateHeure: string;
  statut: AppointmentStatusEnum;
  commentaire?: string;
  justificationAnnulation?: string;
  patientNom?: string;
  medecinNom?: string;
}

export interface CreateRendezVousRequest {
  patientId: string;
  medecinId: string;
  dateHeure: string;
  commentaire?: string;
}

export interface UpdateRendezVousRequest {
  patientId: string;
  medecinId: string;
  dateHeure: string;
  commentaire?: string;
}

export interface AnnulerMedecinRequest {
  justification: string;
}

export interface RendezVousStats {
  totalRdv: number;
  rdvParMedecin: { medecinId: string; medecinNom: string; count: number }[];
  patientsUniques: number;
}

export type AppointmentFormData = {
  patientId: string;
  medecinId: string;
  dateHeure: string;
  time?: string;
  reason?: string;
};

export type DoctorType = {
  id: string;
  name: string;
};

export type PatientType = {
  id: string;
  name: string;
};

export type ClinicType = {
  id: string;
  name: string;
};
