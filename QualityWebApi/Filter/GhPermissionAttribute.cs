using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using QualityWebApi.Filter.BLL;
using QualityWebApi.Filter.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QualityWebApi.Filter
{
    public class GhPermissionAttribute: ActionFilterAttribute
    {
         public string AuthId { get; set; }
        //private AudienceService _services { get; set; }
        public GhPermissionAttribute()
        {

        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string userId = context.HttpContext.User.Identity.Name;
            if(string.IsNullOrWhiteSpace(AuthId))
            {
                context.Result = new StatusCodeResult(405);
                return;
            }
            if (context.HttpContext.User.Identity.IsAuthenticated==false)
            {
                context.Result = new StatusCodeResult(405);
                return;
            }
            AudienceService services = context.HttpContext.RequestServices.GetService(typeof(AudienceService)) as AudienceService;
            Permission permission = services.GetPermissingById(this.AuthId, userId);
            if (permission == null)
            {

                context.Result = new StatusCodeResult(405);
                return;
            }
            base.OnActionExecuting(context);
        }
    }
}
