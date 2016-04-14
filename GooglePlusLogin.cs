using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Drawing;
using Gabriel.Cat.Extension;
namespace Gabriel.Cat.Google
{
    public class GooglePlusUser
    {
        class Token
        {
            public Token() { }
           [JsonProperty("access_token")]
            public string AccessToken { get; private set; }
            [JsonProperty("token_type")]
            public string TokenType { get; private set; }
            [JsonProperty("expires_in")]
            public string ExpiresIn { get; private set; }
            [JsonProperty("id_token")]
            public string IdToken { get; private set; }

          //  public string RefreshToken { get; private set; }

            public static Token JsonToToken(string jSon)
            {
                return JsonConvert.DeserializeObject<Token>(jSon);
            }
        }

        public static string ClientId { get; set; }
        /// <summary>
        /// Esta direccion recibira la url para obtener el usuario con GetProfile
        /// </summary>
        public static string RedirectUriLocalhost { get; set; }
        public static string ClientSecret { get; set; }

        #region clase user login
        public GooglePlusUser() { }
        Bitmap picture = null;
        public string Id { get; private set; }

        public string Nombre { get; private set; }

        public string GivenName { get; private set; }

        public string FamilyName { get; private set; }

        public string Link { get; private set; }

        public string ImagenPerfilUri { get; private set; }
        public Bitmap ImagenPerfil
        {
            get
            {
                System.Net.WebClient client;
                if (picture == null)
                {
                    //la descargo
                    client = new System.Net.WebClient();
                    picture = (Bitmap)Bitmap.FromStream(new MemoryStream(client.DownloadData(ImagenPerfilUri)));
                }
                return picture;
            }
        }
        public string Gender { get; private set; }

        public string Locale { get; private set; }

        #endregion
        #region Get User login
        public static GooglePlusUser GetProfile(Gabriel.Cat.Xarxa.ClienteServidorHttpSeguro cliente)
        {
            return GetProfile(cliente.Client);
        }
        public static GooglePlusUser GetProfile(System.Net.HttpListenerContext context) {
          //  return GetProfile(context.Request.Url.Fragment);
            return GetProfile(context.Request.QueryString["code"]);
        }
        /// <summary>
        /// Devuelve un perfil google plus con los datos de él
        /// </summary>
        /// <param name="code">es la url resultado que viene del login de google</param>
        /// <returns>null if acces is denied</returns>
        public static async Task<GooglePlusUser> GetProfileAsync(string code)
        {
            GooglePlusUser profile = null;
            Token token;
            //consigo token valido
            token = await GetAccessToken(code);
            //uso el token para obtener los datos del usuario
            profile = await GetUserInfo(token);
            return profile;
        }

        /// <summary>
        /// Devuelve un perfil google plus con los datos de él
        /// </summary>
        /// <param name="code">es la url resultado que viene del login de google</param>
        /// <returns>null if acces is denied</returns>
        public static GooglePlusUser GetProfile(string code)
        {
            Task<GooglePlusUser> tskUsuarioGoogle = GetProfileAsync(code);
            tskUsuarioGoogle.Wait();
            return tskUsuarioGoogle.Result;
        }
        #region Obtener Token valido
        static async Task<Token> GetAccessToken(string code)
        {
            if (String.IsNullOrEmpty(code))
                throw new ArgumentException("the code is required to recibe token!");

            StringContent contentToken = new StringContent("code=" + code + "&client_id=" + ClientId + "&client_secret=" + ClientSecret + "&redirect_uri=" + RedirectUriLocalhost + "&grant_type=authorization_code");
            HttpClient client = new HttpClient();
            HttpResponseMessage response;
            string jSonToken;

            contentToken.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");//mirar que sea esta string
            response = await client.PostAsync("https://accounts.google.com/o/oauth2/token", contentToken);//coger la url correta :D
            jSonToken = await response.Content.ReadAsStringAsync(); // could also use ReadAsStreamAsync and avoid conversion to Stream

            return Token.JsonToToken(jSonToken);
        }


        #endregion
        #region obtener datos usuario con user
        static async Task<GooglePlusUser> GetUserInfo(Token token)
        {
            string query = "https://www.googleapis.com/oauth2/v1/userinfo?access_token=" + token.AccessToken;
            HttpClient client = new HttpClient();
            string userJson="";
            userJson = await client.GetStringAsync(query); // could also use GetStreamAsync and avoid conversion to Stream
            return GooglePlusUser.JsonToGooglePlusUser(userJson);
        }

        #endregion
        #endregion
        public  static GooglePlusUser JsonToGooglePlusUser(string jSon)
        {
            return JsonConvert.DeserializeObject<GooglePlusUser>(jSon);
        }
        /// <summary>
        /// Obtiene un html para poder hacer login a la app (se tiene que configurar antes)
        /// </summary>
        /// <returns></returns>
        public static string HtmlBasicLogin()
        {
            const string CLIENTID="#ClientId#";
            const string URLREDIRECT="#UrlRedirigir#";
            string htmlBasicoLogin = Resource.htmlPaginaBasicaLoginGooglePlus;
            htmlBasicoLogin = htmlBasicoLogin.Replace(CLIENTID, ClientId);
            htmlBasicoLogin = htmlBasicoLogin.Replace(URLREDIRECT,  RedirectUriLocalhost );
            return htmlBasicoLogin;
        }

    }

}
