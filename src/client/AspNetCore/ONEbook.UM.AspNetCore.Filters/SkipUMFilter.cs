using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace ONEbook.UM.AspNetCore.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class SkipUMFilterAttribute: ActionFilterAttribute
    {
    }
}
