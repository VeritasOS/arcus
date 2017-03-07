# arcus
Backup Exec Monitoring


# 1.0	DEPLOYMENT STEPS

## 1.1	Deploying Web Application in your Azure account

- Install Azure PowerShell
- Open Windows PowerShell prompt
- Set the execution policy to unrestricted only for the current session using command: Set-ExecutionPolicy -Scope Process -ExecutionPolicy Unrestricted –Force
- Load the PowerShell script DeployWebApp.ps1
- Run the PowerShell command: DeployWebApp –subscriptionId &quot;&lt;subscription Id&gt;&quot; -resourceGroupName &quot;&lt;Resource Group&gt;&quot; –siteName &quot;&lt;unique site name&gt;&quot; –hostingPlanName -&lt;hosting plane name&gt;

## 1.2	Create DocumentDB Database account

- Sign in to the Azure portal.
- In the Jump bar, click New, click Databases, and then click DocumentDB (NoSQL).
- In the New account blade, specify the desired configuration for the DocumentDB account.
- Once the new DocumentDB account options are configured, click Create. To check the status of the deployment, check the Notifications hub.
- Find the End point URL and Authorization Key in the keys blade

## 1.3	Turn on Authentication and Authorization for the Web Application deployed

- In the [Azure portal](https://portal.azure.com/), navigate to your application. Click **Settings** , and then **Authentication/Authorization**.
- If the Authentication / Authorization feature is not enabled, turn the switch to **On**.
- Click **Azure Active Directory** , and then click **Express** under **Management Mode**.
- Click **OK** to register the application in Azure Active Directory. This will create a new registration. If you want to choose an existing registration instead, click **Select an existing app** and then search for the name of a previously created registration within your tenant. Click the registration to select it and click **OK**. Then click **OK** on the Azure Active Directory settings blade.
- To restrict access to your site to only users authenticated by Azure Active Directory, set Action to take when request is not authenticated to Log in with Azure Active Directory. 
- Save the settings.

## 1.4	Add users to Azure AD

- Sign in to the [Azure classic portal](https://manage.windowsazure.com/) with an account that&#39;s a global admin for the directory.
- Select **Active Directory** , and then select the name of your organization directory.
- Select the **Users** tab, and then, in the command bar, select **Add User**.
- On the **Tell us about this user** page, under **Type of user** , select:
  -  **User with an existing Microsoft account** – adds an existing Microsoft consumer account to your directory (for example, an Outlook account)
- Depending on **Type of user** , enter email address (for a user with a Microsoft account).
- On the user **Profile** page, provide a first and last name, a user-friendly name, and a user role from the **Roles** list.
- On the **Get temporary password** page, select **Create**.

## 1.5	Provide Endpoint URL and Authorization key in Arcus portal and Download the Agent Application

- Login to the Arcus portal and provide the Endpoint URL and Authorization key to DocumentDB credentials page.
- Download the Agent Application from the portal.
- Share the Agent Application with customers. (To run the Agent Application on media servers whose data is to be monitored. )

## 1.6	Run the Agent Application on Backup Exec Media Server

- Install the service using command : &quot;C:\Windows\Microsoft.NET\Framework\v4.0.30319\installutil.exe&quot; &quot;&lt;Path to ArcusWindowsService.exe&gt;&quot;
- Agent Application runs as a windows service.



