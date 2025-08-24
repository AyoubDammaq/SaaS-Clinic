import { createContext } from "react";
import { User, UserRole } from "@/types/auth";

export interface AuthContextType {
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
    confirmPassword: string,
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
  getAllUsers: () => Promise<User[] | null>;
  getUserById: (id: string) => Promise<User | null>;
  changeUserRole: (userId: string, role: UserRole) => Promise<boolean>;
  deleteUser: (
    userId: string,
    options?: { isRollback?: boolean }
  ) => Promise<boolean>;
  checkUserRole: (roles: UserRole[]) => boolean;
  linkToProfile: (userId: string, profileId: string) => Promise<boolean>;
  registerWithDefaultPassword: (
    fullName: string,
    email: string,
    role?: UserRole
  ) => Promise<User | null>;
  linkToProfileHelper: (userId: string, profileId: string, role: number) => Promise<boolean>;
}

export const AuthContext = createContext<AuthContextType | undefined>(
  undefined
);
