import { useState } from "react";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
} from "@/components/ui/card";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Switch } from "@/components/ui/switch";
import { Label } from "@/components/ui/label";
import { Badge } from "@/components/ui/badge";
import { Settings, Shield, Database, Users } from "lucide-react";
import { toast } from "sonner";
import { useAuth } from "@/hooks/useAuth";

export function SuperAdminProfile() {
  const { user } = useAuth();
  const [activeTab, setActiveTab] = useState("profile");
  const [emailNotifications, setEmailNotifications] = useState(true);
  const [systemAlerts, setSystemAlerts] = useState(true);
  const [auditLogs, setAuditLogs] = useState(false);

  const getInitials = (name: string) => {
    return name
      .split(" ")
      .map((n) => n[0])
      .join("")
      .toUpperCase();
  };

  const saveSettings = () => {
    toast.success("Paramètres sauvegardés avec succès");
  };

  if (!user) return null;

  return (
    <Card className="w-full">
      <CardHeader>
        <div className="flex items-center gap-2">
          <Shield className="h-5 w-5 text-primary" />
          <CardTitle>Profil Super Administrateur</CardTitle>
        </div>
        <CardDescription>
          Gérez vos informations et les paramètres système globaux
        </CardDescription>
      </CardHeader>
      <CardContent>
        <Tabs
          defaultValue="profile"
          value={activeTab}
          onValueChange={setActiveTab}
          className="w-full"
        >
          <TabsList className="mb-4 grid w-full grid-cols-4">
            <TabsTrigger value="profile">Profil</TabsTrigger>
            <TabsTrigger value="settings">Paramètres</TabsTrigger>
            <TabsTrigger value="system">Système</TabsTrigger>
            <TabsTrigger value="security">Sécurité</TabsTrigger>
          </TabsList>

          <TabsContent value="profile" className="space-y-4">
            <div className="flex flex-col md:flex-row gap-6">
              <div className="flex flex-col items-center">
                <Avatar className="h-24 w-24">
                  <AvatarFallback className="text-xl bg-red-500 text-white">
                    {getInitials(user.name)}
                  </AvatarFallback>
                </Avatar>
                <Badge variant="destructive" className="mt-2">
                  <Shield className="h-3 w-3 mr-1" />
                  Super Admin
                </Badge>
              </div>

              <div className="flex-1 space-y-4">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <div className="text-sm text-muted-foreground">Nom</div>
                    <div className="font-medium">{user.name}</div>
                  </div>
                  <div>
                    <div className="text-sm text-muted-foreground">Email</div>
                    <div className="font-medium">{user.email}</div>
                  </div>
                  <div>
                    <div className="text-sm text-muted-foreground">Rôle</div>
                    <div className="font-medium">{user.role}</div>
                  </div>
                  <div>
                    <div className="text-sm text-muted-foreground">Accès</div>
                    <div className="font-medium">Toutes les cliniques</div>
                  </div>
                </div>

                <div className="pt-4">
                  <h3 className="text-lg font-medium mb-2">Privilèges</h3>
                  <div className="grid grid-cols-2 md:grid-cols-4 gap-2">
                    <Badge variant="outline">
                      <Database className="h-3 w-3 mr-1" />
                      Gestion BDD
                    </Badge>
                    <Badge variant="outline">
                      <Users className="h-3 w-3 mr-1" />
                      Gestion Utilisateurs
                    </Badge>
                    <Badge variant="outline">
                      <Settings className="h-3 w-3 mr-1" />
                      Config Système
                    </Badge>
                    <Badge variant="outline">
                      <Shield className="h-3 w-3 mr-1" />
                      Sécurité
                    </Badge>
                  </div>
                </div>
              </div>
            </div>
          </TabsContent>

          <TabsContent value="settings">
            <div className="space-y-6">
              <div>
                <h3 className="text-lg font-medium mb-4">
                  Préférences de Notification
                </h3>
                <div className="space-y-4">
                  <div className="flex items-center justify-between">
                    <Label
                      htmlFor="email-notifications"
                      className="flex flex-col space-y-1"
                    >
                      <span>Notifications Email</span>
                      <span className="font-normal text-sm text-muted-foreground">
                        Recevoir les mises à jour importantes du système
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
                      htmlFor="system-alerts"
                      className="flex flex-col space-y-1"
                    >
                      <span>Alertes Système</span>
                      <span className="font-normal text-sm text-muted-foreground">
                        Recevoir les alertes de sécurité et de performance
                      </span>
                    </Label>
                    <Switch
                      id="system-alerts"
                      checked={systemAlerts}
                      onCheckedChange={setSystemAlerts}
                    />
                  </div>

                  <div className="flex items-center justify-between">
                    <Label
                      htmlFor="audit-logs"
                      className="flex flex-col space-y-1"
                    >
                      <span>Logs d'Audit</span>
                      <span className="font-normal text-sm text-muted-foreground">
                        Recevoir un résumé quotidien des activités
                      </span>
                    </Label>
                    <Switch
                      id="audit-logs"
                      checked={auditLogs}
                      onCheckedChange={setAuditLogs}
                    />
                  </div>
                </div>
                <Button onClick={saveSettings} className="mt-4">
                  Sauvegarder les Paramètres
                </Button>
              </div>
            </div>
          </TabsContent>

          <TabsContent value="system">
            <div className="space-y-6">
              <h3 className="text-lg font-medium">Configuration Système</h3>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <Card>
                  <CardHeader>
                    <CardTitle className="text-base">Base de Données</CardTitle>
                  </CardHeader>
                  <CardContent>
                    <Button variant="outline" className="w-full">
                      Gérer la BDD
                    </Button>
                  </CardContent>
                </Card>

                <Card>
                  <CardHeader>
                    <CardTitle className="text-base">
                      Utilisateurs Globaux
                    </CardTitle>
                  </CardHeader>
                  <CardContent>
                    <Button variant="outline" className="w-full">
                      Gérer les Utilisateurs
                    </Button>
                  </CardContent>
                </Card>

                <Card>
                  <CardHeader>
                    <CardTitle className="text-base">Configurations</CardTitle>
                  </CardHeader>
                  <CardContent>
                    <Button variant="outline" className="w-full">
                      Paramètres Système
                    </Button>
                  </CardContent>
                </Card>

                <Card>
                  <CardHeader>
                    <CardTitle className="text-base">Monitoring</CardTitle>
                  </CardHeader>
                  <CardContent>
                    <Button variant="outline" className="w-full">
                      Voir les Métriques
                    </Button>
                  </CardContent>
                </Card>
              </div>
            </div>
          </TabsContent>

          <TabsContent value="security">
            <div className="space-y-6">
              <h3 className="text-lg font-medium">Paramètres de Sécurité</h3>
              <div className="space-y-4">
                <Button variant="outline">Changer le Mot de Passe</Button>
                <Button variant="outline">
                  Authentification à Deux Facteurs
                </Button>
                <Button variant="outline">Sessions Actives</Button>
                <Button variant="outline">Logs de Sécurité</Button>
              </div>
            </div>
          </TabsContent>
        </Tabs>
      </CardContent>
    </Card>
  );
}
