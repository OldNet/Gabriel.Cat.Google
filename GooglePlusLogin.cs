using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
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
            private Token() { }
            public string AccessToken { get; private set; }

            public string TokenType { get; private set; }

            public string ExpiresIn { get; private set; }

            public string IdToken { get; private set; }

            public string RefreshToken { get; private set; }

            public static Token JsonToToken(string jSon)
            {
                System.Xml.XmlDictionaryReader diccionario=  JsonReaderWriterFactory.CreateJsonReader(new MemoryStream(System.Text.ASCIIEncoding.Unicode.GetBytes(jSon)), new System.Xml.XmlDictionaryReaderQuotas());
                Token token = new Token();
                token.AccessToken = diccionario["access_token"];
                token.ExpiresIn = diccionario["expires_in"];
                token.IdToken = diccionario["id_token"];
                token.RefreshToken = diccionario["refresh_token"];
                token.TokenType = diccionario["token_type"];
                return token;
            }
        }

        public static string ClientId { get; set; }
        /// <summary>
        /// Esta direccion recibira la url para obtener el usuario con GetProfile
        /// </summary>
        public static string RedirectUriLocalhost { get; set; }
        public static string ClientSecret { get; set; }

        #region clase user login
        private GooglePlusUser() { }
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
        /// <summary>
        /// Devuelve un perfil google plus con los datos de él
        /// </summary>
        /// <param name="returnUrl">es la url resultado que viene del login de google</param>
        /// <returns>null if acces is denied</returns>
        public static async Task<GooglePlusUser> GetProfileAsync(Uri returnUrl)
        {
            GooglePlusUser profile = null;
            Token token;
            //consigo token valido
            token = await GetAccessToken(returnUrl.GetHttpValueArgument("token"));
            //uso el token para obtener los datos del usuario
            profile = await GetUserInfo(token);
            return profile;
        }

        /// <summary>
        /// Devuelve un perfil google plus con los datos de él
        /// </summary>
        /// <param name="returnUrl">es la url resultado que viene del login de google</param>
        /// <returns>null if acces is denied</returns>
        public static GooglePlusUser GetProfile(Uri returnUrl)
        {
            Task<GooglePlusUser> tskUsuarioGoogle = GetProfileAsync(returnUrl);
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
            response = await client.PostAsync("https://accounts.google.com/o/oauth2/user", contentToken);
            jSonToken = await response.Content.ReadAsStringAsync(); // could also use ReadAsStreamAsync and avoid conversion to Stream

            return Token.JsonToToken(jSonToken);
        }


        #endregion
        #region obtener datos usuario con user
        static async Task<GooglePlusUser> GetUserInfo(Token token)
        {
            string query = "https://www.googleapis.com/oauth2/v1/userinfo?access_token=" + token.AccessToken;
            HttpClient client = new HttpClient();
            string userJson = await client.GetStringAsync(query); // could also use GetStreamAsync and avoid conversion to Stream

            return GooglePlusUser.JsonToGooglePlusUser(userJson);
        }

        #endregion
        #endregion
        public  static GooglePlusUser JsonToGooglePlusUser(string jSon)
        {
            System.Xml.XmlDictionaryReader diccionario = JsonReaderWriterFactory.CreateJsonReader(new MemoryStream(System.Text.ASCIIEncoding.Unicode.GetBytes(jSon)), new System.Xml.XmlDictionaryReaderQuotas());
            GooglePlusUser user = new GooglePlusUser();
            user.Id = diccionario["id"];
            user.Nombre = diccionario["name"];
            user.GivenName = diccionario["given_name"];
            user.FamilyName = diccionario["family_name"];
            user.Link = diccionario["link"];
            user.ImagenPerfilUri = diccionario["picture_uri"];
            user.Gender = diccionario["gender"];
            user.Locale = diccionario["locale"];
            return user;
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
