
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MVC.Filters;

public class SessionManegment : IActionFilter
{
    public void OnActionExecuted(ActionExecutedContext context) { }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        context.HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        context.HttpContext.Response.Headers["Pragma"] = "no-cache";
        context.HttpContext.Response.Headers["Expires"] = "0";

        var role = context.HttpContext.Session.GetString("role");
        var controller = context.RouteData.Values["controller"]?.ToString();
        var action = context.RouteData.Values["action"]?.ToString();

        if (controller == "Auth" && (action == "Login" || action == "Register")) return;
        if (action == "Logout") return;

        if (string.IsNullOrEmpty(role))
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
            return;
        }

        if (role == "User" && controller == "Admin")
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
        }

        if (role == "Admin" && controller == "Employee")
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
        }
    }
}