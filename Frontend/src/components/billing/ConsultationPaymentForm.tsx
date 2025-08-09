import { useState } from "react";
import { z } from "zod";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from "@/components/ui/dialog";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { CreditCard, Banknote } from "lucide-react";
import { Facture, ModePaiement } from "@/types/billing";
import {
  PaymentStatusNotification,
  usePaymentNotifications,
} from "./PaymentStatusNotification";
import { useTranslation } from "@/hooks/useTranslation";

// Luhn Algorithm for card number validation
const isValidCardNumber = (cardNumber: string): boolean => {
  const cleanNumber = cardNumber.replace(/\s/g, "");
  if (!/^\d{16,19}$/.test(cleanNumber)) return false;
  let sum = 0;
  let alternate = false;
  for (let i = cleanNumber.length - 1; i >= 0; i--) {
    let digit = parseInt(cleanNumber[i], 10);
    if (alternate) {
      digit *= 2;
      if (digit > 9) digit -= 9;
    }
    sum += digit;
    alternate = !alternate;
  }
  return sum % 10 === 0;
};

// Validate future expiry date
const isFutureExpiryDate = (expiry: string): boolean => {
  const [month, year] = expiry.split("/").map(Number);
  const expiryYear = 2000 + year;
  const expiryDate = new Date(expiryYear, month, 0); // Last day of the month
  return expiryDate > new Date();
};

const paymentFormSchema = z.object({
  paymentMethod: z
    .string()
    .min(1, { message: "Veuillez sélectionner un mode de paiement." })
    .refine((val) => Object.values(ModePaiement).includes(val as ModePaiement), {
      message: "Mode de paiement invalide.",
    }),
  cardholderName: z
    .string()
    .min(2, { message: "Le nom du titulaire doit contenir au moins 2 caractères." })
    .optional(),
  cardNumber: z
    .string()
    .min(16, { message: "Le numéro de carte doit contenir au moins 16 chiffres." })
    .max(19, { message: "Le numéro de carte doit contenir au maximum 19 chiffres." })
    .refine((val) => !val || isValidCardNumber(val), {
      message: "Numéro de carte invalide (échec de la validation Luhn).",
    })
    .optional(),
  expiryDate: z
    .string()
    .regex(/^(0[1-9]|1[0-2])\/([0-9]{2})$/, {
      message: "La date d'expiration doit être au format MM/AA.",
    })
    .refine((val) => !val || isFutureExpiryDate(val), {
      message: "La date d'expiration doit être dans le futur.",
    })
    .optional(),
  cvv: z
    .string()
    .min(3, { message: "Le CVV doit contenir au moins 3 chiffres." })
    .max(4, { message: "Le CVV doit contenir au maximum 4 chiffres." })
    .refine((val) => !val || /^\d{3,4}$/.test(val), {
      message: "Le CVV doit contenir uniquement des chiffres.",
    })
    .optional(),
}).refine(
  (data) =>
    data.paymentMethod !== ModePaiement.CarteBancaire ||
    (data.cardholderName && data.cardNumber && data.expiryDate && data.cvv),
  {
    message: "Tous les champs de carte sont requis pour un paiement par carte.",
    path: ["paymentMethod"],
  }
);

export type PaymentFormValues = z.infer<typeof paymentFormSchema>;

interface ConsultationPaymentFormProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: PaymentFormValues) => Promise<boolean>;
  invoice: Facture | null;
}

export function ConsultationPaymentForm({
  isOpen,
  onClose,
  onSubmit,
  invoice,
}: ConsultationPaymentFormProps) {
  const { t } = useTranslation("billing");
  const tCommon = useTranslation("common").t;
  const [isSubmitting, setIsSubmitting] = useState(false);
  const { showSuccessPayment, showErrorPayment, showInfoPayment } =
    usePaymentNotifications();

  const form = useForm<PaymentFormValues>({
    resolver: zodResolver(paymentFormSchema),
    defaultValues: {
      paymentMethod: "",
      cardholderName: "",
      cardNumber: "",
      expiryDate: "",
      cvv: "",
    },
  });

  const selectedPaymentMethod = form.watch("paymentMethod");

  // Format card number with spaces after every 4 digits
  const formatCardNumber = (value: string) => {
    const v = value.replace(/\s+/g, "").replace(/[^0-9]/gi, "");
    const matches = v.match(/\d{4,16}/g);
    const match = (matches && matches[0]) || "";
    const parts = [];
    for (let i = 0; i < match.length; i += 4) {
      parts.push(match.substring(i, i + 4));
    }
    return parts.length ? parts.join(" ") : value;
  };

  // Format expiry date as MM/AA
  const formatExpiryDate = (value: string) => {
    const v = value.replace(/\s+/g, "").replace(/[^0-9]/gi, "");
    if (v.length >= 3) {
      return `${v.substring(0, 2)}/${v.substring(2, 4)}`;
    }
    return v;
  };

  const formatCurrency = (amount: number, currency: string = "EUR") => {
    return new Intl.NumberFormat("fr-FR", {
      style: "currency",
      currency: currency,
    }).format(amount);
  };

  const handleSubmit = async (data: PaymentFormValues) => {
    if (!invoice) return;

    setIsSubmitting(true);
    try {
      console.log("Form values:", data); // Debug form values
      // Validate montant
      const montant = invoice.montantTotal - invoice.montantPaye;
      if (montant <= 0) {
        throw new Error(t("noAmountToPay"));
      }

      // Show info for non-card payments
      if (data.paymentMethod === ModePaiement.Especes) {
        showInfoPayment("cash", t("cashPaymentInfo"));
      } else if (data.paymentMethod === ModePaiement.Virement) {
        showInfoPayment("transfer", t("transferPaymentInfo"));
      } else if (data.paymentMethod === ModePaiement.Chèque) {
        showInfoPayment("cheque", t("chequePaymentInfo"));
      } else if (data.paymentMethod === ModePaiement.Mobile) {
        showInfoPayment("mobile", t("mobilePaymentInfo"));
      }

      const success = await onSubmit({
        paymentMethod: data.paymentMethod,
        cardholderName: data.cardholderName,
        cardNumber: data.cardNumber ? data.cardNumber.replace(/\s/g, "") : undefined,
        expiryDate: data.expiryDate,
        cvv: data.cvv,
      });

      if (success) {
        showSuccessPayment(data.paymentMethod, montant, "EUR");
        form.reset();
        onClose();
      }
    } catch (error) {
      console.error("Error processing payment:", error);
      const errorMessage = error instanceof Error ? error.message : t("paymentError");
      showErrorPayment(errorMessage);
    } finally {
      setIsSubmitting(false);
    }
  };

  if (!invoice || !isOpen) return null;

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="sm:max-w-[500px]">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <CreditCard className="h-5 w-5" aria-hidden="true" />
            <span>{t("invoicePayment")}</span>
          </DialogTitle>
        </DialogHeader>

        <div className="py-2 px-4 bg-muted/30 rounded-md mb-4">
          <div className="space-y-2">
            <div className="flex justify-between items-center">
              <span className="text-sm font-medium text-muted-foreground">
                {t("invoiceNumber")}
              </span>
              <span className="text-sm font-mono">
                {invoice.id.substring(0, 8).toUpperCase()}
              </span>
            </div>
            <div className="flex justify-between items-center">
              <span className="text-sm font-medium text-muted-foreground">
                {t("amountToPay")}
              </span>
              <span className="text-lg font-bold text-primary">
                {formatCurrency(invoice.montantTotal - invoice.montantPaye)}
              </span>
            </div>
          </div>
        </div>

        {isSubmitting && (
          <PaymentStatusNotification
            isProcessing={isSubmitting}
            paymentMethod={selectedPaymentMethod}
            amount={invoice.montantTotal - invoice.montantPaye}
            currency="EUR"
          />
        )}

        <Form {...form}>
          <form
            onSubmit={form.handleSubmit(handleSubmit)}
            className="space-y-4"
          >
            <FormField
              control={form.control}
              name="paymentMethod"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{t("paymentMethod")}</FormLabel>
                  <Select
                    onValueChange={(value) => {
                      field.onChange(value);
                      if (value !== ModePaiement.CarteBancaire) {
                        form.setValue("cardholderName", "");
                        form.setValue("cardNumber", "");
                        form.setValue("expiryDate", "");
                        form.setValue("cvv", "");
                        form.reset();
                        onClose();
                      }
                    }}
                    defaultValue={field.value}
                  >
                    <FormControl>
                      <SelectTrigger>
                        <SelectValue placeholder={t("selectPaymentMethod")} />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      <SelectItem value={ModePaiement.CarteBancaire}>
                        <div className="flex items-center gap-2">
                          <CreditCard className="h-4 w-4" aria-hidden="true" />
                          {t("creditCard")}
                        </div>
                      </SelectItem>
                      <SelectItem value={ModePaiement.Especes}>
                        <div className="flex items-center gap-2">
                          <Banknote className="h-4 w-4" aria-hidden="true" />
                          {t("cashOnSite")}
                        </div>
                      </SelectItem>
                      <SelectItem value={ModePaiement.Virement}>
                        <div className="flex items-center gap-2">
                          <Banknote className="h-4 w-4" aria-hidden="true" />
                          {t("bankTransfer")}
                        </div>
                      </SelectItem>
                      <SelectItem value={ModePaiement.Chèque}>
                        <div className="flex items-center gap-2">
                          <Banknote className="h-4 w-4" aria-hidden="true" />
                          {t("check")}
                        </div>
                      </SelectItem>
                      <SelectItem value={ModePaiement.Mobile}>
                        <div className="flex items-center gap-2">
                          <Banknote className="h-4 w-4" aria-hidden="true" />
                          {t("mobilePayment")}
                        </div>
                      </SelectItem>
                    </SelectContent>
                  </Select>
                  <FormMessage />
                </FormItem>
              )}
            />

            {selectedPaymentMethod === ModePaiement.CarteBancaire && (
              <>
                <FormField
                  control={form.control}
                  name="cardholderName"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>{t("cardholderName")}</FormLabel>
                      <FormControl>
                        <Input placeholder={t("cardholderPlaceholder")} {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
                <FormField
                  control={form.control}
                  name="cardNumber"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>{t("cardNumber")}</FormLabel>
                      <FormControl>
                        <Input
                          placeholder={t("cardNumberPlaceholder")}
                          value={field.value || ""}
                          onChange={(e) => field.onChange(formatCardNumber(e.target.value))}
                          maxLength={19}
                        />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
                <div className="grid grid-cols-2 gap-4">
                  <FormField
                    control={form.control}
                    name="expiryDate"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{t("expiryDate")}</FormLabel>
                        <FormControl>
                          <Input
                            placeholder={t("expiryDatePlaceholder")}
                            maxLength={5}
                            value={field.value || ""}
                            onChange={(e) => field.onChange(formatExpiryDate(e.target.value))}
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={form.control}
                    name="cvv"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{t("cvv")}</FormLabel>
                        <FormControl>
                          <Input
                            placeholder={t("cvvPlaceholder")}
                            maxLength={4}
                            type="password"
                            value={field.value || ""}
                            onChange={field.onChange}
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>
              </>
            )}

            {selectedPaymentMethod === ModePaiement.Especes && (
              <div className="p-4 bg-blue-50 rounded-md">
                <p className="text-sm text-blue-800">
                  {t("cashPaymentInfo")}
                </p>
              </div>
            )}

            {selectedPaymentMethod === ModePaiement.Virement && (
              <div className="p-4 bg-green-50 rounded-md">
                <p className="text-sm text-green-800">
                  {t("transferPaymentInfo")}
                </p>
              </div>
            )}

            {selectedPaymentMethod === ModePaiement.Chèque && (
              <div className="p-4 bg-yellow-50 rounded-md">
                <p className="text-sm text-yellow-800">
                  {t("chequePaymentInfo")}
                </p>
              </div>
            )}

            {selectedPaymentMethod === ModePaiement.Mobile && (
              <div className="p-4 bg-purple-50 rounded-md">
                <p className="text-sm text-purple-800">
                  {t("mobilePaymentInfo")}
                </p>
              </div>
            )}

            <DialogFooter>
              <Button type="button" variant="outline" onClick={onClose}>
                {tCommon("cancel")}
              </Button>
              <Button type="submit" disabled={isSubmitting}>
                {isSubmitting
                  ? t("processing")
                  : `${t("pay")} ${formatCurrency(invoice.montantTotal - invoice.montantPaye)}`}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}