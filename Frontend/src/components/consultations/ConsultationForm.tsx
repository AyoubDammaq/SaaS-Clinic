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

const consultationFormSchema = z.object({
  patientId: z.string().min(1, { message: "Please select a patient." }),
  medecinId: z.string().min(1, { message: "Please select a doctor." }),
  date: z.string().min(1, { message: "Please select a date." }),
  time: z.string().min(1, { message: "Please select a time." }),
  diagnostic: z
    .string()
    .min(5, { message: "Le diagnostic doit contenir au moins 5 caractères." }),
  notes: z.string().optional(),
});

export type ConsultationFormValues = z.infer<typeof consultationFormSchema>;

interface ConsultationFormProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: ConsultationFormValues) => void;
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
      date: initialData?.date || "",
      time: initialData?.time || "",
      diagnostic: initialData?.diagnostic || "",
      notes: initialData?.notes || "",
    },
  });

  // Reset form values when initialData changes
  useEffect(() => {
    if (initialData) {
      form.reset(initialData);
    } else {
      form.reset({
        medecinId: user.role === "Doctor" ? user.medecinId || "" : "",
      });
    }
  }, [initialData, user, form]);

  const handleSubmit = async (data: ConsultationFormValues) => {
    setIsSubmitting(true);
    try {
      const [hours, minutes] = data.time.split(":").map(Number);
      const dateHeure = new Date(data.date);
      dateHeure.setHours(hours, minutes, 0, 0);
      const pad = (num: number) => String(num).padStart(2, "0");
      const localIso = `${dateHeure.getFullYear()}-${pad(
        dateHeure.getMonth() + 1
      )}-${pad(dateHeure.getDate())}T${pad(dateHeure.getHours())}:${pad(
        dateHeure.getMinutes()
      )}:00`;

      const requestPayload: ConsultationFormValues = {
        ...data,
        date: localIso,
      };

      await onSubmit(requestPayload);
      toast.success(
        initialData
          ? "Consultation updated successfully!"
          : "Consultation created successfully!"
      );
      form.reset();
      onClose();
    } catch (error) {
      console.error("Error submitting consultation form:", error);
      toast.error("Failed to save consultation. Please try again.");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="sm:max-w-[550px]">
        <DialogHeader>
          <DialogTitle>
            {initialData ? "Edit Consultation" : "Schedule New Consultation"}
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
                  <FormLabel>Patient</FormLabel>
                  <Select
                    onValueChange={field.onChange}
                    defaultValue={field.value}
                  >
                    <FormControl>
                      <SelectTrigger>
                        <SelectValue placeholder="Sélectionner un patient" />
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
                  <FormMessage />
                </FormItem>
              )}
            />

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <FormField
                control={form.control}
                name="date"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Date</FormLabel>
                    <FormControl>
                      <Input type="date" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="time"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Time</FormLabel>
                    <FormControl>
                      <Input type="time" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>
            <FormField
              control={form.control}
              name="diagnostic"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Diagnostic</FormLabel>
                  <FormControl>
                    <Input placeholder="Indiquer le diagnostic..." {...field} />
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
                  <FormLabel>Notes complémentaires</FormLabel>
                  <FormControl>
                    <Textarea
                      placeholder="Ajouter des détails supplémentaires"
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
              <Button type="submit" disabled={isSubmitting}>
                {isSubmitting
                  ? "Saving..."
                  : initialData
                  ? "Update"
                  : "Schedule"}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
