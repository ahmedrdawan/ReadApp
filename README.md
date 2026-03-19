# 📚 MyReadsApp API

MyReadsApp is a layered **ASP.NET Core 8 Web API** for a social reading platform. It supports authentication, role-based authorization, book/author management, posting, comments, likes, favorites, shelves (user books), following, friendships, and Google sign-in.

---

## ✨ Features

- **Authentication & Identity**
  - Sign up / sign in
  - JWT token authentication
  - Email confirmation flow
  - Role-based access (`Admin`, `User`)
- **Catalog Management**
  - Author CRUD
  - Book CRUD
- **Social Features**
  - Posts tied to books
  - Comments on posts
  - Likes on posts
  - Follow / unfollow users
  - Friendship requests and filtering by status
- **Personal Library**
  - Favorite books
  - User book shelf/status management
- **Infrastructure**
  - EF Core with SQL Server
  - Centralized exception middleware
  - Swagger/OpenAPI in Development

---

## 🧱 Solution Structure

```text
Backend/MyReadsApp/
├── MyReadsApp.API           # Controllers, API DTOs, middleware, startup
├── MyReadsApp.Core          # Entities, service interfaces, enums, exceptions, common response models
├── MyReadsApp.Infstructure  # EF Core DbContext, DI wiring, service implementations, migrations, seeders
└── MyReadsApp.sln
```

---

## ⚙️ Tech Stack

- .NET 8 / ASP.NET Core Web API
- Entity Framework Core 8
- SQL Server
- ASP.NET Core Identity
- JWT Bearer Authentication
- SendGrid (email service)
- Swagger (Swashbuckle)

---

## 📋 Requirements

- .NET SDK 8.0+
- SQL Server instance
- (Optional) Redis instance for distributed caching (`localhost:6379` by default)
- (Optional) SendGrid API key for email confirmation delivery
- (Optional) Google OAuth credentials for external login

---

## 🔧 Configuration

Main configuration lives in:

- `Backend/MyReadsApp/MyReadsApp.API/appsettings.json`
- `Backend/MyReadsApp/MyReadsApp.API/appsettings.Development.json`

### Required keys

```json
{
  "ConnectionStrings": {
    "default": "<sql-server-connection-string>",
    "redis": "localhost:6379"
  },
  "JwtSettings": {
    "Key": "<strong-random-key>",
    "Issuer": "BookLibrary",
    "Audience": "BookLibraryClient",
    "ExpiryInMinutes": 30,
    "RefreshTokenExpiration": 30
  },
  "BaseAppSetting": {
    "appURL": "http://localhost:<frontend-port>/"
  },
  "GridSetting": {
    "SenderGridApiKey": "<sendgrid-api-key>",
    "SenderEmail": "<verified-sender-email>"
  },
  "Authentication": {
    "Google": {
      "ClientId": "<google-client-id>",
      "ClientSecret": "<google-client-secret>"
    }
  }
}
```

> ⚠️ Do not commit production secrets. Prefer environment variables or user-secrets for sensitive values.

---

## 🚀 Getting Started

From the repo root:

```bash
cd Backend/MyReadsApp
```

### 1) Restore packages

```bash
dotnet restore MyReadsApp.sln
```

### 2) Apply migrations

```bash
dotnet ef database update --project MyReadsApp.Infstructure --startup-project MyReadsApp.API
```

### 3) Run API

```bash
dotnet run --project MyReadsApp.API
```

Swagger UI is available in Development at `/swagger` (for example, `https://localhost:7097/swagger`).

---

## 🔐 Authentication & Authorization

- JWT authentication is registered in infrastructure DI.
- Request pipeline includes both:
  - `UseAuthentication()`
  - `UseAuthorization()`
- Roles seeded: `Admin`, `User`.
- Several controllers enforce role restrictions with `[Authorize(Roles = "...")]`.

### Seeded Admin User

On startup, seeders create roles and an admin account:

- Email: `admin1@gmail.com`
- Password: `Admin123@3`
- Role: `Admin`

> Change seeded credentials before any non-local environment deployment.

---

## 📘 API Endpoints (Current Routes)

Base host example: `https://localhost:<port>`

### Auth (`/api/Auth`)

- `POST /api/Auth/Sign-Up`
- `POST /api/Auth/Sign-In`
- `POST /api/Auth/refresh-token`
- `GET /api/Auth/google-login`
- `GET /api/Auth/google-callback`
- `GET /api/Auth/confirm-email?userId={userId}&token={token}`

### Author (`/api/Author`) *(Admin)*

- `GET /api/Author/{AuthorId}`
- `POST /api/Author`
- `PUT /api/Author/{AuthorId}`
- `DELETE /api/Author/{AuthorId}`

### Book (`/api/Book`) *(Admin)*

- `GET /api/Book/{BookId}`
- `POST /api/Book`
- `PUT /api/Book/{BookId}`
- `DELETE /api/Book/{BookId}`

### Post (`/api/Post`) *(User)*

- `GET /api/Post/{PostId}`
- `POST /api/Post`
- `PUT /api/Post/{PostId}`
- `DELETE /api/Post/{PostId}`

### Comment (`/api`) *(User)*

- `GET /api/Post/Comment/{CommentId}`
- `POST /api/Post/{PostId}/Comment`
- `PUT /api/Post/{PostId}/Comment/{CommentId}`
- `DELETE /api/Post/Comment/{CommentId}`

### Like (`/api/post`)

- `GET /api/post/{postId}/likes`
- `POST /api/post/{postId}/likes`
- `DELETE /api/post/{postId}/likes`

### Favorite Books (`/api/Faviorates`) *(User)*

- `POST /api/Faviorates/{BookId}`
- `DELETE /api/Faviorates/{BookId}`
- `GET /api/Faviorates/{BookId}`

### User Books (`/api/UserBook`) *(User)*

- `POST /api/UserBook/{BookId}`
- `PUT /api/UserBook/{UserBookId}`
- `DELETE /api/UserBook/{UserBookId}`
- `GET /api/UserBook/{UserBookId}`
- `GET /api/UserBook`

### User Follow (`/api/UserFollow`)

- `POST /api/UserFollow/{FollowingId}`
- `DELETE /api/UserFollow/{FollowingId}`
- `GET /api/UserFollow/followers`
- `GET /api/UserFollow/following`

### Friendship (`/api/FriendShip`)

- `POST /api/FriendShip/{friendId}`
- `DELETE /api/FriendShip/{friendId}`
- `GET /api/FriendShip/accepted`
- `GET /api/FriendShip/bloked`
- `GET /api/FriendShip/pending`

> Route casing and some path names reflect the current implementation (including legacy typos), to match actual runtime routes.

---

## 🧰 Common Response Pattern

Services use shared response wrappers in `MyReadsApp.Core/Common/Response.cs`:

- `Response`
- `Response<T>`

Typical fields:

- `IsSuccess`
- `StatusCode`
- `Error` / `Errors`
- `Value` (generic only)

---

## 🧯 Error Handling

Global middleware: `ExceptionHandeler`

Handled custom exceptions include:

- `NotFoundException` → 404
- `ConfilectException` → 409
- `NotAuthorizeException` → 401
- fallback `Exception` → 500

---

## 🗃️ Database & Migrations

Migrations are stored in:

- `Backend/MyReadsApp/MyReadsApp.Infstructure/Migrations`

Create a new migration:

```bash
dotnet ef migrations add <MigrationName> --project MyReadsApp.Infstructure --startup-project MyReadsApp.API
```

Update database:

```bash
dotnet ef database update --project MyReadsApp.Infstructure --startup-project MyReadsApp.API
```

---

## 🧪 Testing

There is currently no dedicated test project in the repository.

Recommended next steps:

1. Add unit tests for service layer logic.
2. Add integration tests for auth + protected endpoints.
3. Add CI pipeline for restore/build/test.

---

## 🤝 Contributing

1. Create a feature branch.
2. Keep changes scoped and small.
3. Verify formatting/build/tests locally.
4. Submit a PR with summary + validation steps.

---

## 📝 Notes

- The project currently uses some legacy naming/typos in class and route names (`Infstructure`, `Faviorate`, etc.). Documentation intentionally mirrors real code/routes.
- If you want a cleanup pass, do it in a dedicated refactor PR to avoid breaking API consumers unexpectedly.
