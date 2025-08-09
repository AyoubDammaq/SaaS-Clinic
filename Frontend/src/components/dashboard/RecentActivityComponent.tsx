import {
  Activity,
  Calendar,
  Clock,
  CreditCard,
  FileText,
  User,
} from "lucide-react";
import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
} from "@/components/ui/card";
import { cn } from "@/lib/utils";
import { useTranslation } from "@/hooks/useTranslation";

type ActivityType = "appointment" | "consultation" | "payment" | "registration";

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

export function RecentActivityComponent({ activities }: RecentActivityProps) {
  const { t } = useTranslation("dashboard");

  return (
    <Card className="overflow-hidden">
      <CardHeader>
        <CardTitle>{t("recent_activity_title") || "Recent Activity"}</CardTitle>
        <CardDescription>
          {t("recent_activity_description") || "Overview of your recent activities"}
        </CardDescription>
      </CardHeader>
      <CardContent className="p-4 space-y-3">
        {activities.map((activity) => (
          <div
            key={activity.id}
            className={cn(
              "flex items-start gap-3 rounded-lg border p-3 transition-all duration-200 hover:bg-muted/20",
              !activity.seen && "bg-muted/40"
            )}
          >
            <div className="rounded-full bg-muted p-2 text-muted-foreground">
              {getActivityIcon(activity.type)}
            </div>
            <div className="space-y-1 flex-1">
              <p className="font-medium text-sm">{activity.title}</p>
              <p className="text-xs text-muted-foreground line-clamp-2">
                {activity.description}
              </p>
              <div className="flex items-center gap-1 text-xs text-muted-foreground">
                <Clock className="h-3 w-3" />
                <span>{activity.time}</span>
              </div>
            </div>
          </div>
        ))}
      </CardContent>
    </Card>
  );
}

function getActivityIcon(type: ActivityType) {
  switch (type) {
    case "appointment":
      return <Calendar className="h-4 w-4" />;
    case "consultation":
      return <FileText className="h-4 w-4" />;
    case "payment":
      return <CreditCard className="h-4 w-4" />;
    case "registration":
      return <User className="h-4 w-4" />;
    default:
      return <Activity className="h-4 w-4" />;
  }
}
