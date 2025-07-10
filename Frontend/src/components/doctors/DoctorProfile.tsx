import { useState } from "react";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
  CardFooter,
} from "@/components/ui/card";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Doctor } from "@/types/doctor"; // Import corrigé pour utiliser le type global
import { toast } from "sonner";
import { Switch } from "@/components/ui/switch";
import { Label } from "@/components/ui/label";
import { DoctorSchedule } from "./DoctorSchedule";
import { useNavigate } from "react-router-dom";

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
  const [activeTab, setActiveTab] = useState("profile");
  const [emailNotifications, setEmailNotifications] = useState(true);
  const [smsNotifications, setSmsNotifications] = useState(false);
  const navigate = useNavigate();

  // Find clinic name by id
  const clinicName = doctor.cliniqueId
    ? clinics.find((clinic) => clinic.id === doctor.cliniqueId)?.name ||
      "Unknown Clinic"
    : "None";

  // Get initials from first and last name
  const getInitials = (prenom: string, nom: string) => {
    return `${prenom[0]}${nom[0]}`.toUpperCase();
  };

  const saveSettings = () => {
    toast.success("Settings saved successfully");
  };

  return (
    <Card className="w-full">
      <CardHeader>
        <CardTitle>Doctor Profile</CardTitle>
        <CardDescription>
          Manage your professional information and settings
        </CardDescription>
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
              userRole === "Patient" ? "grid-cols-2" : "grid-cols-3"
            }`}
          >
            <TabsTrigger value="profile">Profile</TabsTrigger>
            {userRole !== "Patient" && (
              <TabsTrigger value="settings">Settings</TabsTrigger>
            )}
            <TabsTrigger value="schedule">Schedule</TabsTrigger>
          </TabsList>

          <TabsContent value="profile" className="space-y-4">
            <div className="flex flex-col md:flex-row gap-6">
              <div className="flex flex-col items-center">
                <Avatar className="h-24 w-24">
                  <AvatarFallback className="text-xl bg-clinic-500 text-white">
                    {getInitials(doctor.prenom, doctor.nom)}
                  </AvatarFallback>
                </Avatar>
                {userRole !== "Patient" && (
                  <Button
                    variant="outline"
                    className="mt-4"
                    onClick={() => onEdit(doctor)}
                  >
                    Edit Profile
                  </Button>
                )}
              </div>

              <div className="flex-1 space-y-4">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <div className="text-sm text-muted-foreground">
                      First Name
                    </div>
                    <div className="font-medium">{doctor.prenom}</div>
                  </div>
                  <div>
                    <div className="text-sm text-muted-foreground">
                      Last Name
                    </div>
                    <div className="font-medium">{doctor.nom}</div>
                  </div>
                  <div>
                    <div className="text-sm text-muted-foreground">Email</div>
                    <div className="font-medium">{doctor.email}</div>
                  </div>
                  <div>
                    <div className="text-sm text-muted-foreground">
                      Specialty
                    </div>
                    <div className="font-medium">{doctor.specialite}</div>
                  </div>
                  <div>
                    <div className="text-sm text-muted-foreground">Phone</div>
                    <div className="font-medium">{doctor.telephone}</div>
                  </div>
                  <div>
                    <div className="text-sm text-muted-foreground">Clinic</div>
                    <div className="font-medium flex items-center gap-2">
                      {clinicName}
                      {doctor.cliniqueId && userRole !== "ClinicAdmin" && (
                        <Button
                          variant="link"
                          className="text-sm text-blue-600 p-0 h-auto"
                          onClick={() =>
                            navigate(`/clinics/${doctor.cliniqueId}`)
                          }
                        >
                          Voir la clinique
                        </Button>
                      )}
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </TabsContent>

          {userRole !== "Patient" && (
            <TabsContent value="settings">
              <div className="space-y-6">
                <div>
                  <h3 className="text-lg font-medium mb-4">
                    Notification Preferences
                  </h3>
                  <div className="space-y-4">
                    <div className="flex items-center justify-between">
                      <Label
                        htmlFor="email-notifications"
                        className="flex flex-col space-y-1"
                      >
                        <span>Email Notifications</span>
                        <span className="font-normal text-sm text-muted-foreground">
                          Receive updates about appointments and patient results
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
                        <span>SMS Notifications</span>
                        <span className="font-normal text-sm text-muted-foreground">
                          Receive text messages for urgent matters
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
                    Save Settings
                  </Button>
                </div>

                <div>
                  <h3 className="text-lg font-medium mb-4">Account Settings</h3>
                  <div className="space-y-4">
                    <Button variant="outline">Change Password</Button>
                    <Button variant="outline" className="text-red-500">
                      Deactivate Account
                    </Button>
                  </div>
                </div>
              </div>
            </TabsContent>
          )}

          <TabsContent value="schedule">
            <DoctorSchedule doctorId={doctor.id} />
          </TabsContent>
        </Tabs>
      </CardContent>
    </Card>
  );
}
