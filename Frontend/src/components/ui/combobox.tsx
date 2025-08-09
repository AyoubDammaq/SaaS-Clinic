"use client";

import { useEffect, useState } from "react";
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
} from "@/components/ui/command";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";
import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils";
import { Check, ChevronsUpDown } from "lucide-react";

export type ComboboxOption = {
  label: string;
  value: string;
};

type ComboboxProps = {
  options?: ComboboxOption[]; // optionnel, fallback = []
  value?: string;
  onChange: (value: string) => void;
  placeholder?: string;
  emptyMessage?: string;
  searchPlaceholder?: string;
  disabled?: boolean;
  className?: string;
  width?: string;
};

export const Combobox = ({
  options = [],
  value,
  onChange,
  placeholder = "Sélectionner une option",
  emptyMessage = "Aucune option disponible",
  searchPlaceholder = "Rechercher...",
  disabled = false,
  className,
  width = "w-[200px]",
}: ComboboxProps) => {
  const [open, setOpen] = useState(false);

  const selectedOption = options.find((option) => option.value === value);

  // Refermer le popover quand la sélection change
  useEffect(() => {
    if (value !== undefined) {
      setOpen(false);
    }
  }, [value]);

  return (
    <Popover open={open} onOpenChange={setOpen}>
      <PopoverTrigger asChild>
        <Button
          variant="outline"
          role="combobox"
          disabled={disabled}
          className={cn("justify-between", width, className)}
        >
          {selectedOption?.label ?? placeholder}
          <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
        </Button>
      </PopoverTrigger>

      <PopoverContent className={cn("p-0", width)}>
        <Command>
          <CommandInput placeholder={searchPlaceholder} />
          <CommandEmpty>{emptyMessage}</CommandEmpty>
          <CommandGroup>
            {(options ?? []).map((option) => (
              <CommandItem
                key={option.value}
                onSelect={() => {
                  onChange(option.value);
                }}
                className="cursor-pointer"
              >
                <span className="flex items-center justify-between w-full">
                  {option.label}
                  {option.value === value && (
                    <Check className="ml-2 h-4 w-4 text-primary" />
                  )}
                </span>
              </CommandItem>
            ))}
          </CommandGroup>
        </Command>
      </PopoverContent>
    </Popover>
  );
};
