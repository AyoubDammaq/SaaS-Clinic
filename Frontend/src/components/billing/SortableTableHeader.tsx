
import { TableHead } from "@/components/ui/table";
import { ArrowDown, ArrowUp } from "lucide-react";
import { cn } from "@/lib/utils";

interface SortableTableHeaderProps {
  field: string;
  label: string;
  sortField: string;
  sortDirection: 'asc' | 'desc';
  onSort: (field: string) => void;
  className?: string;
}

export function SortableTableHeader({
  field,
  label,
  sortField,
  sortDirection,
  onSort,
  className
}: SortableTableHeaderProps) {
  const isActive = sortField === field;
  
  return (
    <TableHead 
      className={cn("cursor-pointer select-none", className)}
      onClick={() => onSort(field)}
      role="columnheader"
      aria-sort={
        isActive 
          ? sortDirection === 'asc' ? 'ascending' : 'descending'
          : undefined
      }
    >
      <div className="flex items-center gap-1">
        <span>{label}</span>
        {isActive && (
          <span className="inline-flex">
            {sortDirection === 'asc' ? (
              <ArrowUp className="h-3 w-3" aria-hidden="true" />
            ) : (
              <ArrowDown className="h-3 w-3" aria-hidden="true" />
            )}
          </span>
        )}
      </div>
    </TableHead>
  );
}
