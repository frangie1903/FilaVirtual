# FilaVirtual

**Aplicación móvil para la gestión y seguimiento de turnos virtuales mediante .NET MAUI**

> Proyecto de la asignatura *Programación de Aplicaciones Móviles*

| | |
|---|---|
| **Plataforma** | Android / iOS (.NET MAUI) |
| **Lenguaje** | C# 12 · XAML |
| **Arquitectura** | MVVM en la app · API REST en capas |
| **Datos** | SQL Server (servidor) · SQLite (caché local en el teléfono) |

---

## Tabla de contenido

1. [Resumen](#1-resumen)
2. [Problemática y justificación](#2-problemática-y-justificación)
3. [Objetivos](#3-objetivos)
4. [Alcance](#4-alcance)
5. [Usuarios y roles](#5-usuarios-y-roles)
6. [Requisitos](#6-requisitos)
7. [Funcionamiento del sistema](#7-funcionamiento-del-sistema)
8. [Modelo de datos](#8-modelo-de-datos)
9. [Arquitectura y tecnologías](#9-arquitectura-y-tecnologías)
10. [API REST](#10-api-rest)
11. [Estructura del repositorio](#11-estructura-del-repositorio)
12. [Cómo ejecutar el proyecto](#12-cómo-ejecutar-el-proyecto)
13. [Plan de desarrollo](#13-plan-de-desarrollo)
14. [Limitaciones conocidas y mejoras futuras](#14-limitaciones-conocidas-y-mejoras-futuras)

---

## 1. Resumen

**FilaVirtual** permite que una persona tome un turno desde su teléfono en lugar de esperar físicamente en una fila. El usuario elige un establecimiento, selecciona el servicio que necesita, obtiene un turno digital y sigue su posición y el tiempo estimado en tiempo real. El establecimiento, por su parte, cuenta con un panel para controlar la fila, llamar al siguiente turno y consultar estadísticas de atención.

```text
Usuario ──► Establecimiento ──► Servicio ──► Fila ──► Turno  (C-038, 5 personas delante, ~15 min)
```

## 2. Problemática y justificación

En muchos lugares de atención al público, la espera ocurre de pie y sin información. Esto genera:

- pérdida de tiempo y acumulación de personas;
- filas desorganizadas y sin control de quién fue atendido;
- imposibilidad de saber cuánto falta;
- pérdida del turno si la persona se aleja unos minutos.

**FilaVirtual** traslada la espera a un sistema digital: el usuario sabe su turno, consulta su posición desde cualquier lugar y recibe avisos cuando se acerca su atención. Para el establecimiento, el sistema ordena la atención y deja registro para medir la demanda.

Desde el punto de vista académico, el proyecto integra: interfaces y navegación móvil, MVVM, almacenamiento local, consumo de servicios REST, autenticación con roles, códigos QR y notificaciones.

## 3. Objetivos

**Objetivo general.** Desarrollar una aplicación móvil con .NET MAUI que gestione filas y turnos virtuales, facilitando a los usuarios obtener y seguir su turno y dando a los establecimientos herramientas para administrar la atención.

**Objetivos específicos**

| # | Objetivo | Módulo |
|---|---|---|
| 1 | Registro e inicio de sesión de usuarios | Autenticación |
| 2 | Consultar establecimientos disponibles | Establecimientos |
| 3 | Mostrar los servicios de cada establecimiento | Servicios |
| 4 | Obtener un turno virtual | Turnos |
| 5 | Mostrar la posición dentro de la fila | Turnos |
| 6 | Mostrar un tiempo aproximado de espera | Turnos |
| 7 | Cancelar un turno | Turnos |
| 8 | Consultar el historial de turnos | Turnos |
| 9 | Permitir a los establecimientos administrar sus filas | Operador |
| 10 | Llamar, completar o cancelar turnos | Operador |
| 11 | Notificar cambios en el estado del turno | Notificaciones |
| 12 | Registrar datos para estadísticas de atención | Estadísticas |

## 4. Alcance

| Incluye (versión 1) | No incluye inicialmente |
|---|---|
| Aplicación móvil MAUI | Pagos |
| Registro, inicio de sesión y roles | Integración con bancos reales |
| Establecimientos, servicios, filas y turnos | Información médica real |
| Seguimiento del turno (posición y tiempo estimado) | Inteligencia artificial |
| Panel del operador | Reconocimiento facial |
| Código QR y notificaciones | Integración con sistemas gubernamentales |
| Historial y estadísticas básicas | Gestión de múltiples países |
| API REST y base de datos | |

## 5. Usuarios y roles

| Rol | Descripción | Funciones principales |
|---|---|---|
| **Usuario** | Persona que necesita atención | Registrarse, iniciar sesión, consultar establecimientos y servicios, tomar y cancelar turnos, ver su posición, recibir notificaciones, consultar su historial |
| **Operador** | Empleado que gestiona la fila de **su** establecimiento | Ver turnos pendientes, llamar al siguiente, marcar atendido o ausente, cancelar turnos, abrir/cerrar la fila |
| **Administrador** | Gestiona todo el sistema | Administrar establecimientos, servicios y operadores; consultar estadísticas; configurar parámetros |

## 6. Requisitos

### 6.1 Requisitos funcionales

| Código | Requisito |
|---|---|
| RF-01 | El sistema permitirá registrar usuarios y autenticarlos con correo y contraseña. |
| RF-02 | El sistema restringirá las funciones según el rol (Usuario, Operador, Administrador). |
| RF-03 | El usuario podrá consultar y buscar establecimientos activos. |
| RF-04 | El usuario podrá ver los servicios de un establecimiento y el estado de su fila (personas esperando, tiempo estimado). |
| RF-05 | El usuario podrá tomar un turno; recibirá un código con el formato `<Prefijo>-<Número>` (ej. `C-038`). |
| RF-06 | El usuario podrá ver su posición y tiempo estimado, y cancelar su turno mientras esté en espera o llamado. |
| RF-07 | El operador podrá llamar al siguiente turno y marcar turnos como atendidos, ausentes o cancelados. |
| RF-08 | El operador podrá abrir y cerrar la fila del día de un servicio. |
| RF-09 | El sistema notificará al usuario cuando su turno se cree, esté próximo, sea llamado o sea cancelado. |
| RF-10 | El usuario podrá acceder a una fila escaneando el código QR del servicio. |
| RF-11 | El sistema mostrará estadísticas del día: creados, atendidos, cancelados, no presentados, tiempo promedio y horas de mayor demanda. |
| RF-12 | El usuario podrá consultar su historial, incluso sin conexión (caché local). |

### 6.2 Requisitos no funcionales

| Código | Requisito |
|---|---|
| RNF-01 | **Seguridad:** las contraseñas se almacenan con hash (BCrypt); la API usa tokens JWT y valida el rol en cada endpoint. |
| RNF-02 | **Integridad:** no existen dos turnos con el mismo número en una fila ni un usuario con dos turnos activos a la vez (restricciones únicas en la base de datos). |
| RNF-03 | **Usabilidad:** flujo principal en pocos toques (establecimiento → servicio → tomar turno). |
| RNF-04 | **Disponibilidad parcial:** establecimientos, servicios e historial se pueden consultar sin conexión gracias a SQLite. |
| RNF-05 | **Mantenibilidad:** separación en capas (Models / Views / ViewModels / Services en la app; Controllers / Services / Data en la API). |
| RNF-06 | **Transparencia:** el tiempo de espera se muestra siempre como *estimación*, no como garantía. |

## 7. Funcionamiento del sistema

### 7.1 Flujo del usuario

```text
Iniciar sesión → Buscar establecimiento → Elegir servicio → Consultar fila
      → Tomar turno → (esperar; seguimiento y avisos) → Turno llamado → Atención → Completado → Historial
```

**Pantallas:** `Login` → `Inicio` (establecimientos) → `Servicios` → `Mi turno` · `Historial` · `Panel del operador`

**Ejemplo.** Un usuario elige *Clínica Central → Consulta general*. La app muestra «Personas esperando: 7 · Tiempo estimado: 28 min». Al presionar **TOMAR TURNO** recibe `C-038`. Luego recibe «Tu turno está próximo. Tienes 2 personas delante.» y finalmente «Es tu turno. Dirígete al área de atención.».

### 7.2 Estados de un turno

```text
ESPERANDO ──► LLAMADO ──► ATENDIENDO ──► COMPLETADO
    │            │             │
    ├────────────┴─────────────┴──► CANCELADO
    └────────────► NO_PRESENTADO   (desde ESPERANDO o LLAMADO)
```

| Acción | Quién | Estado de origen | Estado resultante |
|---|---|---|---|
| Tomar turno | Usuario | — | ESPERANDO |
| Llamar siguiente | Operador | ESPERANDO (menor número) | LLAMADO |
| Atender | Operador | LLAMADO | ATENDIENDO |
| Completar | Operador | LLAMADO o ATENDIENDO | COMPLETADO |
| Marcar ausente | Operador | ESPERANDO o LLAMADO | NO_PRESENTADO |
| Cancelar | Usuario | ESPERANDO o LLAMADO | CANCELADO |
| Cancelar | Operador | Cualquier estado activo | CANCELADO |

### 7.3 Reglas de negocio

- Existe **una fila por servicio y por día**; se crea automáticamente con el primer turno del día.
- El número de turno es consecutivo dentro de la fila; el código usa el prefijo del servicio.
- Un usuario solo puede tener **un turno activo** (esperando, llamado o atendiendo).
- No se pueden tomar turnos si el establecimiento o el servicio están inactivos, ni si la fila está cerrada.
- Un operador solo gestiona los servicios de su establecimiento; el administrador gestiona todos.

### 7.4 Tiempo estimado

```text
personas delante × tiempo promedio de atención ÷ operadores activos   (redondeado hacia arriba)

Ejemplo: 6 personas × 4 min ÷ 1 operador = 24 minutos
```

> El tiempo mostrado es una **estimación** y no representa un tiempo garantizado de atención. La posición y el tiempo se **calculan** en cada consulta; no se almacenan porque cambian constantemente.

### 7.5 Código QR

Cada servicio puede publicar un QR con el contenido `filavirtual://servicio/{id}`. Al escanearlo, la app consulta `GET /api/servicios/{id}/fila` y lleva al usuario directamente a la fila, sin buscar el establecimiento.

```text
Escanear QR → Servicio → Fila disponible → Obtener turno
```

### 7.6 Notificaciones

| Evento | Mensaje de ejemplo |
|---|---|
| Turno creado | «Tu turno es C-038. Personas delante: 5.» |
| Turno próximo (≤ 2 delante) | «Tu turno está próximo. Tienes 2 personas delante.» |
| Turno llamado | «Es tu turno. Dirígete al área de atención.» |
| Turno cancelado / no presentado | «Tu turno fue cancelado.» |

En la versión 1 son **notificaciones locales**: la app consulta la API cada 10 segundos mientras «Mi turno» está visible y avisa al detectar cambios. Los avisos con la app cerrada requieren notificaciones push (ver sección 14).

### 7.7 Estadísticas

```text
ESTADÍSTICAS DEL DÍA
Turnos creados       82        Tiempo promedio
Atendidos            64        de atención          7 min
Cancelados            9
No presentados        9        + distribución de turnos por hora (horas de mayor demanda)
```

## 8. Modelo de datos

```text
Usuario ──1:N── Turno ──N:1── Fila ──N:1── Servicio ──N:1── Establecimiento
                                                                  ▲
                                         Usuario (operador) ──────┘ (EstablecimientoId, opcional)
```

**USUARIOS** — `Id`, `Nombre`, `Correo` (único), `PasswordHash` (BCrypt), `Rol` (Usuario/Operador/Administrador), `Activo`, `EstablecimientoId` (solo operadores)

**ESTABLECIMIENTOS** — `Id`, `Nombre`, `Descripcion`, `Direccion`, `Horario`, `Activo`

**SERVICIOS** — `Id`, `EstablecimientoId`, `Nombre`, `Descripcion`, `Prefijo` (letra del código), `TiempoPromedio` (min), `OperadoresActivos`, `Activo`

**FILAS** — `Id`, `ServicioId`, `Nombre`, `Abierta`, `Fecha` · *índice único (ServicioId, Fecha)*

**TURNOS** — `Id`, `FilaId`, `UsuarioId`, `Numero`, `Codigo`, `Estado`, `FechaCreacion`, `HoraLlamado`, `HoraAtencion`, `HoraFinalizacion` · *índice único (FilaId, Numero) e índice único filtrado: un turno activo por usuario*

> **Cambios respecto al diseño inicial:** `Password` pasó a `PasswordHash` (nunca se guarda la contraseña); `Posicion` y `TiempoEstimado` dejaron de ser columnas porque son datos derivados que se calculan al consultar; se añadieron `Prefijo` y `OperadoresActivos` (necesarios para el código y el cálculo del tiempo) y `EstablecimientoId` en usuarios (para limitar al operador a su sede).

El script completo está en [`db/filavirtual.sql`](db/filavirtual.sql).

## 9. Arquitectura y tecnologías

```text
 ┌──────────────────────────┐      HTTP / REST + JWT      ┌──────────────────────┐      ┌────────────┐
 │  App móvil (.NET MAUI)   │ ──────────────────────────► │  ASP.NET Core Web API │ ───► │ SQL Server │
 │  MVVM · SQLite (caché)   │ ◄────────────────────────── │  EF Core · BCrypt     │      └────────────┘
 └──────────────────────────┘                             └──────────────────────┘
```

| Capa | Tecnología |
|---|---|
| App móvil | .NET 8 MAUI, C#, XAML, Shell (navegación) |
| Patrón | MVVM con `CommunityToolkit.Mvvm` |
| Almacenamiento local | SQLite (`sqlite-net-pcl`) para caché; `SecureStorage` para la sesión |
| Notificaciones | `Plugin.LocalNotification` (locales) |
| Backend | ASP.NET Core 8 Web API, Entity Framework Core |
| Base de datos | SQL Server |
| Seguridad | JWT (Bearer), hash BCrypt, autorización por rol |
| Documentación de la API | Swagger / OpenAPI |

## 10. API REST

Todas las rutas requieren `Authorization: Bearer <token>` salvo las de `auth`.

| Método | Ruta | Rol | Descripción |
|---|---|---|---|
| POST | `/api/auth/registro` | Público | Crear cuenta de usuario |
| POST | `/api/auth/login` | Público | Iniciar sesión (devuelve el token) |
| POST | `/api/auth/recuperar` | Público | Recuperar contraseña *(pendiente: requiere correo)* |
| GET | `/api/establecimientos` | Autenticado | Listar establecimientos activos |
| GET | `/api/establecimientos/{id}` | Autenticado | Detalle |
| GET | `/api/establecimientos/{id}/servicios` | Autenticado | Servicios de un establecimiento |
| POST / PUT | `/api/establecimientos[/{id}]` | Administrador | Crear / editar establecimiento |
| GET | `/api/servicios/{id}/fila` | Autenticado | Personas esperando y tiempo estimado |
| POST / PUT | `/api/servicios[/{id}]` | Administrador | Crear / editar servicio |
| POST | `/api/turnos` | Usuario | Tomar turno `{ "servicioId": 1 }` |
| GET | `/api/turnos/mio` | Usuario | Turno activo (204 si no tiene) |
| GET | `/api/turnos/historial` | Usuario | Últimos 100 turnos |
| POST | `/api/turnos/{id}/cancelar` | Usuario | Cancelar turno propio |
| GET | `/api/operador/servicios` | Operador / Admin | Servicios que puede gestionar |
| GET | `/api/operador/servicios/{id}/fila` | Operador / Admin | Panel: siguiente, en proceso, pendientes, atendidos |
| POST | `/api/operador/servicios/{id}/llamar` | Operador / Admin | Llamar al siguiente turno |
| PUT | `/api/operador/servicios/{id}/estado?abierta=true` | Operador / Admin | Abrir o cerrar la fila del día |
| POST | `/api/operador/turnos/{id}/{accion}` | Operador / Admin | `atender`, `completar`, `ausente` o `cancelar` |
| GET | `/api/estadisticas/dia?establecimientoId=1` | Operador / Admin | Estadísticas del día |

Los errores devuelven `{ "mensaje": "..." }` con el código HTTP correspondiente (400, 401, 403, 404, 409).

## 11. Estructura del repositorio

```text
FilaVirtual-Solucion/
├── README.md
├── db/
│   └── filavirtual.sql                  # Script de SQL Server
├── api/FilaVirtual.Api/                 # ASP.NET Core Web API
│   ├── Controllers/                     # Auth, Establecimientos, Servicios, Turnos, Operador, Estadisticas
│   ├── Entities/  Dtos/  Data/          # Modelo, contratos, DbContext y datos iniciales
│   ├── Services/TurnoQueries.cs         # Fila del día, tiempo estimado, mapeo a DTO
│   ├── Program.cs  appsettings.json
└── app/FilaVirtual/                     # Archivos de la app MAUI (se copian sobre un proyecto MAUI nuevo)
    ├── Models/  Services/  Data/
    ├── ViewModels/                      # Login, Inicio, Servicios, Turno, Historial, Operador
    ├── Views/                           # Páginas XAML
    ├── AppShell.xaml(.cs)  App.xaml.cs  MauiProgram.cs
    └── SETUP_PAQUETES.txt
```

## 12. Cómo ejecutar el proyecto

**Requisitos:** .NET 8 SDK, workload MAUI (`dotnet workload install maui`), SQL Server o LocalDB, Visual Studio 2022 (o VS Code con la extensión de .NET MAUI), emulador Android o dispositivo.

### 12.1 Base de datos (elige **una** opción)

- **A. Script SQL:** ejecutar `db/filavirtual.sql` en SQL Server Management Studio.
- **B. Migraciones de EF:** desde `api/FilaVirtual.Api` ejecutar `dotnet ef migrations add Inicial` y `dotnet ef database update`.

### 12.2 API

```bash
cd api/FilaVirtual.Api
# Ajustar ConnectionStrings:Default y Jwt:Key en appsettings.json
dotnet run
```

Swagger queda disponible en `http://localhost:5080/swagger` (entorno Development). Al iniciar, la API crea los usuarios de prueba si no existen:

| Rol | Correo | Contraseña |
|---|---|---|
| Administrador | `admin@filavirtual.com` | `Admin123!` |
| Operador (Clínica Central) | `operador@filavirtual.com` | `Operador123!` |

> Son credenciales **solo para desarrollo**: cámbialas y usa una `Jwt:Key` propia y secreta antes de cualquier despliegue.

### 12.3 App móvil

```bash
dotnet new maui -n FilaVirtual
cd FilaVirtual
# 1. Copiar el contenido de app/FilaVirtual/ sobre el proyecto (reemplazar MauiProgram.cs, App.xaml.cs, AppShell.*)
#    y eliminar MainPage.xaml(.cs) de la plantilla.
# 2. Instalar los paquetes indicados en SETUP_PAQUETES.txt
# 3. Revisar la URL de la API en Services/ApiConfig.cs
dotnet build -t:Run -f net8.0-android
```

En el emulador Android, `10.0.2.2` apunta al `localhost` de la PC. En un teléfono físico usa la IP de tu PC en la red local y permite HTTP solo durante el desarrollo (ver `SETUP_PAQUETES.txt`).

### 12.4 Prueba rápida del flujo completo

1. Registrar un usuario en la app y tomar un turno en *Clínica Central → Consulta general*.
2. En otra instancia (o emulador) iniciar sesión como operador y pulsar **LLAMAR**.
3. En la app del usuario, «Mi turno» cambia a **LLAMADO** y aparece la notificación.
4. El operador pulsa **Completar**; el turno aparece como COMPLETADO en el historial del usuario.

## 13. Plan de desarrollo

**MVP (primera entrega):** autenticación + establecimientos + servicios + filas + turnos + seguimiento + panel del operador + SQLite/API.

**Segunda fase:** código QR, notificaciones, estadísticas, GPS.

| Fase | Contenido | Estado |
|---|---|---|
| 1 | Modelo de datos, API de autenticación y catálogo | Base entregada |
| 2 | Turnos, seguimiento y panel del operador | Base entregada |
| 3 | Notificaciones locales y estadísticas | Base entregada (estadísticas: solo API) |
| 4 | Escaneo de QR, pantalla de estadísticas, administración desde la app | Pendiente |
| 5 | Pruebas, ajustes de interfaz y presentación | Pendiente |

## 14. Limitaciones conocidas y mejoras futuras

**Limitaciones de la versión 1**

- El seguimiento usa consultas periódicas (*polling*); no es tiempo real puro.
- Las notificaciones son locales: no llegan si la app está cerrada.
- La recuperación de contraseña está definida pero requiere un servicio de correo.
- El administrador gestiona establecimientos y servicios por API (Swagger); aún no hay pantallas para ello en la app.
- Las horas se guardan en la hora local del servidor.

**Mejoras futuras**

- Notificaciones push (Firebase Cloud Messaging / APNs) y actualización en tiempo real con SignalR.
- GPS y mapa de establecimientos.
- Calificación del servicio.
- Predicción más precisa del tiempo de espera usando el historial real.
- Panel web administrativo y dashboard avanzado.
- Soporte para múltiples sucursales y widgets móviles.
- Análisis de horarios de mayor demanda.

---

*FilaVirtual — proyecto académico de Programación de Aplicaciones Móviles.*
