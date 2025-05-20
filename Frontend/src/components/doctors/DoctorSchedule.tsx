
import { useState } from 'react';
import { Button } from '@/components/ui/button';
import { Calendar } from '@/components/ui/calendar';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { toast } from 'sonner';
import { format, addDays } from 'date-fns';
import { Clock } from 'lucide-react';
import { useTranslation } from '@/hooks/useTranslation';

const timeSlots = [
  '08:00', '08:30', '09:00', '09:30', '10:00', '10:30', '11:00', '11:30',
  '13:00', '13:30', '14:00', '14:30', '15:00', '15:30', '16:00', '16:30'
];

interface Availability {
  date: Date;
  slots: string[];
}

interface DoctorScheduleProps {
  doctorId: string;
  initialAvailability?: Availability[];
}

export function DoctorSchedule({ doctorId, initialAvailability = [] }: DoctorScheduleProps) {
  const [selectedDate, setSelectedDate] = useState<Date | undefined>(new Date());
  const [availabilities, setAvailabilities] = useState<Availability[]>(initialAvailability);
  const [selectedSlots, setSelectedSlots] = useState<string[]>([]);
  const { t } = useTranslation('appointments');
  
  const getAvailabilityForDate = (date: Date | undefined): string[] => {
    if (!date) return [];
    
    const dateStr = format(date, 'yyyy-MM-dd');
    const found = availabilities.find(
      a => format(a.date, 'yyyy-MM-dd') === dateStr
    );
    
    return found ? found.slots : [];
  };
  
  const currentAvailableSlots = selectedDate ? getAvailabilityForDate(selectedDate) : [];
  
  const handleDateSelect = (date: Date | undefined) => {
    setSelectedDate(date);
    setSelectedSlots(getAvailabilityForDate(date));
  };
  
  const toggleTimeSlot = (slot: string) => {
    setSelectedSlots(prev => 
      prev.includes(slot) 
        ? prev.filter(s => s !== slot) 
        : [...prev, slot].sort()
    );
  };
  
  const saveAvailability = () => {
    if (!selectedDate) return;
    
    const dateStr = format(selectedDate, 'yyyy-MM-dd');
    
    // Remove any existing availability for this date
    const filtered = availabilities.filter(
      a => format(a.date, 'yyyy-MM-dd') !== dateStr
    );
    
    // Add the new availability
    const newAvailabilities = [
      ...filtered,
      { date: selectedDate, slots: selectedSlots }
    ];
    
    setAvailabilities(newAvailabilities);
    
    // Here you would typically make an API call to save this to the backend
    // For now, we'll just show a success toast
    toast.success("Availability updated successfully");
    
    // In a real app, you would have API calls like:
    // await updateDoctorAvailability(doctorId, newAvailabilities);
  };
  
  const copyToNextWeek = () => {
    if (!selectedDate) return;
    
    const nextWeekDate = addDays(selectedDate, 7);
    const dateStr = format(nextWeekDate, 'yyyy-MM-dd');
    
    // Remove any existing availability for next week's date
    const filtered = availabilities.filter(
      a => format(a.date, 'yyyy-MM-dd') !== dateStr
    );
    
    // Copy current availability to next week
    const newAvailabilities = [
      ...filtered,
      { date: nextWeekDate, slots: [...selectedSlots] }
    ];
    
    setAvailabilities(newAvailabilities);
    toast.success(`Copied availability to ${format(nextWeekDate, 'EEEE, MMMM d')}`);
  };
  
  const isDateWithAvailability = (date: Date) => {
    const dateStr = format(date, 'yyyy-MM-dd');
    return availabilities.some(a => format(a.date, 'yyyy-MM-dd') === dateStr);
  };

  return (
    <div className="space-y-6">
      <div className="grid grid-cols-1 md:grid-cols-12 gap-6">
        <Card className="md:col-span-5">
          <CardHeader>
            <CardTitle>{t('selectDate')}</CardTitle>
          </CardHeader>
          <CardContent>
            <Calendar
              mode="single"
              selected={selectedDate}
              onSelect={handleDateSelect}
              className="p-3"
              modifiersClassNames={{
                selected: 'bg-primary text-primary-foreground',
                hasAvailability: 'border-b-2 border-green-500 font-bold'
              }}
              modifiers={{
                hasAvailability: (date) => isDateWithAvailability(date),
              }}
            />
            <div className="mt-4">
              <Button 
                variant="outline" 
                onClick={copyToNextWeek} 
                className="w-full"
                disabled={!selectedDate || selectedSlots.length === 0}
              >
                {t('copyToNextWeek')}
              </Button>
            </div>
          </CardContent>
        </Card>
        
        <Card className="md:col-span-7">
          <CardHeader>
            <CardTitle>
              {selectedDate 
                ? `${t('availability')} ${format(selectedDate, 'EEEE, MMMM d')}` 
                : t('selectDate')}
            </CardTitle>
          </CardHeader>
          <CardContent>
            {selectedDate ? (
              <div className="space-y-4">
                <div>
                  <div className="mb-2 block">{t('availableTimeSlots')}</div>
                  <div className="grid grid-cols-2 md:grid-cols-4 gap-2">
                    {timeSlots.map(slot => (
                      <Button
                        key={slot}
                        variant={selectedSlots.includes(slot) ? "default" : "outline"}
                        className="justify-start"
                        onClick={() => toggleTimeSlot(slot)}
                      >
                        <Clock className="mr-2 h-4 w-4" />
                        {slot}
                      </Button>
                    ))}
                  </div>
                </div>
                
                <div className="flex justify-end">
                  <Button onClick={saveAvailability}>{t('saveAvailability')}</Button>
                </div>
              </div>
            ) : (
              <div className="text-center py-8 text-muted-foreground">
                {t('selectDateFromCalendar')}
              </div>
            )}
          </CardContent>
        </Card>
      </div>
      
      <Card>
        <CardHeader>
          <CardTitle>{t('upcomingAvailability')}</CardTitle>
        </CardHeader>
        <CardContent>
          {availabilities.length > 0 ? (
            <div className="space-y-4">
              {availabilities
                .sort((a, b) => a.date.getTime() - b.date.getTime())
                .filter(a => a.date >= new Date(new Date().setHours(0, 0, 0, 0)))
                .slice(0, 5)
                .map((availability, i) => (
                  <div key={i} className="flex justify-between items-center">
                    <div>
                      <p className="font-medium">{format(availability.date, 'EEEE, MMMM d')}</p>
                      <div className="flex flex-wrap gap-1 mt-1">
                        {availability.slots.map(slot => (
                          <Badge key={slot} variant="outline" className="bg-blue-50 text-blue-600">
                            {slot}
                          </Badge>
                        ))}
                      </div>
                    </div>
                    <Button 
                      variant="ghost" 
                      size="sm"
                      onClick={() => handleDateSelect(availability.date)}
                    >
                      Edit
                    </Button>
                  </div>
                ))}
            </div>
          ) : (
            <p className="text-center py-4 text-muted-foreground">
              {t('noAvailabilitySet')}
            </p>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
