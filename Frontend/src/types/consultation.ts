
// Consultation types
export interface Consultation {
  id: string;
  patientId: string;
  medecinId: string;
  clinicId: string;
  dateConsultation: string;
  diagnostic: string;
  notes: string;
  documents: DocumentMedical[];
}

export interface ConsultationDTO {
  id?: string;
  patientId: string;
  medecinId: string;
  clinicId?: string; // Ce champ peut être rempli automatiquement backend
  dateConsultation: string;
  diagnostic: string;
  notes: string;
  documents?: DocumentMedicalDTO[];
}


export interface DocumentMedical {
  id: string;
  consultationId: string;
  fileName: string; 
  type: string;
  fichierURL: string; // Renommé pour correspondre au backend
  dateAjout: string;
}

export interface DocumentMedicalDTO {
  id?: string;
  consultationId: string;
  fileName: string;
  type: string;
  fichierURL: string;
  dateAjout?: string;
}
