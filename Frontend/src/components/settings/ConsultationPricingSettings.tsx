import { useState, useEffect } from "react";
import { useAuth } from "@/hooks/useAuth";
import { useBilling } from "@/hooks/useBilling";
import { ConsultationType, consultationTypes } from "@/types/consultation";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Separator } from "@/components/ui/separator";
import { Save, DollarSign, Loader2 } from "lucide-react";
import { useToast } from "@/hooks/use-toast";

// Préparer options consultation
const consultationTypeOptions = Object.entries(consultationTypes).map(
  ([key, label]) => ({
    id: Number(key) as ConsultationType,
    label,
  })
);

interface ConsultationPricingSettingsProps {
  clinicId?: string;
}

export function ConsultationPricingSettings({ clinicId }: ConsultationPricingSettingsProps) {
  const { user } = useAuth();
  const { toast } = useToast();

  clinicId = user?.cliniqueId;

  const {
    tarifications,
    isTarifsLoading,
    tarifsError,
    fetchTarifications,
    refreshTarifications,
    addTarification,
    updateTarification,
  } = useBilling();

  // State local prix (clé = id consultationType)
  const [pricing, setPricing] = useState<Record<number, number>>({});
  const [saving, setSaving] = useState(false);

  // Charger tarifications à la sélection de la clinique
  useEffect(() => {
    if (clinicId) {
      fetchTarifications(clinicId);
    }
  }, [clinicId, fetchTarifications]);

  // Synchroniser tarifications dans pricing local
  useEffect(() => {
    if (tarifications.length > 0) {
      const priceMap: Record<number, number> = {};
      consultationTypeOptions.forEach((type) => {
        const existing = tarifications.find(
          (t) => t.consultationType === type.id
        );
        priceMap[type.id] = existing?.prix || 0;
      });
      setPricing(priceMap);
    }
  }, [tarifications]);

  // Modifier un prix localement
  const handlePriceChange = (typeId: number, value: string) => {
    const numericValue = parseFloat(value) || 0;
    setPricing((prev) => ({
      ...prev,
      [typeId]: numericValue,
    }));
  };

  // Sauvegarder / Mettre à jour les prix via hook billing
  const handleSavePricing = async () => {
    if (!clinicId) return;

    setSaving(true);
    try {
      // Pour chaque type, soit update soit add
      const promises = consultationTypeOptions.map(async (type) => {
        const price = pricing[type.id];
        const existing = tarifications.find(t => t.consultationType === type.id);

        if (existing) {
          return updateTarification({
            id: existing.id,
            consultationType: type.id,
            prix: price,
          });
        } else if (price > 0) {
          return addTarification({
            consultationType: type.id,
            prix: price,
            clinicId,
          });
        }
      });

      await Promise.all(promises.filter(Boolean));
      toast({
        title: "Succès",
        description: "Les tarifs ont été sauvegardés avec succès",
      });

      refreshTarifications(clinicId);
    } catch (error) {
      console.error("Erreur sauvegarde tarifs:", error);
      toast({
        title: "Erreur",
        description: "Impossible de sauvegarder les tarifs",
        variant: "destructive",
      });
    } finally {
      setSaving(false);
    }
  };

  if (user?.role !== "ClinicAdmin") {
    return null;
  }

  if (isTarifsLoading) {
    return (
      <div className="flex items-center justify-center py-8">
        <Loader2 className="h-6 w-6 animate-spin" />
        <span className="ml-2">Chargement des tarifs...</span>
      </div>
    );
  }

  if (tarifsError) {
    return (
      <div className="text-red-600 py-4 text-center">
        Erreur: {tarifsError}
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div>
        <h3 className="text-lg font-medium">Tarification des Consultations</h3>
        <p className="text-sm text-muted-foreground">
          Configurez les tarifs pour chaque type de consultation
        </p>
      </div>
      <Separator />

      <div className="grid gap-4">
        {consultationTypeOptions.map((type) => (
          <Card key={type.id} className="p-4">
            <div className="flex items-center justify-between">
              <div className="flex-1">
                <div className="flex items-center gap-2 mb-1">
                  <DollarSign className="h-4 w-4 text-muted-foreground" />
                  <Label className="font-medium">{type.label}</Label>
                </div>
              </div>

              <div className="flex items-center gap-2">
                <Input
                  type="number"
                  min="0"
                  step="0.01"
                  value={pricing[type.id] ?? ""}
                  onChange={(e) => handlePriceChange(type.id, e.target.value)}
                  placeholder="0.00"
                  className="w-24 text-right"
                />
                <span className="text-sm text-muted-foreground">€</span>
              </div>
            </div>
          </Card>
        ))}
      </div>

      <div className="flex justify-end">
        <Button
          onClick={handleSavePricing}
          disabled={saving}
          className="flex items-center gap-2"
        >
          {saving ? <Loader2 className="h-4 w-4 animate-spin" /> : <Save className="h-4 w-4" />}
          {saving ? "Sauvegarde..." : "Sauvegarder les Tarifs"}
        </Button>
      </div>
    </div>
  );
}
