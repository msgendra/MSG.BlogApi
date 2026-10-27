# MSG.BlogApi

Backend Coding Challenge: RESTful API for managing a simple blogging platform.

Tech Stack: **.NET 10**, **ASP.NET Core**, **EF Core + PostgreSQL**, **Serilog** and **Docker**.

## Endpoints

| Method | Route                       | Description                                    | Responses       |
|--------|-----------------------------|------------------------------------------------|-----------------|
| GET    | `/api/posts`                | List all posts with their comment count        | 200             |
| POST   | `/api/posts`                | Create a new post                              | 201, 400        |
| GET    | `/api/posts/{id}`           | Get a post with its content and comments       | 200, 404        |
| POST   | `/api/posts/{id}/comments`  | Add a comment to a post                        | 201, 400, 404   |
| GET    | `/health`                   | Health check (includes database connectivity)  | 200, 503        |

Errors are returned as [RFC 9457 Problem Details](https://www.rfc-editor.org/rfc/rfc9457).

## Project structure

```
src/
  MSG.BlogApi.Domain/          Entities (BlogPost, Comment). No dependencies.
  MSG.BlogApi.Application/     Use cases (BlogPostService), DTOs, repository contract.
  MSG.BlogApi.Infrastructure/  EF Core DbContext, mappings, migrations, repository.
  MSG.BlogApi.Api/             Controllers, Serilog, Swagger.
tests/
  MSG.BlogApi.Application.Tests/  Unit tests for the application layer (xUnit + Moq).
```

## Running the application

### Option 1 — everything in Docker

```bash
docker compose up -d --build
```

- API: http://localhost:5080
- Swagger UI: http://localhost:5080/swagger
- PostgreSQL: `localhost:5433` (user `blog`, password `blog`, database `blogdb`)

Database migrations are applied automatically on startup.

To stop: `docker compose down` (add `-v` to also delete the database volume).

### Option 2 — database in Docker, API with `dotnet run`

```bash
docker compose up -d db
dotnet run --project src/MSG.BlogApi.Api --launch-profile http
```

- API: http://localhost:5086
- Swagger UI: http://localhost:5086/swagger

The development connection string is in `src/MSG.BlogApi.Api/appsettings.Development.json`.
In any other environment, set it via the `ConnectionStrings__BlogDb` environment variable.

## Running the tests

```bash
dotnet test
```

## Logging

Serilog writes to the console and to rolling daily files (7 days retained):

- `dotnet run`: `src/MSG.BlogApi.Api/logs/blogapi-YYYYMMDD.log`
- Docker: `/app/logs` inside the `api` container (`docker compose exec api ls logs`)

Levels and sinks are configured in the `Serilog` section of `appsettings.json`.

## Next steps

With more time I would add:

- **Pagination** (and sorting) on `GET /api/posts`.
- **Authentication and authorization** (JWT), so comments and posts have a real author.
- **Update and delete** endpoints for posts and comments.
- **Caching** for the post list.

