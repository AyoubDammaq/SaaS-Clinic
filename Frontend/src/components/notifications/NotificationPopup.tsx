
import { useEffect, useState } from "react";
import { X } from "lucide-react";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { useTranslation } from "@/hooks/useTranslation";

export interface Notification {
  id: string;
  title: string;
  message: string;
  type: "Appointment" | "Consultation" | "Message" | "System";
  priority: "Normal" | "Important" | "Urgent";
  date: string;
  time: string;
  read: boolean;
}

interface NotificationPopupProps {
  notification: Notification | null;
  onClose: () => void;
  onMarkAsRead: (id: string) => void;
}

export function NotificationPopup({ notification, onClose, onMarkAsRead }: NotificationPopupProps) {
  const [isOpen, setIsOpen] = useState(false);
  const { t } = useTranslation();

  useEffect(() => {
    setIsOpen(!!notification);
  }, [notification]);

  const handleClose = () => {
    setIsOpen(false);
    onClose();
  };

  const handleMarkAsRead = () => {
    if (notification) {
      onMarkAsRead(notification.id);
      handleClose();
    }
  };

  const getPriorityClass = (priority: string) => {
    switch (priority) {
      case "Normal":
        return "text-blue-600 bg-blue-50 border-blue-200";
      case "Important":
        return "text-amber-600 bg-amber-50 border-amber-200";
      case "Urgent":
        return "text-red-600 bg-red-50 border-red-200";
      default:
        return "text-blue-600 bg-blue-50 border-blue-200";
    }
  };

  return (
    <Dialog open={isOpen} onOpenChange={setIsOpen}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle className="flex items-center justify-between">
            <span>{notification?.title}</span>
            {notification && (
              <Badge variant="outline" className={getPriorityClass(notification.priority)}>
                {notification.priority}
              </Badge>
            )}
          </DialogTitle>
        </DialogHeader>
        <div className="space-y-4">
          <p className="text-sm text-gray-700">{notification?.message}</p>
          <div className="text-xs text-gray-500">
            {notification?.date} at {notification?.time}
          </div>
          <div className="flex justify-end gap-2">
            {!notification?.read && (
              <Button size="sm" onClick={handleMarkAsRead}>
                Mark as read
              </Button>
            )}
            <Button size="sm" variant="outline" onClick={handleClose}>
              {t('close')}
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
}
