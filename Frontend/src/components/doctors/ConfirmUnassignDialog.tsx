import {
  AlertDialog,
  AlertDialogTrigger,
  AlertDialogContent,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogFooter,
  AlertDialogCancel,
  AlertDialogAction,
  AlertDialogDescription,
} from "@/components/ui/alert-dialog";
import { Button } from "@/components/ui/button";
import { X } from "lucide-react";
import { ReactNode } from "react";
import { useTranslation } from "@/hooks/useTranslation";

interface ConfirmUnassignDialogProps {
  onConfirm: () => void;
  children?: ReactNode; // optional custom trigger
  title?: string;
  description?: string;
}

export const ConfirmUnassignDialog = ({
  onConfirm,
  children,
  title,
  description,
}: ConfirmUnassignDialogProps) => {
  const { t } = useTranslation("doctors");

  return (
    <AlertDialog>
      <AlertDialogTrigger asChild>
        {children || (
          <Button
            size="sm"
            variant="ghost"
            className="text-destructive"
            title={t("unassignDoctor")}
            onClick={(e) => e.stopPropagation()}
          >
            <X className="h-4 w-4" />
          </Button>
        )}
      </AlertDialogTrigger>
      <AlertDialogContent onClick={(e) => e.stopPropagation()}>
        <AlertDialogHeader>
          <AlertDialogTitle>{title || t("confirmUnassignTitle")}</AlertDialogTitle>
          <AlertDialogDescription>
            {description || t("confirmUnassignDescription")}
          </AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel aria-label={t("cancel")}>
            {t("cancel")}
          </AlertDialogCancel>
          <AlertDialogAction
            className="bg-destructive text-white hover:bg-destructive/90"
            onClick={onConfirm}
            aria-label={t("unassignDoctor")}
          >
            {t("unassignDoctor")}
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
};