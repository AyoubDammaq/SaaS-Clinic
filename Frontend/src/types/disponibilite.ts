
// Disponibilité (Availability) types
export interface Disponibilite {
  id: string;
  medecinId: string;
  dateDisponibilite: string; // ISO date string
  heureDebut: string; // Time in format "HH:MM:SS"
  heureFin: string; // Time in format "HH:MM:SS"
  estDisponible: boolean;
}

export interface DisponibiliteDto {
  medecinId: string;
  dateDisponibilite: string;
  heureDebut: string;
  heureFin: string;
  estDisponible: boolean;
}
