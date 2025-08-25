import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "@/hooks/useAuth";
import DoctorFormPage from "./DoctorFormPage";
import PatientFormPage from "./PatientFormPage";
import ClinicAdminFormPage from "./ClinicAdminFormPage";

function RoleFormRouter() {
  const { user, isAuthenticated, isLoading } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    if (!isLoading && !isAuthenticated) {
      navigate("/login");
    }
  }, [isAuthenticated, isLoading, navigate]);

  if (isLoading || !isAuthenticated || !user) {
    return (
      <div className="flex h-screen items-center justify-center">
        Loading...
      </div>
    );
  }

  // Route to appropriate form based on user role
  switch (user.role) {
    case "Doctor":
      return <DoctorFormPage />;
    case "Patient":
      return <PatientFormPage />;
    case "ClinicAdmin":
      return <ClinicAdminFormPage />;
    default:
      // For roles without specific forms (like SuperAdmin)
      navigate("/dashboard");
      return null;
  }
}

export default RoleFormRouter;
