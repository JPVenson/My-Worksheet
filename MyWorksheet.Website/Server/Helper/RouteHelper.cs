using System;
using System.Reflection;
using MyWorksheet.Website.Server.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace MyWorksheet.Webpage.Helper;

public static class RouteHelper
{
    public static string GetRoute(Type controller, string function)
    {
        var prefix = controller.GetCustomAttribute<RouteAttribute>()?.Template ?? controller.Name;
        var func = controller.GetMethod(function);
        var functionName = func.GetCustomAttribute<RouteAttribute>()?.Template ?? func.Name;
        return prefix + "/" + functionName;
    }

    public static string GetRoute<TController>(string function) where TController : ApiControllerBase
    {
        return GetRoute(typeof(TController), function);
    }
}