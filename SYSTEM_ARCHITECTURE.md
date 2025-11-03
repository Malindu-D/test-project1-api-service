# SYSTEM ARCHITECTURE - Complete Overview

## 📋 System Purpose

This is a multi-application system for collecting user data (name & age), storing in database, and sending email notifications using Azure services.

## 🏗️ Complete Architecture

### Applications Overview (5 Total)

1. **API Service App** (.NET) - Central Hub

   - Central hub for all API requests
   - Routes data between applications
   - Sends messages to Azure Service Bus
   - Location: `Api-service-app`
   - Deployment: Azure App Service
   - GitHub Actions: Uses Azure auto-generated workflow

2. **Name-Age App** (HTML/JS) - **THIS APPLICATION**

   - Frontend form to collect name and age
   - Light blue color theme
   - Manual API connection test button (user clicks to test)
   - Users can enter data freely without testing API first
   - API test only required when user attempts to submit data
   - API endpoint loaded from Azure configuration
   - Visual status feedback after button click (green/red/yellow indicators)
   - Sends data to API Service App
   - Location: `name-age-app`
   - Deployment: Azure Static Web Apps
   - GitHub Actions: Uses Azure auto-generated workflow

3. **Test Function App** (.NET)

   - Listens to Azure Service Bus queue
   - Receives messages from Service Bus (sent by API Service)
   - Saves data to Azure SQL Database
   - Service Bus triggered function
   - Location: `test-function-app`
   - Deployment: Azure Functions
   - GitHub Actions: Uses Azure auto-generated workflow

4. **Email Notification App** (HTML/JS)

   - Simple frontend to send email notifications
   - User enters receiver email address
   - Manual API connection test button (user clicks to test)
   - Users can enter email freely without testing API first
   - API test only required when user attempts to send email
   - API endpoint loaded from Azure configuration
   - Visual status feedback after button click (green/red/yellow indicators)
   - Light blue color theme
   - Calls API Service App to trigger emails
   - Location: `email-notification-app`
   - Deployment: Azure Static Web Apps
   - GitHub Actions: Uses Azure auto-generated workflow

5. **Java Email Service** (Java)
   - Reads data from Azure SQL Database
   - Creates HTML email with table of all user data
   - Uses Azure Communication Service to send emails
   - Called by API Service App
   - Location: `java-email-service`
   - Deployment: Azure App Service
   - GitHub Actions: Uses Azure auto-generated workflow

### Database

- **Azure SQL Database**
- Stores user data (name, age, timestamp)
- Accessed by: Test Function App (write), Spring Email Service (read)

### Azure Services Used

1. **Azure App Service** (x2) - Hosts API Service App & Java Email Service
2. **Azure Static Web Apps** (x2) - Hosts Name-Age App & Email Notification App
3. **Azure Functions** - Hosts Test Function App
4. **Azure Service Bus** - Message queue between API Service and Function App
5. **Azure SQL Database** - Stores user data
6. **Azure Communication Service** - Sends emails

## 🔄 Data Flow

### Flow 1: Collecting User Data

```
User fills form in Name-Age App (HTML/JS) - THIS APP
    ↓
Name-Age App sends POST request to API Service App
    ↓
API Service App validates data
    ↓
API Service App sends message to Azure Service Bus
    ↓
Test Function App triggers on new Service Bus message
    ↓
Test Function App saves data to Azure SQL Database
    ↓
Success response back to user
```

### Flow 2: Sending Email Notifications

```
User enters receiver email in Email Notification App (HTML/JS)
    ↓
Email Notification App sends POST request to API Service App (with receiver email)
    ↓
API Service App calls Java Email Service endpoint
    ↓
Java Email Service reads all entries from Azure SQL Database
    ↓
Java Email Service creates HTML email with data table
    ↓
Java Email Service uses Azure Communication Service to send email
    ↓
Email sent to receiver with all user data
    ↓
Success response back to user
```

## 🔐 Security & Configuration

### Environment Variables Pattern

Every application uses environment variables for:

- Database connection strings
- API endpoints/URLs
- Azure Service Bus connection strings
- Azure Communication Service keys
- Any secrets or sensitive data

**Static Web Apps (Name-Age & Email Notification):**

- Use Azure Static Web Apps Configuration API
- Environment variable: `API_ENDPOINT` (required)
- Accessed via serverless function `/api/config`
- Automatically loaded on page load, no manual user input
- Example: `API_ENDPOINT=https://your-api-service.azurewebsites.net`

### Deployment Pattern

All applications follow the same deployment pattern:

1. Create Azure resource in Azure Portal
2. Enable GitHub deployment from Azure Portal (Deployment Center)
3. Azure automatically creates workflow file in your GitHub repository
4. Code pushed to GitHub repository
5. Azure auto-generated workflow triggers automatically
6. Application built and deployed to Azure
7. Environment variables configured in Azure Portal UI
8. Static Web Apps: Configure `API_ENDPOINT` in Configuration → Application settings

## 📋 Ground Rules (ALWAYS FOLLOW)

1. **No Azure CLI** - All deployment via Azure Portal UI
2. **Azure Auto-Generated Workflows** - Use Azure Portal Deployment Center to create GitHub Actions workflows automatically
3. **Environment Variables** - All sensitive data in env vars
4. **Simple Deployment Guide** - Each app has `DEPLOYMENT.md`
5. **Keep It Simple** - No over-engineering, easy to understand code

## 🗂️ Database Schema

### Table: Users (or similar)

- `Id` (Primary Key, Auto-increment)
- `Name` (String/Varchar)
- `Age` (Integer)
- `CreatedAt` (DateTime)
- `Email` (String/Varchar) - Optional for email sending

## 🔧 API Service App Endpoints

### Current Endpoints:

1. `POST /api/userdata` - Receives name & age from Name-Age App

   - Validates data
   - Sends to Service Bus
   - Returns success/failure

2. `POST /api/email/send` - Triggers email sending

   - Called by Email Notification App
   - Forwards request to Spring Email Service
   - Returns success/failure

3. `GET /api/health` - Health check endpoint

### Future Extensibility:

- Add new endpoints by creating new controllers
- Follow same pattern: validate → process → respond
- Use dependency injection for services
- Keep controllers thin, logic in services

## 🚀 Future Development Guidelines

### Adding New Features to API Service:

1. Create new controller in `Controllers` folder
2. Create corresponding service in `Services` folder
3. Add environment variables to `appsettings.json` and Azure App Service config
4. Update `DEPLOYMENT.md` if new Azure resources needed
5. Test locally before pushing to GitHub

### Adding New Applications:

1. Create new directory at same level as existing apps
2. Follow same structure: code + GitHub Actions + DEPLOYMENT.md
3. Update this SYSTEM_ARCHITECTURE.md in all apps
4. Connect to API Service App or other apps as needed

### Modifying Database:

1. Update schema in Azure SQL Database via Portal
2. Update all apps that interact with database
3. Test Function App (writes data)
4. Java Email Service (reads data)

## 📞 Inter-Application Communication

### Who Calls Who:

- Name-Age App → API Service App (THIS APP calls API)
- API Service App → Azure Service Bus
- Azure Service Bus → Test Function App (trigger)
- Test Function App → Azure SQL Database
- Email Notification App → API Service App
- API Service App → Java Email Service
- Java Email Service → Azure SQL Database
- Java Email Service → Azure Communication Service

### No Direct Communication Between:

- Static Web Apps don't call each other
- Function App doesn't call other apps (only triggered by Service Bus)
- Java Email Service only called by API Service

## 🛠️ Technology Stack Summary

| Application            | Technology        | Deployment Target     |
| ---------------------- | ----------------- | --------------------- |
| API Service App        | .NET 8.0          | Azure App Service     |
| Name-Age App           | HTML/CSS/JS       | Azure Static Web Apps |
| Test Function App      | .NET 8.0 Isolated | Azure Functions       |
| Email Notification App | HTML/CSS/JS       | Azure Static Web Apps |
| Java Email Service     | Java 17           | Azure App Service     |

## 📚 Important Notes for Future You

- **This file exists in every application** - Keep them synchronized
- **Always check environment variables** - Most issues are config-related
- **GitHub Actions deploy automatically** - Don't manually deploy unless testing
- **Keep it simple** - User doesn't know programming, avoid complexity
- **Azure Portal UI only** - No CLI commands in documentation
- **Test locally first** - Use local emulators/databases before pushing

## � Project Structure

### API Service App (.NET)

Following Azure deployment best practices with clean project structure:

```
Api-service-app/                          (Repository root)
├── Api-service-app.sln                   ✅ Solution file at root
├── README.md                             📖 Project overview
├── DEPLOYMENT.md                         📋 Deployment guide
├── SYSTEM_ARCHITECTURE.md                🏗️ This file
├── QUICK_START.md                        🚀 Quick start guide
├── .gitignore
├── .env.example
├── .github/
│   └── workflows/                        🔄 Azure auto-generated workflows
└── src/
    └── ApiServiceApp/                    📦 Main project directory
        ├── ApiServiceApp.csproj
        ├── Program.cs
        ├── appsettings.json
        ├── appsettings.Development.json
        ├── Controllers/                  🎮 API endpoints
        │   ├── EmailController.cs
        │   ├── HealthController.cs
        │   └── UserDataController.cs
        ├── Models/                       📊 Data models
        │   ├── ApiResponse.cs
        │   ├── EmailRequest.cs
        │   └── UserDataRequest.cs
        ├── Services/                     ⚙️ Business logic
        │   ├── IEmailService.cs
        │   ├── EmailService.cs
        │   ├── IServiceBusService.cs
        │   └── ServiceBusService.cs
        ├── bin/                          🔧 Build output
        └── obj/                          🔧 Build artifacts
```

### Why This Structure?

✅ **Azure Workflow Compatibility** - Azure auto-generated workflows expect one `.sln` at root  
✅ **.NET Conventions** - Standard structure recognized by Visual Studio and dotnet CLI  
✅ **Clean Root** - Only documentation and configuration files at top level  
✅ **Scalable** - Easy to add more projects to solution (e.g., `src/ApiServiceApp.Tests/`)  
✅ **Build Simplicity** - `dotnet build` works without specifying file path

### Build Commands (All Valid)

```bash
# Build using solution file (explicit)
dotnet build Api-service-app.sln --configuration Release

# Build using default discovery (Azure workflow uses this)
dotnet build --configuration Release

# Restore dependencies
dotnet restore

# Run the application
dotnet run --project src/ApiServiceApp/ApiServiceApp.csproj
```

## �📄 Version History

- v1.0 (Oct 30, 2025) - Initial system setup with 5 applications
- v1.1 (Oct 30, 2025) - Added Name-Age App
- v1.2 (Oct 31, 2025) - Added Test Function App
- v1.3 (Oct 31, 2025) - Changed Spring Email Service to Java Email Service (Azure App Service instead of Container Apps)
- v1.4 (Oct 31, 2025) - Added Email Notification App with receiver email input and API test button
- v1.5 (Oct 31, 2025) - Completed Java Email Service with database reading and HTML email table generation
- v1.6 (Nov 3, 2025) - Updated Static Web Apps to use Azure environment variables instead of manual API endpoint input, automatic connection testing on page load
- v1.7 (Nov 3, 2025) - Updated to use Azure auto-generated GitHub Actions workflows instead of custom workflows
- v1.8 (Nov 3, 2025) - Changed Static Web Apps from automatic connection test to manual button-triggered test
- v1.9 (Nov 3, 2025) - Fixed Static Web Apps UX: users can now enter data freely, API test only required before submission
- v2.0 (Nov 3, 2025) - Restructured API Service App to follow Azure deployment best practices (src/ folder structure, solution at root)
