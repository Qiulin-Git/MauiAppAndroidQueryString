using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace WebUI.Auth
{
    public static class ServiceCollectionExtension
    {

        public static IServiceCollection AddAuth(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

               services.AddBlazoredLocalStorage(config =>
               {
                  config.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                  config.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
                  config.JsonSerializerOptions.WriteIndented = true;
               })
                .AddAuthorizationCore(options =>
                 {

                     options.AddPolicy("WebClientPermission",
                              policy => policy.Requirements.Add(new WebClientPermissionRequirement()));
                 }) 
                 .AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>()
                 .AddSingleton<IAuthorizationHandler, WebClientPermissionHandler>()
                 .AddSingleton(new WebClientPermissionRequirement());


            return services;

        }

    }

}
