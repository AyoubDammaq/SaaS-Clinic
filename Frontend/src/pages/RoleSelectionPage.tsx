import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { UserRole } from "@/types/auth";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { useTranslation } from "@/hooks/useTranslation";
import { Building, User, Users, FileText } from "lucide-react";

function RoleSelectionPage() {
  const [selectedRole, setSelectedRole] = useState<UserRole | null>(null);
  const navigate = useNavigate();
  const { t } = useTranslation();

  const roleOptions: {
    role: UserRole;
    icon: React.ElementType;
    description: string;
  }[] = [
    {
      role: "SuperAdmin",
      icon: Building,
      description: t("superAdminDescription"),
    },
    {
      role: "ClinicAdmin",
      icon: Building,
      description: t("clinicAdminDescription"),
    },
    {
      role: "Doctor",
      icon: User,
      description: t("doctorDescription"),
    },
    {
      role: "Patient",
      icon: FileText,
      description: t("patientDescription"),
    },
  ];

  const handleContinue = () => {
    if (selectedRole) {
      // In a real app, we would save this role to the user's account
      // For now, we'll just navigate to the dashboard
      navigate("/dashboard");
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 dark:bg-gray-900 p-4">
      <div className="w-full max-w-md">
        <div className="text-center mb-8">
          <div className="inline-block mb-2 w-12 h-12 bg-clinic-500 text-white rounded-lg flex items-center justify-center font-bold text-xl mx-auto">
            SC
          </div>
          <h1 className="text-2xl font-bold text-gray-900 dark:text-white">
            SaaS-Clinic
          </h1>
          <p className="text-gray-600 dark:text-gray-400 mt-1">
            Medical Management System
          </p>
        </div>

        <Card>
          <CardHeader>
            <CardTitle>{t("selectRole")}</CardTitle>
            <CardDescription>{t("selectRoleDescription")}</CardDescription>
          </CardHeader>
          <CardContent>
            <div className="grid gap-4">
              {roleOptions.map((option) => (
                <div
                  key={option.role}
                  className={`p-4 border rounded-md cursor-pointer transition-all ${
                    selectedRole === option.role
                      ? "border-primary bg-primary/10"
                      : "hover:border-primary/50"
                  }`}
                  onClick={() => setSelectedRole(option.role)}
                >
                  <div className="flex items-center gap-4">
                    <div
                      className={`p-2 rounded-full ${
                        selectedRole === option.role
                          ? "bg-primary text-primary-foreground"
                          : "bg-muted"
                      }`}
                    >
                      <option.icon className="h-5 w-5" />
                    </div>
                    <div>
                      <h3 className="font-medium">
                        {t(option.role.toLowerCase())}
                      </h3>
                      <p className="text-sm text-muted-foreground">
                        {option.description}
                      </p>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
          <CardFooter>
            <Button
              className="w-full"
              onClick={handleContinue}
              disabled={!selectedRole}
            >
              {t("continue")}
            </Button>
          </CardFooter>
        </Card>
      </div>
    </div>
  );
}

export default RoleSelectionPage;
