
import { useEffect, useMemo } from 'react';
import { useAuth } from "@/hooks/useAuth";
import { LineChart, Line, BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';
import { Calendar, CreditCard, Users, User, Activity, Building } from 'lucide-react';
import { StatCard } from '@/components/dashboard/StatCard';
import { AppointmentList } from '@/components/dashboard/AppointmentList';
import { RecentActivity } from '@/components/dashboard/RecentActivity';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';

// Mock data for dashboard stats
const mockAppointments = [
  { id: '1', patientName: 'John Doe', doctorName: 'Dr. Smith', date: '2025-04-30', time: '09:00 AM', status: 'Scheduled' as const },
  { id: '2', patientName: 'Jane Smith', doctorName: 'Dr. Johnson', date: '2025-04-30', time: '10:30 AM', status: 'Scheduled' as const },
  { id: '3', patientName: 'Robert Brown', doctorName: 'Dr. Smith', date: '2025-04-30', time: '11:45 AM', status: 'Cancelled' as const },
  { id: '4', patientName: 'Emily Davis', doctorName: 'Dr. Wilson', date: '2025-05-01', time: '02:15 PM', status: 'Scheduled' as const },
  { id: '5', patientName: 'Michael Wilson', doctorName: 'Dr. Johnson', date: '2025-05-01', time: '03:30 PM', status: 'Scheduled' as const },
];

const mockActivities = [
  { 
    id: '1', 
    type: 'appointment' as const, 
    title: 'New Appointment', 
    description: 'Jane Smith scheduled an appointment for May 2nd', 
    time: '15 minutes ago', 
    seen: false 
  },
  { 
    id: '2', 
    type: 'consultation' as const, 
    title: 'Consultation Completed', 
    description: 'Dr. Smith completed consultation with John Doe', 
    time: '1 hour ago', 
    seen: true 
  },
  { 
    id: '3', 
    type: 'payment' as const, 
    title: 'Payment Received', 
    description: 'Payment of $150 received from Michael Wilson', 
    time: '3 hours ago', 
    seen: true 
  },
  { 
    id: '4', 
    type: 'registration' as const, 
    title: 'New Patient Registration', 
    description: 'Emily Davis registered as a new patient', 
    time: '5 hours ago', 
    seen: true 
  },
];

const patientData = [
  { month: 'Jan', count: 45 },
  { month: 'Feb', count: 52 },
  { month: 'Mar', count: 61 },
  { month: 'Apr', count: 67 },
  { month: 'May', count: 70 },
  { month: 'Jun', count: 78 },
];

const appointmentData = [
  { name: 'Mon', scheduled: 12, completed: 10 },
  { name: 'Tue', scheduled: 15, completed: 13 },
  { name: 'Wed', scheduled: 18, completed: 15 },
  { name: 'Thu', scheduled: 14, completed: 12 },
  { name: 'Fri', scheduled: 16, completed: 14 },
  { name: 'Sat', scheduled: 10, completed: 8 },
  { name: 'Sun', scheduled: 5, completed: 3 },
];

function DashboardPage() {
  const { user } = useAuth();

  const dashboardContent = useMemo(() => {
    if (!user) return null;

    switch (user.role) {
      case 'SuperAdmin':
        return <SuperAdminDashboard />;
      case 'ClinicAdmin':
        return <ClinicAdminDashboard />;
      case 'Doctor':
        return <DoctorDashboard />;
      case 'Patient':
        return <PatientDashboard />;
      default:
        return <div>Unknown role</div>;
    }
  }, [user]);

  return (
    <div className="dashboard-container space-y-6 pb-8">
      <div className="flex flex-col gap-2">
        <h1 className="text-3xl font-bold tracking-tight">Dashboard</h1>
        <p className="text-muted-foreground">
          Welcome back, {user?.name}
        </p>
      </div>
      {dashboardContent}
    </div>
  );
}

function SuperAdminDashboard() {
  return (
    <div className="space-y-6">
      <div className="grid gap-4 grid-cols-1 sm:grid-cols-2 lg:grid-cols-4">
        <StatCard
          title="Total Clinics"
          value="12"
          icon={<Building className="h-4 w-4" />}
          trend={{ value: 20, isPositive: true }}
        />
        <StatCard
          title="Total Patients"
          value="3,120"
          icon={<Users className="h-4 w-4" />}
          trend={{ value: 15, isPositive: true }}
        />
        <StatCard
          title="Total Doctors"
          value="78"
          icon={<User className="h-4 w-4" />}
          trend={{ value: 5, isPositive: true }}
        />
        <StatCard
          title="Total Revenue"
          value="$185,324"
          icon={<CreditCard className="h-4 w-4" />}
          trend={{ value: 12, isPositive: true }}
        />
      </div>
      
      <div className="grid gap-6 grid-cols-1 lg:grid-cols-2">
        <Card>
          <CardHeader>
            <CardTitle>Clinic Growth</CardTitle>
            <CardDescription>New patient registrations across all clinics</CardDescription>
          </CardHeader>
          <CardContent className="h-80">
            <ResponsiveContainer width="100%" height="100%">
              <LineChart data={patientData}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="month" />
                <YAxis />
                <Tooltip />
                <Line type="monotone" dataKey="count" stroke="#0073b6" strokeWidth={2} />
              </LineChart>
            </ResponsiveContainer>
          </CardContent>
        </Card>
        
        <Card>
          <CardHeader>
            <CardTitle>Appointments Overview</CardTitle>
            <CardDescription>Weekly appointment statistics</CardDescription>
          </CardHeader>
          <CardContent className="h-80">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={appointmentData}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="name" />
                <YAxis />
                <Tooltip />
                <Bar dataKey="scheduled" fill="#0073b6" />
                <Bar dataKey="completed" fill="#00a0b0" />
              </BarChart>
            </ResponsiveContainer>
          </CardContent>
        </Card>
      </div>
      
      <div className="grid gap-6 grid-cols-1 lg:grid-cols-2">
        <AppointmentList appointments={mockAppointments} />
        <RecentActivity activities={mockActivities} />
      </div>
    </div>
  );
}

function ClinicAdminDashboard() {
  return (
    <div className="space-y-6">
      <div className="grid gap-4 grid-cols-1 sm:grid-cols-2 lg:grid-cols-4">
        <StatCard
          title="Total Patients"
          value="428"
          icon={<Users className="h-4 w-4" />}
          trend={{ value: 8, isPositive: true }}
        />
        <StatCard
          title="Active Doctors"
          value="16"
          icon={<User className="h-4 w-4" />}
          trend={{ value: 2, isPositive: true }}
        />
        <StatCard
          title="Today's Appointments"
          value="24"
          icon={<Calendar className="h-4 w-4" />}
          description="5 pending confirmation"
        />
        <StatCard
          title="Monthly Revenue"
          value="$24,532"
          icon={<CreditCard className="h-4 w-4" />}
          trend={{ value: 5, isPositive: true }}
        />
      </div>
      
      <div className="grid gap-6 grid-cols-1 lg:grid-cols-2">
        <AppointmentList appointments={mockAppointments} />
        <RecentActivity activities={mockActivities} />
      </div>
      
      <div className="grid gap-6 grid-cols-1">
        <Card>
          <CardHeader>
            <CardTitle>Weekly Activity</CardTitle>
            <CardDescription>Appointment statistics for your clinic</CardDescription>
          </CardHeader>
          <CardContent className="h-80">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={appointmentData}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="name" />
                <YAxis />
                <Tooltip />
                <Bar dataKey="scheduled" fill="#0073b6" />
                <Bar dataKey="completed" fill="#00a0b0" />
              </BarChart>
            </ResponsiveContainer>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}

function DoctorDashboard() {
  return (
    <div className="space-y-6">
      <div className="grid gap-4 grid-cols-1 sm:grid-cols-2 lg:grid-cols-3">
        <StatCard
          title="Today's Appointments"
          value="8"
          icon={<Calendar className="h-4 w-4" />}
          description="2 pending confirmation"
        />
        <StatCard
          title="Assigned Patients"
          value="96"
          icon={<Users className="h-4 w-4" />}
          description="12 new this month"
        />
        <StatCard
          title="Completed Consultations"
          value="145"
          icon={<Activity className="h-4 w-4" />}
          trend={{ value: 12, isPositive: true }}
          description="This month"
        />
      </div>
      
      <AppointmentList appointments={mockAppointments} />
      
      <div className="grid gap-6 grid-cols-1">
        <Card>
          <CardHeader>
            <CardTitle>Weekly Schedule</CardTitle>
            <CardDescription>Your appointment load for the week</CardDescription>
          </CardHeader>
          <CardContent className="h-80">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={appointmentData}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="name" />
                <YAxis />
                <Tooltip />
                <Bar dataKey="scheduled" fill="#0073b6" />
                <Bar dataKey="completed" fill="#00a0b0" />
              </BarChart>
            </ResponsiveContainer>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}

function PatientDashboard() {
  return (
    <div className="space-y-6">
      <div className="grid gap-4 grid-cols-1 sm:grid-cols-2 lg:grid-cols-3">
        <StatCard
          title="Upcoming Appointments"
          value="2"
          icon={<Calendar className="h-4 w-4" />}
          description="Next: April 30, 9:00 AM"
        />
        <StatCard
          title="Completed Consultations"
          value="8"
          icon={<Activity className="h-4 w-4" />}
          description="Since registration"
        />
        <StatCard
          title="Recent Payments"
          value="$120"
          icon={<CreditCard className="h-4 w-4" />}
          description="Last payment: April 15"
        />
      </div>
      
      <div className="grid gap-6 grid-cols-1 lg:grid-cols-2">
        <AppointmentList appointments={mockAppointments.slice(0, 2)} />
        <RecentActivity activities={mockActivities.filter(a => a.type !== 'registration')} />
      </div>
    </div>
  );
}

export default DashboardPage;
