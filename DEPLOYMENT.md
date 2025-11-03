# API Service App - Deployment Guide

## 📋 Prerequisites

- Azure Account
- GitHub Account
- Git installed on your computer

## 🚀 Step-by-Step Deployment

### Step 1: Create Azure Resources

#### 1.1 Create Azure App Service

1. Go to [Azure Portal](https://portal.azure.com)
2. Click **"Create a resource"**
3. Search for **"Web App"** and click **Create**
4. Fill in the details:
   - **Subscription**: Select your subscription
   - **Resource Group**: Create new or select existing (e.g., `user-data-system-rg`)
   - **Name**: Enter unique name (e.g., `api-service-app-xyz123`)
   - **Publish**: Select **Code**
   - **Runtime stack**: Select **.NET 8 (LTS)**
   - **Operating System**: **Windows** or **Linux**
   - **Region**: Select nearest region
   - **Pricing Plan**: Select **Free F1** or **Basic B1**
5. Click **Review + Create**, then **Create**
6. Wait for deployment to complete (2-3 minutes)

#### 1.2 Create Azure Service Bus

1. In Azure Portal, click **"Create a resource"**
2. Search for **"Service Bus"** and click **Create**
3. Fill in the details:
   - **Subscription**: Select your subscription
   - **Resource Group**: Use same as above (e.g., `user-data-system-rg`)
   - **Namespace name**: Enter unique name (e.g., `userdata-servicebus-xyz`)
   - **Location**: Same region as App Service
   - **Pricing tier**: Select **Basic** (enough for this project)
4. Click **Review + Create**, then **Create**
5. Wait for deployment to complete
6. After creation, go to the Service Bus resource
7. Click **"Queues"** in left menu → Click **"+ Queue"**
8. **Name**: `userdata-queue`
9. Leave other settings as default, click **Create**

#### 1.3 Get Service Bus Connection String

1. In your Service Bus resource, click **"Shared access policies"** (left menu)
2. Click **"RootManageSharedAccessKey"**
3. **Copy** the **"Primary Connection String"** - Save it somewhere safe!

### Step 2: Push Code to GitHub

1. Open Terminal/PowerShell in your `Api-service-app` folder
2. Run these commands:

```powershell
git init
git add .
git commit -m "Initial commit - API Service App"
```

3. Go to [GitHub](https://github.com) and create a **new repository**
   - Name: `api-service-app`
   - Make it **Private** or **Public**
   - **Don't** initialize with README
   - Click **Create repository**
4. Copy the commands shown and run them in your terminal:

```powershell
git remote add origin https://github.com/YOUR-USERNAME/api-service-app.git
git branch -M main
git push -u origin main
```

### Step 3: Configure GitHub Actions

#### 3.1 Get Azure Publish Profile

1. Go to Azure Portal → Your App Service (`api-service-app-xyz123`)
2. Click **"Get publish profile"** at the top (Download button)
3. A file will download - open it with Notepad
4. **Copy all the content** of this file

#### 3.2 Add Secrets to GitHub

1. Go to your GitHub repository
2. Click **Settings** → **Secrets and variables** → **Actions**
3. Click **"New repository secret"**
4. Add first secret:
   - **Name**: `AZURE_WEBAPP_PUBLISH_PROFILE`
   - **Value**: Paste the publish profile content you copied
   - Click **Add secret**
5. Add second secret:
   - Click **"New repository secret"** again
   - **Name**: `AZURE_WEBAPP_NAME`
   - **Value**: Your app service name (e.g., `api-service-app-xyz123`)
   - Click **Add secret**

### Step 4: Configure Environment Variables in Azure

1. Go to Azure Portal → Your App Service
2. Click **"Environment variables"** in left menu (or **Configuration**)
3. Click **"+ New application setting"** and add these:

**First Variable:**

- **Name**: `AZURE_SERVICEBUS_CONNECTIONSTRING`
- **Value**: Your Service Bus connection string (from Step 1.3)
- Click **OK**

**Second Variable:**

- **Name**: `AZURE_SERVICEBUS_QUEUENAME`
- **Value**: `userdata-queue`
- Click **OK**

**Third Variable:**

- **Name**: `JAVA_EMAIL_SERVICE_URL`
- **Value**: (Leave empty for now - you'll add this after deploying Java Email Service)
- Click **OK**

4. Click **"Save"** at the top, then **"Continue"**
5. Wait for app to restart

### Step 5: Deploy Using GitHub Actions

1. Go to your GitHub repository
2. Click **Actions** tab at the top
3. You should see a workflow running (triggered by your push)
4. If not, click on the workflow name and click **"Run workflow"**
5. Wait for deployment to complete (green checkmark ✅)
6. If it fails (red X), click on it to see errors

### Step 6: Verify Deployment

1. Go to Azure Portal → Your App Service
2. Click **"Browse"** at the top
3. Your browser will open - add `/api/health` to the URL
   - Example: `https://api-service-app-xyz123.azurewebsites.net/api/health`
4. You should see JSON response:

```json
{
  "success": true,
  "message": "API Service is running",
  "data": {
    "timestamp": "2025-10-30T...",
    "version": "1.0.0"
  }
}
```

## ✅ Success!

Your API Service App is now deployed and running on Azure!

## 📝 Important URLs to Save

- **App Service URL**: `https://YOUR-APP-NAME.azurewebsites.net`
- **Health Check**: `https://YOUR-APP-NAME.azurewebsites.net/api/health`
- **User Data Endpoint**: `https://YOUR-APP-NAME.azurewebsites.net/api/userdata`
- **Email Endpoint**: `https://YOUR-APP-NAME.azurewebsites.net/api/email/send`

## 🔄 Future Updates

To update the app after code changes:

1. Make your code changes
2. Commit and push to GitHub:

```powershell
git add .
git commit -m "Your change description"
git push
```

3. GitHub Actions will automatically deploy the update!

## ⚠️ Troubleshooting

**Problem**: Deployment fails in GitHub Actions

- **Solution**: Check if publish profile is correct in GitHub secrets

**Problem**: App shows error 500

- **Solution**: Check environment variables in Azure App Service Configuration

**Problem**: Service Bus connection error

- **Solution**: Verify Service Bus connection string in environment variables

**Problem**: Can't access the app

- **Solution**: Make sure App Service is running (Start it from Azure Portal if stopped)

## 🔗 Next Steps

After this deployment, you need to:

1. Deploy **Test Function App** (to process Service Bus messages)
2. Deploy **Java Email Service** (then add its URL to `JAVA_EMAIL_SERVICE_URL` variable)
3. Deploy **Name-Age App** (frontend to send data here)
4. Deploy **Email Notification App** (frontend to trigger emails)
