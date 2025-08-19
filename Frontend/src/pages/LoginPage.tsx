import { useState, FormEvent, useEffect } from "react";
import { useNavigate, useLocation, Link } from "react-router-dom";
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
import { ThemeSwitcher } from "@/components/ui/theme-switcher";
import { LanguageSwitcher } from "@/components/ui/language-switcher";
import { toast } from "sonner";

function LoginPage() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const { login, isLoading, error, user, isAuthenticated } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const { t } = useTranslation("auth");

  // Get the return URL from location state or default to dashboard
  const from =
    (location.state as unknown as { from?: string })?.from || "/dashboard";

  // Redirect if already authenticated
  useEffect(() => {
    if (isAuthenticated && user) {
      navigate("/dashboard");
    }
  }, [isAuthenticated, user, navigate]);

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();

    try {
      await login(email, password);
      // Navigation will happen in the useEffect when auth state changes
    } catch (err) {
      // L'erreur est déjà gérée dans le contexte auth
      console.error("Login error:", err);
    }
  };

  const loginWithDemo = async (demoEmail: string, role: string) => {
    setEmail(demoEmail);
    setPassword("password");

    try {
      await login(demoEmail, "password");
      toast.success(`Logged in as ${role}`);
      // Navigation will happen in the useEffect when auth state changes
    } catch (err) {
      console.error("Demo login error:", err);
    }
  };

  const showDemoSection = !import.meta.env.VITE_API_URL;

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 dark:bg-gray-900 p-4">
      <div className="absolute top-4 right-4 flex gap-2">
        <LanguageSwitcher />
        <ThemeSwitcher />
      </div>
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
            <CardTitle>{t("signIn")}</CardTitle>
            <CardDescription>{t("enterCredentials")}</CardDescription>
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
                  <Label htmlFor="email">{t("email")}</Label>
                  <Input
                    id="email"
                    type="email"
                    placeholder="name@example.com"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    autoComplete="email"
                    required
                  />
                </div>

                <div className="grid gap-2">
                  <div className="flex items-center justify-between">
                    <Label htmlFor="password">{t("password")}</Label>
                  </div>
                  <Input
                    id="password"
                    type="password"
                    placeholder="••••••••"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    autoComplete="current-password"
                    required
                  />
                </div>

                <Button type="submit" className="w-full" disabled={isLoading}>
                  {isLoading ? t("signingIn") : t("signInButton")}
                </Button>
              </div>
            </form>
          </CardContent>
          <CardFooter className="flex flex-col space-y-4 items-center">
            <div className="text-center">
              <p className="text-sm text-muted-foreground mb-2">
                {t("noAccount")}{" "}
                <Link to="/register" className="text-primary font-medium">
                  {t("registerNow")}
                </Link>
              </p>
              {showDemoSection && (
                <div className="text-sm text-muted-foreground">
                  {t("demoCredentials")}
                </div>
              )}
            </div>
            {showDemoSection && (
              <div className="grid grid-cols-2 gap-2 text-xs w-full text-muted-foreground">
                <Button
                  variant="outline"
                  size="sm"
                  className="border rounded p-2 flex flex-col items-start h-auto"
                  onClick={() =>
                    loginWithDemo("admin@example.com", "Super Admin")
                  }
                >
                  <div className="font-medium">{t("superAdmin")}</div>
                  <div>admin@example.com</div>
                  <div>password</div>
                </Button>
                <Button
                  variant="outline"
                  size="sm"
                  className="border rounded p-2 flex flex-col items-start h-auto"
                  onClick={() =>
                    loginWithDemo("clinic@example.com", "Clinic Admin")
                  }
                >
                  <div className="font-medium">{t("clinicAdmin")}</div>
                  <div>clinic@example.com</div>
                  <div>password</div>
                </Button>
                <Button
                  variant="outline"
                  size="sm"
                  className="border rounded p-2 flex flex-col items-start h-auto"
                  onClick={() => loginWithDemo("doctor@example.com", "Doctor")}
                >
                  <div className="font-medium">{t("doctor")}</div>
                  <div>doctor@example.com</div>
                  <div>password</div>
                </Button>
                <Button
                  variant="outline"
                  size="sm"
                  className="border rounded p-2 flex flex-col items-start h-auto"
                  onClick={() =>
                    loginWithDemo("patient@example.com", "Patient")
                  }
                >
                  <div className="font-medium">{t("patient")}</div>
                  <div>patient@example.com</div>
                  <div>password</div>
                </Button>
              </div>
            )}
          </CardFooter>
        </Card>
      </div>
    </div>
  );
}

export default LoginPage;
