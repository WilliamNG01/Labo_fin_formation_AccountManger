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
