import { api } from "@/utils/apiClient";
import { API_ENDPOINTS } from "@/config/api";
import {
  AddTarificationRequest,
  CreateFactureRequest,
  Facture,
  FactureStatsDTO,
  PayInvoiceRequest,
  TarifConsultation,
  UpdateFactureRequest,
  UpdateTarificationRequest,
} from "@/types/billing";

export const billingService = {
  // Facture endpoints (FactureController)
  async getAllFactures(): Promise<Facture[]> {
    return api.get(API_ENDPOINTS.BILLING.FACTURE.GET_ALL);
  },

  async getFactureById(id: string): Promise<Facture> {
    return api.get(API_ENDPOINTS.BILLING.FACTURE.GET_BY_ID(id));
  },

  async addFacture(data: CreateFactureRequest): Promise<void> {
    return api.post(API_ENDPOINTS.BILLING.FACTURE.ADD, data);
  },

  async updateFacture(data: UpdateFactureRequest): Promise<void> {
    return api.put(API_ENDPOINTS.BILLING.FACTURE.UPDATE, data);
  },

  async deleteFacture(id: string): Promise<void> {
    return api.delete(API_ENDPOINTS.BILLING.FACTURE.DELETE(id));
  },

  async getFacturesByClinicId(cliniqueId: string): Promise<Facture[]> {
    return api.get(API_ENDPOINTS.BILLING.FACTURE.BY_CLINIC(cliniqueId));
  },

  async getFacturesByPatientId(patientId: string): Promise<Facture[]> {
    return api.get(API_ENDPOINTS.BILLING.FACTURE.BY_PATIENT_ID(patientId));
  },

  async getFacturesByConsultationId(consultationId: string): Promise<Facture> {
    return api.get(
      API_ENDPOINTS.BILLING.FACTURE.BY_CONSULTATION_ID(consultationId)
    );
  },

  async getFacturesByDateRange(
    startDate: string,
    endDate: string
  ): Promise<Facture[]> {
    return api.get(
      API_ENDPOINTS.BILLING.FACTURE.GET_BY_DATE_RANGE(startDate, endDate)
    );
  },

  async filterFactures(filters: {
    clinicId?: string;
    patientId?: string;
    status?: string;
  }): Promise<Facture[]> {
    const params = new URLSearchParams();
    if (filters.clinicId) params.append("clinicId", filters.clinicId);
    if (filters.patientId) params.append("patientId", filters.patientId);
    if (filters.status) params.append("status", filters.status);

    const url = `${API_ENDPOINTS.BILLING.FACTURE.FILTER}?${params.toString()}`;

    return api.get<Facture[]>(url);
  },

  async exportFacturePdf(id: string): Promise<Blob> {
    const url = API_ENDPOINTS.BILLING.FACTURE.EXPORT(id);
    const response = await fetch(url, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${localStorage.getItem("accessToken")}`, // si nécessaire
      },
    });
    if (!response.ok) {
      throw new Error("Failed to download PDF");
    }
    return response.blob();
  },

  async getStatsFacturesByStatus(): Promise<FactureStatsDTO> {
    return api.get(API_ENDPOINTS.BILLING.FACTURE.STATS.BY_STATUS);
  },

  async getStatsFacturesByClinic(): Promise<FactureStatsDTO> {
    return api.get(API_ENDPOINTS.BILLING.FACTURE.STATS.BY_CLINIC);
  },

  async getStatsFacturesByStatusByClinic(): Promise<FactureStatsDTO> {
    return api.get(API_ENDPOINTS.BILLING.FACTURE.STATS.BY_STATUS_CLINIC);
  },

  async getStatsFacturesByStatusInClinic(
    clinicId: string
  ): Promise<FactureStatsDTO> {
    return api.get(
      API_ENDPOINTS.BILLING.FACTURE.STATS.BY_STATUS_IN_CLINIC(clinicId)
    );
  },

  async getStatsFacturesByPeriod(
    start: string,
    end: string
  ): Promise<FactureStatsDTO> {
    return api.get(API_ENDPOINTS.BILLING.FACTURE.STATS_BY_PERIOD(start, end));
  },

  // Paiement endpoints (PaiementController)
  async payerFacture(
    factureId: string,
    data: PayInvoiceRequest & { montant: number }
  ): Promise<boolean> {
    try {
      const response = await api.post(
        API_ENDPOINTS.BILLING.PAIEMENT.PAYER(factureId),
        {
          MoyenPaiement: data.MoyenPaiement,
          Montant: data.montant,
          CardDetails:
            data.MoyenPaiement === "CarteBancaire"
              ? data.CardDetails
              : undefined,
        }
      );
      return response === "Paiement effectué avec succès.";
    } catch (error) {
      throw new Error(error.response?.data || "Erreur lors du paiement");
    }
  },

  async imprimerRecuPaiement(factureId: string): Promise<Blob> {
    const url = API_ENDPOINTS.BILLING.PAIEMENT.RECU(factureId);
    const response = await fetch(url, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${localStorage.getItem("accessToken")}`, // si nécessaire
      },
    });
    if (!response.ok) {
      throw new Error("Failed to download receipt PDF");
    }
    return response.blob();
  },

  // Tarification endpoints (TarificationController)
  async getAllTarifications(): Promise<TarifConsultation[]> {
    return api.get(API_ENDPOINTS.BILLING.TARIFICATION.GET_ALL);
  },

  async addTarification(data: AddTarificationRequest): Promise<void> {
    return api.post(API_ENDPOINTS.BILLING.TARIFICATION.ADD, data);
  },

  async updateTarification(data: UpdateTarificationRequest): Promise<void> {
    return api.put(API_ENDPOINTS.BILLING.TARIFICATION.UPDATE, data);
  },

  async deleteTarification(id: string): Promise<void> {
    return api.delete(API_ENDPOINTS.BILLING.TARIFICATION.DELETE(id));
  },

  async getTarificationById(id: string): Promise<TarifConsultation> {
    return api.get(API_ENDPOINTS.BILLING.TARIFICATION.GET_BY_ID(id));
  },

  async getTarificationByClinicId(
    clinicId: string
  ): Promise<TarifConsultation[]> {
    return api.get(
      API_ENDPOINTS.BILLING.TARIFICATION.GET_BY_CLINIC_ID(clinicId)
    );
  },

  async getConsultationPricing(clinicId: string): Promise<TarifConsultation[]> {
    return await api.get(
      API_ENDPOINTS.BILLING.TARIFICATION.GET_BY_CLINIC_ID(clinicId)
    );
  },
};
