
-- 1. Crear el esquema 'Seguros'
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Seguros')
BEGIN
    EXEC('CREATE SCHEMA Seguros;');
END;
GO


IF OBJECT_ID('Seguros.Poliza', 'U') IS NOT NULL
    DROP TABLE Seguros.Poliza;
GO

IF OBJECT_ID('Seguros.StatusPoliza', 'U') IS NOT NULL
    DROP TABLE Seguros.StatusPoliza;
GO

IF OBJECT_ID('Seguros.TipoPoliza', 'U') IS NOT NULL
    DROP TABLE Seguros.TipoPoliza;
GO


CREATE TABLE Seguros.StatusPoliza (
    idStatusPoliza INT PRIMARY KEY IDENTITY(1,1),
    nombreStatus VARCHAR(20) NOT NULL
);
GO


CREATE TABLE Seguros.TipoPoliza (
    idTipoPoliza INT PRIMARY KEY IDENTITY(1,1),
    nombrePoliza VARCHAR(20) NOT NULL
);
GO


CREATE TABLE Seguros.Poliza (
    id INT PRIMARY KEY IDENTITY(1,1),
    idTipoPoliza INT NOT NULL,
    NumeroPoliza INT UNIQUE,
    idUser NVARCHAR(450) NOT NULL, 
    FechaInicio DATETIME NOT NULL,
    FechaFin DATETIME NOT NULL,
    MontoPrima DECIMAL(18, 2) NOT NULL,
    idStatusPoliza INT NOT NULL,
    

    CONSTRAINT FK_Poliza_TipoPoliza FOREIGN KEY (idTipoPoliza) REFERENCES Seguros.TipoPoliza(idTipoPoliza),
    CONSTRAINT FK_Poliza_StatusPoliza FOREIGN KEY (idStatusPoliza) REFERENCES Seguros.StatusPoliza(idStatusPoliza),
    

    CONSTRAINT FK_Poliza_AspNetUsers FOREIGN KEY (idUser) REFERENCES dbo.AspNetUsers(Id)
);
GO


INSERT INTO Seguros.StatusPoliza (nombreStatus)
VALUES 
    ('Cotizada'),
    ('Autorizada'),
    ('Rechazada');
GO

INSERT INTO Seguros.TipoPoliza (nombrePoliza)
VALUES 
    ('Vida'),
    ('Auto'),
    ('Hogar'),
    ('Salud');
GO

CREATE OR ALTER PROCEDURE [Seguros].[usp_GetUsers]
    @user_email NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    -- Declara las variables para el rol y el ID del usuario actual
    DECLARE @current_user_id NVARCHAR(450);
    DECLARE @current_user_role NVARCHAR(256);

    -- Obtener el ID y el rol del usuario que ejecuta el stored procedure
    SELECT
        @current_user_id = U.Id,
        @current_user_role = R.Name
    FROM
        dbo.AspNetUsers U
        INNER JOIN dbo.AspNetUserRoles UR ON U.Id = UR.UserId
        INNER JOIN dbo.AspNetRoles R ON UR.RoleId = R.Id
    WHERE
        U.Email = @user_email;

    -- Validar si el usuario existe
    IF @current_user_id IS NULL
    BEGIN
        SELECT 'Error: El usuario no existe o no tiene un rol asignado.' AS ErrorMessage;
        RETURN;
    END;

    -- Lógica para la recuperación de usuarios según el rol
    IF @current_user_role = 'Admin'
    BEGIN
        -- Si es 'Admin', regresa a todos los usuarios con todos los datos del modelo
        SELECT
            U.Nombre,
            U.ApellidoPaterno,
            U.ApellidoMaterno,
            U.Edad,
            U.PaisNacimiento,
            U.Genero,
            U.Email AS CorreoElectronico,
            U.PhoneNumber AS Telefono,
            R.Name AS Rol
        FROM
            dbo.AspNetUsers U
            INNER JOIN dbo.AspNetUserRoles UR ON U.Id = UR.UserId
            INNER JOIN dbo.AspNetRoles R ON UR.RoleId = R.Id;
    END
    ELSE IF @current_user_role = 'Broker'
    BEGIN
        -- Si es 'Broker', regresa solo a los usuarios con rol 'Cliente'
        SELECT
            U.Nombre,
            U.ApellidoPaterno,
            U.ApellidoMaterno,
            U.Edad,
            U.PaisNacimiento,
            U.Genero,
            U.Email AS CorreoElectronico,
            U.PhoneNumber AS Telefono,
            R.Name AS Rol
        FROM
            dbo.AspNetUsers U
            INNER JOIN dbo.AspNetUserRoles UR ON U.Id = UR.UserId
            INNER JOIN dbo.AspNetRoles R ON UR.RoleId = R.Id
        WHERE
            R.Name = 'Cliente';
    END
    ELSE IF @current_user_role = 'Cliente'
    BEGIN
        -- Si es 'Cliente', regresa solo su propia información
        SELECT
            U.Nombre,
            U.ApellidoPaterno,
            U.ApellidoMaterno,
            U.Edad,
            U.PaisNacimiento,
            U.Genero,
            U.Email AS CorreoElectronico,
            U.PhoneNumber AS Telefono,
            R.Name AS Rol
        FROM
            dbo.AspNetUsers U
            INNER JOIN dbo.AspNetUserRoles UR ON U.Id = UR.UserId
            INNER JOIN dbo.AspNetRoles R ON UR.RoleId = R.Id
        WHERE
            U.Id = @current_user_id;
    END
    ELSE
    BEGIN
        -- Manejo de roles no autorizados
        SELECT 'Error: El rol del usuario no tiene permisos para esta acción.' AS ErrorMessage;
        RETURN;
    END
END;
GO