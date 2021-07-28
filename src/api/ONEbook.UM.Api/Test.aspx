<%@ Page Language="C#" AutoEventWireup="true" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <%
        DateTime startDateTime;
        DateTime endDateTime;
        int errorCode;
        var siteId = "2000000";
        var isUnderUM = ONELab.UMService.UMClient.ServiceClient.IsUnderUM(siteId, out startDateTime, out endDateTime, out errorCode);
        Response.Write(string.Format("errorCode: {0} <br />", errorCode));
        Response.Write(string.Format("isUnderUM: {0} <br />", isUnderUM));
        Response.Write(string.Format("startDateTime: {0} <br />", startDateTime));
        Response.Write(string.Format("endDateTime: {0} <br />", endDateTime));
    %>
</body>
</html>
