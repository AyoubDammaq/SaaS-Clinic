import { Input } from "@/components/ui/input";
import { Search } from "lucide-react";
import { useTranslation } from "@/hooks/useTranslation";

interface SearchBarProps {
  value: string;
  onChange: (value: string) => void;
  placeholder?: string;
  className?: string;
}

export function SearchBar({
  value,
  onChange,
  placeholder,
  className,
}: SearchBarProps) {
  const { t } = useTranslation();

  return (
    <div className={`relative ${className || ""}`}>
      <Search
        className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground"
        aria-hidden="true"
      />
      <Input
        value={value}
        onChange={(e) => onChange(e.target.value)}
        placeholder={placeholder || t("search")}
        className="pl-8 w-full sm:w-[250px]"
        aria-label={placeholder || t("search")}
      />
    </div>
  );
}
