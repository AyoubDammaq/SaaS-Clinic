import { Navigate, Outlet, useLocation } from "react-router-dom";
import { useAuth } from "@/hooks/useAuth";
import { UserRole } from "@/types/auth";
import { DashboardLayout } from "@/components/layout/DashboardLayout";

interface PrivateRouteProps {
  children?: React.ReactNode;
  allowedRoles?: UserRole[];
}

export const PrivateRoute = ({ allowedRoles, children }: PrivateRouteProps) => {
  const { isAuthenticated, isLoading, user } = useAuth();
  const location = useLocation();

  if (isLoading) {
    return (
      <div className="flex h-screen items-center justify-center">
        Loading...
      </div>
    );
  }

  // Check if user is authenticated
  if (!isAuthenticated) {
    // Redirect to login with return path
    return <Navigate to="/login" state={{ from: location.pathname }} replace />;
  }

  // Check role-based access if roles are specified
  if (allowedRoles && user && !allowedRoles.includes(user.role)) {
    // User doesn't have required role, redirect to dashboard
    return <Navigate to="/dashboard" replace />;
  }

  // User is authenticated and has correct role, render the protected content
  return <DashboardLayout>{children || <Outlet />}</DashboardLayout>;
};
