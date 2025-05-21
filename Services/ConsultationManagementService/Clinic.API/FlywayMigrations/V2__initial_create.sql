CREATE TABLE Consultations (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    PatientId UNIQUEIDENTIFIER NOT NULL,
    MedecinId UNIQUEIDENTIFIER NOT NULL,
    DateConsultation DATETIME2 NOT NULL,
    Diagnostic NVARCHAR(500) NOT NULL,
    Notes NVARCHAR(1000) NOT NULL
);

CREATE TABLE DocumentsMedicaux (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    ConsultationId UNIQUEIDENTIFIER NOT NULL,
    Type NVARCHAR(50) NOT NULL,
    FichierURL NVARCHAR(MAX) NOT NULL,
    DateAjout DATETIME2 NOT NULL,
    CONSTRAINT FK_DocumentsMedicaux_Consultations_ConsultationId FOREIGN KEY (ConsultationId)
        REFERENCES Consultations(Id)
        ON DELETE CASCADE
);

CREATE INDEX IX_DocumentsMedicaux_ConsultationId ON DocumentsMedicaux (ConsultationId);
