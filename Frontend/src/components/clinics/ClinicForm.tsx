// ClinicForm.tsx
import { useState, useEffect } from "react";
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
import { Textarea } from "@/components/ui/textarea";
import { Button } from "@/components/ui/button";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { TypeClinique, StatutClinique, Clinique } from "@/types/clinic";
import { toast } from "sonner";
import { useTranslation } from "@/hooks/useTranslation";
import { useAuth } from "@/hooks/useAuth";
import { useCliniques } from "@/hooks/useCliniques";

const clinicFormSchema = z.object({
  nom: z.string().min(2, { message: "name_min_length" }),
  adresse: z.string().min(5, { message: "address_min_length" }),
  numeroTelephone: z.string().min(5, { message: "phone_min_length" }),
  email: z.string().email({ message: "invalid_email" }),
  siteWeb: z
    .string()
    .url({ message: "invalid_url" })
    .optional()
    .or(z.literal("")),
  description: z.string().optional(),
  typeClinique: z.string().refine(
    (value) => {
      const num = Number(value);
      return Object.values(TypeClinique).includes(num);
    },
    { message: "invalid_clinic_type" }
  ),
  statut: z.string().refine(
    (value) => {
      const num = Number(value);
      return Object.values(StatutClinique).includes(num);
    },
    { message: "invalid_clinic_status" }
  ),
});

export type ClinicFormValues = z.infer<typeof clinicFormSchema>;

interface ClinicFormProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (
    data: Omit<ClinicFormValues, "typeClinique" | "statut"> & {
      typeClinique: number;
      statut: number;
    }
  ) => Promise<Clinique>;
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

export function ClinicForm({
  isOpen,
  onClose,
  onSubmit,
  initialData,
  isLoading = false,
}: ClinicFormProps) {
  const { t } = useTranslation("clinics");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const { registerWithDefaultPassword, linkToProfileHelper, deleteUser } =
    useAuth();
  const { linkUserToClinique, handleDeleteClinique } = useCliniques();

  const form = useForm<ClinicFormValues>({
    resolver: zodResolver(clinicFormSchema),
    defaultValues: {
      nom: initialData?.nom || "",
      adresse: initialData?.adresse || "",
      numeroTelephone: initialData?.numeroTelephone || "",
      email: initialData?.email || "",
      siteWeb: initialData?.siteWeb || "",
      description: initialData?.description || "",
      typeClinique:
        initialData?.typeClinique !== undefined
          ? String(initialData.typeClinique)
          : String(TypeClinique.Publique),
      statut:
        initialData?.statut !== undefined
          ? String(initialData.statut)
          : String(StatutClinique.Active),
    },
  });

  // Update form values when initialData changes
  useEffect(() => {
    form.reset({
      nom: initialData?.nom || "",
      adresse: initialData?.adresse || "",
      numeroTelephone: initialData?.numeroTelephone || "",
      email: initialData?.email || "",
      siteWeb: initialData?.siteWeb || "",
      description: initialData?.description || "",
      typeClinique:
        initialData?.typeClinique !== undefined
          ? String(initialData.typeClinique)
          : String(TypeClinique.Publique),
      statut:
        initialData?.statut !== undefined
          ? String(initialData.statut)
          : String(StatutClinique.Active),
    });
  }, [initialData, form]);

  const handleSubmit = async (data: ClinicFormValues) => {
    setIsSubmitting(true);

    let createdClinicId: string | null = null;
    let createdUserId: string | null = null;

    try {
      // 1️⃣ Créer ou mettre à jour la clinique
      const clinicPayload = {
        ...data,
        siteWeb: data.siteWeb?.trim() === "" ? undefined : data.siteWeb,
        description:
          data.description?.trim() === "" ? undefined : data.description,
        typeClinique: Number(data.typeClinique),
        statut: Number(data.statut),
      };

      const newClinic = await onSubmit(clinicPayload);
      createdClinicId = newClinic.id;

      toast.success(
        initialData ? t("clinic_updated_success") : t("clinic_added_success")
      );

      if (!initialData) {
        // 🚀 Flux création : créer l'utilisateur et lier
        const userCreated = await registerWithDefaultPassword(
          data.nom,
          data.email,
          "ClinicAdmin"
        );
        createdUserId = userCreated.id;

        await linkUserToClinique(createdUserId, createdClinicId);
        await linkToProfileHelper(createdUserId, createdClinicId, 1);
      }

      form.reset();
      onClose();
    } catch (error: unknown) {
      toast.error(
        error instanceof Error ? error.message : t("clinic_save_failed")
      );

      // 🔄 Rollback uniquement si création
      if (!initialData && createdClinicId) {
        try {
          await handleDeleteClinique(createdClinicId);
          if (createdUserId) {
            await deleteUser(createdUserId, { isRollback: true });
          }
        } catch (rollbackError) {
          console.error("❌ Rollback failed:", rollbackError);
        }
      }
    } finally {
      setIsSubmitting(false);
      console.log("🏁 Submission process finished");
    }
  };

  const getTypeCliniqueLabel = (value: number): string => {
    const label =
      TypeClinique[value as unknown as keyof typeof TypeClinique as string];
    return typeof label === "string" ? label.toLowerCase() : "unknown";
  };

  const getStatutCliniqueLabel = (value: number): string => {
    const label =
      StatutClinique[value as unknown as keyof typeof StatutClinique as string];
    return typeof label === "string" ? label.toLowerCase() : "unknown";
  };

  const getErrorMessage = (errorMessage: string | undefined): string | null => {
    if (!errorMessage) return null;
    return t(errorMessage);
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="max-w-md max-h-[80vh] overflow-y-auto p-4">
        <DialogHeader>
          <DialogTitle>
            {initialData ? t("edit_clinic") : t("add_clinic")}
          </DialogTitle>
        </DialogHeader>
        <Form {...form}>
          <form
            onSubmit={form.handleSubmit(handleSubmit)}
            className="space-y-3"
          >
            <FormField
              control={form.control}
              name="nom"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{t("name")}</FormLabel>
                  <FormControl>
                    <Input placeholder={t("name_placeholder")} {...field} />
                  </FormControl>
                  <FormMessage>
                    {getErrorMessage(form.formState.errors.nom?.message)}
                  </FormMessage>
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="email"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{t("email")}</FormLabel>
                  <FormControl>
                    <Input
                      placeholder={t("email_placeholder")}
                      type="email"
                      {...field}
                    />
                  </FormControl>
                  <FormMessage>
                    {getErrorMessage(form.formState.errors.email?.message)}
                  </FormMessage>
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="numeroTelephone"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{t("phone")}</FormLabel>
                  <FormControl>
                    <Input placeholder={t("phone_placeholder")} {...field} />
                  </FormControl>
                  <FormMessage>
                    {getErrorMessage(
                      form.formState.errors.numeroTelephone?.message
                    )}
                  </FormMessage>
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="adresse"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{t("address")}</FormLabel>
                  <FormControl>
                    <Input placeholder={t("address_placeholder")} {...field} />
                  </FormControl>
                  <FormMessage>
                    {getErrorMessage(form.formState.errors.adresse?.message)}
                  </FormMessage>
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="siteWeb"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{t("website")}</FormLabel>
                  <FormControl>
                    <Input placeholder={t("website_placeholder")} {...field} />
                  </FormControl>
                  <FormMessage>
                    {getErrorMessage(form.formState.errors.siteWeb?.message)}
                  </FormMessage>
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="typeClinique"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{t("type")}</FormLabel>
                  <Select onValueChange={field.onChange} value={field.value}>
                    <SelectTrigger>
                      <SelectValue placeholder={t("select_type")} />
                    </SelectTrigger>
                    <SelectContent>
                      {Object.values(TypeClinique)
                        .filter((v) => typeof v === "number")
                        .map((type) => (
                          <SelectItem key={type} value={type.toString()}>
                            {t(getTypeCliniqueLabel(type as number))}
                          </SelectItem>
                        ))}
                    </SelectContent>
                  </Select>
                  <FormMessage>
                    {getErrorMessage(
                      form.formState.errors.typeClinique?.message
                    )}
                  </FormMessage>
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="statut"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{t("status")}</FormLabel>
                  <Select onValueChange={field.onChange} value={field.value}>
                    <SelectTrigger>
                      <SelectValue placeholder={t("select_status")} />
                    </SelectTrigger>
                    <SelectContent>
                      {Object.values(StatutClinique)
                        .filter((v) => typeof v === "number")
                        .map((statut) => (
                          <SelectItem key={statut} value={statut.toString()}>
                            {t(getStatutCliniqueLabel(statut as number))}
                          </SelectItem>
                        ))}
                    </SelectContent>
                  </Select>
                  <FormMessage>
                    {getErrorMessage(form.formState.errors.statut?.message)}
                  </FormMessage>
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="description"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{t("description")}</FormLabel>
                  <FormControl>
                    <Textarea
                      placeholder={t("description_placeholder")}
                      className="resize-none"
                      {...field}
                    />
                  </FormControl>
                  <FormMessage>
                    {getErrorMessage(
                      form.formState.errors.description?.message
                    )}
                  </FormMessage>
                </FormItem>
              )}
            />
            <DialogFooter className="flex justify-end gap-2 pt-4">
              <Button type="button" variant="outline" onClick={onClose}>
                {t("cancel")}
              </Button>
              <Button type="submit" disabled={isSubmitting || isLoading}>
                {isSubmitting || isLoading
                  ? t("saving")
                  : initialData
                  ? t("edit")
                  : t("create")}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
