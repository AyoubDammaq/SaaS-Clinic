import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "@/contexts/AuthContext";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { useTranslation } from "@/hooks/useTranslation";
import { toast } from "sonner";
import { useDoctors } from "@/hooks/useDoctors";

function DoctorFormPage() {
  const { user, linkToProfile, deleteUser } = useAuth();
  const { addDoctor, linkUserToDoctor, deleteDoctor } = useDoctors();
  const navigate = useNavigate();
  const { t } = useTranslation("doctors");

  const [formData, setFormData] = useState({
    prenom: "",
    nom: "",
    email: user?.email || "",
    specialite: "",
    telephone: "",
    photoUrl: "",
  });

  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const specialties = [
    "Cardiologie", "Dermatologie", "Endocrinologie", "Gastroentérologie",
    "Neurologie", "Obstétrique", "Ophtalmologie", "Pédiatrie", "Psychiatrie", "Médecin généraliste"
  ];

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { id, value } = e.target;
    setFormData((prev) => ({ ...prev, [id]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsSubmitting(true);
    setError(null);

    let createdDoctorId: string | null = null;

    try {
      if (!user) throw new Error(t("notAuthenticated") || "Utilisateur non authentifié");
      if (!formData.specialite) throw new Error(t("specialtyRequired") || "Veuillez sélectionner une spécialité");

      const doctor = await addDoctor({
        prenom: formData.prenom,
        nom: formData.nom,
        email: formData.email,
        telephone: formData.telephone,
        specialite: formData.specialite,
        cliniqueId: null, // 🛠️ à gérer si tu l’ajoutes
        photoUrl: formData.photoUrl || null,
      });

      createdDoctorId = doctor.id;

      await linkUserToDoctor(user.id, doctor.id);
      const result = await linkToProfile(user.id, doctor.id);
      if (!result) throw new Error(t("profileLinkFailed") || "Échec de liaison avec le profil");

      toast.success(t("doctorCreated") || "Profil docteur créé avec succès");
      navigate("/dashboard");
    } catch (err) {
      const message = err instanceof Error ? err.message : t("unknownError") || "Une erreur inconnue est survenue";
      setError(message);
      toast.error(message);

      if (createdDoctorId) {
        try {
          console.warn("⚠️ Rollback activé...");
          await deleteDoctor(createdDoctorId);
          await deleteUser(user.id, { isRollback: true });
          console.log("♻️ Rollback terminé : docteur + user supprimés");
        } catch (rollbackError) {
          console.error("❌ Rollback échoué :", rollbackError);
        }
      }
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 dark:bg-gray-900 p-4">
      <div className="w-full max-w-md">
        <div className="text-center mb-8">
          <h1 className="text-2xl font-bold text-gray-900 dark:text-white">SaaS-Clinic</h1>
          <p className="text-gray-600 dark:text-gray-400 mt-1">
            {t("doctorFormSubtitle") || "Complete Your Doctor Profile"}
          </p>
        </div>

        <Card>
          <CardHeader>
            <CardTitle>{t("doctorInfo") || "Doctor Information"}</CardTitle>
            <CardDescription>
              {t("doctorInfoDesc") || "Please provide your professional information"}
            </CardDescription>
          </CardHeader>

          <CardContent>
            <form onSubmit={handleSubmit}>
              {error && (
                <Alert variant="destructive" className="mb-4">
                  <AlertDescription>{error}</AlertDescription>
                </Alert>
              )}

              <div className="grid gap-4">
                <div className="grid grid-cols-2 gap-4">
                  <div className="grid gap-2">
                    <Label htmlFor="prenom">{t("firstName") || "First Name"}</Label>
                    <Input
                      id="prenom"
                      value={formData.prenom}
                      onChange={handleChange}
                      placeholder="John"
                      required
                    />
                  </div>
                  <div className="grid gap-2">
                    <Label htmlFor="nom">{t("lastName") || "Last Name"}</Label>
                    <Input
                      id="nom"
                      value={formData.nom}
                      onChange={handleChange}
                      placeholder="Doe"
                      required
                    />
                  </div>
                </div>

                <div className="grid gap-2">
                  <Label htmlFor="specialite">{t("specialty") || "Specialty"}</Label>
                  <Select value={formData.specialite} onValueChange={(value) => setFormData(prev => ({ ...prev, specialite: value }))}>
                    <SelectTrigger>
                      <SelectValue placeholder={t("selectSpecialty") || "Select your specialty"} />
                    </SelectTrigger>
                    <SelectContent>
                      {specialties.map((specialty) => (
                        <SelectItem key={specialty} value={specialty}>
                          {specialty}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>

                <div className="grid gap-2">
                  <Label htmlFor="telephone">{t("phone") || "Phone Number"}</Label>
                  <Input
                    id="telephone"
                    value={formData.telephone}
                    onChange={handleChange}
                    placeholder="+1 (555) 123-4567"
                    required
                  />
                </div>

                <div className="grid gap-2">
                  <Label htmlFor="photoUrl">{t("photoUrl") || "Profile Photo URL (optional)"}</Label>
                  <Input
                    id="photoUrl"
                    value={formData.photoUrl}
                    onChange={handleChange}
                    placeholder="https://example.com/photo.jpg"
                  />
                </div>

                <Button type="submit" className="w-full" disabled={isSubmitting}>
                  {isSubmitting
                    ? t("creatingProfile") || "Creating Profile..."
                    : t("completeRegistration") || "Complete Registration"}
                </Button>
              </div>
            </form>
          </CardContent>

          <CardFooter className="flex justify-center">
            <div className="text-sm text-muted-foreground">
              {t("profileUpdatable") || "Your profile information can be updated later"}
            </div>
          </CardFooter>
        </Card>
      </div>
    </div>
  );
}

export default DoctorFormPage;