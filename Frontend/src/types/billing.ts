import { ConsultationType } from "./consultation";

// Correspondance des enums C# en TypeScript
export enum FactureStatus {
  IMPAYEE = "IMPAYEE",
  PARTIELLEMENT_PAYEE = "PARTIELLEMENT_PAYEE",
  PAYEE = "PAYEE",
  ANNULEE = "ANNULEE",
}

export enum ModePaiement {
  Especes = "Especes",
  CarteBancaire = "CarteBancaire",
  Virement = "Virement",
  Chèque = "Chèque",
  Mobile = "Mobile",
}

// Entité Facture
export interface Facture {
  id: string; // Guid
  patientId: string; // Guid
  consultationId: string; // Guid
  clinicId: string; // Guid
  dateEmission: string; // Date ISO string
  montantTotal: number; // decimal
  montantPaye: number; // decimal
  status: FactureStatus; // Enum
  paiement?: Paiement | null;
}

// Entité Paiement
export interface Paiement {
  id: string; // Guid
  montant: number; // decimal
  datePaiement: string; // Date ISO string
  mode: ModePaiement; // Enum
  factureId: string; // Guid
}

// Tarif consultation
export interface TarifConsultation {
  id: string;
  clinicId: string;
  consultationType: ConsultationType; // enum définie ci-dessous
  prix: number;
  dateCreation: string; // Date ISO string
}

// DTOs utiles

export interface CreateFactureRequest {
  patientId: string;
  consultationId: string;
  clinicId: string;
  montantTotal: number;
}

export interface UpdateFactureRequest extends CreateFactureRequest {
  id: string;
}

export interface AddTarificationRequest {
  clinicId: string;
  consultationType: ConsultationType;
  prix: number;
}

export interface UpdateTarificationRequest {
  id: string;
  consultationType: ConsultationType;
  prix: number;
}

export interface CreateInvoiceRequest {
  consultationId: string;
  patientId: string;
  clinicId: string;
  consultationTypeId: string;
  description: string;
}

export interface PayInvoiceRequest {
  MoyenPaiement: ModePaiement;
  CardDetails?: {
    CardNumber: string;
    ExpiryDate: string;
    Cvv: string;
    CardholderName: string;
  };
}

export interface PayInvoiceResponse {
  success: boolean;
  message: string;
}


export interface PaiementDto {
  moyenPaiement: ModePaiement;
  montant: number;
}

export interface RecentPaiementDto {
  montant: number;
  datePaiement: string;
}

export interface FactureStatsDTO {
  cle: string;
  nombre: number;
}

export interface StatistiquesFacturesDto {
  nombreTotal: number;
  nombrePayees: number;
  nombreImpayees: number;
  nombrePartiellementPayees: number;
  montantTotal: number;
  montantTotalPaye: number;
  nombreParClinique: Record<string, number>; // clé = Guid clinique
}

export interface BillingStatsDto {
  revenue: number;
  pendingAmount: number;
  overdueAmount: number;
  paymentRate: number;
}

export function mapFactureStatus(value: number | string): FactureStatus {
  const mapping: Record<number, FactureStatus> = {
    0: FactureStatus.IMPAYEE,
    1: FactureStatus.PARTIELLEMENT_PAYEE,
    2: FactureStatus.PAYEE,
    3: FactureStatus.ANNULEE,
  };

  if (typeof value === "string") return value as FactureStatus;
  return mapping[value] ?? (value as unknown as FactureStatus);
}
