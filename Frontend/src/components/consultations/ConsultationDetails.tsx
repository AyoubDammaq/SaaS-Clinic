
import { useState } from "react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
  DialogFooter,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Calendar, Clock, FileText, User } from "lucide-react";
import { Badge } from "@/components/ui/badge";

interface ConsultationDetailsProps {
  isOpen: boolean;
  onClose: () => void;
  consultation: {
    id: string;
    patientName: string;
    doctorName: string;
    date: string;
    time: string;
    duration: number;
    reason: string;
    status: 'Completed' | 'Pending' | 'Cancelled';
    notes?: string;
    prescription?: string;
  } | null;
}

export function ConsultationDetails({ 
  isOpen, 
  onClose, 
  consultation 
}: ConsultationDetailsProps) {
  if (!consultation) return null;

  const getStatusBadgeClass = (status: string) => {
    switch (status) {
      case 'Completed':
        return 'text-green-600 bg-green-50 border-green-200';
      case 'Pending':
        return 'text-blue-600 bg-blue-50 border-blue-200';
      case 'Cancelled':
        return 'text-red-600 bg-red-50 border-red-200';
      default:
        return '';
    }
  };

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
              <p className="text-sm">
                {consultation.date} at {consultation.time} ({consultation.duration} minutes)
              </p>
            </div>
            <Badge variant="outline" className={getStatusBadgeClass(consultation.status)}>
              {consultation.status}
            </Badge>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <h3 className="font-medium flex items-center gap-1">
                <User className="h-4 w-4 text-muted-foreground" />
                Patient
              </h3>
              <p className="text-sm">{consultation.patientName}</p>
            </div>
            <div>
              <h3 className="font-medium flex items-center gap-1">
                <User className="h-4 w-4 text-muted-foreground" />
                Doctor
              </h3>
              <p className="text-sm">{consultation.doctorName}</p>
            </div>
          </div>

          <div>
            <h3 className="font-medium flex items-center gap-1">
              <FileText className="h-4 w-4 text-muted-foreground" />
              Reason for Consultation
            </h3>
            <p className="text-sm">{consultation.reason}</p>
          </div>

          {consultation.notes && (
            <div>
              <h3 className="font-medium">Medical Notes</h3>
              <p className="text-sm whitespace-pre-wrap border p-3 rounded-md bg-muted/30">
                {consultation.notes}
              </p>
            </div>
          )}

          {consultation.prescription && (
            <div>
              <h3 className="font-medium">Prescription</h3>
              <p className="text-sm whitespace-pre-wrap border p-3 rounded-md bg-blue-50">
                {consultation.prescription}
              </p>
            </div>
          )}
        </div>

        <DialogFooter>
          <Button onClick={onClose}>Close</Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
