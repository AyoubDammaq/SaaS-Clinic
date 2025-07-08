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
import { useCliniques } from "@/hooks/useCliniques";

function ClinicAdminFormPage() {
  const { user, linkToProfile, deleteUser } = useAuth();
  const { handleAddClinique, linkUserToClinique, handleDeleteClinique } = useCliniques();
  const navigate = useNavigate();
  const { t } = useTranslation();

  const [formData, setFormData] = useState({
    clinicName: "",
    address: "",
    phoneNumber: "",
    website: "",
  });
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { id, value } = e.target;
    setFormData(prev => ({ ...prev, [id]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsSubmitting(true);
    setError(null);

    let createdClinicId: string | null = null;

    try {
      if (!user) throw new Error(t("userNotAuthenticated") || "User not authenticated");

      const newClinic = await handleAddClinique({
        nom: formData.clinicName,
        adresse: formData.address,
        numeroTelephone: formData.phoneNumber,
        siteWeb: formData.website || null,
        email: user.email,
        typeClinique: 0,
        statut: 0,
        description: "",
      });

      createdClinicId = newClinic.id;

      await linkUserToClinique(user.id, newClinic.id);

      const linked = await linkToProfile(user.id, newClinic.id);
      if (!linked) throw new Error(t("failedToLinkProfile") || "Failed to link clinic profile");

      toast.success(t("clinicCreatedSuccess") || "Clinic created successfully");
      navigate("/dashboard");
    } catch (err: unknown) {
      const errorObj = err instanceof Error ? err : new Error(t("unknownError") || "Unknown error");
      setError(errorObj.message);
      toast.error(errorObj.message);

      if (createdClinicId) {
        try {
          console.warn("⚠️ Rollback activated...");
          await handleDeleteClinique(createdClinicId);
          await deleteUser(user.id, { isRollback: true }); // à vérifier si supporté
          console.log("♻️ Rollback completed: clinic + user deleted");
        } catch (rollbackError) {
          console.error("❌ Rollback failed:", rollbackError);
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
          <p className="text-gray-600 dark:text-gray-400 mt-1">{t("registerClinic") || "Register Your Clinic"}</p>
        </div>

        <Card>
          <CardHeader>
            <CardTitle>{t("clinicInformation") || "Clinic Information"}</CardTitle>
            <CardDescription>{t("provideClinicInfo") || "Please provide information about your clinic"}</CardDescription>
          </CardHeader>

          <CardContent>
            <form onSubmit={handleSubmit}>
              {error && (
                <Alert variant="destructive" className="mb-4">
                  <AlertDescription>{error}</AlertDescription>
                </Alert>
              )}

              <div className="grid gap-4">
                <div className="grid gap-2">
                  <Label htmlFor="clinicName">{t("clinicName") || "Clinic Name"}</Label>
                  <Input
                    id="clinicName"
                    value={formData.clinicName}
                    onChange={handleChange}
                    placeholder={t("clinicNamePlaceholder") || "City Medical Center"}
                    required
                  />
                </div>

                <div className="grid gap-2">
                  <Label htmlFor="address">{t("address") || "Address"}</Label>
                  <Input
                    id="address"
                    value={formData.address}
                    onChange={handleChange}
                    placeholder={t("addressPlaceholder") || "123 Main St, City, Country"}
                    required
                  />
                </div>

                <div className="grid gap-2">
                  <Label htmlFor="phoneNumber">{t("phoneNumber") || "Phone Number"}</Label>
                  <Input
                    id="phoneNumber"
                    value={formData.phoneNumber}
                    onChange={handleChange}
                    placeholder={t("phoneNumberPlaceholder") || "+1 (555) 123-4567"}
                    required
                  />
                </div>

                <div className="grid gap-2">
                  <Label htmlFor="website">{t("websiteOptional") || "Website (optional)"}</Label>
                  <Input
                    id="website"
                    value={formData.website}
                    onChange={handleChange}
                    placeholder="https://example.com"
                  />
                </div>

                <Button type="submit" className="w-full" disabled={isSubmitting}>
                  {isSubmitting ? (t("creatingClinic") || "Creating Clinic...") : (t("completeRegistration") || "Complete Registration")}
                </Button>
              </div>
            </form>
          </CardContent>

          <CardFooter className="flex justify-center">
            <div className="text-sm text-muted-foreground">
              {t("clinicInfoUpdatable") || "Your clinic information can be updated later"}
            </div>
          </CardFooter>
        </Card>
      </div>
    </div>
  );
}

export default ClinicAdminFormPage;