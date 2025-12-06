# 📚 MyReadsApp API

A complete **ASP.NET Core Web API** that manages **Users, Authentication, Books, Authors, and Posts**.  
The system uses **JWT authentication**, **Entity Framework Core**, and provides full **CRUD operations** for all main entities.

---

## 🚀 Features

### 🔐 Authentication
- Sign Up  
- Sign In  
- JWT Token Generation  

### 👤 Authors
- Create Author  
- Get Author  
- Update Author  
- Delete Author  

### 📘 Books
- Create Book  
- Get Book  
- Update Book  
- Delete Book  

### 📝 Posts
Posts represent a link between **User → Book**.

- Create Post  
- Get Post  
- Update Post  
- Delete Post  
- Validates UserId and BookId  

---

## 📁 Project Structure

MyReadsApp.API
│── Controllers
│ ├── AuthController.cs
│ ├── AuthorController.cs
│ ├── BookController.cs
│ └── PostController.cs
│
│── DTOs
│ ├── Account DTOs
│ ├── Author DTOs
│ ├── Book DTOs
│ └── Post DTOs
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

yaml
Copy code

---

## 📑 API Documentation

### 🔐 Authentication

#### ➡️ POST `/api/Auth/Sign-Up`
Registers a new user.

**Body**
```json
{
  "userName": "Ahmed",
  "email": "test@example.com",
  "password": "P@ss1234"
}
➡️ POST /api/Auth/Sign-In
Logs in the user and returns a JWT token.

Body

json
Copy code
{
  "email": "test@example.com",
  "password": "P@ss1234"
}
👤 Authors
➡️ GET /api/Author/{AuthorId}
Returns a single author.

➡️ POST /api/Author
Creates a new author.

Body

json
Copy code
{
  "authorName": "John Doe",
  "authorImage": "image-url",
  "bio": "Writer biography"
}
➡️ PUT /api/Author/{AuthorId}
Updates an author's information.

➡️ DELETE /api/Author/{AuthorId}
Deletes an author.

📘 Books
➡️ GET /api/Book/{BookId}
Returns full book details.

➡️ POST /api/Book
json
Copy code
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
Posts represent a User → Book relation.

➡️ GET /api/Post/{PostId}
➡️ POST /api/Post
json
Copy code
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

Clean Architecture

⚙️ Setup Instructions
1️⃣ Clone the repository
bash
Copy code
git clone https://github.com/ahmedrdawan/ReadsApp.git
2️⃣ Update appsettings.json
json
Copy code
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=MyReadsApp;Trusted_Connection=True;"
},
"Jwt": {
  "Key": "YOUR_SECRET_KEY",
  "Issuer": "BookLibraryApi",
  "Audience": "BookLibraryApiUsers"
},
"appURL": "http://localhost:4200"
3️⃣ Apply migrations
bash
Copy code
dotnet ef database update
4️⃣ Run the API
bash
Copy code
dotnet run
⭐ Contribution
Pull requests are welcome!