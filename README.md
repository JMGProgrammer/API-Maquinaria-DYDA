# API Maquinaria — DYDA

API REST de maquinaria construida con **ASP.NET Core** y **Entity Framework Core** sobre **MySQL**. Proyecto personal para llevar registro de las clases (port del proyecto del profe).

## Stack

- .NET 10 / ASP.NET Core Web API
- Entity Framework Core (migración inicial: `Init`)
- MySQL (instalación local o Docker)
- Patrón en capas: Controllers → Services (reglas de negocio) → Repositories (datos)
  con interfaces (`IEquipmentService`, `IEquipmentRepository`) en el medio.
- Flujo de datos: entrada por `EquipmentCreateDTO` / `EquipmentUpdateDTO`
  (heredan de `EquipmentInputDTO`), salida por `EquipmentReadDTO`. El modelo
  `Equipment` vive solo entre Service y Repository.

## Estructura

```
API-Maquinaria-juan/
├── API-Maquinaria-juan/        # Proyecto principal
│   ├── Controllers/            # EquipmentController (rutas + try/catch → status codes)
│   ├── Interfaces/             # Contratos (di) de service y repository
│   ├── Services/               # EquipmentService (Validar, ToEntity, ToReadDto)
│   ├── Repositories/           # EquipmentRepository (EF Core contra MySQL)
│   ├── DTOs/                   # EquipmentInputDTO (base), Create/Update/Read
│   ├── Data/                   # DbContext (EF Core)
│   ├── Models/                 # Equipment
│   ├── Migrations/             # Migraciones EF (Init)
│   └── Program.cs              # Registro de dependencias (AddScoped)
├── Docs/                       # Documentación de estudio y decisiones
└── API-Maquinaria-juan.sln
```

## Requisitos

- .NET SDK 10
- MySQL corriendo en `localhost:3306`
- `.NET tool` de EF si vas a crear migraciones o aplicar la DB: `dotnet tool install --global dotnet-ef`

## Cómo levantar el proyecto desde cero (clone, secrets, DB)

Todo lo que es máquina-local no viaja con el repo: en `appsettings.json` la
`DefaultConnection` está **vacía** a propósito y la DB se crea aparte. Si un paso
se salta, los síntomas son: connection string vacía → 500; DB sin crear → 500
al primer GET ("tabla inexistente").

```bash
# 0. Clonar y restaurar
git clone https://github.com/JMGProgrammer/API-Maquinaria-DYDA.git
cd API-Maquinaria-DYDA

# 1. Setear TU cadena de conexión (user-secrets: queda FUERA del repo)
cd API-Maquinaria-juan
dotnet user-secrets set "ConnectionStrings:DefaultConnection" \
  "Server=localhost;Port=3306;Database=maquinaria_api;User=root;Password=TU_PASSWORD;"

# 2. Crear la base de datos desde la migración que sí está en el repo
dotnet ef database update

# 3. Ejecutar
dotnet run
```

> `dotnet user-secrets` guarda el valor en `~/.microsoft/usersecrets/`, asociado
> al `UserSecretsId` del `.csproj`: el clon nunca trae la contraseña de nadie;
> cada máquina tiene la suya.

## Ejecución

```bash
dotnet run --project API-Maquinaria-juan
```

URLs que define `Properties/launchSettings.json`:

- `https://localhost:7221` y `http://localhost:5244` (local)
- `http://0.0.0.0:5244` — binding a toda la LAN: se puede probar desde otro
  equipo o desde una app móvil (Expo) con `http://<IP-de-tu-pc>:5244/api/equipment`
- Documentación OpenAPI (solo en Development): `http://localhost:5244/openapi/v1.json`

## Migraciones

```bash
# Crear una nueva migración
dotnet ef migrations add NombreDeLaMigracion --project API-Maquinaria-juan

# Aplicar migraciones a la base
dotnet ef database update --project API-Maquinaria-juan
```

## Endpoints

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/equipment` | Lista todo el equipo |
| GET | `/api/equipment/{id}` | Obtiene uno por id |
| POST | `/api/equipment` | Crea un equipo |
| PUT | `/api/equipment/{id}` | Actualiza un equipo |
| DELETE | `/api/equipment/{id}` | Elimina un equipo |

### Status codes que devuelve

| Código | Cuándo |
|---|---|
| 200 | GET/PUT/DELETE OK |
| 201 | POST OK (header `Location` al nuevo recurso) |
| 400 | Datos inválidos: `Validar()` del service lanza `ArgumentException` y el controller la convierte en `BadRequest(ex.Message)` |
| 404 | El `id` no existe (service devuelve `null` → controller `NotFound`) |

> Nota: el chequeo de código duplicado existe pero está **retirado** del código
> (no visto en clase). Está documentado completo en
> [`Docs/ChequeoCodigoDuplicado.md`](Docs/ChequeoCodigoDuplicado.md), incluido
> cómo reactivarlo.

## Comentarios de estudio

El código está comentado con la teoría de cada concepto nuevo que se usa:
qué es una interface y para qué funciona la inyección de dependencias
(`Interfaces/`, `Program.cs`), try & catch y el reparto service/controller
(`EquipmentController`), y los mapeadores DTO ↔ entidad con las reglas de
qué campo entra/sale (`EquipmentService`, `DTOs/`).
