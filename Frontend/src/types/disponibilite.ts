export type DayOfWeek =
  | "Sunday"
  | "Monday"
  | "Tuesday"
  | "Wednesday"
  | "Thursday"
  | "Friday"
  | "Saturday";

export const dayNames = [
  "Sunday", // 0
  "Monday", // 1
  "Tuesday", // 2
  "Wednesday", // 3
  "Thursday", // 4
  "Friday", // 5
  "Saturday", // 6
];

// Import or define UUID type
export type UUID = string;

// Disponibilité (Availability) types
export interface Disponibilite {
  id: UUID;
  medecinId: UUID;
  jour: number; // Date in YYYY-MM-DD format
  heureDebut: string; // Time in HH:mm format
  heureFin: string; // Time in HH:mm format
}

export interface CreneauDisponibleDto {
  dateHeureDebut: string;
  dateHeureFin: string;
}

export interface AvailableDoctor {
  id: UUID;
}
