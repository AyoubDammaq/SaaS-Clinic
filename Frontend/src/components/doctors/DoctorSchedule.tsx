import React, { useEffect, useState } from 'react';
import { useDisponibilite } from '@/hooks/useDisponibilites';
import { Disponibilite, DayOfWeek, dayNames } from '@/types/disponibilite';
import { Button } from '@/components/ui/button';
import { Modal } from '@/components/ui/modal';
import { TimePicker } from '@/components/ui/time-picker';
import { DatePicker } from '@/components/ui/date-picker';
import { Card, CardContent } from '@/components/ui/card';
import { FileEdit, Trash2 } from 'lucide-react';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';

interface DoctorScheduleProps {
  doctorId: string;
}

export const DoctorSchedule: React.FC<DoctorScheduleProps> = ({ doctorId }) => {
  const {
    disponibilites,
    isLoading,
    isSubmitting,
    permissions,
    getAvailabilitiesByDoctor,
    addDisponibilite,
    updateDisponibilite,
    deleteDisponibilite,
  } = useDisponibilite(doctorId);

  const [modalOpen, setModalOpen] = useState(false);
  const [editingDisponibilite, setEditingDisponibilite] = useState<Disponibilite | null>(null);

  const [selectedDate, setSelectedDate] = useState<Date | undefined>(undefined);
  const [day, setDay] = useState<string>('');
  const [startTime, setStartTime] = useState<string>('');
  const [endTime, setEndTime] = useState<string>('');
  const [formError, setFormError] = useState<string>('');

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
    setDay('');
    setStartTime('');
    setEndTime('');
    setSelectedDate(undefined);
    setFormError('');
    setModalOpen(true);
  };

  const padTime = (time: string) => {
    const [h, m] = time.split(':');
    return `${h.padStart(2, '0')}:${m.padStart(2, '0')}`;
  };

  const openEditModal = (dispo: Disponibilite) => {
    setEditingDisponibilite(dispo);
    setDay(dayNames[dispo.jour]);
    setStartTime(padTime(dispo.heureDebut));
    setEndTime(padTime(dispo.heureFin));
    setSelectedDate(getNextDateForDay(dispo.jour));
    setFormError('');
    setModalOpen(true);
  };

  const closeModal = () => {
    setModalOpen(false);
    setEditingDisponibilite(null);
    setDay('');
    setStartTime('');
    setEndTime('');
    setSelectedDate(undefined);
    setFormError('');
  };

  const dayOfWeekToNumber: Record<DayOfWeek, number> = {
    Sunday: 0,
    Monday: 1,
    Tuesday: 2,
    Wednesday: 3,
    Thursday: 4,
    Friday: 5,
    Saturday: 6,
  };

  const handleSubmit = async () => {
    setFormError('');
    if (!day || !startTime || !endTime) {
      setFormError('Please fill in all fields.');
      return;
    }
    if (startTime >= endTime) {
      setFormError('Start time must be before end time.');
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
          setFormError('Invalid availability ID.');
          return;
        }
        await updateDisponibilite(editingDisponibilite.id, dispoData);
      } else {
        await addDisponibilite(dispoData);
      }
      closeModal();
      await getAvailabilitiesByDoctor(doctorId);
    } catch (error) {
      setFormError('Error saving availability');
      console.error(error);
    }
  };

  const handleDelete = async (id: string) => {
    if (window.confirm('Are you sure you want to delete this availability?')) {
      try {
        await deleteDisponibilite(id);
        await getAvailabilitiesByDoctor(doctorId);
      } catch (error) {
        alert('Error deleting availability');
        console.error(error);
      }
    }
  };

  return (
    <div>
      <div className="flex items-center justify-between mb-4">
        <h2 className="text-xl font-semibold">Doctor Schedule</h2>
        {permissions.canCreate && (
          <Button onClick={openAddModal} disabled={isSubmitting}>
            + Add Availability
          </Button>
        )}
      </div>
      <Card>
        <CardContent className="pt-6">
          {isLoading ? (
            <div className="flex justify-center items-center h-40">
              <p className="text-muted-foreground">Loading availabilities...</p>
            </div>
          ) : disponibilites.length === 0 ? (
            <div className="flex justify-center items-center h-40 text-muted-foreground">
              No availabilities found.
            </div>
          ) : (
            <div className="rounded-md border">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Day</TableHead>
                    <TableHead>Start Time</TableHead>
                    <TableHead>End Time</TableHead>
                    {(permissions.canEdit || permissions.canDelete) && <TableHead>Actions</TableHead>}
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
                              >
                                <FileEdit className="h-4 w-4" />
                              </Button>
                            )}
                            {permissions.canDelete && (
                              <Button
                                size="sm"
                                variant="ghost"
                                className="text-red-500"
                                onClick={() => handleDelete(dispo.id)}
                                disabled={isSubmitting}
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
          title={editingDisponibilite ? 'Edit Availability' : 'Add Availability'}
        >
          <form
            onSubmit={e => {
              e.preventDefault();
              handleSubmit();
            }}
            className="min-w-[320px] p-2"
          >
            <div className="mb-4">
              <label className="block mb-1 font-medium">Day</label>
              <DatePicker
                date={selectedDate}
                onChange={date => {
                  setSelectedDate(date);
                  if (date) {
                    const dayName = date.toLocaleDateString('en-US', { weekday: 'long' });
                    setDay(dayName);
                  } else {
                    setDay('');
                  }
                }}
                disabled={!!editingDisponibilite}
              />
              <small className="text-muted-foreground">
                {editingDisponibilite ? 'Day cannot be changed when editing.' : ''}
              </small>
            </div>

            <div className="mb-4">
              <label className="block mb-1 font-medium">Start Time</label>
              <TimePicker
                value={startTime}
                onChange={setStartTime}
                placeholder="e.g. 09:00"
              />
            </div>

            <div className="mb-4">
              <label className="block mb-1 font-medium">End Time</label>
              <TimePicker
                value={endTime}
                onChange={setEndTime}
                placeholder="e.g. 12:00"
              />
            </div>

            {formError && (
              <div className="text-red-600 mb-3">{formError}</div>
            )}

            <div className="flex justify-end gap-2">
              <Button type="button" variant="outline" onClick={closeModal}>
                Cancel
              </Button>
              <Button type="submit" disabled={isSubmitting}>
                {isSubmitting
                  ? (editingDisponibilite ? 'Updating...' : 'Adding...')
                  : (editingDisponibilite ? 'Update' : 'Add')}
              </Button>
            </div>
          </form>
        </Modal>
      )}
    </div>
  );
};