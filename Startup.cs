using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SimpleAPI.Controllers;
using SimpleAPI.Models;



namespace SimpleAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //get jwtsetting from the appsettings.json file
            services.Configure<JWTSettings>(Configuration.GetSection("JWTSettings"));

            services.AddDbContext<SecurityContext>(options =>
                        options.UseSqlServer(Configuration.GetConnectionString("SecurityConnection")));

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                    .AddEntityFrameworkStores<SecurityContext>()
                    .AddDefaultTokenProviders();

        var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("JWTSettings:SecretKey").Value));
            
        var tokenValidationParameters = new TokenValidationParameters
        {
            // The signing key must match!
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            // Validate the JWT Issuer (iss) claim
            ValidateIssuer = true,
            ValidIssuer = Configuration.GetSection("JWTSettings:Issuer").Value,
            // Validate the JWT Audience (aud) claim
            ValidateAudience = true,
            ValidAudience = Configuration.GetSection("JWTSettings:Audience").Value,

            RequireExpirationTime = true,


            // Validate the token expiry
            ValidateLifetime = true,
            LifetimeValidator = CustomLifetimeValidator,
            // If you want to allow a certain amount of clock drift, set that here:
            ClockSkew = TimeSpan.Zero
        };
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => {
                options.RequireHttpsMetadata = false; //add this one because fiddler was bitching
                options.TokenValidationParameters = tokenValidationParameters; 
        });
        services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build();
        });
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowAnyOrigin()
                .AllowCredentials());
        }); 
        

        services.AddMvc();

        }
        private bool CustomLifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken tokenToValidate, TokenValidationParameters @param)
        {
            if (expires != null)
            {
                return expires > DateTime.UtcNow;
            }
            return false;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseStaticFiles();
            app.UseAuthentication();



            app.UseMvc();

        }

    }

}
