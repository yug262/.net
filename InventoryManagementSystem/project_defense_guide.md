# 🎓 Inventory Management System: Complete Architecture Guide

This guide is designed specifically for your college project defense (Viva). It explains exactly how everything in your Inventory Management System works, from the database up to the user interface.

## 🏗️ 1. The Big Picture (Architecture)
Your project is built using a **Three-Tier Architecture**, which is the industry standard for modern web applications.

1. **The Database (PostgreSQL)**: Where all your data (users, categories, products) is permanently stored.
2. **The Backend (ASP.NET Core Web API)**: The "brain" of your app. It talks to the database, enforces business rules, and serves data over the internet securely using APIs.
3. **The Frontend (Angular 19)**: The user interface. It runs in the user's web browser, takes their input, and displays data dynamically by calling the backend APIs.

---

## ⚙️ 2. The Backend (C# & ASP.NET Core)
Located in the `backend/` folder. This project uses the **Repository Pattern** mapped with Entity Framework.

### Key Folders & Files:
*   **`Models/`**: These files ([Category.cs](file:///D:/.NET/InventoryManagementSystem/backend/Models/Category.cs), [Product.cs](file:///D:/.NET/InventoryManagementSystem/backend/Models/Product.cs), [User.cs](file:///D:/.NET/InventoryManagementSystem/backend/Models/User.cs)) map exactly to the tables in your PostgreSQL database.
*   **[Data/AppDbContext.cs](file:///D:/.NET/InventoryManagementSystem/backend/Data/AppDbContext.cs)**: This is Entity Framework Core's magic file. It bridges the C# code to the actual SQL database. It knows how to translate C# objects into SQL `INSERT`, `UPDATE`, and `SELECT` queries.
*   **`Controllers/`**: These files define your **API Endpoints**. Imagine them as different customer service counters:
    *   [AuthController.cs](file:///D:/.NET/InventoryManagementSystem/backend/Controllers/AuthController.cs): Handles `/api/auth/login`. (Takes a username/password, returns a secure JWT token).
    *   [ProductsController.cs](file:///D:/.NET/InventoryManagementSystem/backend/Controllers/ProductsController.cs): Handles all `/api/products` requests (Create, Read, Update, Delete).
    *   [DashboardController.cs](file:///D:/.NET/InventoryManagementSystem/backend/Controllers/DashboardController.cs): Handles `/api/dashboard`. This runs special SQL queries to count things (e.g., "How many products are low on stock?").
*   **`Services/`**: This is where your actual business logic lives. The Controllers ask the Services to do the heavy lifting.
    *   *Example*: [ProductService.cs](file:///D:/.NET/InventoryManagementSystem/backend/Services/ProductService.cs) contains the actual code that checks the database for a specific product, modifies its quantity, and saves it.
*   **[Program.cs](file:///D:/.NET/InventoryManagementSystem/backend/Program.cs)**: The starting point of the backend. It wires everything together: connecting the database, setting up Cross-Origin Resource Sharing (CORS) so your Angular app is allowed to talk to it, and configuring JWT Security.

### How Data Moves (Example: Getting all products):
1. **Frontend** asks `GET /api/products`.
2. **[ProductsController.cs](file:///D:/.NET/InventoryManagementSystem/backend/Controllers/ProductsController.cs)** receives the request.
3. Controller asks **[ProductService.cs](file:///D:/.NET/InventoryManagementSystem/backend/Services/ProductService.cs)** for the data.
4. Service asks **[AppDbContext.cs](file:///D:/.NET/InventoryManagementSystem/backend/Data/AppDbContext.cs)** to fetch it.
5. EF Core translates that into a SQL `SELECT * FROM Products`.
6. The data travels back up the chain and is sent to the Frontend as raw JSON text.

---

## 🎨 3. The Frontend (TypeScript & Angular)
Located in the `frontend/` folder. It is built using Angular, a framework by Google for building single-page applications.

### Key Folders & Files:
*   **`src/app/models/`**: These are TypeScript interfaces (e.g., [product.model.ts](file:///D:/.NET/InventoryManagementSystem/frontend/src/app/models/product.model.ts)). They tell the frontend exactly what shape the JSON data from the backend will take.
*   **`src/app/services/`**: Every backend controller has a matching frontend service. 
    *   *Example*: [product.service.ts](file:///D:/.NET/InventoryManagementSystem/frontend/src/app/services/product.service.ts) uses Angular's `HttpClient` to make actual network requests to `http://localhost:5160/api/products`.
*   **`src/app/pages/`**: These are the physical screens the user sees. Every page is a "Component" made of two main files:
    *   **[.html](file:///D:/.NET/InventoryManagementSystem/frontend/src/app/app.html)**: The structure and design (buttons, tables).
    *   **[.ts](file:///D:/.NET/InventoryManagementSystem/frontend/src/main.ts)**: The brain of that page. It calls the services and holds the data variables.
*   **[src/app/core/interceptors/auth.interceptor.ts](file:///D:/.NET/InventoryManagementSystem/frontend/src/app/core/interceptors/auth.interceptor.ts)**: This is a security guard file. Whenever the frontend makes *any* API call, this file automatically intercepts it, grabs the user's login token from local storage, and attaches it. Without this, the backend would block all requests.
*   **[src/styles.css](file:///D:/.NET/InventoryManagementSystem/frontend/src/styles.css)**: The global design system. This holds all the custom design classes (like `.btn-primary-custom` and `.premium-card`) that make the app look like a modern dashboard instead of a basic high school project.
*   **[proxy.conf.json](file:///D:/.NET/InventoryManagementSystem/frontend/proxy.conf.json)**: This solves the Dev Tunnel issue. Since Angular and .NET run on different ports, Angular uses this proxy to pretend the backend is running locally within Angular, preventing mobile phones from getting confused about where `localhost` is.

---

## 🔐 4. How the Login System Works
When a teacher asks: *"How did you secure this app?"*

1. The user types `admin` and `admin123` into the Angular Login form.
2. Angular sends a `POST` request to the backend [AuthController](file:///D:/.NET/InventoryManagementSystem/backend/Controllers/AuthController.cs#13-17).
3. The backend checks the database to see if `admin` exists and verifies the password.
4. If successful, the backend generates a **JSON Web Token (JWT)**. This is a cryptographically signed digital VIP pass.
5. The backend sends the JWT back to Angular.
6. Angular saves this JWT secretly inside the browser's `localStorage`.
7. For every subsequent request (e.g., trying to read Products), Angular attaches the JWT to the HTTP Header. The backend verifies the signature. If it's valid, the data is returned.

---

## 🚀 5. How to Answer "What did you learn?"
If they ask what the hardest part was or what you learned, use these speaking points:
*   **Separation of Concerns**: "I learned how to keep my database logic separated from my UI logic by using APIs and Services."
*   **Entity Framework Migration**: "I learned a Code-First approach. I wrote C# classes, and EF Core automatically built the PostgreSQL tables for me."
*   **State Management & Reactive Forms**: "On the frontend, I learned how to bind HTML inputs directly to TypeScript variables so the UI updates instantly without reloading the page."
*   **Cors & Proxies**: "I learned how to manage network requests across different ports using CORS in .NET and an API Proxy in Angular."
