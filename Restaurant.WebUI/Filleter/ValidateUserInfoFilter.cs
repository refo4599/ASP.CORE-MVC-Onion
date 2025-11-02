using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.RegularExpressions;

namespace Restaurant.WebUI.Filters
{
    public class ValidateUserInfoFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            foreach (var arg in context.ActionArguments.Values)
            {
                if (arg == null) continue;

                // verify full name
                var nameProp = arg.GetType().GetProperty("FullName");
                if (nameProp != null)
                {
                    var name = nameProp.GetValue(arg)?.ToString();
                    if (string.IsNullOrWhiteSpace(name) || name.Length <= 3)
                    {
                        SetToast(context, " The name must be at least 3 letters long..");
                        return;
                    }
                }

                //  Verify phone number
                var phoneProp = arg.GetType().GetProperty("Phone");
                if (phoneProp != null)
                {
                    var phone = phoneProp.GetValue(arg)?.ToString();
                    if (string.IsNullOrWhiteSpace(phone) || phone.Length != 11 || !phone.All(char.IsDigit))
                    {
                        SetToast(context, " The phone number must consist of 11 digits, with no symbols or letters..");
                        return;
                    }
                }

                //  Verify email
                var emailProp = arg.GetType().GetProperty("Email");
                if (emailProp != null)
                {
                    var email = emailProp.GetValue(arg)?.ToString();
                    if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
                    {
                        SetToast(context, " Email is invalid.");
                        return;
                    }
                }
            }

            base.OnActionExecuting(context);
        }

        public bool IsValidEmail(string email)
        {
           
            if (string.IsNullOrWhiteSpace(email)) return false;

            var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
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
