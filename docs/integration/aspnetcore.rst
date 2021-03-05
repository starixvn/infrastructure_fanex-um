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
            services.AddSession();
            services.AddSingleton(new UMClientConfiguration().Initialize(Configuration));
            services.AddSingleton<IUMCacheService, UMCacheService>();
            services.AddSingleton<IUMRestClient, UMRestClient>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IHttpContextHelper, HttpContextHelper>();
            services.AddSingleton<IMaintenanceService, MaintenanceService>();
            ...
       }

Usage
^^^^^

.. code:: csharp

        private readonly IMaintenanceService maintenanceService;

        public HomeController(IMaintenanceService maintenanceService)
        {
            this.maintenanceService = maintenanceService;
        }

.. _ONEbook.UM.AspNetCore: http://nuget.nexdev.net/packages/ONEbook.UM.AspNetCore/
.. _ONEbook.UM.Transport: http://nuget.nexdev.net/packages/ONEbook.UM.Transport/
.. _ONEbook.UM.AspNetMVC: http://nuget.nexdev.net/packages/ONEbook.UM.AspNetMVC/