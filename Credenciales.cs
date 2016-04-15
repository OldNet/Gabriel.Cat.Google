/*
 * Creado por SharpDevelop.
 * Usuario: Pingu
 * Fecha: 15/04/2016
 * Hora: 17:12
 * 
 * Para cambiar esta plantilla use Herramientas | Opciones | Codificación | Editar Encabezados Estándar
 */
using System;
using Newtonsoft.Json;

namespace Gabriel.Cat.Google
{
	/// <summary>
	/// Description of Credenciales.
	/// </summary>
	public class Credenciales
	{
		Web web;
		[JsonProperty("web")]
		public Web Web
		{ 
			get{return web;}
			set{web=value;}
		}
	}
	public class Web
	{
		string clientId;
		string projectId;
		string authUri;
		string tokenUri;
		string authProviderX509Cert;
		string clientSecret;
		string[] redirectUris;
		string[] javaScriptOrigins;
		
		[JsonProperty("client_id")]
		public string ClientId
		{
			get{return clientId;}
			set{clientId=value;}
		}
		[JsonProperty("project_id")]
		public string ProjectId
		{
			get{return projectId;}
			set{projectId=value;}
		}
		[JsonProperty("autho_uri")]
		public string AuthoUri
		{
			get{return authUri;}
			set{authUri=value;}
		}
				[JsonProperty("token_uri")]
		public string TokenUri
		{
			get{return tokenUri;}
			set{tokenUri=value;}
		}
		[JsonProperty("auth_provider_x509_cert")]
		public string AuthProviderX509Cert
		{
			get{return authProviderX509Cert;}
			set{authProviderX509Cert=value;}
		}
		[JsonProperty("client_secret")]
		public string ClientSecret
		{
			get{return clientSecret;}
			set{clientSecret=value;}
		}
		[JsonProperty("redirect_uris")]
		public string[] RedirectUris
		{
			get{return redirectUris;}
			set{redirectUris=value;}
		}
		[JsonProperty("javascript_origins")]
		public string[] JavaScritOrigins
		{
			get{return javaScriptOrigins;}
			set{javaScriptOrigins=value;}
		}
		
	}
}
