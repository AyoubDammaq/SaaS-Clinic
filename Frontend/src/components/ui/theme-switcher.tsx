
import { useTheme } from "@/components/theme-provider";
import { useTranslation } from "@/hooks/useTranslation";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
  DropdownMenuLabel,
  DropdownMenuSeparator
} from "@/components/ui/dropdown-menu";
import { Moon, Sun, Laptop } from "lucide-react";
import { cn } from "@/lib/utils";
import type { Theme } from "@/components/theme-provider"; // Now properly imported

export function ThemeSwitcher() {
  const { theme, setTheme } = useTheme();
  const { t } = useTranslation();
  
  const themes: { value: Theme; label: string; icon: React.ElementType }[] = [
    { 
      value: 'light', 
      label: t('lightMode'), 
      icon: Sun 
    },
    { 
      value: 'dark', 
      label: t('darkMode'), 
      icon: Moon 
    },
    { 
      value: 'system', 
      label: t('system'), // Use the existing translation key
      icon: Laptop 
    }
  ];
  
  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <Button variant="ghost" size="icon" className="relative">
          <Sun className="h-5 w-5 rotate-0 scale-100 transition-all dark:-rotate-90 dark:scale-0" />
          <Moon className="absolute h-5 w-5 rotate-90 scale-0 transition-all dark:rotate-0 dark:scale-100" />
          <span className="sr-only">{t('darkMode')}</span>
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end" className="w-40">
        <DropdownMenuLabel>{t('theme')}</DropdownMenuLabel>
        <DropdownMenuSeparator />
        {themes.map((item) => (
          <DropdownMenuItem 
            key={item.value} 
            onClick={() => setTheme(item.value)}
            className={cn(
              "cursor-pointer flex items-center gap-2",
              theme === item.value && "bg-accent"
            )}
          >
            <item.icon className="h-4 w-4" />
            {item.label}
            {theme === item.value && (
              <span className="ml-auto">✓</span>
            )}
          </DropdownMenuItem>
        ))}
      </DropdownMenuContent>
    </DropdownMenu>
  );
}
