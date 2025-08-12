import { useState, useEffect } from "react";
import { useAuth } from "@/hooks/useAuth";
import { useNotifications } from "@/hooks/useNotifications";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import {
  Bell,
  Calendar,
  Clock,
  FileText,
  MessageCircle,
  Settings,
  Trash2,
  User,
  CheckCircle,
} from "lucide-react";
import { Badge } from "@/components/ui/badge";
import { cn } from "@/lib/utils";
import { NotificationPopup } from "@/components/notifications/NotificationPopup";
import { toast } from "sonner";
import { format, parseISO } from "date-fns";
import {
  NotificationDto,
  NotificationType as BackendNotificationType,
  NotificationStatus,
  NotificationPriority as BackendNotificationPriority,
} from "@/types/notification";
import {
  Pagination,
  PaginationContent,
  PaginationItem,
  PaginationNext,
  PaginationPrevious,
} from "@/components/ui/pagination";
import { useTranslation } from "@/hooks/useTranslation";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
  DialogFooter,
} from "@/components/ui/dialog";

type NotificationType = "Appointment" | "Consultation" | "Message" | "System";
type NotificationPriority = "Normal" | "Important" | "Urgent";

interface Notification {
  id: string;
  type: NotificationType;
  priority: NotificationPriority;
  title: string;
  message: string;
  date: string;
  time: string;
  read: boolean;
  relatedId?: string;
  relatedName?: string;
}

interface ConfirmDeleteDialogProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: () => void;
  title: string;
  message: string;
}

const ConfirmDeleteDialog = ({
  isOpen,
  onClose,
  onConfirm,
  title,
  message,
}: ConfirmDeleteDialogProps) => {
  const { t } = useTranslation("notifications"); // Use notifications namespace for consistency

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{title}</DialogTitle>
          <DialogDescription>{message}</DialogDescription>
        </DialogHeader>
        <DialogFooter>
          <Button variant="outline" onClick={onClose}>
            {t("cancel")}
          </Button>
          <Button variant="destructive" onClick={onConfirm}>
            {t("confirm")}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};

const mapNotificationDtoToNotification = (
  dto: NotificationDto
): Notification => {
  // Map backend NotificationType to frontend NotificationType
  const typeMap: Record<number, NotificationType> = {
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

  // Map backend NotificationPriority to frontend NotificationPriority
  const priorityMap: Record<BackendNotificationPriority, NotificationPriority> =
    {
      [BackendNotificationPriority.Low]: "Normal",
      [BackendNotificationPriority.Normal]: "Normal",
      [BackendNotificationPriority.High]: "Important",
      [BackendNotificationPriority.Critical]: "Urgent",
    };

  // Parse createdAt for date and time
  const createdAt = parseISO(dto.createdAt);
  const date = format(createdAt, "yyyy-MM-dd");
  const time = format(createdAt, "HH:mm");

  return {
    id: dto.id,
    type: typeMap[dto.type] || "System", // Fallback to System
    priority: priorityMap[dto.priority] || "Normal", // Fallback to Normal
    title: dto.title,
    message: dto.content,
    date,
    time,
    read: dto.status === NotificationStatus.IsRead,
    relatedId: dto.recipientId, // Use recipientId as relatedId (adjust if needed)
    relatedName: undefined, // Fetch or derive from metadata if available
  };
};

function NotificationsPage() {
  const { t: tCommon } = useTranslation("common");
  const { t } = useTranslation("notifications");
  const { user } = useAuth();
  const {
    filteredNotifications,
    isLoading,
    isSubmitting,
    permissions,
    searchTerm,
    setSearchTerm,
    markNotificationAsRead,
    markAllNotificationsAsRead,
    deleteNotification,
    deleteAllNotifications,
    setSelectedNotification,
  } = useNotifications();
  const [filter, setFilter] = useState<NotificationType | "All">("All");
  const [frontendNotifications, setFrontendNotifications] = useState<
    Notification[]
  >([]);
  const [selectedFrontendNotification, setSelectedFrontendNotification] =
    useState<Notification | null>(null);
  const [currentPage, setCurrentPage] = useState(1); // State for current page
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const [isDeleteAllDialogOpen, setIsDeleteAllDialogOpen] = useState(false);
  const [notificationToDelete, setNotificationToDelete] = useState<
    string | null
  >(null);

  // Map backend notifications to frontend format and filter by search term
  useEffect(() => {
    const mappedNotifications = filteredNotifications.map(
      mapNotificationDtoToNotification
    );
    const filteredBySearch = mappedNotifications.filter(
      (notification) =>
        notification.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
        notification.message.toLowerCase().includes(searchTerm.toLowerCase())
    );
    setFrontendNotifications(
      filter === "All"
        ? filteredBySearch
        : filteredBySearch.filter((n) => n.type === filter)
    );
    setCurrentPage(1); // Reset to first page when filters change
  }, [filteredNotifications, searchTerm, filter]);

  const unreadCount = frontendNotifications.filter((n) => !n.read).length;

  // Pagination constants
  const ITEMS_PER_PAGE = 5;
  const totalPages = Math.ceil(frontendNotifications.length / ITEMS_PER_PAGE);
  const startIndex = (currentPage - 1) * ITEMS_PER_PAGE;
  const paginatedNotifications = frontendNotifications.slice(
    startIndex,
    startIndex + ITEMS_PER_PAGE
  );

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
    } catch (error) {
      console.error("Error marking all notifications as read:", error);
      toast.error(t("all_notifications_marked_as_read_failed"));
    }
  };

  const handleDeleteAll = () => {
    setIsDeleteAllDialogOpen(true);
  };

  const confirmDeleteAll = async () => {
    const recipientId =
      user?.role === "Patient"
        ? user.patientId
        : user?.role === "Doctor"
        ? user.medecinId
        : "";
    if (!recipientId) {
      toast.error(t("no_valid_recipient_for_deleting"));
      setIsDeleteAllDialogOpen(false);
      return;
    }
    try {
      await deleteAllNotifications(recipientId);
      setFrontendNotifications([]);
      toast.success(t("all_notifications_deleted"));
    } catch (error) {
      console.error("Error deleting all notifications:", error);
      toast.error(t("all_notifications_deleted_failed"));
    } finally {
      setIsDeleteAllDialogOpen(false);
    }
  };

  const handleDelete = (id: string) => {
    setNotificationToDelete(id);
    setIsDeleteDialogOpen(true);
  };

  const confirmDelete = async () => {
    if (!notificationToDelete) return;
    try {
      await deleteNotification(notificationToDelete);
      setFrontendNotifications((prev) =>
        prev.filter((notification) => notification.id !== notificationToDelete)
      );
      toast.success(t("notification_deleted"));
    } catch (error) {
      console.error("Error deleting notification:", error);
      toast.error(t("notification_deleted_failed"));
    } finally {
      setIsDeleteDialogOpen(false);
      setNotificationToDelete(null);
    }
  };

  const handleNotificationClick = (notification: Notification) => {
    setSelectedFrontendNotification(notification);
    // Find the corresponding NotificationDto for the popup
    const backendNotification =
      filteredNotifications.find((n) => n.id === notification.id) || null;
    setSelectedNotification(backendNotification);
  };

  const getNotificationTypeIcon = (type: NotificationType) => {
    switch (type) {
      case "Appointment":
        return <Calendar className="h-4 w-4" />;
      case "Consultation":
        return <FileText className="h-4 w-4" />;
      case "Message":
        return <MessageCircle className="h-4 w-4" />;
      case "System":
        return <Settings className="h-4 w-4" />;
    }
  };

  const getNotificationPriorityLabel = (priority: NotificationPriority) => {
    return t(`priority_${priority.toLowerCase()}`);
  };

  const getNotificationTypeLabel = (type: NotificationType) => {
    return t(`type_${type.toLowerCase()}`);
  };

  const getNotificationPriorityClass = (priority: NotificationPriority) => {
    switch (priority) {
      case "Normal":
        return "text-blue-600 bg-blue-50 border-blue-200";
      case "Important":
        return "text-amber-600 bg-amber-50 border-amber-200";
      case "Urgent":
        return "text-red-600 bg-red-50 border-red-200";
    }
  };

  return (
    <div className="space-y-6 pb-8">
      <div className="flex flex-col gap-2">
        <h1 className="text-3xl font-bold tracking-tight">{t("title")}</h1>
        <p className="text-muted-foreground">{t("description")}</p>
      </div>

      <div className="flex items-center justify-between mb-4">
        <div className="flex items-center gap-2 flex-wrap">
          <Button
            variant={filter === "All" ? "default" : "outline"}
            size="sm"
            onClick={() => setFilter("All")}
          >
            {t("filter_all")}
          </Button>
          <Button
            variant={filter === "Appointment" ? "default" : "outline"}
            size="sm"
            onClick={() => setFilter("Appointment")}
          >
            <Calendar className="mr-1 h-4 w-4" />
            {t("filter_appointment")}
          </Button>
          <Button
            variant={filter === "Consultation" ? "default" : "outline"}
            size="sm"
            onClick={() => setFilter("Consultation")}
          >
            <FileText className="mr-1 h-4 w-4" />
            {t("filter_consultation")}
          </Button>
          <Button
            variant={filter === "Message" ? "default" : "outline"}
            size="sm"
            onClick={() => setFilter("Message")}
          >
            <MessageCircle className="mr-1 h-4 w-4" />
            {t("filter_message")}
          </Button>
        </div>

        <div className="flex items-center gap-2">
          {unreadCount > 0 && permissions.canMarkAsRead && (
            <Button
              variant="outline"
              size="sm"
              onClick={markAllAsRead}
              disabled={isSubmitting}
            >
              <CheckCircle className="mr-1 h-4 w-4" />
              {t("mark_all_as_read")}
            </Button>
          )}
          {permissions.canDelete && (
            <Button
              variant="outline"
              size="sm"
              onClick={handleDeleteAll}
              disabled={isSubmitting || frontendNotifications.length === 0}
            >
              <Trash2 className="mr-1 h-4 w-4" />
              {t("delete_all")}
            </Button>
          )}
        </div>
      </div>

      <Card>
        <CardHeader>
          <div className="flex items-center justify-between">
            <div>
              <CardTitle>{t("your_notifications")}</CardTitle>
              <CardDescription>
                {unreadCount === 0
                  ? t("all_up_to_date")
                  : t("unread_notifications_other", {
                      values: { count: unreadCount },
                    }).replace("{count}", unreadCount.toString())}
              </CardDescription>
            </div>
            <Bell
              className={cn(
                "h-6 w-6",
                unreadCount > 0 ? "text-clinic-500" : "text-muted-foreground"
              )}
            />
          </div>
        </CardHeader>
        <CardContent className="space-y-4">
          {isLoading ? (
            <div className="text-center py-8 text-muted-foreground">
              {t("loading")}
            </div>
          ) : paginatedNotifications.length === 0 ? (
            <div className="text-center py-8 text-muted-foreground">
              {t("no_notifications_found")}
            </div>
          ) : (
            paginatedNotifications.map((notification) => (
              <div
                key={notification.id}
                className={cn(
                  "border p-4 rounded-lg relative cursor-pointer transition-colors",
                  notification.read ? "bg-background" : "bg-muted/20",
                  "hover:bg-muted/30"
                )}
                onClick={() => handleNotificationClick(notification)}
              >
                <div className="flex items-start gap-4">
                  <div
                    className={cn(
                      "p-2 rounded-full",
                      notification.read ? "bg-muted" : "bg-clinic-100"
                    )}
                  >
                    {getNotificationTypeIcon(notification.type)}
                  </div>
                  <div className="flex-1">
                    <div className="flex items-center justify-between">
                      <h4 className="font-medium">{notification.title}</h4>
                      <Badge
                        variant="outline"
                        className={getNotificationPriorityClass(
                          notification.priority
                        )}
                      >
                        {getNotificationPriorityLabel(notification.priority)}
                      </Badge>
                    </div>
                    <p className="text-muted-foreground mt-1">
                      {notification.message}
                    </p>

                    {notification.relatedName && (
                      <div className="text-sm mt-2 text-muted-foreground">
                        <User className="inline h-3.5 w-3.5 mr-1" />
                        {notification.relatedName}
                      </div>
                    )}

                    <div className="flex items-center justify-between mt-2">
                      <div className="text-sm text-muted-foreground flex items-center">
                        <Clock className="inline h-3.5 w-3.5 mr-1" />
                        {notification.date} à {notification.time}
                      </div>
                      <div className="flex gap-2">
                        {!notification.read && permissions.canMarkAsRead && (
                          <Button
                            size="sm"
                            variant="ghost"
                            onClick={(e) => {
                              e.stopPropagation();
                              markAsRead(notification.id);
                            }}
                            disabled={isSubmitting}
                          >
                            {t("mark_as_read")}
                          </Button>
                        )}
                        {permissions.canDelete && (
                          <Button
                            variant="ghost"
                            size="icon"
                            onClick={(e) => {
                              e.stopPropagation();
                              handleDelete(notification.id);
                            }}
                            disabled={isSubmitting}
                          >
                            <Trash2 className="h-4 w-4 text-muted-foreground" />
                          </Button>
                        )}
                      </div>
                    </div>
                  </div>
                </div>
                {!notification.read && (
                  <div className="absolute top-4 right-4 h-2 w-2 rounded-full bg-clinic-500"></div>
                )}
              </div>
            ))
          )}
          {totalPages > 1 && (
            <Pagination>
              <PaginationContent>
                <PaginationItem>
                  <PaginationPrevious
                    onClick={() =>
                      setCurrentPage((prev) => Math.max(prev - 1, 1))
                    }
                    className={cn(
                      currentPage <= 1 && "pointer-events-none opacity-50"
                    )}
                  />
                </PaginationItem>
                <PaginationItem className="flex items-center">
                  <span className="text-sm">
                    {tCommon("page")} {currentPage} {tCommon("of")} {totalPages}
                  </span>
                </PaginationItem>
                <PaginationItem>
                  <PaginationNext
                    onClick={() =>
                      setCurrentPage((prev) => Math.min(prev + 1, totalPages))
                    }
                    className={cn(
                      currentPage >= totalPages &&
                        "pointer-events-none opacity-50"
                    )}
                  />
                </PaginationItem>
              </PaginationContent>
            </Pagination>
          )}
        </CardContent>
      </Card>

      <ConfirmDeleteDialog
        isOpen={isDeleteDialogOpen}
        onClose={() => {
          setIsDeleteDialogOpen(false);
          setNotificationToDelete(null);
        }}
        onConfirm={confirmDelete}
        title={t("confirm_delete_title")}
        message={t("confirm_delete_message")}
      />

      <ConfirmDeleteDialog
        isOpen={isDeleteAllDialogOpen}
        onClose={() => setIsDeleteAllDialogOpen(false)}
        onConfirm={confirmDeleteAll}
        title={t("confirm_delete_all_title")}
        message={t("confirm_delete_all")}
      />

      <NotificationPopup
        notification={selectedFrontendNotification}
        onClose={() => {
          setSelectedFrontendNotification(null);
          setSelectedNotification(null);
        }}
        onMarkAsRead={markAsRead}
      />
    </div>
  );
}

export default NotificationsPage;
