import { useState, useEffect } from "react";
import { z } from "zod";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter
} from "@/components/ui/dialog";
import {
  Form, FormControl, FormField, FormItem, FormLabel, FormMessage
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import {
  Select, SelectContent, SelectItem, SelectTrigger, SelectValue
} from "@/components/ui/select";
import { API_ENDPOINTS } from "@/config/api";
import { useAuth } from "@/contexts/AuthContext";
import { toast } from "sonner";

const doctorFormSchema = z.object({
  prenom: z.string().min(2, { message: "First name is required" }),
  nom: z.string().min(2, { message: "Last name is required" }),
  email: z.string().email({ message: "Please enter a valid email address." }),
  telephone: z.string().min(5, { message: "Phone number must be at least 5 characters." }),
  specialite: z.string().min(1, { message: "Please select a specialty." }),
  cliniqueId: z.string().nullable().optional(),
  photoUrl: z.string().url().or(z.literal("")).optional(),
  id: z.string().optional()
});

type DoctorFormValues = z.infer<typeof doctorFormSchema>;

interface DoctorFormProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: DoctorFormValues) => void;
  initialData?: Partial<DoctorFormValues>;
  clinics?: { id: string; name: string }[];
}

const specialties = [
  "General Practitioner", "Pediatrician", "Cardiologist", "Dermatologist",
  "Neurologist", "Psychiatrist", "Ophthalmologist", "Gynecologist",
  "Orthopedist", "Dentist"
];

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
  const { token } = useAuth(); // ✅ Use AuthContext
  const [isSubmitting, setIsSubmitting] = useState(false);

  const form = useForm<DoctorFormValues>({
    resolver: zodResolver(doctorFormSchema),
    defaultValues: {
      ...defaultValues,
      ...initialData,
    },
  });

    useEffect(() => {
      console.log("Resetting form with initialData:", initialData);
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
        toast.error("You are not authenticated.");
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
          "Authorization": `Bearer ${token}`,
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

      toast.success(isUpdating ? "Doctor updated!" : "Doctor created!");
      form.reset();
      onClose();
      onSubmit({ ...data, id: initialData?.id || undefined }); 
    } catch (error) {
      console.error("Error submitting doctor form:", error);
      toast.error("Failed to save doctor. Please try again.");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="sm:max-w-[425px]">
        <DialogHeader>
          <DialogTitle>{initialData ? "Edit Doctor" : "Add New Doctor"}</DialogTitle>
        </DialogHeader>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(handleSubmit)} className="space-y-4">
            {/** First Name */}
            <FormField
              control={form.control}
              name="prenom"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>First Name</FormLabel>
                  <FormControl>
                    <Input placeholder="Dr. Jane" {...field} />
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
                  <FormLabel>Last Name</FormLabel>
                  <FormControl>
                    <Input placeholder="Smith" {...field} />
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
                  <FormLabel>Email</FormLabel>
                  <FormControl>
                    <Input placeholder="doctor@example.com" type="email" {...field} />
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
                  <FormLabel>Phone</FormLabel>
                  <FormControl>
                    <Input placeholder="555-5678" {...field} />
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
                  <FormLabel>Specialty</FormLabel>
                  <Select onValueChange={field.onChange} defaultValue={field.value}>
                    <FormControl>
                      <SelectTrigger>
                        <SelectValue placeholder="Select a specialty" />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      {specialties.map((specialty) => (
                        <SelectItem key={specialty} value={specialty}>
                          {specialty}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  <FormMessage />
                </FormItem>
              )}
            />
            {/** Clinic (if available) */}
            {clinics.length > 0 && (
              <FormField
                control={form.control}
                name="cliniqueId"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Clinic</FormLabel>
                    <Select value={field.value} onValueChange={field.onChange}>
                      <FormControl>
                        <SelectTrigger>
                          <SelectValue placeholder="Select a clinic" />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        {clinics.map((clinic) => (
                          <SelectItem key={clinic.id} value={clinic.id}>
                            {clinic.name}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                    <FormMessage />
                  </FormItem>
                )}
              />
            )}
            {/** Photo URL */}
            <FormField
              control={form.control}
              name="photoUrl"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Photo URL</FormLabel>
                  <FormControl>
                    <Input placeholder="https://..." {...field} />
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
                {isSubmitting ? "Saving..." : initialData ? "Update" : "Create"}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
