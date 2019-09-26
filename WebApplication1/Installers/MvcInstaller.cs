﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Post_Surfer.Options;
using Post_Surfer.Services;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Text;

namespace Post_Surfer.Installers
{
    public class MvcInstaller: Installer
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
           
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddApiVersioning(o => o.ApiVersionReader = new HeaderApiVersionReader("api-version"));

            #region Authentication   

            JwtSettings jwtSettings = new JwtSettings();
            configuration.Bind(nameof(JwtSettings), jwtSettings);

            services.AddSingleton(jwtSettings);

            services.AddScoped<IIdentityService, IdentityService>();

            var tokenValidationParametres= new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key: Encoding.ASCII.GetBytes(jwtSettings.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = true
            };

            services.AddSingleton(tokenValidationParametres);

            services.AddAuthentication(configureOptions: x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; 
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.SaveToken = true;
                x.TokenValidationParameters = tokenValidationParametres;
            });

            #endregion

            #region Swagger

            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new Info { Version = "v1", Title = "Post Surfer" });
                var security = new Dictionary<string, IEnumerable<String>>
                {
                    { "Bearer", new string[0]}
                };

                x.AddSecurityDefinition(name: "Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the bearer scheme",
                    Name = "Authorization",
                    In = "Header",
                    Type = "apiKey"
                }
                );
        
                x.AddSecurityRequirement(security);

            });
            
            #endregion


            services.AddApiVersioning(o =>
            {
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(new DateTime(2016, 7, 1));
            });
        }
    }
}
