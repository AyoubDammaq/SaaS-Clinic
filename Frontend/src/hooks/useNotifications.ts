import { useState, useEffect, useCallback } from "react";
import { toast } from "sonner";
import { useAuth } from "@/hooks/useAuth";
import { notificationService } from "@/services/notificationService";
import {
  NotificationDto,
  CreateNotificationRequest,
  NotificationFilterRequest,
  MarkNotificationAsSentRequest,
  NotificationStatus,
  SendNotificationRequest,
  NotificationPriority,
} from "@/types/notification";

interface UseNotificationsState {
  notifications: NotificationDto[];
  filteredNotifications: NotificationDto[];
  isLoading: boolean;
  isSubmitting: boolean;
  permissions: {
    canCreate: boolean;
    canEdit: boolean;
    canDelete: boolean;
    canView: boolean;
    canMarkAsRead: boolean;
  };
  selectedNotification: NotificationDto | null;
  searchTerm: string;
  setSearchTerm: (term: string) => void;
  setSelectedNotification: (notification: NotificationDto | null) => void;
  createNotification: (
    data: CreateNotificationRequest
  ) => Promise<{ id: string }>;
  getNotificationById: (id: string) => Promise<void>;
  getNotifications: (filter: NotificationFilterRequest) => Promise<void>;
  sendNotification: (data: SendNotificationRequest) => Promise<void>;
  markAsSent: (data: MarkNotificationAsSentRequest) => Promise<void>;
  getNotificationsByRecipientId: (recipientId: string) => Promise<void>;
  deleteNotification: (notificationId: string) => Promise<void>;
  markNotificationAsRead: (id: string) => Promise<void>;
  markAllNotificationsAsRead: (recipientId: string) => Promise<void>;
  refetchNotifications: () => Promise<void>;
}

export function useNotifications(): UseNotificationsState {
  const { user } = useAuth();
  const [notifications, setNotifications] = useState<NotificationDto[]>([]);
  const [filteredNotifications, setFilteredNotifications] = useState<
    NotificationDto[]
  >([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [searchTerm, setSearchTerm] = useState("");
  const [selectedNotification, setSelectedNotification] =
    useState<NotificationDto | null>(null);
  const [permissions, setPermissions] = useState({
    canCreate: false,
    canEdit: false,
    canDelete: false,
    canView: false,
    canMarkAsRead: false,
  });

  // Set permissions based on user role
  useEffect(() => {
    if (user) {
      const canCreate =
        user.role === "SuperAdmin" || user.role === "ClinicAdmin";
      const canEdit = user.role === "SuperAdmin" || user.role === "ClinicAdmin";
      const canDelete = user.role === "SuperAdmin";
      const canView = true;
      const canMarkAsRead = user.role === "Doctor" || user.role === "Patient";
      setPermissions({ canCreate, canEdit, canDelete, canView, canMarkAsRead });
    }
  }, [user]);

  // Fetch notifications for the current user
  const fetchNotifications = useCallback(async () => {
    setIsLoading(true);
    try {
      let data: NotificationDto[] = [];

      if (user?.role === "Patient" && user.patientId) {
        data = await notificationService.getNotificationsByRecipientId(
          user.patientId
        );
      } else if (user?.role === "Doctor" && user.medecinId) {
        data = await notificationService.getNotificationsByRecipientId(
          user.medecinId
        );
      } else if (user?.role === "ClinicAdmin" || user?.role === "SuperAdmin") {
        data = (await notificationService.getNotifications({
          page: 1,
          pageSize: 20,
        })) as NotificationDto[];
      } else {
        data = [];
      }

      setNotifications(data);
      setFilteredNotifications(data);
    } catch (error) {
      console.error("Erreur lors de la récupération des notifications:", error);
      toast.error("Échec du chargement des notifications");
    } finally {
      setIsLoading(false);
    }
  }, [user]);

  // Filter notifications based on search term
  useEffect(() => {
    if (notifications.length === 0) return;

    const results = notifications.filter(
      (notification) =>
        notification.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
        notification.content.toLowerCase().includes(searchTerm.toLowerCase())
    );
    setFilteredNotifications(results);
  }, [searchTerm, notifications]);

  // Load initial notifications
  useEffect(() => {
    fetchNotifications();
  }, [fetchNotifications]);

  // Create a new notification
  const createNotification = async (
    data: CreateNotificationRequest
  ): Promise<{ id: string }> => {
    setIsSubmitting(true);
    try {
      const newNotification = await notificationService.createNotification(
        data
      );
      const newDto: NotificationDto = {
        id: newNotification.id,
        recipientId: data.recipientId,
        recipientType: data.recipientType,
        type: data.type,
        channel: 2, // Default to Email if not specified
        title: data.title,
        content: data.content,
        priority: data.priority || NotificationPriority.Normal,
        status: NotificationStatus.Pending,
        createdAt: new Date().toISOString(),
        sentAt: null,
      };
      setNotifications((prev) => [...prev, newDto]);
      setFilteredNotifications((prev) => [...prev, newDto]);
      toast.success("Notification créée avec succès");
      return newNotification;
    } catch (error) {
      console.error("Erreur lors de la création de la notification:", error);
      toast.error("Échec de la création de la notification");
      throw error;
    } finally {
      setIsSubmitting(false);
    }
  };

  // Get a notification by ID
  const getNotificationById = async (id: string) => {
    setIsLoading(true);
    try {
      const data = await notificationService.getNotificationById(id);
      if (data) {
        setSelectedNotification(data);
      } else {
        toast.error("Notification non trouvée");
      }
    } catch (error) {
      console.error(
        "Erreur lors de la récupération de la notification:",
        error
      );
      toast.error("Échec de la récupération de la notification");
    } finally {
      setIsLoading(false);
    }
  };

  // Get notifications with filters
  const getNotifications = async (filter: NotificationFilterRequest) => {
    setIsLoading(true);
    try {
      const data = (await notificationService.getNotifications(
        filter
      )) as NotificationDto[];
      setFilteredNotifications(data);
    } catch (error) {
      console.error("Erreur lors de la récupération des notifications:", error);
      toast.error("Échec de la récupération des notifications");
    } finally {
      setIsLoading(false);
    }
  };

  // Send a notification
  const sendNotification = async (data: SendNotificationRequest) => {
    setIsSubmitting(true);
    try {
      await notificationService.sendNotification(data);
      toast.success("Notification envoyée avec succès");
    } catch (error) {
      console.error("Erreur lors de l'envoi de la notification:", error);
      toast.error("Échec de l'envoi de la notification");
      throw error;
    } finally {
      setIsSubmitting(false);
    }
  };

  // Mark a notification as sent
  const markAsSent = async (data: MarkNotificationAsSentRequest) => {
    setIsSubmitting(true);
    try {
      await notificationService.markAsSent(data);
      setNotifications((prev) =>
        prev.map((notification) =>
          notification.id === data.notificationId
            ? {
                ...notification,
                status: NotificationStatus.Sent,
                sentAt: data.sentAt,
              }
            : notification
        )
      );
      setFilteredNotifications((prev) =>
        prev.map((notification) =>
          notification.id === data.notificationId
            ? {
                ...notification,
                status: NotificationStatus.Sent,
                sentAt: data.sentAt,
              }
            : notification
        )
      );
      toast.success("Notification marquée comme envoyée avec succès");
    } catch (error) {
      console.error(
        "Erreur lors du marquage de la notification comme envoyée:",
        error
      );
      toast.error("Échec du marquage de la notification comme envoyée");
      throw error;
    } finally {
      setIsSubmitting(false);
    }
  };

  // Get notifications by recipient ID
  const getNotificationsByRecipientId = async (recipientId: string) => {
    setIsLoading(true);
    try {
      const data = await notificationService.getNotificationsByRecipientId(
        recipientId
      );
      setNotifications(data);
      setFilteredNotifications(data);
    } catch (error) {
      console.error(
        "Erreur lors de la récupération des notifications par destinataire:",
        error
      );
      toast.error(
        "Échec de la récupération des notifications par destinataire"
      );
    } finally {
      setIsLoading(false);
    }
  };

  // Delete a notification
  const deleteNotification = async (notificationId: string) => {
    if (
      !window.confirm("Êtes-vous sûr de vouloir supprimer cette notification ?")
    ) {
      return;
    }

    setIsSubmitting(true);
    try {
      await notificationService.deleteNotification(notificationId);
      setNotifications((prev) =>
        prev.filter((notification) => notification.id !== notificationId)
      );
      setFilteredNotifications((prev) =>
        prev.filter((notification) => notification.id !== notificationId)
      );
      toast.success("Notification supprimée avec succès");
    } catch (error) {
      console.error("Erreur lors de la suppression de la notification:", error);
      toast.error("Échec de la suppression de la notification");
      throw error;
    } finally {
      setIsSubmitting(false);
    }
  };

  // Mark a notification as read
  const markNotificationAsRead = async (id: string) => {
    setIsSubmitting(true);
    try {
      await notificationService.markNotificationAsRead(id);
      setNotifications((prev) =>
        prev.map((notification) =>
          notification.id === id
            ? { ...notification, status: NotificationStatus.IsRead }
            : notification
        )
      );
      setFilteredNotifications((prev) =>
        prev.map((notification) =>
          notification.id === id
            ? { ...notification, status: NotificationStatus.IsRead }
            : notification
        )
      );
      toast.success("Notification marquée comme lue avec succès");
    } catch (error) {
      console.error(
        "Erreur lors du marquage de la notification comme lue:",
        error
      );
      toast.error("Échec du marquage de la notification comme lue");
      throw error;
    } finally {
      setIsSubmitting(false);
    }
  };

  // Mark all notifications as read for a recipient
  const markAllNotificationsAsRead = async (recipientId: string) => {
    setIsSubmitting(true);
    try {
      await notificationService.markAllNotificationsAsRead(recipientId);
      setNotifications((prev) =>
        prev.map((notification) =>
          notification.recipientId === recipientId
            ? { ...notification, status: NotificationStatus.IsRead }
            : notification
        )
      );
      setFilteredNotifications((prev) =>
        prev.map((notification) =>
          notification.recipientId === recipientId
            ? { ...notification, status: NotificationStatus.IsRead }
            : notification
        )
      );
      toast.success("Toutes les notifications marquées comme lues avec succès");
    } catch (error) {
      console.error(
        "Erreur lors du marquage de toutes les notifications comme lues:",
        error
      );
      toast.error("Échec du marquage de toutes les notifications comme lues");
      throw error;
    } finally {
      setIsSubmitting(false);
    }
  };

  return {
    notifications,
    filteredNotifications,
    isLoading,
    isSubmitting,
    permissions,
    selectedNotification,
    searchTerm,
    setSearchTerm,
    setSelectedNotification,
    createNotification,
    getNotificationById,
    getNotifications,
    sendNotification,
    markAsSent,
    getNotificationsByRecipientId,
    deleteNotification,
    markNotificationAsRead,
    markAllNotificationsAsRead,
    refetchNotifications: fetchNotifications,
  };
}
