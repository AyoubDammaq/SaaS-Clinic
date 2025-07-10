import { useEffect, useState } from "react";
import { useDisponibilite } from "@/hooks/useDisponibilites";
import { CreneauDisponibleDto } from "@/types/disponibilite";

interface Props {
  doctorId: string;
  selectedDate: string; // format ISO: "2025-07-15"
}

export const DoctorAvailableSlots = ({ doctorId, selectedDate }: Props) => {
  const { getAvailableSlots } = useDisponibilite();
  const [slots, setSlots] = useState<CreneauDisponibleDto[]>([]);

  useEffect(() => {
    const fetchSlots = async () => {
      try {
        const data = await getAvailableSlots(doctorId, selectedDate);
        setSlots(data);
      } catch (err) {
        console.error("Erreur lors du chargement des créneaux disponibles");
      }
    };

    fetchSlots();
  }, [doctorId, selectedDate, getAvailableSlots]);

  return (
    <div className="p-4">
      <h3 className="text-lg font-semibold mb-2">Créneaux disponibles</h3>
      {slots.length === 0 ? (
        <p>Aucun créneau disponible</p>
      ) : (
        <ul className="space-y-1">
          {slots.map((slot, index) => (
            <li
              key={index}
              className="bg-green-100 text-green-800 px-3 py-1 rounded shadow-sm"
            >
              {new Date(slot.dateHeureDebut).toLocaleTimeString("fr-FR", {
                hour: "2-digit",
                minute: "2-digit",
              })}{" "}
              -{" "}
              {new Date(slot.dateHeureFin).toLocaleTimeString("fr-FR", {
                hour: "2-digit",
                minute: "2-digit",
              })}
            </li>
          ))}
        </ul>
      )}
    </div>
  );
};
