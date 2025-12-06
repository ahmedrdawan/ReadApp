# MyReadsApp API

A complete ASP.NET Core Web API that manages Users, Authentication, Books, Authors, and Posts.
The system supports JWT authentication, and CRUD operations for all main entities.

🚀 Features
🔐 Authentication

Sign Up

Sign In

JWT Token Generation

👤 Authors

Create Author

Get Author

Update Author

Delete Author

📘 Books

Create Book

Get Book

Update Book

Delete Book

📝 Posts

Create Post (User → Book)

Get Post

Update Post

Delete Post

Validation for UserId & BookId

📁 Project Structure
MyReadsApp.API
│── Controllers
│   ├── AuthController.cs
│   ├── AuthorController.cs
│   ├── BookController.cs
│   └── PostController.cs
│
│── DTOs
│   ├── Account DTOs
│   ├── Author DTOs
│   ├── Book DTOs
│   └── Post DTOs
│
MyReadsApp.Core
│── Entities
│── Services (Interfaces)
│── DTOs (Requests / Responses)
│── Exceptions
│── Generic Repository
│── Common
│
MyReadsApp.Infrastructure
│── Data (AppDbContext)
│── Services Implementations
│── Repositories

📑 API Documentation
🔐 Authentication
➡️ POST /api/Auth/Sign-Up

Registers a new user.

Body

{
  "userName": "Ahmed",
  "email": "test@example.com",
  "password": "P@ss1234"
}

➡️ POST /api/Auth/Sign-In

Logs in a user and returns JWT.

Body

{
  "email": "test@example.com",
  "password": "P@ss1234"
}

👤 Authors
➡️ GET /api/Author/{AuthorId}

Returns a single author.

➡️ POST /api/Author

Create a new author.

{
  "authorName": "John Doe",
  "authorImage": "image-url",
  "bio": "Writer biography"
}

➡️ PUT /api/Author/{AuthorId}

Updates author information.

➡️ DELETE /api/Author/{AuthorId}

Deletes an author.

📘 Books
➡️ GET /api/Book/{BookId}

Returns full book data.

➡️ POST /api/Book
{
  "title": "Clean Code",
  "description": "Programming book",
  "content": "PDF or text content",
  "authorId": "guid_here",
  "bookImage": "image-url"
}

➡️ PUT /api/Book/{BookId}

Updates a book.

➡️ DELETE /api/Book/{BookId}

Deletes a book.

📝 Posts

Posts represent a link between User → Book.

➡️ GET /api/Post/{PostId}
➡️ POST /api/Post
{
  "userId": "guid_here",
  "bookId": "guid_here"
}

➡️ PUT /api/Post/{PostId}
➡️ DELETE /api/Post/{PostId}
🛠️ Technologies Used

ASP.NET Core Web API

Entity Framework Core

SQL Server

JWT Authentication

Repository Pattern

Clean Architecture (modular)

⚙️ Setup Instructions

1️⃣ Clone repository

git clone https://github.com/ahmedrdawan/MyReadsApp.git


2️⃣ Update appsettings.json

"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=MyReadsApp;Trusted_Connection=True;"
},
"Jwt": {
  "Key": "YOUR_SECRET_KEY",
  "Issuer": "BookLibraryApi",
  "Audience": "BookLibraryApiUsers"
},
"appURL": "http://localhost:4200"


3️⃣ Run migrations

dotnet ef database update


4️⃣ Run the API

dotnet run