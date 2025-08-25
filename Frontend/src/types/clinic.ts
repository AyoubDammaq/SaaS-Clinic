export enum TypeClinique {
  Publique = 0,
  Privee = 1,
  Dentaire = 2,
  Veterinaire = 3,
  Esthetique = 4,
  Specialisee = 5,
  Generale = 6,
  Autre = 7,
}

export enum StatutClinique {
  Active = 0,
  Inactive = 1,
}

export interface Clinique {
  id: string;
  nom: string;
  adresse: string;
  numeroTelephone: string;
  email: string;
  siteWeb?: string;
  description?: string;
  statut: number;
  typeClinique: number;
  dateCreation: string;
}

export interface StatistiqueDTO {
  mois: string;
  valeur: number;
}

export interface StatistiqueCliniqueDTO {
  nombreMedecins: number;
  nombrePatients: number;
  nombreConsultations: number;
  nombreConsultationsParMois: Record<number, number>;
  nombreNouveauxPatientsParMois: Record<number, number>;
  revenusParMois: Record<number, number>;
}

export interface LinkUserToClinicDto {
  userId: string; // ID of the user to link
  clinicId: string; // ID of the patient to link
}
