import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "@/hooks/useAuth";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
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
import { usePatients } from "@/hooks/usePatients";

function PatientFormPage() {
  const { user, linkToProfile, deleteUser } = useAuth();
  const { handleAddPatient, linkUserToPatient, handleDeletePatient } =
    usePatients();
  const navigate = useNavigate();
  const { t } = useTranslation();

  const [formData, setFormData] = useState({
    firstName: "",
    lastName: "",
    dateOfBirth: "",
    phoneNumber: "",
    address: "",
    sexe: undefined as "M" | "F" | undefined,
  });

  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>
  ) => {
    const { id, value } = e.target;
    setFormData((prev) => ({ ...prev, [id]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsSubmitting(true);
    setError(null);

    let createdPatientId: string | null = null;

    try {
      if (!user)
        throw new Error(t("notAuthenticated") || "User not authenticated");
      if (!formData.sexe)
        throw new Error(t("sexRequired") || "Sex is required");

      const newPatient = await handleAddPatient({
        prenom: formData.firstName,
        nom: formData.lastName,
        dateNaissance: formData.dateOfBirth,
        telephone: formData.phoneNumber,
        adresse: formData.address,
        email: user.email,
        sexe: formData.sexe,
      });

      createdPatientId = newPatient.id;

      await linkUserToPatient(user.id, newPatient.id);
      const linked = await linkToProfile(user.id, newPatient.id);
      if (!linked)
        throw new Error(
          t("profileLinkFailed") || "Failed to link patient profile"
        );

      toast.success(
        t("patientCreated") || "Patient profile created successfully"
      );
      navigate("/dashboard");
    } catch (err) {
      const message =
        err instanceof Error
          ? err.message
          : t("unknownError") || "An unknown error occurred";
      setError(message);
      toast.error(message);

      if (createdPatientId) {
        try {
          console.warn("⚠️ Rollback activé...");
          await handleDeletePatient(createdPatientId);
          await deleteUser(user.id, { isRollback: true });
          console.log("♻️ Rollback terminé : patient + user supprimés");
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
          <h1 className="text-2xl font-bold text-gray-900 dark:text-white">
            SaaS-Clinic
          </h1>
          <p className="text-gray-600 dark:text-gray-400 mt-1">
            {t("patientFormSubtitle") || "Complete Your Patient Profile"}
          </p>
        </div>

        <Card>
          <CardHeader>
            <CardTitle>{t("patientInfo") || "Patient Information"}</CardTitle>
            <CardDescription>
              {t("patientInfoDesc") ||
                "Please provide your personal information"}
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
                    <Label htmlFor="firstName">
                      {t("firstName") || "First Name"}
                    </Label>
                    <Input
                      id="firstName"
                      value={formData.firstName}
                      onChange={handleChange}
                      placeholder="John"
                      required
                    />
                  </div>
                  <div className="grid gap-2">
                    <Label htmlFor="lastName">
                      {t("lastName") || "Last Name"}
                    </Label>
                    <Input
                      id="lastName"
                      value={formData.lastName}
                      onChange={handleChange}
                      placeholder="Doe"
                      required
                    />
                  </div>
                </div>

                <div className="grid gap-2">
                  <Label htmlFor="dateOfBirth">
                    {t("dateOfBirth") || "Date of Birth"}
                  </Label>
                  <Input
                    id="dateOfBirth"
                    type="date"
                    value={formData.dateOfBirth}
                    onChange={handleChange}
                    required
                  />
                </div>

                <div className="grid gap-2">
                  <Label htmlFor="phoneNumber">
                    {t("phone") || "Phone Number"}
                  </Label>
                  <Input
                    id="phoneNumber"
                    value={formData.phoneNumber}
                    onChange={handleChange}
                    placeholder="+1 (555) 123-4567"
                    required
                  />
                </div>

                <div className="grid gap-2">
                  <Label htmlFor="address">{t("address") || "Address"}</Label>
                  <Input
                    id="address"
                    value={formData.address}
                    onChange={handleChange}
                    placeholder="123 Main St, City, Country"
                    required
                  />
                </div>

                <div className="grid gap-2">
                  <Label htmlFor="sexe">{t("sex") || "Sex"}</Label>
                  <select
                    id="sexe"
                    value={formData.sexe || ""}
                    onChange={(e) =>
                      setFormData({
                        ...formData,
                        sexe: e.target.value as "M" | "F",
                      })
                    }
                    required
                    className="border rounded px-3 py-2"
                  >
                    <option value="">{t("selectSex") || "Select sex"}</option>
                    <option value="M">{t("male") || "Male"}</option>
                    <option value="F">{t("female") || "Female"}</option>
                  </select>
                </div>

                <Button
                  type="submit"
                  className="w-full"
                  disabled={isSubmitting}
                >
                  {isSubmitting
                    ? t("creatingProfile") || "Creating Profile..."
                    : t("completeRegistration") || "Complete Registration"}
                </Button>
              </div>
            </form>
          </CardContent>

          <CardFooter className="flex justify-center">
            <div className="text-sm text-muted-foreground">
              {t("profileUpdatable") ||
                "Your profile information can be updated later"}
            </div>
          </CardFooter>
        </Card>
      </div>
    </div>
  );
}

export default PatientFormPage;
