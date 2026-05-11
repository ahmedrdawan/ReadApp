# Readify — Mobile Integration Guide

This document summarizes the backend API, authentication, common endpoints, request/response examples, and quick steps for mobile developers to integrate the Readify mobile app with the backend.

Base URL (local):

- Run the API from the `MyReadsApp.API` project folder:

```powershell
cd "c:\Users\Elbrine\Desktop\ReadApp\Backend\MyReadsApp\MyReadsApp.API"
dotnet run --project MyReadsApp.API.csproj
```

- Default Swagger & runtime URLs (when running locally):
  - Swagger UI: `https://localhost:5001/swagger` or `http://localhost:5000/swagger`
  - OpenAPI JSON: `https://localhost:5001/swagger/v1/swagger.json`

Authentication
--------------
- The API uses JWT Bearer tokens. Authenticate via the auth endpoints (login / register) and include the token in the `Authorization` header for protected endpoints:

```
Authorization: Bearer <token>
```

- Example login (POST /api/auth/login) returns an access token. Use that token for subsequent authenticated calls.

Key Endpoints (summary)
-----------------------
- Authentication
  - `POST /api/auth/login` — Log in, returns token
  - `POST /api/auth/register` — Create account

- Books
  - `GET /api/book` — Paginated list of books. Query params: `pageNumber`, `pageSize`, `categoryId` (optional)
  - `GET /api/book/{bookId}` — Get single book
  - `GET /api/book/search?q={query}&pageNumber=&pageSize=&categoryId=` — Search books
  - `POST /api/book/{bookId}/rating` — (Auth) Rate or update rating for book. Body: `{ "value": 1..5 }`
  - `GET /api/book/{bookId}/rating` — Get rating summary: average, count, user's rating (if authenticated)

- Categories
  - `GET /api/categories` — List categories (name + optional icon)
  - Note: Create/update category endpoints are planned; if needed now we can implement `POST /api/categories` and `PUT /api/categories/{id}` (admin only).

- Posts & Feed
  - `GET /api/post` — Feed (paginated). Query params: `pageNumber`, `pageSize`
  - `GET /api/Post/{PostId}/Comments?pageNumber=&pageSize=` — Paginated comments for a post
  - Likes: `POST /api/post/{postId}/likes` (Auth) to like, `DELETE /api/post/{postId}/likes` (Auth) to unlike

- User interactions
  - Follow user: `POST /api/UserFollow/{FollowingId}` (Auth)
  - Unfollow: `DELETE /api/UserFollow/{FollowingId}` (Auth)
  - Friend requests: `POST /api/FriendShip/{friendId}` (Auth) — create friend request; other endpoints for accept/block exist

Request & Response Examples
---------------------------
- GET paginated books

Request:

```
GET /api/book?pageNumber=1&pageSize=10
```

Successful response (200, simplified):

```json
{
  "isSuccess": true,
  "statusCode": 200,
  "value": {
    "items": [
      {
        "id": "<book-guid>",
        "title": "Book title",
        "description": "...",
        "bookImage": "https://...",
        "authorId": "<author-guid>",
        "authorName": "Author Full Name"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 123,
    "totalPages": 13
  }
}
```

- POST rating (authenticated)

Request:

```
POST /api/book/{bookId}/rating
Authorization: Bearer <token>
Content-Type: application/json

{ "value": 5 }
```

Response: 200 (success) or 400/404 on error. To fetch rating summary use `GET /api/book/{bookId}/rating`.

How to get OpenAPI / Postman
----------------------------
- Run the API and open the OpenAPI JSON at `/swagger/v1/swagger.json`; import it into Postman (File → Import → Link or raw JSON). This generates a full collection automatically.

- Quick export steps (manual):

```powershell
# Run the API
cd "c:\Users\Elbrine\Desktop\ReadApp\Backend\MyReadsApp\MyReadsApp.API"
dotnet run --project MyReadsApp.API.csproj
# Open http://localhost:5000/swagger/v1/swagger.json in a browser and save JSON
# In Postman: Import -> Upload the saved JSON
```

Security notes
--------------
- Some endpoints require authentication. The `BookController` rating endpoints and user interaction endpoints require a valid JWT.
- Some admin-level endpoints (creating/updating books or categories) should be protected with `Roles = Admin`. Confirm role mappings with your auth provider.

Local setup for mobile developers
---------------------------------
- To run locally and test from the mobile app (emulator or device), either run the API on a host reachable from the device or use a tunneling tool (ngrok) to expose the local server.

Example using ngrok:

```powershell
# Run the API
dotnet run --project MyReadsApp.API.csproj
# In another terminal (ngrok must be installed)
ngrok http 5000
# Use the ngrok HTTPS URL as the base URL in the mobile app
```

Notes & Known Gaps
------------------
- Category create/update endpoints are not implemented yet; GET `/api/categories` is available.
- Some controllers were recently secured with `[Authorize]`. If you see 401 responses, ensure the mobile app includes the `Authorization` header.
- We can export a Postman collection for you after we finalize auth flows (token structure) — let me know if you want me to generate and commit a `Readify.postman_collection.json` file.

Contact / Next steps
--------------------
- If you want, I can:
  - Implement category create/update endpoints (admin-protected).
  - Generate and commit a Postman collection file for the current API surface.
  - Export a trimmed OpenAPI JSON focused on mobile-relevant endpoints.

Files added:

- [Docs/MOBILE_INTEGRATION.md](Docs/MOBILE_INTEGRATION.md)
