ASP.NET MVC
==============

Install
-------

`ONEbook.UM.AspNetMVC`_

`ONEbook.UM.Transport`_

Configuration
-------------

Please add to *appSettings* section in your *web.config* file

.. code:: xml

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

Integration
-----------

Please add these code lines to your *Global.asax* file

.. code:: csharp

       var umConfig = new UMClientConfiguration();
       UMClientManager
           .UseConfig(umConfig)
           .UseRestClient(new RestClient(umConfig))
           .UseHttpContext(new HttpContextHelper())
           .AddErrorHandler(errorHandler);

Usage
^^^^^

.. code:: csharp

       var maintenanceService = new MaintenanceService(); 
       var underMaintenanceInfo = await maintenanceService.GetUnderMaintenance

.. _ONEbook.UM.AspNetCore: http://nuget.nexdev.net/packages/ONEbook.UM.AspNetCore/
.. _ONEbook.UM.Transport: http://nuget.nexdev.net/packages/ONEbook.UM.Transport/
.. _ONEbook.UM.AspNetMVC: http://nuget.nexdev.net/packages/ONEbook.UM.AspNetMVC/