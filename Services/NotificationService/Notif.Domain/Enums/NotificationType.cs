namespace Notif.Domain.Enums
{
    public enum NotificationType
    {
        // Patient notifications
        AppointmentConfirmation = 1,
        AppointmentReminder = 2,
        AppointmentCancellation = 3,
        PrescriptionReady = 4,
        TestResultsAvailable = 5,
        PaymentDue = 6,
        PaymentConfirmation = 7,
        AppointmentCancelledByDoctor = 8, 
        FactureAdded = 9,
        AppointmentCreatetd = 10,
        AppointmentUpdated = 11,

        // Doctor notifications
        NewAppointment = 20,
        AppointmentModified = 21,
        PatientArrived = 22,
        UrgentMessage = 23,
        TestResultsReceived = 24,

        // ClinicAdmin notifications
        NewPatientRegistered = 40,
        LastMinuteCancellation = 41,
        PaymentIssue = 42,
        StockAlert = 43,
        EquipmentMaintenance = 44,

        // SuperAdmin notifications
        SystemAlert = 60,
        SecurityAlert = 61,
        BackupFailed = 62,
        PerformanceReport = 63
    }
}
