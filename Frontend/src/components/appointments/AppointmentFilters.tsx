
import { useState } from "react";
import { Search, CalendarRange, Filter, X, CalendarPlus2 } from "lucide-react";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Popover, PopoverTrigger, PopoverContent } from "@/components/ui/popover";
import { Calendar } from "@/components/ui/calendar";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { format } from "date-fns";
import { cn } from "@/lib/utils";
import { AppointmentStatus } from "./AppointmentStatusBadge";
import { Badge } from "@/components/ui/badge";
import { useTranslation } from "@/hooks/useTranslation";

interface AppointmentFiltersProps {
  searchTerm: string;
  onSearchChange: (value: string) => void;
  dateFilter: Date | null;
  onDateFilterChange: (date: Date | null) => void;
  statusFilter: AppointmentStatus | "all";
  onStatusFilterChange: (status: AppointmentStatus | "all") => void;
  onCreateAppointment?: () => void;
  userRole?: string;
}

export const AppointmentFilters = ({
  searchTerm,
  onSearchChange,
  dateFilter,
  onDateFilterChange,
  statusFilter,
  onStatusFilterChange,
  onCreateAppointment,
  userRole
}: AppointmentFiltersProps) => {
  const { t } = useTranslation('appointments');
  const tCommon = useTranslation('common').t;
  const [filtersExpanded, setFiltersExpanded] = useState(false);
  
  const hasActiveFilters = dateFilter || statusFilter !== "all";
  
  const clearFilters = () => {
    onDateFilterChange(null);
    onStatusFilterChange("all");
  };

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap gap-3 items-center">
        <div className="relative flex-grow max-w-sm">
          <Search className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input
            placeholder={t('searchAppointments')}
            value={searchTerm}
            onChange={(e) => onSearchChange(e.target.value)}
            className="pl-8"
          />
        </div>
        
        <div className="flex gap-2">
          <Button 
            variant="outline" 
            size="icon"
            onClick={() => setFiltersExpanded(!filtersExpanded)}
            className={cn(hasActiveFilters && "border-primary text-primary")}
          >
            <Filter className="h-4 w-4" />
          </Button>
          
          <Popover>
            <PopoverTrigger asChild>
              <Button 
                variant="outline" 
                size="icon"
                className={cn(dateFilter && "border-primary text-primary")}
              >
                <CalendarRange className="h-4 w-4" />
              </Button>
            </PopoverTrigger>
            <PopoverContent className="w-auto p-0" align="end">
              <Calendar
                mode="single"
                selected={dateFilter || undefined}
                onSelect={onDateFilterChange}
                initialFocus
                className={cn("p-3 pointer-events-auto")}
              />
              {dateFilter && (
                <div className="border-t p-3 flex justify-between">
                  <div className="text-sm font-medium">
                    {format(dateFilter, "PPP")}
                  </div>
                  <Button 
                    variant="ghost" 
                    size="sm"
                    onClick={() => onDateFilterChange(null)}
                  >
                    <X className="h-4 w-4 mr-1" /> {tCommon('clear')}
                  </Button>
                </div>
              )}
            </PopoverContent>
          </Popover>
          
          {(userRole === 'Patient' || userRole === 'ClinicAdmin') && (
            <Button onClick={onCreateAppointment}>
              <CalendarPlus2 className="mr-1 h-4 w-4" /> 
              {userRole === 'Patient' ? t('bookAppointment') : t('createAppointment')}
            </Button>
          )}
        </div>
      </div>
      
      {filtersExpanded && (
        <div className="bg-accent/30 p-3 rounded-md flex flex-wrap gap-4 items-end">
          <div className="space-y-2 min-w-[200px]">
            <Label htmlFor="status-filter">{tCommon('status')}</Label>
            <Select
              value={statusFilter}
              onValueChange={(value) => 
                onStatusFilterChange(value as AppointmentStatus | "all")
              }
            >
              <SelectTrigger id="status-filter">
                <SelectValue placeholder={t('allStatuses')} />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">{t('allStatuses')}</SelectItem>
                <SelectItem value="Scheduled">{t('scheduled')}</SelectItem>
                <SelectItem value="Completed">{t('completed')}</SelectItem>
                <SelectItem value="Cancelled">{t('cancelled')}</SelectItem>
                <SelectItem value="No-show">{t('noShow')}</SelectItem>
              </SelectContent>
            </Select>
          </div>
          
          {hasActiveFilters && (
            <Button variant="ghost" size="sm" onClick={clearFilters} className="ml-auto">
              <X className="h-4 w-4 mr-1" /> {tCommon('clearFilters')}
            </Button>
          )}
        </div>
      )}
      
      {hasActiveFilters && !filtersExpanded && (
        <div className="flex flex-wrap items-center gap-2">
          <span className="text-sm text-muted-foreground">{t('activeFilters')}:</span>
          {dateFilter && (
            <Badge variant="secondary" className="flex gap-1 items-center">
              <CalendarRange className="h-3 w-3" /> 
              {format(dateFilter, "PP")}
              <X 
                className="h-3 w-3 ml-1 cursor-pointer" 
                onClick={() => onDateFilterChange(null)} 
              />
            </Badge>
          )}
          {statusFilter !== "all" && (
            <Badge variant="secondary" className="flex gap-1 items-center">
              {statusFilter}
              <X 
                className="h-3 w-3 ml-1 cursor-pointer" 
                onClick={() => onStatusFilterChange("all")} 
              />
            </Badge>
          )}
          <Button variant="ghost" size="sm" onClick={clearFilters} className="h-7 px-2">
            {tCommon('clearAll')}
          </Button>
        </div>
      )}
    </div>
  );
};
