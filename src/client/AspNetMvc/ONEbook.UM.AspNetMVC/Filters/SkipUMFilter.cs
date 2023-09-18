using System;
using System.Web.Mvc;

namespace ONEbook.UM.AspNetMVC.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public sealed class SkipUMFilterAttribute : ActionFilterAttribute
    {

    }
}