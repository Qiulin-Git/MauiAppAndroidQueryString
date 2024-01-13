using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace WebUI.Auth
{
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        public ApiAuthenticationStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            //设置客户端 ClientVisitorId
           var authName = await GetAuthNameFromLocalStorage();

         
            if ( string.IsNullOrEmpty(authName))
            {
                await MarkUserAsLoggedOut(); 
                return new AuthenticationState(new ClaimsPrincipal());
            }
            else
            {
                var claims = new List<Claim>
                {
                     new Claim(ClaimTypes.Name, authName),
                };

                claims.Add(new Claim("MenuUrl", "/"));
                claims.Add(new Claim("MenuUrl", "/counter"));


                var identity = new ClaimsIdentity(claims, "ApiAuth");

                var authenticatedUser = new ClaimsPrincipal(identity);

                return new AuthenticationState(authenticatedUser);
            }


        }

        public async Task MarkUserAsAuthenticated(string userName)
        {
            if ( !string.IsNullOrEmpty(userName))
            {
                await SetAuthNameToLocalStorage(userName);
            }

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        /// <summary>
        /// 标记注销-退出登录
        /// </summary>
        public async Task MarkUserAsLoggedOut()
        {

            await RemoveAuthNameFromLocalStorage();

            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }



        public async Task SetAuthNameToLocalStorage(string userName)
        {
           await _localStorage.SetItemAsStringAsync("AuthName", userName);
        }


        public async Task<string> GetAuthNameFromLocalStorage()
        {
             string authName =string.Empty;    
             var itemExist = await _localStorage.ContainKeyAsync("AuthName");

             if (itemExist)
             {
                  authName = await _localStorage.GetItemAsStringAsync("AuthName");
             }
            return authName;

        }



        public async Task RemoveAuthNameFromLocalStorage()
        {
            //判断键值是否存在，存在就删除
            var itemExist = await _localStorage.ContainKeyAsync("AuthName");
            if (itemExist)
            {
                await _localStorage.RemoveItemAsync("AuthName");
            }

        }
    }
}
