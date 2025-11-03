# API Service App

## 📖 Overview

Central API service for the user data management system. This application acts as a hub for all API requests between frontend applications and backend services.

## 🎯 Purpose

- Receives user data from Name-Age App
- Sends data to Azure Service Bus for processing
- Triggers email sending via Java Email Service
- Provides health check endpoint

## 🔗 Connections

- **Receives from**: Name-Age App, Email Notification App
- **Sends to**: Azure Service Bus, Java Email Service
- **Azure Services**: Azure App Service, Azure Service Bus

## 🏗️ Technology Stack

- .NET 8.0
- ASP.NET Core Web API
- Azure Service Bus SDK
- Swagger/OpenAPI

## 📚 Complete System Documentation

See `SYSTEM_ARCHITECTURE.md` for complete system overview and how this app fits into the bigger picture.

## 🚀 Deployment

See `DEPLOYMENT.md` for step-by-step deployment instructions using Azure Portal UI.

## 🔧 API Endpoints

### Health Check

- **GET** `/api/health`
- Returns service status

### Submit User Data

- **POST** `/api/userdata`
- Accepts name and age
- Sends to Service Bus

### Trigger Email Sending

- **POST** `/api/email/send`
- Triggers Java Email Service

## 🛠️ Local Development

1. Install .NET 8.0 SDK
2. Update `appsettings.json` with your Service Bus connection string
3. Run:

```bash
dotnet restore
dotnet run
```

4. Open browser: `https://localhost:5001/swagger`

## 📝 Environment Variables

- `AZURE_SERVICEBUS_CONNECTIONSTRING` - Service Bus connection string
- `AZURE_SERVICEBUS_QUEUENAME` - Queue name (default: userdata-queue)
- `JAVA_EMAIL_SERVICE_URL` - Java Email Service URL

## 📄 License

Internal use only
