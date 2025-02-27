-- 1. Créer la base de données fl_db_identity
IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = 'fl_db_identity')
BEGIN
    CREATE DATABASE fl_db_identity;
END
GO

-- 2. Se connecter à la base
USE fl_db_identity;
GO

-- 3. Créer un schéma spécifique pour les tables Identity
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'Idtty')
BEGIN
    EXEC('CREATE SCHEMA [Idtty] AUTHORIZATION dbo');
	PRINT 'Schema "Idtty" created.';
END
GO

-- 3.1. Créer un schéma spécifique pour les tables, functions, procedures stoquées, vues, ... BackOffice
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'BackOffice')
BEGIN
    EXEC('CREATE SCHEMA [BackOffice] AUTHORIZATION dbo');
	PRINT 'Schema "BackOffice" created.';
END
GO

-- 4. Créer les logins SQL Server pour les accès
-- a.1. Login pour l'application Identity (Back office)
IF NOT EXISTS (SELECT * FROM sys.syslogins WHERE [name] = N'IdttyBackOffice ')
BEGIN
    CREATE LOGIN IdttyBackOffice  WITH PASSWORD = '$(AccesAppOffice)';
    PRINT 'LOGIN IdttyBackOffice created.';
END
GO

-- a.2. Login pour l'application Identity (accès standard)
IF NOT EXISTS (SELECT * FROM sys.syslogins WHERE [name] = N'IdttyAppUser')
BEGIN
    CREATE LOGIN IdttyAppUser WITH PASSWORD = '$(AccesAppUser)';
END
GO

-- b. Login pour l'administration (accès complet)
IF NOT EXISTS (SELECT * FROM sys.syslogins WHERE [name] = N'IdttyAdmin')
BEGIN
    CREATE LOGIN IdttyAdmin WITH PASSWORD = '$(AccesGold)';
END
GO

-- c. Login en lecture seule (par exemple pour des outils de reporting)
IF NOT EXISTS (SELECT * FROM sys.syslogins WHERE [name] = N'IdttyReadOnly')
BEGIN
    CREATE LOGIN IdttyReadOnly WITH PASSWORD = '$(AccessReport)!';
END
GO

-- 5. Créer les utilisateurs associés dans la base fl_db_identity
USE fl_db_identity;
GO

-- a.1. Utilisateur pour l'application
IF NOT EXISTS (SELECT * FROM sys.sysusers WHERE [name] = N'IdentityBackOffice')
BEGIN
    CREATE USER IdentityBackOffice FOR LOGIN IdttyBackOffice;
END
GO
-- a.2. Utilisateur pour l'application
IF NOT EXISTS (SELECT * FROM sys.sysusers WHERE [name] = N'IdentityAppUser')
BEGIN
    CREATE USER IdentityAppUser FOR LOGIN IdttyAppUser;
END
GO

-- b. Utilisateur pour l'administration
IF NOT EXISTS (SELECT * FROM sys.sysusers WHERE [name] = N'IdentityAdmin')
BEGIN
    CREATE USER IdentityAdmin FOR LOGIN IdttyAdmin;
END
GO

-- c. Utilisateur en lecture seule
IF NOT EXISTS (SELECT * FROM sys.sysusers WHERE [name] = N'IdentityReadOnly')
BEGIN
    CREATE USER IdentityReadOnly FOR LOGIN IdttyReadOnly;
END
GO

-- 6. Sécuriser l'accès aux autres schémas si nécessaire
-- (Restreindre IdentityAppUser à accéder uniquement au schéma Identity)
DENY SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo TO IdentityAppUser;
GO

ALTER ROLE db_datareader ADD MEMBER IdentityAppUser;
ALTER ROLE db_datawriter ADD MEMBER IdentityAppUser;

ALTER ROLE db_datareader ADD MEMBER IdentityBackOffice;
ALTER ROLE db_datawriter ADD MEMBER IdentityBackOffice;

-- Donner accès aux vues système pour gestion
GRANT VIEW DEFINITION TO IdentityBackOffice;
-- Accès spécifique aux schémas de gestion (ex : logs, audits)
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::BackOffice TO IdentityBackOffice;

ALTER ROLE db_owner ADD MEMBER IdentityAdmin;
ALTER ROLE db_datareader ADD MEMBER IdentityReadOnly;

-- 7. Vérifier les utilisateurs et leurs rôles
SELECT dp.name AS UserName, dp.type_desc AS UserType, 
       rl.name AS RoleName
FROM sys.database_principals dp
LEFT JOIN sys.database_role_members drm ON dp.principal_id = drm.member_principal_id
LEFT JOIN sys.database_principals rl ON drm.role_principal_id = rl.principal_id
WHERE dp.type IN ('S', 'U', 'G')
ORDER BY dp.name;
GO

