
import { useState, useEffect, ComponentType } from 'react';
import { LoadingSpinner } from "@/components/ui/loading-spinner";

interface UseLazyComponentOptions {
  delayMs?: number;
  minimumLoadTimeMs?: number;
}

export function useLazyComponent<T>(
  factory: () => Promise<{ default: ComponentType<T> }>,
  options: UseLazyComponentOptions = {}
) {
  const { delayMs = 0, minimumLoadTimeMs = 300 } = options;
  const [Component, setComponent] = useState<ComponentType<T> | null>(null);
  const [error, setError] = useState<Error | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    let isMounted = true;
    let startTime = Date.now();
    let timeout: number | undefined;
    
    // Add initial delay if specified
    if (delayMs > 0) {
      timeout = window.setTimeout(loadComponent, delayMs);
    } else {
      loadComponent();
    }
    
    function loadComponent() {
      factory()
        .then(module => {
          // Calculate how much time has elapsed
          const elapsedTime = Date.now() - startTime;
          const remainingTime = Math.max(0, minimumLoadTimeMs - elapsedTime);
          
          // Wait for minimum load time if needed
          if (remainingTime > 0) {
            setTimeout(() => {
              if (isMounted) {
                setComponent(() => module.default);
                setIsLoading(false);
              }
            }, remainingTime);
          } else {
            if (isMounted) {
              setComponent(() => module.default);
              setIsLoading(false);
            }
          }
        })
        .catch(err => {
          if (isMounted) {
            console.error('Failed to lazy load component:', err);
            setError(err);
            setIsLoading(false);
          }
        });
    }

    return () => {
      isMounted = false;
      if (timeout) clearTimeout(timeout);
    };
  }, [factory, delayMs, minimumLoadTimeMs]);

  if (error) {
    return {
      Component: (() => (
        <div className="p-4 text-center text-red-500 border rounded-md bg-red-50 dark:bg-red-900/10">
          Failed to load component
        </div>
      )) as unknown as ComponentType<T>,
      isLoading: false
    };
  }

  if (isLoading || !Component) {
    return {
      Component: (() => (
        <div className="flex items-center justify-center py-8">
          <LoadingSpinner size="md" text="Loading component..." />
        </div>
      )) as unknown as ComponentType<T>,
      isLoading: true
    };
  }

  return { Component, isLoading: false };
}

export default useLazyComponent;
