import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
  DialogFooter,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Calendar, FileText, User } from "lucide-react";
import { Consultation } from "@/types/consultation";
import { format } from "date-fns";
import { useState } from "react";
import { ConsultationDetailsComplete } from "./ConsultationDetailsComplete";

interface ConsultationDetailsProps {
  isOpen: boolean;
  onClose: () => void;
  consultation: Consultation | null;
  patientName: string;
  doctorName: string;
}

export function ConsultationDetails({
  isOpen,
  onClose,
  consultation,
  patientName,
  doctorName,
}: ConsultationDetailsProps) {
  const [showCompleteDetails, setShowCompleteDetails] = useState(false);

  if (!consultation) return null;

  const splitDateTime = (dateTime: string) => {
    const [datePart, timePart] = dateTime.split("T");
    return {
      date: format(new Date(datePart), "yyyy-MM-dd"),
      time: timePart?.slice(0, 5) || "",
    };
  };

  const { date, time } = splitDateTime(consultation.dateConsultation);

  if (showCompleteDetails) {
    return (
      <ConsultationDetailsComplete
        isOpen={showCompleteDetails}
        onClose={() => {
          setShowCompleteDetails(false);
          onClose();
        }}
        consultation={consultation}
        patientName={patientName}
        doctorName={doctorName}
      />
    );
  }

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="sm:max-w-[550px]">
        <DialogHeader>
          <DialogTitle>Consultation Details</DialogTitle>
          <DialogDescription>
            View details for this medical consultation
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-4 py-4">
          <div className="flex justify-between items-start">
            <div>
              <h3 className="font-medium flex items-center gap-1">
                <Calendar className="h-4 w-4 text-muted-foreground" />
                Date & Time
              </h3>
            </div>
            <p className="text-sm">{date} at {time}</p>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <h3 className="font-medium flex items-center gap-1">
                <User className="h-4 w-4 text-muted-foreground" />
                Patient
              </h3>
              <p className="text-sm">{patientName}</p>
            </div>
            <div>
              <h3 className="font-medium flex items-center gap-1">
                <User className="h-4 w-4 text-muted-foreground" />
                Doctor
              </h3>
              <p className="text-sm">{doctorName}</p>
            </div>
          </div>

          <div>
            <h3 className="font-medium flex items-center gap-1">
              <FileText className="h-4 w-4 text-muted-foreground" />
              Diagnostic
            </h3>
            <p className="text-sm">{consultation.diagnostic}</p>
          </div>

          {consultation.notes && (
            <div>
              <h3 className="font-medium">Medical Notes</h3>
              <p className="text-sm whitespace-pre-wrap border p-3 rounded-md bg-muted/30">
                {consultation.notes}
              </p>
            </div>
          )}
        </div>

        <DialogFooter>
          <Button 
            variant="outline" 
            onClick={() => setShowCompleteDetails(true)}
          >
            Voir détails complets
          </Button>
          <Button onClick={onClose}>Close</Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
