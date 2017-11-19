﻿using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Swastika.Identity.Data;
using Swastika.Identity.Models;
using Swastika.Identity.Services;
using Swastika.IO.Cms.Lib;
using Swastika.IO.Identity.Identity.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swastika.IO.Admin
{
    public partial class Startup
    {
        public void ConfigureApiServices(IServiceCollection services)
        {
            services.AddCors(o =>
            {
                o.AddPolicy("default", policy =>
                {
                    policy.AllowAnyOrigin();
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                    policy.WithExposedHeaders("WWW-Authenticate");
                });
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AddEditUser", policy =>
                {
                    policy.RequireClaim("Add User", "Add User");
                    policy.RequireClaim("Edit User", "Edit User");
                });
                options.AddPolicy("DeleteUser", policy => policy.RequireClaim("Delete User", "Delete User"));
            });
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureAuth(IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = SWCmsConstants.AuthConfiguration.ConnectionString;
            int authCookieExpiration = SWCmsConstants.AuthConfiguration.AuthCookieExpiration;
            string authCookieLoginPath = SWCmsConstants.AuthConfiguration.AuthCookieLoginPath;
            string authCookieLogoutPath = SWCmsConstants.AuthConfiguration.AuthCookieLogoutPath;
            string authCookieAccessDeniedPath = SWCmsConstants.AuthConfiguration.AuthCookieAccessDeniedPath;

            //string apiEndPoint = SWCmsConstants.AuthConfiguration.ApiEndPoint;

            //string fbId = SWCmsConstants.AuthConfiguration.FacebookId;
            //string fbSecret = SWCmsConstants.AuthConfiguration.FacebookSecret;

            //string ggId = SWCmsConstants.AuthConfiguration.GoogleId;
            //string ggSecret = SWCmsConstants.AuthConfiguration.GoogleSecret;

            //string msId = SWCmsConstants.AuthConfiguration.MicrosoftId;
            //string msSecret = SWCmsConstants.AuthConfiguration.MicrosoftSecret;

            //string twConsumerKey = SWCmsConstants.AuthConfiguration.TwitterKey;
            //string twConsumerSecret = SWCmsConstants.AuthConfiguration.TwitterSecret;

            //string openIdAuthority = SWCmsConstants.AuthConfiguration.OpenIdAuthority;
            //string openIdClientId = SWCmsConstants.AuthConfiguration.OpenIdClientId;

            //string authTokenIssuer = SWCmsConstants.AuthConfiguration.AuthTokenIssuer;

            services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 6;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.Cookie.Expiration = TimeSpan.FromDays(authCookieExpiration);
                options.LoginPath = authCookieLoginPath; // If the LoginPath is not set here, ASP.NET Core will default to /Account/Login
                options.LogoutPath = authCookieLogoutPath; // If the LogoutPath is not set here, ASP.NET Core will default to /Account/Logout
                options.AccessDeniedPath = authCookieAccessDeniedPath; // If the AccessDeniedPath is not set here, ASP.NET Core will default to /Account/AccessDenied
                options.SlidingExpiration = true;
            });
           
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;                    
                    cfg.TokenValidationParameters = new TokenValidationParameters()
                    {
                        IssuerSigningKey = SWCmsConstants.AuthConfiguration.AuthTokenKey,
                        ValidAudience = SWCmsConstants.AuthConfiguration.Audience,
                        ValidIssuer = SWCmsConstants.AuthConfiguration.AuthTokenIssuer,
                        // When receiving a token, check that we've signed it.
                        ValidateIssuerSigningKey = true,
                        // When receiving a token, check that it is still valid.
                        ValidateLifetime = true,                       
                        // This defines the maximum allowable clock skew - i.e. provides a tolerance on the token expiry time 
                        // when validating the lifetime. As we're creating the tokens locally and validating them on the same 
                        // machines which should have synchronised time, this can be set to zero. and default value will be 5minutes
                        ClockSkew = TimeSpan.FromMinutes(0)
                    };
                })
                //.AddCookie()
               // Auth with OpenId
               //.AddOpenIdConnect(options =>
               //{
               //    options.Authority = openIdAuthority;
               //    options.ClientId = openIdClientId;
               //})
               // Auth with Facebook
               //.AddFacebook(options =>
               //{
               //    options.AppId = fbId;
               //    options.AppSecret = fbSecret;
               //})
               // Auth with Google
               //.AddGoogle(options =>
               //{
               //    options.ClientId = ggId;
               //    options.ClientSecret = ggSecret;
               //})
               // Auth with Microsoft
               //.AddMicrosoftAccount(options =>
               //{
               //    options.ClientId = msId;
               //    options.ClientSecret = msSecret;
               //})
               // Auth with Twitter
               //.AddTwitter(options =>
               //{
               //    options.ConsumerKey = twConsumerKey;
               //    options.ConsumerSecret = twConsumerSecret;
               //})
               ;

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

        }
    }
}
