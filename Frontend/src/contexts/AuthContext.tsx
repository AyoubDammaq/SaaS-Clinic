
import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import { 
  User, 
  UserRole, 
  TokenResponseDto, 
  LoginRequestDTO, 
  ChangePasswordDto, 
  ForgotPasswordDto, 
  ResetPasswordDto, 
  RegisterDto,
  RefreshTokenRequestDto
} from '@/types/auth';
import { API_ENDPOINTS } from '@/config/api';
import { api } from '@/utils/apiClient';
import { toast } from 'sonner';
import { useTranslation } from '@/hooks/useTranslation';
import { AxiosError } from 'axios';
import jwt_decode from 'jwt-decode';

interface AuthContextType {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  error: string | null;
  login: (email: string, password: string) => Promise<void>;
  logout: () => Promise<void>;
  register: (name: string, email: string, password: string, role?: UserRole) => Promise<boolean>;
  changePassword: (currentPassword: string, newPassword: string) => Promise<boolean>;
  forgotPassword: (email: string) => Promise<boolean>;
  resetPassword: (email: string, token: string, newPassword: string) => Promise<boolean>;
  checkUserRole: (roles: UserRole[]) => boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const { t } = useTranslation('auth');

  useEffect(() => {
    // Check for existing auth in localStorage
    const storedToken = localStorage.getItem('accessToken');
    const storedUser = localStorage.getItem('user');
    
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

          const response = await api.post<TokenResponseDto>(API_ENDPOINTS.AUTH.LOGIN, loginData, {
              withAuth: false, // Don't need auth token for login
          });

          // Vérifiez si l'API renvoie un token valide
          if (!response || !response.accessToken) {
              throw new Error('Invalid response from server');
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
              name: decodedToken.name || '', // Vous pouvez ajouter des données supplémentaires si disponibles
              email: decodedToken.sub,
              role: decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || "Unknown", // Récupérer le rôle depuis le token
          };

          // Sauvegarder les données dans l'état et le localStorage
          setUser(userData);
          setToken(response.accessToken);
          setIsAuthenticated(true);

          localStorage.setItem('accessToken', response.accessToken);
          localStorage.setItem('refreshToken', response.refreshToken || '');
          localStorage.setItem('user', JSON.stringify(userData));

          toast.success(t('loginSuccess'));
      } catch (err: unknown) {
          console.error('Login error:', err);

          if (err instanceof AxiosError) {
              let errorMessage = t('loginError');

              if (err.response && err.response.data) {
                  console.error('Error response data:', err.response.data);
                  if (typeof err.response.data === 'string') {
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
              setError(t('loginError'));
              toast.error(t('loginError'));
          }
      } finally {
          setIsLoading(false);
      }
  };


  const logout = async () => {
    setIsLoading(true);
    
    try {
      // For demo purposes, we'll check if VITE_API_URL is set
      if (import.meta.env.VITE_API_URL && user) {
        // Call logout API - send userId as body
        await api.post(API_ENDPOINTS.AUTH.LOGOUT, user.id);
      }
      
      // Clear local state regardless of API call success
      setUser(null);
      setToken(null);
      setIsAuthenticated(false);
      localStorage.removeItem('accessToken');
      localStorage.removeItem('refreshToken');
      localStorage.removeItem('user');
      
      toast.success(t('logoutSuccess'));
    } catch (err) {
      console.error('Logout error:', err);
      toast.error(t('logoutError'));
      
      // Still clear local state even if API call fails
      setUser(null);
      setToken(null);
      setIsAuthenticated(false);
      localStorage.removeItem('accessToken');
      localStorage.removeItem('refreshToken');
      localStorage.removeItem('user');
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

    try {
      // Créer l'objet utilisateur avec les noms de propriétés corrects pour l'API
      const userData: RegisterDto = {
        FullName,
        Email,
        Password,
        ConfirmPassword,
        Role: Role ? mapRoleToNumber(Role) : undefined // Convertir la valeur Role en nombre
      };

      const response = await api.post<User>(API_ENDPOINTS.AUTH.REGISTER, userData, {
        withAuth: false, // Pas besoin de token pour l'inscription
      });

      if (!response) {
        throw new Error('Failed to register user');
      }

      toast.success(t('registerSuccess'));
      return true;

    } catch (err: unknown) {
      console.error('Registration error:', err);

      let errorMessage = t('registerError');

      if (err instanceof AxiosError && err.response) {
        const data = err.response.data;

        if (typeof data === 'string') {
          errorMessage = data;
        } else if (typeof data === 'object' && data !== null) {
          if ('message' in data && typeof data.message === 'string') {
            errorMessage = data.message;
          } else {
            try {
              const errorValues = Object.values(data).flat();
              if (errorValues.length > 0) {
                errorMessage = Array.isArray(errorValues[0])
                  ? (errorValues[0] as string[]).join(', ')
                  : errorValues.join(', ');
              }
            } catch (e) {
              console.error('Error parsing validation errors:', e);
            }
          }
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

  const changePassword = async (currentPassword: string, newPassword: string): Promise<boolean> => {
    if (!user) return false;

    setIsLoading(true);
    setError(null);

    try {
      const changePasswordData: ChangePasswordDto = {
        userId: user.id,
        currentPassword,
        newPassword
      };

      await api.post(API_ENDPOINTS.AUTH.CHANGE_PASSWORD, changePasswordData);

      toast.success(t('passwordChangeSuccess') || 'Password changed successfully');
      return true;
    } catch (err: unknown) {
      let errorMessage = t('passwordChangeError') || 'Password change failed';

      if (err instanceof AxiosError && err.response?.data) {
        const data = err.response.data;
        if (typeof data === 'string') {
          errorMessage = data;
        } else if (typeof data === 'object' && data !== null && 'message' in data && typeof data.message === 'string') {
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
      
      toast.success(t('passwordResetEmailSent') || 'Password reset email sent. Please check your inbox.');
      return true;
    } catch (err: unknown) {
      let errorMessage = t('passwordResetEmailError') || 'Failed to send password reset email';

      if (err instanceof AxiosError && err.response?.data) {
        const data = err.response.data;
        if (typeof data === 'string') {
          errorMessage = data;
        } else if (typeof data === 'object' && data !== null && 'message' in data && typeof data.message === 'string') {
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
        t('passwordResetSuccess') ||
          'Password reset successful! Please log in with your new password.'
      );
      return true;
    } catch (err: unknown) {
      let errorMessage =
        t('passwordResetError') || 'Password reset failed';

      if (err instanceof AxiosError && err.response?.data) {
        const data = err.response.data;
        if (typeof data === 'string') {
          errorMessage = data;
        } else if (
          typeof data === 'object' &&
          data !== null &&
          'message' in data &&
          typeof data.message === 'string'
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

  function mapRoleToNumber(role: UserRole): number {
    switch (role) {
      case 'SuperAdmin':
        return 2;
      case 'ClinicAdmin':
        return 1;
      case 'Doctor':
        return 3;
      case 'Patient':
        return 4;
      default:
        return 4;  // Valeur par défaut en cas d'erreur
    }
  }

  function mapNumberToRole(roleNumber?: number): UserRole {
    switch (roleNumber) {
      case 2:
        return 'SuperAdmin';
      case 1:
        return 'ClinicAdmin';
      case 3:
        return 'Doctor';
      case 4:
        return 'Patient';
      default:
        return 'Patient';  // Valeur par défaut en cas d'erreur
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
        checkUserRole
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export function useAuth() {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
