export interface Patient {
  id: string;
  nom: string;
  prenom: string;
  dateNaissance: string;
  sexe: "M" | "F";
  adresse: string;
  telephone: string;
  email: string;
  clinicId?: string;
  dateCreation: string;
}

export interface PatientDTO extends Omit<Patient, "id"> {
  id?: string;
}
export interface Document {
  id: string;
  nom: string;
  type: string;
  url: string;
  dateCreation: string;
}

export interface CreateDocumentRequest extends Document {
  nom: string;
  url: string;
  type: string;
}

export interface DossierMedical {
  id: string;
  patientId: string; // Foreign key to Patient
  allergies: string;
  maladiesChroniques: string; // Correspond à "chronicDiseases"
  medicamentsActuels: string; // Correspond à "currentMedications"
  antécédentsFamiliaux: string; // Correspond à "familyHistory"
  antécédentsPersonnels: string; // Correspond à "personalHistory"
  groupeSanguin: string; // Correspond à "bloodType"
  documents: Document[]; // Liste des documents associés
  dateCreation: string; // Correspond à "creationDate"
}

export interface DossierMedicalDTO
  extends Omit<DossierMedical, "id" | "documents" | "dateCreation"> {
  patientId: string;
  allergies: string;
  medicamentsActuels: string;
  maladiesChroniques: string;
  antécédentsFamiliaux: string;
  antécédentsPersonnels: string;
  groupeSanguin: string;
}

export interface PatientStatistique {
  totalPatients: number;
  nouveauxPatients: number;
  patientsParSexe: {
    masculin: number;
    feminin: number;
    autre: number;
  };
  patientsParGroupeDAge: {
    moinsDeVingt: number;
    vingtATrente: number;
    trenteAQuarante: number;
    quaranteACinquante: number;
    cinquanteASoixante: number;
    plusDeSoixante: number;
  };
}

export interface LinkUserToPatientDto {
  userId: string; // ID of the user to link
  patientId: string; // ID of the patient to link
}
