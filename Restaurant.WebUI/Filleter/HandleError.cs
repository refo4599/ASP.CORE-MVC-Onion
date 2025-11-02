using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Restaurant.WebUI.Filleter
{
    public class HandleError :Attribute,IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            ViewResult result = new ViewResult();
            result.ViewName = "~/Views/Shared/Error.cshtml";
            context.Result = result;

        }
  
        }
    }