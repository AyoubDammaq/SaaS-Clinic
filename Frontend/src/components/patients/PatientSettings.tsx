import { useState } from "react";
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Switch } from "@/components/ui/switch";
import { Label } from "@/components/ui/label";
import { toast } from "sonner";

export function PatientSettings() {
  const [notificationsEnabled, setNotificationsEnabled] = useState(true);
  const [appointmentReminders, setAppointmentReminders] = useState(true);
  const [medicationReminders, setMedicationReminders] = useState(true);
  const [newResultsNotifications, setNewResultsNotifications] = useState(true);
  const [isSaving, setIsSaving] = useState(false);

  const handleSaveSettings = () => {
    setIsSaving(true);

    // Simulate API call to save settings
    setTimeout(() => {
      setIsSaving(false);
      toast.success("Settings saved successfully");
    }, 1000);
  };

  return (
    <div className="space-y-6">
      <Card>
        <CardHeader>
          <CardTitle>Notification Settings</CardTitle>
          <CardDescription>
            Configure how and when you receive notifications about your
            healthcare
          </CardDescription>
        </CardHeader>
        <CardContent className="space-y-6">
          <div className="flex items-center justify-between">
            <Label htmlFor="notifications" className="flex flex-col space-y-1">
              <span>Enable notifications</span>
              <span className="font-normal text-sm text-muted-foreground">
                Receive notifications about your appointments and test results
              </span>
            </Label>
            <Switch
              id="notifications"
              checked={notificationsEnabled}
              onCheckedChange={setNotificationsEnabled}
            />
          </div>

          <div className="space-y-4 pl-2 border-l-2 border-muted">
            <div className="flex items-center justify-between">
              <Label
                htmlFor="appointment-reminders"
                className="flex flex-col space-y-1"
              >
                <span>Appointment reminders</span>
                <span className="font-normal text-sm text-muted-foreground">
                  Receive reminders before scheduled appointments
                </span>
              </Label>
              <Switch
                id="appointment-reminders"
                checked={appointmentReminders && notificationsEnabled}
                disabled={!notificationsEnabled}
                onCheckedChange={setAppointmentReminders}
              />
            </div>

            <div className="flex items-center justify-between">
              <Label
                htmlFor="medication-reminders"
                className="flex flex-col space-y-1"
              >
                <span>Medication reminders</span>
                <span className="font-normal text-sm text-muted-foreground">
                  Get reminders to take your prescribed medications
                </span>
              </Label>
              <Switch
                id="medication-reminders"
                checked={medicationReminders && notificationsEnabled}
                disabled={!notificationsEnabled}
                onCheckedChange={setMedicationReminders}
              />
            </div>

            <div className="flex items-center justify-between">
              <Label htmlFor="new-results" className="flex flex-col space-y-1">
                <span>Test results</span>
                <span className="font-normal text-sm text-muted-foreground">
                  Be notified when new test results are available
                </span>
              </Label>
              <Switch
                id="new-results"
                checked={newResultsNotifications && notificationsEnabled}
                disabled={!notificationsEnabled}
                onCheckedChange={setNewResultsNotifications}
              />
            </div>
          </div>
        </CardContent>
        <CardFooter>
          <Button onClick={handleSaveSettings} disabled={isSaving}>
            {isSaving ? "Saving..." : "Save Settings"}
          </Button>
        </CardFooter>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Privacy Settings</CardTitle>
          <CardDescription>
            Manage how your information is used and shared
          </CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="flex items-center justify-between">
            <Label htmlFor="data-sharing" className="flex flex-col space-y-1">
              <span>Data sharing for research</span>
              <span className="font-normal text-sm text-muted-foreground">
                Allow anonymized data to be used for medical research
              </span>
            </Label>
            <Switch id="data-sharing" defaultChecked={false} />
          </div>

          <div className="flex items-center justify-between">
            <Label htmlFor="third-party" className="flex flex-col space-y-1">
              <span>Third-party access</span>
              <span className="font-normal text-sm text-muted-foreground">
                Allow trusted third parties to access your medical data
              </span>
            </Label>
            <Switch id="third-party" defaultChecked={false} />
          </div>
        </CardContent>
        <CardFooter>
          <Button onClick={() => toast.success("Privacy settings saved")}>
            Save Privacy Settings
          </Button>
        </CardFooter>
      </Card>
    </div>
  );
}
