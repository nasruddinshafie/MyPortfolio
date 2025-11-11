# My Portfolio

A full-stack portfolio application built with .NET 9 and Blazor Server, following Clean Architecture principles.

## Features

- **Bio Management** - Personal information, contact details, and social media links
- **Project Showcase** - Display and manage portfolio projects with:
  - Project descriptions and detailed information
  - Technology stack badges
  - Live project and GitHub repository links
  - Project images and timeline
  - Active/inactive status management
- **Clean Architecture** - Well-organized codebase with clear separation of concerns
- **CQRS Pattern** - Using MediatR for command and query handling
- **Responsive UI** - Beautiful Blazor Server interface that works on all devices

## Technology Stack

### Backend
- .NET 9
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- MediatR (CQRS)
- Clean Architecture

### Frontend
- Blazor Server
- Bootstrap 5
- Font Awesome icons

### Testing
- xUnit
- Moq
- In-Memory Database for integration tests

## Project Structure

```
MyPortfolio/
├── Domain/              # Core business entities
├── Application/         # Business logic and CQRS handlers
├── Infrastructure/      # Data access and external services
├── WebApi/             # REST API endpoints
├── Portfolio.UI/       # Blazor Server frontend
└── Tests/              # Unit and integration tests
```

## Getting Started

### Prerequisites

- .NET 9 SDK
- SQL Server (LocalDB or Express)
- Visual Studio 2022 or VS Code

### Installation

1. Clone the repository
   ```bash
   git clone https://github.com/YOUR_USERNAME/MyPortfolio.git
   cd MyPortfolio
   ```

2. Restore dependencies
   ```bash
   dotnet restore
   ```

3. Update the database
   ```bash
   dotnet ef database update --project Infrastructure --startup-project WebApi
   ```

4. Run the API (Terminal 1)
   ```bash
   dotnet run --project WebApi
   ```
   API will be available at: https://localhost:7278

5. Run the UI (Terminal 2)
   ```bash
   dotnet run --project Portfolio.UI
   ```
   UI will be available at the port shown in the terminal

### Running Tests

```bash
dotnet test
```

## API Endpoints

### Bio
- `GET /api/bio` - Get bio information
- `POST /api/bio` - Create bio
- `PUT /api/bio/{id}` - Update bio

### Projects
- `GET /api/projects` - Get all projects
- `GET /api/projects/{id}` - Get project by ID
- `POST /api/projects` - Create new project
- `PUT /api/projects/{id}` - Update project
- `DELETE /api/projects/{id}` - Delete project

## Usage

1. **Create Your Bio**: Navigate to "Create Bio" to add your personal information
2. **Add Projects**: Go to "Projects" and click "Create New Project" to showcase your work
3. **View Portfolio**: Visit the homepage to see your complete portfolio with bio and projects

## Architecture Highlights

- **Clean Architecture**: Clear separation between Domain, Application, Infrastructure, and Presentation layers
- **CQRS Pattern**: Commands and Queries separated for better maintainability
- **Repository Pattern**: Abstraction over data access
- **Dependency Injection**: Following SOLID principles
- **Comprehensive Testing**: 36+ tests covering repositories, handlers, and controllers

## License

This project is open source and available under the MIT License.

## Author

Nasruddin Shafie

---

Built with Claude Code
