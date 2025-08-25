import { toast } from "sonner";

export function usePaymentNotifications() {
  const showSuccessPayment = (
    paymentMethod: string,
    amount: number,
    currency: string
  ) => {
    const formatCurrency = (amount: number, currency: string = "EUR") => {
      return new Intl.NumberFormat("fr-FR", {
        style: "currency",
        currency: currency,
      }).format(amount);
    };

    const getMethodLabel = (method: string) => {
      switch (method) {
        case "card":
          return "carte bancaire";
        case "cash":
          return "espèces";
        case "transfer":
          return "virement bancaire";
        default:
          return method;
      }
    };

    toast.success(`Paiement confirmé!`, {
      description: `${formatCurrency(
        amount,
        currency
      )} payé par ${getMethodLabel(paymentMethod)}`,
      duration: 5000,
      action: {
        label: "Voir la facture",
        onClick: () => {
          // Action pour voir la facture
          console.log("Voir la facture");
        },
      },
    });
  };

  const showErrorPayment = (error: string) => {
    toast.error("Échec du paiement", {
      description: error,
      duration: 7000,
      action: {
        label: "Réessayer",
        onClick: () => {
          // Action pour réessayer
          console.log("Réessayer le paiement");
        },
      },
    });
  };

  const showInfoPayment = (paymentMethod: string, message: string) => {
    const getMethodLabel = (method: string) => {
      switch (method) {
        case "cash":
          return "Paiement en espèces";
        case "transfer":
          return "Virement bancaire";
        default:
          return "Information de paiement";
      }
    };

    toast.info(getMethodLabel(paymentMethod), {
      description: message,
      duration: 8000,
    });
  };

  return {
    showSuccessPayment,
    showErrorPayment,
    showInfoPayment,
  };
}
