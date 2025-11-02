using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.RegularExpressions;

namespace Restaurant.WebUI.Filleter
{
    public class ValidateNameFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            foreach (var arg in context.ActionArguments.Values)
            {
                if (arg == null) continue;

                var nameProp = arg.GetType().GetProperty("Name");
                if (nameProp != null)
                {
                    var name = nameProp.GetValue(arg)?.ToString();

                    if (string.IsNullOrWhiteSpace(name) || name.Length < 3)
                    {
                        SetToast(context, "❌ الاسم يجب ألا يقل عن 3 أحرف.");
                        return;
                    }

                    if (Regex.IsMatch(name, @"\d"))
                    {
                        SetToast(context, "❌ الاسم لا يجب أن يحتوي على أرقام.");
                        return;
                    }
                }
            }

            base.OnActionExecuting(context);
        }

        private void SetToast(ActionExecutingContext context, string message)
        {
            if (context.Controller is Controller controller)
            {
                controller.TempData["ToastMessage"] = message;

                var routeValues = context.RouteData.Values;
                context.Result = new RedirectToActionResult(
                    routeValues["action"]?.ToString(),
                    routeValues["controller"]?.ToString(),
                    routeValues
                );
            }
        }
    }
}
