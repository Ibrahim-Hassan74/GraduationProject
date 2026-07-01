# Contributing to Wasla

First off, thank you for considering contributing to Wasla! It's people like you that make the open-source community such an amazing place to learn, inspire, and create.

## Welcome

We welcome contributions of all kinds from anyone. You don't have to be a senior developer to contribute. Whether it's reporting a bug, proposing a new feature, or improving our documentation, we appreciate your help!

## How to Fork and Clone the Repository

1. **Fork the repo** by clicking the "Fork" button in the top right corner of the GitHub page.
2. **Clone your fork** locally:
   ```bash
   git clone https://github.com/YOUR-USERNAME/GraduationProject.git
   cd GraduationProject
   ```
3. **Add the upstream remote** so you can pull the latest changes:
   ```bash
   git remote add upstream https://github.com/Ibrahim-Hassan74/GraduationProject.git
   ```

## Local Development Setup

### Required Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker & Docker Compose](https://www.docker.com/products/docker-desktop)
- [SQL Server Management Studio (SSMS)](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms) or Azure Data Studio (optional, for DB management)
- EF Core CLI (`dotnet tool install --global dotnet-ef`)

### Setup Instructions

1. Run `docker-compose up -d` to start SQL Server and Redis.
2. Apply database migrations:
   ```bash
   dotnet ef database update --project SmartMicrobus.Infrastructure --startup-project SmartMicrobus.API
   ```
3. Run the API project (`SmartMicrobus.API`).
4. Access the Swagger UI at `http://localhost:5000/swagger`.

## Branch Naming Convention

Please create a new branch for your work. Use the following prefixes for branch names:

- `feature/` or `feat/` for new features (e.g., `feat/add-driver-reports`)
- `bugfix/` or `fix/` for bug fixes (e.g., `fix/qr-code-validation`)
- `docs/` for documentation updates
- `chore/` for maintenance tasks

## Conventional Commits

We follow the [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/) specification for commit messages.

- `feat: [description]` for new features
- `fix: [description]` for bug fixes
- `docs: [description]` for documentation changes
- `refactor: [description]` for code refactoring
- `test: [description]` for adding/updating tests

## Pull Request Guidelines

1. Ensure your code builds cleanly and passes all tests.
2. Ensure you have added XML comments for new API endpoints.
3. Keep PRs focused on a single issue or feature.
4. Link the PR to a specific GitHub issue (e.g., "Closes #12").
5. Request a review from the maintainers once ready.

## Coding Standards

- Follow standard C# naming conventions (PascalCase for classes/methods, camelCase for variables).
- Maintain the principles of **Clean Architecture** (do not bypass the Core layer).
- Always use `ApiResponse` or `ApiResponseWithData<T>` envelopes for controllers.
- Use asynchronous methods (`async` / `await`) for all I/O operations.

## Issue Reporting Guidelines

Before opening a new issue, please search the issue tracker to see if it has already been reported. When creating an issue, include:

- A clear, descriptive title.
- Steps to reproduce the bug.
- Expected vs actual behavior.
- Logs or error stack traces (use markdown code blocks).
