
import { useState } from "react";
import { z } from "zod";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from "@/components/ui/dialog";
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { toast } from 'sonner';
import { CreditCard } from "lucide-react";

const paymentFormSchema = z.object({
  cardName: z.string().min(2, { message: "Cardholder name must be at least 2 characters." }),
  cardNumber: z.string()
    .min(16, { message: "Card number must be at least 16 digits." })
    .max(19, { message: "Card number must be at most 19 digits." })
    .refine((val) => /^\d{16,19}$/.test(val.replace(/\s/g, '')), {
      message: "Card number must contain only digits.",
    }),
  expiryDate: z.string()
    .regex(/^(0[1-9]|1[0-2])\/([0-9]{2})$/, { message: "Expiry date must be in MM/YY format." }),
  cvv: z.string()
    .min(3, { message: "CVV must be at least 3 digits." })
    .max(4, { message: "CVV must be at most 4 digits." })
    .refine((val) => /^\d{3,4}$/.test(val), {
      message: "CVV must contain only digits.",
    }),
});

type PaymentFormValues = z.infer<typeof paymentFormSchema>;

interface PaymentFormProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: PaymentFormValues) => void;
  invoiceAmount: number;
  invoiceId: string;
}

export function PaymentForm({ isOpen, onClose, onSubmit, invoiceAmount, invoiceId }: PaymentFormProps) {
  const [isSubmitting, setIsSubmitting] = useState(false);
  
  const form = useForm<PaymentFormValues>({
    resolver: zodResolver(paymentFormSchema),
    defaultValues: {
      cardName: "",
      cardNumber: "",
      expiryDate: "",
      cvv: "",
    },
  });

  // Format card number with spaces after every 4 digits
  const formatCardNumber = (value: string) => {
    const v = value.replace(/\s+/g, '').replace(/[^0-9]/gi, '');
    const matches = v.match(/\d{4,16}/g);
    const match = matches && matches[0] || '';
    const parts = [];

    for (let i = 0; i < match.length; i += 4) {
      parts.push(match.substring(i, i + 4));
    }

    if (parts.length) {
      return parts.join(' ');
    } else {
      return value;
    }
  };

  // Format expiry date as MM/YY
  const formatExpiryDate = (value: string) => {
    const v = value.replace(/\s+/g, '').replace(/[^0-9]/gi, '');
    
    if (v.length >= 3) {
      return `${v.substring(0, 2)}/${v.substring(2, 4)}`;
    }
    return v;
  };

  const handleSubmit = async (data: PaymentFormValues) => {
    setIsSubmitting(true);
    try {
      // In a real application, you would process the payment
      // through a secure payment gateway like Stripe
      await onSubmit(data);
      
      // Simulate a successful payment
      setTimeout(() => {
        toast.success("Payment processed successfully!");
        form.reset();
        onClose();
        setIsSubmitting(false);
      }, 1500);
    } catch (error) {
      console.error("Error processing payment:", error);
      toast.error("Failed to process payment. Please try again.");
      setIsSubmitting(false);
    }
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="sm:max-w-[450px]">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <CreditCard className="h-5 w-5" />
            <span>Payment for Invoice #{invoiceId}</span>
          </DialogTitle>
        </DialogHeader>
        
        <div className="py-2 px-4 bg-muted/30 rounded-md mb-4">
          <div className="flex justify-between items-center">
            <span className="text-sm font-medium text-muted-foreground">Amount Due:</span>
            <span className="text-lg font-bold text-clinic-500">${invoiceAmount.toFixed(2)}</span>
          </div>
        </div>
        
        <Form {...form}>
          <form onSubmit={form.handleSubmit(handleSubmit)} className="space-y-4">
            <FormField
              control={form.control}
              name="cardName"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Cardholder Name</FormLabel>
                  <FormControl>
                    <Input placeholder="John Doe" {...field} />
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
                  <FormLabel>Card Number</FormLabel>
                  <FormControl>
                    <Input 
                      placeholder="4242 4242 4242 4242" 
                      value={formatCardNumber(field.value)}
                      onChange={e => field.onChange(e.target.value)}
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
                    <FormLabel>Expiry Date</FormLabel>
                    <FormControl>
                      <Input 
                        placeholder="MM/YY" 
                        maxLength={5}
                        value={formatExpiryDate(field.value)}
                        onChange={e => field.onChange(e.target.value)}
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
                    <FormLabel>CVV</FormLabel>
                    <FormControl>
                      <Input 
                        placeholder="123" 
                        maxLength={4}
                        type="password"
                        {...field}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>
            <DialogFooter>
              <Button type="button" variant="outline" onClick={onClose}>
                Cancel
              </Button>
              <Button type="submit" disabled={isSubmitting}>
                {isSubmitting ? "Processing..." : `Pay $${invoiceAmount.toFixed(2)}`}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
