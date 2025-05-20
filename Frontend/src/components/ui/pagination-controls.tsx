
import { ChevronLeft, ChevronRight } from "lucide-react";
import { Button } from "./button";

interface PaginationControlsProps {
  currentPage: number;
  totalPages: number;
  onPageChange: (page: number) => void;
  className?: string;
}

export function PaginationControls({
  currentPage,
  totalPages,
  onPageChange,
  className
}: PaginationControlsProps) {
  // Create page numbers array with ellipsis for large ranges
  const getPageNumbers = () => {
    const pages = [];
    const maxVisiblePages = 5;
    
    if (totalPages <= maxVisiblePages) {
      // Show all page numbers
      for (let i = 1; i <= totalPages; i++) {
        pages.push(i);
      }
    } else {
      // Always include first page
      pages.push(1);
      
      // Calculate middle section
      const leftBoundary = Math.max(2, currentPage - 1);
      const rightBoundary = Math.min(totalPages - 1, currentPage + 1);
      
      // Add ellipsis before middle section if needed
      if (leftBoundary > 2) {
        pages.push(-1); // -1 represents ellipsis
      }
      
      // Add middle section page numbers
      for (let i = leftBoundary; i <= rightBoundary; i++) {
        pages.push(i);
      }
      
      // Add ellipsis after middle section if needed
      if (rightBoundary < totalPages - 1) {
        pages.push(-2); // -2 represents another ellipsis marker
      }
      
      // Always include last page
      pages.push(totalPages);
    }
    
    return pages;
  };

  return (
    <div className={`flex items-center justify-center space-x-2 ${className}`} role="navigation" aria-label="Pagination">
      <Button
        variant="outline"
        size="icon"
        onClick={() => onPageChange(currentPage - 1)}
        disabled={currentPage <= 1}
        aria-label="Previous page"
      >
        <ChevronLeft className="h-4 w-4" />
      </Button>
      
      <div className="flex items-center space-x-1">
        {getPageNumbers().map((page, index) => 
          page < 0 ? (
            // Render ellipsis
            <span key={`ellipsis-${index}`} className="px-2 text-muted-foreground">
              …
            </span>
          ) : (
            // Render page number button
            <Button
              key={page}
              variant={currentPage === page ? "default" : "outline"}
              size="sm"
              onClick={() => onPageChange(page)}
              aria-label={`Page ${page}`}
              aria-current={currentPage === page ? "page" : undefined}
              className="w-8 h-8 p-0"
            >
              {page}
            </Button>
          )
        )}
      </div>
      
      <Button
        variant="outline"
        size="icon"
        onClick={() => onPageChange(currentPage + 1)}
        disabled={currentPage >= totalPages}
        aria-label="Next page"
      >
        <ChevronRight className="h-4 w-4" />
      </Button>
    </div>
  );
}
