
// Consultation types
export interface Consultation {
  id: string;
  patientId: string;
  medecinId: string;
  dateConsultation: string;
  heureDebut: string;
  heureFin: string;
  raison: string;
  notes?: string;
  statut: 'Programmée' | 'Terminée' | 'Annulée';
  documents?: DocumentMedical[];
}

export interface ConsultationDTO {
  id?: string;
  patientId: string;
  medecinId: string;
  dateConsultation: string;
  heureDebut: string;
  heureFin: string;
  raison: string;
  notes?: string;
  statut: 'Programmée' | 'Terminée' | 'Annulée';
}

export interface DocumentMedical {
  id: string;
  consultationId: string;
  nom: string;
  type: string;
  contenu: string;
  dateUpload: string;
}

export interface DocumentMedicalDTO {
  consultationId: string;
  nom: string;
  type: string;
  contenu: string;
}
