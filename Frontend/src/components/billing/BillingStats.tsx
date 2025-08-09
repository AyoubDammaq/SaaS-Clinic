import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import {
  TrendingUp,
  Clock,
  AlertTriangle,
  CheckCircle,
} from 'lucide-react';
import { Facture, FactureStatus } from '@/types/billing';
import { useTranslation } from "@/hooks/useTranslation";

interface BillingStatsProps {
  invoices: Facture[];
  isLoading?: boolean;
}

export function BillingStats({ invoices, isLoading = false }: BillingStatsProps) {
  const { t } = useTranslation('billing');

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

  // Exclure les factures annulées des calculs principaux
  const activeInvoices = invoices.filter(inv => inv.status !== FactureStatus.ANNULEE);

  // Calcul des statistiques alignées avec StatistiquesFacturesDto
  const nombreTotal = activeInvoices.length;
  const nombrePayees = activeInvoices.filter(inv => inv.status === FactureStatus.PAYEE).length;
  const nombreImpayees = activeInvoices.filter(inv => inv.status === FactureStatus.IMPAYEE).length;
  const nombrePartiellementPayees = activeInvoices.filter(inv => inv.status === FactureStatus.PARTIELLEMENT_PAYEE).length;

  // Montant total et montant payé
  const montantTotal = activeInvoices.reduce((sum, inv) => sum + inv.montantTotal, 0);
  const montantTotalPaye = activeInvoices.reduce((sum, inv) => sum + inv.montantPaye, 0);

  // Montant en attente : somme des (montantTotal - montantPaye) pour IMPAYEE et PARTIELLEMENT_PAYEE
  const pendingAmount = activeInvoices
    .filter(inv => inv.status === FactureStatus.IMPAYEE || inv.status === FactureStatus.PARTIELLEMENT_PAYEE)
    .reduce((sum, inv) => sum + (inv.montantTotal - inv.montantPaye), 0);

  // Montant en retard : somme des (montantTotal - montantPaye) pour factures en retard
  // Supposer que les factures sont dues 30 jours après dateEmission
  const today = new Date();
  const overdueInvoices = activeInvoices.filter(
    inv =>
      (inv.status === FactureStatus.IMPAYEE || inv.status === FactureStatus.PARTIELLEMENT_PAYEE) &&
      new Date(new Date(inv.dateEmission).setDate(new Date(inv.dateEmission).getDate() + 30)) < today
  ).length;

  const overdueAmount = activeInvoices
    .filter(
      inv =>
        (inv.status === FactureStatus.IMPAYEE || inv.status === FactureStatus.PARTIELLEMENT_PAYEE) &&
        new Date(new Date(inv.dateEmission).setDate(new Date(inv.dateEmission).getDate() + 30)) < today
    )
    .reduce((sum, inv) => sum + (inv.montantTotal - inv.montantPaye), 0);

  // Taux de paiement : pourcentage du montantTotalPaye par rapport au montantTotal
  const paymentRate = montantTotal > 0 ? Math.round((montantTotalPaye / montantTotal) * 100) : 0;

  const formatCurrency = (amount: number, currency: string = 'EUR') =>
    new Intl.NumberFormat('fr-FR', {
      style: 'currency',
      currency,
    }).format(amount);

  const statCards = [
    {
      title: t('revenue'),
      value: formatCurrency(montantTotalPaye),
      subtitle: `${nombrePayees + nombrePartiellementPayees} ${t('paidPartiallyInvoices')}`,
      icon: <TrendingUp className="h-4 w-4" aria-label={t('revenue')} />,
      variant: 'default' as const,
      trend: paymentRate > 75 ? 'positive' : paymentRate > 50 ? 'neutral' : 'negative',
    },
    {
      title: t('pending'),
      value: formatCurrency(pendingAmount),
      subtitle: `${nombreImpayees + nombrePartiellementPayees} ${t('invoices')}`,
      icon: <Clock className="h-4 w-4" aria-label={t('pending')} />,
      variant: 'secondary' as const,
      trend: 'neutral',
    },
    {
      title: t('overdue'),
      value: formatCurrency(overdueAmount),
      subtitle: `${overdueInvoices} ${t('invoices')}`,
      icon: <AlertTriangle className="h-4 w-4" aria-label={t('overdue')} />,
      variant: 'destructive' as const,
      trend: overdueInvoices > 0 ? 'negative' : 'neutral',
    },
    {
      title: t('paymentRate'),
      value: `${paymentRate}%`,
      subtitle: `${nombreTotal} ${t('totalInvoices')}`,
      icon: <CheckCircle className="h-4 w-4" aria-label={t('paymentRate')} />,
      variant: paymentRate > 75 ? 'default' : 'secondary' as const,
      trend: paymentRate > 75 ? 'positive' : paymentRate > 50 ? 'neutral' : 'negative',
    },
  ];

  return (
    <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
      {statCards.map((stat, index) => {
        const cardClasses = {
          destructive: 'border-destructive/20 bg-destructive/5',
          secondary: 'border-muted bg-muted/20',
          default: 'border-primary/20 bg-primary/5',
        };

        const iconBgClasses = {
          destructive: 'bg-destructive/10 text-destructive',
          secondary: 'bg-muted text-muted-foreground',
          default: 'bg-primary/10 text-primary',
        };

        return (
          <Card key={index} className={`card-hover ${cardClasses[stat.variant]}`}>
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
                {stat.trend === 'positive' && (
                  <Badge
                    variant="secondary"
                    className="ml-1 bg-green-100 text-green-700 border-green-200"
                  >
                    {t('good')}
                  </Badge>
                )}
                {stat.trend === 'negative' &&
                  stat.title === t('overdue') &&
                  overdueInvoices > 0 && (
                    <Badge variant="destructive" className="ml-1">
                      {t('attention')}
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