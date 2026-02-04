# 📦 SimpleProductAPI

A simple REST API for product management.

## 🚀 About the Project

SimpleProductAPI is a simple API for working with products. It provides basic REST endpoints that you can use as a backend for web or mobile applications.

This project serves as a simple demo.

## 🔧 Tech Stack

- Backend: .NET10
- Data storage: SQL Server 2025

## 🛠️ Installation

1. **Clone the repository**  
```sh
git clone https://github.com/JiRost87/SimpleProductAPI.git
cd SimpleProductAPI
```
2. **Prerequisities**  
Project requires MSSQL Server for data storage. Can be standalone installation or docker container.
Ports 8080, 8081 needs to be free or they need to be adjusted in launchsettings.json or docker-compose.yml
For running a project from IDE/Console:
-  data in sql folder needs to be seeded to db using sql client eg. SSMS, DataGip, Azure Data Studio, sqlcmd in container 
-  adjusted connection string in appsettings.json to match connection to db eg. server name, port


3. **Run the application**  
There are multiple ways to run apllication:
- Using Visual Studio IDE
- Run app from console in project folder
```sh
docker run
```
- Run app in docker container by running following command in project root folder (This method doesn't require SQL data seeding)
```sh
docker compose up
```

## 📡 API Endpoints
Basic overview of endpoints:
There are two versions of endpoints v1 and v2, v2 endpoint has support for pagination.

| Method | Endpoint                                   | Description                              |
| ------ | ------------------------------------------ | ---------------------------------------- |
| GET    | v*/Products/GetProducts                    | Get all products                         |
| GET    | v*/Products/GetProductById/{id}            | Get a product by ID                      |
| PUT    | v*/Products/UpdateProductDescription/{id}  | Updates description for existing product |

Documentation is available at /swagger endpoint.


## 📄 Example Product JSON
```json
{
  "id": 1,
  "name": "Example product",
  "price": 12.99,
  "description": "Short description here"
}
```
## 🧪 Testing
Unit tests are located in SimpleProductAPI.Tests folder.
To run tests you can use 
- Visual Studio IDE by running tests on Tests project
- From console running following command in test project folder
```sh
dotnet test
