# Terraform Infrastructure Requirements

## Overview

This document outlines the requirements for deploying the API Service App infrastructure using Terraform on Azure.

## ✅ Application Updates Completed (Oct 31, 2025)

### Changes Made:

1. **Added Azure Communication Services SDK** - `Azure.Communication.Email` v1.0.1
2. **Updated EmailService** - Now uses Azure Communication Service directly instead of calling Java service
3. **Updated Configuration** - Changed from `JavaEmailService` to `AzureCommunicationServices` in appsettings.json
4. **Updated SYSTEM_ARCHITECTURE.md** - Reflected the new email service architecture

### New Configuration Required:

```json
"AzureCommunicationServices": {
  "ConnectionString": "",  // Will be provided by Key Vault
  "SenderEmail": ""       // Will be from Azure Email Communication Service
}
```

---

## 🔐 Secrets to Store in Key Vault

| Secret Name                            | Purpose               | Used By                               |
| -------------------------------------- | --------------------- | ------------------------------------- |
| `azure-servicebus-connectionstring`    | Service Bus access    | API Service App                       |
| `azure-sql-connectionstring`           | SQL Database access   | API Service App, Email Export Service |
| `azure-communication-connectionstring` | Email sending via ACS | API Service App, Email Export Service |

**Estimated Key Vault Cost**: < $1/month (FREE tier eligible)

---

## 🏗️ Azure Resources Required

### 1. App Services (x2)

- **api-app**: Hosts this .NET 8.0 API Service App
- **email-export-app**: Hosts Java service for bulk email exports

### 2. Azure Service Bus

- Namespace with Standard tier
- Queue: `userdata-queue` (configurable via tfvars)

### 3. Azure SQL Database

- SQL Server + Database
- Initial schema creation via SQL script

### 4. Azure Communication Services

- Communication Service resource
- Email Communication Service with verified domain
- Provides default sender email address

### 5. Azure Key Vault

- Standard tier
- Stores all connection strings and secrets

### 6. Azure Storage Account (for Terraform state)

- Created via bootstrap Terraform
- Stores .tfstate files for main infrastructure

---

## 📁 Terraform Directory Structure

```
../terraform/                          (sibling to Api-service-app)
├── bootstrap/                         (Run once to create state storage)
│   ├── main.tf
│   ├── variables.tf
│   ├── outputs.tf
│   └── README.md
├── modules/
│   ├── app-service/
│   │   ├── main.tf
│   │   ├── variables.tf
│   │   └── outputs.tf
│   ├── service-bus/
│   │   ├── main.tf
│   │   ├── variables.tf
│   │   └── outputs.tf
│   ├── sql-database/
│   │   ├── main.tf
│   │   ├── variables.tf
│   │   └── outputs.tf
│   ├── communication-service/
│   │   ├── main.tf
│   │   ├── variables.tf
│   │   └── outputs.tf
│   └── key-vault/
│       ├── main.tf
│       ├── variables.tf
│       └── outputs.tf
├── environments/
│   ├── dev.tfvars
│   ├── prod.tfvars
│   └── github.tfvars           (empty template for CI/CD variables)
├── main.tf
├── variables.tf
├── outputs.tf
├── locals.tf
├── providers.tf
├── backend.tf
└── README.md
```

---

## 🎯 Naming Convention

Format: `{projectname}-{resourcename}-{resourceinsideresourcename}`

Examples:

- App Service: `myproject-api-app` and `myproject-email-export-app`
- Service Bus: `myproject-servicebus-ns` (namespace), queue: `userdata-queue`
- SQL Server: `myproject-sql-server`, Database: `myproject-sql-db`
- Key Vault: `myproject-kv-secrets`
- Communication: `myproject-communication-svc`

**Note**: `projectname` will be a local variable sourced from tfvars for easy customization.

---

## 🌍 Environments

### Development (dev.tfvars)

- Lower SKUs/tiers for cost savings
- Development-specific configurations

### Production (prod.tfvars)

- Production-grade SKUs
- High availability configurations
- Separate resource group

---

## 🔄 Bootstrap Process Explained

### What is Bootstrap?

Bootstrap is the **one-time setup** to create the Azure Storage Account that will store your Terraform state files remotely.

### Why Needed?

- **Problem**: Terraform state tracks your infrastructure. You can't use Terraform to create its own state storage (chicken-egg problem).
- **Solution**: Run a small bootstrap Terraform config with **local state** that creates the storage account.

### Process:

1. **First**: Run `bootstrap/` Terraform (stores state locally)

   - Creates: Storage Account + Container for state files
   - Outputs: Storage account details

2. **Then**: Configure main Terraform to use remote backend

   - Update `backend.tf` with storage account details
   - Run `terraform init` to migrate local state to Azure Storage

3. **Future**: All changes auto-sync to Azure Storage
   - Team collaboration enabled
   - State file versioning and locking

### Hybrid Approach:

- Bootstrap state = Local (one-time)
- Main infrastructure state = Azure Storage (ongoing)
- Manual backups optional for extra safety

---

## 📝 Configuration Variables (tfvars)

### Core Variables Needed:

```hcl
# Project Identification
project_name = "myproject"
environment  = "dev" # or "prod"

# Azure Location
location = "eastus"

# Service Bus
servicebus_sku        = "Standard"
servicebus_queue_name = "userdata-queue"

# SQL Database
sql_admin_username = "sqladmin"
sql_database_sku   = "Basic" # or "S0" for prod

# App Service
app_service_sku = "B1" # or "S1" for prod

# Tags
tags = {
  Environment = "dev"
  ManagedBy   = "Terraform"
  Project     = "ApiServiceApp"
}
```

### github.tfvars (Template - Empty Values)

```hcl
# GitHub-related variables for CI/CD
# github_repository = ""
# github_branch     = ""
# github_token      = "" # Store in Key Vault or GitHub Secrets
```

---

## 🚀 Deployment Workflow

### Initial Setup:

```powershell
# 1. Bootstrap (one-time)
cd ../terraform/bootstrap
terraform init
terraform apply

# 2. Configure main Terraform with remote backend
cd ../
# Update backend.tf with storage account details from bootstrap output
terraform init

# 3. Deploy infrastructure
terraform plan -var-file="environments/dev.tfvars"
terraform apply -var-file="environments/dev.tfvars"
```

### Ongoing Changes:

```powershell
cd ../terraform
terraform plan -var-file="environments/dev.tfvars"
terraform apply -var-file="environments/dev.tfvars"
```

---

## 📌 Important Notes

1. ✅ **No changes to SYSTEM_ARCHITECTURE.md** - As requested
2. ✅ **Locals + Variables pattern** - All customizable via tfvars
3. ✅ **Provider**: azurerm
4. ✅ **Naming convention**: Using locals for projectname
5. ✅ **Location**: Outside Api-service-app directory
6. ✅ **Environments**: Shared modules with environment-specific tfvars
7. ⏳ **Application Insights**: Marked for future addition
8. ⏳ **VNet/Private Endpoints**: Will be added with Front Door later

---

## 🎯 Next Steps

1. **Clarify remaining questions** (if any)
2. **Generate Terraform code** with all modules and configurations
3. **Create SQL initialization script** for database schema
4. **Test bootstrap process**
5. **Deploy to dev environment**

---

**Document Version**: 1.0  
**Last Updated**: October 31, 2025  
**Status**: Ready for Terraform code generation
