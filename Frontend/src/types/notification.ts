export enum NotificationChannel {
  None = 0,
  SMS = 1,
  Email = 2,
  Push = 3,
  InApp = 4,
}

export enum NotificationPriority {
  Low = 1,
  Normal = 2,
  High = 3,
  Critical = 4,
}

export enum NotificationStatus {
  Pending = 1,
  Sent = 2,
  Failed = 3,
  Cancelled = 4,
  IsRead = 5,
}

export enum NotificationType {
  AppointmentConfirmation = 1,
  AppointmentReminder = 2,
  AppointmentCancellation = 3,
  PrescriptionReady = 4,
  TestResultsAvailable = 5,
  PaymentDue = 6,
  PaymentConfirmation = 7,
  AppointmentCancelledByDoctor = 8,
  FactureAdded = 9,
  AppointmentCreated = 10,
  AppointmentUpdated = 11,
  NewAppointment = 20,
  AppointmentModified = 21,
  PatientArrived = 22,
  UrgentMessage = 23,
  TestResultsReceived = 24,
  NewPatientRegistered = 40,
  LastMinuteCancellation = 41,
  PaymentIssue = 42,
  StockAlert = 43,
  EquipmentMaintenance = 44,
  SystemAlert = 60,
  SecurityAlert = 61,
  BackupFailed = 62,
  PerformanceReport = 63,
}

export enum UserType {
  Patient = 1,
  Doctor = 2,
  ClinicAdmin = 3,
  SuperAdmin = 4,
}

export interface CreateNotificationRequest {
  recipientId: string;
  recipientType: UserType;
  type: NotificationType;
  title: string;
  content: string;
  priority?: NotificationPriority;
  scheduledAt?: string | null;
  metadata?: Record<string, unknown> | null;
}

export interface MarkNotificationAsSentRequest {
  notificationId: string;
  sentAt: string;
}

export interface NotificationDto {
  id: string;
  recipientId: string;
  recipientType: UserType;
  type: NotificationType;
  channel: NotificationChannel;
  title: string;
  content: string;
  priority: NotificationPriority;
  status: NotificationStatus;
  createdAt: string;
  sentAt?: string | null;
}

export interface NotificationSummaryDto {
  id: string;
  title: string;
  content: string;
  status: NotificationStatus;
  createdAt: string;
}

export interface NotificationFilterRequest {
  recipientId?: string | null;
  recipientType?: UserType | null;
  status?: NotificationStatus | null;
  type?: NotificationType | null;
  from?: string | null;
  to?: string | null;
  page?: number;
  pageSize?: number;
}

export interface SendNotificationRequest {
  notificationId: string;
}
