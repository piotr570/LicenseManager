# LicenseManager
## Overview
**LicenseManager** is a modular, scalable license management system designed for seamless license tracking, assignment, and auditing. Using **Clean Architecture**, the application emphasizes maintainability, scalability, and testability. Built with **ASP.NET Core Minimal APIs** and **MediatR**, it separates commands and queries, making the business logic concise and reusable.
This application is suitable for organizations that need to efficiently manage license lifecycles, user assignments, and audit reports, catering to a wide range of industries.
## Key Features
1. **License Management**:
    - Add, delete, and fetch licenses.
    - Reserve or assign licenses to specific users.

2. **User Management**:
    - CRUD operations for users (create, update, delete, and retrieve users).
    - Manage user-level licenses efficiently.

3. **Audit Tracking**:
    - Generate detailed audit reports of actions performed on licenses or users.
    - Track changes and assignments for compliance and reporting purposes.

4. **Modular & Scalable Design**:
    - Each domain area (licenses, users, audits) is encapsulated in a modular structure, enabling easy extension and maintenance.

5. **Clean API Design**:
    - Follows REST principles with intuitive endpoint routing for easy client integration.
    - Designed with the OpenAPI standard to generate API documentation through **Swagger** automatically.

6. **Extensibility**:
    - Modular architecture allows you to add new features (e.g., role management or advanced reporting) without changing the existing codebase significantly.

## Architecture Overview
The application is designed using **Clean Architecture**, which ensures clear separation of concerns and scalability. The architecture consists of four main layers:
### 1. **Presentation Layer**:
- Handles HTTP requests and responses.
- Implements ASP.NET Core Minimal APIs for lightweight and efficient routing.
- Contains modular route definitions for each domain (licenses, users, audits).

### 2. **Application Layer**:
- Includes **Commands**, **Queries**, and their associated **Handlers**, following the **CQRS** (Command Query Responsibility Segregation) pattern.
- Manages business rules and application-specific logic.
- Acts as middleware between the presentation and domain layers.

### 3. **Domain Layer**:
- Contains core business entities and their associated logic (e.g., License, User, and Audit entities).
- Fully isolated from the infrastructure and application layers, making it portable and testable.

### 4. **Infrastructure Layer**:
- Handles external dependencies such as database connectivity or third-party integrations.
- Implements repository patterns where needed.

Each layer communicates with the others through well-defined interfaces and dependency injection, ensuring loose coupling and high testability.
## Project Directory Structure
Below is an abstract view of the project directory structure:
``` plaintext
LicenseManager/
├── Application/                 # Application layer (CQRS, DTOs, Interfaces)
│   ├── UseCases/
│   │   ├── Licenses/            # License-specific commands, queries, and handlers
│   │   ├── Users/               # User-specific use cases
│   │   ├── Audits/              # Audit-specific use cases
│   └── ...
├── Domain/                      # Core domain logic and entities
│   ├── Entities/
│   ├── Services/
│   └── ...
├── Infrastructure/              # External dependencies and implementations
│   ├── Data/                    # Database access
│   └── ...
├── Modules/                     # Modular API route definitions
│   ├── LicenseModule.cs         # License-specific endpoints
│   ├── UserModule.cs            # User-specific endpoints
│   ├── AuditModule.cs           # Audit-specific endpoints
│   └── ...
├── README.md                    # Project documentation
├── Program.cs                   # Application Entry Point
└── ...
```
## Core Modules
### 1. **License Module**
Handles all operations related to licenses.
#### Key Endpoints:
- `GET /license/{licenseId}`: Fetch a license by its ID.
- `POST /license`: Add a new license.
- `DELETE /license/{licenseId}`: Delete a license.
- `POST /license/{licenseId}/assign/{userId}`: Assign a license to a user.
- `POST /license/{licenseId}/reserve/{userId}`: Reserve a license for a user.

#### Features:
- Uses CQRS for clean separation of responsibilities between license commands and queries.
- Facilitates modular addition or alteration of license-related functionality.

### 2. **User Module**
Manages users and their relationship with licenses.
#### Key Endpoints:
- `GET /users/{id}`: Retrieve user details by ID.
- `POST /users`: Create a new user.
- `PUT /users/{id}`: Update user information.
- `DELETE /users/{id}`: Delete a user.

#### Features:
- Direct integration with license operations through relational queries.
- Provides a foundation for managing user-specific functionality such as roles or permissions.

### 3. **Audit Module**
Handles the generation and retrieval of audit reports.
#### Key Endpoints:
- `GET /audit`: Generate an audit report for specified query parameters (like start and end dates or user IDs).

#### Features:
- Tracks operations performed on licenses and users.
- Ensures compliance and transparency through detailed reporting.

## Getting Started
### Prerequisites
- **.NET 9.0**: The application is built on .NET 9.0, so make sure it's installed on your system.
- **Database Configuration**: Ensure a database connection (PostgreSQL) is configured.

### Installation
1. Restore dependencies:
``` bash
   dotnet restore
```
1. Run the application:
``` bash
   dotnet run
```
1. Access the application in your browser at:
``` 
   http://localhost:5000/swagger
```
## API Documentation
The project uses **Swagger** for providing interactive API documentation. After running the app, you can access the Swagger UI at:
``` 
http://localhost:5000/swagger
```
This UI allows you to test all available endpoints with structured request and response examples.
## Design Principles
**LicenseManager** adheres to the following design principles:
1. **Single Responsibility Principle (SRP)**:
    - Each module and class serves a single purpose for maintainability.

2. **Modular Design**:
    - Each feature of the application is encapsulated into its respective module, making the addition of new features straightforward.

3. **Dependency Injection**:
    - All services (e.g., `IMediator`) are injected, promoting loose coupling.

4. **Separation of Concerns**:
    - Layers in the architecture ensure no overlap between presentation, business logic, and infrastructure.

5. **Scalability**:
    - The architecture supports the addition of new modules or features without significant refactoring.

## Roadmap
Features planned or under development:

### **Address Domain Layer Sharing Issues**
Currently, the domain layer is shared across multiple layers (e.g., Application, Infrastructure, and Presentation) in a way that violates **Clean Architecture** principles. This was intentionally done for the sake of simplicity during initial development. Ideally, the domain should remain isolated, with each layer interacting through well-defined interfaces, DTOs, or mappers to prevent direct dependency on core domain entities. Moving forward, refactoring efforts should focus on ensuring proper boundaries between layers, such as using application-specific models for communication and repositories that abstract domain persistence. This will improve maintainability, reduce coupling, and adhere to core architectural principles.

### **Unify Business Rule Validation**
Currently, the validation of business rules is inconsistent across the project. In some cases, business rules are enforced within domain entities, while in others, validation is handled at the application or API layer. This approach was intentional to demonstrate potential solutions for handling business logic validation and explore trade-offs between different layers of responsibility. However, going forward, a unified approach is needed to ensure clarity, maintainability, and adherence to **Clean Architecture** principles. This includes defining clear boundaries for where and how business validation should occur, whether through domain entities, domain services, or application-level validation, and ensuring consistency across the entire project.

- **Auditing**:
    - Add detailed reports for users, licenses, and audits with filtering options.
    - a. **Define Domain Entities**
    - b. **Define Application Use Cases**
    - c. **Enhance the Domain Layer**
    - d. **Implement Infrastructure for Data Persistence**
    - e. **Integrate Auditing into Existing Workflows**
    - f. **Build Presentation Layer Endpoints**

## Contributing
1. Fork the repository.
2. Create a branch for your feature or bug fix:
``` bash
   git checkout -b feature/your-feature-name
```
1. Commit your changes:
``` bash
   git commit -m "Description of changes"
```
1. Push your branch:
``` bash
   git push origin feature/your-feature-name
```
1. Submit a pull request for review.

## License
This project is licensed under the MIT License. 