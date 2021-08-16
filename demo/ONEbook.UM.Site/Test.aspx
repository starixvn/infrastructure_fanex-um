<%@ Page Language="C#" AutoEventWireup="true" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <%   
        try
        {
            var currentUser = HttpContext.Current.User;
            Response.Write(string.Format("Name: {0} <br />", currentUser.Identity.Name));
            Response.Write(string.Format("AuthenticationType: {0} <br />", currentUser.Identity.AuthenticationType));
            Response.Write(string.Format("IsAuthenticated {0} <br />", currentUser.Identity.IsAuthenticated));

            DateTime startDateTime;
            DateTime endDateTime;
            int errorCode;
            var siteId = ONEbook.UM.UMClientManager.UMClientConfigs.SiteId.ToString();
            var isUnderUM = ONELab.UMService.UMClient.ServiceClient.IsUnderUM(siteId, out startDateTime, out endDateTime, out errorCode);
            Response.Write(string.Format("siteId: {0} <br />", siteId));
            Response.Write(string.Format("errorCode: {0} <br />", errorCode));
            Response.Write(string.Format("isUnderUM: {0} <br />", isUnderUM));
            Response.Write(string.Format("startDateTime: {0} <br />", startDateTime));
            Response.Write(string.Format("endDateTime: {0} <br />", endDateTime));
        }
        catch (Exception ex)
        {
            Response.Write(ex.Message);
        }
    %>
</body>
</html>
