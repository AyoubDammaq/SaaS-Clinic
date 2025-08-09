// ClinicAdminProfile.tsx
import { useState, useEffect, useRef } from "react";
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
import { Building2, Users, Settings, BarChart3 } from "lucide-react";
import { toast } from "sonner";
import { useAuth } from "@/hooks/useAuth";
import { useCliniques } from "@/hooks/useCliniques";
import { User } from "@/types/auth";

export function ClinicAdminProfile() {
  const { user, getUserById } = useAuth();
  const { cliniques } = useCliniques();
  const [activeTab, setActiveTab] = useState("profile");
  const [emailNotifications, setEmailNotifications] = useState(true);
  const [appointmentAlerts, setAppointmentAlerts] = useState(true);
  const [billingNotifications, setBillingNotifications] = useState(false);
  const [fullUser, setFullUser] = useState<User | null>(null);
  const userCache = useRef<Record<string, User>>({});

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

  // Récupérer les informations complètes de l'utilisateur
  useEffect(() => {
    if (user?.id) {
      // Vérifier le cache d'abord
      if (userCache.current[user.id]) {
        setFullUser(userCache.current[user.id]);
        return;
      }

      // Récupérer si non mis en cache
      getUserById(user.id).then((data) => {
        if (data) {
          userCache.current[user.id] = data;
          setFullUser(data);
        }
      });
    }
  }, [user?.id, getUserById]);

  if (!user) return null;

  // Trouver la clinique gérée par cet administrateur
  const adminClinic = cliniques.find((clinic) => clinic.id === user.cliniqueId);

  return (
    <Card className="w-full">
      <CardHeader>
        <div className="flex items-center gap-2">
          <Building2 className="h-5 w-5 text-primary" />
          <CardTitle>Profil Administrateur de Clinique</CardTitle>
        </div>
        <CardDescription>
          Gérez votre profil et les paramètres de votre clinique
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
            <TabsTrigger value="clinic">Clinique</TabsTrigger>
            <TabsTrigger value="settings">Paramètres</TabsTrigger>
            <TabsTrigger value="reports">Rapports</TabsTrigger>
          </TabsList>

          <TabsContent value="profile" className="space-y-4">
            <div className="flex flex-col md:flex-row gap-6">
              <div className="flex flex-col items-center">
                <Avatar className="h-24 w-24">
                  <AvatarFallback className="text-xl bg-blue-500 text-white">
                    {getInitials(fullUser?.fullName ?? user.name)}
                  </AvatarFallback>
                </Avatar>
                <Badge variant="secondary" className="mt-2">
                  <Building2 className="h-3 w-3 mr-1" />
                  Admin Clinique
                </Badge>
              </div>

              <div className="flex-1 space-y-4">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <div className="text-sm text-muted-foreground">Nom</div>
                    <div className="font-medium">
                      {fullUser?.fullName ?? user.name}
                    </div>
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
                    <div className="text-sm text-muted-foreground">
                      Clinique
                    </div>
                    <div className="font-medium">
                      {adminClinic?.nom || "Non assignée"}
                    </div>
                  </div>
                </div>

                <div className="pt-4">
                  <h3 className="text-lg font-medium mb-2">Responsabilités</h3>
                  <div className="grid grid-cols-2 md:grid-cols-3 gap-2">
                    <Badge variant="outline">
                      <Users className="h-3 w-3 mr-1" />
                      Personnel
                    </Badge>
                    <Badge variant="outline">
                      <Settings className="h-3 w-3 mr-1" />
                      Configuration
                    </Badge>
                    <Badge variant="outline">
                      <BarChart3 className="h-3 w-3 mr-1" />
                      Rapports
                    </Badge>
                  </div>
                </div>
              </div>
            </div>
          </TabsContent>

          <TabsContent value="clinic">
            <div className="space-y-6">
              <h3 className="text-lg font-medium">
                Informations de la Clinique
              </h3>
              {adminClinic ? (
                <Card>
                  <CardHeader>
                    <CardTitle className="text-base">
                      {adminClinic.nom}
                    </CardTitle>
                    <CardDescription>{adminClinic.adresse}</CardDescription>
                  </CardHeader>
                  <CardContent>
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                      <div>
                        <div className="text-sm text-muted-foreground">
                          Téléphone
                        </div>
                        <div className="font-medium">
                          {adminClinic.numeroTelephone}
                        </div>
                      </div>
                      <div>
                        <div className="text-sm text-muted-foreground">
                          Email
                        </div>
                        <div className="font-medium">{adminClinic.email}</div>
                      </div>
                    </div>
                    <Button className="mt-4" variant="outline">
                      Modifier les Informations
                    </Button>
                  </CardContent>
                </Card>
              ) : (
                <Card>
                  <CardContent className="text-center py-8">
                    <p className="text-muted-foreground">
                      Aucune clinique assignée
                    </p>
                  </CardContent>
                </Card>
              )}
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
                        Recevoir les mises à jour importantes de la clinique
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
                      htmlFor="appointment-alerts"
                      className="flex flex-col space-y-1"
                    >
                      <span>Alertes Rendez-vous</span>
                      <span className="font-normal text-sm text-muted-foreground">
                        Recevoir les notifications de rendez-vous
                      </span>
                    </Label>
                    <Switch
                      id="appointment-alerts"
                      checked={appointmentAlerts}
                      onCheckedChange={setAppointmentAlerts}
                    />
                  </div>

                  <div className="flex items-center justify-between">
                    <Label
                      htmlFor="billing-notifications"
                      className="flex flex-col space-y-1"
                    >
                      <span>Notifications Facturation</span>
                      <span className="font-normal text-sm text-muted-foreground">
                        Recevoir les alertes de paiement et facturation
                      </span>
                    </Label>
                    <Switch
                      id="billing-notifications"
                      checked={billingNotifications}
                      onCheckedChange={setBillingNotifications}
                    />
                  </div>
                </div>
                <Button onClick={saveSettings} className="mt-4">
                  Sauvegarder les Paramètres
                </Button>
              </div>

              <div>
                <h3 className="text-lg font-medium mb-4">
                  Paramètres du Compte
                </h3>
                <div className="space-y-4">
                  <Button variant="outline">Changer le Mot de Passe</Button>
                  <Button variant="outline" className="text-orange-500">
                    Désactiver le Compte
                  </Button>
                </div>
              </div>
            </div>
          </TabsContent>

          <TabsContent value="reports">
            <div className="space-y-6">
              <h3 className="text-lg font-medium">Rapports et Analyses</h3>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <Card>
                  <CardHeader>
                    <CardTitle className="text-base">Rapport Mensuel</CardTitle>
                  </CardHeader>
                  <CardContent>
                    <Button variant="outline" className="w-full">
                      Télécharger
                    </Button>
                  </CardContent>
                </Card>

                <Card>
                  <CardHeader>
                    <CardTitle className="text-base">
                      Statistiques Patients
                    </CardTitle>
                  </CardHeader>
                  <CardContent>
                    <Button variant="outline" className="w-full">
                      Voir les Stats
                    </Button>
                  </CardContent>
                </Card>

                <Card>
                  <CardHeader>
                    <CardTitle className="text-base">Revenus</CardTitle>
                  </CardHeader>
                  <CardContent>
                    <Button variant="outline" className="w-full">
                      Analyser
                    </Button>
                  </CardContent>
                </Card>

                <Card>
                  <CardHeader>
                    <CardTitle className="text-base">Performance</CardTitle>
                  </CardHeader>
                  <CardContent>
                    <Button variant="outline" className="w-full">
                      Évaluer
                    </Button>
                  </CardContent>
                </Card>
              </div>
            </div>
          </TabsContent>
        </Tabs>
      </CardContent>
    </Card>
  );
}