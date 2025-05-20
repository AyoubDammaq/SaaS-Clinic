
import { useState, useEffect } from 'react';
import { useAuth } from '@/contexts/AuthContext';
import { 
  Card, CardContent, CardDescription, CardHeader, CardTitle 
} from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import {
  Bell,
  Calendar,
  Clock,
  FileText,
  MessageCircle,
  Settings,
  Trash2,
  User,
  CheckCircle
} from 'lucide-react';
import { Badge } from '@/components/ui/badge';
import { cn } from '@/lib/utils';
import { NotificationPopup } from '@/components/notifications/NotificationPopup';
import { toast } from 'sonner';

type NotificationType = 'Appointment' | 'Consultation' | 'Message' | 'System';
type NotificationPriority = 'Normal' | 'Important' | 'Urgent';

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

const mockNotifications: Notification[] = [
  {
    id: '1',
    type: 'Appointment',
    priority: 'Normal',
    title: 'Upcoming Appointment Reminder',
    message: 'You have an appointment scheduled for tomorrow at 10:30 AM with Dr. Michael Brown.',
    date: '2025-04-29',
    time: '09:00',
    read: false,
    relatedId: '4',
    relatedName: 'Dr. Michael Brown'
  },
  {
    id: '2',
    type: 'Consultation',
    priority: 'Important',
    title: 'Consultation Notes Available',
    message: 'Dr. Sarah Johnson has shared consultation notes from your recent visit.',
    date: '2025-04-28',
    time: '14:45',
    read: true,
    relatedId: '1',
    relatedName: 'Dr. Sarah Johnson'
  },
  {
    id: '3',
    type: 'Message',
    priority: 'Normal',
    title: 'New Message from Dr. Emily Harris',
    message: 'Dr. Emily Harris has sent you a message regarding your treatment plan.',
    date: '2025-04-28',
    time: '11:20',
    read: false,
    relatedId: '3',
    relatedName: 'Dr. Emily Harris'
  },
  {
    id: '4',
    type: 'System',
    priority: 'Urgent',
    title: 'System Maintenance',
    message: 'The system will be undergoing maintenance on April 30th from 2:00 AM to 4:00 AM. Some features may be temporarily unavailable.',
    date: '2025-04-27',
    time: '10:00',
    read: true
  },
  {
    id: '5',
    type: 'Appointment',
    priority: 'Important',
    title: 'Appointment Rescheduled',
    message: 'Your appointment with Dr. James Wilson has been rescheduled to May 5th at 11:00 AM.',
    date: '2025-04-26',
    time: '16:30',
    read: false,
    relatedId: '2',
    relatedName: 'Dr. James Wilson'
  }
];

function NotificationsPage() {
  const { user } = useAuth();
  const [notifications, setNotifications] = useState(mockNotifications);
  const [filter, setFilter] = useState<NotificationType | 'All'>('All');
  const [selectedNotification, setSelectedNotification] = useState<Notification | null>(null);
  
  // Filter notifications based on user role and filter selection
  const getFilteredNotifications = () => {
    if (!user) return [];
    
    // In a real app, notifications would be filtered based on the user's ID or role
    // For this demo, we'll just use the mock data for all users
    
    // Apply type filter
    if (filter === 'All') {
      return notifications;
    } else {
      return notifications.filter(notification => notification.type === filter);
    }
  };

  const filteredNotifications = getFilteredNotifications();
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
  };

  const deleteNotification = (id: string) => {
    setNotifications(prevNotifications => 
      prevNotifications.filter(notification => notification.id !== id)
    );
    toast.success("Notification deleted");
  };

  const handleNotificationClick = (notification: Notification) => {
    setSelectedNotification(notification);
  };

  const getNotificationTypeIcon = (type: NotificationType) => {
    switch (type) {
      case 'Appointment':
        return <Calendar className="h-4 w-4" />;
      case 'Consultation':
        return <FileText className="h-4 w-4" />;
      case 'Message':
        return <MessageCircle className="h-4 w-4" />;
      case 'System':
        return <Settings className="h-4 w-4" />;
    }
  };

  const getNotificationPriorityClass = (priority: NotificationPriority) => {
    switch (priority) {
      case 'Normal':
        return 'text-blue-600 bg-blue-50 border-blue-200';
      case 'Important':
        return 'text-amber-600 bg-amber-50 border-amber-200';
      case 'Urgent':
        return 'text-red-600 bg-red-50 border-red-200';
    }
  };

  return (
    <div className="space-y-6 pb-8">
      <div className="flex flex-col gap-2">
        <h1 className="text-3xl font-bold tracking-tight">Notifications</h1>
        <p className="text-muted-foreground">
          Stay updated with important information and alerts
        </p>
      </div>

      <div className="flex items-center justify-between mb-4">
        <div className="flex items-center gap-2 flex-wrap">
          <Button 
            variant={filter === 'All' ? 'default' : 'outline'} 
            size="sm"
            onClick={() => setFilter('All')}
          >
            All
          </Button>
          <Button 
            variant={filter === 'Appointment' ? 'default' : 'outline'} 
            size="sm"
            onClick={() => setFilter('Appointment')}
          >
            <Calendar className="mr-1 h-4 w-4" /> 
            Appointments
          </Button>
          <Button 
            variant={filter === 'Consultation' ? 'default' : 'outline'} 
            size="sm"
            onClick={() => setFilter('Consultation')}
          >
            <FileText className="mr-1 h-4 w-4" /> 
            Consultations
          </Button>
          <Button 
            variant={filter === 'Message' ? 'default' : 'outline'} 
            size="sm"
            onClick={() => setFilter('Message')}
          >
            <MessageCircle className="mr-1 h-4 w-4" /> 
            Messages
          </Button>
        </div>
        
        {unreadCount > 0 && (
          <Button 
            variant="outline" 
            size="sm"
            onClick={markAllAsRead}
          >
            <CheckCircle className="mr-1 h-4 w-4" /> 
            Mark all as read
          </Button>
        )}
      </div>

      <Card>
        <CardHeader>
          <div className="flex items-center justify-between">
            <div>
              <CardTitle>Your Notifications</CardTitle>
              <CardDescription>
                {unreadCount === 0 
                  ? 'All caught up!' 
                  : `You have ${unreadCount} unread notification${unreadCount !== 1 ? 's' : ''}`}
              </CardDescription>
            </div>
            <Bell className={cn(
              "h-6 w-6", 
              unreadCount > 0 ? "text-clinic-500" : "text-muted-foreground"
            )} />
          </div>
        </CardHeader>
        <CardContent className="space-y-4">
          {filteredNotifications.length === 0 ? (
            <div className="text-center py-8 text-muted-foreground">
              No notifications found
            </div>
          ) : (
            filteredNotifications.map((notification) => (
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
                  <div className={cn(
                    "p-2 rounded-full",
                    notification.read ? "bg-muted" : "bg-clinic-100"
                  )}>
                    {getNotificationTypeIcon(notification.type)}
                  </div>
                  <div className="flex-1">
                    <div className="flex items-center justify-between">
                      <h4 className="font-medium">{notification.title}</h4>
                      <Badge variant="outline" className={getNotificationPriorityClass(notification.priority)}>
                        {notification.priority}
                      </Badge>
                    </div>
                    <p className="text-muted-foreground mt-1">{notification.message}</p>
                    
                    {notification.relatedName && (
                      <div className="text-sm mt-2 text-muted-foreground">
                        <User className="inline h-3.5 w-3.5 mr-1" />
                        {notification.relatedName}
                      </div>
                    )}
                    
                    <div className="flex items-center justify-between mt-2">
                      <div className="text-sm text-muted-foreground flex items-center">
                        <Clock className="inline h-3.5 w-3.5 mr-1" />
                        {notification.date} at {notification.time}
                      </div>
                      <div className="flex gap-2">
                        {!notification.read && (
                          <Button 
                            size="sm" 
                            variant="ghost"
                            onClick={(e) => { e.stopPropagation(); markAsRead(notification.id); }}
                          >
                            Mark as read
                          </Button>
                        )}
                        <Button 
                          variant="ghost" 
                          size="icon"
                          onClick={(e) => { e.stopPropagation(); deleteNotification(notification.id); }}
                        >
                          <Trash2 className="h-4 w-4 text-muted-foreground" />
                        </Button>
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
        </CardContent>
      </Card>

      {/* Notification Popup */}
      <NotificationPopup 
        notification={selectedNotification}
        onClose={() => setSelectedNotification(null)}
        onMarkAsRead={markAsRead}
      />
    </div>
  );
}

export default NotificationsPage;
