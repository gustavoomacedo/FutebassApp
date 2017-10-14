using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FutebassApp.Authentication;
using FutebassApp.Helpers;
using Microsoft.WindowsAzure.MobileServices;
using FutebassApp.Droid.Authentication;
using Xamarin.Forms;
using Android.Webkit;

[assembly: Xamarin.Forms.Dependency(typeof(SocialAuthentication))]
namespace FutebassApp.Droid.Authentication
{
    public class SocialAuthentication : IAuthentication
    {
        public async Task<MobileServiceUser> LoginAsync(MobileServiceClient client, MobileServiceAuthenticationProvider provider, IDictionary<string, string> parameters = null)
        {
            try
            {
                var user = await client.LoginAsync(Forms.Context, provider);
                
                Settings.AuthToken = user?.MobileServiceAuthenticationToken ?? string.Empty;
                Settings.UserId = user?.UserId;
                
                return user;
            }
            catch (Exception)
            {
                //TODO: LogError
                return null;
            }
        }

        public async Task LogoutAsync(MobileServiceClient client, IDictionary<string, string> parameters = null)
        {
            try
            {
                CookieManager.Instance.RemoveAllCookie();
                await client.LogoutAsync();
            }
            catch (Exception)
            {
                // TODO: Log error
                throw;
            }
        }
    }
}