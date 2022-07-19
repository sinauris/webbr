using System;
using System.Globalization;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Webbr.Extensions;
using Webbr.Hubs;
using Webbr.Jwt;
using Webbr.Tasks;

namespace Webbr
{
    public class Startup
    {
        private static readonly string EnvSignKey = Environment.GetEnvironmentVariable("WEBBR_SIGN_KEY", EnvironmentVariableTarget.Process);
        private readonly SymmetricSecurityKey _signKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(EnvSignKey));
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddTransient<IWebbrSsh, WebbrSsh>(); // SSH
            services.AddSingleton<IJwtFactory, JwtFactory>(); // JWT Токен
            services.AddSingleton<IWebbrLogger, WebbrLogger>(); // Логирование
            services.AddTransient<IWebbrDatabase, WebbrDatabase>(); // База данных MySQL

            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = "webbr_jwt";
                options.Audience = "http://localhost:5000/";
                options.SigningCredentials = new SigningCredentials(_signKey, SecurityAlgorithms.HmacSha256);
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.ClaimsIssuer = "webbr_jwt";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = "webbr_jwt",
                    ValidateAudience = true,
                    ValidAudience = "http://localhost:5000/",
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _signKey,
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                };
                options.SaveToken = true;

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/webbrhub")) context.Token = accessToken;
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthorization(options =>
            {
                // Роль: Guest
                // Входят все доступные роли пользователей
                options.AddPolicy(Jwt.Helpers.Constants.JwtPolicy.Guest, policy =>
                    policy.RequireClaim(Jwt.Helpers.Constants.JwtClaimIdentifiers.Rol,
                        Jwt.Helpers.Constants.JwtPolicy.Guest,
                        Jwt.Helpers.Constants.JwtPolicy.Basic,
                        Jwt.Helpers.Constants.JwtClaims.Oit,
                        Jwt.Helpers.Constants.JwtClaims.Oaod,
                        Jwt.Helpers.Constants.JwtClaims.Otp,
                        Jwt.Helpers.Constants.JwtClaims.Mc,
                        Jwt.Helpers.Constants.JwtClaims.Rg,
                        Jwt.Helpers.Constants.JwtClaims.Admin));
                
                // Роль: Basic
                // Входят все доступные роли пользователей, кроме Guest
                options.AddPolicy(Jwt.Helpers.Constants.JwtPolicy.Basic, policy =>
                    policy.RequireClaim(Jwt.Helpers.Constants.JwtClaimIdentifiers.Rol,
                        Jwt.Helpers.Constants.JwtClaims.Oit,
                        Jwt.Helpers.Constants.JwtClaims.Oaod,
                        Jwt.Helpers.Constants.JwtClaims.Otp,
                        Jwt.Helpers.Constants.JwtClaims.Mc,
                        Jwt.Helpers.Constants.JwtClaims.Rg,
                        Jwt.Helpers.Constants.JwtClaims.Admin));

                // Роль: OtpOnly
                // Входят ОТП и роли выше, исключая ОИТ и ОАОД
                options.AddPolicy(Jwt.Helpers.Constants.JwtPolicy.OtpOnly, policy =>
                    policy.RequireClaim(Jwt.Helpers.Constants.JwtClaimIdentifiers.Rol,
                        Jwt.Helpers.Constants.JwtClaims.Otp,
                        Jwt.Helpers.Constants.JwtClaims.Mc,
                        Jwt.Helpers.Constants.JwtClaims.Rg,
                        Jwt.Helpers.Constants.JwtClaims.Admin));

                // Роль: Mc
                // Входят MC и роли выше, исключая ОТП, ОИТ и ОАОД
                options.AddPolicy(Jwt.Helpers.Constants.JwtPolicy.Mc, policy =>
                    policy.RequireClaim(Jwt.Helpers.Constants.JwtClaimIdentifiers.Rol,
                        Jwt.Helpers.Constants.JwtClaims.Mc,
                        Jwt.Helpers.Constants.JwtClaims.Rg,
                        Jwt.Helpers.Constants.JwtClaims.Admin));

                // Роль: Rg
                // Входят RG и роли выше, исключая MC, ОТП, ОИТ и ОАОД
                options.AddPolicy(Jwt.Helpers.Constants.JwtPolicy.Rg, policy =>
                    policy.RequireClaim(Jwt.Helpers.Constants.JwtClaimIdentifiers.Rol,
                        Jwt.Helpers.Constants.JwtClaims.Rg,
                        Jwt.Helpers.Constants.JwtClaims.Admin));

                // Роль: Admin
                options.AddPolicy(Jwt.Helpers.Constants.JwtPolicy.Admin, policy =>
                    policy.RequireClaim(Jwt.Helpers.Constants.JwtClaimIdentifiers.Rol,
                        Jwt.Helpers.Constants.JwtClaims.Admin));
            });
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddResponseCaching();
            services.AddResponseCompression().Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
            
            // SignalR
            services.AddSignalR();
            
            // Задания
            services.AddTask<GrafanaTask>(o => { o.AutoStart(TimeSpan.FromSeconds(5)); o.RunCulture = new CultureInfo("en-US"); });
            services.AddTask<ZabbixTask>(o => o.AutoStart(TimeSpan.FromSeconds(5)));
            services.AddTask<UpsTask>(o => o.AutoStart(TimeSpan.FromSeconds(5)));
            services.AddTask<NaumenLicenseTask>(o => o.AutoStart(TimeSpan.FromSeconds(5)));
            services.AddTask<MtsTunnelTask>(o => o.AutoStart(TimeSpan.FromSeconds(5)));
            services.AddTask<MtsChannelTask>(o => o.AutoStart(TimeSpan.FromSeconds(5)));
            services.AddTask<MtsMgwAgentTask>(o => o.AutoStart(TimeSpan.FromSeconds(5)));
            services.AddTask<MtsMgwJobsTask>(o => o.AutoStart(TimeSpan.FromSeconds(5)));
            services.AddTask<MtsImportTask>(o => o.AutoStart(TimeSpan.FromSeconds(5)));
            services.AddTask<PortmapTask>(o => o.AutoStart(TimeSpan.FromSeconds(60)));
            services.AddTask<DomainTask>(o => o.AutoStart(TimeSpan.FromMinutes(1)));
            services.AddTask<ServerTask>(o => o.AutoStart(TimeSpan.FromMinutes(1)));
            services.AddTask<NaumenTask>(o => o.AutoStart(TimeSpan.FromMinutes(5)));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment()) app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions { HotModuleReplacement = true });

            // Кэширование
            app.UseResponseCaching();
            
            // Сжатие
            app.UseResponseCompression();

            // JWT
            app.UseAuthentication();
            app.UseStaticFiles();

            // SignalR
            app.UseSignalR(r => r.MapHub<WebbrHub>("/webbrhub"));

            // MVC
            app.UseMvcWithDefaultRoute();
            app.MapWhen(m => !m.Request.Path.Value.StartsWith("/api"), r => r.UseMvc(routes =>
            {
                routes.MapSpaFallbackRoute("spa-fallback", new {controller = "Home", action = "Index"});
            }));
        }
    }
}