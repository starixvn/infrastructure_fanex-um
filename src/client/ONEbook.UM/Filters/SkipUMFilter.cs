#if NET5_0_OR_GREATER
namespace ONEbook.UM.Filters;

using System;
using Microsoft.AspNetCore.Mvc.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class SkipUMFilterAttribute: ActionFilterAttribute;

#endif