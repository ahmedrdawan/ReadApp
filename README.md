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

MyReadsApp.API:
  Controllers:
    - AuthController.cs
    - AuthorController.cs
    - BookController.cs
    - PostController.cs
  DTOs:
    - Account DTOs
    - Author DTOs
    - Book DTOs
    - Post DTOs

MyReadsApp.Core:
  - Entities
  - Services (Interfaces)
  - DTOs (Requests / Responses)
  - Exceptions
  - Generic Repository
  - Common

MyReadsApp.Infrastructure:
  - Data (AppDbContext)
  - Services Implementations
  - Repositories
📑 API Documentation
🔐 Authentication
➡️ POST /api/Auth/Sign-Up
Registers a new user.
