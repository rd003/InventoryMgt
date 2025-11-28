# Inventory Management System

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Angular](https://img.shields.io/badge/Angular-20-DD0031?logo=angular)](https://angular.io/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker&logoColor=white)](https://www.docker.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

A full-stack **Inventory Management System** built with Angular and .NET Core APIs, featuring comprehensive book inventory tracking, sales and purchase management, supplier management, and user authentication.

## 📑 Table of Contents

- [✨ Features](#-features)
- [🛠️ Tech Stack](#-tech-stack)
- [📜 Project History](#-project-history)
- [🚀 Quick Start with Docker](#-quick-start-with-docker)
- [💻 Development Setup](#-development-setup)
  - [A. Backend](#a-backend)
  - [B. Frontend](#b-frontend)
- [🔧 Entity Framework Migrations](#-entity-framework-migrations)
- [📸 Screenshots](#-screenshots)
- [🤝 Contributing](#-contributing)
- [📄 License](#-license)
- [⭐ Show Your Support](#-show-your-support)

## ✨ Features

- **📚 Book Management**: Add, edit, delete, and search books with detailed information
- **📊 Inventory Tracking**: Real-time inventory monitoring and stock level management
- **💰 Sales Management**: Record and track sales transactions with PDF report generation
- **🛒 Purchase Management**: Manage purchase orders and supplier transactions with PDF export
- **🏢 Supplier Management**: Maintain supplier information and relationships
- **👤 User Authentication**: Secure login system with role-based access control
- **📄 PDF Reports**: Generate detailed PDF reports for sales and purchase records
- **🎨 Modern UI**: Clean and responsive interface built with Angular Material
- **🔍 Advanced Search**: Filter and search across all entities
- **📱 Responsive Design**: Works seamlessly on desktop and mobile devices

## 🛠️ Tech Stack

- **Backend:** ASP.NET Core Web API 10.0
- **Database:** PostgreSQL 16 (migrated from SQL Server 2022)
- **ORM:** Dapper (primary) + Entity Framework Core (for migrations)
- **Frontend:** Angular 20
- **UI Library:** Angular Material
- **State Management:** NgRx Component Store & Signals
- **Containerization:** Docker & Docker Compose

## 📜 Project History

This project has evolved significantly since its inception:

**Early 2024 - Initial Development**
- Started with `.NET 8` and `Angular 17`
- Originally maintained as two separate repositories (frontend & backend)
- Used SQL Server 2022 with stored procedures

**Major Upgrades & Migrations**
- ✅ Upgraded to `.NET 10` and `Angular 20`
- ✅ Migrated from SQL Server to `PostgreSQL` - A challenging but rewarding experience that deepened my understanding of database migrations without relying solely on Entity Framework
- ✅ Consolidated into a single monorepo for better maintainability

**Feature Additions**
- ✅ Supplier management system
- ✅ User authentication and authorization
- ✅ PDF report generation for sales and purchases
- ✅ Enhanced UI with Angular Material

**Technical Improvements**
- ✅ Added Entity Framework Core for database migrations (maintaining dual databases across machines was challenging!)
- ✅ Implemented NgRx Component Store for state management
- ✅ Adopted Angular Signals for newer features
- ✅ Docker support for easy deployment

## 🚀 Quick Start with Docker

The easiest way to get started is using Docker:

1. Navigate to the `src` directory:
   ```bash
   cd src
   ```
2. Start the containers:
   ```bash
   docker compose up -d
   ```
3. Open your browser and visit: `http://localhost:3001/`

That's it! The application should be running with all dependencies configured.

## 💻 Development Setup

**Prerequisites:**
- .NET 10.0 SDK
- Node.js (latest LTS version)
- Angular CLI
- PostgreSQL 16 

**Installation Steps:**

1. Clone the repository:
   ```bash
   git clone https://github.com/rd003/InventoryMgt.git
   ```

2. Navigate to the project directory:
   ```bash
   cd InventoryMgt
   ```

3. Open in VS Code (optional):
   ```bash
   code .
   ```

You'll need to set up both the backend and frontend separately:

### A. Backend

1. Navigate to the backend API directory:
   ```bash
   cd backend/InventoryMgt.Api
   ```

2. Update the connection string in `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Database=InventoryMgtDb;Username=your_user;Password=your_password"
   }
   ```

3. Run the application:
   ```bash
   dotnet run
   ```
   
   The database will be automatically created and seeded with initial data on first run.

### B. Frontend

1. Open a new terminal and navigate to the frontend directory:
   ```bash
   cd frontend/InventoryMgt.Client
   ```
   *(Keep the backend terminal running)*

2. Install dependencies:
   ```bash
   npm install
   ```

3. Start the development server:
   ```bash
   ng serve --open
   ```

4. The application will open in your browser. Use these credentials to log in:
   ```
   Username: admin
   Password: Admin@123
   ```

## 🔧 Entity Framework Migrations

If you encounter issues running migrations with the .NET CLI (due to multiple projects), use these commands:

**Common Error:**
```txt
Unable to create a 'DbContext' of type 'RuntimeType'...
```

**Solution:**

Navigate to the `backend` directory first:
```bash
cd backend
```

**Creating a Migration:**
```bash
dotnet ef migrations add YourMigrationName --project InventoryMgt.Data --startup-project InventoryMgt.Api
```
*Replace `YourMigrationName` with a descriptive name (e.g., `AddSupplierTable`)*

**Updating the Database:**
```bash
dotnet ef database update --project InventoryMgt.Data --startup-project InventoryMgt.Api
```

> **Note:** Migrations are automatically applied when you run the application, but you can manually execute them using the command above.

> **Tip:** If using Visual Studio, you can use the Package Manager Console instead, which handles multi-project solutions more gracefully.

---

## 📸 Screenshots

![1](./screenshots/1.png)

![2](./screenshots/2.png)

![3](./screenshots/3.png)

![4](./screenshots/4.png)

![5](./screenshots/5.png)

![6](./screenshots/6.png)

![7](./screenshots/7.png)

![8](./screenshots/8.png)

![9](./screenshots/9.png)

![10](./screenshots/10.png)

![11](./screenshots/11.png)

---

## 🤝 Contributing

Contributions are welcome! If you'd like to contribute:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## ⭐ Show Your Support

If you found this project helpful, please consider giving it a star on GitHub! ⭐

**Built with ❤️ by [rd003](https://github.com/rd003)**
