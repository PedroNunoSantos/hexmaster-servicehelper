# ServiceDebugger

![Screen Shot](.\Graphics\screenshot.png)

Service Debugger is a helper utility that allows debugging Windows Services

If you've created a service in Visual Studio.NET, the program.cs contains the following line:
```csharp 
ServiceBase.Run(ServicesToRun);
```

After installing the service helper, change this line like so:
<br>

```csharp

 ServiceBase[] servicesToRun = { new Service1() };
 
ServiceDebugger.Helper.Run(ServicesToRun);

// or

using ServiceDebugger;
...
ServicesToRun.Run();
```

Now if you run your project with a debugger attached, you will be able to run the
service from a window that pops up, else the service will run as is would without
having the service helper installed.

## App.Config AppSettings
**ServiceDebugger.AutoStart=true**<br>
Starts all services immediately

**ServiceDebugger.RunEvenIfNotAttached=true**<br>
Start's even if there is no debugger attached to the process.

**ServiceDebugger.StartMinimized=true**<br>
Window will start minimized (click on the notify icon to restore window)

### Example
```xml
<configuration>
	<appSettings>
		<add key="ServiceDebugger.AutoStart" value="true"/>
		<add key="ServiceDebugger.RunEvenIfNotAttached" value="true"/>
		<add key="ServiceDebugger.StartMinimized" value="true"/>
	</appSettings>
</configuration>
