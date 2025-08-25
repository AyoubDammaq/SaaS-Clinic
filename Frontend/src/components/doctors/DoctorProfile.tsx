import { useEffect, useRef, useState, useCallback } from "react";
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
import { Switch } from "@/components/ui/switch";
import { Label } from "@/components/ui/label";
import { Badge } from "@/components/ui/badge";
import { Stethoscope, Settings } from "lucide-react";
import { toast } from "sonner";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "@/hooks/useTranslation";
import { useAuth } from "@/hooks/useAuth";
import { useDoctors } from "@/hooks/useDoctors";
import { Doctor } from "@/types/doctor";
import { DoctorSchedule } from "./DoctorSchedule";

interface DoctorProfileProps {
  doctor: Doctor;
  onEdit: (doctor: Doctor) => void;
  clinics: { id: string; name: string }[];
  userRole: string;
}

// Specialty translation
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

export function DoctorProfile({
  doctor,
  onEdit,
  clinics,
  userRole,
}: DoctorProfileProps) {
  const { t } = useTranslation("doctors");
  const { fetchDoctorById } = useDoctors();
  const [fullDoctor, setFullDoctor] = useState<Doctor | null>(null);
  const [activeTab, setActiveTab] = useState("profile");
  const [emailNotifications, setEmailNotifications] = useState(true);
  const [smsNotifications, setSmsNotifications] = useState(false);
  const navigate = useNavigate();
  const doctorCache = useRef<Record<string, Doctor>>({});

  // Translation wrapper with fallback
  const translate = useCallback((key: string) => t(key) || key, [t]);

  const translateSpecialty = useCallback(
    (specialty: string) => {
      const specialtyIndex = specialtyValues.findIndex(
        (value) => value.toLowerCase() === specialty.toLowerCase()
      );
      return specialtyIndex !== -1
        ? translate(specialtyKeys[specialtyIndex])
        : translate("unknownSpecialty");
    },
    [translate]
  );

  // Fetch full doctor data
  useEffect(() => {
    if (doctor.id) {
      if (doctorCache.current[doctor.id]) {
        setFullDoctor(doctorCache.current[doctor.id]);
        return;
      }

      fetchDoctorById(doctor.id).then((data) => {
        if (data) {
          doctorCache.current[doctor.id] = data;
          setFullDoctor(data);
        } else {
          toast.error(translate("errorFetchingDoctor"));
        }
      });
    }
  }, [doctor.id, fetchDoctorById, translate]);

  // Get clinic name
  const clinicName = doctor.cliniqueId
    ? clinics.find((clinic) => clinic.id === doctor.cliniqueId)?.name ||
      translate("unknownClinic")
    : translate("noClinicAssigned");

  // Get initials for avatar
  const getInitials = (prenom: string, nom: string) => {
    return `${prenom[0] || ""}${nom[0] || ""}`.toUpperCase();
  };

  // Save settings
  const saveSettings = () => {
    toast.success(translate("settingsSavedSuccess"));
  };

  return (
    <Card className="w-full">
      <CardHeader>
        <div className="flex items-center gap-2">
          <Stethoscope className="h-5 w-5 text-primary" />
          <CardTitle>{translate("doctorProfileTitle")}</CardTitle>
        </div>
        <CardDescription>{translate("manageProfile")}</CardDescription>
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
            <TabsTrigger value="profile">{translate("profileTab")}</TabsTrigger>
            {userRole !== "Patient" && (
              <TabsTrigger value="settings">
                {translate("settingsTab")}
              </TabsTrigger>
            )}
            <TabsTrigger value="schedule">
              {translate("scheduleTab")}
            </TabsTrigger>
          </TabsList>

          {/* Profile Tab */}
          <TabsContent value="profile" className="space-y-4">
            <div className="flex flex-col md:flex-row gap-6">
              <div className="flex flex-col items-center">
                <Avatar className="h-24 w-24">
                  <AvatarFallback className="text-xl bg-blue-500 text-white">
                    {getInitials(
                      fullDoctor?.prenom || doctor.prenom,
                      fullDoctor?.nom || doctor.nom
                    )}
                  </AvatarFallback>
                </Avatar>
                <Badge variant="secondary" className="mt-2">
                  <Stethoscope className="h-3 w-3 mr-1" />
                  {translateSpecialty(
                    fullDoctor?.specialite || doctor.specialite
                  )}
                </Badge>
                {userRole !== "Patient" && userRole !== "SuperAdmin" && (
                  <Button
                    variant="outline"
                    className="mt-4"
                    onClick={() => onEdit(fullDoctor || doctor)}
                    aria-label={translate("editDoctor")}
                  >
                    {translate("editDoctor")}
                  </Button>
                )}
              </div>

              <div className="flex-1 space-y-4">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <div className="text-sm text-muted-foreground">
                      {translate("firstName")}
                    </div>
                    <div className="font-medium">
                      {fullDoctor?.prenom || doctor.prenom}
                    </div>
                  </div>
                  <div>
                    <div className="text-sm text-muted-foreground">
                      {translate("lastName")}
                    </div>
                    <div className="font-medium">
                      {fullDoctor?.nom || doctor.nom}
                    </div>
                  </div>
                  {userRole !== "Patient" && (
                    <div>
                      <div className="text-sm text-muted-foreground">
                        {translate("email")}
                      </div>
                      <div className="font-medium">
                        {fullDoctor?.email || doctor.email}
                      </div>
                    </div>
                  )}
                  <div>
                    <div className="text-sm text-muted-foreground">
                      {translate("specialty")}
                    </div>
                    <div className="font-medium">
                      {translateSpecialty(
                        fullDoctor?.specialite || doctor.specialite
                      )}
                    </div>
                  </div>
                  {userRole !== "Patient" && (
                    <div>
                      <div className="text-sm text-muted-foreground">
                        {translate("phone")}
                      </div>
                      <div className="font-medium">
                        {fullDoctor?.telephone || doctor.telephone}
                      </div>
                    </div>
                  )}
                  <div>
                    <div className="text-sm text-muted-foreground">
                      {translate("clinic")}
                    </div>
                    <div className="font-medium flex items-center gap-2">
                      {clinicName}
                      {doctor.cliniqueId &&
                        userRole !== "ClinicAdmin" &&
                        userRole !== "Patient" && (
                          <Button
                            variant="link"
                            className="text-sm text-blue-600 p-0 h-auto"
                            onClick={() =>
                              navigate(`/clinics/${doctor.cliniqueId}`)
                            }
                            aria-label={translate("viewClinic")}
                          >
                            {translate("viewClinic")}
                          </Button>
                        )}
                    </div>
                  </div>
                </div>

                {userRole !== "Patient" && (
                  <div className="pt-4">
                    <h3 className="text-lg font-medium mb-2">
                      {translate("specialty")}
                    </h3>
                    <div className="grid grid-cols-2 md:grid-cols-3 gap-2">
                      <Badge variant="outline">
                        <Stethoscope className="h-3 w-3 mr-1" />
                        {translateSpecialty(
                          fullDoctor?.specialite || doctor.specialite
                        )}
                      </Badge>
                    </div>
                  </div>
                )}
              </div>
            </div>
          </TabsContent>

          {/* Settings Tab */}
          {userRole !== "Patient" && (
            <TabsContent value="settings">
              <div className="space-y-6">
                <div>
                  <h3 className="text-lg font-medium mb-4">
                    {translate("notificationPreferences")}
                  </h3>
                  <div className="space-y-4">
                    <div className="flex items-center justify-between">
                      <Label
                        htmlFor="email-notifications"
                        className="flex flex-col space-y-1"
                      >
                        <span>{translate("emailNotifications")}</span>
                        <span className="font-normal text-sm text-muted-foreground">
                          {translate("emailNotificationsDescription")}
                        </span>
                      </Label>
                      <Switch
                        id="email-notifications"
                        checked={emailNotifications}
                        onCheckedChange={setEmailNotifications}
                        aria-label={translate("emailNotifications")}
                      />
                    </div>
                    <div className="flex items-center justify-between">
                      <Label
                        htmlFor="sms-notifications"
                        className="flex flex-col space-y-1"
                      >
                        <span>{translate("smsNotifications")}</span>
                        <span className="font-normal text-sm text-muted-foreground">
                          {translate("smsNotificationsDescription")}
                        </span>
                      </Label>
                      <Switch
                        id="sms-notifications"
                        checked={smsNotifications}
                        onCheckedChange={setSmsNotifications}
                        aria-label={translate("smsNotifications")}
                      />
                    </div>
                  </div>
                  <Button onClick={saveSettings} className="mt-4">
                    {translate("saveSettings")}
                  </Button>
                </div>
                <div>
                  <h3 className="text-lg font-medium mb-4">
                    {translate("accountSettings")}
                  </h3>
                  <div className="space-y-4">
                    <Button variant="outline">
                      {translate("changePassword")}
                    </Button>
                    <Button variant="outline" className="text-red-500">
                      {translate("disableAccount")}
                    </Button>
                  </div>
                </div>
              </div>
            </TabsContent>
          )}

          {/* Schedule Tab */}
          <TabsContent value="schedule">
            <div className="space-y-6">
              <h3 className="text-lg font-medium">
                {translate("scheduleTab")}
              </h3>
              <DoctorSchedule doctorId={doctor.id} />
            </div>
          </TabsContent>
        </Tabs>
      </CardContent>
    </Card>
  );
}
