# ServiceDesk
[![NuGet](https://img.shields.io/nuget/vpre/ServiceDeskTools.svg)](https://www.nuget.org/packages/ServiceDeskTools/)
[![Downloads](https://img.shields.io/nuget/dt/ServiceDeskTools.svg)](https://www.nuget.org/packages/ServiceDeskTools)

**Updated to .Net 5**

Set of tools to do remote administration for Windows.

## Active Directory

Use :

```csharp
var userActiveDirectory = UserActiveDirectory(adresseAD, loginAdmin, passwordAdmin);
if(userActiveDirectory.IsUserExist("john.doe"))
{
   userActiveDirectory.AddUserToGroup("john.doe", "UnidentifiedPersonGroup")
}
```
     
or with a Logger
```csharp
var userActiveDirectory = UserActiveDirectory(adresseAD, loginAdmin, passwordAdmin, logger);
if(userActiveDirectory.IsUserExist("john.doe"))
{
  userActiveDirectory.AddUserToGroup("john.doe", "UnidentifiedPersonGroup")
}
```
Out of the Logger :    
    
    Add john.doe in the group UnidentifiedPersonGroup
    

Here is the list of methods for active directory :

```csharp
public void AddComputerToGroup(string groupName, ComputerPrincipal computer);
public void AddComputerToGroup(string groupName, string computerName);
public void AddUserToGroup(string groupName, string userName);
public void AddUserToGroup(string groupName, UserPrincipal user);
public IEnumerable<string> GetComputerGroupes(string machineName);
public HomeDirectory GetLecteurReseau(string userName);
public UserPrincipal GetUser(string name);
public IEnumerable<string> GetUserGroups(string userName);
public Task<string> GetUserSid(string userName);
public bool IsComputerExist(string machineName);
public Task<bool> IsGroupExist(string groupName);
public bool IsUserExist(string userName);
public bool IsUserMemberOf(string userName, string groupName);
public void RemoveComputerToGroup(string groupName, string computerName);
public void RemoveLecteurReseau(string userName, string directory);
public void RemoveUserToGroup(string groupName, string userName);
public void RemoveUserToGroup(string groupName, UserPrincipal user);
public void SetLecteurReseau(string userName, string lettreReseau, string directory);
```

## WMI

Use :
```csharp
var wmiManager = new WmiManagement(yourLoginAdmin, yourPasswordAdmin, logger);
List<Software> softwares = await wmiManager.GetListSofware("computerName");

// checking for a directory
string pathDirectory = @"C:\\Your\\Path\\Directory";
string wqlDirectory = string.Format(@"SELECT * FROM Win32_Directory Where Name='{0}'", pathDirectory);

if(await wmiManager.IsDirectoryExistAsync(nomMachine, wqlDirectory))
{
  logger.Success(pathDirectory + " exist");
}
else
{
  logger.Warn(pathDirectory + " not exist");
}
```


## ILogger
The "logger" is type of `ILogger` in `ServiceDeskToolsCore.ILogger`:

```csharp
/// <summary>
/// Message pour Debug
/// </summary>
/// <param name="message"></param>
void Debug(string message);

/// <summary>
/// Message d'erreur avec son exception
/// </summary>
/// <param name="message"></param>
/// <param name="exception"></param>
void Error(string message, Exception exception);

/// <summary>
/// Message d'erreur sans Exception !
/// </summary>
/// <param name="message"></param>
void Error(string message);

/// <summary>
/// Message d'information
/// </summary>
/// <param name="messageInfo"></param>
void Info(string messageInfo);

/// <summary>
/// Message d'information success
/// </summary>
/// <param name="message"></param>
void Success(string message);

/// <summary>
/// Message d'attention
/// </summary>
/// <param name="messageWarn"></param>
void Warn(string messageWarn);
```
