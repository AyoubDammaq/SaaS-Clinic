
import { useState, useEffect } from 'react';
import { Bell, Calendar, FileText, MessageCircle, Settings, Check, Eye } from 'lucide-react';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Separator } from '@/components/ui/separator';
import { NotificationPopup, Notification } from '@/components/notifications/NotificationPopup';
import { useNavigate } from 'react-router-dom';
import { toast } from 'sonner';

interface NotificationDropdownProps {
  onClose: () => void;
}

// Mock notifications - in a real app, this would come from an API or context
const mockNotifications: Notification[] = [
  {
    id: '1',
    type: 'Appointment',
    priority: 'Normal',
    title: 'Upcoming Appointment',
    message: 'You have an appointment scheduled for tomorrow at 10:30 AM with Dr. Michael Brown.',
    date: '2025-04-29',
    time: '09:00',
    read: false
  },
  {
    id: '2',
    type: 'Consultation',
    priority: 'Important',
    title: 'Consultation Notes Available',
    message: 'Dr. Sarah Johnson has shared consultation notes from your recent visit.',
    date: '2025-04-28',
    time: '14:45',
    read: true
  },
  {
    id: '3',
    type: 'Message',
    priority: 'Normal',
    title: 'New Message',
    message: 'Dr. Emily Harris has sent you a message regarding your treatment plan.',
    date: '2025-04-28',
    time: '11:20',
    read: false
  }
];

export function NotificationDropdown({ onClose }: NotificationDropdownProps) {
  const navigate = useNavigate();
  const [notifications, setNotifications] = useState<Notification[]>(mockNotifications);
  const [selectedNotification, setSelectedNotification] = useState<Notification | null>(null);
  
  const unreadCount = notifications.filter(n => !n.read).length;
  
  const markAsRead = (id: string) => {
    setNotifications(prevNotifications => 
      prevNotifications.map(notification => 
        notification.id === id 
          ? { ...notification, read: true } 
          : notification
      )
    );
    toast.success("Notification marked as read");
  };

  const markAllAsRead = () => {
    setNotifications(prevNotifications => 
      prevNotifications.map(notification => ({ ...notification, read: true }))
    );
    toast.success("All notifications marked as read");
    onClose();
  };
  
  const handleNotificationClick = (notification: Notification) => {
    setSelectedNotification(notification);
  };
  
  const viewAllNotifications = () => {
    navigate('/notifications');
    onClose();
  };
  
  const getNotificationTypeIcon = (type: string) => {
    switch (type) {
      case 'Appointment':
        return <Calendar className="h-4 w-4" />;
      case 'Consultation':
        return <FileText className="h-4 w-4" />;
      case 'Message':
        return <MessageCircle className="h-4 w-4" />;
      case 'System':
        return <Settings className="h-4 w-4" />;
      default:
        return <Bell className="h-4 w-4" />;
    }
  };

  return (
    <>
      <div className="p-4 flex items-center justify-between">
        <div>
          <h3 className="font-medium">Notifications</h3>
          <p className="text-xs text-muted-foreground">
            {unreadCount > 0 ? `You have ${unreadCount} unread notification${unreadCount !== 1 ? 's' : ''}` : 'No new notifications'}
          </p>
        </div>
        {unreadCount > 0 && (
          <Button variant="ghost" size="sm" onClick={markAllAsRead} className="h-8">
            <Check className="h-4 w-4 mr-1" /> Mark all read
          </Button>
        )}
      </div>
      
      <Separator />

      <div className="max-h-[300px] overflow-y-auto">
        {notifications.length === 0 ? (
          <div className="p-4 text-center text-muted-foreground">
            No notifications
          </div>
        ) : (
          notifications.map(notification => (
            <div 
              key={notification.id}
              className={`p-3 border-b last:border-b-0 cursor-pointer hover:bg-muted/30 ${!notification.read ? 'bg-muted/20' : ''}`}
              onClick={() => handleNotificationClick(notification)}
            >
              <div className="flex gap-3">
                <div className={`rounded-full p-2 ${notification.read ? 'bg-muted' : 'bg-clinic-100'}`}>
                  {getNotificationTypeIcon(notification.type)}
                </div>
                <div>
                  <div className="flex items-start justify-between">
                    <p className="font-medium text-sm">{notification.title}</p>
                    {!notification.read && (
                      <Badge variant="outline" className="ml-2 text-xs bg-clinic-50 text-clinic-700 border-clinic-200">
                        New
                      </Badge>
                    )}
                  </div>
                  <p className="text-xs text-muted-foreground line-clamp-1 mt-1">
                    {notification.message}
                  </p>
                  <p className="text-xs text-muted-foreground mt-1">
                    {notification.date} at {notification.time}
                  </p>
                </div>
              </div>
            </div>
          ))
        )}
      </div>

      <Separator />
      
      <div className="p-2">
        <Button variant="outline" size="sm" className="w-full" onClick={viewAllNotifications}>
          <Eye className="h-4 w-4 mr-1" /> View all notifications
        </Button>
      </div>

      <NotificationPopup 
        notification={selectedNotification}
        onClose={() => setSelectedNotification(null)}
        onMarkAsRead={markAsRead}
      />
    </>
  );
}
