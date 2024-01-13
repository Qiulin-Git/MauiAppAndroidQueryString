using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebUI.Auth
{


    public class WebClientPermissionHandler : AuthorizationHandler<WebClientPermissionRequirement>
    {

        public WebClientPermissionHandler()
        {

        }



        /// <summary>
        /// 重写异步处理程序
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, WebClientPermissionRequirement requirement)
        {

            if (string.IsNullOrEmpty(context.User.Identity?.Name)
                || context.User.Identity?.IsAuthenticated != true)
            {
                context.Fail();
                return;

            }
            else
            {

                var currentUserMenuUrlList = (from item in context.User.Claims
                                              where item.Type == "MenuUrl"
                                              select item.Value).ToList();
                if (currentUserMenuUrlList.Count <= 0)
                {
                    context.Fail();
                    return;
                }


                if (context.Resource is RouteData routeData)
                {
                    var routeAttr = routeData.PageType.CustomAttributes.FirstOrDefault(x =>
                        x.AttributeType == typeof(RouteAttribute));
                    if (routeAttr == null || routeAttr.ConstructorArguments.Count <= 0)
                    {
                        context.Fail();
                        return;
                    }
                    else
                    {
                        var routeUrl = routeAttr.ConstructorArguments[0].Value as string;
                        var isMatchUrl = currentUserMenuUrlList.Exists(item => item.ToLower() == routeUrl?.ToLower());

                        if (isMatchUrl)
                        {
                            context.Succeed(requirement);
                        }
                        else
                        {
                            context.Fail();
                            return;
                        }
                    }
                }
                else
                {
                    context.Fail();
                }


            }


        }
    }
}
