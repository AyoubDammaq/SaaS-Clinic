-- ========================================
-- Create Patients table
-- ========================================
CREATE TABLE Patients (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    Nom NVARCHAR(MAX) NOT NULL,
    Prenom NVARCHAR(MAX) NOT NULL,
    DateNaissance DATETIME2 NOT NULL,
    Sexe NVARCHAR(MAX) NOT NULL,
    Adresse NVARCHAR(MAX) NOT NULL,
    Telephone NVARCHAR(MAX) NOT NULL,
    Email NVARCHAR(MAX) NOT NULL,
    DateCreation DATETIME2 NOT NULL,
    DossierMedicalId UNIQUEIDENTIFIER NULL
);

-- ========================================
-- Create DossiersMedicaux table
-- ========================================
CREATE TABLE DossiersMedicaux (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    Allergies NVARCHAR(MAX) NOT NULL,
    MaladiesChroniques NVARCHAR(MAX) NOT NULL,
    MedicamentsActuels NVARCHAR(MAX) NOT NULL,
    AntécédentsFamiliaux NVARCHAR(MAX) NOT NULL,
    AntécédentsPersonnels NVARCHAR(MAX) NOT NULL,
    GroupeSanguin NVARCHAR(MAX) NOT NULL,
    DateCreation DATETIME2 NOT NULL,
    PatientId UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT FK_DossiersMedicaux_Patients_PatientId FOREIGN KEY (PatientId)
        REFERENCES Patients (Id) ON DELETE CASCADE
);

-- Ensure 1-to-1 relationship between Patients and DossiersMedicaux
CREATE UNIQUE INDEX IX_DossiersMedicaux_PatientId ON DossiersMedicaux (PatientId);

-- ========================================
-- Create Documents table (final version)
-- ========================================
CREATE TABLE Documents (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    Nom NVARCHAR(MAX) NOT NULL,
    Type NVARCHAR(MAX) NOT NULL,
    DateCreation DATETIME2 NOT NULL DEFAULT('0001-01-01T00:00:00'),
    Url NVARCHAR(MAX) NOT NULL DEFAULT(''),
    DossierMedicalId UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT FK_Documents_DossiersMedicaux_DossierMedicalId FOREIGN KEY (DossierMedicalId)
        REFERENCES DossiersMedicaux (Id) ON DELETE CASCADE
);

CREATE INDEX IX_Documents_DossierMedicalId ON Documents (DossierMedicalId);
