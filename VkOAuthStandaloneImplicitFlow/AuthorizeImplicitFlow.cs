using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VkOAuthStandaloneImplicitFlow
{
    public static class AuthorizeImplicitFlow
    {
        public static AuthDetails Authorize(string appId, string permissionsScope = "friends", string displayMode = "page")
        {
            var callbackListener = new CallbackListener(3000);
            var callbackUrl = callbackListener.CallbackUrl;
            var parameters = new Dictionary<string, string>
            {
                { "client_id", appId },
                { "scope", permissionsScope },
                { "display", displayMode },
                { "redirect_uri", callbackUrl },
                { "response_type", "token" },
                { "v", "5.50" }
            };

            var urlParametersArray = parameters.Select(pair => $"{HttpUtility.UrlEncode(pair.Key)}={HttpUtility.UrlEncode(pair.Value)}").ToArray();
            var urlParametersString = string.Join("&", urlParametersArray);
            System.Diagnostics.Process.Start($"https://oauth.vk.com/authorize?{urlParametersString}");
            return callbackListener.GetAuthResponse();
        }
    }
}
