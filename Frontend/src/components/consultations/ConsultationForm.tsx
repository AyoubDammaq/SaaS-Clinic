import { useEffect, useState } from "react";
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
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Button } from "@/components/ui/button";
import { toast } from "sonner";
import { Patient } from "@/types/patient";
import { Doctor } from "@/types/doctor";
import { useTranslation } from "@/hooks/useTranslation";
import { ConsultationType, consultationTypes } from "@/types/consultation";

type ConsultationTypeKey = keyof typeof consultationTypes;

const consultationFormSchema = z.object({
  patientId: z.string().min(1, { message: "Please select a patient." }),
  medecinId: z.string().min(1, { message: "Please select a doctor." }),
  type: z.nativeEnum(ConsultationType),
  diagnostic: z
    .string()
    .min(5, { message: "Le diagnostic doit contenir au moins 5 caractères." }),
  notes: z.string().optional(),
});

export type ConsultationFormValues = z.infer<typeof consultationFormSchema>;

interface ConsultationFormProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: ConsultationFormValues & { date: string }) => void;
  initialData?: Partial<ConsultationFormValues>;
  patients: Patient[];
  doctors: Doctor[];
  user: { role: string; medecinId?: string };
}

export function ConsultationForm({
  isOpen,
  onClose,
  onSubmit,
  initialData,
  patients,
  doctors,
  user,
}: ConsultationFormProps) {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const { t } = useTranslation("consultations");
  const tCommon = useTranslation("common").t;

  const form = useForm<ConsultationFormValues>({
    resolver: zodResolver(consultationFormSchema),
    defaultValues: {
      patientId: initialData?.patientId || "",
      medecinId: initialData?.medecinId || "",
      diagnostic: initialData?.diagnostic || "",
      notes: initialData?.notes || "",
      type: initialData?.type || ConsultationType.ConsultationGenerale,
    },
  });

  // Reset form values when initialData changes
  useEffect(() => {
    if (initialData) {
      form.reset(initialData);
    } else {
      form.reset({
        patientId: "",
        medecinId: user.role === "Doctor" ? user.medecinId || "" : "",
        diagnostic: "",
        notes: "",
        type: ConsultationType.ConsultationGenerale,
      });
    }
  }, [initialData, user, form]);

  const handleSubmit = async (data: ConsultationFormValues) => {
    console.log("✅ Form submitted with values:", data);
    setIsSubmitting(true);
    try {
      // Obtenir la date et l'heure actuelles
      const dateHeure = new Date();
      const pad = (num: number) => String(num).padStart(2, "0");
      const localIso = `${dateHeure.getFullYear()}-${pad(
        dateHeure.getMonth() + 1
      )}-${pad(dateHeure.getDate())}T${pad(dateHeure.getHours())}:${pad(
        dateHeure.getMinutes()
      )}:00`;

      const requestPayload: ConsultationFormValues & { date: string } = {
        ...data,
        date: localIso,
      };

      await onSubmit(requestPayload);
      toast.success(
        initialData
          ? t("consultationUpdateSuccess")
          : t("consultationAddSuccess")
      );
      form.reset();
      onClose();
    } catch (error) {
      console.error("Error submitting consultation form:", error);
      toast.error(
        initialData
          ? t("errorUpdatingConsultation")
          : t("errorAddingConsultation")
      );
    } finally {
      setIsSubmitting(false);
    }
  };

  // Trouver les noms du patient et du médecin pour l'affichage en lecture seule
  const selectedPatient = patients.find((p) => p.id === initialData?.patientId);
  const selectedDoctor = doctors.find((d) => d.id === initialData?.medecinId);

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="sm:max-w-[550px]">
        <DialogHeader>
          <DialogTitle>
            {initialData
              ? t("editConsultationTitle")
              : t("scheduleNewConsultation")}
          </DialogTitle>
        </DialogHeader>
        <Form {...form}>
          <form
            onSubmit={form.handleSubmit(handleSubmit)}
            className="space-y-4"
          >
            <FormField
              control={form.control}
              name="patientId"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{tCommon("patient")}</FormLabel>
                  {initialData ? (
                    <FormControl>
                      <Input
                        value={
                          `${selectedPatient?.prenom || ""} ${
                            selectedPatient?.nom || ""
                          }`.trim() || "N/A"
                        }
                        disabled
                      />
                    </FormControl>
                  ) : (
                    <Select
                      onValueChange={field.onChange}
                      defaultValue={field.value}
                    >
                      <FormControl>
                        <SelectTrigger>
                          <SelectValue
                            placeholder={t("selectPatientPlaceholder")}
                          />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        {patients.length === 0 ? (
                          <SelectItem disabled value="">
                            {t("noPatientsAvailable")}
                          </SelectItem>
                        ) : (
                          patients.map((patient) => (
                            <SelectItem key={patient.id} value={patient.id}>
                              {patient.prenom} {patient.nom}
                            </SelectItem>
                          ))
                        )}
                      </SelectContent>
                    </Select>
                  )}
                  <FormMessage />
                </FormItem>
              )}
            />

            {user.role !== "Doctor" && (
              <FormField
                control={form.control}
                name="medecinId"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>{tCommon("doctor")}</FormLabel>
                    {initialData ? (
                      <FormControl>
                        <Input
                          value={
                            `${selectedDoctor?.prenom || ""} ${
                              selectedDoctor?.nom || ""
                            }`.trim() || "N/A"
                          }
                          disabled
                        />
                      </FormControl>
                    ) : (
                      <Select
                        onValueChange={field.onChange}
                        defaultValue={field.value}
                      >
                        <FormControl>
                          <SelectTrigger>
                            <SelectValue
                              placeholder={t("selectDoctorPlaceholder")}
                            />
                          </SelectTrigger>
                        </FormControl>
                        <SelectContent>
                          {doctors.length === 0 ? (
                            <SelectItem disabled value="">
                              {t("noDoctorsAvailable")}
                            </SelectItem>
                          ) : (
                            doctors.map((doctor) => (
                              <SelectItem key={doctor.id} value={doctor.id}>
                                {doctor.prenom} {doctor.nom}
                              </SelectItem>
                            ))
                          )}
                        </SelectContent>
                      </Select>
                    )}
                    <FormMessage />
                  </FormItem>
                )}
              />
            )}

            <FormField
              control={form.control}
              name="type"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{t("typeConsultation")}</FormLabel>
                  <Select
                    onValueChange={(val) => field.onChange(parseInt(val))}
                    value={field.value?.toString()}
                    defaultValue={field.value?.toString()}
                    disabled={!!initialData?.type}
                  >
                    <FormControl>
                      <SelectTrigger>
                        <SelectValue placeholder={t("selectTypePlaceholder")} />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      {Object.entries(consultationTypes).map(
                        ([value, label]) => (
                          <SelectItem key={value} value={value}>
                            {t(label)}
                          </SelectItem>
                        )
                      )}
                    </SelectContent>
                  </Select>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="diagnostic"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{t("reason")}</FormLabel>
                  <FormControl>
                    <Input
                      placeholder={t("diagnosticPlaceholder")}
                      {...field}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="notes"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{t("notes")}</FormLabel>
                  <FormControl>
                    <Textarea
                      placeholder={t("notesPlaceholder")}
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
                {t("cancelButton")}
              </Button>
              <Button type="submit" disabled={isSubmitting}>
                {isSubmitting
                  ? t("savingButton")
                  : initialData
                  ? t("updateButton")
                  : t("scheduleButton")}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
