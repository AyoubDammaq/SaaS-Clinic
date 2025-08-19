import { api } from "@/utils/apiClient";
import { API_ENDPOINTS } from "@/config/api";
import {
  CreateNotificationRequest,
  NotificationDto,
  NotificationFilterRequest,
  NotificationSummaryDto,
  MarkNotificationAsSentRequest,
} from "@/types/notification";
import { toast } from "sonner";

export const notificationService = {
  // Create a new notification
  createNotification: async (
    request: CreateNotificationRequest
  ): Promise<{ id: string }> => {
    try {
      return await api.post<{ id: string }>(
        API_ENDPOINTS.NOTIFICATIONS.BASE,
        request
      );
    } catch (error) {
      console.error("Échec lors de la création de la notification:", error);
      toast.error("Échec lors de la création de la notification");
      throw error;
    }
  },

  // Get a notification by ID
  getNotificationById: async (id: string): Promise<NotificationDto | null> => {
    try {
      return await api.get<NotificationDto>(
        API_ENDPOINTS.NOTIFICATIONS.GET_BY_ID(id)
      );
    } catch (error) {
      console.error(
        `Échec lors de la récupération de la notification ${id}:`,
        error
      );
      toast.error("Échec lors de la récupération de la notification");
      throw error;
    }
  },

  // Get notifications with filters
  getNotifications: async (
    filter: NotificationFilterRequest
  ): Promise<NotificationSummaryDto[]> => {
    try {
      const queryParams = new URLSearchParams();
      if (filter.recipientId)
        queryParams.append("recipientId", filter.recipientId);
      if (filter.recipientType)
        queryParams.append("recipientType", filter.recipientType.toString());
      if (filter.status) queryParams.append("status", filter.status.toString());
      if (filter.type) queryParams.append("type", filter.type.toString());
      if (filter.from) queryParams.append("from", filter.from);
      if (filter.to) queryParams.append("to", filter.to);
      if (filter.page) queryParams.append("page", filter.page.toString());
      if (filter.pageSize)
        queryParams.append("pageSize", filter.pageSize.toString());

      const url = `${
        API_ENDPOINTS.NOTIFICATIONS.GET_ALL
      }?${queryParams.toString()}`;
      return await api.get<NotificationSummaryDto[]>(url);
    } catch (error) {
      console.error("Échec lors de la récupération des notifications:", error);
      throw error;
    }
  },

  // Send a notification
  sendNotification: async (request: {
    notificationId: string;
  }): Promise<void> => {
    try {
      await api.post<void>(API_ENDPOINTS.NOTIFICATIONS.SEND, request);
    } catch (error) {
      console.error("Échec lors de l'envoi de la notification:", error);
      toast.error("Échec lors de l'envoi de la notification");
      throw error;
    }
  },

  // Mark a notification as sent
  markAsSent: async (request: MarkNotificationAsSentRequest): Promise<void> => {
    try {
      await api.put<void>(API_ENDPOINTS.NOTIFICATIONS.MARK_AS_SENT, request);
    } catch (error) {
      console.error(
        "Échec lors du marquage de la notification comme envoyée:",
        error
      );
      toast.error("Échec lors du marquage de la notification comme envoyée");
      throw error;
    }
  },

  // Get notifications by recipient ID
  getNotificationsByRecipientId: async (
    recipientId: string
  ): Promise<NotificationDto[]> => {
    try {
      return await api.get<NotificationDto[]>(
        API_ENDPOINTS.NOTIFICATIONS.GET_BY_RECIPIENT_ID(recipientId)
      );
    } catch (error) {
      console.error(
        `Échec lors de la récupération des notifications pour le destinataire ${recipientId}:`,
        error
      );
      toast.error(
        "Échec lors de la récupération des notifications pour le destinataire"
      );
      throw error;
    }
  },

  // Delete a notification
  deleteNotification: async (notificationId: string): Promise<void> => {
    try {
      await api.delete<void>(
        API_ENDPOINTS.NOTIFICATIONS.DELETE(notificationId)
      );
    } catch (error) {
      console.error(
        `Échec lors de la suppression de la notification ${notificationId}:`,
        error
      );
      toast.error("Échec lors de la suppression de la notification");
      throw error;
    }
  },

  // Delete all notifications for a recipient
  deleteAllNotifications: async (
    recipientId: string
  ): Promise<void> => {
    try {
      await api.delete<void>(
        API_ENDPOINTS.NOTIFICATIONS.DELETE_ALL(recipientId)
      );
    } catch (error) {
      console.error(
        `Échec lors de la suppression de toutes les notifications pour le destinataire ${recipientId}:`,
        error
      );
      toast.error("Échec lors de la suppression des notifications");
      throw error;
    }
  },

  // Mark a notification as read
  markNotificationAsRead: async (id: string): Promise<void> => {
    try {
      await api.put<void>(API_ENDPOINTS.NOTIFICATIONS.MARK_AS_READ(id));
    } catch (error) {
      console.error(
        `Échec lors du marquage de la notification ${id} comme lue:`,
        error
      );
      toast.error("Échec lors du marquage de la notification comme lue");
      throw error;
    }
  },

  // Mark all notifications as read for a recipient
  markAllNotificationsAsRead: async (recipientId: string): Promise<void> => {
    try {
      await api.put<void>(
        API_ENDPOINTS.NOTIFICATIONS.MARK_ALL_AS_READ(recipientId)
      );
    } catch (error) {
      console.error(
        `Échec lors du marquage de toutes les notifications comme lues pour le destinataire ${recipientId}:`,
        error
      );
      toast.error(
        "Échec lors du marquage de toutes les notifications comme lues"
      );
      throw error;
    }
  },
};
