
import { useState, useEffect } from "react";
import { z } from "zod";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from "@/components/ui/dialog";
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Button } from "@/components/ui/button";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { TypeClinique, StatutClinique } from "@/types/clinic";
import { toast } from 'sonner';

const clinicFormSchema = z.object({
  nom: z.string().min(2, { message: "Name must be at least 2 characters." }),
  adresse: z.string().min(5, { message: "Address must be at least 5 characters." }),
  numeroTelephone: z.string().min(5, { message: "Phone number must be at least 5 characters." }),
  email: z.string().email({ message: "Please enter a valid email address." }),
  siteWeb: z.string().url().optional().or(z.literal('')),
  description: z.string().optional(),
  typeClinique: z.string(),
  statut: z.string(),
});

export type ClinicFormValues = z.infer<typeof clinicFormSchema>;

interface ClinicFormProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: Omit<ClinicFormValues, "typeClinique" | "statut"> & { typeClinique: number; statut: number }) => void;
  initialData?: {
    nom?: string;
    adresse?: string;
    numeroTelephone?: string;
    email?: string;
    siteWeb?: string;
    description?: string;
    typeClinique?: string; 
    statut?: string;       
  };
  isLoading?: boolean;
}


export function ClinicForm({ isOpen, onClose, onSubmit, initialData, isLoading = false }: ClinicFormProps) {
  const [isSubmitting, setIsSubmitting] = useState(false);
  
  const form = useForm<ClinicFormValues>({
    resolver: zodResolver(clinicFormSchema),
    defaultValues: {
      nom: initialData?.nom || "",
      adresse: initialData?.adresse || "",
      numeroTelephone: initialData?.numeroTelephone || "",
      email: initialData?.email || "",
      siteWeb: initialData?.siteWeb || "",
      description: initialData?.description || "",
      typeClinique: initialData?.typeClinique !== undefined ? String(initialData.typeClinique) : String(TypeClinique.Publique),
      statut: initialData?.statut !== undefined ? String(initialData.statut) : String(StatutClinique.Active),
    },
  });

    // Add this effect to update form values when initialData changes
  useEffect(() => {
    form.reset({
      nom: initialData?.nom || "",
      adresse: initialData?.adresse || "",
      numeroTelephone: initialData?.numeroTelephone || "",
      email: initialData?.email || "",
      siteWeb: initialData?.siteWeb || "",
      description: initialData?.description || "",
      typeClinique: initialData?.typeClinique !== undefined ? String(initialData.typeClinique) : String(TypeClinique.Publique),
      statut: initialData?.statut !== undefined ? String(initialData.statut) : String(StatutClinique.Active),
    });
  }, [initialData, form]);

  const handleSubmit = async (data: ClinicFormValues) => {
    setIsSubmitting(true);
    try {
      await onSubmit({
        ...data,
        typeClinique: Number(data.typeClinique),
        statut: Number(data.statut),
      });
      toast.success(initialData ? "Clinic updated successfully!" : "Clinic created successfully!");
      form.reset();
      onClose();
    } catch (error) {
      console.error("Error submitting clinic form:", error);
      toast.error("Failed to save clinic. Please try again.");
    } finally {
      setIsSubmitting(false);
    }
  };

    const getTypeCliniqueLabel = (value: number) => {
      return TypeClinique[value as unknown as keyof typeof TypeClinique];
    };
    const getStatutCliniqueLabel = (value: number) => {
      return StatutClinique[value as unknown as keyof typeof StatutClinique];
    };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="sm:max-w-[550px]">
        <DialogHeader>
          <DialogTitle>{initialData ? "Edit Clinic" : "Add New Clinic"}</DialogTitle>
        </DialogHeader>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(handleSubmit)} className="space-y-4">
            <FormField
              control={form.control}
              name="nom"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Clinic Name</FormLabel>
                  <FormControl>
                    <Input placeholder="Health Care Clinic" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="email"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Email</FormLabel>
                  <FormControl>
                    <Input placeholder="clinic@example.com" type="email" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="numeroTelephone"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Phone</FormLabel>
                  <FormControl>
                    <Input placeholder="555-1234" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="adresse"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Address</FormLabel>
                  <FormControl>
                    <Input placeholder="123 Main St" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="siteWeb"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Website</FormLabel>
                  <FormControl>
                    <Input placeholder="https://clinic.com" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="typeClinique"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Clinic Type</FormLabel>
                  <Select onValueChange={field.onChange} value={field.value}>
                    <SelectTrigger>
                      <SelectValue placeholder="Select type" />
                    </SelectTrigger>
                    <SelectContent>
                      {Object.values(TypeClinique)
                        .filter((v) => typeof v === "number")
                        .map((type) => (
                          <SelectItem key={type} value={type.toString()}>
                            {getTypeCliniqueLabel(type as number)}
                          </SelectItem>
                        ))}
                    </SelectContent>
                  </Select>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="statut"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Status</FormLabel>
                  <Select onValueChange={field.onChange} value={field.value}>
                    <SelectTrigger>
                      <SelectValue placeholder="Select status" />
                    </SelectTrigger>
                    <SelectContent>
                      {Object.values(StatutClinique)
                        .filter((v) => typeof v === "number")
                        .map((statut) => (
                          <SelectItem key={statut} value={statut.toString()}>
                            {getStatutCliniqueLabel(statut as number)}
                          </SelectItem>
                        ))}
                    </SelectContent>
                  </Select>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="description"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Description</FormLabel>
                  <FormControl>
                    <Textarea 
                      placeholder="Provide a brief description of the clinic" 
                      className="resize-none" 
                      {...field} 
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <DialogFooter>
              <Button type="button" variant="outline" onClick={onClose}>
                Cancel
              </Button>
              <Button type="submit" disabled={isSubmitting || isLoading}>
                {isSubmitting || isLoading ? "Saving..." : initialData ? "Update" : "Create"}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
