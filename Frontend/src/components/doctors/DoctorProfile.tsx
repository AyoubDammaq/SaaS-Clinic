import { useState } from "react";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
} from "@/components/ui/card";
import { Tabs, TabsList, TabsTrigger, TabsContent } from "@/components/ui/tabs";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Doctor } from "@/types/doctor";
import { toast } from "sonner";
import { Switch } from "@/components/ui/switch";
import { Label } from "@/components/ui/label";
import { DoctorSchedule } from "./DoctorSchedule";
import { useNavigate } from "react-router-dom";
import { Stethoscope } from "lucide-react";
import { useTranslation } from "@/hooks/useTranslation";

interface DoctorProfileProps {
  doctor: Doctor;
  onEdit: (doctor: Doctor) => void;
  clinics: { id: string; name: string }[];
  userRole: string;
}

export function DoctorProfile({
  doctor,
  onEdit,
  clinics,
  userRole,
}: DoctorProfileProps) {
  const { t } = useTranslation("doctors");
  const [activeTab, setActiveTab] = useState("profile");
  const [emailNotifications, setEmailNotifications] = useState(true);
  const [smsNotifications, setSmsNotifications] = useState(false);
  const navigate = useNavigate();

  // Définir les valeurs brutes des spécialités (pour compatibilité avec la base de données)
  const specialtyValues = [
    "General Practitioner",
    "Pediatrician",
    "Cardiologist",
    "Dermatologist",
    "Neurologist",
    "Psychiatrist",
    "Ophthalmologist",
    "Gynecologist",
    "Orthopedist",
    "Dentist",
  ];

  // Définir les clés de traduction correspondantes
  const specialtyKeys = [
    "generalPractitioner",
    "pediatrician",
    "cardiologist",
    "dermatologist",
    "neurologist",
    "psychiatrist",
    "ophthalmologist",
    "gynecologist",
    "orthopedist",
    "dentist",
  ];

  const clinicName = doctor.cliniqueId
    ? clinics.find((clinic) => clinic.id === doctor.cliniqueId)?.name ||
      t("unknownClinic")
    : t("noClinicAssigned");

  const getInitials = (prenom: string, nom: string) => {
    return `${prenom[0]}${nom[0]}`.toUpperCase();
  };

  // Traduire la spécialité
  const translateSpecialty = (specialty: string) => {
    const specialtyIndex = specialtyValues.findIndex(
      (value) => value.toLowerCase() === specialty.toLowerCase()
    );
    return specialtyIndex !== -1
      ? t(specialtyKeys[specialtyIndex])
      : t("unknownSpecialty");
  };

  const saveSettings = () => {
    toast.success(t("settingsSavedSuccess"));
  };

  return (
    <Card className="w-full">
      <CardHeader>
        <div className="flex items-center gap-2">
          <Stethoscope className="h-5 w-5 text-primary" />
          <CardTitle>{t("doctorProfileTitle")}</CardTitle>
        </div>
        <CardDescription>{t("manageProfile")}</CardDescription>
      </CardHeader>
      <CardContent>
        <Tabs
          defaultValue="profile"
          value={activeTab}
          onValueChange={setActiveTab}
          className="w-full"
        >
          <TabsList
            className={`mb-4 grid w-full ${
              userRole !== "Patient" ? "grid-cols-3" : "grid-cols-2"
            }`}
          >
            <TabsTrigger value="profile">{t("profileTab")}</TabsTrigger>
            {userRole !== "Patient" && (
              <TabsTrigger value="settings">{t("settingsTab")}</TabsTrigger>
            )}
            <TabsTrigger value="schedule">{t("scheduleTab")}</TabsTrigger>
          </TabsList>

          {/* Profil */}
          <TabsContent value="profile" className="space-y-4">
            <div className="flex flex-col md:flex-row gap-6">
              <div className="flex flex-col items-center">
                <Avatar className="h-24 w-24">
                  <AvatarFallback className="text-xl bg-clinic-500 text-white">
                    {getInitials(doctor.prenom, doctor.nom)}
                  </AvatarFallback>
                </Avatar>

                {/* Montrer "Modifier Profil" sauf si Patient ou SuperAdmin */}
                {userRole !== "Patient" && userRole !== "SuperAdmin" && (
                  <Button
                    variant="outline"
                    className="mt-4"
                    onClick={() => onEdit(doctor)}
                    aria-label={t("editDoctor")}
                  >
                    {t("editDoctor")}
                  </Button>
                )}
              </div>

              <div className="flex-1 space-y-4">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <div className="text-sm text-muted-foreground">
                      {t("firstName")}
                    </div>
                    <div className="font-medium">{doctor.prenom}</div>
                  </div>

                  <div>
                    <div className="text-sm text-muted-foreground">
                      {t("lastName")}
                    </div>
                    <div className="font-medium">{doctor.nom}</div>
                  </div>

                  {/* Masquer l'email pour les patients */}
                  {userRole !== "Patient" && (
                    <div>
                      <div className="text-sm text-muted-foreground">
                        {t("email")}
                      </div>
                      <div className="font-medium">{doctor.email}</div>
                    </div>
                  )}

                  <div>
                    <div className="text-sm text-muted-foreground">
                      {t("specialty")}
                    </div>
                    <div className="font-medium">
                      {translateSpecialty(doctor.specialite)}
                    </div>
                  </div>

                  {/* Masquer le téléphone pour les patients */}
                  {userRole !== "Patient" && (
                    <div>
                      <div className="text-sm text-muted-foreground">
                        {t("phone")}
                      </div>
                      <div className="font-medium">{doctor.telephone}</div>
                    </div>
                  )}

                  <div>
                    <div className="text-sm text-muted-foreground">
                      {t("clinic")}
                    </div>
                    <div className="font-medium flex items-center gap-2">
                      {clinicName}

                      {/* Montrer le bouton "Voir la clinique" seulement si admin */}
                      {doctor.cliniqueId &&
                        userRole !== "ClinicAdmin" &&
                        userRole !== "Patient" && (
                          <Button
                            variant="link"
                            className="text-sm text-blue-600 p-0 h-auto"
                            onClick={() =>
                              navigate(`/clinics/${doctor.cliniqueId}`)
                            }
                            aria-label={t("viewClinic")}
                          >
                            {t("viewClinic")}
                          </Button>
                        )}
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </TabsContent>

          {/* Paramètres (notifications + compte) */}
          {userRole !== "Patient" && (
            <TabsContent value="settings">
              <div className="space-y-6">
                <div>
                  <h3 className="text-lg font-medium mb-4">
                    {t("notificationPreferences")}
                  </h3>
                  <div className="space-y-4">
                    <div className="flex items-center justify-between">
                      <Label
                        htmlFor="email-notifications"
                        className="flex flex-col space-y-1"
                      >
                        <span>{t("emailNotifications")}</span>
                        <span className="font-normal text-sm text-muted-foreground">
                          {t("emailNotificationsDescription")}
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
                        htmlFor="sms-notifications"
                        className="flex flex-col space-y-1"
                      >
                        <span>{t("smsNotifications")}</span>
                        <span className="font-normal text-sm text-muted-foreground">
                          {t("smsNotificationsDescription")}
                        </span>
                      </Label>
                      <Switch
                        id="sms-notifications"
                        checked={smsNotifications}
                        onCheckedChange={setSmsNotifications}
                      />
                    </div>
                  </div>
                  <Button onClick={saveSettings} className="mt-4">
                    {t("saveSettings")}
                  </Button>
                </div>

                <div>
                  <h3 className="text-lg font-medium mb-4">
                    {t("accountSettings")}
                  </h3>
                  <div className="space-y-4">
                    <Button variant="outline">{t("changePassword")}</Button>
                    <Button variant="outline" className="text-red-500">
                      {t("disableAccount")}
                    </Button>
                  </div>
                </div>
              </div>
            </TabsContent>
          )}

          {/* Planning */}
          <TabsContent value="schedule">
            <DoctorSchedule doctorId={doctor.id} />
          </TabsContent>
        </Tabs>
      </CardContent>
    </Card>
  );
}