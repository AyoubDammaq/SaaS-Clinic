import React, { useEffect, useState } from "react";
import { useDisponibilite } from "@/hooks/useDisponibilites";
import { Disponibilite, DayOfWeek, CreneauDisponibleDto } from "@/types/disponibilite";
import { Button } from "@/components/ui/button";
import { Modal } from "@/components/ui/modal";
import { TimePicker } from "@/components/ui/time-picker";
import { DatePicker } from "@/components/ui/date-picker";
import { Card, CardContent } from "@/components/ui/card";
import { FileEdit, Trash2 } from "lucide-react";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { useTranslation } from "@/hooks/useTranslation";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
  DialogFooter,
} from "@/components/ui/dialog";
import { useAuth } from "@/hooks/useAuth";

interface DoctorScheduleProps {
  doctorId: string;
}

export const DoctorSchedule: React.FC<DoctorScheduleProps> = ({ doctorId }) => {
  const { t } = useTranslation("doctors");
  const { user } = useAuth();
  const {
    disponibilites,
    isLoading,
    isSubmitting,
    permissions,
    getAvailabilitiesByDoctor,
    getAvailableSlots,
    addDisponibilite,
    updateDisponibilite,
    deleteDisponibilite,
  } = useDisponibilite();

  const [modalOpen, setModalOpen] = useState(false);
  const [editingDisponibilite, setEditingDisponibilite] =
    useState<Disponibilite | null>(null);
  const [selectedDateForSlots, setSelectedDateForSlots] = useState<Date | null>(
    null
  );
  const [creneauxDisponibles, setCreneauxDisponibles] = useState<
    CreneauDisponibleDto[]
  >([]);
  const [selectedDate, setSelectedDate] = useState<Date | undefined>(undefined);
  const [day, setDay] = useState<string>(""); // Stores English day name for logic
  const [startTime, setStartTime] = useState<string>("");
  const [endTime, setEndTime] = useState<string>("");
  const [formError, setFormError] = useState<string>("");
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [availabilityIdToDelete, setAvailabilityIdToDelete] = useState<string | null>(null);

  // Traduire les noms des jours pour l'affichage
  const dayNames: Record<number, string> = {
    0: t("sunday"),
    1: t("monday"),
    2: t("tuesday"),
    3: t("wednesday"),
    4: t("thursday"),
    5: t("friday"),
    6: t("saturday"),
  };

  // Mappage des jours pour la logique (en anglais pour correspondre à DayOfWeek)
  const dayOfWeekToNumber: Record<DayOfWeek, number> = {
    Sunday: 0,
    Monday: 1,
    Tuesday: 2,
    Wednesday: 3,
    Thursday: 4,
    Friday: 5,
    Saturday: 6,
  };

  useEffect(() => {
    if (doctorId) {
      getAvailabilitiesByDoctor(doctorId);
    }
  }, [doctorId, getAvailabilitiesByDoctor]);

  const getNextDateForDay = (weekday: number) => {
    const today = new Date();
    const todayDay = today.getDay();
    let diff = weekday - todayDay;
    if (diff < 0) diff += 7;
    const nextDate = new Date(today);
    nextDate.setDate(today.getDate() + diff);
    return nextDate;
  };

  const openAddModal = () => {
    setEditingDisponibilite(null);
    setDay("");
    setStartTime("");
    setEndTime("");
    setSelectedDate(undefined);
    setFormError("");
    setModalOpen(true);
  };

  const padTime = (time: string) => {
    const [h, m] = time.split(":");
    return `${h.padStart(2, "0")}:${m.padStart(2, "0")}`;
  };

  const openEditModal = (dispo: Disponibilite) => {
    setEditingDisponibilite(dispo);
    const englishDayName = Object.keys(dayOfWeekToNumber)[dispo.jour];
    setDay(englishDayName);
    setStartTime(padTime(dispo.heureDebut));
    setEndTime(padTime(dispo.heureFin));
    setSelectedDate(getNextDateForDay(dispo.jour));
    setFormError("");
    setModalOpen(true);
  };

  const closeModal = () => {
    setModalOpen(false);
    setEditingDisponibilite(null);
    setDay("");
    setStartTime("");
    setEndTime("");
    setSelectedDate(undefined);
    setFormError("");
  };

  const handleSubmit = async () => {
    setFormError("");
    if (!day || !startTime || !endTime) {
      setFormError(t("fillAllFields"));
      return;
    }
    if (startTime >= endTime) {
      setFormError(t("invalidTimeRange"));
      return;
    }

    const dispoData = {
      medecinId: doctorId,
      jour: dayOfWeekToNumber[day as DayOfWeek],
      heureDebut: startTime,
      heureFin: endTime,
    };

    try {
      if (editingDisponibilite) {
        if (!editingDisponibilite.id) {
          setFormError(t("invalidAvailabilityId"));
          return;
        }
        await updateDisponibilite(editingDisponibilite.id, dispoData);
      } else {
        await addDisponibilite(dispoData);
      }
      closeModal();
      await getAvailabilitiesByDoctor(doctorId);
    } catch (error) {
      setFormError(t("errorSavingAvailability"));
      console.error(error);
    }
  };

  const openDeleteDialog = (id: string) => {
    setAvailabilityIdToDelete(id);
    setDeleteDialogOpen(true);
  };

  const closeDeleteDialog = () => {
    setDeleteDialogOpen(false);
    setAvailabilityIdToDelete(null);
  };

  const handleDelete = async () => {
    if (!availabilityIdToDelete) return;
    try {
      await deleteDisponibilite(availabilityIdToDelete);
      await getAvailabilitiesByDoctor(doctorId);
      closeDeleteDialog();
    } catch (error) {
      alert(t("errorDeletingAvailability"));
      console.error(error);
    }
  };

  return (
    <div>
      <div className="flex items-center justify-between mb-4">
        <h2 className="text-xl font-semibold">{t("doctorScheduleTitle")}</h2>
        {permissions.canCreate && user.role !== "SuperAdmin" &&(
          <Button onClick={openAddModal} disabled={isSubmitting}>
            {t("addAvailability")}
          </Button>
        )}
      </div>
      <Card>
        <CardContent className="pt-6">
          {isLoading ? (
            <div className="flex justify-center items-center h-40">
              <p className="text-muted-foreground">{t("loadingAvailabilities")}</p>
            </div>
          ) : disponibilites.length === 0 ? (
            <div className="flex justify-center items-center h-40 text-muted-foreground">
              {t("noAvailabilityFound")}
            </div>
          ) : (
            <div className="rounded-md border">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>{t("day")}</TableHead>
                    <TableHead>{t("startTime")}</TableHead>
                    <TableHead>{t("endTime")}</TableHead>
                    {(permissions.canEdit || permissions.canDelete) && (
                      <TableHead>{t("actions")}</TableHead>
                    )}
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {disponibilites.map((dispo) => (
                    <TableRow key={dispo.id} className="hover:bg-muted/60">
                      <TableCell>{dayNames[dispo.jour]}</TableCell>
                      <TableCell>{dispo.heureDebut}</TableCell>
                      <TableCell>{dispo.heureFin}</TableCell>
                      {(permissions.canEdit || permissions.canDelete) && (
                        <TableCell>
                          <div className="flex items-center gap-2">
                            {permissions.canEdit && (
                              <Button
                                size="sm"
                                variant="ghost"
                                onClick={() => openEditModal(dispo)}
                                disabled={isSubmitting}
                                aria-label={t("editAvailability")}
                              >
                                <FileEdit className="h-4 w-4" />
                              </Button>
                            )}
                            {permissions.canDelete && (
                              <Button
                                size="sm"
                                variant="ghost"
                                className="text-red-500"
                                onClick={() => openDeleteDialog(dispo.id)}
                                disabled={isSubmitting}
                                aria-label={t("deleteAvailability")}
                              >
                                <Trash2 className="h-4 w-4" />
                              </Button>
                            )}
                          </div>
                        </TableCell>
                      )}
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </div>
          )}
        </CardContent>
      </Card>

      {/* Modal for add/edit */}
      {modalOpen && (
        <Modal
          isOpen={modalOpen}
          onClose={closeModal}
          title={
            editingDisponibilite ? t("editAvailability") : t("addAvailability")
          }
        >
          <form
            onSubmit={(e) => {
              e.preventDefault();
              handleSubmit();
            }}
            className="min-w-[320px] p-2"
          >
            <div className="mb-4">
              <label className="block mb-1 font-medium">{t("day")}</label>
              <DatePicker
                date={selectedDate}
                onChange={(date) => {
                  setSelectedDate(date);
                  if (date) {
                    const dayName = date.toLocaleDateString("en-US", {
                      weekday: "long",
                    });
                    setDay(dayName); // Store English day name
                  } else {
                    setDay("");
                  }
                }}
                disabled={!!editingDisponibilite}
              />
              <small className="text-muted-foreground">
                {editingDisponibilite ? t("dayCannotBeChanged") : ""}
              </small>
            </div>

            <div className="mb-4">
              <label className="block mb-1 font-medium">{t("startTime")}</label>
              <TimePicker
                value={startTime}
                onChange={setStartTime}
                placeholder={t("timePlaceholder")}
              />
            </div>

            <div className="mb-4">
              <label className="block mb-1 font-medium">{t("endTime")}</label>
              <TimePicker
                value={endTime}
                onChange={setEndTime}
                placeholder={t("timePlaceholder")}
              />
            </div>

            {formError && <div className="text-red-600 mb-3">{formError}</div>}

            <div className="flex justify-end gap-2">
              <Button type="button" variant="outline" onClick={closeModal}>
                {t("cancel")}
              </Button>
              <Button type="submit" disabled={isSubmitting}>
                {isSubmitting
                  ? editingDisponibilite
                    ? t("saving")
                    : t("saving")
                  : editingDisponibilite
                  ? t("update")
                  : t("create")}
              </Button>
            </div>
          </form>
        </Modal>
      )}

      {/* Confirm Delete Dialog */}
      {deleteDialogOpen && (
        <Dialog open={deleteDialogOpen} onOpenChange={closeDeleteDialog}>
          <DialogContent>
            <DialogHeader>
              <DialogTitle>{t("deleteAvailability")}</DialogTitle>
              <DialogDescription>{t("confirmDeleteAvailability")}</DialogDescription>
            </DialogHeader>
            <DialogFooter>
              <Button variant="outline" onClick={closeDeleteDialog}>
                {t("cancel")}
              </Button>
              <Button variant="destructive" onClick={handleDelete}>
                {t("confirm")}
              </Button>
            </DialogFooter>
          </DialogContent>
        </Dialog>
      )}
    </div>
  );
};