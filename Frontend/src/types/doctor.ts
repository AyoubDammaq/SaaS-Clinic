
// Doctor types matching the backend models

// Import or define UUID type
type UUID = string;

export interface Doctor {
  id: UUID;
  prenom: string;
  nom: string;
  specialite: string;
  email: string;
  telephone: string;
  cliniqueId?: UUID;
  photoUrl?: string;
  dateCreation: string;
}

export interface DoctorDto {
  prenom: string;
  nom: string;
  specialite: string;
  email: string;
  telephone: string;
  cliniqueId?: UUID;
  photoUrl?: string;
}
export interface AttribuerMedecinDto {
  medecinId: string;
  cliniqueId: string;
}

export interface SpecialiteStatistique {
  specialite: string;
  nombreMedecins: number;
}

export interface CliniqueStatistique {
  cliniqueId: string;
  cliniqueName: string; // May need to be populated from clinic service
  nombreMedecins: number;
}


export interface LinkUserToDoctorDto {
  userId: string; // ID of the user to link
  doctorId: string; // ID of the patient to link
}