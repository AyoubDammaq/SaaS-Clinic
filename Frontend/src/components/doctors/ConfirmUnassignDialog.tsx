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

interface ConfirmUnassignDialogProps {
  onConfirm: () => void;
  children?: ReactNode; // optional custom trigger
  title?: string;
  description?: string;
}

export const ConfirmUnassignDialog = ({
  onConfirm,
  children,
  title = "Confirmer la désassignation",
  description = "Voulez-vous vraiment désassigner ce médecin de la clinique ? Cette action est irréversible.",
}: ConfirmUnassignDialogProps) => {
  return (
    <AlertDialog>
      <AlertDialogTrigger asChild>
        {children || (
          <Button
            size="sm"
            variant="ghost"
            className="text-destructive"
            title="Désassigner"
            onClick={(e) => e.stopPropagation()}
          >
            <X className="h-4 w-4" />
          </Button>
        )}
      </AlertDialogTrigger>
      <AlertDialogContent onClick={(e) => e.stopPropagation()}>
        <AlertDialogHeader>
          <AlertDialogTitle>{title}</AlertDialogTitle>
          <AlertDialogDescription>{description}</AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel>Annuler</AlertDialogCancel>
          <AlertDialogAction
            className="bg-destructive text-white hover:bg-destructive/90"
            onClick={onConfirm}
          >
            Désassigner
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
};
