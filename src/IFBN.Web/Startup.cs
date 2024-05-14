// <copyright file="Startup.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Web
{
	using IFBN.Data;
	using IFBN.Data.Entities;
    using IFBN.Web.Core;
    using Microsoft.AspNetCore.Authentication.Cookies;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Logging;
    using Microsoft.IdentityModel.Protocols;
	using Microsoft.IdentityModel.Protocols.OpenIdConnect;
	using Microsoft.IdentityModel.Tokens;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
	using System.Net.Http;
	using WebApiContrib.Core.Formatter.Csv;

	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
			BuildAppSettingsProvider();
		}

		public IConfiguration Configuration { get; }

		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseHsts();
			}

			app.UseRouting();
			app.UseAuthentication();
			app.UseAuthorization();

			app.UseHttpsRedirection();
			app.UseStaticFiles(new StaticFileOptions() { ServeUnknownFileTypes = true });
			app.UseCookiePolicy();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
			});
			//app.UseMvc(routes => { routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}"); });
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<CookiePolicyOptions>(options =>
			{
				// This lambda determines whether user consent for non-essential cookies is needed for a given request.
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});
			var configManager = new ConfigurationManager<OpenIdConnectConfiguration>($"https://identity.geo-wiki.org/.well-known/openid-configuration", new OpenIdConnectConfigurationRetriever());
			var openidconfig = configManager.GetConfigurationAsync().Result;
			services.AddAuthentication()
				.AddCookie()
				.AddFacebook(fb =>
				{
					fb.AppId = "xxx"; 

					fb.AppSecret = "xxx";
					
					fb.Scope.Add("public_profile");
					fb.Scope.Add("email");
				})
				.AddGoogle(g =>
				{

					g.ClientId = "xxx.apps.googleusercontent.com";
					g.ClientSecret = "xxx";
					g.Scope.Add("email");
					g.Scope.Add("openid");
				}).AddOpenIdConnect("FusionAuth", "Fusion Auth", options =>
				{
					options.Authority = "https://identity.geo-wiki.org";
					options.ClientId = "xxx";

					options.ResponseType = "token id_token";
					options.CallbackPath = new PathString("/signin-fusionauth");

					options.Scope.Add("openid");
					options.TokenValidationParameters = new TokenValidationParameters
					{
						IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
						{
							var client = new HttpClient();
							var response = client.GetAsync("https://identity.geo-wiki.org/.well-known/jwks.json").Result;
							var responseString = response.Content.ReadAsStringAsync().Result;
							var keys = JsonConvert.DeserializeObject<JwkList>(responseString);

							return keys.Keys;
						},
						ValidIssuers = new List<string> { "https://identity.geo-wiki.org" },
						NameClaimType = "name",
					};
				});

			// Add framework services.
			services.AddDbContext<IFBNContext>(options =>
				options.UseNpgsql(Configuration.GetConnectionString("IFBNDatabase")).UseLazyLoadingProxies());

			services.AddDefaultIdentity<ApplicationUser>().AddEntityFrameworkStores<IFBNContext>().AddDefaultTokenProviders();

			// services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

			// for Import and Export CSV
			var csvFormatterOptions = new CsvFormatterOptions();
			csvFormatterOptions.CsvDelimiter = ";";

			services.AddMvc(options =>
			{
				options.InputFormatters.Add(new CsvInputFormatter(csvFormatterOptions));
				options.OutputFormatters.Add(new CsvOutputFormatter(csvFormatterOptions));

				options.FormatterMappings.SetMediaTypeMappingForFormat("csv", "text/csv");
			}).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

			services.AddMvc()
				.AddCsvSerializerFormatters();

			services.AddHttpContextAccessor();
		}

		private void BuildAppSettingsProvider()
		{
			AppSettingsProvider.DbConnectionString = Configuration.GetConnectionString("IFBNDatabase");
			AppSettingsProvider.IsDevelopment = Configuration["IsDev"];
		}
	}
}