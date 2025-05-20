import { useState, useEffect } from "react";
import { z } from "zod";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from "@/components/ui/dialog";
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { toast } from "sonner";
import { format } from "date-fns";

const patientFormSchema = z.object({
  prenom: z.string().min(2, { message: "Le prénom doit contenir au moins 2 caractères." }),
  nom: z.string().min(2, { message: "Le nom doit contenir au moins 2 caractères." }),
  email: z.string().email({ message: "Veuillez entrer une adresse email valide." }),
  telephone: z.string().min(5, { message: "Le numéro de téléphone doit contenir au moins 5 caractères." }),
  dateNaissance: z.string().refine((value) => {
    const date = new Date(value);
    return !isNaN(date.getTime()) && date <= new Date();
  }, { message: "Veuillez entrer une date de naissance valide." }),
  adresse: z.string().min(5, { message: "L'adresse doit contenir au moins 5 caractères." }),
  sexe: z.enum(["M", "F"]),
});

type PatientFormValues = z.infer<typeof patientFormSchema>;

interface PatientFormProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: PatientFormValues) => void;
  initialData?: Partial<PatientFormValues>;
  isLoading?: boolean;
}

export function PatientForm({ isOpen, onClose, onSubmit, initialData, isLoading }: PatientFormProps) {
  const [isSubmitting, setIsSubmitting] = useState(false);

  const form = useForm<PatientFormValues>({
    resolver: zodResolver(patientFormSchema),
    defaultValues: {
      prenom: initialData?.prenom || "",
      nom: initialData?.nom || "",
      email: initialData?.email || "",
      telephone: initialData?.telephone || "",
      dateNaissance: initialData?.dateNaissance
        ? format(new Date(initialData.dateNaissance), "yyyy-MM-dd")
        : "",
      adresse: initialData?.adresse || "",
      sexe: initialData?.sexe || undefined,
    },
  });

  // Reset form values when initialData changes
  useEffect(() => {
    if (initialData) {
      form.reset({
        prenom: initialData.prenom || "",
        nom: initialData.nom || "",
        email: initialData.email || "",
        telephone: initialData.telephone || "",
        dateNaissance: initialData.dateNaissance
          ? format(new Date(initialData.dateNaissance), "yyyy-MM-dd")
          : "",
        adresse: initialData.adresse || "",
        sexe: initialData.sexe || undefined,
      });
    }
  }, [initialData, form]);

  const handleSubmit = async (data: PatientFormValues) => {
    setIsSubmitting(true);
    try {
      await onSubmit(data);
      toast.success(initialData ? "Patient mis à jour avec succès!" : "Patient créé avec succès!");
      form.reset();
      onClose();
    } catch (error) {
      console.error("Erreur lors de la soumission du formulaire du patient:", error);
      toast.error("Échec de l'enregistrement du patient. Veuillez réessayer.");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="sm:max-w-[425px]">
        <DialogHeader>
          <DialogTitle>{initialData ? "Modifier le patient" : "Ajouter un nouveau patient"}</DialogTitle>
        </DialogHeader>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(handleSubmit)} className="space-y-4">
            <FormField
              control={form.control}
              name="prenom"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Prénom</FormLabel>
                  <FormControl>
                    <Input placeholder="Jean" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="nom"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Nom</FormLabel>
                  <FormControl>
                    <Input placeholder="Dupont" {...field} />
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
                    <Input placeholder="jean.dupont@example.com" type="email" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="telephone"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Téléphone</FormLabel>
                  <FormControl>
                    <Input placeholder="01 23 45 67 89" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="dateNaissance"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Date de naissance</FormLabel>
                  <FormControl>
                    <Input type="date" {...field} max={format(new Date(), "yyyy-MM-dd")} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="sexe"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Sexe</FormLabel>
                  <Select onValueChange={field.onChange} defaultValue={field.value}>
                    <FormControl>
                      <SelectTrigger>
                        <SelectValue placeholder="Sélectionnez le sexe" />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      <SelectItem value="M">Homme</SelectItem>
                      <SelectItem value="F">Femme</SelectItem>
                    </SelectContent>
                  </Select>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="adresse"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Adresse</FormLabel>
                  <FormControl>
                    <Input placeholder="123 Rue de la Paix, Paris" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <DialogFooter>
              <Button type="button" variant="outline" onClick={onClose}>
                Annuler
              </Button>
              <Button type="submit" disabled={isSubmitting || isLoading}>
                {isSubmitting || isLoading
                  ? "Enregistrement..."
                  : initialData
                  ? "Mettre à jour"
                  : "Créer"}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}