namespace ONEbook.UM.Site.Filters
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using ONEbook.UM.Site.Configurations;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public sealed class UMSiteAuthorization : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext", "Filter context cannot be null!");
            }

            var currentUser = filterContext.HttpContext.User;

            if (currentUser.Identity.IsAuthenticated)
            {
                var isUserAllowed = UMSiteConfiguration.AllowedLoginAccounts.Any<string>(accountName => currentUser.Identity.Name.Contains(accountName));

                if (isUserAllowed)
                {
                    return;
                }
            }

            ShowUnauthorizedMessage(filterContext);
        }

        private void ShowUnauthorizedMessage(AuthorizationContext filterContext)
        {
            var unauthorizedResult = new ContentResult();
            unauthorizedResult.Content = "<center><b>You are not authorized!</b></center>";
            unauthorizedResult.ContentType = "text/html";
            filterContext.Result = unauthorizedResult;
        }
    }
}