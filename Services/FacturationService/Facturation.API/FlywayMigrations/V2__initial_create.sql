-- V1__Initial_Create.sql

-- Create Factures Table
CREATE TABLE Factures (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    PatientId UNIQUEIDENTIFIER NOT NULL,
    ConsultationId UNIQUEIDENTIFIER NOT NULL,
    ClinicId UNIQUEIDENTIFIER NOT NULL,
    DateEmission DATETIME2 NOT NULL,
    MontantTotal DECIMAL(18,2) NOT NULL,
    Status INT NOT NULL
);

-- Create Paiements Table
CREATE TABLE Paiements (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    Montant DECIMAL(18,2) NOT NULL,
    DatePaiement DATETIME2 NOT NULL,
    Mode INT NOT NULL,
    FactureId UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT FK_Paiements_Factures_FactureId
        FOREIGN KEY (FactureId) REFERENCES Factures(Id)
        ON DELETE CASCADE
);

-- Create Unique Index on Paiement.FactureId
CREATE UNIQUE INDEX IX_Paiements_FactureId ON Paiements(FactureId);
