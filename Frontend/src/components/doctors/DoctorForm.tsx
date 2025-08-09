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
import { Button } from "@/components/ui/button";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { API_ENDPOINTS } from "@/config/api";
import { useAuth } from "@/hooks/useAuth";
import { toast } from "sonner";
import { useTranslation } from "@/hooks/useTranslation";

// Définir les valeurs brutes des spécialités pour la soumission au serveur
const specialties = [
  { value: "General Practitioner", key: "generalPractitioner" },
  { value: "Pediatrician", key: "pediatrician" },
  { value: "Cardiologist", key: "cardiologist" },
  { value: "Dermatologist", key: "dermatologist" },
  { value: "Neurologist", key: "neurologist" },
  { value: "Psychiatrist", key: "psychiatrist" },
  { value: "Ophthalmologist", key: "ophthalmologist" },
  { value: "Gynecologist", key: "gynecologist" },
  { value: "Orthopedist", key: "orthopedist" },
  { value: "Dentist", key: "dentist" },
];

const doctorFormSchema = (t: (key: string) => string) =>
  z.object({
    prenom: z.string().min(2, { message: t("firstNameRequired") }),
    nom: z.string().min(2, { message: t("lastNameRequired") }),
    email: z.string().email({ message: t("invalidEmail") }),
    telephone: z.string().min(5, { message: t("phoneLength") }),
    specialite: z.string().min(1, { message: t("specialtyRequired") }),
    cliniqueId: z.string().nullable().optional(),
    photoUrl: z.string().url().or(z.literal("")).optional(),
    id: z.string().optional(),
  });

type DoctorFormValues = z.infer<ReturnType<typeof doctorFormSchema>>;

interface DoctorFormProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: DoctorFormValues) => void;
  initialData?: Partial<DoctorFormValues>;
  clinics?: { id: string; name: string }[];
}

const defaultValues: Partial<DoctorFormValues> = {
  prenom: "",
  nom: "",
  email: "",
  telephone: "",
  specialite: "General Practitioner",
  cliniqueId: "",
  photoUrl: "",
};

export function DoctorForm({
  isOpen,
  onClose,
  onSubmit,
  initialData,
  clinics = [],
}: DoctorFormProps) {
  const { token } = useAuth();
  const { t } = useTranslation("doctors");
  const [isSubmitting, setIsSubmitting] = useState(false);

  const form = useForm<DoctorFormValues>({
    resolver: zodResolver(doctorFormSchema(t)),
    defaultValues: {
      ...defaultValues,
      ...initialData,
    },
  });

  useEffect(() => {
    form.reset({
      ...defaultValues,
      ...initialData,
    });
  }, [initialData, form]);

  const handleSubmit = async (data: DoctorFormValues) => {
    console.log("[DoctorForm] Submitting form with values:", data);
    setIsSubmitting(true);

    try {
      if (!token) {
        toast.error(t("loginRequired"));
        return;
      }

      const isUpdating = Boolean(initialData?.id);
      const url = isUpdating
        ? API_ENDPOINTS.DOCTORS.UPDATE(initialData.id!)
        : API_ENDPOINTS.DOCTORS.CREATE;

      const method = isUpdating ? "PUT" : "POST";

      console.log("Sending", method, "request to", url);

      const response = await fetch(url, {
        method,
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({
          prenom: data.prenom,
          nom: data.nom,
          email: data.email,
          telephone: data.telephone,
          specialite: data.specialite,
          cliniqueId: data.cliniqueId || null,
          photoUrl: data.photoUrl || null,
          id: initialData?.id || undefined,
        }),
      });

      if (!response.ok) {
        throw new Error("Request failed");
      }

      toast.success(
        isUpdating ? t("doctorUpdateSuccess") : t("doctorAddSuccess")
      );
      form.reset();
      onClose();
      onSubmit({ ...data, id: initialData?.id || undefined });
    } catch (error) {
      console.error("Error submitting doctor form:", error);
      toast.error(t("errorAddingDoctor"));
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="sm:max-w-[425px]">
        <DialogHeader>
          <DialogTitle>
            {initialData ? t("editDoctor") : t("addDoctor")}
          </DialogTitle>
        </DialogHeader>
        <Form {...form}>
          <form
            onSubmit={form.handleSubmit(handleSubmit)}
            className="space-y-4"
          >
            {/** First Name */}
            <FormField
              control={form.control}
              name="prenom"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{t("firstName")}</FormLabel>
                  <FormControl>
                    <Input placeholder={t("firstName")} {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            {/** Last Name */}
            <FormField
              control={form.control}
              name="nom"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{t("lastName")}</FormLabel>
                  <FormControl>
                    <Input placeholder={t("lastName")} {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            {/** Email */}
            <FormField
              control={form.control}
              name="email"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{t("email")}</FormLabel>
                  <FormControl>
                    <Input placeholder={t("email")} type="email" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            {/** Phone */}
            <FormField
              control={form.control}
              name="telephone"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{t("phone")}</FormLabel>
                  <FormControl>
                    <Input placeholder={t("phone")} {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            {/** Specialty */}
            <FormField
              control={form.control}
              name="specialite"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{t("specialty")}</FormLabel>
                  <Select onValueChange={field.onChange} value={field.value}>
                    <FormControl>
                      <SelectTrigger>
                        <SelectValue placeholder={t("specialty")} />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      {specialties.map(({ value, key }) => (
                        <SelectItem key={value} value={value}>
                          {t(key)}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  <FormMessage />
                </FormItem>
              )}
            />

            {/** Photo URL */}
            <FormField
              control={form.control}
              name="photoUrl"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{t("photo")}</FormLabel>
                  <FormControl>
                    <Input placeholder={t("photo")} {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <DialogFooter>
              <Button type="button" variant="outline" onClick={onClose}>
                {t("cancel")}{" "}
                {/* Ajouter la clé 'cancel' si elle n'existe pas */}
              </Button>
              <Button type="submit" disabled={isSubmitting}>
                {isSubmitting
                  ? t("saving") // Ajouter la clé 'saving' si nécessaire
                  : initialData
                  ? t("update") // Ajouter la clé 'update' si nécessaire
                  : t("create")}{" "}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
