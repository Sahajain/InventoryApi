
# Product Inventory REST API

A simple RESTful API built with ASP.NET Core Web API (.NET 9) to manage products in an inventory system.

## Features

- ✅ Add, retrieve, update, and soft-delete products
- ✅ Filter products by category
- ✅ Search products by name or description
- ✅ Sort products by price, name, stock, category, or creation date
- ✅ Pagination support
- ✅ Low-stock alerts (products with stock < 5)
- ✅ Input validation with FluentValidation
- ✅ Proper HTTP status codes
- ✅ SQLite database with Entity Framework Core
- ✅ Swagger/OpenAPI documentation

## Technologies Used

- ASP.NET Core Web API (.NET 9)
- Entity Framework Core 9.0
- SQLite Database
- FluentValidation
- Swagger/OpenAPI

## Setup Instructions

### Prerequisites
- .NET 9 SDK
- Git

### Installation

1. **Clone the repository**
   ```bash
   git clone <your-repo-url>
   cd ProductInventoryAPI
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Run the application**
   ```bash
   dotnet run
   ```

4. **Access the API**
   - API Base URL: `https://localhost:7xxx` or `http://localhost:5xxx`
   - Swagger UI: `https://localhost:7xxx/swagger`

The database will be automatically created with sample data on first run.

## API Endpoints

### Products

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/products` | Get all products (with filtering, search, pagination) |
| GET | `/api/products/{id}` | Get product by ID |
| POST | `/api/products` | Create a new product |
| PUT | `/api/products/{id}` | Update a product |
| DELETE | `/api/products/{id}` | Soft delete a product |
| GET | `/api/products/low-stock` | Get products with low stock |

### Query Parameters for GET /api/products

- `page` (int): Page number (default: 1)
- `pageSize` (int): Items per page (default: 10)
- `category` (string): Filter by category
- `search` (string): Search in name or description
- `sortBy` (string): Sort by field (name, price, stock, category, created)
- `sortDescending` (bool): Sort order (default: false)

## Sample API Calls

### Create Product
```bash
curl -X POST "https://localhost:7xxx/api/products" \
-H "Content-Type: application/json" \
-d '{
  "name": "Gaming Keyboard",
  "description": "Mechanical gaming keyboard with RGB lighting",
  "price": 129.99,
  "stockQuantity": 25,
  "category": "Electronics"
}'
```

### Get Products with Filtering
```bash
curl "https://localhost:7xxx/api/products?category=Electronics&page=1&pageSize=5&sortBy=price&sortDescending=true"
```

### Search Products
```bash
curl "https://localhost:7xxx/api/products?search=laptop&page=1&pageSize=10"
```

### Update Product
```bash
curl -X PUT "https://localhost:7xxx/api/products/1" \
-H "Content-Type: application/json" \
-d '{
  "price": 899.99,
  "stockQuantity": 20
}'
```

### Get Low Stock Products
```bash
curl "https://localhost:7xxx/api/products/low-stock"
```

## Response Examples

### Product Response
```json
{
  "id": 1,
  "name": "Laptop",
  "description": "High-performance laptop",
  "price": 999.99,
  "stockQuantity": 15,
  "category": "Electronics",
  "isActive": true,
  "isLowStock": false,
  "createdAt": "2024-01-01T10:00:00Z",
  "updatedAt": "2024-01-01T10:00:00Z"
}
```

### Paginated Response
```json
{
  "items": [...],
  "totalCount": 50,
  "page": 1,
  "pageSize": 10,
  "totalPages": 5,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

## Database Commands

```bash
# Create migration (if needed)
dotnet ef migrations add InitialCreate

# Update database (if needed)
dotnet ef database update

# Drop database (for reset)
dotnet ef database drop
```

## Testing

Use the included Swagger UI at `/swagger` to test all endpoints interactively.

## Project Structure

- **Controllers/**: API controllers
- **Models/**: Data models and DTOs
- **Data/**: Entity Framework DbContext
- **Services/**: Business logic layer
- **Validators/**: Input validation rules

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test your changes
5. Submit a pull request

## License

This project is licensed under the MIT License.
```

## Commands to Run

1. **Create the project:**
```bash
mkdir ProductInventoryAPI
cd ProductInventoryAPI
dotnet new webapi
```

2. **Add packages:**
```bash
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package FluentValidation
dotnet add package FluentValidation.DependencyInjectionExtensions
```

3. **Run the project:**
```bash
dotnet run
```

