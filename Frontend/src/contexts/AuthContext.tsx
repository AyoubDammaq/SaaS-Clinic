import React, {
  createContext,
  useContext,
  useState,
  useEffect,
  ReactNode,
} from "react";
import {
  User,
  UserRole,
  TokenResponseDto,
  LoginRequestDTO,
  ChangePasswordDto,
  ForgotPasswordDto,
  ResetPasswordDto,
  RegisterDto,
  RefreshTokenRequestDto,
} from "@/types/auth";
import { API_ENDPOINTS } from "@/config/api";
import { api } from "@/utils/apiClient";
import { toast } from "sonner";
import { useTranslation } from "@/hooks/useTranslation";
import { AxiosError } from "axios";
import jwt_decode from "jwt-decode";

interface AuthContextType {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  error: string | null;
  login: (email: string, password: string) => Promise<void>;
  logout: () => Promise<void>;
  register: (
    name: string,
    email: string,
    password: string,
    role?: UserRole
  ) => Promise<boolean>;
  changePassword: (
    currentPassword: string,
    newPassword: string
  ) => Promise<boolean>;
  forgotPassword: (email: string) => Promise<boolean>;
  resetPassword: (
    email: string,
    token: string,
    newPassword: string
  ) => Promise<boolean>;
  deleteUser: (
    userId: string,
    options?: { isRollback?: boolean }
  ) => Promise<boolean>;
  checkUserRole: (roles: UserRole[]) => boolean;
  linkToProfile: (userId: string, profileId: string) => Promise<boolean>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const { t } = useTranslation("auth");

  useEffect(() => {
    // Check for existing auth in localStorage
    const storedToken = localStorage.getItem("accessToken");
    const storedUser = localStorage.getItem("user");

    if (storedToken && storedUser) {
      setToken(storedToken);
      setUser(JSON.parse(storedUser));
      setIsAuthenticated(true);
    }

    setIsLoading(false);
  }, []);

  const login = async (email: string, password: string) => {
    setIsLoading(true);
    setError(null);

    try {
      const loginData: LoginRequestDTO = { Email: email, Password: password };

      const response = await api.post<TokenResponseDto>(
        API_ENDPOINTS.AUTH.LOGIN,
        loginData,
        {
          withAuth: false, // Don't need auth token for login
        }
      );

      // Vérifiez si l'API renvoie un token valide
      if (!response || !response.accessToken) {
        throw new Error("Invalid response from server");
      }

      interface DecodedToken {
        sub: string; // ID ou email
        name?: string; // Nom de l'utilisateur
        "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"?: string; // Rôle
      }

      // Décoder le JWT pour récupérer les informations du token
      const decodedToken: any = jwt_decode(response.accessToken);

      // Extraire l'utilisateur à partir du token décodé
      const userData = {
        id: decodedToken.sub, // L'email (sub) est utilisé comme ID
        name: decodedToken.name || "", // Vous pouvez ajouter des données supplémentaires si disponibles
        email: decodedToken.email || "",
        role:
          decodedToken[
            "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
          ] || "Unknown", // Récupérer le rôle depuis le token
      };

      // Sauvegarder les données dans l'état et le localStorage
      setUser(userData);
      setToken(response.accessToken);
      setIsAuthenticated(true);

      localStorage.setItem("accessToken", response.accessToken);
      localStorage.setItem("refreshToken", response.refreshToken || "");
      localStorage.setItem("user", JSON.stringify(userData));

      toast.success(t("loginSuccess"));
    } catch (err: unknown) {
      console.error("Login error:", err);

      if (err instanceof AxiosError) {
        let errorMessage = t("loginError");

        if (err.response && err.response.data) {
          console.error("Error response data:", err.response.data);
          if (typeof err.response.data === "string") {
            errorMessage = err.response.data;
          } else if (err.response.data.message) {
            errorMessage = err.response.data.message;
          }
        }

        setError(errorMessage);
        toast.error(errorMessage);
      } else if (err instanceof Error) {
        setError(err.message);
        toast.error(err.message);
      } else {
        setError(t("loginError"));
        toast.error(t("loginError"));
      }
    } finally {
      setIsLoading(false);
    }
  };

  const logout = async () => {
    setIsLoading(true);

    try {
      if (import.meta.env.VITE_API_URL && user) {
        const payload = { email: user.email }; // ⚠️ attention à la casse

        console.log("🟡 Envoi logout payload :", payload);

        const response = await api.post(API_ENDPOINTS.AUTH.LOGOUT, payload);

        console.log("🟢 Réponse succès logout :", response);
      }

      // Reset local storage et état local
      setUser(null);
      setToken(null);
      setIsAuthenticated(false);
      localStorage.removeItem("accessToken");
      localStorage.removeItem("refreshToken");
      localStorage.removeItem("user");

      toast.success(t("logoutSuccess"));
    } catch (err: any) {
      console.error("🔴 Erreur logout :", err);

      if (err.response) {
        console.error("🔴 Détails réponse :", {
          status: err.response.status,
          data: err.response.data,
          errors: err.response.data?.errors,
        });
      }

      toast.error(t("logoutError"));

      // Nettoyage même en cas d’erreur
      setUser(null);
      setToken(null);
      setIsAuthenticated(false);
      localStorage.removeItem("accessToken");
      localStorage.removeItem("refreshToken");
      localStorage.removeItem("user");
    } finally {
      setIsLoading(false);
    }
  };

  const register = async (
    FullName: string,
    Email: string,
    Password: string,
    ConfirmPassword: string,
    Role?: UserRole
  ): Promise<boolean> => {
    setIsLoading(true);
    setError(null);

    const translateError = (msg: string): string => {
      const translations: Record<string, string> = {
        "Passwords do not match.":
          t("passwordMismatch") || "Les mots de passe ne correspondent pas.",
        "Password must be at least 8 characters.":
          t("passwordTooShort") ||
          "Le mot de passe doit comporter au moins 8 caractères.",
        "Password must contain an uppercase letter.":
          t("passwordUppercase") ||
          "Le mot de passe doit contenir une lettre majuscule.",
        "Password must contain a lowercase letter.":
          t("passwordLowercase") ||
          "Le mot de passe doit contenir une lettre minuscule.",
        "Password must contain a digit.":
          t("passwordDigit") || "Le mot de passe doit contenir un chiffre.",
        "Email already exists.":
          t("emailExistsError") || "Cet email est déjà utilisé.",
      };
      return translations[msg] || msg;
    };

    try {
      const userData: RegisterDto = {
        FullName,
        Email,
        Password,
        ConfirmPassword,
        Role: Role ? roleMap[Role] : 4,
      };

      const response = await api.post<User>(
        API_ENDPOINTS.AUTH.REGISTER,
        userData,
        {
          withAuth: false,
        }
      );

      if (!response) {
        throw new Error("Registration failed");
      }

      // Auto login
      await login(Email, Password);

      toast.success(t("registerSuccess"));
      return true;
    } catch (err: unknown) {
      console.error("Registration error:", err);

      let errorMessage = t("registerError") || "Registration failed";

      if (err instanceof AxiosError && err.response?.data) {
        const data = err.response.data;

        if (typeof data === "string") {
          errorMessage = translateError(data);
        } else if (typeof data === "object" && data !== null) {
          if ("message" in data && typeof data.message === "string") {
            errorMessage = translateError(data.message);
          } else {
            try {
              const messages: string[] = [];

              for (const key in data) {
                if (Array.isArray(data[key])) {
                  messages.push(...data[key]);
                } else if (typeof data[key] === "string") {
                  messages.push(data[key]);
                }
              }

              if (messages.length > 0) {
                errorMessage = messages.map(translateError).join(", ");
              }
            } catch (e) {
              console.error("Error parsing validation errors:", e);
            }
          }
        }
      } else if (err instanceof Error) {
        errorMessage = translateError(err.message);
      }

      if (!errorMessage || errorMessage === t("registerError")) {
        errorMessage = "An unknown error occurred during registration.";
      }

      setError(errorMessage);
      toast.error(errorMessage);
      return false;
    } finally {
      setIsLoading(false);
    }
  };

  const changePassword = async (
    currentPassword: string,
    newPassword: string
  ): Promise<boolean> => {
    if (!user) return false;

    setIsLoading(true);
    setError(null);

    try {
      const changePasswordData: ChangePasswordDto = {
        userId: user.id,
        currentPassword,
        newPassword,
      };

      await api.post(API_ENDPOINTS.AUTH.CHANGE_PASSWORD, changePasswordData);

      toast.success(
        t("passwordChangeSuccess") || "Password changed successfully"
      );
      return true;
    } catch (err: unknown) {
      let errorMessage = t("passwordChangeError") || "Password change failed";

      if (err instanceof AxiosError && err.response?.data) {
        const data = err.response.data;
        if (typeof data === "string") {
          errorMessage = data;
        } else if (
          typeof data === "object" &&
          data !== null &&
          "message" in data &&
          typeof data.message === "string"
        ) {
          errorMessage = data.message;
        }
      } else if (err instanceof Error) {
        errorMessage = err.message;
      }

      setError(errorMessage);
      toast.error(errorMessage);
      return false;
    } finally {
      setIsLoading(false);
    }
  };

  const forgotPassword = async (email: string): Promise<boolean> => {
    setIsLoading(true);
    setError(null);

    try {
      const forgotPasswordData: ForgotPasswordDto = { email };

      await api.post(API_ENDPOINTS.AUTH.FORGOT_PASSWORD, forgotPasswordData);

      toast.success(
        t("passwordResetEmailSent") ||
          "Password reset email sent. Please check your inbox."
      );
      return true;
    } catch (err: unknown) {
      let errorMessage =
        t("passwordResetEmailError") || "Failed to send password reset email";

      if (err instanceof AxiosError && err.response?.data) {
        const data = err.response.data;
        if (typeof data === "string") {
          errorMessage = data;
        } else if (
          typeof data === "object" &&
          data !== null &&
          "message" in data &&
          typeof data.message === "string"
        ) {
          errorMessage = data.message;
        }
      } else if (err instanceof Error) {
        errorMessage = err.message;
      }

      setError(errorMessage);
      toast.error(errorMessage);
      return false;
    } finally {
      setIsLoading(false);
    }
  };

  const resetPassword = async (
    email: string,
    token: string,
    newPassword: string
  ): Promise<boolean> => {
    setIsLoading(true);
    setError(null);

    try {
      const resetPasswordData: ResetPasswordDto = {
        email,
        token,
        newPassword,
      };

      await api.post(API_ENDPOINTS.AUTH.RESET_PASSWORD, resetPasswordData);

      toast.success(
        t("passwordResetSuccess") ||
          "Password reset successful! Please log in with your new password."
      );
      return true;
    } catch (err: unknown) {
      let errorMessage = t("passwordResetError") || "Password reset failed";

      if (err instanceof AxiosError && err.response?.data) {
        const data = err.response.data;
        if (typeof data === "string") {
          errorMessage = data;
        } else if (
          typeof data === "object" &&
          data !== null &&
          "message" in data &&
          typeof data.message === "string"
        ) {
          errorMessage = data.message;
        }
      } else if (err instanceof Error) {
        errorMessage = err.message;
      }

      setError(errorMessage);
      toast.error(errorMessage);
      return false;
    } finally {
      setIsLoading(false);
    }
  };

  const deleteUser = async (
    userId: string,
    options?: { isRollback?: boolean }
  ): Promise<boolean> => {
    setIsLoading(true);
    setError(null);
    try {
      await api.delete(`${API_ENDPOINTS.AUTH.DELETE_USER}/${userId}`);

      if (!options?.isRollback) {
        // Reset state *seulement si ce n’est pas un rollback*
        setUser(null);
        setToken(null);
        setIsAuthenticated(false);
        localStorage.removeItem("accessToken");
        localStorage.removeItem("refreshToken");
        localStorage.removeItem("user");
        toast.success(t("userDeletedSuccess") || "User deleted successfully");
      }

      return true;
    } catch (err: unknown) {
      let errorMessage = t("userDeleteError") || "Failed to delete user";
      if (err instanceof AxiosError && err.response?.data) {
        const data = err.response.data;
        if (typeof data === "string") {
          errorMessage = data;
        } else if (
          typeof data === "object" &&
          data !== null &&
          "message" in data
        ) {
          errorMessage = data.message;
        }
      } else if (err instanceof Error) {
        errorMessage = err.message;
      }

      setError(errorMessage);
      toast.error(errorMessage);
      return false;
    } finally {
      setIsLoading(false);
    }
  };

  const checkUserRole = (roles: UserRole[]): boolean => {
    if (!user) return false;
    return roles.includes(user.role);
  };

  const roleMap: Record<UserRole, number> = {
    SuperAdmin: 2,
    ClinicAdmin: 1,
    Doctor: 3,
    Patient: 4,
  };

  const linkToProfile = async (
    userId: string,
    profileId: string
  ): Promise<boolean> => {
    setIsLoading(true);
    setError(null);

    try {
      const roleMapped = roleMap[user.role];
      const roleFinal = mapNumberToRole(roleMapped);

      const linkProfileData = {
        userId,
        entityId: profileId,
        role: roleMapped,
      };

      console.log("🔗 [linkToProfile] Sending data to API:", {
        userId,
        profileId,
        role: user.role,
        roleMapped,
        roleFinal,
        body: linkProfileData,
      });

      const response = await api.post(
        API_ENDPOINTS.AUTH.LINK_PROFILE,
        linkProfileData
      );

      console.log("✅ [linkToProfile] Success:", response);

      toast.success(t("profileLinkedSuccess") || "Profile linked successfully");
      return true;
    } catch (err: unknown) {
      let errorMessage = t("profileLinkError") || "Failed to link profile";

      if (err instanceof AxiosError && err.response?.data) {
        console.error(
          "❌ [linkToProfile] API error response:",
          err.response.data
        );
        const data = err.response.data;
        if (typeof data === "string") {
          errorMessage = data;
        } else if (
          typeof data === "object" &&
          data !== null &&
          "message" in data &&
          typeof data.message === "string"
        ) {
          errorMessage = data.message;
        }
      } else if (err instanceof Error) {
        errorMessage = err.message;
      }

      console.error("❌ [linkToProfile] Caught error:", errorMessage);
      setError(errorMessage);
      toast.error(errorMessage);
      return false;
    } finally {
      setIsLoading(false);
    }
  };

  function mapNumberToRole(roleNumber?: number): UserRole {
    switch (roleNumber) {
      case 2:
        return "SuperAdmin";
      case 1:
        return "ClinicAdmin";
      case 3:
        return "Doctor";
      case 4:
        return "Patient";
      default:
        return "Patient"; // Valeur par défaut en cas d'erreur
    }
  }

  return (
    <AuthContext.Provider
      value={{
        user,
        token,
        isAuthenticated,
        isLoading,
        error,
        login,
        logout,
        register,
        changePassword,
        forgotPassword,
        resetPassword,
        deleteUser,
        checkUserRole,
        linkToProfile,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
}
