# API Maquinaria — DYDA

API REST de maquinaria construida con **ASP.NET Core** y **Entity Framework Core** sobre **MySQL**. Proyecto personal para llevar registro de las clases (port del proyecto del profe).

## Stack

- .NET 10 / ASP.NET Core Web API
- Entity Framework Core (migración inicial: `Init`)
- MySQL (base de datos `maquinaria_api`, Docker o instalación local)
- Patrón: Controllers → DbContext (EF) con DTOs para entrada

## Estructura

```
API-Maquinaria-juan/
├── API-Maquinaria-juan/        # Proyecto principal
│   ├── Controllers/            # EquipmentController (CRUD)
│   ├── DTOs/                   # EquipmentInput
│   ├── Data/                   # DbContext (EF Core)
│   ├── Models/                 # Equipment
│   ├── Migrations/             # Migraciones EF (Init)
│   └── Program.cs
└── API-Maquinaria-juan.sln
```

## Requisitos

- .NET SDK 10
- MySQL corriendo en `localhost:3306`
- `.NET tool` de EF si vas a crear migraciones: `dotnet tool install --global dotnet-ef`

## Configuración (secrets)

La cadena de conexión **no** vive en `appsettings.json` — se guarda con *User Secrets* para no versionar la contraseña:

```bash
cd API-Maquinaria-juan
dotnet user-secrets set "ConnectionStrings:DefaultConnection" \
  "Server=localhost;Port=3306;Database=maquinaria_api;User=root;Password=TU_PASSWORD;"
```

## Ejecución

```bash
dotnet run --project API-Maquinaria-juan
```

Swagger queda disponible en la ruta que define `launchSettings.json` (por defecto `https://localhost:xxxx/swagger`).

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

*(Verificar rutas exactas en `Controllers/EquipmentController.cs`.)*
