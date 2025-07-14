import { useState } from "react";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";
import { useTranslation } from "@/hooks/useTranslation";

interface CancelByDoctorDialogProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (justification: string) => void;
}

export const CancelByDoctorDialog = ({ isOpen, onClose, onSubmit }: CancelByDoctorDialogProps) => {
  const [justification, setJustification] = useState("");
  const t = useTranslation("appointments").t;
  const tCommon = useTranslation("common").t;

  const handleSubmit = () => {
    if (!justification.trim()) return;
    onSubmit(justification.trim());
    setJustification(""); // reset
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{t("cancelAppointment")}</DialogTitle>
        </DialogHeader>
        <Textarea
          placeholder={t("enterJustification")}
          value={justification}
          onChange={(e) => setJustification(e.target.value)}
        />
        <DialogFooter>
          <Button variant="outline" onClick={onClose}>
            {tCommon("cancel")}
          </Button>
          <Button variant="destructive" onClick={handleSubmit}>
            {t("confirmCancellation")}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};
