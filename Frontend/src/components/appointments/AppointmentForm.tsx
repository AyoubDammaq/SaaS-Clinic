
import { useState } from "react";
import { z } from "zod";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { 
  Dialog, 
  DialogContent, 
  DialogHeader, 
  DialogTitle, 
  DialogFooter 
} from "@/components/ui/dialog";
import { 
  Form, 
  FormControl, 
  FormField, 
  FormItem, 
  FormLabel, 
  FormMessage 
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { 
  Select, 
  SelectContent, 
  SelectItem, 
  SelectTrigger, 
  SelectValue 
} from "@/components/ui/select";
import { Button } from "@/components/ui/button";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import { Calendar } from "@/components/ui/calendar";
import { format } from "date-fns";
import { CalendarIcon } from "lucide-react";
import { cn } from "@/lib/utils";
import { toast } from "sonner";
import { useTranslation } from "@/hooks/useTranslation";

export interface Doctor {
  id: string;
  name: string;
  specialty?: string;
}

export interface Patient {
  id: string;
  name: string;
}

const getAppointmentFormSchema = (t: (key: string) => string) => z.object({
  patientId: z.string().min(1, { message: t('selectPatientRequired') }),
  doctorId: z.string().min(1, { message: t('selectDoctorRequired') }),
  date: z.date({ required_error: t('dateRequired') }),
  time: z.string().min(1, { message: t('timeRequired') }),
  duration: z.string().min(1, { message: t('durationRequired') }),
  reason: z.string().min(5, { message: t('reasonMinimum') }),
});

interface AppointmentFormProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: any) => void;
  initialData?: any;
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
  patientId
}: AppointmentFormProps) => {
  const { t } = useTranslation('appointments');
  const tCommon = useTranslation('common').t;
  const [isSubmitting, setIsSubmitting] = useState(false);
  
  const AppointmentFormSchema = getAppointmentFormSchema(t);
  type AppointmentFormValues = z.infer<typeof AppointmentFormSchema>;
  
  const form = useForm<AppointmentFormValues>({
    resolver: zodResolver(AppointmentFormSchema),
    defaultValues: {
      patientId: initialData?.patientId || patientId || "",
      doctorId: initialData?.doctorId || "",
      date: initialData?.date ? new Date(initialData.date) : undefined,
      time: initialData?.time || "",
      duration: initialData?.duration?.toString() || "30",
      reason: initialData?.reason || "",
    },
  });

  const handleSubmit = async (data: AppointmentFormValues) => {
    setIsSubmitting(true);
    try {
      await onSubmit({
        ...data,
        date: format(data.date, "yyyy-MM-dd"),
        duration: parseInt(data.duration),
      });
      toast.success(initialData ? t('appointmentUpdated') : t('appointmentCreated'));
      form.reset();
      onClose();
    } catch (error) {
      console.error("Error submitting appointment form:", error);
      toast.error(t('appointmentError'));
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="sm:max-w-[550px]">
        <DialogHeader>
          <DialogTitle>
            {initialData ? t('editAppointment') : t('scheduleNewAppointment')}
          </DialogTitle>
        </DialogHeader>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(handleSubmit)} className="space-y-4">
            {/* If patient ID is not provided, show patient select */}
            {!patientId && (
              <FormField
                control={form.control}
                name="patientId"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>{tCommon('patient')}</FormLabel>
                    <Select onValueChange={field.onChange} defaultValue={field.value}>
                      <FormControl>
                        <SelectTrigger>
                          <SelectValue placeholder={t('selectPatient')} />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        {patients.map((patient) => (
                          <SelectItem key={patient.id} value={patient.id}>
                            {patient.name}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                    <FormMessage />
                  </FormItem>
                )}
              />
            )}
            
            <FormField
              control={form.control}
              name="doctorId"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{tCommon('doctor')}</FormLabel>
                  <Select onValueChange={field.onChange} defaultValue={field.value}>
                    <FormControl>
                      <SelectTrigger>
                        <SelectValue placeholder={t('selectDoctor')} />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      {doctors.map((doctor) => (
                        <SelectItem key={doctor.id} value={doctor.id}>
                          {doctor.name} {doctor.specialty ? `- ${doctor.specialty}` : ''}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  <FormMessage />
                </FormItem>
              )}
            />
            
            <div className="grid grid-cols-2 gap-4">
              <FormField
                control={form.control}
                name="date"
                render={({ field }) => (
                  <FormItem className="flex flex-col">
                    <FormLabel>{t('date')}</FormLabel>
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
                              <span>{t('selectDate')}</span>
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
                          disabled={(date) => date < new Date() || date < new Date('1900-01-01')}
                          initialFocus
                          className={cn("p-3 pointer-events-auto")}
                        />
                      </PopoverContent>
                    </Popover>
                    <FormMessage />
                  </FormItem>
                )}
              />
              
              <FormField
                control={form.control}
                name="time"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>{t('time')}</FormLabel>
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
              name="duration"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{t('durationMinutes')}</FormLabel>
                  <Select onValueChange={field.onChange} defaultValue={field.value}>
                    <FormControl>
                      <SelectTrigger>
                        <SelectValue placeholder={t('selectDuration')} />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      <SelectItem value="15">15 {t('min')}</SelectItem>
                      <SelectItem value="30">30 {t('min')}</SelectItem>
                      <SelectItem value="45">45 {t('min')}</SelectItem>
                      <SelectItem value="60">60 {t('min')}</SelectItem>
                      <SelectItem value="90">90 {t('min')}</SelectItem>
                    </SelectContent>
                  </Select>
                  <FormMessage />
                </FormItem>
              )}
            />
            
            <FormField
              control={form.control}
              name="reason"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{t('reasonForAppointment')}</FormLabel>
                  <FormControl>
                    <Input placeholder={t('reasonPlaceholder')} {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            
            <DialogFooter>
              <Button type="button" variant="outline" onClick={onClose}>
                {tCommon('cancel')}
              </Button>
              <Button type="submit" disabled={isSubmitting}>
                {isSubmitting ? t('saving') : initialData ? t('update') : t('schedule')}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
};
