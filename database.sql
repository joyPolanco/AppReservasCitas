
CREATE database SistemaReservas
USE SistemaReservas;


-- Tabla de roles
CREATE TABLE Roles (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(50) NOT NULL,
    Descripcion NVARCHAR(255)
);
--importante insertar
Insert into Roles (Nombre, Descripcion) VALUES
('admin', 'Realiza configuraciones y planificaciones'),
('usuario', 'Realiza reservas');


-- Tabla de usuarios con referencia a Roles
CREATE TABLE Usuarios (
    Id INT PRIMARY KEY IDENTITY(1,1),
    PublicId UNIQUEIDENTIFIER NOT NULL UNIQUE DEFAULT NEWID(),
    Nombre NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    HashContrasena NVARCHAR(255) NOT NULL,
    IdRol INT NOT NULL,
    FOREIGN KEY (IdRol) REFERENCES Roles(Id)
);
INSERT INTO Usuarios (Nombre, Email,HashContrasena,IdRol)
values ('admin','a@gmail.com','$2a$12$4mC7TRaZa3FcJ0rXwUThl.CQZhjH/rjSFmVU4jJ25P/Md9QJiinLK',1);

GO

CREATE TABLE Planificacion (
    PlanificacionId INT IDENTITY PRIMARY KEY,
    FechaRegistro DATE DEFAULT GETDATE(),
	IdUsuario INT NOT NULL FOREIGN KEY REFERENCES Usuarios(Id)
);

CREATE TABLE DiaHabilitado (
    DiaHabilitadoId INT IDENTITY PRIMARY KEY,
    PlanificacionId INT NOT NULL,
    Fecha DATE NOT NULL,
    CONSTRAINT FK_DiaHabilitado_Planificacion FOREIGN KEY (PlanificacionId) REFERENCES Planificacion(PlanificacionId),
    CONSTRAINT UQ_DiaHabilitado_Fecha UNIQUE (PlanificacionId, Fecha) -- evita días duplicados para una planificación
);

CREATE TABLE Estacion (
    EstacionId INT IDENTITY PRIMARY KEY,
    DiaHabilitadoId INT NOT NULL,
    Nombre VARCHAR(100) ,
    CONSTRAINT FK_Estacion_DiaHabilitado FOREIGN KEY (DiaHabilitadoId) REFERENCES DiaHabilitado(DiaHabilitadoId)
);

CREATE TABLE Turno (
    TurnoId INT IDENTITY PRIMARY KEY,
    IdDiaHabilitado INT NOT NULL,
    Nombre VARCHAR(50) NOT NULL, -- Ejemplo: 'Matutino', 'Vespertino', 'Nocturno'
    HoraInicio TIME NOT NULL,
    HoraFin TIME NOT NULL,
	FechaCreacion DATE DEFAULT GETDATE()
    CONSTRAINT FK_Turno_Estacion FOREIGN KEY (IdDiaHabilitado) REFERENCES DiaHabilitado(DiaHabilitadoId)
);
CREATE TABLE Slot (
    SlotId INT IDENTITY PRIMARY KEY,
    PublicId UNIQUEIDENTIFIER NOT NULL UNIQUE DEFAULT NEWID(), -- identificador público
    IdDiaHabilitado INT NOT NULL references DiaHabilitado( DiaHabilitadoId),
    HoraInicio TIME NOT NULL,
    HoraFin TIME NOT NULL,
    CapacidadMaxima INT NOT NULL,
    ReservasActuales INT NOT NULL DEFAULT 0,
    CONSTRAINT CK_Reservas CHECK (ReservasActuales <= CapacidadMaxima)
);

CREATE TABLE Log (
    LogId INT IDENTITY PRIMARY KEY,
    FechaHora DATETIME NOT NULL DEFAULT GETDATE(),
    Tipo VARCHAR(50) NOT NULL,  -- Ejemplo: 'Error', 'Accion', 'Info'
    UsuarioId INT NULL,          -- Opcional, si aplica
    Mensaje NVARCHAR(MAX) NOT NULL
);

CREATE TABLE Reserva (
    ReservaId INT IDENTITY PRIMARY KEY,
    UsuarioId INT NOT NULL FOREIGN KEY REFERENCES Usuarios(Id),
    SlotId INT NOT NULL FOREIGN KEY REFERENCES Slot(SlotId),
    FechaReserva DATETIME NOT NULL DEFAULT GETDATE(),
    Estado VARCHAR(20) NOT NULL CHECK (Estado IN ('Pendiente','Confirmada','Cancelada')) DEFAULT 'Pediente',
    FechaExpiracion DATETIME NULL,
    IdEstacion INT REFERENCES Estacion(EstacionId)
);
CREATE TABLE TokenCancelacion (
    Id INT PRIMARY KEY IDENTITY,
    ReservaId INT NOT NULL,
    Token UNIQUEIDENTIFIER NOT NULL,
    FechaCreacion DATETIME DEFAULT GETDATE() null,
    Usado BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (ReservaId) REFERENCES Reserva(ReservaId)
);



CREATE TABLE AppConfiguracion (
    Id INT PRIMARY KEY IDENTITY(1,1),
    CupoEstaciones INT NOT NULL,
    HoraInicioMatutino TIME NOT NULL,
    HoraFinMatutino TIME NOT NULL,
    HoraInicioVespertino TIME NOT NULL,
    HoraFinVespertino TIME NOT NULL,
    HoraInicioNocturno TIME NOT NULL,
    HoraFinNocturno TIME NOT NULL,
    DuracionCitas INT NOT NULL
);
--configuracion Generica (importante insertarla)
INSERT INTO AppConfiguracion (
    CupoEstaciones,
    HoraInicioMatutino,
    HoraFinMatutino,
    HoraInicioVespertino,
    HoraFinVespertino,
    HoraInicioNocturno,
    HoraFinNocturno,
    DuracionCitas
)
VALUES (
    10,                -- Cupo de estaciones
    '08:00',           -- Inicio turno matutino
    '12:00',           -- Fin turno matutino
    '13:00',           -- Inicio turno vespertino
    '17:00',           -- Fin turno vespertino
    '18:00',           -- Inicio turno nocturno
    '21:00',           -- Fin turno nocturno
    30                 -- Duración de cada cita en minutos
);

