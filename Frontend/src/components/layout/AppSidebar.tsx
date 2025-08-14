import { useMemo } from "react";
import { Link, useLocation } from "react-router-dom";
import {
  Sidebar,
  SidebarContent,
  SidebarHeader,
  SidebarFooter,
  SidebarMenu,
  SidebarMenuItem,
  SidebarMenuButton,
  SidebarRail,
} from "@/components/ui/sidebar";
import { useAuth } from "@/hooks/useAuth";
import {
  Home,
  Users,
  Calendar,
  FileText,
  CreditCard,
  Bell,
  BarChart2,
  Settings,
  LogOut,
  Building,
  ClipboardList,
  User,
  BookText,
} from "lucide-react";
import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";
import { useTranslation } from "@/hooks/useTranslation";

export function AppSidebar() {
  const { user, logout } = useAuth();
  const location = useLocation();
  const { t } = useTranslation("common");

  const roleBasedMenuItems = useMemo(() => {
    const items = [
      {
        name: t("dashboard"), // Translate
        key: "dashboard", // Key for fallback
        path: "/dashboard",
        icon: Home,
        roles: ["SuperAdmin", "ClinicAdmin", "Doctor", "Patient"],
      },
      {
        name: t("medical_record"),
        key: "medical_record",
        path: "/medicalrecord",
        icon: BookText,
        roles: ["Patient"],
      },
      {
        name: t("clinics"),
        key: "clinics",
        path: "/clinics",
        icon: Building,
        roles: ["SuperAdmin", "Patient", "Doctor"],
      },
      {
        name: t("patients"),
        key: "patients",
        path: "/patients",
        icon: Users,
        roles: ["SuperAdmin", "ClinicAdmin", "Doctor"],
      },
      {
        name: t("doctors"),
        key: "doctors",
        path: "/doctors",
        icon: User,
        roles: ["SuperAdmin", "ClinicAdmin", "Patient"],
      },
      {
        name: t("appointments"),
        key: "appointments",
        path: "/appointments",
        icon: Calendar,
        roles: ["SuperAdmin", "ClinicAdmin", "Doctor", "Patient"],
      },
      {
        name: t("consultations"),
        key: "consultations",
        path: "/consultations",
        icon: ClipboardList,
        roles: ["SuperAdmin", "ClinicAdmin", "Doctor", "Patient"],
      },
      {
        name: t("billing"),
        key: "billing",
        path: "/billing",
        icon: CreditCard,
        roles: ["SuperAdmin", "ClinicAdmin", "Patient"],
      },
      {
        name: t("notifications"),
        key: "notifications",
        path: "/notifications",
        icon: Bell,
        roles: ["SuperAdmin", "ClinicAdmin", "Doctor", "Patient"],
      },
      {
        name: t("settings"),
        key: "settings",
        path: "/settings",
        icon: Settings,
        roles: ["SuperAdmin", "ClinicAdmin"],
      },
      {
        name: t("user_management"),
        key: "user_management",
        path: "/users",
        icon: FileText,
        roles: ["SuperAdmin"],
      },
    ];

    if (!user) return [];

    return items.filter((item) => item.roles.includes(user.role));
  }, [user, t]); // Add t to dependencies to update translations if language changes

  return (
    <Sidebar>
      <SidebarRail />
      <SidebarHeader className="p-4">
        <Link to="/dashboard" className="flex items-center space-x-2">
          <div className="w-8 h-8 bg-clinic-500 text-white rounded-lg flex items-center justify-center font-bold">
            SC
          </div>
          <span className="text-xl font-bold">SaaS-Clinic</span>
        </Link>
      </SidebarHeader>

      <SidebarContent className="px-3 py-2">
        <SidebarMenu>
          {roleBasedMenuItems.map((item) => (
            <SidebarMenuItem key={item.path}>
              <SidebarMenuButton
                asChild
                isActive={location.pathname === item.path}
                tooltip={item.name}
              >
                <Link
                  to={item.path}
                  className={cn(
                    "nav-link flex w-full items-center gap-3 px-3 py-2 rounded-md",
                    location.pathname === item.path &&
                      "bg-sidebar-accent text-clinic-500"
                  )}
                >
                  <item.icon className="h-5 w-5" />
                  <span>{item.name}</span>
                </Link>
              </SidebarMenuButton>
            </SidebarMenuItem>
          ))}
        </SidebarMenu>
      </SidebarContent>

      <SidebarFooter className="p-3">
        <Button
          variant="ghost"
          className="w-full justify-start gap-3 text-muted-foreground"
          onClick={logout}
        >
          <LogOut className="h-5 w-5" />
          <span>{t("logout")}</span>
        </Button>
      </SidebarFooter>
    </Sidebar>
  );
}