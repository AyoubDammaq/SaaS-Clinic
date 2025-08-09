import { useEffect, useState } from 'react';
import { toast } from 'sonner';
import { CheckCircle, XCircle, AlertCircle, Clock } from 'lucide-react';
import { Alert, AlertDescription } from '@/components/ui/alert';

interface PaymentStatusNotificationProps {
  isProcessing: boolean;
  paymentMethod: string;
  amount: number;
  currency: string;
}

export function PaymentStatusNotification({ 
  isProcessing, 
  paymentMethod, 
  amount, 
  currency 
}: PaymentStatusNotificationProps) {
  const [progress, setProgress] = useState(0);

  useEffect(() => {
    if (isProcessing) {
      const interval = setInterval(() => {
        setProgress(prev => {
          if (prev >= 90) return prev;
          return prev + Math.random() * 15;
        });
      }, 200);

      return () => clearInterval(interval);
    } else {
      setProgress(0);
    }
  }, [isProcessing]);

  const formatCurrency = (amount: number, currency: string = 'EUR') => {
    return new Intl.NumberFormat('fr-FR', {
      style: 'currency',
      currency: currency,
    }).format(amount);
  };

  const getPaymentMethodIcon = () => {
    switch (paymentMethod) {
      case 'card':
        return <CheckCircle className="h-4 w-4 text-green-600" />;
      case 'cash':
        return <AlertCircle className="h-4 w-4 text-orange-600" />;
      case 'transfer':
        return <Clock className="h-4 w-4 text-blue-600" />;
      default:
        return <Clock className="h-4 w-4 text-gray-600" />;
    }
  };

  const getStatusMessage = () => {
    if (isProcessing) {
      switch (paymentMethod) {
        case 'card':
          return 'Traitement du paiement par carte en cours...';
        case 'cash':
          return 'Enregistrement du paiement en espèces...';
        case 'transfer':
          return 'Validation du virement bancaire...';
        default:
          return 'Traitement du paiement...';
      }
    }
    return 'Paiement traité avec succès';
  };

  if (!isProcessing) return null;

  return (
    <Alert className="border-primary/20 bg-primary/5">
      <div className="flex items-center gap-2">
        {getPaymentMethodIcon()}
        <AlertDescription className="flex-1">
          <div className="space-y-2">
            <p className="text-sm font-medium">
              {getStatusMessage()}
            </p>
            <p className="text-xs text-muted-foreground">
              Montant: {formatCurrency(amount, currency)}
            </p>
            <div className="w-full bg-gray-200 rounded-full h-2">
              <div 
                className="bg-primary h-2 rounded-full transition-all duration-300"
                style={{ width: `${progress}%` }}
              />
            </div>
          </div>
        </AlertDescription>
      </div>
    </Alert>
  );
}

// Hook pour gérer les notifications de paiement
export function usePaymentNotifications() {
  const showSuccessPayment = (paymentMethod: string, amount: number, currency: string) => {
    const formatCurrency = (amount: number, currency: string = 'EUR') => {
      return new Intl.NumberFormat('fr-FR', {
        style: 'currency',
        currency: currency,
      }).format(amount);
    };

    const getMethodLabel = (method: string) => {
      switch (method) {
        case 'card': return 'carte bancaire';
        case 'cash': return 'espèces';
        case 'transfer': return 'virement bancaire';
        default: return method;
      }
    };

    toast.success(
      `Paiement confirmé!`,
      {
        description: `${formatCurrency(amount, currency)} payé par ${getMethodLabel(paymentMethod)}`,
        duration: 5000,
        action: {
          label: "Voir la facture",
          onClick: () => {
            // Action pour voir la facture
            console.log("Voir la facture");
          }
        }
      }
    );
  };

  const showErrorPayment = (error: string) => {
    toast.error(
      "Échec du paiement",
      {
        description: error,
        duration: 7000,
        action: {
          label: "Réessayer",
          onClick: () => {
            // Action pour réessayer
            console.log("Réessayer le paiement");
          }
        }
      }
    );
  };

  const showInfoPayment = (paymentMethod: string, message: string) => {
    const getMethodLabel = (method: string) => {
      switch (method) {
        case 'cash': return 'Paiement en espèces';
        case 'transfer': return 'Virement bancaire';
        default: return 'Information de paiement';
      }
    };

    toast.info(
      getMethodLabel(paymentMethod),
      {
        description: message,
        duration: 8000,
      }
    );
  };

  return {
    showSuccessPayment,
    showErrorPayment,
    showInfoPayment
  };
}