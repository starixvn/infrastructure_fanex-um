namespace ONEbook.UM.Site.Filters
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Configurations;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class UMSiteAuthorization : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (filterContext == null)
            {
                throw new ArgumentNullException(nameof(filterContext), "Filter context cannot be null!");
            }

            var currentUser = filterContext.HttpContext.User;

            if (currentUser.Identity.IsAuthenticated)
            {
                var isUserAllowed = UMSiteConfiguration.AllowedLoginAccounts.Any(accountName => currentUser.Identity.Name.Contains(accountName));

                if (isUserAllowed)
                {
                    return;
                }
            }

            ShowUnauthorizedMessage(filterContext);
        }

        private void ShowUnauthorizedMessage(AuthorizationContext filterContext)
        {
            var unauthorizedResult = new ContentResult
            {
                Content = "<center><b>You are not authorized!</b></center>",
                ContentType = "text/html"
            };
            filterContext.Result = unauthorizedResult;
        }
    }
}