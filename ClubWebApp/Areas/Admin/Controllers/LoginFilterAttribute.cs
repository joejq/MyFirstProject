using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;

namespace ClubWebApp.Areas.Admin.Controllers
{
    public class LoginFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            //string request= actionContext.RouteData.Values[]
            if(actionContext.HttpContext.Session["username"] == null || actionContext.HttpContext.Session["pass"] == null)
            {
                actionContext.HttpContext.Response.Redirect("/Admin/Login/Index", true);
            }
           // base.OnActionExecuting(actionContext);
        }
    }
}