// Consultation types
export enum ConsultationType {
  ConsultationGenerale = 1,
  ConsultationSpecialiste = 2,
  ConsultationUrgence = 3,
  ConsultationSuivi = 4,
  ConsultationLaboratoire = 5,
}

export const consultationTypes: Record<ConsultationType, string> = {
  [ConsultationType.ConsultationGenerale]: "consultationType.general",
  [ConsultationType.ConsultationSpecialiste]: "consultationType.specialist",
  [ConsultationType.ConsultationUrgence]: "consultationType.emergency",
  [ConsultationType.ConsultationSuivi]: "consultationType.followUp",
  [ConsultationType.ConsultationLaboratoire]: "consultationType.laboratory",
};

export interface Consultation {
  id: string;
  patientId: string;
  medecinId: string;
  clinicId: string;
  type: ConsultationType;
  dateConsultation: string;
  diagnostic: string;
  notes: string;
  documents: DocumentMedical[];
}

export interface ConsultationDTO {
  id?: string;
  patientId: string;
  medecinId: string;
  clinicId?: string;
  type: ConsultationType;
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
