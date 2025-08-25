export type InvoiceStatus = "Paid" | "Pending" | "Overdue";

export interface Invoice {
  id: string;
  patient: string;
  date: string;
  amount: number;
  status: InvoiceStatus;
  description: string;
  dueDate: string;
}

export interface PaymentDetails {
  cardName: string;
  cardNumber: string;
  expiryDate: string;
  cvv: string;
}
