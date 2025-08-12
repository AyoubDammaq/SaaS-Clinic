import { useEffect, useState } from 'react';
import { X } from 'lucide-react';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription } from '@/components/ui/dialog';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { useTranslation } from '@/hooks/useTranslation';
import { cn } from '@/lib/utils';

export interface Notification {
  id: string;
  title: string;
  message: string;
  type: 'Appointment' | 'Consultation' | 'Message' | 'System';
  priority: 'Normal' | 'Important' | 'Urgent';
  date: string;
  time: string;
  read: boolean;
  relatedId?: string;
  relatedName?: string;
}

interface NotificationPopupProps {
  notification: Notification | null;
  onClose: () => void;
  onMarkAsRead: (id: string) => void;
}

export function NotificationPopup({ notification, onClose, onMarkAsRead }: NotificationPopupProps) {
  const { t: tCommon } = useTranslation('common');
  const { t } = useTranslation('notifications');
  const [isOpen, setIsOpen] = useState(false);

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

  const getPriorityClass = (priority: Notification['priority']) => {
    switch (priority) {
      case 'Normal':
        return 'text-blue-600 bg-blue-50 border-blue-200';
      case 'Important':
        return 'text-amber-600 bg-amber-50 border-amber-200';
      case 'Urgent':
        return 'text-red-600 bg-red-50 border-red-200';
      default:
        return 'text-blue-600 bg-blue-50 border-blue-200';
    }
  };

  const getPriorityLabel = (priority: Notification['priority']) => {
    return t(`priority_${priority.toLowerCase()}`);
  };

  return (
    <Dialog open={isOpen} onOpenChange={setIsOpen}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle className="flex items-center justify-between">
            <span>{notification?.title}</span>
            {notification && (
              <Badge variant="outline" className={cn(getPriorityClass(notification.priority))}>
                {getPriorityLabel(notification.priority)}
              </Badge>
            )}
          </DialogTitle>
          <DialogDescription className="sr-only">
            {t('notification_details')}
          </DialogDescription>
        </DialogHeader>
        <div className="space-y-4">
          <p className="text-sm text-muted-foreground">{notification?.message}</p>
          {notification?.relatedName && (
            <p className="text-sm text-muted-foreground">
              <span className="font-medium">{t('related_to')}:</span> {notification.relatedName}
            </p>
          )}
          <div className="text-xs text-muted-foreground">
            {notification?.date} {tCommon('at')} {notification?.time}
          </div>
          <div className="flex justify-end gap-2">
            {!notification?.read && (
              <Button size="sm" onClick={handleMarkAsRead}>
                {t('mark_as_read')}
              </Button>
            )}
            <Button size="sm" variant="outline" onClick={handleClose}>
              <X className="h-4 w-4 mr-1" />
              {tCommon('close')}
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
}