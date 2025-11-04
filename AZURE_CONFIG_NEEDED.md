# ⚠️ CRITICAL: Azure Configuration Required

## 🚨 YOU MUST DO THIS NOW FOR THE APP TO WORK

The code fixes have been deployed, but you need to configure Azure settings manually.

---

## 📋 Step 1: Configure Name-Age App (Static Web App)

### **What:** Add API_ENDPOINT environment variable

### **Why:** So the name-age app knows where to connect

### **Instructions:**

1. Open [Azure Portal](https://portal.azure.com)
2. Search for **"Static Web Apps"**
3. Click on your Static Web App (e.g., `name-age-app` or `brave-bush-05db6a300`)
4. In the left menu, click **"Configuration"** or **"Environment variables"**
5. Click **"+ Add"** button
6. Enter:

   ```
   Name:  API_ENDPOINT
   Value: https://test-project-api-service.azurewebsites.net
   ```

   ⚠️ **IMPORTANT:**

   - Replace `test-project-api-service` with YOUR actual App Service name
   - NO trailing slash (/)
   - NO `/api/userdata` at the end
   - Just the base URL!

7. Click **"OK"** or **"Save"**
8. Wait 1-2 minutes for restart

### **How to Find Your API Service URL:**

1. Go to Azure Portal
2. Search for **"App Services"**
3. Click on your API Service (e.g., `test-project-api-service`)
4. On the **Overview** page, copy the **"Default domain"**
5. Should look like: `https://test-project-api-service.azurewebsites.net`

---

## 📋 Step 2: Verify API Service Configuration (App Service)

### **What:** Check Service Bus connection string

### **Why:** Without this, data cannot be sent to Service Bus

### **Instructions:**

1. Open [Azure Portal](https://portal.azure.com)
2. Search for **"App Services"**
3. Click on your API Service (e.g., `test-project-api-service`)
4. In the left menu, click **"Environment variables"** or **"Configuration"**
5. Look for these settings:

   **Check #1: AZURE_SERVICEBUS_CONNECTIONSTRING**

   - ✅ Should have a long connection string starting with `Endpoint=sb://...`
   - ❌ If it's empty or missing → YOU MUST ADD IT

   **Check #2: AZURE_SERVICEBUS_QUEUENAME**

   - ✅ Should be: `userdata-queue`
   - ❌ If it's empty or missing → YOU MUST ADD IT

### **If Connection String is Missing:**

1. Go to Azure Portal → Search for **"Service Bus"**
2. Click on your Service Bus namespace (e.g., `userdata-servicebus-xyz`)
3. In the left menu, click **"Shared access policies"**
4. Click **"RootManageSharedAccessKey"**
5. Click the **Copy** button next to **"Primary Connection String"**
6. Go back to your App Service → **Configuration**
7. Click **"+ New application setting"**
8. Enter:
   ```
   Name:  AZURE_SERVICEBUS_CONNECTIONSTRING
   Value: [Paste the connection string you copied]
   ```
9. Click **"OK"**
10. Click **"+ New application setting"** again
11. Enter:
    ```
    Name:  AZURE_SERVICEBUS_QUEUENAME
    Value: userdata-queue
    ```
12. Click **"OK"**
13. Click **"Save"** at the top
14. Click **"Continue"** to restart the app

---

## ✅ After Configuration - Test Everything

### **Test 1: API Health (5 minutes after GitHub Actions completes)**

Open in browser: `https://test-project-api-service.azurewebsites.net/api/health`

**Expected:**

```json
{
  "success": true,
  "message": "API Service is running",
  "data": {
    "timestamp": "2025-11-04T...",
    "version": "1.0.0"
  }
}
```

✅ **If you see this** → API is working!
❌ **If 404 or error** → Deployment might still be running. Wait 2 more minutes.

---

### **Test 2: Name-Age App Connection (After configuring API_ENDPOINT)**

1. Open your Static Web App URL in browser
2. **Hard refresh:** Press `Ctrl + Shift + R` (or `Cmd + Shift + R` on Mac)
3. You should see: **"✅ API endpoint loaded from Azure configuration"**
4. Click **"🔌 Test API Connection"** button
5. Should see: **"✅ API connection successful! You can now submit data."**

---

### **Test 3: Submit Data**

1. Enter Name: `Test User`
2. Enter Age: `25`
3. Click **"✅ Submit Data"**
4. Should see: **"✅ Success! Data submitted for Test User, Age: 25"**

---

## 🎯 Quick Checklist

- [ ] GitHub Actions deployment completed successfully
- [ ] API health endpoint returns success
- [ ] `API_ENDPOINT` configured in Static Web App
- [ ] Service Bus connection string configured in App Service
- [ ] Name-age app loads without asking for manual input
- [ ] Test connection button shows green success
- [ ] Submit data works and shows success message

---

## 🆘 If Something Doesn't Work

### **GitHub Actions Status:**

Check: https://github.com/Malindu-D/test-project1-api-service/actions

- Look for green checkmark ✅
- If red X ❌, click to see error details

### **App Service Running:**

1. Azure Portal → Your App Service
2. Check if status is **"Running"** (green)
3. If stopped, click **"Start"**

### **Static Web App Still Shows Manual Input:**

1. Double-check `API_ENDPOINT` is configured correctly
2. Hard refresh browser (Ctrl + Shift + R)
3. Check browser console (F12) for errors

### **Test Connection Fails:**

1. Verify API Service is running
2. Test health endpoint directly
3. Check API Service logs in Azure Portal → "Log stream"

---

**Created:** November 4, 2025
**Status:** ⚠️ Waiting for Azure configuration
