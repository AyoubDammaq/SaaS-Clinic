import { useState } from "react";
import { differenceInYears, format, parseISO } from "date-fns";
import { fr } from "date-fns/locale";
import { enUS } from "date-fns/locale";
import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
} from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Badge } from "@/components/ui/badge";
import { Switch } from "@/components/ui/switch";
import { Label } from "@/components/ui/label";
import { User, CalendarCheck, Settings } from "lucide-react";
import { toast } from "sonner";
import { PatientForm } from "@/components/patients/PatientForm";
import { useTranslation } from "@/hooks/useTranslation";
import { Patient } from "@/types/patient";

interface PatientProfileProps {
  patient: {
    id: string;
    name: string;
    email: string;
    phone: string;
    dateOfBirth: string;
    gender: "M" | "F";
    address?: string;
    lastVisit?: string;
  };
  onEditPatient: (patient: Partial<Patient>) => void;
}

export function PatientProfile({
  patient,
  onEditPatient,
}: PatientProfileProps) {
  const [activeTab, setActiveTab] = useState("profile");
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [emailNotifications, setEmailNotifications] = useState(true);
  const [appointmentReminders, setAppointmentReminders] = useState(true);
  const { t, language } = useTranslation("patients");

  const calculateAge = (dateOfBirth: string) =>
    differenceInYears(new Date(), new Date(dateOfBirth));

  const getInitials = (name: string) =>
    name
      .split(" ")
      .map((n) => n[0])
      .join("")
      .toUpperCase();

  const saveSettings = () => {
    toast.success(t("settings_saved"));
  };

  const getGenderLabel = (gender: string) => {
    switch (gender) {
      case "M":
        return t("male") || "Male";
      case "F":
        return t("female") || "Female";
      default:
        return gender;
    }
  };

  const locale = language === "fr" ? fr : enUS;

  const formatDateOfBirth = (dateStr: string) => {
    return format(parseISO(dateStr), "PPP", { locale });
  };

  // Prepare initialData for PatientForm
  const initialData = {
    id: patient.id,
    prenom: patient.name.split(" ")[0] || "",
    nom: patient.name.split(" ").slice(1).join(" ") || "",
    email: patient.email,
    telephone: patient.phone,
    dateNaissance: patient.dateOfBirth,
    sexe: patient.gender as "M" | "F",
    adresse: patient.address || "",
  };

  return (
    <>
      <Card className="w-full">
        <CardHeader>
          <div className="flex items-center gap-2">
            <User className="h-5 w-5 text-primary" />
            <CardTitle>{t("patient_profile")}</CardTitle>
          </div>
          <CardDescription>{t("manage_your_info")}</CardDescription>
        </CardHeader>
        <CardContent>
          <Tabs
            defaultValue="profile"
            value={activeTab}
            onValueChange={setActiveTab}
            className="w-full"
          >
            <TabsList className="mb-4 grid w-full grid-cols-2">
              <TabsTrigger value="profile">{t("profile")}</TabsTrigger>
              {/* <TabsTrigger value="history">{t("history")}</TabsTrigger> */}
              <TabsTrigger value="settings">{t("settings")}</TabsTrigger>
            </TabsList>

            <TabsContent value="profile" className="space-y-4">
              <div className="flex flex-col md:flex-row gap-6">
                <div className="flex flex-col items-center">
                  <Avatar className="h-24 w-24">
                    <AvatarFallback className="text-xl bg-green-600 text-white">
                      {getInitials(patient.name)}
                    </AvatarFallback>
                  </Avatar>
                  <Badge variant="secondary" className="mt-2">
                    {t("patient")}
                  </Badge>
                </div>
                <div className="flex-1 space-y-4">
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div>
                      <div className="text-sm text-muted-foreground">
                        {t("name")}
                      </div>
                      <div className="font-medium">{patient.name}</div>
                    </div>
                    <div>
                      <div className="text-sm text-muted-foreground">
                        {t("email")}
                      </div>
                      <div className="font-medium">{patient.email}</div>
                    </div>
                    <div>
                      <div className="text-sm text-muted-foreground">
                        {t("phone")}
                      </div>
                      <div className="font-medium">{patient.phone}</div>
                    </div>
                    <div>
                      <div className="text-sm text-muted-foreground">
                        {t("age")}
                      </div>
                      <div className="font-medium">
                        {calculateAge(patient.dateOfBirth)}
                      </div>
                    </div>
                    <div>
                      <div className="text-sm text-muted-foreground">
                        {t("gender")}
                      </div>
                      <div className="font-medium">{getGenderLabel(patient.gender)}</div>
                    </div>
                    <div>
                      <div className="text-sm text-muted-foreground">
                        {t("dob")}
                      </div>
                      <div className="font-medium">{formatDateOfBirth(patient.dateOfBirth)}</div>
                    </div>
                    {patient.address && (
                      <div>
                        <div className="text-sm text-muted-foreground">
                          {t("address")}
                        </div>
                        <div className="font-medium">{patient.address}</div>
                      </div>
                    )}
                  </div>
                  <Button variant="outline" onClick={() => setIsFormOpen(true)}>
                    {t("edit_profile")}
                  </Button>
                </div>
              </div>
            </TabsContent>

            {/* <TabsContent value="history" className="space-y-4">
              <h3 className="text-lg font-medium flex items-center gap-2">
                <CalendarCheck className="h-5 w-5 text-primary" />
                {t("visit_history")}
              </h3>
              <div className="text-muted-foreground">
                {patient.lastVisit
                  ? `${t("last_visit")}: ${patient.lastVisit}`
                  : t("no_visits")}
              </div>
            </TabsContent> */}

            <TabsContent value="settings" className="space-y-6">
              <h3 className="text-lg font-medium flex items-center gap-2">
                <Settings className="h-5 w-5 text-primary" />
                {t("notification_preferences")}
              </h3>
              <div className="space-y-4 max-w-md">
                <div className="flex items-center justify-between">
                  <Label
                    htmlFor="email-notifications"
                    className="flex flex-col space-y-1"
                  >
                    <span>{t("email_notifications")}</span>
                    <span className="font-normal text-sm text-muted-foreground">
                      {t("receive_important_updates")}
                    </span>
                  </Label>
                  <Switch
                    id="email-notifications"
                    checked={emailNotifications}
                    onCheckedChange={setEmailNotifications}
                  />
                </div>

                <div className="flex items-center justify-between">
                  <Label
                    htmlFor="appointment-reminders"
                    className="flex flex-col space-y-1"
                  >
                    <span>{t("appointment_reminders")}</span>
                    <span className="font-normal text-sm text-muted-foreground">
                      {t("receive_appointment_reminders")}
                    </span>
                  </Label>
                  <Switch
                    id="appointment-reminders"
                    checked={appointmentReminders}
                    onCheckedChange={setAppointmentReminders}
                  />
                </div>
              </div>

              <Button onClick={saveSettings}>{t("save_settings")}</Button>
            </TabsContent>
          </Tabs>
        </CardContent>
      </Card>

      <PatientForm
        isOpen={isFormOpen}
        onClose={() => setIsFormOpen(false)}
        onSubmit={(data) => {
          const updatedPatient: Partial<Patient> = {
            id: patient.id,
            nom: data.nom,
            prenom: data.prenom,
            email: data.email,
            telephone: data.telephone,
            dateNaissance: data.dateNaissance,
            sexe: data.sexe as "M" | "F",
            adresse: data.adresse,
          };
          onEditPatient(updatedPatient);
          setIsFormOpen(false);
          toast.success(t("profile_updated"));
        }}
        initialData={initialData}
      />
    </>
  );
}