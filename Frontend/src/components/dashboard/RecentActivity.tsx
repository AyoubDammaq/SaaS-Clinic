
import { Activity, Calendar, Clock, CreditCard, FileText, User } from "lucide-react";
import { cn } from "@/lib/utils";

type ActivityType = 'appointment' | 'consultation' | 'payment' | 'registration';

interface ActivityItem {
  id: string;
  type: ActivityType;
  title: string;
  description: string;
  time: string;
  seen: boolean;
}

interface RecentActivityProps {
  activities: ActivityItem[];
}

export function RecentActivity({ activities }: RecentActivityProps) {
  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h3 className="text-lg font-medium">Recent Activity</h3>
      </div>
      
      <div className="space-y-4">
        {activities.map((activity) => (
          <div 
            key={activity.id}
            className={cn(
              "flex items-start gap-3 rounded-lg border p-3",
              !activity.seen && "bg-muted/40"
            )}
          >
            <div className="rounded-full bg-muted p-1.5 text-muted-foreground">
              {getActivityIcon(activity.type)}
            </div>
            
            <div className="space-y-1 flex-1">
              <p className="font-medium">{activity.title}</p>
              <p className="text-sm text-muted-foreground">{activity.description}</p>
              <div className="flex items-center gap-1 text-xs text-muted-foreground">
                <Clock className="h-3 w-3" />
                <span>{activity.time}</span>
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}

function getActivityIcon(type: ActivityType) {
  switch (type) {
    case 'appointment':
      return <Calendar className="h-4 w-4" />;
    case 'consultation':
      return <FileText className="h-4 w-4" />;
    case 'payment':
      return <CreditCard className="h-4 w-4" />;
    case 'registration':
      return <User className="h-4 w-4" />;
    default:
      return <Activity className="h-4 w-4" />;
  }
}
