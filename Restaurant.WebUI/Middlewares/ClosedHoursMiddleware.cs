using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Restaurant.Web.Middleware
{
    public class ClosedHoursMiddleware
    {
        private readonly RequestDelegate _next;

        public ClosedHoursMiddleware(RequestDelegate next)
        {
            _next = next;
        }


        // Middleware to display a "Site Closed" page during closed hours (9 PM to 10 PM)
        public async Task InvokeAsync(HttpContext context)
        {
            var now = DateTime.Now.TimeOfDay;
            var start = new TimeSpan(10, 0, 0); //
            var end = new TimeSpan(11, 0, 0);   //

            var path = context.Request.Path.Value?.ToLower();

           
            bool isMenuPage = path != null && path.Contains("/menu");

            if (now >= start && now < end && !isMenuPage)
            {
                var viewResult = await RenderViewAsync(context, "Shared/SiteClosed");
                context.Response.ContentType = "text/html; charset=utf-8";
                await context.Response.WriteAsync(viewResult);
            }
            else
            {
                await _next(context);
            }
        }

        private async Task<string> RenderViewAsync(HttpContext context, string viewPath)
        {
            var serviceProvider = context.RequestServices;
            var viewEngine = serviceProvider.GetService<ICompositeViewEngine>();
            var tempDataProvider = serviceProvider.GetService<ITempDataProvider>();

            var actionContext = new ActionContext(context, new Microsoft.AspNetCore.Routing.RouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());

            using (var sw = new StringWriter())
            {
                var viewResult = viewEngine.FindView(actionContext, viewPath, false);

                if (viewResult.Success == false)
                    throw new InvalidOperationException($"Couldn't find view '{viewPath}'.");

                var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
                var tempData = new TempDataDictionary(context, tempDataProvider);

                var viewContext = new ViewContext(actionContext, viewResult.View, viewDictionary, tempData, sw, new HtmlHelperOptions());
                await viewResult.View.RenderAsync(viewContext);

                return sw.ToString();
            }
        }
    }
}
