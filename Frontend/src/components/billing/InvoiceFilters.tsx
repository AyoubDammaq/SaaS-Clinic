import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Card, CardContent } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Calendar } from "@/components/ui/calendar";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";
import { Input } from "@/components/ui/input";
import {
  Filter,
  X,
  Calendar as CalendarIcon,
  Euro,
  FileText,
} from "lucide-react";
import { format } from "date-fns";
import { cn } from "@/lib/utils";
import { FactureStatus } from "@/types/billing";
import { useTranslation } from "@/hooks/useTranslation";

export interface InvoiceFilterState {
  status: string;
  dateRange: {
    from: Date | undefined;
    to: Date | undefined;
  };
  amountRange: {
    min: string;
    max: string;
  };
  searchTerm: string;
}

interface InvoiceFiltersProps {
  filters: InvoiceFilterState;
  onFiltersChange: (filters: InvoiceFilterState) => void;
  isExpanded: boolean;
  onToggleExpanded: () => void;
}

export function InvoiceFilters({
  filters,
  onFiltersChange,
  isExpanded,
  onToggleExpanded,
}: InvoiceFiltersProps) {
  const { t } = useTranslation('billing');
  const tCommon = useTranslation('common').t;
  const [dateFrom, setDateFrom] = useState<Date | undefined>(filters.dateRange.from);
  const [dateTo, setDateTo] = useState<Date | undefined>(filters.dateRange.to);

  const handleStatusChange = (value: string) => {
    onFiltersChange({
      ...filters,
      status: value === "all" ? "" : value,
    });
  };

  const handleDateRangeChange = (from: Date | undefined, to: Date | undefined) => {
    setDateFrom(from);
    setDateTo(to);
    onFiltersChange({
      ...filters,
      dateRange: { from, to },
    });
  };

  const handleAmountChange = (field: "min" | "max", value: string) => {
    onFiltersChange({
      ...filters,
      amountRange: {
        ...filters.amountRange,
        [field]: value,
      },
    });
  };

  const handleSearchChange = (value: string) => {
    onFiltersChange({
      ...filters,
      searchTerm: value,
    });
  };

  const clearFilters = () => {
    onFiltersChange({
      status: "",
      dateRange: { from: undefined, to: undefined },
      amountRange: { min: "", max: "" },
      searchTerm: "",
    });
    setDateFrom(undefined);
    setDateTo(undefined);
  };

  const getActiveFiltersCount = () => {
    let count = 0;
    if (filters.status) count++;
    if (filters.dateRange.from || filters.dateRange.to) count++;
    if (filters.amountRange.min || filters.amountRange.max) count++;
    if (filters.searchTerm) count++;
    return count;
  };

  const activeFiltersCount = getActiveFiltersCount();

  // Fonction pour traduire les statuts en français pour l'affichage
  const getStatusDisplay = (status: string) => {
    switch (status) {
      case FactureStatus.PAYEE:
        return t('paid');
      case FactureStatus.PARTIELLEMENT_PAYEE:
        return t('pending');
      case FactureStatus.IMPAYEE:
        return t('overdue');
      case FactureStatus.ANNULEE:
        return t('cancelled');
      default:
        return t('allStatuses');
    }
  };

  return (
    <div className="space-y-4">
      {/* Header avec bouton de toggle */}
      <div className="flex items-center justify-between">
        <Button
          variant="outline"
          onClick={onToggleExpanded}
          className="flex items-center gap-2"
        >
          <Filter className="h-4 w-4" />
          {tCommon('filters')}
          {activeFiltersCount > 0 && (
            <Badge variant="secondary" className="ml-2">
              {activeFiltersCount}
            </Badge>
          )}
        </Button>

        {activeFiltersCount > 0 && (
          <Button
            variant="ghost"
            size="sm"
            onClick={clearFilters}
            className="text-muted-foreground hover:text-foreground"
          >
            <X className="h-4 w-4 mr-1" />
            {tCommon('clear')}
          </Button>
        )}
      </div>

      {/* Filtres actifs sous forme de badges */}
      {activeFiltersCount > 0 && (
        <div className="flex flex-wrap gap-2">
          {filters.status && (
            <Badge variant="secondary" className="flex items-center gap-1">
              <FileText className="h-3 w-3" />
              {t('status')}: {getStatusDisplay(filters.status)}
              <button
                onClick={() => handleStatusChange("all")}
                className="ml-1 hover:bg-muted rounded-full p-0.5"
              >
                <X className="h-3 w-3" />
              </button>
            </Badge>
          )}

          {(filters.dateRange.from || filters.dateRange.to) && (
            <Badge variant="secondary" className="flex items-center gap-1">
              <CalendarIcon className="h-3 w-3" />
              {t('date')}:{" "}
              {filters.dateRange.from
                ? format(filters.dateRange.from, "dd/MM/yy")
                : "∞"}{" "}
              -{" "}
              {filters.dateRange.to
                ? format(filters.dateRange.to, "dd/MM/yy")
                : "∞"}
              <button
                onClick={() => handleDateRangeChange(undefined, undefined)}
                className="ml-1 hover:bg-muted rounded-full p-0.5"
              >
                <X className="h-3 w-3" />
              </button>
            </Badge>
          )}

          {(filters.amountRange.min || filters.amountRange.max) && (
            <Badge variant="secondary" className="flex items-center gap-1">
              <Euro className="h-3 w-3" />
              {t('amount')}: {filters.amountRange.min || "0"}€ -{" "}
              {filters.amountRange.max || "∞"}€
              <button
                onClick={() => {
                  handleAmountChange("min", "");
                  handleAmountChange("max", "");
                }}
                className="ml-1 hover:bg-muted rounded-full p-0.5"
              >
                <X className="h-3 w-3" />
              </button>
            </Badge>
          )}
        </div>
      )}

      {/* Panneau de filtres étendu */}
      {isExpanded && (
        <Card className="animate-fade-in">
          <CardContent className="pt-6">
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
              {/* Recherche */}
              <div className="space-y-2">
                <Label>{t('search')}</Label>
                <Input
                  placeholder={t('searchInvoices')}
                  value={filters.searchTerm}
                  onChange={(e) => handleSearchChange(e.target.value)}
                />
              </div>

              {/* Statut */}
              <div className="space-y-2">
                <Label>{t('status')}</Label>
                <Select
                  value={filters.status || "all"}
                  onValueChange={handleStatusChange}
                >
                  <SelectTrigger>
                    <SelectValue placeholder={t('allStatuses')} />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all">{t('allStatuses')}</SelectItem>
                    <SelectItem value={FactureStatus.PAYEE}>{t('paid')}</SelectItem>
                    <SelectItem value={FactureStatus.IMPAYEE}>{t('unpaid')}</SelectItem>
                    <SelectItem value={FactureStatus.ANNULEE}>{t('cancelled')}</SelectItem>
                  </SelectContent>
                </Select>
              </div>

              {/* Date de début */}
              <div className="space-y-2">
                <Label>{t('startDate')}</Label>
                <Popover>
                  <PopoverTrigger asChild>
                    <Button
                      variant="outline"
                      className={cn(
                        "w-full justify-start text-left font-normal",
                        !dateFrom && "text-muted-foreground"
                      )}
                    >
                      <CalendarIcon className="mr-2 h-4 w-4" />
                      {dateFrom ? format(dateFrom, "dd/MM/yyyy") : t('select')}
                    </Button>
                  </PopoverTrigger>
                  <PopoverContent className="w-auto p-0" align="start">
                    <Calendar
                      mode="single"
                      selected={dateFrom}
                      onSelect={(date) => handleDateRangeChange(date, dateTo)}
                      initialFocus
                    />
                  </PopoverContent>
                </Popover>
              </div>

              {/* Date de fin */}
              <div className="space-y-2">
                <Label>{t('endDate')}</Label>
                <Popover>
                  <PopoverTrigger asChild>
                    <Button
                      variant="outline"
                      className={cn(
                        "w-full justify-start text-left font-normal",
                        !dateTo && "text-muted-foreground"
                      )}
                    >
                      <CalendarIcon className="mr-2 h-4 w-4" />
                      {dateTo ? format(dateTo, "dd/MM/yyyy") : t('select')}
                    </Button>
                  </PopoverTrigger>
                  <PopoverContent className="w-auto p-0" align="start">
                    <Calendar
                      mode="single"
                      selected={dateTo}
                      onSelect={(date) => handleDateRangeChange(dateFrom, date)}
                      initialFocus
                    />
                  </PopoverContent>
                </Popover>
              </div>
            </div>

            {/* Plage de montants */}
            <div className="mt-4">
              <Label className="mb-2 block">{t('amountRange')}</Label>
              <div className="grid grid-cols-2 gap-2">
                <Input
                  type="number"
                  placeholder={t('minAmount')}
                  value={filters.amountRange.min}
                  onChange={(e) => handleAmountChange("min", e.target.value)}
                />
                <Input
                  type="number"
                  placeholder={t('maxAmount')}
                  value={filters.amountRange.max}
                  onChange={(e) => handleAmountChange("max", e.target.value)}
                />
              </div>
            </div>
          </CardContent>
        </Card>
      )}
    </div>
  );
}