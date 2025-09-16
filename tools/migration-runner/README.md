# Association Registry Migration Generator

A TUI-based tool for generating Marten database migrations using `db-patch` across different projects in the Association Registry.

## Features

- Terminal User Interface using Spectre.Console
- Generates migration scripts by comparing current DB schema with Marten's expected schema
- Support for multiple projects (Admin API, Admin Projections, Public Projections, ACM API)
- Automatic script numbering (00001, 00002, etc.)
- Automatic sequential numbering for migration scripts
- Custom script execution (like regen-marten)
- YAML-based configuration

## Prerequisites

- .NET SDK
- dotnet-script (`dotnet tool install -g dotnet-script`)
- PostgreSQL database running

## Usage

1. Run the migration generator in interactive mode:
   ```bash
   cd tools/migration-runner
   dotnet-script migration-generator.csx
   ```

2. Run for a specific project directly:
   ```bash
   cd tools/migration-runner
   dotnet-script migration-generator.csx "Admin API"
   # or
   dotnet-script migration-generator.csx "Admin Projections"
   # or
   dotnet-script migration-generator.csx "Public Projections"
   # or
   dotnet-script migration-generator.csx "ACM API"
   ```

2. On first run, it will create a `config.yaml` file with default settings. Update connection strings as needed.

3. Select a project from the menu.

4. The tool will:
   - Run `dotnet run -- db-patch` to generate migration SQL
   - Show you the generated SQL
   - Prompt you to name the migration
   - Save it with the next sequential number in the migrations folder

## Configuration

The `config.yaml` file contains:

- **Projects**: Configuration for each project including:
  - `csprojPath`: Path to the project's .csproj file
  - `migrationDestination`: Where to save generated migration scripts
  - `connectionString`: PostgreSQL connection string for comparing schemas

- **CustomScripts**: Additional scripts like regen-marten

## Script Naming Convention

Generated migrations are saved with the naming format:
```
00001_<your_provided_name>.sql
```

The number is automatically incremented based on existing scripts in the destination folder.

## Example Workflow

1. Make changes to your Marten document models or projections
2. Run the migration generator
3. Select "Admin API" (or relevant project)
4. The tool runs `db-patch` and shows the generated SQL
5. Review the SQL and enter a descriptive name (e.g., "add_email_to_vereniging")
6. File is saved as: `migrations/production/admin.api/scripts/up/00002_add_email_to_vereniging.sql`

## How it Works

The tool uses Marten's `db-patch` command which:
1. Connects to your PostgreSQL database
2. Compares the current schema with what Marten expects based on your code
3. Generates SQL to migrate from current state to expected state
4. Outputs the migration SQL to a file