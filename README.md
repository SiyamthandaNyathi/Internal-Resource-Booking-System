# Internal Resource Booking System

## Project Overview
A web application for managing and booking shared company resources (meeting rooms, vehicles, equipment). Built with ASP.NET Core MVC and Entity Framework Core.

## Features
- Resource management (CRUD)
- Booking management (CRUD, conflict prevention)
- Dashboard for upcoming bookings
- Server-side validation and error handling
- Basic Bootstrap styling

## Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [SQL Server Express LocalDB](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)
- (Optional) [SQL Server Management Studio (SSMS)](https://aka.ms/ssms) for database viewing

## Setup Instructions
1. **Clone the repository:**
   ```
   git clone <your-repo-url>
   cd ResourceBookingSystem
   ```
2. **Restore dependencies:**
   ```
   dotnet restore
   ```
3. **Apply migrations and create the database:**
   ```
   dotnet ef database update
   ```
   > This will create the `ResourceBookingSystemDb.mdf` file using the connection string in `appsettings.json`.

4. **Run the application:**
   ```
   dotnet run --project ResourceBookingSystem
   ```
   The app will be available at `[https://localhost:5001](http://localhost:5214/)` or `http://localhost:5000`.

## Connecting to the Database
- The database is a LocalDB `.mdf` file located in the project root as `ResourceBookingSystemDb.mdf`.
- To view or edit the database, open SSMS and connect to `(localdb)\\mssqllocaldb`. Find `ResourceBookingSystemDb` under Databases.

##  Database
1. **Stop the application** to ensure the database is not in use.
2. **Copy the following files** to your backup location:
   - `ResourceBookingSystemDb.mdf`
   - `ResourceBookingSystemDb_log.ldf`
##  Database possible errors run
   sqllocaldb info
   sqllocaldb start mssqllocaldb



## Contact
For questions, contact 0786968342 at sidwellsiyamthanda17@gmail.com. 
