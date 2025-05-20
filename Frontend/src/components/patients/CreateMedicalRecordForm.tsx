import { useState, useEffect } from "react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";
import { Input } from "@/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

interface CreateMedicalRecordFormProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: {
    allergies: string;
    chronicDiseases: string;
    currentMedications: string;
    bloodType: string;
    personalHistory: string;
    familyHistory: string;
  }) => Promise<void>;
  isSubmitting: boolean;
}

export function CreateMedicalRecordModal({ 
    isOpen,
    onClose,
    onSubmit, 
    isSubmitting,
}: CreateMedicalRecordFormProps) {
  const [open, setOpen] = useState(false);
  const [formData, setFormData] = useState({
    allergies: '',
    chronicDiseases: '', 
    currentMedications: '', 
    bloodType: '', 
    personalHistory: '', 
    familyHistory: '', 
  });

  useEffect(() => {
    setOpen(isOpen);
  }, [isOpen]);

  const handleInputChange = (field: string, value: string) => {
    setFormData((prev) => ({ ...prev, [field]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
        await onSubmit(formData);
        onClose();
    } catch (error) {
        console.error(error);
    }
  };


  return (
    <Dialog open={isOpen} onOpenChange={(value) => !value && onClose()}>
      <DialogContent className="max-w-xl">
        <DialogHeader>
          <DialogTitle>Créer un dossier médical</DialogTitle>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium mb-2">Allergies</label>
            <Input
              placeholder="Entrez les allergies"
              value={formData.allergies}
              onChange={(e) => handleInputChange('allergies', e.target.value)}
            />
          </div>
          <div>
            <label className="block text-sm font-medium mb-2">Maladies Chroniques</label>
            <Input
              placeholder="Entrez les maladies chroniques"
              value={formData.chronicDiseases}
              onChange={(e) => handleInputChange('chronicDiseases', e.target.value)}
            />
          </div>
          <div>
            <label className="block text-sm font-medium mb-2">Médicaments Actuels</label>
            <Input
              placeholder="Entrez les médicaments actuels"
              value={formData.currentMedications}
              onChange={(e) => handleInputChange('currentMedications', e.target.value)}
            />
          </div>
          <div>
            <label className="block text-sm font-medium mb-2">Groupe Sanguin</label>
            <Select
              value={formData.bloodType}
              onValueChange={(value) => handleInputChange('bloodType', value)}
            >
              <SelectTrigger>
                <SelectValue placeholder="Sélectionnez le groupe sanguin" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="A+">A+</SelectItem>
                <SelectItem value="A-">A-</SelectItem>
                <SelectItem value="B+">B+</SelectItem>
                <SelectItem value="B-">B-</SelectItem>
                <SelectItem value="AB+">AB+</SelectItem>
                <SelectItem value="AB-">AB-</SelectItem>
                <SelectItem value="O+">O+</SelectItem>
                <SelectItem value="O-">O-</SelectItem>
              </SelectContent>
            </Select>
          </div>
          <div>
            <label className="block text-sm font-medium mb-2">Antécédents Personnels</label>
            <Textarea
              placeholder="Entrez les antécédents personnels"
              value={formData.personalHistory}
              onChange={(e) => handleInputChange('personalHistory', e.target.value)}
            />
          </div>
          <div>
            <label className="block text-sm font-medium mb-2">Antécédents Familiaux</label>
            <Textarea
              placeholder="Entrez les antécédents familiaux"
              value={formData.familyHistory}
              onChange={(e) => handleInputChange('familyHistory', e.target.value)}
            />
          </div>
          <Button type="submit" disabled={isSubmitting} className="w-full">
            {isSubmitting ? 'Création...' : 'Créer'}
          </Button>
        </form>
      </DialogContent>
    </Dialog>
  );
}