Backdoor to access site in UM
==================================
ONEbook.UM 3.0 keeps using UMToken like before for both ASP.NET MVC and ASP.NET Core

UM Token
~~~~~~~~~~~~~

- Get UM Token by access this url: http://ha.nexdev.net:7255/token/index
- UM Token is specific for each ONEbook.UM.SiteID (configure in Web.config or AppSettings.json)
- Currently, only users in SAS team can have permission to access. Please contact RnD team if you want to have permission

Access Page in UM
~~~~~~~~~~~~~~~~~~~~~~~~
http://<your_page_url>?UMToken=<UMToken>

