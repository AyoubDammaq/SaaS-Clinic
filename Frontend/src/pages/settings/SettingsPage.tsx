import { useState } from "react";
import { useAuth } from "@/hooks/useAuth";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Switch } from "@/components/ui/switch";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import {
  Settings,
  Bell,
  Lock,
  User,
  Building,
  Clock,
  Mail,
  CreditCard,
  Users,
  Save,
} from "lucide-react";
import { Badge } from "@/components/ui/badge";
import { cn } from "@/lib/utils";
import { Separator } from "@/components/ui/separator";
import { ConsultationPricingSettings } from "@/components/settings/ConsultationPricingSettings";

function SettingsPage() {
  const { user } = useAuth();

  // Security settings
  const [twoFactorEnabled, setTwoFactorEnabled] = useState(false);
  const [emailNotifications, setEmailNotifications] = useState(true);
  const [smsNotifications, setSmsNotifications] = useState(true);
  const [darkMode, setDarkMode] = useState(false);

  // Clinic settings (for ClinicAdmin)
  const [clinicName, setClinicName] = useState("Main Street Medical Center");
  const [clinicAddress, setClinicAddress] = useState(
    "123 Main St, Anytown, CA 12345"
  );
  const [clinicPhone, setClinicPhone] = useState("555-123-4567");
  const [clinicEmail, setClinicEmail] = useState("info@mainstreetmedical.com");
  const [openingTime, setOpeningTime] = useState("08:00");
  const [closingTime, setClosingTime] = useState("18:00");

  const [workingDays, setWorkingDays] = useState({
    monday: true,
    tuesday: true,
    wednesday: true,
    thursday: true,
    friday: true,
    saturday: false,
    sunday: false,
  });

  const toggleWorkingDay = (day: keyof typeof workingDays) => {
    setWorkingDays((prev) => ({
      ...prev,
      [day]: !prev[day],
    }));
  };

  // System settings (for SuperAdmin)
  const [maintenanceMode, setMaintenanceMode] = useState(false);
  const [allowNewRegistrations, setAllowNewRegistrations] = useState(true);

  const renderUserSettings = () => {
    return (
      <>
        <div className="space-y-6">
          <div>
            <h3 className="text-lg font-medium">Profile Settings</h3>
            <p className="text-sm text-muted-foreground">
              Update your personal information and preferences
            </p>
          </div>
          <Separator />

          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div className="space-y-2">
              <Label htmlFor="fullName">Full Name</Label>
              <Input
                id="fullName"
                value={user?.name}
                placeholder="Enter your full name"
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="email">Email</Label>
              <Input
                id="email"
                value={user?.email}
                type="email"
                placeholder="Enter your email address"
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="phone">Phone Number</Label>
              <Input id="phone" placeholder="Enter your phone number" />
            </div>

            <div className="space-y-2">
              <Label htmlFor="language">Preferred Language</Label>
              <select
                id="language"
                className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
              >
                <option value="en">English</option>
                <option value="fr">Français</option>
                <option value="es">Español</option>
              </select>
            </div>
          </div>

          <div className="flex justify-end">
            <Button>
              <Save className="mr-2 h-4 w-4" /> Save Profile
            </Button>
          </div>
        </div>
      </>
    );
  };

  const renderNotificationSettings = () => {
    return (
      <>
        <div className="space-y-6">
          <div>
            <h3 className="text-lg font-medium">Notification Preferences</h3>
            <p className="text-sm text-muted-foreground">
              Control how you receive notifications and alerts
            </p>
          </div>
          <Separator />

          <div className="space-y-4">
            <div className="flex items-center justify-between">
              <div className="space-y-0.5">
                <Label>Email Notifications</Label>
                <p className="text-sm text-muted-foreground">
                  Receive notifications via email
                </p>
              </div>
              <Switch
                checked={emailNotifications}
                onCheckedChange={setEmailNotifications}
              />
            </div>

            <div className="flex items-center justify-between">
              <div className="space-y-0.5">
                <Label>SMS Notifications</Label>
                <p className="text-sm text-muted-foreground">
                  Receive notifications via text message
                </p>
              </div>
              <Switch
                checked={smsNotifications}
                onCheckedChange={setSmsNotifications}
              />
            </div>

            <Separator />
            <p className="font-medium">Notification Types</p>

            <div className="flex items-center justify-between">
              <div className="space-y-0.5">
                <Label>Appointment Reminders</Label>
                <p className="text-sm text-muted-foreground">
                  Reminders for upcoming appointments
                </p>
              </div>
              <Switch defaultChecked />
            </div>

            <div className="flex items-center justify-between">
              <div className="space-y-0.5">
                <Label>Account Updates</Label>
                <p className="text-sm text-muted-foreground">
                  Important account and security updates
                </p>
              </div>
              <Switch defaultChecked />
            </div>

            <div className="flex items-center justify-between">
              <div className="space-y-0.5">
                <Label>Billing and Payment</Label>
                <p className="text-sm text-muted-foreground">
                  Invoices and payment confirmations
                </p>
              </div>
              <Switch defaultChecked />
            </div>
          </div>

          <div className="flex justify-end">
            <Button>
              <Save className="mr-2 h-4 w-4" /> Save Preferences
            </Button>
          </div>
        </div>
      </>
    );
  };

  const renderSecuritySettings = () => {
    return (
      <>
        <div className="space-y-6">
          <div>
            <h3 className="text-lg font-medium">Security Settings</h3>
            <p className="text-sm text-muted-foreground">
              Manage your account security and login preferences
            </p>
          </div>
          <Separator />

          <div className="space-y-4">
            <div className="flex items-center justify-between">
              <div className="space-y-0.5">
                <Label>Two-Factor Authentication</Label>
                <p className="text-sm text-muted-foreground">
                  Add an extra layer of security to your account
                </p>
              </div>
              <Switch
                checked={twoFactorEnabled}
                onCheckedChange={setTwoFactorEnabled}
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="current-password">Current Password</Label>
              <Input
                id="current-password"
                type="password"
                placeholder="Enter your current password"
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="new-password">New Password</Label>
              <Input
                id="new-password"
                type="password"
                placeholder="Enter your new password"
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="confirm-password">Confirm New Password</Label>
              <Input
                id="confirm-password"
                type="password"
                placeholder="Confirm your new password"
              />
            </div>
          </div>

          <div className="flex justify-end">
            <Button>
              <Save className="mr-2 h-4 w-4" /> Update Password
            </Button>
          </div>

          <Separator />

          <div>
            <h4 className="font-medium mb-2">Login Sessions</h4>
            <div className="space-y-2">
              <div className="border p-3 rounded-md">
                <div className="flex justify-between items-center">
                  <div>
                    <p className="font-medium">Current Session</p>
                    <p className="text-sm text-muted-foreground">
                      Web Browser • Started 2 hours ago
                    </p>
                  </div>
                  <Badge
                    variant="outline"
                    className="bg-green-50 text-green-600 border-green-200"
                  >
                    Active
                  </Badge>
                </div>
              </div>

              <div className="border p-3 rounded-md">
                <div className="flex justify-between items-center">
                  <div>
                    <p className="font-medium">Mobile App</p>
                    <p className="text-sm text-muted-foreground">
                      iOS • Last active 2 days ago
                    </p>
                  </div>
                  <Button variant="outline" size="sm">
                    Revoke
                  </Button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </>
    );
  };

  const renderClinicSettings = () => {
    if (user?.role !== "ClinicAdmin" && user?.role !== "SuperAdmin") {
      return null;
    }

    return (
      <>
        <div className="space-y-6">
          <div>
            <h3 className="text-lg font-medium">Clinic Settings</h3>
            <p className="text-sm text-muted-foreground">
              Configure your clinic's basic information and operating hours
            </p>
          </div>
          <Separator />

          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div className="space-y-2">
              <Label htmlFor="clinic-name">Clinic Name</Label>
              <Input
                id="clinic-name"
                value={clinicName}
                onChange={(e) => setClinicName(e.target.value)}
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="clinic-email">Email</Label>
              <Input
                id="clinic-email"
                value={clinicEmail}
                onChange={(e) => setClinicEmail(e.target.value)}
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="clinic-address">Address</Label>
              <Input
                id="clinic-address"
                value={clinicAddress}
                onChange={(e) => setClinicAddress(e.target.value)}
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="clinic-phone">Phone</Label>
              <Input
                id="clinic-phone"
                value={clinicPhone}
                onChange={(e) => setClinicPhone(e.target.value)}
              />
            </div>
          </div>

          <Separator />

          <div>
            <h4 className="font-medium mb-4">Operating Hours</h4>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-4">
              <div className="space-y-2">
                <Label htmlFor="opening-time">Opening Time</Label>
                <Input
                  id="opening-time"
                  type="time"
                  value={openingTime}
                  onChange={(e) => setOpeningTime(e.target.value)}
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="closing-time">Closing Time</Label>
                <Input
                  id="closing-time"
                  type="time"
                  value={closingTime}
                  onChange={(e) => setClosingTime(e.target.value)}
                />
              </div>
            </div>

            <div>
              <Label className="mb-2 block">Working Days</Label>
              <div className="flex flex-wrap gap-2">
                <Button
                  variant={workingDays.monday ? "default" : "outline"}
                  size="sm"
                  onClick={() => toggleWorkingDay("monday")}
                >
                  Monday
                </Button>
                <Button
                  variant={workingDays.tuesday ? "default" : "outline"}
                  size="sm"
                  onClick={() => toggleWorkingDay("tuesday")}
                >
                  Tuesday
                </Button>
                <Button
                  variant={workingDays.wednesday ? "default" : "outline"}
                  size="sm"
                  onClick={() => toggleWorkingDay("wednesday")}
                >
                  Wednesday
                </Button>
                <Button
                  variant={workingDays.thursday ? "default" : "outline"}
                  size="sm"
                  onClick={() => toggleWorkingDay("thursday")}
                >
                  Thursday
                </Button>
                <Button
                  variant={workingDays.friday ? "default" : "outline"}
                  size="sm"
                  onClick={() => toggleWorkingDay("friday")}
                >
                  Friday
                </Button>
                <Button
                  variant={workingDays.saturday ? "default" : "outline"}
                  size="sm"
                  onClick={() => toggleWorkingDay("saturday")}
                >
                  Saturday
                </Button>
                <Button
                  variant={workingDays.sunday ? "default" : "outline"}
                  size="sm"
                  onClick={() => toggleWorkingDay("sunday")}
                >
                  Sunday
                </Button>
              </div>
            </div>
          </div>

          <div className="flex justify-end">
            <Button>
              <Save className="mr-2 h-4 w-4" /> Save Clinic Settings
            </Button>
          </div>
        </div>
      </>
    );
  };

  const renderSystemSettings = () => {
    if (user?.role !== "SuperAdmin") {
      return null;
    }

    return (
      <>
        <div className="space-y-6">
          <div>
            <h3 className="text-lg font-medium">System Settings</h3>
            <p className="text-sm text-muted-foreground">
              Configure global platform settings
            </p>
          </div>
          <Separator />

          <div className="space-y-4">
            <div className="flex items-center justify-between">
              <div className="space-y-0.5">
                <Label>Maintenance Mode</Label>
                <p className="text-sm text-muted-foreground">
                  Enable to temporarily restrict access while performing updates
                </p>
              </div>
              <Switch
                checked={maintenanceMode}
                onCheckedChange={setMaintenanceMode}
              />
            </div>

            <div className="flex items-center justify-between">
              <div className="space-y-0.5">
                <Label>New Clinic Registrations</Label>
                <p className="text-sm text-muted-foreground">
                  Allow new clinics to register on the platform
                </p>
              </div>
              <Switch
                checked={allowNewRegistrations}
                onCheckedChange={setAllowNewRegistrations}
              />
            </div>

            <Separator />

            <div className="space-y-2">
              <Label htmlFor="system-email">System Email</Label>
              <Input id="system-email" defaultValue="system@saas-clinic.com" />
              <p className="text-sm text-muted-foreground">
                Used for system-generated notifications
              </p>
            </div>

            <div className="space-y-2">
              <Label htmlFor="storage-limit">Default Storage Limit (GB)</Label>
              <Input id="storage-limit" type="number" defaultValue="5" />
              <p className="text-sm text-muted-foreground">
                Default storage allocation for new clinics
              </p>
            </div>
          </div>

          <div className="flex justify-end">
            <Button>
              <Save className="mr-2 h-4 w-4" /> Save System Settings
            </Button>
          </div>
        </div>
      </>
    );
  };

  return (
    <div className="space-y-6 pb-8">
      <div className="flex flex-col gap-2">
        <h1 className="text-3xl font-bold tracking-tight">Settings</h1>
        <p className="text-muted-foreground">
          Manage your account settings and preferences
        </p>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>User Preferences</CardTitle>
          <CardDescription>
            Manage your account settings, notifications, and security
            preferences
          </CardDescription>
        </CardHeader>
        <CardContent>
          <Tabs defaultValue="account">
            <TabsList className="mb-6 flex flex-wrap">
              <TabsTrigger value="account">
                <User className="h-4 w-4 mr-2" /> Account
              </TabsTrigger>
              <TabsTrigger value="notifications">
                <Bell className="h-4 w-4 mr-2" /> Notifications
              </TabsTrigger>
              <TabsTrigger value="security">
                <Lock className="h-4 w-4 mr-2" /> Security
              </TabsTrigger>
              {(user?.role === "ClinicAdmin" ||
                user?.role === "SuperAdmin") && (
                <TabsTrigger value="clinic">
                  <Building className="h-4 w-4 mr-2" /> Clinic
                </TabsTrigger>
              )}
              {user?.role === "ClinicAdmin" && (
                <TabsTrigger value="pricing">
                  <CreditCard className="h-4 w-4 mr-2" /> Tarification
                </TabsTrigger>
              )}
              {user?.role === "SuperAdmin" && (
                <TabsTrigger value="system">
                  <Settings className="h-4 w-4 mr-2" /> System
                </TabsTrigger>
              )}
            </TabsList>
            <TabsContent value="account">{renderUserSettings()}</TabsContent>
            <TabsContent value="notifications">
              {renderNotificationSettings()}
            </TabsContent>
            <TabsContent value="security">
              {renderSecuritySettings()}
            </TabsContent>
            <TabsContent value="clinic">{renderClinicSettings()}</TabsContent>
            <TabsContent value="pricing">
              <ConsultationPricingSettings />
            </TabsContent>
            <TabsContent value="system">{renderSystemSettings()}</TabsContent>
          </Tabs>
        </CardContent>
      </Card>
    </div>
  );
}

export default SettingsPage;
