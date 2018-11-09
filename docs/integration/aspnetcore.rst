ASP.NET Core
==============

Install
-------

`ONEbook.UM.AspNetCore`_

`ONEbook.UM.Transport`_


Configuration
-------------

ASP.NET Core
^^^^^^^^^^^^

Please add to your *appsettings.json* file

.. code:: json

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

Integration
-----------

Please add these code lines to your *Startup.cs* file

.. code:: csharp

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

Usage
^^^^^

.. code:: csharp

       var maintenanceService = new MaintenanceService(); 
       var underMaintenanceInfo = await maintenanceService.GetUnderMaintenance

.. _ONEbook.UM.AspNetCore: http://nuget.nexdev.net/packages/ONEbook.UM.AspNetCore/
.. _ONEbook.UM.Transport: http://nuget.nexdev.net/packages/ONEbook.UM.Transport/
.. _ONEbook.UM.AspNetMVC: http://nuget.nexdev.net/packages/ONEbook.UM.AspNetMVC/