# ONEbook.UM 3.0

New ONEbook.UM version will support .NET Standard and apply new UM Tool to get scheduled UM information.

## Install
#### ASP.NET Core 
[ONEbook.UM.AspNetCore](http://nuget.nexdev.net/packages/ONEbook.UM.AspNetCore/3.0.0-beta)

[ONEbook.UM.Transport](http://nuget.nexdev.net/packages/ONEbook.UM.Transport/3.0.0-beta)

#### ASP.NET Mvc
[ONEbook.UM.AspNetMVC](http://nuget.nexdev.net/packages/ONEbook.UM.AspNetMVC/3.0.0-beta)

[ONEbook.UM.Transport](http://nuget.nexdev.net/packages/ONEbook.UM.Transport/3.0.0-beta)

## Configuration
#### ASP.NET Core
Please add to your *appsettings.json* file

```json
    "ONEbook.UM": {
        "ServiceUri": "http://ha.nexdev.net:8889/api",
        "EnableCheckUM": true,
        "SiteId": 200000,
        "EnableUnscheduledUM": false,
        "UMFrom": "3:00",
        "UMTo": "4:00",
        "UMPageUrl": "/UserLogin/Common/Offline",
        "ResultCacheDuration": 60,
        "UMLoggingCategory": "ONEbook-UM",
        "ErrorUMPageUrl": ""
    },
```

#### ASP.NET MVC
Please add to *appSettings* section in your *web.config* file

```xml
    <add key="ONEbook.UM:ServiceUri" value="http://ha.nexdev.net:8889/api" />
    <add key="ONEbook.UM:EnableCheckUM" value="true" />
    <add key="ONEbook.UM:SiteId" value="2000000" />
    <add key="ONEbook.UM:EnableUnscheduledUM" value="false" />
    <add key="ONEbook.UM:UMFrom" value="3:00" />
    <add key="ONEbook.UM:UMTo" value="4:00" />
    <add key="ONEbook.UM:UMPageUrl" value="/UserLogin/Common/Offline" />
    <add key="ONEbook.UM:ResultCacheDuration" value="60" />
    <add key="ONEbook.UM:UMLoggingCategory" value="ONEbook-UM" />
    <add key="ONEbook.UM:ErrorUMPageUrl" value="" />
```

#### Service Url
- LOCAL: http://ha.nexdev.net:8889/api
- UAT http://10.40.40.248:8889/api
- PROD: http://10.40.40.47:7435/api

## Integration
#### ASP.NET Core
Please add these code lines to your *Startup.cs* file

```csharp
    public void ConfigureServices(IServiceCollection services){
        ...
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        ...
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env){
        ...
        var umConfig = new UMClientConfiguration(Configuration);
        UMClientManager
            .UseConfig(umConfig)
            .UseRestClient(new RestClient(umConfig))
            .UseHttpContext(new HttpContextHelper(app.ApplicationServices.GetService<IHttpContextAccessor>()))
            .AddErrorHandler(errorHandler);
    }
```

#### ASP.NET MVC
Please add these code lines to your *Global.asax* file

```csharp
    var umConfig = new UMClientConfiguration();
    UMClientManager
        .UseConfig(umConfig)
        .UseRestClient(new RestClient(umConfig))
        .UseHttpContext(new HttpContextHelper())
        .AddErrorHandler(errorHandler);
```

#### Usage

```csharp
    var maintenanceService = new MaintenanceService();
    var underMaintenanceInfo = await maintenanceService.GetUnderMaintenanceTime();
```


