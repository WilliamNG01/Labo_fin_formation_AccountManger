-- Création de la base de données fl_db_medical_documents
IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = 'fl_db_medical_documents')
BEGIN
    CREATE DATABASE fl_db_medical_documents;
END
GO

USE fl_db_medical_documents;
GO

-- Création des schémas
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'dcmt')
BEGIN
    EXEC('CREATE SCHEMA dcmt AUTHORIZATION dbo');
	PRINT 'Schema "dcmt" created.';
end
go
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'scty')
BEGIN
    EXEC('CREATE SCHEMA scty AUTHORIZATION dbo');
	PRINT 'Schema "scty" created.';
end
GO

-- Création des logins et utilisateurs sécurisés
IF NOT EXISTS (SELECT * FROM sys.syslogins WHERE [name] = N'dbmd_admin ')
BEGIN
    CREATE LOGIN dbmd_admin WITH PASSWORD = '$(doc_owner)';
END
go

IF NOT EXISTS (SELECT * FROM sys.syslogins WHERE [name] = N'dbmd_reader ')
BEGIN
    CREATE LOGIN dbmd_reader WITH PASSWORD = '$(doc_writer)';
END
go

IF NOT EXISTS (SELECT * FROM sys.syslogins WHERE [name] = N'dbmd_writer ')
BEGIN
    CREATE LOGIN dbmd_writer WITH PASSWORD = '$(doc_reader)';
END
GO

-- Création des utilisateurs dans la base
IF NOT EXISTS (SELECT * FROM sys.sysusers WHERE [name] = N'dbmd_admin')
BEGIN
    CREATE USER dbmd_admin FOR LOGIN dbmd_admin;
end

go

IF NOT EXISTS (SELECT * FROM sys.sysusers WHERE [name] = N'dbmd_reader')
BEGIN
    CREATE USER dbmd_reader FOR LOGIN dbmd_reader;
end
go
IF NOT EXISTS (SELECT * FROM sys.sysusers WHERE [name] = N'dbmd_writer')
BEGIN
    CREATE USER dbmd_writer FOR LOGIN dbmd_writer;
end
GO

-- Attribution des rôles SQL Server
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'DocumentReader' AND type = 'R')
BEGIN
    CREATE ROLE DocumentReader;
END
GO

GRANT SELECT ON SCHEMA::dcmt TO DocumentReader;
GRANT SELECT ON SCHEMA::scty TO DocumentReader;

IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'DocumentWriter' AND type = 'R')
BEGIN
    CREATE ROLE DocumentWriter;
END
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dcmt TO DocumentWriter;
GRANT SELECT ON SCHEMA::scty TO DocumentWriter;

-- Attribution des rôles aux utilisateurs
ALTER ROLE db_owner ADD MEMBER dbmd_admin;
ALTER ROLE DocumentReader ADD MEMBER dbmd_reader;
ALTER ROLE DocumentWriter ADD MEMBER dbmd_writer;
GO

-- Table des ConfidentialityLevel
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ConfidentialityLevel' AND schema_id = SCHEMA_ID('dcmt'))
BEGIN
    CREATE TABLE dcmt.ConfidentialityLevel (
        Id int primary key Identity (1,1),
        LevelName NVARCHAR(255) NOT NULL,
        LevelDescription NVARCHAR(255) NOT NULL,
        LevelPriority int NOT NULL
    );
    INSERT INTO dcmt.ConfidentialityLevel (LevelName, LevelDescription, LevelPriority) VALUES ('Public', 'Public', 1);
    INSERT INTO dcmt.ConfidentialityLevel (LevelName, LevelDescription, LevelPriority) VALUES ('Interne', 'Interne', 2);
    INSERT INTO dcmt.ConfidentialityLevel (LevelName, LevelDescription, LevelPriority) VALUES ('Restreint', 'Restreint', 3);
    INSERT INTO dcmt.ConfidentialityLevel (LevelName, LevelDescription, LevelPriority) VALUES ('Confidentiel', 'Confidentiel', 4);
END
GO
-- Table des documents
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Documents' AND schema_id = SCHEMA_ID('dcmt'))
BEGIN
    CREATE TABLE dcmt.Documents (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        Name NVARCHAR(255) NOT NULL,
        Path NVARCHAR(500) NOT NULL,
        PriorityLevel INT FOREIGN KEY REFERENCES dcmt.ConfidentialityLevel(Id) ON DELETE SET NULL,
        CreatedBy UNIQUEIDENTIFIER NOT NULL,
        CreatedName NVARCHAR(100) NOT NULL,
        CreatedAt DATETIME DEFAULT GETDATE(),
        DeletedBy UNIQUEIDENTIFIER NULL,
        DeletedAt DATETIME DEFAULT Null,
        UpdatedBy UNIQUEIDENTIFIER NULL,
        UpdatedAt DATETIME DEFAULT NULL
    );
END
GO
-- Table des AccessLevel
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'AccessLevel' AND schema_id = SCHEMA_ID('dcmt'))
BEGIN
    CREATE TABLE dcmt.AccessLevel (
        Id int PRIMARY KEY identity (1,1),
        AccessLevelName NVARCHAR(255) NOT NULL,
        AccessLevelDescription NVARCHAR(255) NULL,
        AccessPriority int  NULL
    );
    INSERT INTO dcmt.AccessLevel (AccessLevelName, AccessLevelDescription, AccessPriority) VALUES ('Lecture', 'Lecture',1);
    INSERT INTO dcmt.AccessLevel (AccessLevelName, AccessLevelDescription, AccessPriority) VALUES ('Écriture', 'Écriture',1);
    INSERT INTO dcmt.AccessLevel (AccessLevelName, AccessLevelDescription, AccessPriority) VALUES ('Admin', 'Admin',1);
END
GO

-- Table des accès aux documents
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'DocumentAccess' AND schema_id = SCHEMA_ID('dcmt'))
BEGIN
CREATE TABLE dcmt.DocumentAccess (
    DocumentId UNIQUEIDENTIFIER NOT NULL,
    UserId UNIQUEIDENTIFIER NOT NULL,
    AccessLevel INT FOREIGN KEY REFERENCES dcmt.AccessLevel(Id) ON DELETE SET NULL,
    PRIMARY KEY (DocumentId, UserId),
    FOREIGN KEY (DocumentId) REFERENCES dcmt.Documents(Id)
);
END
GO
-- Table des policies
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Policies' AND schema_id = SCHEMA_ID('scty'))
BEGIN
CREATE TABLE scty.Policies (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(100) NOT NULL UNIQUE
);
END
GO

-- Table d'association entre les rôles Identity et les policies
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'RolePolicies' AND schema_id = SCHEMA_ID('scty'))
BEGIN
CREATE TABLE scty.RolePolicies (
    RoleName NVARCHAR(100) NOT NULL,
    PolicyId UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY (RoleName, PolicyId),
    FOREIGN KEY (PolicyId) REFERENCES scty.Policies(Id)
);
END
GO
-- Insertion des policies
INSERT INTO scty.Policies (Name) VALUES
('FullAccessPolicy'),
('MedicalRecordsPolicy'),
('CoordinationPolicy'),
('SupplyChainPolicy'),
('InternalDocumentsPolicy'),
('DefaultRestrictedPolicy');

-- Mapping des rôles existants aux policies
INSERT INTO scty.RolePolicies (RoleName, PolicyId)
SELECT 'SuperAdmin', Id FROM scty.Policies WHERE Name = 'FullAccessPolicy'
UNION ALL
SELECT 'Admin', Id FROM scty.Policies WHERE Name = 'FullAccessPolicy'
UNION ALL
SELECT 'Médecin', Id FROM scty.Policies WHERE Name = 'MedicalRecordsPolicy'
UNION ALL
SELECT 'Infirmier', Id FROM scty.Policies WHERE Name = 'MedicalRecordsPolicy'
UNION ALL
SELECT 'Patient', Id FROM scty.Policies WHERE Name = 'MedicalRecordsPolicy'
UNION ALL
SELECT 'Coordinateur', Id FROM scty.Policies WHERE Name = 'CoordinationPolicy'
UNION ALL
SELECT 'Admin', Id FROM scty.Policies WHERE Name = 'CoordinationPolicy'
UNION ALL
SELECT 'Fournisseur', Id FROM scty.Policies WHERE Name = 'SupplyChainPolicy'
UNION ALL
SELECT 'Employé', Id FROM scty.Policies WHERE Name = 'SupplyChainPolicy'
UNION ALL
SELECT 'Employé', Id FROM scty.Policies WHERE Name = 'InternalDocumentsPolicy'
UNION ALL
SELECT 'Bénévole', Id FROM scty.Policies WHERE Name = 'InternalDocumentsPolicy'
UNION ALL
SELECT 'DefaultUser', Id FROM scty.Policies WHERE Name = 'DefaultRestrictedPolicy';

  INSERT INTO [fl_db_medical_documents].[dcmt].[Documents] (Id, Name, Path, PriorityLevel, CreatedBy, CreatedName, CreatedAt)
VALUES
  (NEWID(), 'Document_1', '/documents/doc1.pdf', FLOOR(RAND() * 4) + 1, 'a39182c1-7a5f-47c7-9002-03dabdbdefe0', 'janeDoe', GETDATE()),
  (NEWID(), 'Document_2', '/documents/doc2.pdf', FLOOR(RAND() * 4) + 1, 'a39182c1-7a5f-47c7-9002-03dabdbdefe0', 'janeDoe', GETDATE()),
  (NEWID(), 'Document_3', '/documents/doc3.pdf', FLOOR(RAND() * 4) + 1, 'a39182c1-7a5f-47c7-9002-03dabdbdefe0', 'janeDoe', GETDATE()),
  (NEWID(), 'Document_4', '/documents/doc4.pdf', FLOOR(RAND() * 4) + 1, 'a39182c1-7a5f-47c7-9002-03dabdbdefe0', 'janeDoe', GETDATE()),
  (NEWID(), 'Document_5', '/documents/doc5.pdf', FLOOR(RAND() * 4) + 1, 'a39182c1-7a5f-47c7-9002-03dabdbdefe0', 'janeDoe', GETDATE()),
  (NEWID(), 'Document_6', '/documents/doc6.pdf', FLOOR(RAND() * 4) + 1, 'a39182c1-7a5f-47c7-9002-03dabdbdefe0', 'janeDoe', GETDATE()),
  (NEWID(), 'Document_7', '/documents/doc7.pdf', FLOOR(RAND() * 4) + 1, 'a39182c1-7a5f-47c7-9002-03dabdbdefe0', 'janeDoe', GETDATE());

INSERT INTO [fl_db_medical_documents].[dcmt].[Documents] (Id, Name, Path, PriorityLevel, CreatedBy, CreatedName, CreatedAt)
VALUES
  (NEWID(), 'Rapport_Annuel', '/documents/rapport_annuel.pdf', FLOOR(RAND() * 4) + 1, '0e9b4e2d-fd84-4fde-9257-7e323cc6fbb8', 'JohnDoe', GETDATE()),
  (NEWID(), 'Contrat_Client', '/documents/contrat_client.docx', FLOOR(RAND() * 4) + 1, '0e9b4e2d-fd84-4fde-9257-7e323cc6fbb8', 'JohnDoe', GETDATE()),
  (NEWID(), 'Facture_Janvier', '/documents/facture_janvier.xlsx', FLOOR(RAND() * 4) + 1, '0e9b4e2d-fd84-4fde-9257-7e323cc6fbb8', 'JohnDoe', GETDATE()),
  (NEWID(), 'Plan_Projet', '/documents/plan_projet.pptx', FLOOR(RAND() * 4) + 1, '0e9b4e2d-fd84-4fde-9257-7e323cc6fbb8', 'JohnDoe', GETDATE()),
  (NEWID(), 'Compte_Rendu_Réunion', '/documents/compte_rendu.pdf', FLOOR(RAND() * 4) + 1, '0e9b4e2d-fd84-4fde-9257-7e323cc6fbb8', 'JohnDoe', GETDATE()),
  (NEWID(), 'Photo_Événement', '/documents/photo_evenement.jpg', FLOOR(RAND() * 4) + 1, '0e9b4e2d-fd84-4fde-9257-7e323cc6fbb8', 'JohnDoe', GETDATE()),
  (NEWID(), 'Vidéo_Promotionnelle', '/documents/video_promo.mp4', FLOOR(RAND() * 4) + 1, '0e9b4e2d-fd84-4fde-9257-7e323cc6fbb8', 'JohnDoe', GETDATE()),
  (NEWID(), 'Rapport_Financier', '/documents/rapport_financier.pdf', FLOOR(RAND() * 4) + 1, '0e9b4e2d-fd84-4fde-9257-7e323cc6fbb8', 'JohnDoe', GETDATE()),
  (NEWID(), 'Note_Interne', '/documents/note_interne.txt', FLOOR(RAND() * 4) + 1, '0e9b4e2d-fd84-4fde-9257-7e323cc6fbb8', 'JohnDoe', GETDATE()),
  (NEWID(), 'Données_Brut', '/documents/donnees_brut.csv', FLOOR(RAND() * 4) + 1, '0e9b4e2d-fd84-4fde-9257-7e323cc6fbb8', 'JohnDoe', GETDATE());
