# Fullstack Developer Test – Patient Management System 🩺

This is a full-stack application developed for a technical interview. It features a secure and modern architecture using **.NET 8**, **React + TypeScript**, **Dapper**, **Ant Design**, and **JWT authentication**.

---

## 🧱 Architecture

- **Backend**: Clean Architecture (.NET 8)
  - API (Controllers, Swagger, Auth)
  - Application (DTOs, Services, Interfaces)
  - Domain (Entities)
  - Infrastructure (Dapper Repositories, Encryption Service)
- **Frontend**: Vite + React + TypeScript + Ant Design
  - Login system with JWT Bearer token
  - CRUD patient management with polished UI

---

## 🔐 Features

- JWT Authentication
- AES-256 encryption for sensitive data (SSN)
- RESTful CRUD operations (Patients)
- Secure login and role-based logic
- Responsive, styled UI with Ant Design
- Clean separation of concerns (SOLID principles)
- Async/Await and modern coding practices
- SQL Server Express LocalDB for development

---

## 🧪 Technologies Used

| Layer    | Stack                                            |
| -------- | ------------------------------------------------ |
| Backend  | .NET 8, C#, Dapper, JWT, AES, SQL Server Express |
| Frontend | React 18, TypeScript, Vite, Ant Design, Axios    |
| Tooling  | Swagger, VS Code / Visual Studio, Postman        |

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [SQL Server Express LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)
- Node.js + npm
- Visual Studio or VS Code

### Backend Setup

```bash
cd Backend/PatientManagement.Api
dotnet build
dotnet run
```

> Ensure `appsettings.json` has a valid DB connection and encryption key.

### Database

- Run `Database/CreateSchema.sql` in your local SQL Server instance.

### Frontend Setup

```bash
cd Frontend
npm install
npm run dev
```

---

## 🔑 Default Login

```
Email: admin@demo.com
Password: 123456
```

## 📡 Key Endpoints

| Method | Route                    | Auth      | Description                 |
| ------ | ------------------------ | --------- | --------------------------- |
| POST   | `/api/auth/login`      | ❌ Public | Get JWT token               |
| POST   | `/api/auth/register`   | ❌ Public | Register a new user         |
| GET    | `/api/patients`        | ✅ Token  | Get all patients            |
| GET    | `/api/patients/{id}`   | ✅ Token  | Get patient by ID           |
| GET    | `/api/patients/search` | ✅ Token  | Filtered/paged patient list |
| POST   | `/api/patients`        | ✅ Token  | Create a patient            |
| PUT    | `/api/patients/{id}`   | ✅ Token  | Update patient              |
| DELETE | `/api/patients/{id}`   | ✅ Token  | Delete patient              |

## ✅ Completed Requirements

- CRUD API in .NET without Entity Framework
- Secure data encryption
- Validation, authentication and authorization
- Full React UI with login and protected access
- Snake_case DB conventions

---

## 📂 Project Structure├── Backend/

```
│   ├── PatientManagement.API
|   ├──├── Application
│   ├── Domain
│   ├── Infrastructure
├── Frontend/
│   ├── pages/
│   ├── services/
├── README.md
└── .gitignore

```

---

## 🙌 Author

Created with ❤️ by Joel Liranzo for the Fullstack Developer Test.
