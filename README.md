# PG Manager

This project is designed to manage PGs/Hostels. It consists of 3 types of users and their responsibilities:

#### Admin :
- Manage master data and users
- Can create owner accounts and lock/unlock them.
#### Owner : 
- Save/Change information about their PG
- Add/Remove Photos
- Manage Price Tiers for PG/Hostel
- Add Rooms and Beds
- Manage requests to join/leave.

#### Tenant : 
- Search for PG/Hostel
- Apply/Leave PG
- Manage document(IDs for verification by PG owner)

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

- [.Net Core 3.1](https://dotnet.microsoft.com/download) - This project uses .NET core 3.1
- (Optional) [Microsoft SQL Server Express](https://www.microsoft.com/en-au/sql-server/sql-server-downloads) - If you wish to use MS SQL server as your database.
- (Optional) [PostgreSQL](https://www.postgresql.org/download/) - If you wish to use PostgreSQL as your database.
- (Optional) [SQLite](https://www.sqlite.org/download.html) - If you wish to use PSQLiteostgreSQL as your database.

### Installing

A step by step series of examples that tell you how to get a development env running

Say what the step will be
#### Restore
Use CLI to navigate to main project directory (PGManager) and run below command
```
dotnet restore
```

#### Select Database
Set appropriate database in Startup.cs
##### MS SQL Server
```
services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(
Configuration.GetConnectionString("DefaultConnection")));
```

##### SQLite
```
services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlite(
Configuration.GetConnectionString("DefaultConnection")));
```

##### PostgreSQL
```
services.AddDbContext<ApplicationDbContext>(options =>
options.UseNpgsql(
Configuration.GetConnectionString("DefaultConnection")));
```

#### Add Migrations
Delete the Migrations folder under PGManager.DataAccess. Set connection string as per your choice of database in appsettings.json.
Use CLI to navigate to main project directory (PGManager) and run below command

```
dotnet ef migrations add [MigrationName] --project ..\PGManager.DataAccess\PGManager.DataAccess.csproj
```

#### Manage seed data
Check files under PGManager/SeedData and change any data as per your requirement

#### Run the project
Use CLI to navigate to main project directory (PGManager) and run below command
```
dotnet run
```
This will start your Kestrel server and you can browse the URL in terminal from any browser.
###### [Note] : All migrations and seed data will be applied to database if not present when you run the application. If database is not present, it will get created. Admin users from seed data are automatically created from SeedData/AdminUsers.json.

## Built With

* [.Net Core](https://dotnet.microsoft.com/) - This project is implemented in .NET core 3.1
* [Bootstrap 4](https://getbootstrap.com/)
* [Font Awesome](https://fontawesome.com/) - Vector icons
* [Datatables](https://datatables.net/) - Used for fast and responsive tables
* [Summernote](https://summernote.org/) - A rich text editor
* [Alertify](https://alertifyjs.com/) - A JS libary for showing alerts

## License

This project is licensed under the MIT License. You may use or alter this in any way you want.
