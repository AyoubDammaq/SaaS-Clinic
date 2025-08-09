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
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Button } from "@/components/ui/button";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";
import { Calendar } from "@/components/ui/calendar";
import { format } from "date-fns";
import { CalendarIcon } from "lucide-react";
import { cn } from "@/lib/utils";
import { toast } from "sonner";
import { useTranslation } from "@/hooks/useTranslation";
import { Patient } from "@/types/patient";
import { Doctor } from "@/types/doctor";
import { AppointmentFormData } from "@/types/rendezvous";
import { useDisponibilite } from "@/hooks/useDisponibilites";
import { CreneauDisponibleDto } from "@/types/disponibilite";

const getAppointmentFormSchema = (t: (key: string) => string) =>
  z.object({
    patientId: z.string().min(1, { message: t("selectPatientRequired") }),
    medecinId: z.string().min(1, { message: t("selectDoctorRequired") }),
    date: z.date({ required_error: t("dateRequired") }),
    time: z.string().min(1, { message: t("timeRequired") }),
    reason: z.string().min(5, { message: t("reasonMinimum") }),
  });

interface AppointmentFormProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: AppointmentFormData) => void;
  initialData?: AppointmentFormData;
  doctors: Doctor[];
  patients?: Patient[]; // Optional for when the patient is logged in
  patientId?: string; // Current patient ID if a patient is logged in
}

export const AppointmentForm = ({
  isOpen,
  onClose,
  onSubmit,
  initialData,
  doctors,
  patients = [],
  patientId,
}: AppointmentFormProps) => {
  const { t } = useTranslation("appointments");
  const tCommon = useTranslation("common").t;
  const [isSubmitting, setIsSubmitting] = useState(false);

  const AppointmentFormSchema = getAppointmentFormSchema(t);
  type AppointmentFormValues = z.infer<typeof AppointmentFormSchema>;

  const { getAvailableSlots } = useDisponibilite();
  const [availableSlots, setAvailableSlots] = useState<CreneauDisponibleDto[]>(
    []
  );

  const form = useForm<AppointmentFormValues>({
    resolver: zodResolver(AppointmentFormSchema),
    defaultValues: {
      patientId: patientId || "",
      medecinId: "",
      date: undefined,
      time: "",
      reason: "",
    },
  });

  const medecinId = form.watch("medecinId");
  const date = form.watch("date");

  useEffect(() => {
    if (isOpen) {
      // Reset form when dialog opens
      if (!initialData) {
        form.reset({
          patientId: patientId || "",
          medecinId: "",
          date: undefined,
          time: "",
          reason: "",
        });
        setAvailableSlots([]); // Reset available slots
      } else {
        form.reset({
          patientId: initialData.patientId || patientId || "",
          medecinId: initialData.medecinId || "",
          date: initialData.dateHeure ? new Date(initialData.dateHeure) : undefined,
          time: initialData.time || "",
          reason: initialData.reason || "",
        });
      }
      console.log(
        "[AppointmentForm] Form reset with initial data:",
        initialData
      );
    }
  }, [isOpen, initialData, patientId, form]);

  useEffect(() => {
    const fetchSlots = async () => {
      if (medecinId && date) {
        try {
          const formattedDate =
            typeof date === "string" ? date : date.toLocaleDateString("fr-CA");

          const slots = await getAvailableSlots(medecinId, formattedDate);
          setAvailableSlots(slots);
        } catch (err) {
          setAvailableSlots([]);
          console.error("Erreur lors du chargement des créneaux disponibles");
        }
      } else {
        setAvailableSlots([]); // Clear slots if no doctor or date selected
      }
    };

    fetchSlots();
  }, [medecinId, date, getAvailableSlots]);

  const handleSubmit = async (data: AppointmentFormValues) => {
    console.log("[AppointmentForm] Validated form data:", data);
    if (patientId) {
      data.patientId = patientId; // Force patientId for logged-in patient
    }
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

      const requestPayload = {
        patientId: data.patientId,
        medecinId: data.medecinId,
        dateHeure: localIso,
        commentaire: data.reason,
      };

      await onSubmit({
        ...requestPayload,
      });
      toast.success(
        initialData ? t("appointmentUpdated") : t("appointmentCreated")
      );
      form.reset();
      onClose();
    } catch (error) {
      console.error("[AppointmentForm] Submission error:", error);
      toast.error(t("appointmentError"));
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="sm:max-w-[550px]">
        <DialogHeader>
          <DialogTitle>
            {initialData ? t("editAppointment") : t("scheduleNewAppointment")}
          </DialogTitle>
        </DialogHeader>
        <Form {...form}>
          <form
            onSubmit={form.handleSubmit(handleSubmit)}
            className="space-y-4"
          >
            {/* If patient ID is not provided, show patient select */}
            {!patientId && (
              <FormField
                control={form.control}
                name="patientId"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>{tCommon("patient")}</FormLabel>
                    <Select
                      onValueChange={field.onChange}
                      defaultValue={field.value}
                    >
                      <FormControl>
                        <SelectTrigger>
                          <SelectValue placeholder={t("selectPatient")} />
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
            )}

            <FormField
              control={form.control}
              name="medecinId"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{tCommon("doctor")}</FormLabel>
                  <Select
                    onValueChange={field.onChange}
                    defaultValue={field.value}
                  >
                    <FormControl>
                      <SelectTrigger>
                        <SelectValue placeholder={t("selectDoctor")} />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      {doctors
                        .filter((doctor) => !!doctor.cliniqueId)
                        .map((doctor) => (
                          <SelectItem key={doctor.id} value={doctor.id}>
                            {doctor.prenom} {doctor.nom}{" "}
                            {doctor.specialite ? `- ${doctor.specialite}` : ""}
                          </SelectItem>
                        ))}
                    </SelectContent>
                  </Select>
                  <FormMessage />
                </FormItem>
              )}
            />
            <div className="grid grid-cols-2 gap-4">
              {/* Date Picker */}
              <FormField
                control={form.control}
                name="date"
                render={({ field }) => (
                  <FormItem className="flex flex-col">
                    <FormLabel>{t("date")}</FormLabel>
                    <Popover>
                      <PopoverTrigger asChild>
                        <FormControl>
                          <Button
                            variant={"outline"}
                            className={cn(
                              "pl-3 text-left font-normal",
                              !field.value && "text-muted-foreground"
                            )}
                          >
                            {field.value ? (
                              format(field.value, "PPP")
                            ) : (
                              <span>{t("selectDate")}</span>
                            )}
                            <CalendarIcon className="ml-auto h-4 w-4 opacity-50" />
                          </Button>
                        </FormControl>
                      </PopoverTrigger>
                      <PopoverContent className="w-auto p-0" align="start">
                        <Calendar
                          mode="single"
                          selected={field.value}
                          onSelect={field.onChange}
                          disabled={(date) =>
                            date < new Date() || date < new Date("1900-01-01")
                          }
                          initialFocus
                          className="p-3 pointer-events-auto"
                        />
                      </PopoverContent>
                    </Popover>
                    <FormMessage />
                  </FormItem>
                )}
              />

              {/* Time Selector */}
              <FormField
                control={form.control}
                name="time"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>{t("time")}</FormLabel>
                    <FormControl>
                      <Select
                        onValueChange={field.onChange}
                        value={field.value}
                      >
                        <FormControl>
                          <SelectTrigger>
                            <SelectValue placeholder={t("selectTime")} />
                          </SelectTrigger>
                        </FormControl>
                        <SelectContent>
                          {availableSlots.length === 0 ? (
                            <div className="px-3 py-2 text-sm text-muted-foreground">
                              {t("noAvailableSlots")}
                            </div>
                          ) : (
                            availableSlots
                              .filter(
                                (slot) =>
                                  new Date(slot.dateHeureDebut) > new Date()
                              )
                              .map((slot, index) => {
                                const start = new Date(slot.dateHeureDebut);
                                return (
                                  <SelectItem
                                    key={index}
                                    value={start.toTimeString().slice(0, 5)}
                                  >
                                    {start.toLocaleTimeString("fr-FR", {
                                      hour: "2-digit",
                                      minute: "2-digit",
                                    })}
                                  </SelectItem>
                                );
                              })
                          )}
                        </SelectContent>
                      </Select>
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            <FormField
              control={form.control}
              name="reason"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{t("reasonForAppointment")}</FormLabel>
                  <FormControl>
                    <Input placeholder={t("reasonPlaceholder")} {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <DialogFooter>
              <Button type="button" variant="outline" onClick={onClose}>
                {tCommon("cancel")}
              </Button>
              <Button type="submit" disabled={isSubmitting}>
                {isSubmitting
                  ? t("saving")
                  : initialData
                  ? t("update")
                  : t("schedule")}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
};