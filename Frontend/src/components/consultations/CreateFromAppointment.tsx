import { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
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
import { useForm } from "react-hook-form";
import { toast } from "sonner";
import { useTranslation } from "@/hooks/useTranslation";
import {
  ConsultationDTO,
  ConsultationType,
  consultationTypes,
} from "@/types/consultation";
import { useAuth } from "@/hooks/useAuth";

interface Appointment {
  id: string;
  patientId: string;
  medecinId: string;
  date: string;
  raison: string;
  status: string;
}

interface CreateFromAppointmentProps {
  appointment: Appointment | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onCreateConsultation: (data: ConsultationDTO) => Promise<void>;
}

export function CreateFromAppointment({
  appointment,
  open,
  onOpenChange,
  onCreateConsultation,
}: CreateFromAppointmentProps) {
  const { t } = useTranslation("consultations");
  const { user } = useAuth();
  const [isSubmitting, setIsSubmitting] = useState(false);

  const form = useForm<ConsultationDTO>({
    defaultValues: {
      patientId: "",
      medecinId: user?.id || "",
      dateConsultation: "",
      diagnostic: "",
      notes: "",
      type: ConsultationType.ConsultationGenerale,
    },
  });

  // Update form when appointment changes
  useEffect(() => {
    if (appointment) {
      form.reset({
        patientId: appointment.patientId,
        medecinId: appointment.medecinId,
        dateConsultation: appointment.date,
        diagnostic: "",
        notes: "",
        type: ConsultationType.ConsultationGenerale,
      });
    }
  }, [appointment, form]);

  const onSubmit = async (data: ConsultationDTO) => {
    setIsSubmitting(true);
    try {
      await onCreateConsultation(data);
      onOpenChange(false);
      toast.success(t("consultationAddSuccess"));
    } catch (error) {
      console.error("Error creating consultation:", error);
      toast.error(t("errorAddingConsultation"));
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[600px]">
        <DialogHeader>
          <DialogTitle>{t("consultationFromAppointment")}</DialogTitle>
          <DialogDescription>
            Create a new consultation based on this appointment.
          </DialogDescription>
        </DialogHeader>

        <Form {...form}>
          <form
            onSubmit={form.handleSubmit(onSubmit)}
            className="space-y-4 py-4"
          >
            <div className="grid grid-cols-2 gap-4">
              <FormField
                control={form.control}
                name="dateConsultation"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>{t("date")}</FormLabel>
                    <FormControl>
                      <Input type="date" {...field} disabled />
                    </FormControl>
                  </FormItem>
                )}
              />
            </div>

            <FormField
              control={form.control}
              name="type"
              render={({ field, fieldState }) => (
                <FormItem>
                  <FormLabel>
                    {t("typeConsultation") || "Type de consultation"}
                  </FormLabel>
                  <Select
                    onValueChange={(val) => field.onChange(parseInt(val))}
                    value={field.value?.toString()}
                  >
                    <FormControl>
                      <SelectTrigger>
                        <SelectValue
                          placeholder={
                            t("selectType") || "Sélectionner le type"
                          }
                        />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      {Object.entries(consultationTypes).map(
                        ([value, labelKey]) => (
                          <SelectItem key={value} value={value}>
                            {t(labelKey)}
                          </SelectItem>
                        )
                      )}
                    </SelectContent>
                  </Select>
                  <FormMessage>{fieldState.error?.message}</FormMessage>
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="diagnostic"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{t("diagnostic")}</FormLabel>
                  <FormControl>
                    <Input placeholder={t("Write the diagnostic")} {...field} />
                  </FormControl>
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
                      rows={5}
                      placeholder={t("Enter Notes")}
                      {...field}
                    />
                  </FormControl>
                </FormItem>
              )}
            />

            <DialogFooter>
              <Button
                type="button"
                variant="outline"
                onClick={() => onOpenChange(false)}
              >
                Cancel
              </Button>
              <Button type="submit" disabled={isSubmitting}>
                {isSubmitting ? "Creating..." : "Create Consultation"}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
