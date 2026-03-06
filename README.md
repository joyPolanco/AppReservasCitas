
# Sistema de Reservas de Citas

## Descripción del Proyecto

El **Sistema de Reservas de Citas** es una aplicación diseñada para gestionar turnos de atención en servicios que requieren programación previa, como clínicas, salones de belleza o asesorías profesionales.

El sistema permite a los usuarios consultar horarios disponibles, reservar citas y recibir confirmaciones automáticas. Además, ofrece herramientas administrativas para configurar sesiones, estaciones de atención y controlar la disponibilidad del servicio.

### Características principales

- Generación automática de horarios disponibles.
- Validación para evitar duplicidad de reservas.
- Notificaciones por correo electrónico para confirmar o cancelar citas.
- Registro de acciones para fines de auditoría.
- Gestión de configuraciones del sistema por parte del administrador.

---

# Objetivos

## Objetivo general

Desarrollar un sistema de reservas que integre validaciones de disponibilidad, notificaciones automáticas y registro completo de actividades.

## Objetivos específicos

- Centralizar la gestión de turnos, fechas y estaciones de atención.
- Generar automáticamente los horarios disponibles para cada sesión.
- Permitir a los usuarios reservar citas de forma segura.
- Enviar notificaciones automáticas mediante correo electrónico.
- Registrar acciones del sistema como reservas, modificaciones y cancelaciones.

---

# Reglas de negocio

1. Cada usuario puede tener **una sola cita activa por día**.
2. Las reservas deben realizarse dentro de los **horarios configurados**.
3. Los horarios se **bloquean automáticamente** cuando se llenan los cupos.
4. Cada estación puede atender **una cita por turno**.
5. La duración mínima de las citas es de **5 minutos**.
6. Las estaciones pueden tener **cupos personalizables** según el servicio.
7. Todas las acciones del sistema se registran para **auditoría**.
8. Un día puede tener **varios turnos programados**.

---

# Funcionalidades

## Usuario

- Registrarse
- Ver los horarios disponibles
- Reservar un cupo por día
- Confirmar la reserva
- Cancelar la reserva
- Ver reservas realizadas

## Administrador

- Cambiar configuración (duración de citas, rango de turnos, cupos por estaciones)
- Agendar sesiones o cabinas de atención para uno o varios días
- Ver los turnos habilitados
- Ver los administradores registrados

---

# Flujo básico de la aplicación

## Administrador

- Configura la duración de citas y el tiempo de cada turno.
- Planifica sesiones de atención y define la cantidad de estaciones por sesión.
- El sistema valida la información y genera registros en las entidades correspondientes.

## Usuario

- Visualiza días y horarios disponibles según la configuración del administrador.
- Selecciona un slot disponible.
- Recibe un correo electrónico con un enlace para confirmar o cancelar la reserva.

El enlace de confirmación posee un **tiempo límite definido por el sistema**.

---

# Generación automática de slots

El sistema calcula automáticamente los slots disponibles considerando:

- Duración de las citas
- Cantidad de estaciones
- Turnos configurados

Cada slot queda asociado a:

- un turno
- una estación específica

### Validaciones

- Evita duplicidad de reservas.
- Controla que cada estación no exceda su capacidad máxima.

---

# Confirmación y cancelación

## Confirmación

- Actualiza el estado de la reserva.
- Evita que la reserva sea gestionada automáticamente por el servicio de mantenimiento.

## Cancelación

- Utiliza un **token único enviado por correo electrónico**.
- Libera el slot correspondiente, aumentando la disponibilidad del sistema.

---

# Servicio de mantenimiento (Clean Up)

Servicio automático encargado de:

- Revisar reservas con fechas vencidas.
- Cancelar reservas no confirmadas que superen el tiempo límite.
- Liberar slots ocupados por reservas expiradas.

---

# Arquitectura del sistema

El sistema utiliza **Onion Architecture**, una arquitectura basada en capas que permite separar responsabilidades y mantener el código limpio y escalable.

## Capa de Dominio

Contiene las entidades principales del sistema:

- Usuario
- Estación
- Turno
- DíaHabilitado
- Log
- Reserva
- Role
- Slot
- TokenCancelacion
- Planificacion

También define las interfaces de repositorios y servicios.

---

## Capa de Aplicación

Gestiona los **casos de uso del sistema**.

Incluye:

- Validación de disponibilidad
- Gestión de reservas
- Cancelaciones
- Coordinación de notificaciones
- DTOs para transferencia de información entre capas

---

## Capa de Infraestructura

Gestiona recursos externos y persistencia.

Incluye:

- Entity Framework
- JWT para autenticación
- BCrypt para encriptación de contraseñas
- MailKit para envío de correos
- Clean Up Service para mantenimiento automático
- Sistema de logging

---

## Capa Web

### Frontend

Aplicación desarrollada en **React.js** con componentes interactivos para usuarios y administradores.

### Backend

API desarrollada en **ASP.NET Core**, encargada de manejar:

- autenticación
- planificación
- reservas
- gestión de usuarios

---

# Patrones de diseño utilizados

## Singleton

Garantiza una única instancia del **Logger** para toda la aplicación.

## Adapter

Permite integrar servicios externos (como MailKit) con la interfaz definida en la capa de dominio.

## Repository

Abstrae las operaciones CRUD de cada entidad del sistema.

## Unit of Work

Agrupa múltiples operaciones de base de datos en una sola transacción para mantener consistencia.

## Strategy

Permite cambiar dinámicamente el tipo de registro utilizado por el sistema de logging.

---

# Tecnologías utilizadas

## Frontend

- React.js

## Backend

- ASP.NET Core

## ORM

- Entity Framework

## Correo electrónico

- MailKit

## Seguridad

- JWT
- BCrypt

## Patrones de diseño

- Singleton
- Adapter
- Repository
- Unit of Work
- Strategy

---

# Capturas del sistema

## Vista de usuario

Interfaz para consultar horarios disponibles, reservar citas y gestionar reservas.
<img width="408" height="222" alt="image" src="https://github.com/user-attachments/assets/69e2ce05-9f33-41d9-a3ef-6992bbccbddd" />
<img width="399" height="240" alt="image" src="https://github.com/user-attachments/assets/a1defbbb-3054-4517-a896-019844dee8a6" />
<img width="396" height="208" alt="image" src="https://github.com/user-attachments/assets/be9c695a-6dc4-4138-b643-d3374214f635" />
<img width="410" height="213" alt="image" src="https://github.com/user-attachments/assets/d96ab41e-4d57-4781-b4f1-70984c1c1dfb" />
<img width="402" height="205" alt="image" src="https://github.com/user-attachments/assets/e3feab58-e979-4f95-932a-f5bd198b2449" />


## Vista de administrador

Panel de control para configurar turnos, estaciones de atención y parámetros del sistema.
<img width="405" height="198" alt="image" src="https://github.com/user-attachments/assets/724f466c-56bd-4c54-ab2d-2d56df178347" />
<img width="401" height="209" alt="image" src="https://github.com/user-attachments/assets/36a053cf-920b-4a65-90ca-872d5a7952a0" />
<img width="390" height="194" alt="image" src="https://github.com/user-attachments/assets/2d2365df-7c9c-474a-91ab-aee0b62aa052" />
<img width="407" height="219" alt="image" src="https://github.com/user-attachments/assets/b463833a-bdaf-4847-8175-99a2f7b7333c" />
<img width="400" height="199" alt="image" src="https://github.com/user-attachments/assets/0a7a4f4f-0f5b-4a8d-8fba-48a53defd18c" />
