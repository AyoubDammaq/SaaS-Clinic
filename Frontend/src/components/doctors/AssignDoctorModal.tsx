// components/doctors/AssignDoctorModal.tsx
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import {
  Select,
  SelectTrigger,
  SelectContent,
  SelectItem,
  SelectValue,
} from "@/components/ui/select";
import { Loader2 } from "lucide-react";
import { useTranslation } from "@/hooks/useTranslation";

interface AssignDoctorModalProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: () => void;
  selectedClinicId: string | null;
  setSelectedClinicId: (id: string) => void;
  clinics: { id: string; name: string }[];
  isSubmitting?: boolean;
}

export function AssignDoctorModal({
  isOpen,
  onClose,
  onConfirm,
  selectedClinicId,
  setSelectedClinicId,
  clinics,
  isSubmitting,
}: AssignDoctorModalProps) {
  const { t } = useTranslation("doctors");

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{t("assignDoctorToClinic")}</DialogTitle>
        </DialogHeader>

        <div className="space-y-4">
          <Select
            value={selectedClinicId ?? ""}
            onValueChange={(value) => setSelectedClinicId(value)}
          >
            <SelectTrigger>
              <SelectValue placeholder={t("selectClinic")} />
            </SelectTrigger>
            <SelectContent>
              {clinics.map((clinic) => (
                <SelectItem key={clinic.id} value={clinic.id}>
                  {clinic.name}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>

          <div className="flex justify-end gap-2">
            <Button variant="outline" onClick={onClose} disabled={isSubmitting}>
              {t("cancel")}
            </Button>
            <Button onClick={onConfirm} disabled={isSubmitting}>
              {isSubmitting && (
                <Loader2 className="mr-2 h-4 w-4 animate-spin" />
              )}
              {t("confirm")}
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
}
