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
import { useTranslation } from "@/hooks/useTranslation";

const patientFormSchema = (t: (key: string) => string) => z.object({
  prenom: z.string().min(2, { message: t("first_name_min_length") }),
  nom: z.string().min(2, { message: t("last_name_min_length") }),
  email: z.string().email({ message: t("invalid_email") }),
  telephone: z.string().min(5, { message: t("phone_min_length") }),
  dateNaissance: z.string().refine((value) => {
    const date = new Date(value);
    return !isNaN(date.getTime()) && date <= new Date();
  }, { message: t("invalid_birth_date") }),
  adresse: z.string().min(5, { message: t("address_min_length") }),
  sexe: z.enum(["M", "F"], { message: t("invalid_gender") }),
});

type PatientFormValues = z.infer<ReturnType<typeof patientFormSchema>>;

interface PatientFormProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: PatientFormValues) => void;
  initialData?: Partial<PatientFormValues>;
  isLoading?: boolean;
}

export function PatientForm({ isOpen, onClose, onSubmit, initialData, isLoading }: PatientFormProps) {
  const { t } = useTranslation("patients");
  const [isSubmitting, setIsSubmitting] = useState(false);

  const form = useForm<PatientFormValues>({
    resolver: zodResolver(patientFormSchema(t)),
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
    } else {
      form.reset({
        prenom: "",
        nom: "",
        email: "",
        telephone: "",
        dateNaissance: "",
        adresse: "",
        sexe: undefined,
      });
    }
  }, [initialData, form]);

  const handleSubmit = async (data: PatientFormValues) => {
    setIsSubmitting(true);
    try {
      await onSubmit(data);
      toast.success(initialData ? t("success.patient_updated") : t("success.patient_created"));
      form.reset();
      onClose();
    } catch (error) {
      console.error("Error submitting patient form:", error);
      toast.error(t("errors.submit_patient_failed"));
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="max-w-[90vw] sm:max-w-[400px] px-6 py-4">
        <DialogHeader>
          <DialogTitle className="text-lg">{initialData ? t("edit_patient") : t("add_patient")}</DialogTitle>
        </DialogHeader>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(handleSubmit)} className="space-y-3 max-h-[70vh] overflow-y-auto">
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <FormField
                control={form.control}
                name="prenom"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel className="text-sm">{t("first_name")}</FormLabel>
                    <FormControl>
                      <Input 
                        placeholder={t("first_name_placeholder")} 
                        {...field} 
                        aria-label={t("first_name")} 
                        className="text-sm hover:border-primary focus:ring-1 focus:ring-primary focus:ring-offset-1 focus:border-primary" 
                      />
                    </FormControl>
                    <FormMessage className="text-xs" />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="nom"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel className="text-sm">{t("last_name")}</FormLabel>
                    <FormControl>
                      <Input 
                        placeholder={t("last_name_placeholder")} 
                        {...field} 
                        aria-label={t("last_name")} 
                        className="text-sm hover:border-primary focus:ring-1 focus:ring-primary focus:ring-offset-1 focus:border-primary" 
                      />
                    </FormControl>
                    <FormMessage className="text-xs" />
                  </FormItem>
                )}
              />
            </div>
            <FormField
              control={form.control}
              name="email"
              render={({ field }) => (
                <FormItem>
                  <FormLabel className="text-sm">{t("email")}</FormLabel>
                  <FormControl>
                    <Input 
                      placeholder={t("email_placeholder")} 
                      type="email" 
                      {...field} 
                      aria-label={t("email")} 
                      className="text-sm hover:border-primary focus:ring-1 focus:ring-primary focus:ring-offset-1 focus:border-primary" 
                    />
                  </FormControl>
                  <FormMessage className="text-xs" />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="telephone"
              render={({ field }) => (
                <FormItem>
                  <FormLabel className="text-sm">{t("phone")}</FormLabel>
                  <FormControl>
                    <Input 
                      placeholder={t("phone_placeholder")} 
                      {...field} 
                      aria-label={t("phone")} 
                      className="text-sm hover:border-primary focus:ring-1 focus:ring-primary focus:ring-offset-1 focus:border-primary" 
                    />
                  </FormControl>
                  <FormMessage className="text-xs" />
                </FormItem>
              )}
            />
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <FormField
                control={form.control}
                name="dateNaissance"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel className="text-sm">{t("birth_date")}</FormLabel>
                    <FormControl>
                      <Input 
                        type="date" 
                        {...field} 
                        max={format(new Date(), "yyyy-MM-dd")} 
                        aria-label={t("birth_date")} 
                        className="text-sm hover:border-primary focus:ring-1 focus:ring-primary focus:ring-offset-1 focus:border-primary" 
                      />
                    </FormControl>
                    <FormMessage className="text-xs" />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="sexe"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel className="text-sm">{t("gender")}</FormLabel>
                    <Select onValueChange={field.onChange} defaultValue={field.value}>
                      <FormControl>
                        <SelectTrigger 
                          aria-label={t("gender")} 
                          className="text-sm hover:border-primary focus:ring-1 focus:ring-primary focus:ring-offset-1 focus:border-primary"
                        >
                          <SelectValue placeholder={t("select_gender")} />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        <SelectItem value="M">{t("male")}</SelectItem>
                        <SelectItem value="F">{t("female")}</SelectItem>
                      </SelectContent>
                    </Select>
                    <FormMessage className="text-xs" />
                  </FormItem>
                )}
              />
            </div>
            <FormField
              control={form.control}
              name="adresse"
              render={({ field }) => (
                <FormItem>
                  <FormLabel className="text-sm">{t("address")}</FormLabel>
                  <FormControl>
                    <Input 
                      placeholder={t("address_placeholder")} 
                      {...field} 
                      aria-label={t("address")} 
                      className="text-sm hover:border-primary focus:ring-1 focus:ring-primary focus:ring-offset-1 focus:border-primary" 
                    />
                  </FormControl>
                  <FormMessage className="text-xs" />
                </FormItem>
              )}
            />
            <DialogFooter className="flex justify-end gap-2 pt-3">
              <Button 
                type="button" 
                variant="outline" 
                size="sm" 
                onClick={onClose} 
                aria-label={t("cancel")}
                className="hover:bg-gray-100"
              >
                {t("cancel")}
              </Button>
              <Button 
                type="submit" 
                size="sm" 
                disabled={isSubmitting || isLoading} 
                aria-label={initialData ? t("update") : t("create")}
                className="hover:bg-primary-dark"
              >
                {isSubmitting || isLoading
                  ? t("saving")
                  : initialData
                  ? t("update")
                  : t("create")}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}