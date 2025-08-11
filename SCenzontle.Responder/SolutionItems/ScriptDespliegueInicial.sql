
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


CREATE OR ALTER PROCEDURE [Seguros].[usp_CargaCatalogosUnificado]
    @user_email NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    -- Declara la variable para el rol del usuario actual
    DECLARE @current_user_role NVARCHAR(256);

    -- Obtener el rol del usuario actual que solicita los datos
    SELECT @current_user_role = R.Name
    FROM dbo.AspNetUsers U
    INNER JOIN dbo.AspNetUserRoles UR ON U.Id = UR.UserId
    INNER JOIN dbo.AspNetRoles R ON UR.RoleId = R.Id
    WHERE U.Email = @user_email;

    -- Validar si el usuario existe y tiene permisos para acceder a esta información
    IF @current_user_role IS NULL
    BEGIN
        SELECT 'Error: El usuario no existe o no tiene un rol asignado.' AS ErrorMessage;
        RETURN;
    END

    -- Unificar las tres consultas en un único conjunto de resultados
    -- 1. Obtener la lista de Status de Póliza
    SELECT
        'statusPoliza' AS tipoCatalogo,
        CAST(idStatusPoliza AS NVARCHAR(MAX)) AS id,
        nombreStatus AS [Desc]
    FROM
        Seguros.StatusPoliza

    UNION ALL

    -- 2. Obtener la lista de Tipos de Póliza
    SELECT
        'tipoPoliza' AS tipoCatalogo,
        CAST(idTipoPoliza AS NVARCHAR(MAX)) AS id,
        nombrePoliza AS [Desc]
    FROM
        Seguros.TipoPoliza

    UNION ALL

    -- 3. Obtener la lista de usuarios con rol 'Cliente'
    -- Esta parte solo devuelve resultados si el rol del usuario actual es 'Admin' o 'Broker'
    SELECT
        'emailUserRolClient' AS tipoCatalogo,
        U.Email AS id,
        U.Email AS [Desc]
    FROM
        dbo.AspNetUsers U
    INNER JOIN
        dbo.AspNetUserRoles UR ON U.Id = UR.UserId
    INNER JOIN
        dbo.AspNetRoles R ON UR.RoleId = R.Id
    WHERE
        R.Name = 'Cliente'
        AND @current_user_role IN ('Admin', 'Broker');
END;
GO

CREATE OR ALTER PROCEDURE [Seguros].[usp_InsertPoliza]
    @emailCliente NVARCHAR(256),
    @numeroPoliza NVARCHAR(50),
    @idTipoPoliza INT,
    @idStatusPoliza INT,
    @fechaInicio DATETIME,
    @fechaFin DATETIME,
    @costo DECIMAL(18, 2)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Declara una variable para almacenar el ID del cliente
        DECLARE @idCliente NVARCHAR(450);

        -- Obtener el Id del cliente a partir de su correo electrónico
        SELECT @idCliente = Id
        FROM dbo.AspNetUsers
        WHERE Email = @emailCliente;

        -- Validar si el cliente existe
        IF @idCliente IS NULL
        BEGIN
            RAISERROR ('Error: El cliente con el correo electrónico proporcionado no existe.', 16, 1);
            RETURN;
        END

        -- Insertar los datos en la tabla de Póliza
        INSERT INTO [Seguros].[Poliza] (
            idCliente,
            numeroPoliza,
            idTipoPoliza,
            idStatusPoliza,
            fechaInicio,
            fechaFin,
            costo
        )
        VALUES (
            @idCliente,
            @numeroPoliza,
            @idTipoPoliza,
            @idStatusPoliza,
            @fechaInicio,
            @fechaFin,
            @costo
        );

        -- Devolver el ID de la nueva póliza insertada
        SELECT SCOPE_IDENTITY() AS NuevaPolizaId;

    END TRY
    BEGIN CATCH
        -- En caso de error, devolver un mensaje descriptivo
        DECLARE @ErrorMessage NVARCHAR(MAX), @ErrorSeverity INT, @ErrorState INT;
        SELECT
            @ErrorMessage = ERROR_MESSAGE(),
            @ErrorSeverity = ERROR_SEVERITY(),
            @ErrorState = ERROR_STATE();

        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO

