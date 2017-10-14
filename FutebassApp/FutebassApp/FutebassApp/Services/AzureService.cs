using FutebassApp.Authentication;
using FutebassApp.Helpers;
using FutebassApp.Models;
using FutebassApp.Services;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(AzureService))]
namespace FutebassApp.Services
{
    public class AzureService
    {
        static readonly string AppUrl = "https://futebassapp.azurewebsites.net";
        static readonly string BaseUrl = "https://futebassapp.azurewebsites.net/api/";

        public MobileServiceClient Client { get; set; } = null;
        public static bool UserAuth { get; set; } = false;

        public void Initialize()
        {
            Client = new MobileServiceClient(AppUrl);

            if (!string.IsNullOrWhiteSpace(Settings.AuthToken) && !string.IsNullOrWhiteSpace(Settings.UserId))
            {
                Client.CurrentUser = new MobileServiceUser(Settings.UserId)
                {
                    MobileServiceAuthenticationToken = Settings.AuthToken
                };
            }
        }

        public async Task Logout()
        {
            Initialize();

            var auth = DependencyService.Get<IAuthentication>();
            await auth.LogoutAsync(Client);

            Settings.AuthToken = string.Empty;
            Settings.UserId = string.Empty;
            Settings.JogadorId = 0;
        }

        public async Task<bool> LoginAsync()
        {
            Initialize();

            var auth = DependencyService.Get<IAuthentication>();
            var user = await auth.LoginAsync(Client, MobileServiceAuthenticationProvider.Facebook);

            if (user == null)
            {
                Settings.AuthToken = string.Empty;
                Settings.UserId = string.Empty;
                Settings.JogadorId = 0;

                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert("Ops!", "Não conseguimos efetuar o seu login, tente novamente!", "OK");
                });

                return false;
            }
            var _userId = user.UserId.Split(':')[1];
            Settings.AuthToken = user.MobileServiceAuthenticationToken;
            Settings.UserId = _userId;

            GetUserInformationAsync();

            return true;
        }

        public async void GetUserInformationAsync()
        {
            try
            {
                if (!await this.validaJogador(Settings.UserId))
                {
                    var httpClient = new HttpClient();
                    var appServiceIdentities = await Client.InvokeApiAsync<List<AppServiceIdentity>>("/.auth/me");

                    if (appServiceIdentities.Count <= 0)
                        return;

                    var appServiceIdentity = appServiceIdentities[0].access_token;

                    var facebookUrl = $"https://graph.facebook.com/v2.9/me?fields=id%2Cname%2Cpicture.type(large)%7Burl%7D%2Cfirst_name%2Clast_name%2Cbirthday%2Cemail%2Cgender%2Clocation&access_token={appServiceIdentity}";

                    var userJson = await httpClient.GetStringAsync(facebookUrl);
                    var facebookProfile = JsonConvert.DeserializeObject<FacebookUser>(userJson);

                    var jogador = new Jogador();
                    jogador.Nome = facebookProfile.name;
                    jogador.Foto = facebookProfile.picture.data.url;
                    jogador.Cidade = facebookProfile.location == null ? "Dado não público" : facebookProfile.location;
                    jogador.IdSocial = Settings.UserId;
                    jogador.Nivel = "Perna de pau";

                    var saveuser = SalvarJogador(jogador);
                }

            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Ops!", e.Message, "OK");
                throw;
            }
        }

        #region API PARTIDAS

        public async Task<List<Partida>> GetPartidasAsync()
        {
            try
            {
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("X-ZUMO-AUTH", Settings.AuthToken);

                var response = await httpClient.GetAsync($"{BaseUrl}partida").ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    {
                        var retorno = JsonConvert.DeserializeObject<List<Partida>>(
                            await new StreamReader(responseStream)
                                .ReadToEndAsync().ConfigureAwait(false));
                        return retorno;
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Ops!", e.Message, "OK");
                throw;
            }
        }

        public async Task<bool> SalvarPartida(Partida partida)
        {
            partida.JogadorId = Settings.JogadorId;

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("X-ZUMO-AUTH", Settings.AuthToken);

                    var httpContent = new StringContent(JsonConvert.SerializeObject(partida).ToString(), Encoding.UTF8, "application/json");

                    var response = client.PostAsync($"{BaseUrl}partida", httpContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                        {
                            var content = JsonConvert.DeserializeObject<Partida>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

                            if (content != null)
                                return true;
                        }
                    }
                }

                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert("Ops!", "Não conseguimos salvar seus dados, tente novamente!", "OK");
                });

                return false;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Ops!", e.Message, "OK");
                throw;
            }
        }

        #endregion

        #region API Jogador

        public async Task<bool> SalvarJogador(Jogador jogador)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("X-ZUMO-AUTH", Settings.AuthToken);

                    var httpContent = new StringContent(JsonConvert.SerializeObject(jogador).ToString(), Encoding.UTF8, "application/json");

                    var response = client.PostAsync($"{BaseUrl}jogador", httpContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                        {
                            var content = JsonConvert.DeserializeObject<Jogador>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

                            if (content != null)
                            {
                                Settings.JogadorId = content.JogadorId;
                                return true;
                            }

                        }
                    }
                }

                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert("Ops!", "Não conseguimos salvar seus dados, tente novamente!", "OK");
                });

                return false;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Ops!", e.Message, "OK");
                throw;
            }
        }

        public async Task<bool> validaJogador(string id)
        {
            try
            {
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("X-ZUMO-AUTH", Settings.AuthToken);

                var response = await httpClient.GetAsync($"{BaseUrl}jogador/{id}").ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    {
                        var content = JsonConvert.DeserializeObject<Jogador>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

                        if (content != null)
                        {
                            Settings.JogadorId = content.JogadorId;
                            return true;
                        }
                    }
                }

                return false;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Ops!", e.Message, "OK");
                throw;
            }
        }


        public Jogador getJogadorById(string id)
        {
            try
            {
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("X-ZUMO-AUTH", Settings.AuthToken);

                var response = httpClient.GetAsync($"{BaseUrl}jogador/{id}").Result;

                if (response.IsSuccessStatusCode)
                {
                    using (var responseStream = response.Content.ReadAsStreamAsync().Result)
                    {
                        var content = JsonConvert.DeserializeObject<Jogador>( response.Content.ReadAsStringAsync().Result);

                        if (content != null)
                        {
                            return content;
                        }
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                Application.Current.MainPage.DisplayAlert("Ops!", e.Message, "OK");
                throw;
            }
        }

        #endregion
    }
}
