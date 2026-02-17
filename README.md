# GameStore API (.NET 10 Minimal API)

A simple game catalog API built with ASP.NET Core Minimal APIs and Entity Framework Core (SQLite).

## Features

- CRUD endpoints for games
- Read endpoint for genres
- SQLite persistence with EF Core migrations
- Automatic database migration at startup
- Initial genre seeding

## Tech Stack

- ASP.NET Core (`net10.0`)
- Entity Framework Core 10
- SQLite

## Project Structure

- `GameStore.slnx`
- `GameStore.Api/`
  - `Program.cs` (app startup)
  - `Endpoints/` (Minimal API route maps)
  - `Data/` (DbContext, migrations, seeding)
  - `Models/` (entities)
  - `Dtos/` (request/response contracts)
  - `Games.http` (sample HTTP requests)

## Prerequisites

- .NET 10 SDK (preview)

## Getting Started

From repo root:

```bash
cd GameStore.Api
dotnet restore
dotnet run
```

Default URL (Development):

- `http://localhost:5045`

> On startup, the app runs `db.Database.Migrate()` and applies pending migrations automatically.

## Database

Connection string (`GameStore.Api/appsettings.json`):

```json
"ConnectionStrings": {
  "GameStore": "Data Source=GameStore.db"
}
```

Seeded genres are inserted automatically the first time the database is created/migrated.

## API Endpoints

### Games

- `GET /games` - List all games
- `GET /games/{id}` - Get one game by ID
- `POST /games` - Create a game
- `PUT /games/{id}` - Update a game
- `DELETE /games/{id}` - Delete a game

### Genres

- `GET /genres` - List all genres

You can run sample requests from:

- `GameStore.Api/Games.http`

## Example Request Body (Create/Update Game)

```json
{
  "name": "Elden Ring",
  "genreId": 1,
  "price": 59.99,
  "releaseDate": "2022-02-25"
}
```

## EF Core Migrations

Run from `GameStore.Api/`:

```bash
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

If you hit `PendingModelChangesWarning`, create a new migration to match model changes, then update the database.

## Notes

- `GameStore.db` is created in `GameStore.Api/` (based on current configuration).
- Logging for SQL commands is set to `Warning` in `appsettings.json`.
