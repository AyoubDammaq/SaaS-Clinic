import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { TrendingUp, Clock, AlertTriangle, CheckCircle } from "lucide-react";
import { useTranslation } from "@/hooks/useTranslation";
import { BillingStatsDto } from "@/types/billing";
import { LoadingSpinner } from "@/components/ui/loading-spinner";

interface BillingStatsProps {
  stats: BillingStatsDto | null;
  isLoading: boolean;
  error: string | null;
}

export function BillingStats({ stats, isLoading, error }: BillingStatsProps) {
  const { t } = useTranslation("billing");

  if (isLoading) {
    return (
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        {Array.from({ length: 4 }).map((_, i) => (
          <Card key={i} className="animate-pulse">
            <CardHeader className="space-y-0 pb-2">
              <div className="h-4 bg-muted rounded w-20" />
              <div className="h-6 bg-muted rounded w-16" />
            </CardHeader>
            <CardContent>
              <div className="h-4 bg-muted rounded w-24" />
            </CardContent>
          </Card>
        ))}
      </div>
    );
  }

  if (error || !stats) {
    return (
      <Card className="border-destructive/20 bg-destructive/5">
        <CardContent className="pt-6">
          <p className="text-destructive text-center">
            {error || t("failedToLoadStats")}
          </p>
        </CardContent>
      </Card>
    );
  }

  const formatCurrency = (amount: number, currency: string = "EUR") =>
    new Intl.NumberFormat("fr-FR", {
      style: "currency",
      currency,
    }).format(amount);

  const statCards = [
    {
      title: t("revenue"),
      value: formatCurrency(stats.revenue),
      subtitle: t("totalReceived"),
      icon: <TrendingUp className="h-4 w-4" aria-label={t("revenue")} />,
      variant: "default" as const,
      trend:
        stats.paymentRate > 75
          ? "positive"
          : stats.paymentRate > 50
          ? "neutral"
          : "negative",
    },
    {
      title: t("pending"),
      value: formatCurrency(stats.pendingAmount),
      subtitle: t("awaitingPayment"),
      icon: <Clock className="h-4 w-4" aria-label={t("pending")} />,
      variant: "secondary" as const,
      trend: "neutral",
    },
    {
      title: t("overdue"),
      value: formatCurrency(stats.overdueAmount),
      subtitle: t("pastDue"),
      icon: <AlertTriangle className="h-4 w-4" aria-label={t("overdue")} />,
      variant: "destructive" as const,
      trend: stats.overdueAmount > 0 ? "negative" : "neutral",
    },
    {
      title: t("paymentRate"),
      value: `${stats.paymentRate}%`,
      subtitle: t("paymentCompletion"),
      icon: <CheckCircle className="h-4 w-4" aria-label={t("paymentRate")} />,
      variant: stats.paymentRate > 75 ? "default" : ("secondary" as const),
      trend:
        stats.paymentRate > 75
          ? "positive"
          : stats.paymentRate > 50
          ? "neutral"
          : "negative",
    },
  ];

  return (
    <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
      {statCards.map((stat, index) => {
        const cardClasses = {
          destructive: "border-destructive/20 bg-destructive/5",
          secondary: "border-muted bg-muted/20",
          default: "border-primary/20 bg-primary/5",
        };

        const iconBgClasses = {
          destructive: "bg-destructive/10 text-destructive",
          secondary: "bg-muted text-muted-foreground",
          default: "bg-primary/10 text-primary",
        };

        return (
          <Card
            key={index}
            className={`card-hover ${cardClasses[stat.variant]}`}
          >
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium text-muted-foreground">
                {stat.title}
              </CardTitle>
              <div className={`p-2 rounded-md ${iconBgClasses[stat.variant]}`}>
                {stat.icon}
              </div>
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold mb-1">{stat.value}</div>
              <p className="text-xs text-muted-foreground flex items-center gap-1">
                {stat.subtitle}
                {stat.trend === "positive" && (
                  <Badge
                    variant="secondary"
                    className="ml-1 bg-green-100 text-green-700 border-green-200"
                  >
                    {t("good")}
                  </Badge>
                )}
                {stat.trend === "negative" &&
                  stat.title === t("overdue") &&
                  stats.overdueAmount > 0 && (
                    <Badge variant="destructive" className="ml-1">
                      {t("attention")}
                    </Badge>
                  )}
              </p>
            </CardContent>
          </Card>
        );
      })}
    </div>
  );
}
