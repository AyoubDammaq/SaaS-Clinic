import { useState, useEffect } from "react";
import {
  Bell,
  Calendar,
  FileText,
  MessageCircle,
  Settings,
  CheckCircle,
  Eye,
} from "lucide-react";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Separator } from "@/components/ui/separator";
import { NotificationPopup } from "@/components/notifications/NotificationPopup";
import { useNavigate } from "react-router-dom";
import { toast } from "sonner";
import { useTranslation } from "@/hooks/useTranslation";
import { useAuth } from "@/hooks/useAuth";
import { useNotifications } from "@/hooks/useNotifications";
import { cn } from "@/lib/utils";
import { format, parseISO } from "date-fns";
import {
  NotificationDto,
  NotificationType as BackendNotificationType,
  NotificationStatus,
  NotificationPriority as BackendNotificationPriority,
} from "@/types/notification";

// Define the Notification interface to match NotificationsPage.tsx
interface Notification {
  id: string;
  type: "Appointment" | "Consultation" | "Message" | "System";
  priority: "Normal" | "Important" | "Urgent";
  title: string;
  message: string;
  date: string;
  time: string;
  read: boolean;
  relatedId?: string;
  relatedName?: string;
}

interface NotificationDropdownProps {
  onClose: () => void;
}

// Map backend NotificationDto to frontend Notification
const mapNotificationDtoToNotification = (
  dto: NotificationDto
): Notification => {
  const typeMap: Record<BackendNotificationType, Notification["type"]> = {
    [BackendNotificationType.AppointmentConfirmation]: "Appointment",
    [BackendNotificationType.AppointmentReminder]: "Appointment",
    [BackendNotificationType.AppointmentCancellation]: "Appointment",
    [BackendNotificationType.AppointmentCancelledByDoctor]: "Appointment",
    [BackendNotificationType.AppointmentCreated]: "Appointment",
    [BackendNotificationType.AppointmentUpdated]: "Appointment",
    [BackendNotificationType.NewAppointment]: "Appointment",
    [BackendNotificationType.AppointmentModified]: "Appointment",
    [BackendNotificationType.PrescriptionReady]: "Consultation",
    [BackendNotificationType.TestResultsAvailable]: "Consultation",
    [BackendNotificationType.TestResultsReceived]: "Consultation",
    [BackendNotificationType.UrgentMessage]: "Message",
    [BackendNotificationType.NewPatientRegistered]: "System",
    [BackendNotificationType.LastMinuteCancellation]: "System",
    [BackendNotificationType.PaymentIssue]: "System",
    [BackendNotificationType.StockAlert]: "System",
    [BackendNotificationType.EquipmentMaintenance]: "System",
    [BackendNotificationType.SystemAlert]: "System",
    [BackendNotificationType.SecurityAlert]: "System",
    [BackendNotificationType.BackupFailed]: "System",
    [BackendNotificationType.PerformanceReport]: "System",
    [BackendNotificationType.PaymentDue]: "System",
    [BackendNotificationType.PaymentConfirmation]: "System",
    [BackendNotificationType.FactureAdded]: "System",
    [BackendNotificationType.PatientArrived]: "System",
  };

  const priorityMap: Record<
    BackendNotificationPriority,
    Notification["priority"]
  > = {
    [BackendNotificationPriority.Low]: "Normal",
    [BackendNotificationPriority.Normal]: "Normal",
    [BackendNotificationPriority.High]: "Important",
    [BackendNotificationPriority.Critical]: "Urgent",
  };

  const createdAt = parseISO(dto.createdAt);
  const date = format(createdAt, "yyyy-MM-dd");
  const time = format(createdAt, "HH:mm");

  return {
    id: dto.id,
    type: typeMap[dto.type] || "System",
    priority: priorityMap[dto.priority] || "Normal",
    title: dto.title,
    message: dto.content,
    date,
    time,
    read: dto.status === NotificationStatus.IsRead,
    relatedId: dto.recipientId, // Map recipientId to relatedId
    relatedName: undefined, // Extract from metadata if available
  };
};

export function NotificationDropdown({ onClose }: NotificationDropdownProps) {
  const { t: tCommon } = useTranslation("common");
  const { t } = useTranslation("notifications");
  const { user } = useAuth();
  const {
    filteredNotifications,
    isLoading,
    isSubmitting,
    permissions,
    markNotificationAsRead,
    markAllNotificationsAsRead,
    setSelectedNotification,
  } = useNotifications();
  const navigate = useNavigate();
  const [frontendNotifications, setFrontendNotifications] = useState<
    Notification[]
  >([]);
  const [selectedFrontendNotification, setSelectedFrontendNotification] =
    useState<Notification | null>(null);

  // Map backend notifications to frontend format
  useEffect(() => {
    const mappedNotifications = filteredNotifications.map(
      mapNotificationDtoToNotification
    );
    setFrontendNotifications(mappedNotifications);
  }, [filteredNotifications]);

  const unreadCount = frontendNotifications.filter((n) => !n.read).length;

  const markAsRead = async (id: string) => {
    try {
      await markNotificationAsRead(id);
      setFrontendNotifications((prev) =>
        prev.map((notification) =>
          notification.id === id
            ? { ...notification, read: true }
            : notification
        )
      );
      toast.success(t("notification_marked_as_read"));
    } catch (error) {
      console.error("Error marking notification as read:", error);
      toast.error(t("notification_marked_as_read_failed"));
    }
  };

  const markAllAsRead = async () => {
    const recipientId =
      user?.role === "Patient"
        ? user.patientId
        : user?.role === "Doctor"
        ? user.medecinId
        : "";
    if (!recipientId) {
      toast.error(t("no_valid_recipient_for_marking"));
      return;
    }
    try {
      await markAllNotificationsAsRead(recipientId);
      setFrontendNotifications((prev) =>
        prev.map((notification) => ({ ...notification, read: true }))
      );
      toast.success(t("all_notifications_marked_as_read"));
      onClose();
    } catch (error) {
      console.error("Error marking all notifications as read:", error);
      toast.error(t("all_notifications_marked_as_read_failed"));
    }
  };

  const handleNotificationClick = (notification: Notification) => {
    setSelectedFrontendNotification(notification);
    const backendNotification =
      filteredNotifications.find((n) => n.id === notification.id) || null;
    setSelectedNotification(backendNotification);
  };

  const viewAllNotifications = () => {
    navigate("/notifications");
    onClose();
  };

  const getNotificationTypeIcon = (type: Notification["type"]) => {
    switch (type) {
      case "Appointment":
        return <Calendar className="h-4 w-4" />;
      case "Consultation":
        return <FileText className="h-4 w-4" />;
      case "Message":
        return <MessageCircle className="h-4 w-4" />;
      case "System":
        return <Settings className="h-4 w-4" />;
      default:
        return <Bell className="h-4 w-4" />;
    }
  };

  const getNotificationPriorityClass = (priority: Notification["priority"]) => {
    switch (priority) {
      case "Normal":
        return "text-blue-600 bg-blue-50 border-blue-200";
      case "Important":
        return "text-amber-600 bg-amber-50 border-amber-200";
      case "Urgent":
        return "text-red-600 bg-red-50 border-red-200";
    }
  };

  const getNotificationPriorityLabel = (priority: Notification["priority"]) => {
    return t(`priority_${priority.toLowerCase()}`);
  };

  return (
    <>
      <div className="p-4 flex items-center justify-between">
        <div>
          <h3 className="font-medium">{t("title")}</h3>
          <p className="text-xs text-muted-foreground">
            {unreadCount > 0
              ? t("unread_notifications_other", {
                  values: { count: unreadCount },
                }).replace("{count}", unreadCount.toString())
              : t("all_up_to_date")}
          </p>
        </div>
        {unreadCount > 0 && permissions.canMarkAsRead && (
          <Button
            variant="ghost"
            size="sm"
            onClick={markAllAsRead}
            disabled={isSubmitting}
            className="h-8"
          >
            <CheckCircle className="h-4 w-4 mr-1" />
            {t("mark_all_as_read")}
          </Button>
        )}
      </div>

      <Separator />

      <div className="max-h-[300px] overflow-y-auto">
        {isLoading ? (
          <div className="p-4 text-center text-muted-foreground">
            {t("loading")}
          </div>
        ) : frontendNotifications.length === 0 ? (
          <div className="p-4 text-center text-muted-foreground">
            {t("no_notifications_found")}
          </div>
        ) : (
          frontendNotifications.map((notification) => (
            <div
              key={notification.id}
              className={cn(
                "p-3 border-b last:border-b-0 cursor-pointer hover:bg-muted/30",
                notification.read ? "" : "bg-muted/20"
              )}
              onClick={() => handleNotificationClick(notification)}
            >
              <div className="flex gap-3">
                <div
                  className={cn(
                    "rounded-full p-2",
                    notification.read ? "bg-muted" : "bg-clinic-100"
                  )}
                >
                  {getNotificationTypeIcon(notification.type)}
                </div>
                <div>
                  <div className="flex items-start justify-between">
                    <p className="font-medium text-sm">{notification.title}</p>
                    <Badge
                      variant="outline"
                      className={cn(
                        "ml-2 text-xs",
                        getNotificationPriorityClass(notification.priority)
                      )}
                    >
                      {getNotificationPriorityLabel(notification.priority)}
                    </Badge>
                  </div>
                  <p className="text-xs text-muted-foreground line-clamp-1 mt-1">
                    {notification.message}
                  </p>
                  <p className="text-xs text-muted-foreground mt-1">
                    {notification.date} {tCommon("at")} {notification.time}
                  </p>
                </div>
              </div>
              {!notification.read && (
                <div className="absolute top-3 right-3 h-2 w-2 rounded-full bg-clinic-500"></div>
              )}
            </div>
          ))
        )}
      </div>

      <Separator />

      <div className="p-2">
        <Button
          variant="outline"
          size="sm"
          className="w-full"
          onClick={viewAllNotifications}
        >
          <Eye className="h-4 w-4 mr-1" />
          {t("view_all_notifications")}
        </Button>
      </div>

      <NotificationPopup
        notification={selectedFrontendNotification}
        onClose={() => {
          setSelectedFrontendNotification(null);
          setSelectedNotification(null);
        }}
        onMarkAsRead={markAsRead}
      />
    </>
  );
}
