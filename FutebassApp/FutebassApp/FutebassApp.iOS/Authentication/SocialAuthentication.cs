using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using FutebassApp.Authentication;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using FutebassApp.Helpers;
using FutebassApp.iOS.Authentication;

[assembly: Xamarin.Forms.Dependency(typeof(SocialAuthentication))]
namespace FutebassApp.iOS.Authentication
{
    public class SocialAuthentication : IAuthentication
    {
        public async Task<MobileServiceUser> LoginAsync(MobileServiceClient client,
                                                       MobileServiceAuthenticationProvider provider,
                                                       IDictionary<string, string> parametrs = null)
        {
            try
            {
                var current = GetController();
                var user = await client.LoginAsync(current, provider);

                Settings.AuthToken = user?.MobileServiceAuthenticationToken ?? string.Empty;
                Settings.UserId = user?.UserId;

                return user;
            }
            catch (Exception ex)
            {
                //TODO: Log error
                throw;
            }
        }

        public Task LogoutAsync(MobileServiceClient client, IDictionary<string, string> parameters = null)
        {
            throw new NotImplementedException();
        }

        private UIKit.UIViewController GetController()
        {
            var window = UIKit.UIApplication.SharedApplication.KeyWindow;
            var root = window.RootViewController;

            if (root == null) return null;

            var current = root;
            while (current.PresentedViewController != null)
            {
                current = current.PresentedViewController;
            }

            return current;
        }
    }
}