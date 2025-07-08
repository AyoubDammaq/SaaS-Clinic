
export type UserRole = 'SuperAdmin' | 'ClinicAdmin' | 'Doctor' | 'Patient';

export interface User {
  id: string;
  name: string;
  email: string;
  role: UserRole;
  cliniqueId?: string; // Optional for ClinicAdmin and SuperAdmin
  medecinId?: string; // Optional for Doctor
  patientId?: string; // Optional for Patient
}

export interface AuthState {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  error: string | null;
}

// DTOs that match the backend API
export interface UserDto {
  Email: string;
  Password: string;
}

export interface RegisterDto extends UserDto {
  FullName: string;
  Email: string;
  Password: string;
  ConfirmPassword: string;
  Role?: number;
}

// Renamed to match backend DTO
export interface LoginRequestDTO {
  Email: string;
  Password: string;
}

export interface TokenResponseDto {
  accessToken: string;
  refreshToken?: string;
  userId?: string;
  userName?: string;
  role?: number;
  user?: User;
}

export interface RefreshTokenRequestDto {
  refreshToken: string;
}

export interface ChangePasswordDto {
  userId: string; 
  currentPassword: string;
  newPassword: string;
}

export interface ForgotPasswordDto {
  email: string;
}

export interface ResetPasswordDto {
  email: string;
  token: string;
  newPassword: string;
}

export interface ChangeUserRoleRequestDto {
  userId: string;
  newRole: UserRole;
}

export interface LinkProfileDto {
  userId: string;
  entityId: string; 
  role: UserRole;
}

export interface DecodedToken {
  sub: string;
  name?: string;
  email?: string;
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"?: UserRole;
  cliniqueId?: string;
  medecinId?: string;
  patientId?: string;
}