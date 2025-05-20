
import { Loader2 } from "lucide-react";
import { cn } from "@/lib/utils";

interface LoadingSpinnerProps {
  size?: 'sm' | 'md' | 'lg';
  className?: string;
  fullPage?: boolean;
  text?: string;
}

export function LoadingSpinner({ 
  size = 'md', 
  className, 
  fullPage = false,
  text
}: LoadingSpinnerProps) {
  const sizeMap = {
    sm: 'h-4 w-4',
    md: 'h-6 w-6',
    lg: 'h-10 w-10'
  };
  
  const spinner = (
    <div className={cn(
      "flex items-center justify-center",
      fullPage ? "h-full w-full min-h-[200px] flex-col" : "",
      className
    )}>
      <Loader2 
        className={cn(
          "animate-spin text-primary",
          sizeMap[size]
        )}
        aria-hidden="true"
      />
      {text && (
        <p className={cn(
          "text-muted-foreground mt-2", 
          fullPage ? "text-sm" : "text-xs"
        )}>
          {text}
        </p>
      )}
      <span className="sr-only">Loading</span>
    </div>
  );
  
  if (fullPage) {
    return (
      <div className="flex items-center justify-center w-full h-full min-h-[200px]">
        {spinner}
      </div>
    );
  }
  
  return spinner;
}
