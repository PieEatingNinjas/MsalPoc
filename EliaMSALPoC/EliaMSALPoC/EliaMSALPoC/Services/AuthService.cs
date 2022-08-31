using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Xamarin.Essentials;

namespace EliaMSALPoC.Services
{
	public class AuthService
	{
           readonly string AppId = "be.elia.msalpoc2"; //PackageName
           readonly string ClientID = "ad5fd2ea-ace8-4abf-a58b-6f8aa13cbc3e";
           readonly string RedirectUri = "msauth.be.elia.msalpoc2://auth";
            //readonly string AppId = "be.elia.msalpoc"; //PackageName
            //readonly string ClientID = "ad5fd2ea-ace8-4abf-a58b-6f8aa13cbc3e";
            //readonly string RedirectUri = "msauth.be.elia.msalpoc://auth";
        readonly IPublicClientApplication _pca;

        readonly string[] Scopes = { "User.Read" };//what's this?

        public AuthService()
        {
            _pca = PublicClientApplicationBuilder.Create(ClientID)
               .WithIosKeychainSecurityGroup(AppId)
               .WithRedirectUri(RedirectUri)
               //.WithAuthority("https://login.microsoftonline.com/common")
               .Build();
        }


        public async Task<bool> SignInAsync()
        {
            try
            {
                var accounts = await _pca.GetAccountsAsync();
                var firstAccount = accounts.FirstOrDefault();
                var authResult = await _pca.AcquireTokenSilent(Scopes, firstAccount).ExecuteAsync();

                // Store the access token securely for later use.
                await SecureStorage.SetAsync("AccessToken", authResult?.AccessToken);

                return true;
            }
            catch (MsalUiRequiredException)
            {
                try
                {
                    // This means we need to login again through the MSAL window.
                    var authResult = await _pca.AcquireTokenInteractive(Scopes)
                        .WithAuthority("https://login.microsoftonline.com/c018907c-994c-4ac6-a10c-5a7aab1a4f52/oauth2/v2.0/authorize")
                                              //  .WithParentActivityOrWindow(ParentWindow)
                                             // .WithUseEmbeddedWebView(true)
                                                .ExecuteAsync();

                    // Store the access token securely for later use.
                    await SecureStorage.SetAsync("AccessToken", authResult?.AccessToken);

                    return true;
                }
                catch (Exception ex2)
                {
                    Debug.WriteLine(ex2.ToString());
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }

        public async Task<bool> SignOutAsync()
        {
            try
            {
                var accounts = await _pca.GetAccountsAsync();

                // Go through all accounts and remove them.
                while (accounts.Any())
                {
                    await _pca.RemoveAsync(accounts.FirstOrDefault());
                    accounts = await _pca.GetAccountsAsync();
                }

                // Clear our access token from secure storage.
                SecureStorage.Remove("AccessToken");

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }
    }
}

