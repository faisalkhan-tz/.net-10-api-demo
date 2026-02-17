# GameStore API (.NET 10 Minimal API)

A simple game catalog API built with ASP.NET Core Minimal APIs and Entity Framework Core (SQLite).

## Features

- **JWT Bearer Token Authentication** - Secure user authentication with JWT tokens
- **Authorization** - Games are connected to their owners; users can only manage their own games
- CRUD endpoints for games
- Read endpoint for genres
- SQLite persistence with EF Core migrations
- Automatic database migration at startup
- Initial genre seeding

## Tech Stack

- ASP.NET Core (`net10.0`)
- Entity Framework Core 10
- SQLite
- JWT Bearer Authentication
- BCrypt for password hashing

## Project Structure

- `GameStore.slnx`
- `GameStore.Api/`
  - `Program.cs` (app startup)
  - `Endpoints/` (Minimal API route maps)
  - `Data/` (DbContext, migrations, seeding)
  - `Models/` (entities)
  - `Dtos/` (request/response contracts)
  - `Services/` (JWT token generation)
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

## Authentication

The API uses JWT Bearer token authentication. Before accessing game endpoints, you must register or login to obtain a token.

### Authentication Endpoints

- `POST /auth/register` - Register a new user
- `POST /auth/login` - Login with existing user

### Example: Register

```json
POST /auth/register
Content-Type: application/json

{
  "username": "johndoe",
  "password": "securepassword123"
}
```

Response:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "johndoe"
}
```

### Using the Token

Include the token in the `Authorization` header for all game endpoints:

```
Authorization: Bearer YOUR_TOKEN_HERE
```

## API Endpoints

### Authentication

- `POST /auth/register` - Register a new user
- `POST /auth/login` - Login with existing user

### Games (Requires Authentication)

All game endpoints require authentication. Users can only access and manage their own games.

- `GET /games` - List all games owned by the authenticated user
- `GET /games/{id}` - Get one game by ID (only if owned by user)
- `POST /games` - Create a game (automatically assigned to authenticated user)
- `PUT /games/{id}` - Update a game (only if owned by user)
- `DELETE /games/{id}` - Delete a game (only if owned by user)

### Genres

- `GET /genres` - List all genres (no authentication required)

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

## Security Notes

- JWT secret key is configured in `appsettings.json` - **change this in production**
- Passwords are hashed using BCrypt before storage
- Each user can only access and modify their own games
- Game ownership is enforced at the API level

## Notes

- `GameStore.db` is created in `GameStore.Api/` (based on current configuration).
- Logging for SQL commands is set to `Warning` in `appsettings.json`.
